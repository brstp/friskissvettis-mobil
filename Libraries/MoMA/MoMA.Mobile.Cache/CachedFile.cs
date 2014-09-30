using System;
namespace MoMA.Mobile.Cache
{
	[Serializable]
	public class CachedFile
	{
		public string Hash
		{
			get;
			set;
		}
		public string Url
		{
			get;
			set;
		}
		public DateTime Added
		{
			get;
			set;
		}
		public DateTime LastRequested
		{
			get;
			set;
		}
		public long Size
		{
			get;
			set;
		}
		public string Path
		{
			get;
			set;
		}
	}
}
