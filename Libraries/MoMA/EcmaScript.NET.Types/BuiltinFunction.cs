using EcmaScript.NET.Collections;
using EcmaScript.NET.Debugging;
using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET.Types
{
	[ComVisible(true)]
	public abstract class BuiltinFunction : BaseFunction
	{
		public override int Length
		{
			get
			{
				int paramCount = this.ParamCount;
				int result;
				if (this.LanguageVersion != Context.Versions.JS1_2)
				{
					result = paramCount;
				}
				else
				{
					Context currentContext = Context.CurrentContext;
					BuiltinCall builtinCall = ScriptRuntime.findFunctionActivation(currentContext, this);
					if (builtinCall == null)
					{
						result = paramCount;
					}
					else
					{
						result = builtinCall.originalArgs.Length;
					}
				}
				return result;
			}
		}
		public override int Arity
		{
			get
			{
				return this.ParamCount;
			}
		}
		public virtual string EncodedSource
		{
			get
			{
				return null;
			}
		}
		public virtual DebuggableScript DebuggableView
		{
			get
			{
				return null;
			}
		}
		protected internal abstract Context.Versions LanguageVersion
		{
			get;
		}
		protected internal abstract int ParamCount
		{
			get;
		}
		protected internal abstract int ParamAndVarCount
		{
			get;
		}
		public void initScriptFunction(Context cx, IScriptable scope)
		{
			ScriptRuntime.setFunctionProtoAndParent(this, scope);
		}
		internal override string Decompile(int indent, int flags)
		{
			string encodedSource = this.EncodedSource;
			string result;
			if (encodedSource == null)
			{
				result = base.Decompile(indent, flags);
			}
			else
			{
				UintMap uintMap = new UintMap(1);
				uintMap.put(1, indent);
				result = Decompiler.Decompile(encodedSource, flags, uintMap);
			}
			return result;
		}
		protected internal abstract string getParamOrVarName(int index);
	}
}
