using EcmaScript.NET.Collections;
using EcmaScript.NET.Debugging;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public abstract class ScriptableObject : IScriptable, DebuggableObject
	{
		private class Slot : ICloneable
		{
			internal int intKey;
			internal string stringKey;
			internal object value;
			internal short attributes;
			internal sbyte wasDeleted;
			internal ICallable getter;
			internal ICallable setter;
			public object Clone()
			{
				return new ScriptableObject.Slot
				{
					intKey = this.intKey,
					stringKey = this.stringKey,
					value = this.value,
					attributes = this.attributes,
					wasDeleted = this.wasDeleted,
					getter = this.getter,
					setter = this.setter
				};
			}
			internal object GetValue(Context cx, IScriptable scope, IScriptable thisObj)
			{
				object result;
				if (this.getter == null)
				{
					result = this.value;
				}
				else
				{
					if (cx == null)
					{
						cx = Context.CurrentContext;
					}
					result = this.getter.Call(cx, scope, thisObj, ScriptRuntime.EmptyArgs);
				}
				return result;
			}
			internal object SetValue(Context cx, IScriptable scope, IScriptable thisObj, object value)
			{
				object result;
				if (this.setter == null)
				{
					if (this.getter != null)
					{
						throw ScriptRuntime.TypeError("setting a property that has only a getter");
					}
					this.value = value;
					result = value;
				}
				else
				{
					if (cx == null)
					{
						cx = Context.CurrentContext;
					}
					result = this.setter.Call(cx, scope, thisObj, new object[]
					{
						value
					});
				}
				return result;
			}
		}
		public const int EMPTY = 0;
		public const int READONLY = 1;
		public const int DONTENUM = 2;
		public const int PERMANENT = 4;
		private IScriptable prototypeObject;
		private IScriptable parentScopeObject;
		private static readonly object HAS_STATIC_ACCESSORS = typeof(void);
		private static readonly ScriptableObject.Slot REMOVED = new ScriptableObject.Slot();
		private ScriptableObject.Slot[] slots;
		private int count;
		private ScriptableObject.Slot lastAccess = ScriptableObject.REMOVED;
		private volatile Hashtable associatedValues;
		public abstract string ClassName
		{
			get;
		}
		public IScriptable ParentScope
		{
			get
			{
				return this.parentScopeObject;
			}
			set
			{
				this.parentScopeObject = value;
			}
		}
		public virtual object[] AllIds
		{
			get
			{
				return this.GetIds(true);
			}
		}
		public bool Sealed
		{
			get
			{
				return this.count < 0;
			}
		}
		internal static void CheckValidAttributes(int attributes)
		{
			if ((attributes & -8) != 0)
			{
				throw new ArgumentException(Convert.ToString(attributes));
			}
		}
		public ScriptableObject()
		{
		}
		public ScriptableObject(IScriptable scope, IScriptable prototype)
		{
			if (scope == null)
			{
				throw new ArgumentException();
			}
			this.parentScopeObject = scope;
			this.prototypeObject = prototype;
		}
		public virtual bool Has(string name, IScriptable start)
		{
			return null != this.GetNamedSlot(name);
		}
		public virtual bool Has(int index, IScriptable start)
		{
			return null != this.GetSlot(null, index);
		}
		public virtual object Get(string name, IScriptable start)
		{
			ScriptableObject.Slot namedSlot = this.GetNamedSlot(name);
			object result;
			if (namedSlot == null)
			{
				result = UniqueTag.NotFound;
			}
			else
			{
				result = namedSlot.GetValue(null, start, start);
			}
			return result;
		}
		public virtual object Get(int index, IScriptable start)
		{
			ScriptableObject.Slot slot = this.GetSlot(null, index);
			object result;
			if (slot == null)
			{
				result = UniqueTag.NotFound;
			}
			else
			{
				result = slot.GetValue(null, start, start);
			}
			return result;
		}
		public virtual object Put(string name, IScriptable start, object value)
		{
			ScriptableObject.Slot slot = this.lastAccess;
			object result;
			if (name != slot.stringKey || slot.wasDeleted != 0)
			{
				int hashCode = name.GetHashCode();
				slot = this.GetSlot(name, hashCode);
				if (slot == null)
				{
					if (start != this)
					{
						start.Put(name, start, value);
						result = value;
						return result;
					}
					slot = this.AddSlot(name, hashCode, null);
				}
			}
			if (start == this && this.Sealed)
			{
				throw Context.ReportRuntimeErrorById("msg.modify.sealed", new object[]
				{
					name
				});
			}
			if ((slot.attributes & 1) != 0)
			{
				Context currentContext = Context.CurrentContext;
				if (currentContext.Version == Context.Versions.JS1_2)
				{
					throw Context.ReportRuntimeErrorById("msg.read-only", new object[]
					{
						name
					});
				}
				if (currentContext.HasFeature(Context.Features.Strict))
				{
					Context.ReportWarningById("msg.read-only", new string[]
					{
						name
					});
				}
				result = value;
			}
			else
			{
				if (this == start)
				{
					result = slot.SetValue(null, start, start, value);
				}
				else
				{
					if (slot.setter != null)
					{
						ScriptableObject.Slot slot2 = (ScriptableObject.Slot)slot.Clone();
						((ScriptableObject)start).AddSlotImpl(slot2.stringKey, slot2.intKey, slot2);
						result = slot2.SetValue(null, start, start, value);
					}
					else
					{
						result = start.Put(name, start, value);
					}
				}
			}
			return result;
		}
		public virtual object Put(int index, IScriptable start, object value)
		{
			ScriptableObject.Slot slot = this.GetSlot(null, index);
			object result;
			if (slot == null)
			{
				if (start != this)
				{
					result = start.Put(index, start, value);
					return result;
				}
				slot = this.AddSlot(null, index, null);
			}
			if (start == this && this.Sealed)
			{
				throw Context.ReportRuntimeErrorById("msg.modify.sealed", new object[]
				{
					Convert.ToString(index)
				});
			}
			if ((slot.attributes & 1) != 0)
			{
				result = slot.GetValue(null, start, start);
			}
			else
			{
				if (this == start)
				{
					result = slot.SetValue(null, start, start, value);
				}
				else
				{
					result = start.Put(index, start, value);
				}
			}
			return result;
		}
		public virtual void Delete(string name)
		{
			this.RemoveSlot(name, name.GetHashCode());
		}
		public virtual void Delete(int index)
		{
			this.RemoveSlot(null, index);
		}
		public virtual int GetAttributes(string name)
		{
			ScriptableObject.Slot namedSlot = this.GetNamedSlot(name);
			if (namedSlot == null)
			{
				throw Context.ReportRuntimeErrorById("msg.prop.not.found", new object[]
				{
					name
				});
			}
			return (int)namedSlot.attributes;
		}
		public virtual int GetAttributes(int index)
		{
			ScriptableObject.Slot slot = this.GetSlot(null, index);
			if (slot == null)
			{
				throw Context.ReportRuntimeErrorById("msg.prop.not.found", new object[]
				{
					Convert.ToString(index)
				});
			}
			return (int)slot.attributes;
		}
		public virtual void SetAttributes(string name, int attributes)
		{
			ScriptableObject.CheckValidAttributes(attributes);
			ScriptableObject.Slot namedSlot = this.GetNamedSlot(name);
			if (namedSlot == null)
			{
				throw Context.ReportRuntimeErrorById("msg.prop.not.found", new object[]
				{
					name
				});
			}
			namedSlot.attributes = (short)attributes;
		}
		public virtual void SetAttributes(int index, int attributes)
		{
			ScriptableObject.CheckValidAttributes(attributes);
			ScriptableObject.Slot slot = this.GetSlot(null, index);
			if (slot == null)
			{
				throw Context.ReportRuntimeErrorById("msg.prop.not.found", new object[]
				{
					Convert.ToString(index)
				});
			}
			slot.attributes = (short)attributes;
		}
		public virtual IScriptable GetPrototype()
		{
			return this.prototypeObject;
		}
		public virtual void SetPrototype(IScriptable m)
		{
			this.prototypeObject = m;
		}
		public virtual object[] GetIds()
		{
			return this.GetIds(false);
		}
		public virtual object GetDefaultValue(Type typeHint)
		{
			Context context = null;
			for (int i = 0; i < 2; i++)
			{
				bool flag;
				if (typeHint == typeof(string))
				{
					flag = (i == 0);
				}
				else
				{
					flag = (i == 1);
				}
				string name;
				object[] array;
				if (flag)
				{
					name = "toString";
					array = ScriptRuntime.EmptyArgs;
				}
				else
				{
					name = "valueOf";
					array = new object[1];
					string text;
					if (typeHint == null)
					{
						text = "undefined";
					}
					else
					{
						if (typeHint == typeof(string))
						{
							text = "string";
						}
						else
						{
							if (typeHint == typeof(IScriptable))
							{
								text = "object";
							}
							else
							{
								if (typeHint == typeof(IFunction))
								{
									text = "function";
								}
								else
								{
									if (typeHint == typeof(bool) || typeHint == typeof(bool))
									{
										text = "boolean";
									}
									else
									{
										if (!CliHelper.IsNumberType(typeHint) && typeHint != typeof(byte) && typeHint != typeof(sbyte))
										{
											throw Context.ReportRuntimeErrorById("msg.invalid.type", new object[]
											{
												typeHint.ToString()
											});
										}
										text = "number";
									}
								}
							}
						}
					}
					array[0] = text;
				}
				object obj = ScriptableObject.GetProperty(this, name);
				if (obj is IFunction)
				{
					IFunction function = (IFunction)obj;
					if (context == null)
					{
						context = Context.CurrentContext;
					}
					obj = function.Call(context, function.ParentScope, this, array);
					if (obj != null)
					{
						object result;
						if (!(obj is IScriptable))
						{
							result = obj;
						}
						else
						{
							if (typeHint != typeof(IScriptable) && typeHint != typeof(IFunction))
							{
								if (flag && obj is Wrapper)
								{
									object obj2 = ((Wrapper)obj).Unwrap();
									if (obj2 is string)
									{
										result = obj2;
										return result;
									}
								}
								goto IL_296;
							}
							result = obj;
						}
						return result;
					}
					IL_296:;
				}
			}
			string text2 = (typeHint == null) ? "undefined" : typeHint.FullName;
			throw ScriptRuntime.TypeErrorById("msg.default.value", new string[]
			{
				text2
			});
		}
		public virtual bool HasInstance(IScriptable instance)
		{
			throw ScriptRuntime.TypeError("msg.bad.instanceof.rhs");
		}
		protected internal virtual object EquivalentValues(object value)
		{
			return (this == value) ? true : UniqueTag.NotFound;
		}
		public virtual void DefineProperty(string propertyName, object value, int attributes)
		{
			this.Put(propertyName, this, value);
			this.SetAttributes(propertyName, attributes);
		}
		public static void DefineProperty(IScriptable destination, string propertyName, object value, int attributes)
		{
			if (!(destination is ScriptableObject))
			{
				destination.Put(propertyName, destination, value);
			}
			else
			{
				ScriptableObject scriptableObject = (ScriptableObject)destination;
				scriptableObject.DefineProperty(propertyName, value, attributes);
			}
		}
		public static IScriptable GetObjectPrototype(IScriptable scope)
		{
			return ScriptableObject.getClassPrototype(scope, "Object");
		}
		public static IScriptable GetFunctionPrototype(IScriptable scope)
		{
			return ScriptableObject.getClassPrototype(scope, "Function");
		}
		public static IScriptable getClassPrototype(IScriptable scope, string className)
		{
			scope = ScriptableObject.GetTopLevelScope(scope);
			object property = ScriptableObject.GetProperty(scope, className);
			object obj;
			IScriptable result;
			if (property is BaseFunction)
			{
				obj = ((BaseFunction)property).PrototypeProperty;
			}
			else
			{
				if (!(property is IScriptable))
				{
					result = null;
					return result;
				}
				IScriptable scriptable = (IScriptable)property;
				obj = scriptable.Get("prototype", scriptable);
			}
			if (obj is IScriptable)
			{
				result = (IScriptable)obj;
			}
			else
			{
				result = null;
			}
			return result;
		}
		public static IScriptable GetTopLevelScope(IScriptable obj)
		{
			while (true)
			{
				IScriptable parentScope = obj.ParentScope;
				if (parentScope == null)
				{
					break;
				}
				obj = parentScope;
			}
			return obj;
		}
		public virtual void SealObject()
		{
			Monitor.Enter(this);
			try
			{
				if (this.count >= 0)
				{
					this.count = -1 - this.count;
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
		}
		public static object GetProperty(IScriptable obj, string name)
		{
			IScriptable start = obj;
			object obj2;
			do
			{
				obj2 = obj.Get(name, start);
				if (obj2 != UniqueTag.NotFound)
				{
					break;
				}
				obj = obj.GetPrototype();
			}
			while (obj != null);
			return obj2;
		}
		public static object GetProperty(IScriptable obj, int index)
		{
			IScriptable start = obj;
			object obj2;
			do
			{
				obj2 = obj.Get(index, start);
				if (obj2 != UniqueTag.NotFound)
				{
					break;
				}
				obj = obj.GetPrototype();
			}
			while (obj != null);
			return obj2;
		}
		public static bool HasProperty(IScriptable obj, string name)
		{
			return null != ScriptableObject.GetBase(obj, name);
		}
		public static bool HasProperty(IScriptable obj, int index)
		{
			return null != ScriptableObject.GetBase(obj, index);
		}
		public static object PutProperty(IScriptable obj, string name, object value)
		{
			IScriptable scriptable = ScriptableObject.GetBase(obj, name);
			if (scriptable == null)
			{
				scriptable = obj;
			}
			return scriptable.Put(name, obj, value);
		}
		public static object PutProperty(IScriptable obj, int index, object value)
		{
			IScriptable scriptable = ScriptableObject.GetBase(obj, index);
			if (scriptable == null)
			{
				scriptable = obj;
			}
			return scriptable.Put(index, obj, value);
		}
		public static bool DeleteProperty(IScriptable obj, string name)
		{
			IScriptable @base = ScriptableObject.GetBase(obj, name);
			bool result;
			if (@base == null)
			{
				result = true;
			}
			else
			{
				@base.Delete(name);
				result = !@base.Has(name, obj);
			}
			return result;
		}
		public static bool DeleteProperty(IScriptable obj, int index)
		{
			IScriptable @base = ScriptableObject.GetBase(obj, index);
			bool result;
			if (@base == null)
			{
				result = true;
			}
			else
			{
				@base.Delete(index);
				result = !@base.Has(index, obj);
			}
			return result;
		}
		public static object[] GetPropertyIds(IScriptable obj)
		{
			object[] result;
			if (obj == null)
			{
				result = ScriptRuntime.EmptyArgs;
			}
			else
			{
				object[] array = obj.GetIds();
				ObjToIntMap objToIntMap = null;
				while (true)
				{
					obj = obj.GetPrototype();
					if (obj == null)
					{
						break;
					}
					object[] ids = obj.GetIds();
					if (ids.Length != 0)
					{
						if (objToIntMap == null)
						{
							if (array.Length == 0)
							{
								array = ids;
								continue;
							}
							objToIntMap = new ObjToIntMap(array.Length + ids.Length);
							for (int num = 0; num != array.Length; num++)
							{
								objToIntMap.intern(array[num]);
							}
							array = null;
						}
						for (int num = 0; num != ids.Length; num++)
						{
							objToIntMap.intern(ids[num]);
						}
					}
				}
				if (objToIntMap != null)
				{
					array = objToIntMap.getKeys();
				}
				result = array;
			}
			return result;
		}
		public static object CallMethod(IScriptable obj, string methodName, object[] args)
		{
			return ScriptableObject.CallMethod(null, obj, methodName, args);
		}
		public static object CallMethod(Context cx, IScriptable obj, string methodName, object[] args)
		{
			object property = ScriptableObject.GetProperty(obj, methodName);
			if (!(property is IFunction))
			{
				throw ScriptRuntime.NotFunctionError(obj, methodName);
			}
			IFunction function = (IFunction)property;
			IScriptable topLevelScope = ScriptableObject.GetTopLevelScope(obj);
			object result;
			if (cx != null)
			{
				result = function.Call(cx, topLevelScope, obj, args);
			}
			else
			{
				result = Context.Call(null, function, topLevelScope, obj, args);
			}
			return result;
		}
		private static IScriptable GetBase(IScriptable obj, string name)
		{
			while (!obj.Has(name, obj))
			{
				obj = obj.GetPrototype();
				if (obj == null)
				{
					return obj;
				}
			}
			return obj;
		}
		private static IScriptable GetBase(IScriptable obj, int index)
		{
			while (!obj.Has(index, obj))
			{
				obj = obj.GetPrototype();
				if (obj == null)
				{
					return obj;
				}
			}
			return obj;
		}
		public object GetAssociatedValue(object key)
		{
			Hashtable hashtable = this.associatedValues;
			object result;
			if (hashtable == null)
			{
				result = null;
			}
			else
			{
				result = hashtable[key];
			}
			return result;
		}
		public static object GetTopScopeValue(IScriptable scope, object key)
		{
			scope = ScriptableObject.GetTopLevelScope(scope);
			object associatedValue;
			while (true)
			{
				if (scope is ScriptableObject)
				{
					ScriptableObject scriptableObject = (ScriptableObject)scope;
					associatedValue = scriptableObject.GetAssociatedValue(key);
					if (associatedValue != null)
					{
						break;
					}
				}
				scope = scope.GetPrototype();
				if (scope == null)
				{
					goto Block_3;
				}
			}
			object result = associatedValue;
			return result;
			Block_3:
			result = null;
			return result;
		}
		public object AssociateValue(object key, object value)
		{
			if (value == null)
			{
				throw new ArgumentException();
			}
			Hashtable hashtable = this.associatedValues;
			if (hashtable == null)
			{
				Monitor.Enter(this);
				try
				{
					hashtable = this.associatedValues;
					if (hashtable == null)
					{
						hashtable = Hashtable.Synchronized(new Hashtable());
						this.associatedValues = hashtable;
					}
				}
				finally
				{
					Monitor.Exit(this);
				}
			}
			return this.InitHash(hashtable, key, value);
		}
		private object InitHash(Hashtable h, object key, object initialValue)
		{
			object syncRoot;
			Monitor.Enter(syncRoot = h.SyncRoot);
			try
			{
				object obj = h[key];
				if (obj == null)
				{
					h[key] = initialValue;
				}
				else
				{
					initialValue = obj;
				}
			}
			finally
			{
				Monitor.Exit(syncRoot);
			}
			return initialValue;
		}
		private ScriptableObject.Slot GetNamedSlot(string name)
		{
			ScriptableObject.Slot slot = this.lastAccess;
			ScriptableObject.Slot result;
			if (name == slot.stringKey && slot.wasDeleted == 0)
			{
				result = slot;
			}
			else
			{
				int hashCode = name.GetHashCode();
				ScriptableObject.Slot[] array = this.slots;
				int slotPosition = ScriptableObject.GetSlotPosition(array, name, hashCode);
				if (slotPosition < 0)
				{
					result = null;
				}
				else
				{
					slot = array[slotPosition];
					slot.stringKey = name;
					this.lastAccess = slot;
					result = slot;
				}
			}
			return result;
		}
		private ScriptableObject.Slot GetSlot(string id, int index)
		{
			ScriptableObject.Slot[] array = this.slots;
			int slotPosition = ScriptableObject.GetSlotPosition(array, id, index);
			return (slotPosition < 0) ? null : array[slotPosition];
		}
		private static int GetSlotPosition(ScriptableObject.Slot[] slots, string id, int index)
		{
			int result;
			if (slots != null)
			{
				int num = (index & 2147483647) % slots.Length;
				int num2 = num;
				do
				{
					ScriptableObject.Slot slot = slots[num2];
					if (slot == null)
					{
						break;
					}
					if (slot != ScriptableObject.REMOVED && slot.intKey == index && (slot.stringKey == id || (id != null && id.Equals(slot.stringKey))))
					{
						goto Block_7;
					}
					if (++num2 == slots.Length)
					{
						num2 = 0;
					}
				}
				while (num2 != num);
				goto IL_B6;
				Block_7:
				result = num2;
				return result;
				IL_B6:;
			}
			result = -1;
			return result;
		}
		private ScriptableObject.Slot AddSlot(string id, int index, ScriptableObject.Slot newSlot)
		{
			Monitor.Enter(this);
			ScriptableObject.Slot result;
			try
			{
				if (this.Sealed)
				{
					string text = (id != null) ? id : Convert.ToString(index);
					throw Context.ReportRuntimeErrorById("msg.add.sealed", new object[]
					{
						text
					});
				}
				result = this.AddSlotImpl(id, index, newSlot);
			}
			finally
			{
				Monitor.Exit(this);
			}
			return result;
		}
		private ScriptableObject.Slot AddSlotImpl(string id, int index, ScriptableObject.Slot newSlot)
		{
			if (this.slots == null)
			{
				this.slots = new ScriptableObject.Slot[5];
			}
			int num = (index & 2147483647) % this.slots.Length;
			int num2 = num;
			ScriptableObject.Slot slot;
			while (true)
			{
				slot = this.slots[num2];
				if (slot == null || slot == ScriptableObject.REMOVED)
				{
					break;
				}
				if (slot.intKey == index && (slot.stringKey == id || (id != null && id.Equals(slot.stringKey))))
				{
					goto Block_9;
				}
				if (++num2 == this.slots.Length)
				{
					num2 = 0;
				}
				if (num2 == num)
				{
					goto Block_11;
				}
			}
			ScriptableObject.Slot result;
			if (4 * (this.count + 1) > 3 * this.slots.Length)
			{
				this.Grow();
				result = this.AddSlotImpl(id, index, newSlot);
				return result;
			}
			slot = ((newSlot == null) ? new ScriptableObject.Slot() : newSlot);
			slot.stringKey = id;
			slot.intKey = index;
			this.slots[num2] = slot;
			this.count++;
			result = slot;
			return result;
			Block_9:
			result = slot;
			return result;
			Block_11:
			throw new ApplicationException();
		}
		private void RemoveSlot(string name, int index)
		{
			Monitor.Enter(this);
			try
			{
				if (this.Sealed)
				{
					string text = (name != null) ? name : Convert.ToString(index);
					throw Context.ReportRuntimeErrorById("msg.remove.sealed", new object[]
					{
						text
					});
				}
				int slotPosition = ScriptableObject.GetSlotPosition(this.slots, name, index);
				if (slotPosition >= 0)
				{
					ScriptableObject.Slot slot = this.slots[slotPosition];
					if ((slot.attributes & 4) == 0)
					{
						slot.wasDeleted = 1;
						if (slot == this.lastAccess)
						{
							this.lastAccess = ScriptableObject.REMOVED;
						}
						this.count--;
						if (this.count != 0)
						{
							this.slots[slotPosition] = ScriptableObject.REMOVED;
						}
						else
						{
							this.slots[slotPosition] = null;
						}
					}
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
		}
		private void Grow()
		{
			ScriptableObject.Slot[] array = new ScriptableObject.Slot[this.slots.Length * 2 + 1];
			for (int i = this.slots.Length - 1; i >= 0; i--)
			{
				ScriptableObject.Slot slot = this.slots[i];
				if (slot != null && slot != ScriptableObject.REMOVED)
				{
					int num = (slot.intKey & 2147483647) % array.Length;
					while (array[num] != null)
					{
						if (++num == array.Length)
						{
							num = 0;
						}
					}
					array[num] = slot;
				}
			}
			this.slots = array;
		}
		internal virtual object[] GetIds(bool getAll)
		{
			ScriptableObject.Slot[] array = this.slots;
			object[] array2 = ScriptRuntime.EmptyArgs;
			object[] result;
			if (array == null)
			{
				result = array2;
			}
			else
			{
				int num = 0;
				for (int i = 0; i < array.Length; i++)
				{
					ScriptableObject.Slot slot = array[i];
					if (slot != null && slot != ScriptableObject.REMOVED)
					{
						if (getAll || (slot.attributes & 2) == 0)
						{
							if (num == 0)
							{
								array2 = new object[array.Length - i];
							}
							array2[num++] = ((slot.stringKey != null) ? slot.stringKey : slot.intKey);
						}
					}
				}
				if (num == array2.Length)
				{
					result = array2;
				}
				else
				{
					object[] array3 = new object[num];
					Array.Copy(array2, 0, array3, 0, num);
					result = array3;
				}
			}
			return result;
		}
		public virtual void DefineSetter(string name, ICallable setter)
		{
			ScriptableObject.Slot slot = this.GetSlot(name, name.GetHashCode());
			if (slot == null)
			{
				slot = new ScriptableObject.Slot();
				this.AddSlot(name, name.GetHashCode(), slot);
			}
			slot.setter = setter;
		}
		public virtual void DefineSetter(int index, ICallable setter)
		{
			ScriptableObject.Slot slot = this.GetSlot(null, index);
			if (slot == null)
			{
				slot = new ScriptableObject.Slot();
				this.AddSlot(null, index, slot);
			}
			slot.setter = setter;
		}
		public virtual void DefineGetter(int index, ICallable getter)
		{
			ScriptableObject.Slot slot = this.GetSlot(null, index);
			if (slot == null)
			{
				slot = new ScriptableObject.Slot();
				this.AddSlot(null, index, slot);
			}
			slot.getter = getter;
		}
		public virtual void DefineGetter(string name, ICallable getter)
		{
			ScriptableObject.Slot slot = this.GetSlot(name, name.GetHashCode());
			if (slot == null)
			{
				slot = new ScriptableObject.Slot();
				this.AddSlot(name, name.GetHashCode(), slot);
			}
			slot.getter = getter;
		}
		public virtual object LookupGetter(string name)
		{
			ScriptableObject.Slot slot = this.GetSlot(name, name.GetHashCode());
			object result;
			if (slot == null || slot.getter == null)
			{
				result = Undefined.Value;
			}
			else
			{
				result = slot.getter;
			}
			return result;
		}
		public virtual object LookupSetter(string name)
		{
			ScriptableObject.Slot slot = this.GetSlot(name, name.GetHashCode());
			object result;
			if (slot == null || slot.setter == null)
			{
				result = Undefined.Value;
			}
			else
			{
				result = slot.setter;
			}
			return result;
		}
	}
}
