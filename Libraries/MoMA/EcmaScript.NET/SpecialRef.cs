using System;
namespace EcmaScript.NET
{
	internal class SpecialRef : IRef
	{
		public enum Types
		{
			None,
			Proto,
			Parent
		}
		private IScriptable target;
		private SpecialRef.Types type;
		private string name;
		private SpecialRef(IScriptable target, SpecialRef.Types type, string name)
		{
			this.target = target;
			this.type = type;
			this.name = name;
		}
		internal static IRef createSpecial(Context cx, object obj, string name)
		{
			IScriptable scriptable = ScriptConvert.ToObjectOrNull(cx, obj);
			if (scriptable == null)
			{
				throw ScriptRuntime.UndefReadError(obj, name);
			}
			SpecialRef.Types types;
			if (name.Equals("__proto__"))
			{
				types = SpecialRef.Types.Proto;
			}
			else
			{
				if (!name.Equals("__parent__"))
				{
					throw new ArgumentException(name);
				}
				types = SpecialRef.Types.Parent;
			}
			if (!cx.HasFeature(Context.Features.ParentProtoProperties))
			{
				types = SpecialRef.Types.None;
			}
			return new SpecialRef(scriptable, types, name);
		}
		public object Get(Context cx)
		{
			object result;
			switch (this.type)
			{
			case SpecialRef.Types.None:
				result = ScriptRuntime.getObjectProp(this.target, this.name, cx);
				break;
			case SpecialRef.Types.Proto:
				result = this.target.GetPrototype();
				break;
			case SpecialRef.Types.Parent:
				result = this.target.ParentScope;
				break;
			default:
				throw Context.CodeBug();
			}
			return result;
		}
		public object Set(Context cx, object value)
		{
			object result;
			switch (this.type)
			{
			case SpecialRef.Types.None:
				result = ScriptRuntime.setObjectProp(this.target, this.name, value, cx);
				break;
			case SpecialRef.Types.Proto:
			case SpecialRef.Types.Parent:
			{
				IScriptable scriptable = ScriptConvert.ToObjectOrNull(cx, value);
				if (scriptable != null)
				{
					IScriptable scriptable2 = scriptable;
					while (scriptable2 != this.target)
					{
						if (this.type == SpecialRef.Types.Proto)
						{
							scriptable2 = scriptable2.GetPrototype();
						}
						else
						{
							scriptable2 = scriptable2.ParentScope;
						}
						if (scriptable2 == null)
						{
							goto IL_C1;
						}
					}
					throw Context.ReportRuntimeErrorById("msg.cyclic.value", new object[]
					{
						this.name
					});
				}
				IL_C1:
				if (this.type == SpecialRef.Types.Proto)
				{
					this.target.SetPrototype(scriptable);
				}
				else
				{
					this.target.ParentScope = scriptable;
				}
				result = scriptable;
				break;
			}
			default:
				throw Context.CodeBug();
			}
			return result;
		}
		public bool Has(Context cx)
		{
			return this.type != SpecialRef.Types.None || ScriptRuntime.hasObjectElem(this.target, this.name, cx);
		}
		public bool Delete(Context cx)
		{
			return this.type == SpecialRef.Types.None && ScriptRuntime.deleteObjectElem(this.target, this.name, cx);
		}
	}
}
