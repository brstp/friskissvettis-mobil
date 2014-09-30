using ImageResizer.Caching;
using ImageResizer.Util;
using System;
using System.IO;
using System.Runtime.InteropServices;
namespace ImageResizer.Plugins.DiskCache
{
	[ComVisible(true)]
	public class CustomDiskCache
	{
		protected string physicalCachePath;
		protected int subfolders;
		protected bool hashModifiedDate;
		protected LockProvider locks = new LockProvider();
		private CacheIndex index = new CacheIndex();
		public event CacheResultHandler CacheResultReturned;
		public string PhysicalCachePath
		{
			get
			{
				return this.physicalCachePath;
			}
		}
		public LockProvider Locks
		{
			get
			{
				return this.locks;
			}
		}
		public CacheIndex Index
		{
			get
			{
				return this.index;
			}
		}
		public CustomDiskCache(string physicalCachePath, int subfolders, bool hashModifiedDate)
		{
			this.physicalCachePath = physicalCachePath;
			this.subfolders = subfolders;
			this.hashModifiedDate = hashModifiedDate;
		}
		public CacheResult GetCachedFile(string keyBasis, string extension, ResizeImageDelegate writeCallback, DateTime sourceModifiedUtc, int timeoutMs)
		{
			bool hasModifiedDate = !sourceModifiedUtc.Equals(DateTime.MinValue);
			if (this.hashModifiedDate && hasModifiedDate)
			{
				keyBasis = keyBasis + "|" + sourceModifiedUtc.Ticks.ToString();
			}
			string relativePath = new UrlHasher().hash(keyBasis, this.subfolders, "/") + '.' + extension;
			string physicalPath = this.PhysicalCachePath.TrimEnd(new char[]
			{
				'\\',
				'/'
			}) + Path.DirectorySeparatorChar + relativePath.Replace('/', Path.DirectorySeparatorChar);
			CacheResult result = new CacheResult(CacheQueryResult.Hit, physicalPath, relativePath);
			if (((!hasModifiedDate || this.hashModifiedDate) && !this.Index.existsCertain(relativePath, physicalPath)) || !this.Index.modifiedDateMatchesCertainExists(sourceModifiedUtc, relativePath, physicalPath))
			{
				if (!this.Locks.TryExecute(relativePath.ToUpperInvariant(), timeoutMs, delegate
				{
					if (((!hasModifiedDate || this.hashModifiedDate) && !this.Index.exists(relativePath, physicalPath)) || !this.Index.modifiedDateMatches(sourceModifiedUtc, relativePath, physicalPath))
					{
						if (!Directory.Exists(Path.GetDirectoryName(physicalPath)))
						{
							Directory.CreateDirectory(Path.GetDirectoryName(physicalPath));
						}
						FileStream fileStream = new FileStream(physicalPath, FileMode.Create, FileAccess.Write);
						using (fileStream)
						{
							writeCallback(fileStream);
						}
						DateTime utcNow = DateTime.UtcNow;
						if (hasModifiedDate)
						{
							File.SetLastWriteTimeUtc(physicalPath, sourceModifiedUtc);
						}
						File.SetCreationTimeUtc(physicalPath, utcNow);
						this.Index.setCachedFileInfo(relativePath, new CachedFileInfo(sourceModifiedUtc, utcNow, utcNow));
						result.Result = CacheQueryResult.Miss;
					}
				}))
				{
					result.Result = CacheQueryResult.Failed;
				}
			}
			if (this.CacheResultReturned != null)
			{
				this.CacheResultReturned(this, result);
			}
			return result;
		}
	}
}
