using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public class ContextWrapEventArgs : ContextScriptableEventArgs
	{
		private object m_Source = null;
		private object m_Target = null;
		private Type m_StaticType = null;
		public object Source
		{
			get
			{
				return this.m_Source;
			}
		}
		public object Target
		{
			get
			{
				return this.m_Target;
			}
			set
			{
				this.m_Target = null;
			}
		}
		public Type staticType
		{
			get
			{
				return this.m_StaticType;
			}
		}
		public ContextWrapEventArgs(Context cx, IScriptable scope, object obj, Type staticType) : base(cx, scope)
		{
			this.m_Source = obj;
			this.m_StaticType = staticType;
		}
	}
}
