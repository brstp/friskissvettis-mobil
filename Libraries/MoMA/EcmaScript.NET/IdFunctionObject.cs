using System;
using System.Runtime.InteropServices;
using System.Text;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public class IdFunctionObject : BaseFunction
	{
		private IIdFunctionCall idcall;
		private object tag;
		private int m_MethodId;
		private int arity;
		private bool useCallAsConstructor;
		private string functionName;
		public override int Arity
		{
			get
			{
				return this.arity;
			}
		}
		public override int Length
		{
			get
			{
				return this.Arity;
			}
		}
		public override string FunctionName
		{
			get
			{
				return (this.functionName == null) ? "" : this.functionName;
			}
		}
		public int MethodId
		{
			get
			{
				return this.m_MethodId;
			}
		}
		public IdFunctionObject()
		{
		}
		public IdFunctionObject(IIdFunctionCall idcall, object tag, int id, int arity)
		{
			if (arity < 0)
			{
				throw new ArgumentException();
			}
			this.idcall = idcall;
			this.tag = tag;
			this.m_MethodId = id;
			this.arity = arity;
			if (arity < 0)
			{
				throw new ArgumentException();
			}
		}
		public IdFunctionObject(IIdFunctionCall idcall, object tag, int id, string name, int arity, IScriptable scope) : base(scope, null)
		{
			if (arity < 0)
			{
				throw new ArgumentException();
			}
			if (name == null)
			{
				throw new ArgumentException();
			}
			this.idcall = idcall;
			this.tag = tag;
			this.m_MethodId = id;
			this.arity = arity;
			this.functionName = name;
		}
		public virtual void InitFunction(string name, IScriptable scope)
		{
			if (name == null)
			{
				throw new ArgumentException();
			}
			if (scope == null)
			{
				throw new ArgumentException();
			}
			this.functionName = name;
			base.ParentScope = scope;
		}
		public bool HasTag(object tag)
		{
			return this.tag == tag;
		}
		public void MarkAsConstructor(IScriptable prototypeProperty)
		{
			this.useCallAsConstructor = true;
			this.ImmunePrototypeProperty = prototypeProperty;
		}
		public void AddAsProperty(IScriptable target)
		{
			this.AddAsProperty(target, 2);
		}
		public void AddAsProperty(IScriptable target, int attributes)
		{
			ScriptableObject.DefineProperty(target, this.functionName, this, attributes);
		}
		public virtual void ExportAsScopeProperty()
		{
			this.AddAsProperty(base.ParentScope);
		}
		public virtual void ExportAsScopeProperty(int attributes)
		{
			this.AddAsProperty(base.ParentScope, attributes);
		}
		public override IScriptable GetPrototype()
		{
			IScriptable scriptable = base.GetPrototype();
			if (scriptable == null)
			{
				scriptable = ScriptableObject.GetFunctionPrototype(base.ParentScope);
				this.SetPrototype(scriptable);
			}
			return scriptable;
		}
		public override object Call(Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			return this.idcall.ExecIdCall(this, cx, scope, thisObj, args);
		}
		public override IScriptable CreateObject(Context cx, IScriptable scope)
		{
			if (this.useCallAsConstructor)
			{
				return null;
			}
			throw ScriptRuntime.TypeErrorById("msg.not.ctor", new string[]
			{
				this.functionName
			});
		}
		internal override string Decompile(int indent, int flags)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = 0 != (flags & 1);
			if (!flag)
			{
				stringBuilder.Append("function ");
				stringBuilder.Append(this.FunctionName);
				stringBuilder.Append("() { ");
			}
			stringBuilder.Append("[native code for ");
			if (this.idcall is IScriptable)
			{
				IScriptable scriptable = (IScriptable)this.idcall;
				stringBuilder.Append(scriptable.ClassName);
				stringBuilder.Append('.');
			}
			stringBuilder.Append(this.FunctionName);
			stringBuilder.Append(", arity=");
			stringBuilder.Append(this.Arity);
			stringBuilder.Append(flag ? "]\n" : "] }\n");
			return stringBuilder.ToString();
		}
		public ApplicationException Unknown()
		{
			return new ApplicationException(string.Concat(new object[]
			{
				"BAD FUNCTION ID=",
				this.m_MethodId,
				" MASTER=",
				this.idcall
			}));
		}
	}
}
