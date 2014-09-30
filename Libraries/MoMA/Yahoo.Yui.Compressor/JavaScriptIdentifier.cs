using System;
namespace Yahoo.Yui.Compressor
{
	public class JavaScriptIdentifier : JavaScriptToken
	{
		public int RefCount
		{
			get;
			set;
		}
		public string MungedValue
		{
			get;
			set;
		}
		public ScriptOrFunctionScope DeclaredScope
		{
			get;
			set;
		}
		public bool MarkedForMunging
		{
			get;
			set;
		}
		public JavaScriptIdentifier(string value, ScriptOrFunctionScope declaredScope) : base(38, value)
		{
			this.MarkedForMunging = true;
			this.DeclaredScope = declaredScope;
		}
	}
}
