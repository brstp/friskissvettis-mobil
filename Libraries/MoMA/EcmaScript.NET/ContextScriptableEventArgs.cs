using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public class ContextScriptableEventArgs : ContextEventArgs
	{
		private IScriptable m_Scope = null;
		public IScriptable Scope
		{
			get
			{
				return this.m_Scope;
			}
		}
		public ContextScriptableEventArgs(Context cx, IScriptable scope) : base(cx)
		{
			this.m_Scope = scope;
		}
	}
}
