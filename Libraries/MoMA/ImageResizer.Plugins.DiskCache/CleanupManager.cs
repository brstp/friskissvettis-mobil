using ImageResizer.Configuration.Issues;
using ImageResizer.Plugins.DiskCache.Cleanup;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
namespace ImageResizer.Plugins.DiskCache
{
	[ComVisible(true)]
	public class CleanupManager : IIssueProvider, IDisposable
	{
		protected CustomDiskCache cache;
		protected CleanupStrategy cs;
		protected CleanupQueue queue;
		protected CleanupWorker worker;
		public CleanupManager(CustomDiskCache cache, CleanupStrategy cs)
		{
			CleanupManager <>4__this = this;
			this.cache = cache;
			this.cs = cs;
			this.queue = new CleanupQueue();
			cache.CacheResultReturned += delegate(CustomDiskCache sender, CacheResult r)
			{
				if (r.Result == CacheQueryResult.Miss)
				{
					this.AddedFile(r.RelativePath);
					return;
				}
				this.BeLazy();
			};
			cache.Index.FileDisappeared += delegate(string relativePath, string physicalPath)
			{
				<>4__this.queue.ReplaceWith(new CleanupWorkItem(CleanupWorkItem.Kind.CleanFolderRecursive, "", cache.PhysicalCachePath));
				<>4__this.worker.MayHaveWork();
			};
			this.worker = new CleanupWorker(cs, this.queue, cache);
		}
		public void BeLazy()
		{
			this.worker.BeLazy();
		}
		public void AddedFile(string relativePath)
		{
			int num = relativePath.LastIndexOf('/');
			string text = (num > -1) ? relativePath.Substring(0, num) : "";
			char directorySeparatorChar = Path.DirectorySeparatorChar;
			string physicalPath = this.cache.PhysicalCachePath.TrimEnd(new char[]
			{
				directorySeparatorChar
			}) + directorySeparatorChar + text.Replace('/', directorySeparatorChar).Replace('\\', directorySeparatorChar).Trim(new char[]
			{
				directorySeparatorChar
			});
			if (this.queue.QueueIfUnique(new CleanupWorkItem(CleanupWorkItem.Kind.CleanFolderRecursive, text, physicalPath)))
			{
				this.worker.MayHaveWork();
			}
		}
		public void CleanAll()
		{
			if (this.queue.QueueIfUnique(new CleanupWorkItem(CleanupWorkItem.Kind.CleanFolderRecursive, "", this.cache.PhysicalCachePath)))
			{
				this.worker.MayHaveWork();
			}
		}
		public void UsedFile(string relativePath, string physicalPath)
		{
			this.cache.Index.bumpDateIfExists(relativePath);
			this.queue.QueueIfUnique(new CleanupWorkItem(CleanupWorkItem.Kind.FlushAccessedDate, relativePath, physicalPath));
			this.worker.MayHaveWork();
		}
		public void Dispose()
		{
			this.worker.Dispose();
		}
		public IEnumerable<IIssue> GetIssues()
		{
			if (this.worker != null)
			{
				return this.worker.GetIssues();
			}
			return new IIssue[0];
		}
	}
}
