using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public sealed class Continuation : IdScriptableObject, IFunction, IScriptable, ICallable
	{
		private const int Id_constructor = 1;
		private const int MAX_PROTOTYPE_ID = 1;
		private static readonly object FTAG = new object();
		private object implementation;
		public object Implementation
		{
			get
			{
				return this.implementation;
			}
		}
		public override string ClassName
		{
			get
			{
				return "Continuation";
			}
		}
		public static void Init(IScriptable scope, bool zealed)
		{
			Continuation continuation = new Continuation();
			continuation.ExportAsJSClass(1, scope, zealed);
		}
		public void initImplementation(object implementation)
		{
			this.implementation = implementation;
		}
		public IScriptable Construct(Context cx, IScriptable scope, object[] args)
		{
			throw Context.ReportRuntimeError("Direct call is not supported");
		}
		public object Call(Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			return Interpreter.restartContinuation(this, cx, scope, args);
		}
		public static bool IsContinuationConstructor(IdFunctionObject f)
		{
			return f.HasTag(Continuation.FTAG) && f.MethodId == 1;
		}
		protected internal override void InitPrototypeId(int id)
		{
			if (id != 1)
			{
				throw new ArgumentException(Convert.ToString(id));
			}
			int arity = 0;
			string name = "constructor";
			base.InitPrototypeMethod(Continuation.FTAG, id, name, arity);
		}
		public override object ExecIdCall(IdFunctionObject f, Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			if (!f.HasTag(Continuation.FTAG))
			{
				return base.ExecIdCall(f, cx, scope, thisObj, args);
			}
			int methodId = f.MethodId;
			int num = methodId;
			if (num != 1)
			{
				throw new ArgumentException(Convert.ToString(methodId));
			}
			throw Context.ReportRuntimeError("Direct call is not supported");
		}
		protected internal override int FindPrototypeId(string s)
		{
			int result = 0;
			string text = null;
			if (s.Length == 11)
			{
				text = "constructor";
				result = 1;
			}
			if (text != null && text != s && !text.Equals(s))
			{
				result = 0;
			}
			return result;
		}
	}
}
