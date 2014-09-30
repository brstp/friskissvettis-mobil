using ICSharpCode.SharpZipLib.GZip;
using System;
using System.IO;
namespace WURFL.Resource.WURFLResources
{
	public class GZippedWURFLResource : IWURFLResource
	{
		private readonly string resourcePath;
		public GZippedWURFLResource(string resourcePath)
		{
			if (!this.IsGZippedFile(resourcePath))
			{
				throw new ArgumentException(string.Format("[{0}] is not a valid gzipped file.", resourcePath));
			}
			this.resourcePath = resourcePath;
		}
		private bool IsGZippedFile(string path)
		{
			return !string.IsNullOrEmpty(path) && path.EndsWith(".gz");
		}
		public Stream GetStream()
		{
			return new GZipInputStream(File.OpenRead(this.resourcePath));
		}
	}
}
