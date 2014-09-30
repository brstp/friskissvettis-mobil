using System;
namespace WURFL.Resource
{
	public class WURFLInfo
	{
		private readonly string lastUpdated;
		private readonly string version;
		public string Version
		{
			get
			{
				return this.version;
			}
		}
		public string LastUpdated
		{
			get
			{
				return this.lastUpdated;
			}
		}
		public WURFLInfo() : this(string.Empty, string.Empty)
		{
		}
		public WURFLInfo(string version, string lastUpdated)
		{
			this.version = version;
			this.lastUpdated = lastUpdated;
		}
	}
}
