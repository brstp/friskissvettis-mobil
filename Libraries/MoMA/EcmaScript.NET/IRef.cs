using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public interface IRef
	{
		bool Has(Context cx);
		object Get(Context cx);
		object Set(Context cx, object value);
		bool Delete(Context cx);
	}
}
