using System;
using System.Runtime.InteropServices;
namespace ImageResizer.Plugins.DiskCache
{
	[ComVisible(true)]
	public class CacheResult
	{
		private string physicalPath;
		private string relativePath;
		private CacheQueryResult result;
		public string PhysicalPath
		{
			get
			{
				return this.physicalPath;
			}
		}
		public string RelativePath
		{
			get
			{
				return this.relativePath;
			}
		}
		public CacheQueryResult Result
		{
			get
			{
				return this.result;
			}
			set
			{
				this.result = value;
			}
		}
		public CacheResult(CacheQueryResult result, string physicalPath, string relativePath)
		{
			this.result = result;
			this.physicalPath = physicalPath;
			this.relativePath = relativePath;
		}
	}
}
