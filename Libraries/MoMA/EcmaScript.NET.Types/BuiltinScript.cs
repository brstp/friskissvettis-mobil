using System;
namespace EcmaScript.NET.Types
{
	internal class BuiltinScript : BaseFunction
	{
		private const int Id_constructor = 1;
		private const int Id_toString = 2;
		private const int Id_compile = 3;
		private const int Id_exec = 4;
		private const int MAX_PROTOTYPE_ID = 4;
		private static readonly object SCRIPT_TAG = new object();
		private IScript script;
		public override string ClassName
		{
			get
			{
				return "Script";
			}
		}
		public override int Length
		{
			get
			{
				return 0;
			}
		}
		public override int Arity
		{
			get
			{
				return 0;
			}
		}
		internal new static void Init(IScriptable scope, bool zealed)
		{
			BuiltinScript builtinScript = new BuiltinScript(null);
			builtinScript.ExportAsJSClass(4, scope, zealed);
		}
		private BuiltinScript(IScript script)
		{
			this.script = script;
		}
		public override object Call(Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			object result;
			if (this.script != null)
			{
				result = this.script.Exec(cx, scope);
			}
			else
			{
				result = Undefined.Value;
			}
			return result;
		}
		public override IScriptable Construct(Context cx, IScriptable scope, object[] args)
		{
			throw Context.ReportRuntimeErrorById("msg.script.is.not.constructor", new object[0]);
		}
		internal override string Decompile(int indent, int flags)
		{
			string result;
			if (this.script is BuiltinFunction)
			{
				result = ((BuiltinFunction)this.script).Decompile(indent, flags);
			}
			else
			{
				result = base.Decompile(indent, flags);
			}
			return result;
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
				name = "compile";
				break;
			case 4:
				arity = 0;
				name = "exec";
				break;
			default:
				throw new ArgumentException(Convert.ToString(id));
			}
			base.InitPrototypeMethod(BuiltinScript.SCRIPT_TAG, id, name, arity);
		}
		public override object ExecIdCall(IdFunctionObject f, Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			object result;
			if (!f.HasTag(BuiltinScript.SCRIPT_TAG))
			{
				result = base.ExecIdCall(f, cx, scope, thisObj, args);
			}
			else
			{
				int methodId = f.MethodId;
				switch (methodId)
				{
				case 1:
				{
					string source = (args.Length == 0) ? "" : ScriptConvert.ToString(args[0]);
					IScript script = BuiltinScript.compile(cx, source);
					BuiltinScript builtinScript = new BuiltinScript(script);
					ScriptRuntime.setObjectProtoAndParent(builtinScript, scope);
					result = builtinScript;
					break;
				}
				case 2:
				{
					BuiltinScript builtinScript2 = BuiltinScript.realThis(thisObj, f);
					IScript script2 = builtinScript2.script;
					if (script2 == null)
					{
						result = "";
					}
					else
					{
						result = cx.DecompileScript(script2, 0);
					}
					break;
				}
				case 3:
				{
					BuiltinScript builtinScript2 = BuiltinScript.realThis(thisObj, f);
					string source = ScriptConvert.ToString(args, 0);
					builtinScript2.script = BuiltinScript.compile(cx, source);
					result = builtinScript2;
					break;
				}
				case 4:
					throw Context.ReportRuntimeErrorById("msg.cant.call.indirect", new object[]
					{
						"exec"
					});
				default:
					throw new ArgumentException(Convert.ToString(methodId));
				}
			}
			return result;
		}
		private static BuiltinScript realThis(IScriptable thisObj, IdFunctionObject f)
		{
			if (!(thisObj is BuiltinScript))
			{
				throw IdScriptableObject.IncompatibleCallError(f);
			}
			return (BuiltinScript)thisObj;
		}
		private static IScript compile(Context cx, string source)
		{
			int[] array = new int[1];
			int[] array2 = array;
			string text = Context.GetSourcePositionFromStack(array2);
			if (text == null)
			{
				text = "<Script object>";
				array2[0] = 1;
			}
			ErrorReporter compilationErrorReporter = DefaultErrorReporter.ForEval(cx.ErrorReporter);
			return cx.CompileString(source, null, compilationErrorReporter, text, array2[0], null);
		}
		protected internal override int FindPrototypeId(string s)
		{
			int result = 0;
			string text = null;
			int length = s.Length;
			switch (length)
			{
			case 4:
				text = "exec";
				result = 4;
				break;
			case 5:
			case 6:
				break;
			case 7:
				text = "compile";
				result = 3;
				break;
			case 8:
				text = "toString";
				result = 2;
				break;
			default:
				if (length == 11)
				{
					text = "constructor";
					result = 1;
				}
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
