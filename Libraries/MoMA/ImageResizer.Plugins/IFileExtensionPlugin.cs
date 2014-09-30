using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace ImageResizer.Plugins
{
	[ComVisible(true)]
	public interface IFileExtensionPlugin
	{
		IEnumerable<string> GetSupportedFileExtensions();
	}
}
