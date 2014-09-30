using EcmaScript.NET.Collections;
using System;
using System.Runtime.InteropServices;
using System.Text;
namespace EcmaScript.NET.Types
{
	[ComVisible(true)]
	public class BuiltinArray : IdScriptableObject
	{
		private class StringBuilder
		{
			private int m_TopIdx = 0;
			private int m_InnerIdx = 0;
			private string[][] m_Buffer = null;
			public StringBuilder(long length)
			{
				int num = 32000;
				int num2 = Math.Max((int)(length / (long)num), 1);
				this.m_Buffer = new string[num2][];
				for (int i = 0; i < num2; i++)
				{
					int num3 = (int)Math.Min(length, (long)num);
					this.m_Buffer[i] = new string[num3];
					length -= (long)num3;
				}
			}
			public void Append(string value)
			{
				string[] array = this.m_Buffer[this.m_TopIdx];
				if (this.m_InnerIdx > array.Length)
				{
					this.m_TopIdx++;
					this.Append(value);
				}
				else
				{
					array[this.m_InnerIdx++] = value;
				}
			}
			public string ToString(string seperator)
			{
				string text = string.Empty;
				string[][] buffer = this.m_Buffer;
				for (int i = 0; i < buffer.Length; i++)
				{
					string[] value = buffer[i];
					if (text != string.Empty)
					{
						text += seperator;
					}
					text += string.Join(seperator, value);
				}
				return text;
			}
		}
		private const int maximumDenseLength = 10000;
		private const int Id_length = 1;
		private const int MAX_INSTANCE_ID = 1;
		private const int Id_constructor = 1;
		private const int Id_toString = 2;
		private const int Id_toLocaleString = 3;
		private const int Id_toSource = 4;
		private const int Id_join = 5;
		private const int Id_reverse = 6;
		private const int Id_sort = 7;
		private const int Id_push = 8;
		private const int Id_pop = 9;
		private const int Id_shift = 10;
		private const int Id_unshift = 11;
		private const int Id_splice = 12;
		private const int Id_concat = 13;
		private const int Id_slice = 14;
		private const int MAX_PROTOTYPE_ID = 14;
		private long length;
		private object[] dense;
		private static readonly object ARRAY_TAG = new object();
		public override string ClassName
		{
			get
			{
				return "Array";
			}
		}
		protected internal override int MaxInstanceId
		{
			get
			{
				return 1;
			}
		}
		internal static void Init(IScriptable scope, bool zealed)
		{
			BuiltinArray builtinArray = new BuiltinArray();
			builtinArray.ExportAsJSClass(14, scope, zealed, 7);
		}
		private BuiltinArray()
		{
			this.dense = null;
			this.length = 0L;
		}
		public BuiltinArray(long length)
		{
			int num = (int)length;
			if ((long)num == length && num > 0)
			{
				if (num > 10000)
				{
					num = 10000;
				}
				this.dense = new object[num];
				for (int i = 0; i < num; i++)
				{
					this.dense[i] = UniqueTag.NotFound;
				}
			}
			this.length = length;
		}
		public BuiltinArray(object[] array)
		{
			this.dense = array;
			this.length = (long)array.Length;
		}
		protected internal override int FindInstanceIdInfo(string s)
		{
			int result;
			if (s.Equals("length"))
			{
				result = IdScriptableObject.InstanceIdInfo(6, 1);
			}
			else
			{
				result = base.FindInstanceIdInfo(s);
			}
			return result;
		}
		protected internal override string GetInstanceIdName(int id)
		{
			string result;
			if (id == 1)
			{
				result = "length";
			}
			else
			{
				result = base.GetInstanceIdName(id);
			}
			return result;
		}
		protected internal override object GetInstanceIdValue(int id)
		{
			object result;
			if (id == 1)
			{
				result = this.length;
			}
			else
			{
				result = base.GetInstanceIdValue(id);
			}
			return result;
		}
		protected internal override void SetInstanceIdValue(int id, object value)
		{
			if (id == 1)
			{
				this.setLength(value);
			}
			else
			{
				base.SetInstanceIdValue(id, value);
			}
		}
		protected internal override void InitPrototypeId(int id)
		{
			int arity;
			string name;
			switch (id)
			{
			case 1:
				arity = 1;
				name = "constructor";
				break;
			case 2:
				arity = 0;
				name = "toString";
				break;
			case 3:
				arity = 1;
				name = "toLocaleString";
				break;
			case 4:
				arity = 0;
				name = "toSource";
				break;
			case 5:
				arity = 1;
				name = "join";
				break;
			case 6:
				arity = 0;
				name = "reverse";
				break;
			case 7:
				arity = 1;
				name = "sort";
				break;
			case 8:
				arity = 1;
				name = "push";
				break;
			case 9:
				arity = 1;
				name = "pop";
				break;
			case 10:
				arity = 1;
				name = "shift";
				break;
			case 11:
				arity = 1;
				name = "unshift";
				break;
			case 12:
				arity = 1;
				name = "splice";
				break;
			case 13:
				arity = 1;
				name = "concat";
				break;
			case 14:
				arity = 1;
				name = "slice";
				break;
			default:
				throw new ArgumentException(Convert.ToString(id));
			}
			base.InitPrototypeMethod(BuiltinArray.ARRAY_TAG, id, name, arity);
		}
		public override object ExecIdCall(IdFunctionObject f, Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			object result;
			if (!f.HasTag(BuiltinArray.ARRAY_TAG))
			{
				result = base.ExecIdCall(f, cx, scope, thisObj, args);
			}
			else
			{
				int methodId = f.MethodId;
				switch (methodId)
				{
				case 1:
					if (thisObj != null)
					{
						result = f.Construct(cx, scope, args);
					}
					else
					{
						result = BuiltinArray.ImplCtor(cx, scope, args);
					}
					break;
				case 2:
					result = BuiltinArray.toStringHelper(cx, scope, thisObj, cx.HasFeature(Context.Features.ToStringAsSource), false);
					break;
				case 3:
					result = BuiltinArray.toStringHelper(cx, scope, thisObj, false, true);
					break;
				case 4:
					result = BuiltinArray.toStringHelper(cx, scope, thisObj, true, false);
					break;
				case 5:
					result = BuiltinArray.ImplJoin(cx, thisObj, args);
					break;
				case 6:
					result = BuiltinArray.ImplReverse(cx, thisObj, args);
					break;
				case 7:
					result = BuiltinArray.ImplSort(cx, scope, thisObj, args);
					break;
				case 8:
					result = BuiltinArray.ImplPush(cx, thisObj, args);
					break;
				case 9:
					result = BuiltinArray.ImplPop(cx, thisObj, args);
					break;
				case 10:
					result = BuiltinArray.ImplShift(cx, thisObj, args);
					break;
				case 11:
					result = BuiltinArray.ImplUnshift(cx, thisObj, args);
					break;
				case 12:
					result = BuiltinArray.ImplSplice(cx, scope, thisObj, args);
					break;
				case 13:
					result = BuiltinArray.ImplConcat(cx, scope, thisObj, args);
					break;
				case 14:
					result = this.ImplSlice(cx, thisObj, args);
					break;
				default:
					throw new ArgumentException(Convert.ToString(methodId));
				}
			}
			return result;
		}
		public override object Get(int index, IScriptable start)
		{
			object result;
			if (this.dense != null && 0 <= index && index < this.dense.Length)
			{
				result = this.dense[index];
			}
			else
			{
				result = base.Get(index, start);
			}
			return result;
		}
		public override bool Has(int index, IScriptable start)
		{
			bool result;
			if (this.dense != null && 0 <= index && index < this.dense.Length)
			{
				result = (this.dense[index] != UniqueTag.NotFound);
			}
			else
			{
				result = base.Has(index, start);
			}
			return result;
		}
		private static long toArrayIndex(string id)
		{
			double num = ScriptConvert.ToNumber(id);
			long result;
			if (!double.IsNaN(num))
			{
				long num2 = ScriptConvert.ToUint32(num);
				if ((double)num2 == num && num2 != (long)((ulong)-1))
				{
					if (Convert.ToString(num2).Equals(id))
					{
						result = num2;
						return result;
					}
				}
			}
			result = -1L;
			return result;
		}
		public override object Put(string id, IScriptable start, object value)
		{
			object result = base.Put(id, start, value);
			if (start == this)
			{
				long num = BuiltinArray.toArrayIndex(id);
				if (num >= this.length)
				{
					this.length = num + 1L;
				}
			}
			return result;
		}
		public override object Put(int index, IScriptable start, object value)
		{
			object result = value;
			if (start == this && !base.Sealed && this.dense != null && 0 <= index && index < this.dense.Length)
			{
				this.dense[index] = value;
			}
			else
			{
				result = base.Put(index, start, value);
			}
			if (start == this)
			{
				if (this.length <= (long)index)
				{
					this.length = (long)index + 1L;
				}
			}
			return result;
		}
		public override void Delete(int index)
		{
			if (!base.Sealed && this.dense != null && 0 <= index && index < this.dense.Length)
			{
				this.dense[index] = UniqueTag.NotFound;
			}
			else
			{
				base.Delete(index);
			}
		}
		public override object[] GetIds()
		{
			object[] ids = base.GetIds();
			object[] result;
			if (this.dense == null)
			{
				result = ids;
			}
			else
			{
				int num = this.dense.Length;
				long num2 = this.length;
				if ((long)num > num2)
				{
					num = (int)num2;
				}
				if (num == 0)
				{
					result = ids;
				}
				else
				{
					int num3 = ids.Length;
					object[] array = new object[num + num3];
					Array.Copy(this.dense, 0, array, 0, num);
					int num4 = 0;
					for (int num5 = 0; num5 != num; num5++)
					{
						if (array[num5] != UniqueTag.NotFound)
						{
							array[num4] = num5;
							num4++;
						}
					}
					if (num4 != num)
					{
						object[] array2 = new object[num4 + num3];
						Array.Copy(array, 0, array2, 0, num4);
						array = array2;
					}
					Array.Copy(ids, 0, array, num4, num3);
					result = array;
				}
			}
			return result;
		}
		public override object GetDefaultValue(Type hint)
		{
			object result;
			if (CliHelper.IsNumberType(hint))
			{
				Context currentContext = Context.CurrentContext;
				if (currentContext.Version == Context.Versions.JS1_2)
				{
					result = this.length;
					return result;
				}
			}
			result = base.GetDefaultValue(hint);
			return result;
		}
		private static object ImplCtor(Context cx, IScriptable scope, object[] args)
		{
			object result;
			if (args.Length == 0)
			{
				result = new BuiltinArray();
			}
			else
			{
				if (cx.Version == Context.Versions.JS1_2)
				{
					result = new BuiltinArray(args);
				}
				else
				{
					object obj = args[0];
					if (args.Length > 1 || !CliHelper.IsNumber(obj))
					{
						result = new BuiltinArray(args);
					}
					else
					{
						result = new BuiltinArray(BuiltinArray.VerifyOutOfRange(obj));
					}
				}
			}
			return result;
		}
		private static long VerifyOutOfRange(long newLen)
		{
			long num = ScriptConvert.ToUint32((double)newLen);
			if (num < 0L || num != (long)ScriptConvert.ToNumber(newLen))
			{
				throw ScriptRuntime.ConstructError("RangeError", ScriptRuntime.GetMessage("msg.arraylength.bad", new object[0]));
			}
			return num;
		}
		private static long VerifyOutOfRange(object newLen)
		{
			long num = ScriptConvert.ToUint32(newLen);
			if (num < 0L || num != (long)ScriptConvert.ToNumber(newLen))
			{
				throw ScriptRuntime.ConstructError("RangeError", ScriptRuntime.GetMessage("msg.arraylength.bad", new object[0]));
			}
			return num;
		}
		public virtual long getLength()
		{
			return this.length;
		}
		private void setLength(object val)
		{
			long num = BuiltinArray.VerifyOutOfRange(val);
			if (num < this.length)
			{
				if (this.length - num > 4096L)
				{
					object[] ids = this.GetIds();
					for (int i = 0; i < ids.Length; i++)
					{
						object obj = ids[i];
						if (obj is string)
						{
							string text = (string)obj;
							long num2 = BuiltinArray.toArrayIndex(text);
							if (num2 >= num)
							{
								this.Delete(text);
							}
						}
						else
						{
							int num3 = (int)obj;
							if ((long)num3 >= num)
							{
								this.Delete(num3);
							}
						}
					}
				}
				else
				{
					for (long num4 = num; num4 < this.length; num4 += 1L)
					{
						BuiltinArray.deleteElem(this, num4);
					}
				}
			}
			this.length = num;
		}
		internal static long getLengthProperty(Context cx, IScriptable obj)
		{
			long result;
			if (obj is BuiltinString)
			{
				result = (long)((BuiltinString)obj).Length;
			}
			else
			{
				if (obj is BuiltinArray)
				{
					result = ((BuiltinArray)obj).getLength();
				}
				else
				{
					result = ScriptConvert.ToUint32(ScriptRuntime.getObjectProp(obj, "length", cx));
				}
			}
			return result;
		}
		private static object setLengthProperty(Context cx, IScriptable target, long length)
		{
			return ScriptRuntime.setObjectProp(target, "length", length, cx);
		}
		private static void deleteElem(IScriptable target, long index)
		{
			int num = (int)index;
			if ((long)num == index)
			{
				target.Delete(num);
			}
			else
			{
				target.Delete(Convert.ToString(index));
			}
		}
		private static object getElem(Context cx, IScriptable target, long index)
		{
			object result;
			if (index > 2147483647L)
			{
				string property = Convert.ToString(index);
				result = ScriptRuntime.getObjectProp(target, property, cx);
			}
			else
			{
				result = ScriptRuntime.getObjectIndex(target, (int)index, cx);
			}
			return result;
		}
		private static void setElem(Context cx, IScriptable target, long index, object value)
		{
			if (index > 2147483647L)
			{
				string property = Convert.ToString(index);
				ScriptRuntime.setObjectProp(target, property, value, cx);
			}
			else
			{
				ScriptRuntime.setObjectIndex(target, (int)index, value, cx);
			}
		}
		private static string toStringHelper(Context cx, IScriptable scope, IScriptable thisObj, bool toSource, bool toLocale)
		{
			long lengthProperty = BuiltinArray.getLengthProperty(cx, thisObj);
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(256);
			string value;
			if (toSource)
			{
				stringBuilder.Append('[');
				value = ", ";
			}
			else
			{
				value = ",";
			}
			bool flag = false;
			long num = 0L;
			bool flag2;
			bool flag3;
			if (cx.iterating == null)
			{
				flag2 = true;
				flag3 = false;
				cx.iterating = new ObjToIntMap(31);
			}
			else
			{
				flag2 = false;
				flag3 = cx.iterating.has(thisObj);
			}
			try
			{
				if (!flag3)
				{
					cx.iterating.put(thisObj, 0);
					for (num = 0L; num < lengthProperty; num += 1L)
					{
						if (num > 0L)
						{
							stringBuilder.Append(value);
						}
						object obj = BuiltinArray.getElem(cx, thisObj, num);
						if (obj == null || obj == Undefined.Value)
						{
							flag = false;
						}
						else
						{
							flag = true;
							if (toSource)
							{
								stringBuilder.Append(ScriptRuntime.uneval(cx, scope, obj));
							}
							else
							{
								if (obj is string)
								{
									string text = (string)obj;
									if (toSource)
									{
										stringBuilder.Append('"');
										stringBuilder.Append(ScriptRuntime.escapeString(text));
										stringBuilder.Append('"');
									}
									else
									{
										stringBuilder.Append(text);
									}
								}
								else
								{
									if (toLocale && obj != Undefined.Value && obj != null)
									{
										ICallable propFunctionAndThis = ScriptRuntime.getPropFunctionAndThis(obj, "toLocaleString", cx);
										IScriptable thisObj2 = ScriptRuntime.lastStoredScriptable(cx);
										obj = propFunctionAndThis.Call(cx, scope, thisObj2, ScriptRuntime.EmptyArgs);
									}
									stringBuilder.Append(ScriptConvert.ToString(obj));
								}
							}
						}
					}
				}
			}
			finally
			{
				if (flag2)
				{
					cx.iterating = null;
				}
			}
			if (toSource)
			{
				if (!flag && num > 0L)
				{
					stringBuilder.Append(", ]");
				}
				else
				{
					stringBuilder.Append(']');
				}
			}
			return stringBuilder.ToString();
		}
		private static string ImplJoin(Context cx, IScriptable thisObj, object[] args)
		{
			long lengthProperty = BuiltinArray.getLengthProperty(cx, thisObj);
			int num = (int)lengthProperty;
			if (lengthProperty != (long)num)
			{
				throw Context.ReportRuntimeErrorById("msg.arraylength.too.big", new object[]
				{
					Convert.ToString(lengthProperty)
				});
			}
			string text;
			if (args.Length < 1 || args[0] == Undefined.Value)
			{
				text = ",";
			}
			else
			{
				text = ScriptConvert.ToString(args[0]);
			}
			string result;
			if (num == 0)
			{
				result = "";
			}
			else
			{
				string[] array = new string[num];
				int num2 = 0;
				for (int num3 = 0; num3 != num; num3++)
				{
					object elem = BuiltinArray.getElem(cx, thisObj, (long)num3);
					if (elem != null && elem != Undefined.Value)
					{
						string text2 = ScriptConvert.ToString(elem);
						num2 += text2.Length;
						array[num3] = text2;
					}
				}
				num2 += (num - 1) * text.Length;
				System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(num2);
				for (int num3 = 0; num3 != num; num3++)
				{
					if (num3 != 0)
					{
						stringBuilder.Append(text);
					}
					string text2 = array[num3];
					if (text2 != null)
					{
						stringBuilder.Append(text2);
					}
				}
				result = stringBuilder.ToString();
			}
			return result;
		}
		private static IScriptable ImplReverse(Context cx, IScriptable thisObj, object[] args)
		{
			long lengthProperty = BuiltinArray.getLengthProperty(cx, thisObj);
			long num = lengthProperty / 2L;
			for (long num2 = 0L; num2 < num; num2 += 1L)
			{
				long index = lengthProperty - num2 - 1L;
				object elem = BuiltinArray.getElem(cx, thisObj, num2);
				object elem2 = BuiltinArray.getElem(cx, thisObj, index);
				BuiltinArray.setElem(cx, thisObj, num2, elem2);
				BuiltinArray.setElem(cx, thisObj, index, elem);
			}
			return thisObj;
		}
		private static IScriptable ImplSort(Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			long lengthProperty = BuiltinArray.getLengthProperty(cx, thisObj);
			IScriptable result;
			if (lengthProperty <= 1L)
			{
				result = thisObj;
			}
			else
			{
				object cmp;
				object[] cmpBuf;
				if (args.Length > 0 && Undefined.Value != args[0])
				{
					cmp = args[0];
					cmpBuf = new object[2];
				}
				else
				{
					cmp = null;
					cmpBuf = null;
				}
				if (lengthProperty >= 2147483647L)
				{
					BuiltinArray.heapsort_extended(cx, scope, thisObj, lengthProperty, cmp, cmpBuf);
				}
				else
				{
					int num = (int)lengthProperty;
					object[] array = new object[num];
					for (int num2 = 0; num2 != num; num2++)
					{
						array[num2] = BuiltinArray.getElem(cx, thisObj, (long)num2);
					}
					BuiltinArray.heapsort(cx, scope, array, num, cmp, cmpBuf);
					for (int num2 = 0; num2 != num; num2++)
					{
						BuiltinArray.setElem(cx, thisObj, (long)num2, array[num2]);
					}
				}
				result = thisObj;
			}
			return result;
		}
		private static bool IsBigger(Context cx, IScriptable scope, object x, object y, object cmp, object[] cmpBuf)
		{
			if (cmp == null)
			{
				if (cmpBuf != null)
				{
					Context.CodeBug();
				}
			}
			else
			{
				if (cmpBuf == null || cmpBuf.Length != 2)
				{
					Context.CodeBug();
				}
			}
			object value = Undefined.Value;
			bool result;
			if (value == y)
			{
				result = false;
			}
			else
			{
				if (value == x)
				{
					result = true;
				}
				else
				{
					if (cmp == null)
					{
						string strA = ScriptConvert.ToString(x);
						string strB = ScriptConvert.ToString(y);
						result = (string.CompareOrdinal(strA, strB) > 0);
					}
					else
					{
						cmpBuf[0] = x;
						cmpBuf[1] = y;
						ICallable valueFunctionAndThis = ScriptRuntime.getValueFunctionAndThis(cmp, cx);
						IScriptable thisObj = ScriptRuntime.lastStoredScriptable(cx);
						object val = valueFunctionAndThis.Call(cx, scope, thisObj, cmpBuf);
						double num = ScriptConvert.ToNumber(val);
						result = (num > 0.0);
					}
				}
			}
			return result;
		}
		private static void heapsort(Context cx, IScriptable scope, object[] array, int length, object cmp, object[] cmpBuf)
		{
			if (length <= 1)
			{
				Context.CodeBug();
			}
			int num = length / 2;
			while (num != 0)
			{
				num--;
				object pivot = array[num];
				BuiltinArray.heapify(cx, scope, pivot, array, num, length, cmp, cmpBuf);
			}
			num = length;
			while (num != 1)
			{
				num--;
				object pivot = array[num];
				array[num] = array[0];
				BuiltinArray.heapify(cx, scope, pivot, array, 0, num, cmp, cmpBuf);
			}
		}
		private static void heapify(Context cx, IScriptable scope, object pivot, object[] array, int i, int end, object cmp, object[] cmpBuf)
		{
			while (true)
			{
				int num = i * 2 + 1;
				if (num >= end)
				{
					break;
				}
				object obj = array[num];
				if (num + 1 < end)
				{
					object obj2 = array[num + 1];
					if (BuiltinArray.IsBigger(cx, scope, obj2, obj, cmp, cmpBuf))
					{
						num++;
						obj = obj2;
					}
				}
				if (!BuiltinArray.IsBigger(cx, scope, obj, pivot, cmp, cmpBuf))
				{
					break;
				}
				array[i] = obj;
				i = num;
			}
			array[i] = pivot;
		}
		private static void heapsort_extended(Context cx, IScriptable scope, IScriptable target, long length, object cmp, object[] cmpBuf)
		{
			if (length <= 1L)
			{
				Context.CodeBug();
			}
			long num = length / 2L;
			while (num != 0L)
			{
				num -= 1L;
				object elem = BuiltinArray.getElem(cx, target, num);
				BuiltinArray.heapify_extended(cx, scope, elem, target, num, length, cmp, cmpBuf);
			}
			num = length;
			while (num != 1L)
			{
				num -= 1L;
				object elem = BuiltinArray.getElem(cx, target, num);
				BuiltinArray.setElem(cx, target, num, BuiltinArray.getElem(cx, target, 0L));
				BuiltinArray.heapify_extended(cx, scope, elem, target, 0L, num, cmp, cmpBuf);
			}
		}
		private static void heapify_extended(Context cx, IScriptable scope, object pivot, IScriptable target, long i, long end, object cmp, object[] cmpBuf)
		{
			while (true)
			{
				long num = i * 2L + 1L;
				if (num >= end)
				{
					break;
				}
				object obj = BuiltinArray.getElem(cx, target, num);
				if (num + 1L < end)
				{
					object elem = BuiltinArray.getElem(cx, target, num + 1L);
					if (BuiltinArray.IsBigger(cx, scope, elem, obj, cmp, cmpBuf))
					{
						num += 1L;
						obj = elem;
					}
				}
				if (!BuiltinArray.IsBigger(cx, scope, obj, pivot, cmp, cmpBuf))
				{
					break;
				}
				BuiltinArray.setElem(cx, target, i, obj);
				i = num;
			}
			BuiltinArray.setElem(cx, target, i, pivot);
		}
		private static object ImplPush(Context cx, IScriptable thisObj, object[] args)
		{
			long num = BuiltinArray.getLengthProperty(cx, thisObj);
			for (int i = 0; i < args.Length; i++)
			{
				BuiltinArray.setElem(cx, thisObj, num + (long)i, args[i]);
			}
			num += (long)args.Length;
			object obj = BuiltinArray.setLengthProperty(cx, thisObj, num);
			object result;
			if (cx.Version == Context.Versions.JS1_2)
			{
				result = ((args.Length == 0) ? Undefined.Value : args[args.Length - 1]);
			}
			else
			{
				result = obj;
			}
			return result;
		}
		private static object ImplPop(Context cx, IScriptable thisObj, object[] args)
		{
			long num = BuiltinArray.getLengthProperty(cx, thisObj);
			object result;
			if (num > 0L)
			{
				num -= 1L;
				result = BuiltinArray.getElem(cx, thisObj, num);
			}
			else
			{
				result = Undefined.Value;
			}
			BuiltinArray.setLengthProperty(cx, thisObj, num);
			return result;
		}
		private static object ImplShift(Context cx, IScriptable thisObj, object[] args)
		{
			long num = BuiltinArray.getLengthProperty(cx, thisObj);
			object result;
			if (num > 0L)
			{
				long num2 = 0L;
				num -= 1L;
				result = BuiltinArray.getElem(cx, thisObj, num2);
				if (num > 0L)
				{
					for (num2 = 1L; num2 <= num; num2 += 1L)
					{
						object elem = BuiltinArray.getElem(cx, thisObj, num2);
						BuiltinArray.setElem(cx, thisObj, num2 - 1L, elem);
					}
				}
			}
			else
			{
				result = Undefined.Value;
			}
			BuiltinArray.setLengthProperty(cx, thisObj, num);
			return result;
		}
		private static object ImplUnshift(Context cx, IScriptable thisObj, object[] args)
		{
			long num = BuiltinArray.getLengthProperty(cx, thisObj);
			int num2 = args.Length;
			BuiltinArray.VerifyOutOfRange(num + (long)args.Length);
			object result;
			if (args.Length > 0)
			{
				if (num > 0L)
				{
					for (long num3 = num - 1L; num3 >= 0L; num3 -= 1L)
					{
						object elem = BuiltinArray.getElem(cx, thisObj, num3);
						BuiltinArray.setElem(cx, thisObj, num3 + (long)num2, elem);
					}
				}
				for (int i = 0; i < args.Length; i++)
				{
					BuiltinArray.setElem(cx, thisObj, (long)i, args[i]);
				}
				num += (long)args.Length;
				result = BuiltinArray.setLengthProperty(cx, thisObj, num);
			}
			else
			{
				result = num;
			}
			return result;
		}
		private static object ImplSplice(Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			scope = ScriptableObject.GetTopLevelScope(scope);
			object obj = ScriptRuntime.NewObject(cx, scope, "Array", null);
			int num = args.Length;
			object result;
			if (num == 0)
			{
				result = obj;
			}
			else
			{
				long lengthProperty = BuiltinArray.getLengthProperty(cx, thisObj);
				long num2 = BuiltinArray.toSliceIndex(ScriptConvert.ToInteger(args[0]), lengthProperty);
				num--;
				long num3;
				if (args.Length == 1)
				{
					num3 = lengthProperty - num2;
				}
				else
				{
					double num4 = ScriptConvert.ToInteger(args[1]);
					if (num4 < 0.0)
					{
						num3 = 0L;
					}
					else
					{
						if (num4 > (double)(lengthProperty - num2))
						{
							num3 = lengthProperty - num2;
						}
						else
						{
							num3 = (long)num4;
						}
					}
					num--;
				}
				long num5 = num2 + num3;
				if (num3 != 0L)
				{
					if (num3 == 1L && cx.Version == Context.Versions.JS1_2)
					{
						obj = BuiltinArray.getElem(cx, thisObj, num2);
					}
					else
					{
						for (long num6 = num2; num6 != num5; num6 += 1L)
						{
							IScriptable target = (IScriptable)obj;
							object elem = BuiltinArray.getElem(cx, thisObj, num6);
							BuiltinArray.setElem(cx, target, num6 - num2, elem);
						}
					}
				}
				else
				{
					if (num3 == 0L && cx.Version == Context.Versions.JS1_2)
					{
						obj = Undefined.Value;
					}
				}
				long num7 = (long)num - num3;
				BuiltinArray.VerifyOutOfRange(lengthProperty + num7);
				if (num7 > 0L)
				{
					for (long num6 = lengthProperty - 1L; num6 >= num5; num6 -= 1L)
					{
						object elem = BuiltinArray.getElem(cx, thisObj, num6);
						BuiltinArray.setElem(cx, thisObj, num6 + num7, elem);
					}
				}
				else
				{
					if (num7 < 0L)
					{
						for (long num6 = num5; num6 < lengthProperty; num6 += 1L)
						{
							object elem = BuiltinArray.getElem(cx, thisObj, num6);
							BuiltinArray.setElem(cx, thisObj, num6 + num7, elem);
						}
					}
				}
				int num8 = args.Length - num;
				for (int i = 0; i < num; i++)
				{
					BuiltinArray.setElem(cx, thisObj, num2 + (long)i, args[i + num8]);
				}
				BuiltinArray.setLengthProperty(cx, thisObj, lengthProperty + num7);
				result = obj;
			}
			return result;
		}
		private static IScriptable ImplConcat(Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			scope = ScriptableObject.GetTopLevelScope(scope);
			IFunction existingCtor = ScriptRuntime.getExistingCtor(cx, scope, "Array");
			IScriptable scriptable = existingCtor.Construct(cx, scope, ScriptRuntime.EmptyArgs);
			long num = 0L;
			long lengthProperty = BuiltinArray.getLengthProperty(cx, thisObj);
			long num2 = 0L;
			for (int i = 0; i < args.Length; i++)
			{
				if (ScriptRuntime.InstanceOf(args[i], existingCtor, cx))
				{
					IScriptable scriptable2 = (IScriptable)args[i];
					long lengthProperty2 = BuiltinArray.getLengthProperty(cx, scriptable2);
					num2 += lengthProperty2;
				}
				else
				{
					num2 += 1L;
				}
			}
			BuiltinArray.VerifyOutOfRange(lengthProperty + num2);
			if (ScriptRuntime.InstanceOf(thisObj, existingCtor, cx))
			{
				long lengthProperty2 = BuiltinArray.getLengthProperty(cx, thisObj);
				for (num = 0L; num < lengthProperty2; num += 1L)
				{
					object elem = BuiltinArray.getElem(cx, thisObj, num);
					BuiltinArray.setElem(cx, scriptable, num, elem);
				}
			}
			else
			{
				IScriptable arg_FA_1 = scriptable;
				long expr_F4 = num;
				num = expr_F4 + 1L;
				BuiltinArray.setElem(cx, arg_FA_1, expr_F4, thisObj);
			}
			for (int i = 0; i < args.Length; i++)
			{
				if (ScriptRuntime.InstanceOf(args[i], existingCtor, cx))
				{
					IScriptable scriptable2 = (IScriptable)args[i];
					long lengthProperty2 = BuiltinArray.getLengthProperty(cx, scriptable2);
					long num3 = 0L;
					while (num3 < lengthProperty2)
					{
						object elem = BuiltinArray.getElem(cx, scriptable2, num3);
						BuiltinArray.setElem(cx, scriptable, num, elem);
						num3 += 1L;
						num += 1L;
					}
				}
				else
				{
					IScriptable arg_184_1 = scriptable;
					long expr_17B = num;
					num = expr_17B + 1L;
					BuiltinArray.setElem(cx, arg_184_1, expr_17B, args[i]);
				}
			}
			return scriptable;
		}
		private IScriptable ImplSlice(Context cx, IScriptable thisObj, object[] args)
		{
			IScriptable topLevelScope = ScriptableObject.GetTopLevelScope(this);
			IScriptable scriptable = ScriptRuntime.NewObject(cx, topLevelScope, "Array", null);
			long lengthProperty = BuiltinArray.getLengthProperty(cx, thisObj);
			long num;
			long num2;
			if (args.Length == 0)
			{
				num = 0L;
				num2 = lengthProperty;
			}
			else
			{
				num = BuiltinArray.toSliceIndex(ScriptConvert.ToInteger(args[0]), lengthProperty);
				if (args.Length == 1)
				{
					num2 = lengthProperty;
				}
				else
				{
					num2 = BuiltinArray.toSliceIndex(ScriptConvert.ToInteger(args[1]), lengthProperty);
				}
			}
			for (long num3 = num; num3 < num2; num3 += 1L)
			{
				object elem = BuiltinArray.getElem(cx, thisObj, num3);
				BuiltinArray.setElem(cx, scriptable, num3 - num, elem);
			}
			return scriptable;
		}
		private static long toSliceIndex(double value, long length)
		{
			long result;
			if (value < 0.0)
			{
				if (value + (double)length < 0.0)
				{
					result = 0L;
				}
				else
				{
					result = (long)(value + (double)length);
				}
			}
			else
			{
				if (value > (double)length)
				{
					result = length;
				}
				else
				{
					result = (long)value;
				}
			}
			return result;
		}
		protected internal override int FindPrototypeId(string s)
		{
			int result = 0;
			string text = null;
			switch (s.Length)
			{
			case 3:
				text = "pop";
				result = 9;
				break;
			case 4:
			{
				int num = (int)s[0];
				if (num == 106)
				{
					text = "join";
					result = 5;
				}
				else
				{
					if (num == 112)
					{
						text = "push";
						result = 8;
					}
					else
					{
						if (num == 115)
						{
							text = "sort";
							result = 7;
						}
					}
				}
				break;
			}
			case 5:
			{
				int num = (int)s[1];
				if (num == 104)
				{
					text = "shift";
					result = 10;
				}
				else
				{
					if (num == 108)
					{
						text = "slice";
						result = 14;
					}
				}
				break;
			}
			case 6:
			{
				int num = (int)s[0];
				if (num == 99)
				{
					text = "concat";
					result = 13;
				}
				else
				{
					if (num == 115)
					{
						text = "splice";
						result = 12;
					}
				}
				break;
			}
			case 7:
			{
				int num = (int)s[0];
				if (num == 114)
				{
					text = "reverse";
					result = 6;
				}
				else
				{
					if (num == 117)
					{
						text = "unshift";
						result = 11;
					}
				}
				break;
			}
			case 8:
			{
				int num = (int)s[3];
				if (num == 111)
				{
					text = "toSource";
					result = 4;
				}
				else
				{
					if (num == 116)
					{
						text = "toString";
						result = 2;
					}
				}
				break;
			}
			case 11:
				text = "constructor";
				result = 1;
				break;
			case 14:
				text = "toLocaleString";
				result = 3;
				break;
			}
			if (text != null && text != s && !text.Equals(s))
			{
				result = 0;
			}
			return result;
		}
	}
}
