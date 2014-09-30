using System;
using System.IO;
using System.Runtime.InteropServices;
namespace ImageResizer.Plugins.DiskCache
{
	[ComVisible(true)]
	public class CachedFileInfo
	{
		private DateTime modifiedUtc = DateTime.MinValue;
		private DateTime accessedUtc = DateTime.MinValue;
		private DateTime updatedUtc = DateTime.MinValue;
		public DateTime ModifiedUtc
		{
			get
			{
				return this.modifiedUtc;
			}
		}
		public DateTime AccessedUtc
		{
			get
			{
				return this.accessedUtc;
			}
		}
		public DateTime UpdatedUtc
		{
			get
			{
				return this.updatedUtc;
			}
		}
		public CachedFileInfo(FileInfo f)
		{
			this.modifiedUtc = f.LastWriteTimeUtc;
			this.accessedUtc = f.LastAccessTimeUtc;
			this.updatedUtc = f.CreationTimeUtc;
		}
		public CachedFileInfo(FileInfo f, CachedFileInfo old)
		{
			this.modifiedUtc = f.LastWriteTimeUtc;
			this.accessedUtc = f.LastAccessTimeUtc;
			if (old != null && this.accessedUtc < old.accessedUtc)
			{
				this.accessedUtc = old.accessedUtc;
			}
			this.updatedUtc = f.CreationTimeUtc;
		}
		public CachedFileInfo(DateTime modifiedDate, DateTime createdDate)
		{
			this.modifiedUtc = modifiedDate;
			this.updatedUtc = createdDate;
			this.accessedUtc = createdDate;
		}
		public CachedFileInfo(DateTime modifiedDate, DateTime createdDate, DateTime accessedDate)
		{
			this.modifiedUtc = modifiedDate;
			this.updatedUtc = createdDate;
			this.accessedUtc = accessedDate;
		}
		public CachedFileInfo(CachedFileInfo f, DateTime accessedDate)
		{
			this.modifiedUtc = f.modifiedUtc;
			this.updatedUtc = f.updatedUtc;
			this.accessedUtc = accessedDate;
		}
	}
}
