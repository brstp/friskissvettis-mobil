using ImageResizer.Configuration.Issues;
using ImageResizer.Plugins.DiskCache.Cleanup;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
namespace ImageResizer.Plugins.DiskCache
{
	[ComVisible(true)]
	public class CleanupWorker : IssueSink, IDisposable
	{
		private Thread t;
		private EventWaitHandle _queueWait = new AutoResetEvent(false);
		private EventWaitHandle _quitWait = new AutoResetEvent(false);
		private CleanupStrategy cs;
		private CleanupQueue queue;
		private CustomDiskCache cache;
		protected long lastBusy = DateTime.MinValue.Ticks;
		protected long lastWorked = DateTime.MinValue.Ticks;
		protected readonly object _timesLock = new object();
		protected volatile bool shuttingDown;
		public CleanupWorker(CleanupStrategy cs, CleanupQueue queue, CustomDiskCache cache) : base("DiskCache-CleanupWorker")
		{
			this.cs = cs;
			this.queue = queue;
			this.cache = cache;
			this.t = new Thread(new ThreadStart(this.main));
			this.t.IsBackground = true;
			this.t.Start();
		}
		public void MayHaveWork()
		{
			this._queueWait.Set();
		}
		public void BeLazy()
		{
			object timesLock;
			Monitor.Enter(timesLock = this._timesLock);
			try
			{
				this.lastBusy = DateTime.UtcNow.Ticks;
			}
			finally
			{
				Monitor.Exit(timesLock);
			}
		}
		protected void main()
		{
			this._quitWait.WaitOne(this.cs.StartupDelay);
			while (!this.shuttingDown)
			{
				bool flag = false;
				object timesLock;
				Monitor.Enter(timesLock = this._timesLock);
				try
				{
					flag = (DateTime.UtcNow.Subtract(new DateTime(this.lastWorked)) > this.cs.MaxDelay);
				}
				finally
				{
					Monitor.Exit(timesLock);
				}
				bool flag2 = false;
				object timesLock2;
				Monitor.Enter(timesLock2 = this._timesLock);
				try
				{
					flag2 = (DateTime.UtcNow.Subtract(new DateTime(this.lastBusy)) > this.cs.MinDelay);
				}
				finally
				{
					Monitor.Exit(timesLock2);
				}
				bool flag3 = (flag || flag2) && this.DoWorkFor(this.cs.OptimalWorkSegmentLength);
				if (this.shuttingDown)
				{
					return;
				}
				if (!flag3 && this.queue.IsEmpty)
				{
					this._queueWait.WaitOne();
				}
				else
				{
					if (flag3 && flag2)
					{
						this._quitWait.WaitOne(this.cs.OptimalWorkSegmentLength);
					}
					else
					{
						if (flag3 && !flag2)
						{
							long val = 0L;
							object timesLock3;
							Monitor.Enter(timesLock3 = this._timesLock);
							try
							{
								val = (this.cs.MinDelay - DateTime.UtcNow.Subtract(new DateTime(this.lastBusy))).Ticks;
							}
							finally
							{
								Monitor.Exit(timesLock3);
							}
							long val2 = 0L;
							object timesLock4;
							Monitor.Enter(timesLock4 = this._timesLock);
							try
							{
								val2 = (this.cs.MaxDelay - DateTime.UtcNow.Subtract(new DateTime(this.lastWorked))).Ticks;
							}
							finally
							{
								Monitor.Exit(timesLock4);
							}
							this._quitWait.WaitOne(new TimeSpan(Math.Max(val, val2)) + new TimeSpan(0, 0, 1));
						}
					}
				}
				if (this.shuttingDown)
				{
					return;
				}
			}
		}
		protected bool DoWorkFor(TimeSpan length)
		{
			if (this.queue.IsEmpty)
			{
				return false;
			}
			DateTime utcNow = DateTime.UtcNow;
			while (DateTime.UtcNow.Subtract(utcNow) < length && !this.queue.IsEmpty)
			{
				if (this.shuttingDown)
				{
					return true;
				}
				try
				{
					this.DoTask(this.queue.Pop());
				}
				catch (Exception ex)
				{
					if (Debugger.IsAttached)
					{
						throw;
					}
					this.AcceptIssue(new Issue("Failed exeuting task", ex.Message + ex.StackTrace, IssueSeverity.Critical));
				}
			}
			object timesLock;
			Monitor.Enter(timesLock = this._timesLock);
			try
			{
				this.lastWorked = DateTime.UtcNow.Ticks;
			}
			finally
			{
				Monitor.Exit(timesLock);
			}
			return true;
		}
		public void Dispose()
		{
			this.shuttingDown = true;
			this._queueWait.Set();
			this._quitWait.Set();
			this.t.Join();
			this._queueWait.Close();
			this._quitWait.Close();
		}
		protected void DoTask(CleanupWorkItem item)
		{
			if (item.Task == CleanupWorkItem.Kind.RemoveFile)
			{
				this.RemoveFile(item);
				return;
			}
			if (item.Task == CleanupWorkItem.Kind.CleanFolderRecursive || item.Task == CleanupWorkItem.Kind.CleanFolder)
			{
				this.CleanFolder(item, item.Task == CleanupWorkItem.Kind.PopulateFolderRecursive);
				return;
			}
			if (item.Task == CleanupWorkItem.Kind.PopulateFolderRecursive || item.Task == CleanupWorkItem.Kind.PopulateFolder)
			{
				this.PopulateFolder(item, item.Task == CleanupWorkItem.Kind.PopulateFolderRecursive);
				return;
			}
			if (item.Task == CleanupWorkItem.Kind.FlushAccessedDate)
			{
				this.FlushAccessedDate(item);
			}
		}
		protected string addSlash(string s, bool physical)
		{
			if (string.IsNullOrEmpty(s))
			{
				return s;
			}
			if (physical)
			{
				return s.TrimEnd(new char[]
				{
					Path.DirectorySeparatorChar
				}) + Path.DirectorySeparatorChar;
			}
			return s.TrimEnd(new char[]
			{
				'/'
			}) + '/';
		}
		protected void PopulateFolder(CleanupWorkItem item, bool recursive)
		{
			if (!this.cache.Index.GetIsValid(item.RelativePath))
			{
				this.cache.Index.populate(item.RelativePath, item.PhysicalPath);
			}
			if (recursive)
			{
				IList<string> subfolders = this.cache.Index.getSubfolders(item.RelativePath);
				List<CleanupWorkItem> list = new List<CleanupWorkItem>(subfolders.Count);
				foreach (string current in subfolders)
				{
					list.Add(new CleanupWorkItem(CleanupWorkItem.Kind.PopulateFolderRecursive, this.addSlash(item.RelativePath, false) + current, this.addSlash(item.PhysicalPath, true) + current));
				}
				this.queue.InsertRange(list);
			}
		}
		protected void RemoveFile(CleanupWorkItem item)
		{
			LazyTaskProvider lazyProvider = item.LazyProvider;
			item = lazyProvider();
			if (item == null)
			{
				return;
			}
			item.LazyProvider = lazyProvider;
			bool removedFile = false;
			this.cache.Locks.TryExecute(item.RelativePath, 10, delegate
			{
				if (!File.Exists(item.PhysicalPath))
				{
					this.cache.Index.setCachedFileInfo(item.RelativePath, null);
					removedFile = true;
					return;
				}
				this.cache.Index.setCachedFileInfo(item.RelativePath, null);
				try
				{
					File.Delete(item.PhysicalPath);
				}
				catch (IOException)
				{
					return;
				}
				catch (UnauthorizedAccessException)
				{
					return;
				}
				this.cache.Index.setCachedFileInfo(item.RelativePath, null);
				removedFile = true;
			});
			if (!removedFile)
			{
				this.queue.Insert(item);
			}
		}
		protected void CleanFolder(CleanupWorkItem item, bool recursvie)
		{
			if (!this.cache.Index.GetIsValid(item.RelativePath))
			{
				this.queue.InsertRange(new CleanupWorkItem[]
				{
					new CleanupWorkItem(recursvie ? CleanupWorkItem.Kind.PopulateFolderRecursive : CleanupWorkItem.Kind.PopulateFolder, item.RelativePath, item.PhysicalPath),
					item
				});
				return;
			}
			string baseRelative = this.addSlash(item.RelativePath, false);
			string basePhysical = this.addSlash(item.PhysicalPath, true);
			if (item.Task == CleanupWorkItem.Kind.CleanFolderRecursive)
			{
				IList<string> subfolders = this.cache.Index.getSubfolders(item.RelativePath);
				List<CleanupWorkItem> list = new List<CleanupWorkItem>(subfolders.Count);
				foreach (string current in subfolders)
				{
					list.Add(new CleanupWorkItem(CleanupWorkItem.Kind.CleanFolderRecursive, baseRelative + current, basePhysical + current));
				}
				this.queue.InsertRange(list);
			}
			int fileCount = this.cache.Index.getFileCount(item.RelativePath);
			int num = Math.Max(0, fileCount - this.cs.MaximumItemsPerFolder);
			int num2 = Math.Max(0, fileCount - num - this.cs.TargetItemsPerFolder);
			if (num + num2 < 1)
			{
				return;
			}
			LinkedList<KeyValuePair<string, CachedFileInfo>> sortedList = new LinkedList<KeyValuePair<string, CachedFileInfo>>(this.cache.Index.getSortedSubfiles(item.RelativePath));
			CleanupWorkItem item2 = new CleanupWorkItem(CleanupWorkItem.Kind.RemoveFile, delegate
			{
				while (sortedList.Count > 0)
				{
					KeyValuePair<string, CachedFileInfo> value = sortedList.First.Value;
					sortedList.RemoveFirst();
					if (this.cs.ShouldRemove(baseRelative + value.Key, value.Value, true))
					{
						return new CleanupWorkItem(CleanupWorkItem.Kind.RemoveFile, baseRelative + value.Key, basePhysical + value.Key);
					}
				}
				return null;
			});
			CleanupWorkItem item3 = new CleanupWorkItem(CleanupWorkItem.Kind.RemoveFile, delegate
			{
				while (sortedList.Count > 0)
				{
					KeyValuePair<string, CachedFileInfo> value = sortedList.First.Value;
					sortedList.RemoveFirst();
					if (this.cs.ShouldRemove(baseRelative + value.Key, value.Value, false))
					{
						return new CleanupWorkItem(CleanupWorkItem.Kind.RemoveFile, baseRelative + value.Key, basePhysical + value.Key);
					}
				}
				return null;
			});
			for (int i = 0; i < num2; i++)
			{
				this.queue.Insert(item3);
			}
			for (int j = 0; j < num; j++)
			{
				this.queue.Insert(item2);
			}
		}
		public void FlushAccessedDate(CleanupWorkItem item)
		{
			CachedFileInfo cachedFileInfo = this.cache.Index.getCachedFileInfo(item.RelativePath);
			if (cachedFileInfo == null)
			{
				return;
			}
			try
			{
				File.SetLastAccessTimeUtc(item.PhysicalPath, cachedFileInfo.AccessedUtc);
			}
			catch (FileNotFoundException)
			{
			}
			catch (UnauthorizedAccessException)
			{
			}
		}
	}
}
