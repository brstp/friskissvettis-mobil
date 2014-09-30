using System;
using System.IO;
namespace WURFL.Resource.WURFLResources
{
	public class WURFLResource : IWURFLResource
	{
		private readonly string resourcePath;
		public WURFLResource(string resourcePath)
		{
			this.resourcePath = resourcePath;
		}
		public Stream GetStream()
		{
			return new FileStream(this.resourcePath, FileMode.Open, FileAccess.Read);
		}
	}
}
