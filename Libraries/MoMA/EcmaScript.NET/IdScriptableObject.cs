using System;
using System.Runtime.InteropServices;
using System.Threading;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public abstract class IdScriptableObject : ScriptableObject, IIdFunctionCall
	{
		private sealed class PrototypeValues
		{
			private const int VALUE_SLOT = 0;
			private const int NAME_SLOT = 1;
			private const int SLOT_SPAN = 2;
			private IdScriptableObject obj;
			private int maxId;
			private volatile object[] valueArray;
			private volatile short[] attributeArray;
			private volatile int lastFoundId = 1;
			internal int constructorId;
			private IdFunctionObject constructor;
			private short constructorAttrs;
			internal int MaxId
			{
				get
				{
					return this.maxId;
				}
			}
			internal PrototypeValues(IdScriptableObject obj, int maxId)
			{
				if (obj == null)
				{
					throw new ArgumentNullException("obj");
				}
				if (maxId < 1)
				{
					throw new ArgumentException("maxId may not lower than 1");
				}
				this.obj = obj;
				this.maxId = maxId;
			}
			internal void InitValue(int id, string name, object value, int attributes)
			{
				if (1 > id || id > this.maxId)
				{
					throw new ArgumentException();
				}
				if (name == null)
				{
					throw new ArgumentException();
				}
				if (value == UniqueTag.NotFound)
				{
					throw new ArgumentException();
				}
				ScriptableObject.CheckValidAttributes(attributes);
				if (this.obj.FindPrototypeId(name) != id)
				{
					throw new ArgumentException(name);
				}
				if (id == this.constructorId)
				{
					if (!(value is IdFunctionObject))
					{
						throw new ArgumentException("consructor should be initialized with IdFunctionObject");
					}
					this.constructor = (IdFunctionObject)value;
					this.constructorAttrs = (short)attributes;
				}
				else
				{
					this.InitSlot(id, name, value, attributes);
				}
			}
			private void InitSlot(int id, string name, object value, int attributes)
			{
				object[] array = this.valueArray;
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}
				if (value == null)
				{
					value = UniqueTag.NullValue;
				}
				int num = (id - 1) * 2;
				Monitor.Enter(this);
				try
				{
					object obj = array[num];
					if (obj == null)
					{
						array[num] = value;
						array[num + 1] = name;
						this.attributeArray[id - 1] = (short)attributes;
					}
					else
					{
						if (!name.Equals(array[num + 1]))
						{
							throw new ApplicationException();
						}
					}
				}
				finally
				{
					Monitor.Exit(this);
				}
			}
			internal IdFunctionObject createPrecachedConstructor()
			{
				if (this.constructorId != 0)
				{
					throw new ApplicationException();
				}
				this.constructorId = this.obj.FindPrototypeId("constructor");
				if (this.constructorId == 0)
				{
					throw new ApplicationException("No id for constructor property");
				}
				this.obj.InitPrototypeId(this.constructorId);
				if (this.constructor == null)
				{
					throw new ApplicationException(this.obj.GetType().FullName + ".initPrototypeId() did not initialize id=" + this.constructorId);
				}
				this.constructor.InitFunction(this.obj.ClassName, ScriptableObject.GetTopLevelScope(this.obj));
				this.constructor.MarkAsConstructor(this.obj);
				return this.constructor;
			}
			internal int FindId(string name)
			{
				object[] array = this.valueArray;
				int result;
				if (array == null)
				{
					result = this.obj.FindPrototypeId(name);
				}
				else
				{
					int num = this.lastFoundId;
					if (name == array[(num - 1) * 2 + 1])
					{
						result = num;
					}
					else
					{
						num = this.obj.FindPrototypeId(name);
						if (num != 0)
						{
							int num2 = (num - 1) * 2 + 1;
							array[num2] = name;
							this.lastFoundId = num;
						}
						result = num;
					}
				}
				return result;
			}
			internal bool Has(int id)
			{
				object[] array = this.valueArray;
				bool result;
				if (array == null)
				{
					result = true;
				}
				else
				{
					int num = (id - 1) * 2;
					object obj = array[num];
					result = (obj == null || obj != UniqueTag.NotFound);
				}
				return result;
			}
			internal object Get(int id)
			{
				object obj = this.EnsureId(id);
				if (obj == UniqueTag.NullValue)
				{
					obj = null;
				}
				return obj;
			}
			internal void Set(int id, IScriptable start, object value)
			{
				if (value == UniqueTag.NotFound)
				{
					throw new ArgumentException();
				}
				this.EnsureId(id);
				int num = (int)this.attributeArray[id - 1];
				if ((num & 1) == 0)
				{
					if (start == this.obj)
					{
						if (value == null)
						{
							value = UniqueTag.NullValue;
						}
						int num2 = (id - 1) * 2;
						Monitor.Enter(this);
						try
						{
							this.valueArray[num2] = value;
						}
						finally
						{
							Monitor.Exit(this);
						}
					}
					else
					{
						int num3 = (id - 1) * 2 + 1;
						string name = (string)this.valueArray[num3];
						start.Put(name, start, value);
					}
				}
			}
			internal void Delete(int id)
			{
				this.EnsureId(id);
				int num = (int)this.attributeArray[id - 1];
				if ((num & 4) == 0)
				{
					int num2 = (id - 1) * 2;
					Monitor.Enter(this);
					try
					{
						this.valueArray[num2] = UniqueTag.NotFound;
						this.attributeArray[id - 1] = 0;
					}
					finally
					{
						Monitor.Exit(this);
					}
				}
			}
			internal int GetAttributes(int id)
			{
				this.EnsureId(id);
				return (int)this.attributeArray[id - 1];
			}
			internal void SetAttributes(int id, int attributes)
			{
				ScriptableObject.CheckValidAttributes(attributes);
				this.EnsureId(id);
				Monitor.Enter(this);
				try
				{
					this.attributeArray[id - 1] = (short)attributes;
				}
				finally
				{
					Monitor.Exit(this);
				}
			}
			internal object[] GetNames(bool getAll, object[] extraEntries)
			{
				object[] array = null;
				int num = 0;
				for (int i = 1; i <= this.maxId; i++)
				{
					object obj = this.EnsureId(i);
					if (getAll || (this.attributeArray[i - 1] & 2) == 0)
					{
						if (obj != UniqueTag.NotFound)
						{
							int num2 = (i - 1) * 2 + 1;
							string text = (string)this.valueArray[num2];
							if (array == null)
							{
								array = new object[this.maxId];
							}
							array[num++] = text;
						}
					}
				}
				object[] result;
				if (num == 0)
				{
					result = extraEntries;
				}
				else
				{
					if (extraEntries == null || extraEntries.Length == 0)
					{
						if (num != array.Length)
						{
							object[] array2 = new object[num];
							Array.Copy(array, 0, array2, 0, num);
							array = array2;
						}
						result = array;
					}
					else
					{
						int num3 = extraEntries.Length;
						object[] array2 = new object[num3 + num];
						Array.Copy(extraEntries, 0, array2, 0, num3);
						Array.Copy(array, 0, array2, num3, num);
						result = array2;
					}
				}
				return result;
			}
			private object EnsureId(int id)
			{
				object[] array = this.valueArray;
				if (array == null)
				{
					Monitor.Enter(this);
					try
					{
						array = this.valueArray;
						if (array == null)
						{
							array = new object[this.maxId * 2];
							this.valueArray = array;
							this.attributeArray = new short[this.maxId];
						}
					}
					finally
					{
						Monitor.Exit(this);
					}
				}
				int num = (id - 1) * 2;
				object obj = array[num];
				if (obj == null)
				{
					if (id == this.constructorId)
					{
						this.InitSlot(this.constructorId, "constructor", this.constructor, (int)this.constructorAttrs);
						this.constructor = null;
					}
					else
					{
						this.obj.InitPrototypeId(id);
					}
					obj = array[num];
					if (obj == null)
					{
						throw new ApplicationException(this.obj.GetType().FullName + ".initPrototypeId(int id) did not initialize id=" + id);
					}
				}
				return obj;
			}
		}
		private volatile IdScriptableObject.PrototypeValues prototypeValues;
		protected internal virtual int MaxInstanceId
		{
			get
			{
				return 0;
			}
		}
		public IdScriptableObject()
		{
		}
		public IdScriptableObject(IScriptable scope, IScriptable prototype) : base(scope, prototype)
		{
		}
		protected internal object DefaultGet(string name)
		{
			return base.Get(name, this);
		}
		protected internal void DefaultPut(string name, object value)
		{
			base.Put(name, this, value);
		}
		public override bool Has(string name, IScriptable start)
		{
			int num = this.FindInstanceIdInfo(name);
			bool result;
			if (num != 0)
			{
				int num2 = (int)((uint)num >> 16);
				if ((num2 & 4) != 0)
				{
					result = true;
				}
				else
				{
					int num3 = num & 65535;
					result = (UniqueTag.NotFound != this.GetInstanceIdValue(num3));
				}
			}
			else
			{
				if (this.prototypeValues != null)
				{
					int num3 = this.prototypeValues.FindId(name);
					if (num3 != 0)
					{
						result = this.prototypeValues.Has(num3);
						return result;
					}
				}
				result = base.Has(name, start);
			}
			return result;
		}
		public override object Get(string name, IScriptable start)
		{
			int num = this.FindInstanceIdInfo(name);
			object result;
			if (num != 0)
			{
				int num2 = num & 65535;
				result = this.GetInstanceIdValue(num2);
			}
			else
			{
				if (this.prototypeValues != null)
				{
					int num2 = this.prototypeValues.FindId(name);
					if (num2 != 0)
					{
						result = this.prototypeValues.Get(num2);
						return result;
					}
				}
				result = base.Get(name, start);
			}
			return result;
		}
		public override object Put(string name, IScriptable start, object value)
		{
			int num = this.FindInstanceIdInfo(name);
			object result;
			if (num != 0)
			{
				if (start == this && base.Sealed)
				{
					throw Context.ReportRuntimeErrorById("msg.modify.sealed", new object[]
					{
						name
					});
				}
				int num2 = (int)((uint)num >> 16);
				if ((num2 & 1) == 0)
				{
					if (start != this)
					{
						result = start.Put(name, start, value);
						return result;
					}
					int num3 = num & 65535;
					this.SetInstanceIdValue(num3, value);
				}
				result = value;
			}
			else
			{
				if (this.prototypeValues != null)
				{
					int num3 = this.prototypeValues.FindId(name);
					if (num3 != 0)
					{
						if (start == this && base.Sealed)
						{
							throw Context.ReportRuntimeErrorById("msg.modify.sealed", new object[]
							{
								name
							});
						}
						this.prototypeValues.Set(num3, start, value);
						result = value;
						return result;
					}
				}
				result = base.Put(name, start, value);
			}
			return result;
		}
		public override void Delete(string name)
		{
			int num = this.FindInstanceIdInfo(name);
			if (num != 0)
			{
				if (!base.Sealed)
				{
					int num2 = (int)((uint)num >> 16);
					if ((num2 & 4) == 0)
					{
						int num3 = num & 65535;
						this.SetInstanceIdValue(num3, UniqueTag.NotFound);
					}
					return;
				}
			}
			if (this.prototypeValues != null)
			{
				int num3 = this.prototypeValues.FindId(name);
				if (num3 != 0)
				{
					if (!base.Sealed)
					{
						this.prototypeValues.Delete(num3);
					}
					return;
				}
			}
			base.Delete(name);
		}
		public override int GetAttributes(string name)
		{
			int num = this.FindInstanceIdInfo(name);
			int result;
			if (num != 0)
			{
				int num2 = (int)((uint)num >> 16);
				result = num2;
			}
			else
			{
				if (this.prototypeValues != null)
				{
					int num3 = this.prototypeValues.FindId(name);
					if (num3 != 0)
					{
						result = this.prototypeValues.GetAttributes(num3);
						return result;
					}
				}
				result = base.GetAttributes(name);
			}
			return result;
		}
		public override void SetAttributes(string name, int attributes)
		{
			ScriptableObject.CheckValidAttributes(attributes);
			int num = this.FindInstanceIdInfo(name);
			if (num != 0)
			{
				int num2 = (int)((uint)num >> 16);
				if (attributes != num2)
				{
					throw new ApplicationException("Change of attributes for this id is not supported");
				}
			}
			else
			{
				if (this.prototypeValues != null)
				{
					int num3 = this.prototypeValues.FindId(name);
					if (num3 != 0)
					{
						this.prototypeValues.SetAttributes(num3, attributes);
						return;
					}
				}
				base.SetAttributes(name, attributes);
			}
		}
		internal override object[] GetIds(bool getAll)
		{
			object[] array = base.GetIds(getAll);
			if (this.prototypeValues != null)
			{
				array = this.prototypeValues.GetNames(getAll, array);
			}
			int maxInstanceId = this.MaxInstanceId;
			if (maxInstanceId != 0)
			{
				object[] array2 = null;
				int num = 0;
				int num2 = maxInstanceId;
				while (num2 != 0)
				{
					string instanceIdName = this.GetInstanceIdName(num2);
					int num3 = this.FindInstanceIdInfo(instanceIdName);
					if (num3 != 0)
					{
						int num4 = (int)((uint)num3 >> 16);
						if ((num4 & 4) == 0)
						{
							if (UniqueTag.NotFound == this.GetInstanceIdValue(num2))
							{
								goto IL_ED;
							}
						}
						if (getAll || (num4 & 2) == 0)
						{
							if (num == 0)
							{
								array2 = new object[num2];
							}
							array2[num++] = instanceIdName;
						}
					}
					IL_ED:
					num2--;
					continue;
					goto IL_ED;
				}
				if (num != 0)
				{
					if (array.Length == 0 && array2.Length == num)
					{
						array = array2;
					}
					else
					{
						object[] array3 = new object[array.Length + num];
						Array.Copy(array, 0, array3, 0, array.Length);
						Array.Copy(array2, 0, array3, array.Length, num);
						array = array3;
					}
				}
			}
			return array;
		}
		protected internal static int InstanceIdInfo(int attributes, int id)
		{
			return attributes << 16 | id;
		}
		protected internal virtual int FindInstanceIdInfo(string name)
		{
			return 0;
		}
		protected internal virtual string GetInstanceIdName(int id)
		{
			throw new ArgumentException(Convert.ToString(id));
		}
		protected internal virtual object GetInstanceIdValue(int id)
		{
			throw new ApplicationException(Convert.ToString(id));
		}
		protected internal virtual void SetInstanceIdValue(int id, object value)
		{
			throw new ApplicationException(Convert.ToString(id));
		}
		public virtual object ExecIdCall(IdFunctionObject f, Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			throw f.Unknown();
		}
		public IdFunctionObject ExportAsJSClass(int maxPrototypeId, IScriptable scope, bool zealed)
		{
			return this.ExportAsJSClass(maxPrototypeId, scope, zealed, 2);
		}
		public IdFunctionObject ExportAsJSClass(int maxPrototypeId, IScriptable scope, bool zealed, int attributes)
		{
			if (scope != this && scope != null)
			{
				base.ParentScope = scope;
				this.SetPrototype(ScriptableObject.GetObjectPrototype(scope));
			}
			this.ActivatePrototypeMap(maxPrototypeId);
			IdFunctionObject idFunctionObject = this.prototypeValues.createPrecachedConstructor();
			if (zealed)
			{
				this.SealObject();
			}
			this.FillConstructorProperties(idFunctionObject);
			if (zealed)
			{
				idFunctionObject.SealObject();
			}
			idFunctionObject.ExportAsScopeProperty(attributes);
			return idFunctionObject;
		}
		public void ActivatePrototypeMap(int maxPrototypeId)
		{
			IdScriptableObject.PrototypeValues prototypeValues = new IdScriptableObject.PrototypeValues(this, maxPrototypeId);
			Monitor.Enter(this);
			try
			{
				if (this.prototypeValues != null)
				{
					throw new ApplicationException();
				}
				this.prototypeValues = prototypeValues;
			}
			finally
			{
				Monitor.Exit(this);
			}
		}
		public void InitPrototypeMethod(object tag, int id, string name, int arity)
		{
			IScriptable topLevelScope = ScriptableObject.GetTopLevelScope(this);
			IdFunctionObject value = this.NewIdFunction(tag, id, name, arity, topLevelScope);
			this.prototypeValues.InitValue(id, name, value, 2);
		}
		public void InitPrototypeConstructor(IdFunctionObject f)
		{
			int constructorId = this.prototypeValues.constructorId;
			if (constructorId == 0)
			{
				throw new ApplicationException();
			}
			if (f.MethodId != constructorId)
			{
				throw new ArgumentException();
			}
			if (base.Sealed)
			{
				f.SealObject();
			}
			this.prototypeValues.InitValue(constructorId, "constructor", f, 2);
		}
		public void InitPrototypeValue(int id, string name, object value, int attributes)
		{
			this.prototypeValues.InitValue(id, name, value, attributes);
		}
		protected internal virtual void InitPrototypeId(int id)
		{
			throw new ApplicationException(Convert.ToString(id));
		}
		protected internal virtual int FindPrototypeId(string name)
		{
			throw new ApplicationException(name);
		}
		protected internal virtual void FillConstructorProperties(IdFunctionObject ctor)
		{
		}
		protected internal virtual void AddIdFunctionProperty(IScriptable obj, object tag, int id, string name, int arity)
		{
			IScriptable topLevelScope = ScriptableObject.GetTopLevelScope(obj);
			IdFunctionObject idFunctionObject = this.NewIdFunction(tag, id, name, arity, topLevelScope);
			idFunctionObject.AddAsProperty(obj);
		}
		protected internal static EcmaScriptError IncompatibleCallError(IdFunctionObject f)
		{
			throw ScriptRuntime.TypeErrorById("msg.incompat.call", new string[]
			{
				f.FunctionName
			});
		}
		private IdFunctionObject NewIdFunction(object tag, int id, string name, int arity, IScriptable scope)
		{
			IdFunctionObject idFunctionObject = new IdFunctionObject(this, tag, id, name, arity, scope);
			if (base.Sealed)
			{
				idFunctionObject.SealObject();
			}
			return idFunctionObject;
		}
	}
}
