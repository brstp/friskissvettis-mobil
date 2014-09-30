using MoMA.Helpers;
using MoMA.Mobile.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
namespace MoMA.Mobile.Cache
{
	[Serializable]
	public class CachedFileCollection : List<CachedFile>
	{
		public const string filename = "log.xml";
		public string Path
		{
			get
			{
				return PathConfiguration.GetSection().ExternalImageCache + "\\log.xml";
			}
		}
		public void Load()
		{
			if (File.Exists(this.Path))
			{
				CachedFileCollection collection = SerializeHelper.DeserializeFromFile<CachedFileCollection>(this.Path);
				base.AddRange(collection);
			}
		}
		public void Save()
		{
			SerializeHelper.SerializeToFile(this, this.Path);
		}
	}
}
