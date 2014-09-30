using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace ImageResizer.Plugins
{
	[ComVisible(true)]
	public interface IQuerystringPlugin
	{
		IEnumerable<string> GetSupportedQuerystringKeys();
	}
}
