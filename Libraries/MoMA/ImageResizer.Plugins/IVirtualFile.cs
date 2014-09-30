using System;
using System.IO;
using System.Runtime.InteropServices;
namespace ImageResizer.Plugins
{
	[ComVisible(true)]
	public interface IVirtualFile
	{
		string VirtualPath
		{
			get;
		}
		Stream Open();
	}
}
