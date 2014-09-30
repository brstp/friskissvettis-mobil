using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET.Types
{
	[ComVisible(true)]
	public class BuiltinWith : IScriptable, IIdFunctionCall
	{
		private const int Id_constructor = 1;
		private static readonly object FTAG = new object();
		protected internal IScriptable prototype;
		protected internal IScriptable parent;
		public virtual string ClassName
		{
			get
			{
				return "With";
			}
		}
		public virtual IScriptable ParentScope
		{
			get
			{
				return this.parent;
			}
			set
			{
				this.parent = value;
			}
		}
		internal static void Init(IScriptable scope, bool zealed)
		{
			BuiltinWith builtinWith = new BuiltinWith();
			builtinWith.ParentScope = scope;
			builtinWith.SetPrototype(ScriptableObject.GetObjectPrototype(scope));
			IdFunctionObject idFunctionObject = new IdFunctionObject(builtinWith, BuiltinWith.FTAG, 1, "With", 0, scope);
			idFunctionObject.MarkAsConstructor(builtinWith);
			if (zealed)
			{
				idFunctionObject.SealObject();
			}
			idFunctionObject.ExportAsScopeProperty();
		}
		private BuiltinWith()
		{
		}
		internal BuiltinWith(IScriptable parent, IScriptable prototype)
		{
			this.parent = parent;
			this.prototype = prototype;
		}
		public virtual bool Has(string id, IScriptable start)
		{
			return this.prototype.Has(id, this.prototype);
		}
		public virtual bool Has(int index, IScriptable start)
		{
			return this.prototype.Has(index, this.prototype);
		}
		public virtual object Get(string id, IScriptable start)
		{
			if (start == this)
			{
				start = this.prototype;
			}
			return this.prototype.Get(id, start);
		}
		public virtual object Get(int index, IScriptable start)
		{
			if (start == this)
			{
				start = this.prototype;
			}
			return this.prototype.Get(index, start);
		}
		public virtual object Put(string id, IScriptable start, object value)
		{
			if (start == this)
			{
				start = this.prototype;
			}
			return this.prototype.Put(id, start, value);
		}
		public virtual object Put(int index, IScriptable start, object value)
		{
			if (start == this)
			{
				start = this.prototype;
			}
			return this.prototype.Put(index, start, value);
		}
		public virtual void Delete(string id)
		{
			this.prototype.Delete(id);
		}
		public virtual void Delete(int index)
		{
			this.prototype.Delete(index);
		}
		public virtual IScriptable GetPrototype()
		{
			return this.prototype;
		}
		public virtual void SetPrototype(IScriptable prototype)
		{
			this.prototype = prototype;
		}
		public virtual object[] GetIds()
		{
			return this.prototype.GetIds();
		}
		public virtual object GetDefaultValue(Type typeHint)
		{
			return this.prototype.GetDefaultValue(typeHint);
		}
		public virtual bool HasInstance(IScriptable value)
		{
			return this.prototype.HasInstance(value);
		}
		protected internal virtual object UpdateDotQuery(bool value)
		{
			throw new ApplicationException();
		}
		public virtual object ExecIdCall(IdFunctionObject f, Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			if (f.HasTag(BuiltinWith.FTAG))
			{
				if (f.MethodId == 1)
				{
					throw Context.ReportRuntimeErrorById("msg.cant.call.indirect", new object[]
					{
						"With"
					});
				}
			}
			throw f.Unknown();
		}
		internal static bool IsWithFunction(object functionObj)
		{
			bool result;
			if (functionObj is IdFunctionObject)
			{
				IdFunctionObject idFunctionObject = (IdFunctionObject)functionObj;
				result = (idFunctionObject.HasTag(BuiltinWith.FTAG) && idFunctionObject.MethodId == 1);
			}
			else
			{
				result = false;
			}
			return result;
		}
		internal static object NewWithSpecial(Context cx, IScriptable scope, object[] args)
		{
			ScriptRuntime.checkDeprecated(cx, "With");
			scope = ScriptableObject.GetTopLevelScope(scope);
			BuiltinWith builtinWith = new BuiltinWith();
			builtinWith.SetPrototype((args.Length == 0) ? ScriptableObject.getClassPrototype(scope, "Object") : ScriptConvert.ToObject(cx, scope, args[0]));
			builtinWith.ParentScope = scope;
			return builtinWith;
		}
	}
}
