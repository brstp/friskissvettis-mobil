using ImageResizer.Configuration.Issues;
using ImageResizer.Configuration.Xml;
using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.InteropServices;
namespace ImageResizer.Plugins.DiskCache
{
	[ComVisible(true)]
	public class CleanupStrategy : IssueSink
	{
		private TimeSpan startupDelay = new TimeSpan(0, 5, 0);
		private TimeSpan minDelay = new TimeSpan(0, 0, 20);
		private TimeSpan maxDelay = new TimeSpan(0, 5, 0);
		private TimeSpan optimalWorkSegmentLength = new TimeSpan(0, 0, 4);
		private int targetItemsPerFolder = 400;
		private int maximumItemsPerFolder = 1000;
		private TimeSpan avoidRemovalIfUsedWithin = new TimeSpan(96, 0, 0);
		private TimeSpan avoidRemovalIfCreatedWithin = new TimeSpan(24, 0, 0);
		private TimeSpan prohibitRemovalIfUsedWithin = new TimeSpan(0, 5, 0);
		private TimeSpan prohibitRemovalIfCreatedWithin = new TimeSpan(0, 10, 0);
		public TimeSpan StartupDelay
		{
			get
			{
				return this.startupDelay;
			}
			set
			{
				this.startupDelay = value;
			}
		}
		public TimeSpan MinDelay
		{
			get
			{
				return this.minDelay;
			}
			set
			{
				this.minDelay = value;
			}
		}
		public TimeSpan MaxDelay
		{
			get
			{
				return this.maxDelay;
			}
			set
			{
				this.maxDelay = value;
			}
		}
		public TimeSpan OptimalWorkSegmentLength
		{
			get
			{
				return this.optimalWorkSegmentLength;
			}
			set
			{
				this.optimalWorkSegmentLength = value;
			}
		}
		public int TargetItemsPerFolder
		{
			get
			{
				return this.targetItemsPerFolder;
			}
			set
			{
				this.targetItemsPerFolder = value;
			}
		}
		public int MaximumItemsPerFolder
		{
			get
			{
				return this.maximumItemsPerFolder;
			}
			set
			{
				this.maximumItemsPerFolder = value;
			}
		}
		public TimeSpan AvoidRemovalIfUsedWithin
		{
			get
			{
				return this.avoidRemovalIfUsedWithin;
			}
			set
			{
				this.avoidRemovalIfUsedWithin = value;
			}
		}
		public TimeSpan AvoidRemovalIfCreatedWithin
		{
			get
			{
				return this.avoidRemovalIfCreatedWithin;
			}
			set
			{
				this.avoidRemovalIfCreatedWithin = value;
			}
		}
		public TimeSpan ProhibitRemovalIfUsedWithin
		{
			get
			{
				return this.prohibitRemovalIfUsedWithin;
			}
			set
			{
				this.prohibitRemovalIfUsedWithin = value;
			}
		}
		public TimeSpan ProhibitRemovalIfCreatedWithin
		{
			get
			{
				return this.prohibitRemovalIfCreatedWithin;
			}
			set
			{
				this.prohibitRemovalIfCreatedWithin = value;
			}
		}
		public CleanupStrategy() : base("DiskCache.CleanupStrategy")
		{
		}
		public CleanupStrategy(Node n) : base("DiskCache.CleanupStrategy")
		{
			this.LoadFrom(n);
		}
		public void LoadFrom(Node n)
		{
			if (n == null)
			{
				return;
			}
			this.LoadTimeSpan(n.Attrs, "StartupDelay");
			this.LoadTimeSpan(n.Attrs, "MinDelay");
			this.LoadTimeSpan(n.Attrs, "MaxDelay");
			this.LoadTimeSpan(n.Attrs, "OptimalWorkSegmentLength");
			this.LoadTimeSpan(n.Attrs, "AvoidRemovalIfUsedWithin");
			this.LoadTimeSpan(n.Attrs, "AvoidRemovalIfCreatedWithin");
			this.LoadTimeSpan(n.Attrs, "ProhibitRemovalIfUsedWithin");
			this.LoadTimeSpan(n.Attrs, "ProhibitRemovalIfCreatedWithin");
			this.LoadInt(n.Attrs, "TargetItemsPerFolder");
			this.LoadInt(n.Attrs, "MaximumItemsPerFolder");
		}
		protected void LoadTimeSpan(NameValueCollection data, string key)
		{
			string text = data[key];
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			TimeSpan timeSpan = TimeSpan.MinValue;
			if (!TimeSpan.TryParse(text, out timeSpan))
			{
				timeSpan = TimeSpan.MinValue;
			}
			int seconds = -2147483648;
			if (int.TryParse(text, out seconds))
			{
				timeSpan = new TimeSpan(0, 0, seconds);
			}
			if (timeSpan == TimeSpan.MinValue)
			{
				return;
			}
			PropertyInfo property = base.GetType().GetProperty(key);
			property.SetValue(this, timeSpan, null);
		}
		protected void LoadInt(NameValueCollection data, string key)
		{
			string text = data[key];
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			int num = -2147483648;
			if (!int.TryParse(text, out num))
			{
				return;
			}
			PropertyInfo property = base.GetType().GetProperty(key);
			property.SetValue(this, num, null);
		}
		public bool MeetsCleanupCriteria(CachedFileInfo i)
		{
			DateTime utcNow = DateTime.UtcNow;
			return (utcNow.Subtract(i.AccessedUtc) > this.AvoidRemovalIfUsedWithin || this.AvoidRemovalIfUsedWithin <= new TimeSpan(0L) || i.AccessedUtc == i.UpdatedUtc) && (utcNow.Subtract(i.UpdatedUtc) > this.AvoidRemovalIfCreatedWithin || this.AvoidRemovalIfCreatedWithin <= new TimeSpan(0L));
		}
		public bool MeetsOverMaxCriteria(CachedFileInfo i)
		{
			DateTime utcNow = DateTime.UtcNow;
			return (utcNow.Subtract(i.AccessedUtc) > this.ProhibitRemovalIfUsedWithin || this.ProhibitRemovalIfUsedWithin <= new TimeSpan(0L) || i.AccessedUtc == i.UpdatedUtc) && (utcNow.Subtract(i.UpdatedUtc) > this.ProhibitRemovalIfCreatedWithin || this.ProhibitRemovalIfCreatedWithin <= new TimeSpan(0L));
		}
		public bool ShouldRemove(string relativePath, CachedFileInfo info, bool isOverMax)
		{
			if (isOverMax)
			{
				return this.MeetsOverMaxCriteria(info);
			}
			return this.MeetsCleanupCriteria(info);
		}
	}
}
