using System;
namespace EcmaScript.NET
{
	internal interface IIdEnumerable
	{
		IdEnumeration GetEnumeration(Context cx, bool enumValues);
	}
}
