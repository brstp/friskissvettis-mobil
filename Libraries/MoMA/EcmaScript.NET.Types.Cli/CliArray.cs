using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET.Types.Cli
{
	[ComVisible(true)]
	public class CliArray : CliObject
	{
		internal object array;
		internal int length;
		internal Type cls;
		internal IScriptable prototype;
		public override string ClassName
		{
			get
			{
				return "NativeCliArray";
			}
		}
		public CliArray(IScriptable scope, object array) : base(array)
		{
			Type type = array.GetType();
			if (!type.IsArray)
			{
				throw new ApplicationException("Array expected");
			}
			this.array = array;
			this.length = ((Array)array).Length;
			this.cls = type.GetElementType();
		}
		public override bool Has(string id, IScriptable start)
		{
			return id.Equals("length") || base.Has(id, start);
		}
		public override bool Has(int index, IScriptable start)
		{
			return 0 <= index && index < this.length;
		}
		public override object Get(string id, IScriptable start)
		{
			object result;
			if (id.Equals("length"))
			{
				result = this.length;
			}
			else
			{
				object obj = base.Get(id, start);
				if (obj == UniqueTag.NotFound && !ScriptableObject.HasProperty(this.GetPrototype(), id))
				{
					throw Context.ReportRuntimeErrorById("msg.java.member.not.found", new object[]
					{
						this.array.GetType().FullName,
						id
					});
				}
				result = obj;
			}
			return result;
		}
		public override object Get(int index, IScriptable start)
		{
			object result;
			if (0 <= index && index < this.length)
			{
				object value = ((Array)this.array).GetValue(index);
				result = Context.CurrentContext.Wrap(this, value, this.cls);
			}
			else
			{
				result = Undefined.Value;
			}
			return result;
		}
		public override object Put(string id, IScriptable start, object value)
		{
			object result;
			if (!id.Equals("length"))
			{
				result = base.Put(id, start, value);
			}
			else
			{
				result = Undefined.Value;
			}
			return result;
		}
		public override object Put(int index, IScriptable start, object value)
		{
			object result;
			if (0 <= index && index < this.length)
			{
				((Array)this.array).SetValue(Context.JsToCli(value, this.cls), index);
				result = value;
			}
			else
			{
				result = base.Put(index, start, value);
			}
			return result;
		}
		public override object GetDefaultValue(Type hint)
		{
			object result;
			if (hint == null || hint == typeof(string))
			{
				result = this.array.ToString();
			}
			else
			{
				if (hint == typeof(bool))
				{
					result = true;
				}
				else
				{
					if (CliHelper.IsNumberType(hint))
					{
						result = double.NaN;
					}
					else
					{
						result = this;
					}
				}
			}
			return result;
		}
		public override object[] GetIds()
		{
			object[] array = new object[this.length];
			int num = this.length;
			while (--num >= 0)
			{
				array[num] = num;
			}
			return array;
		}
		public override bool HasInstance(IScriptable value)
		{
			bool result;
			if (!(value is Wrapper))
			{
				result = false;
			}
			else
			{
				object o = ((Wrapper)value).Unwrap();
				result = this.cls.IsInstanceOfType(o);
			}
			return result;
		}
		public override IScriptable GetPrototype()
		{
			if (this.prototype == null)
			{
				this.prototype = ScriptableObject.getClassPrototype(base.ParentScope, "Array");
			}
			return this.prototype;
		}
	}
}
