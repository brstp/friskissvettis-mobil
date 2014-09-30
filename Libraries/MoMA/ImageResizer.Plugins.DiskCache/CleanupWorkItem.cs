using System;
using System.Runtime.InteropServices;
namespace ImageResizer.Plugins.DiskCache
{
	[ComVisible(true)]
	public class CleanupWorkItem
	{
		public enum Kind
		{
			PopulateFolderRecursive,
			CleanFolderRecursive,
			CleanFolder,
			PopulateFolder,
			RemoveFile,
			FlushAccessedDate
		}
		private string relativePath;
		private LazyTaskProvider lazyProvider;
		private string physicalPath;
		private CleanupWorkItem.Kind task = CleanupWorkItem.Kind.CleanFolderRecursive;
		public LazyTaskProvider LazyProvider
		{
			get
			{
				return this.lazyProvider;
			}
			set
			{
				this.lazyProvider = value;
			}
		}
		public string RelativePath
		{
			get
			{
				return this.relativePath;
			}
		}
		public string PhysicalPath
		{
			get
			{
				return this.physicalPath;
			}
		}
		public CleanupWorkItem.Kind Task
		{
			get
			{
				return this.task;
			}
			set
			{
				this.task = value;
			}
		}
		public CleanupWorkItem(CleanupWorkItem.Kind task, string relativePath, string physicalPath)
		{
			this.task = task;
			this.relativePath = relativePath;
			this.physicalPath = physicalPath;
			this.relativePath.StartsWith("/");
		}
		public CleanupWorkItem(CleanupWorkItem.Kind task, LazyTaskProvider callback)
		{
			this.task = task;
			this.lazyProvider = callback;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			CleanupWorkItem cleanupWorkItem = obj as CleanupWorkItem;
			return cleanupWorkItem != null && (cleanupWorkItem.Task == this.Task && cleanupWorkItem.RelativePath == this.RelativePath && cleanupWorkItem.PhysicalPath == this.PhysicalPath) && cleanupWorkItem.LazyProvider == this.LazyProvider;
		}
	}
}
