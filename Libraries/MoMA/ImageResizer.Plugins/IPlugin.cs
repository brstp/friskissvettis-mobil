using ImageResizer.Configuration;
using System;
using System.Runtime.InteropServices;
namespace ImageResizer.Plugins
{
	[ComVisible(true)]
	public interface IPlugin
	{
		IPlugin Install(Config c);
		bool Uninstall(Config c);
	}
}
