using System;
using System.Runtime.InteropServices;
namespace ImageResizer.Configuration
{
	[ComVisible(true)]
	public delegate void CacheSelectionHandler(object sender, ICacheSelectionEventArgs e);
}
