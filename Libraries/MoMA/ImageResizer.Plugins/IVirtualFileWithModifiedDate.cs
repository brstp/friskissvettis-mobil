using System;
using System.Runtime.InteropServices;
namespace ImageResizer.Plugins
{
	[ComVisible(true)]
	public interface IVirtualFileWithModifiedDate : IVirtualFile
	{
		DateTime ModifiedDateUTC
		{
			get;
		}
	}
}
