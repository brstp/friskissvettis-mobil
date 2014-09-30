using System;
namespace WURFL.Resource.WURFLResources
{
	public class WURFLResourceFactory
	{
		public IWURFLResource Create(string path)
		{
			if (this.IsZipFile(path))
			{
				return new ZippedWURFLResource(path);
			}
			if (this.IsTarGZippedFile(path))
			{
				return new GZippedWURFLResource(path);
			}
			return new WURFLResource(path);
		}
		private bool IsTarGZippedFile(string path)
		{
			return path.ToLower().EndsWith(".tgz");
		}
		private bool IsZipFile(string path)
		{
			return path.ToLower().EndsWith(".zip");
		}
	}
}
