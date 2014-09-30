using EcmaScript.NET.Types;
using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public class Delegator : IFunction, IScriptable, ICallable
	{
		protected internal IScriptable obj = null;
		public virtual IScriptable Delegee
		{
			get
			{
				return this.obj;
			}
			set
			{
				this.obj = value;
			}
		}
		public virtual string ClassName
		{
			get
			{
				return this.obj.ClassName;
			}
		}
		public virtual IScriptable ParentScope
		{
			get
			{
				return this.obj.ParentScope;
			}
			set
			{
				this.obj.ParentScope = value;
			}
		}
		public Delegator()
		{
		}
		public Delegator(IScriptable obj)
		{
			this.obj = obj;
		}
		protected internal virtual Delegator NewInstance()
		{
			Delegator result;
			try
			{
				result = (Delegator)Activator.CreateInstance(base.GetType());
			}
			catch (Exception e)
			{
				throw Context.ThrowAsScriptRuntimeEx(e);
			}
			return result;
		}
		public virtual object Get(string name, IScriptable start)
		{
			return this.obj.Get(name, start);
		}
		public virtual object Get(int index, IScriptable start)
		{
			return this.obj.Get(index, start);
		}
		public virtual bool Has(string name, IScriptable start)
		{
			return this.obj.Has(name, start);
		}
		public virtual bool Has(int index, IScriptable start)
		{
			return this.obj.Has(index, start);
		}
		public virtual object Put(string name, IScriptable start, object value)
		{
			return this.obj.Put(name, start, value);
		}
		public virtual object Put(int index, IScriptable start, object value)
		{
			return this.obj.Put(index, start, value);
		}
		public virtual void Delete(string name)
		{
			this.obj.Delete(name);
		}
		public virtual void Delete(int index)
		{
			this.obj.Delete(index);
		}
		public virtual IScriptable GetPrototype()
		{
			return this.obj.GetPrototype();
		}
		public virtual void SetPrototype(IScriptable prototype)
		{
			this.obj.SetPrototype(prototype);
		}
		public virtual object[] GetIds()
		{
			return this.obj.GetIds();
		}
		public virtual object GetDefaultValue(Type hint)
		{
			return (hint == null || hint == typeof(IScriptable) || hint == typeof(IFunction)) ? this : this.obj.GetDefaultValue(hint);
		}
		public virtual bool HasInstance(IScriptable instance)
		{
			return this.obj.HasInstance(instance);
		}
		public virtual object Call(Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			return ((IFunction)this.obj).Call(cx, scope, thisObj, args);
		}
		public virtual IScriptable Construct(Context cx, IScriptable scope, object[] args)
		{
			IScriptable result;
			if (this.obj == null)
			{
				Delegator delegator = this.NewInstance();
				IScriptable delegee;
				if (args.Length == 0)
				{
					delegee = new BuiltinObject();
				}
				else
				{
					delegee = ScriptConvert.ToObject(cx, scope, args[0]);
				}
				delegator.Delegee = delegee;
				result = delegator;
			}
			else
			{
				result = ((IFunction)this.obj).Construct(cx, scope, args);
			}
			return result;
		}
	}
}
