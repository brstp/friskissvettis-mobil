using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET.Debugging
{
	[ComVisible(true)]
	public interface DebuggableObject
	{
		object[] AllIds
		{
			get;
		}
	}
}
