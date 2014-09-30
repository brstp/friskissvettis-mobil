using EcmaScript.NET.Debugging;
using EcmaScript.NET.Types;
using System;
namespace EcmaScript.NET
{
	internal sealed class InterpretedFunction : BuiltinFunction, IScript
	{
		internal InterpreterData idata;
		internal SecurityController securityController;
		internal object securityDomain;
		internal IScriptable[] functionRegExps;
		public override string FunctionName
		{
			get
			{
				return (this.idata.itsName == null) ? "" : this.idata.itsName;
			}
		}
		public override string EncodedSource
		{
			get
			{
				return Interpreter.GetEncodedSource(this.idata);
			}
		}
		public override DebuggableScript DebuggableView
		{
			get
			{
				return this.idata;
			}
		}
		protected internal override Context.Versions LanguageVersion
		{
			get
			{
				return this.idata.languageVersion;
			}
		}
		protected internal override int ParamCount
		{
			get
			{
				return this.idata.argCount;
			}
		}
		protected internal override int ParamAndVarCount
		{
			get
			{
				return this.idata.argNames.Length;
			}
		}
		private InterpretedFunction(InterpreterData idata, object staticSecurityDomain)
		{
			this.idata = idata;
			Context currentContext = Context.CurrentContext;
			SecurityController securityController = currentContext.SecurityController;
			object obj;
			if (securityController != null)
			{
				obj = securityController.getDynamicSecurityDomain(staticSecurityDomain);
			}
			else
			{
				if (staticSecurityDomain != null)
				{
					throw new ArgumentException();
				}
				obj = null;
			}
			this.securityController = securityController;
			this.securityDomain = obj;
		}
		private InterpretedFunction(InterpretedFunction parent, int index)
		{
			this.idata = parent.idata.itsNestedFunctions[index];
			this.securityController = parent.securityController;
			this.securityDomain = parent.securityDomain;
		}
		internal static InterpretedFunction createScript(InterpreterData idata, object staticSecurityDomain)
		{
			return new InterpretedFunction(idata, staticSecurityDomain);
		}
		internal static InterpretedFunction createFunction(Context cx, IScriptable scope, InterpreterData idata, object staticSecurityDomain)
		{
			InterpretedFunction interpretedFunction = new InterpretedFunction(idata, staticSecurityDomain);
			interpretedFunction.initInterpretedFunction(cx, scope);
			return interpretedFunction;
		}
		internal static InterpretedFunction createFunction(Context cx, IScriptable scope, InterpretedFunction parent, int index)
		{
			InterpretedFunction interpretedFunction = new InterpretedFunction(parent, index);
			interpretedFunction.initInterpretedFunction(cx, scope);
			return interpretedFunction;
		}
		internal IScriptable[] createRegExpWraps(Context cx, IScriptable scope)
		{
			if (this.idata.itsRegExpLiterals == null)
			{
				Context.CodeBug();
			}
			RegExpProxy regExpProxy = cx.RegExpProxy;
			int num = this.idata.itsRegExpLiterals.Length;
			IScriptable[] array = new IScriptable[num];
			for (int num2 = 0; num2 != num; num2++)
			{
				array[num2] = regExpProxy.Wrap(cx, scope, this.idata.itsRegExpLiterals[num2]);
			}
			return array;
		}
		private void initInterpretedFunction(Context cx, IScriptable scope)
		{
			base.initScriptFunction(cx, scope);
			if (this.idata.itsRegExpLiterals != null)
			{
				this.functionRegExps = this.createRegExpWraps(cx, scope);
			}
		}
		public override object Call(Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			object result;
			if (!ScriptRuntime.hasTopCall(cx))
			{
				result = ScriptRuntime.DoTopCall(this, cx, scope, thisObj, args);
			}
			else
			{
				result = Interpreter.Interpret(this, cx, scope, thisObj, args);
			}
			return result;
		}
		public object Exec(Context cx, IScriptable scope)
		{
			if (this.idata.itsFunctionType != 0)
			{
				throw new ApplicationException();
			}
			object result;
			if (!ScriptRuntime.hasTopCall(cx))
			{
				result = ScriptRuntime.DoTopCall(this, cx, scope, scope, ScriptRuntime.EmptyArgs);
			}
			else
			{
				result = Interpreter.Interpret(this, cx, scope, scope, ScriptRuntime.EmptyArgs);
			}
			return result;
		}
		protected internal override string getParamOrVarName(int index)
		{
			return this.idata.argNames[index];
		}
	}
}
