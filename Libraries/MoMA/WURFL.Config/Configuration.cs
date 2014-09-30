using System;
namespace WURFL.Config
{
	public class Configuration
	{
		private readonly ICache<object, IDevice> cache;
		private readonly string mainFile;
		private readonly string[] patchFiles;
		public string MainFile
		{
			get
			{
				return this.mainFile;
			}
		}
		public string[] PatchFiles
		{
			get
			{
				return this.patchFiles;
			}
		}
		public ICache<object, IDevice> Cache
		{
			get
			{
				return this.cache;
			}
		}
		public Configuration(string mainFile) : this(mainFile, new string[0])
		{
		}
		public Configuration(string mainFile, string[] patchFiles)
		{
			this.mainFile = mainFile;
			this.patchFiles = patchFiles;
		}
		public Configuration(string mainFile, string[] patchFiles, ICache<object, IDevice> cache) : this(mainFile, patchFiles)
		{
			this.cache = cache;
		}
	}
}
