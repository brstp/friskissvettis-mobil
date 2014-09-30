using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public interface Wrapper
	{
		object Unwrap();
	}
}
