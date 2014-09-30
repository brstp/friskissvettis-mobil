using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public sealed class Undefined
	{
		public static readonly object Value = new Undefined();
		private Undefined()
		{
		}
	}
}
