using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
namespace WURFL.Resource.WURFLResources
{
	public class ZippedWURFLResource : IWURFLResource
	{
		private readonly string resourcePath;
		public ZippedWURFLResource(string resourcePath)
		{
			if (!this.IsZipFile(resourcePath))
			{
				throw new ArgumentException(string.Format("[{0}] is not a valid zip file.", resourcePath));
			}
			this.resourcePath = resourcePath;
		}
		public Stream GetStream()
		{
			Stream zippedStream;
			using (ZipInputStream zipInputStream = new ZipInputStream(File.OpenRead(this.resourcePath)))
			{
				if (!this.ZipFileHasAnyEntry(zipInputStream))
				{
					throw new Exception("The Zipped File you passed has no files inside");
				}
				zippedStream = this.GetZippedStream(zipInputStream);
			}
			return zippedStream;
		}
		private bool IsZipFile(string resourcePath)
		{
			return !string.IsNullOrEmpty(resourcePath) && resourcePath.EndsWith(".zip");
		}
		private bool ZipFileHasAnyEntry(ZipInputStream zipInputStream)
		{
			return zipInputStream.GetNextEntry() != null;
		}
		private Stream GetZippedStream(ZipInputStream zipInputStream)
		{
			byte[] buffer = new byte[zipInputStream.Length];
			zipInputStream.Read(buffer, 0, (int)zipInputStream.Length);
			return new MemoryStream(buffer);
		}
	}
}
