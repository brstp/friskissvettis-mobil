using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public class FunctionNode : ScriptOrFnNode
	{
		public const int FUNCTION_STATEMENT = 1;
		public const int FUNCTION_EXPRESSION = 2;
		public const int FUNCTION_EXPRESSION_STATEMENT = 3;
		internal string functionName;
		internal bool itsNeedsActivation;
		internal int itsFunctionType;
		internal bool itsIgnoreDynamicScope;
		public virtual string FunctionName
		{
			get
			{
				return this.functionName;
			}
		}
		public virtual bool IgnoreDynamicScope
		{
			get
			{
				return this.itsIgnoreDynamicScope;
			}
		}
		public virtual int FunctionType
		{
			get
			{
				return this.itsFunctionType;
			}
		}
		public virtual bool RequiresActivation
		{
			get
			{
				return this.itsNeedsActivation;
			}
		}
		public FunctionNode(string name) : base(107)
		{
			this.functionName = name;
		}
	}
}
