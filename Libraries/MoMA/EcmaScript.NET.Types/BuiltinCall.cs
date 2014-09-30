using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET.Types
{
	[ComVisible(true)]
	public sealed class BuiltinCall : IdScriptableObject
	{
		private const int Id_constructor = 1;
		private const int MAX_PROTOTYPE_ID = 1;
		private static readonly object CALL_TAG = new object();
		internal BuiltinFunction function;
		internal object[] originalArgs;
		internal BuiltinCall parentActivationCall;
		public override string ClassName
		{
			get
			{
				return "Call";
			}
		}
		internal static void Init(IScriptable scope, bool zealed)
		{
			BuiltinCall builtinCall = new BuiltinCall();
			builtinCall.ExportAsJSClass(1, scope, zealed);
		}
		internal BuiltinCall()
		{
		}
		internal BuiltinCall(BuiltinFunction function, IScriptable scope, object[] args)
		{
			this.function = function;
			base.ParentScope = scope;
			this.originalArgs = ((args == null) ? ScriptRuntime.EmptyArgs : args);
			int paramAndVarCount = function.ParamAndVarCount;
			int paramCount = function.ParamCount;
			if (paramAndVarCount != 0)
			{
				for (int num = 0; num != paramCount; num++)
				{
					string paramOrVarName = function.getParamOrVarName(num);
					object value = (num < args.Length) ? args[num] : Undefined.Value;
					this.DefineProperty(paramOrVarName, value, 4);
				}
			}
			if (!base.Has("arguments", this))
			{
				this.DefineProperty("arguments", new Arguments(this), 4);
			}
			if (paramAndVarCount != 0)
			{
				for (int num = paramCount; num != paramAndVarCount; num++)
				{
					string paramOrVarName = function.getParamOrVarName(num);
					if (!base.Has(paramOrVarName, this))
					{
						this.DefineProperty(paramOrVarName, Undefined.Value, 4);
					}
				}
			}
		}
		protected internal override int FindPrototypeId(string s)
		{
			return s.Equals("constructor") ? 1 : 0;
		}
		protected internal override void InitPrototypeId(int id)
		{
			if (id == 1)
			{
				int arity = 1;
				string name = "constructor";
				base.InitPrototypeMethod(BuiltinCall.CALL_TAG, id, name, arity);
				return;
			}
			throw new ArgumentException(Convert.ToString(id));
		}
		public override object ExecIdCall(IdFunctionObject f, Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			object result;
			if (!f.HasTag(BuiltinCall.CALL_TAG))
			{
				result = base.ExecIdCall(f, cx, scope, thisObj, args);
			}
			else
			{
				int methodId = f.MethodId;
				if (methodId != 1)
				{
					throw new ArgumentException(Convert.ToString(methodId));
				}
				if (thisObj != null)
				{
					throw Context.ReportRuntimeErrorById("msg.only.from.new", new object[]
					{
						"Call"
					});
				}
				ScriptRuntime.checkDeprecated(cx, "Call");
				BuiltinCall builtinCall = new BuiltinCall();
				builtinCall.SetPrototype(ScriptableObject.GetObjectPrototype(scope));
				result = builtinCall;
			}
			return result;
		}
	}
}
