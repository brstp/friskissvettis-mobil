using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public class ContextEventArgs : EventArgs
	{
		private Context m_Context = null;
		public Context Context
		{
			get
			{
				return this.m_Context;
			}
		}
		public ContextEventArgs(Context cx)
		{
			this.m_Context = cx;
		}
	}
}
