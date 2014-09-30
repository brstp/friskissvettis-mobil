using System;
using System.Collections.Generic;
namespace WURFL.Config
{
	public class InMemoryConfigurer : IWURFLConfigurer
	{
		private readonly List<string> patchFiles = new List<string>();
		private ICache<object, IDevice> cache;
		private string mainFile;
		public Configuration Build()
		{
			if (string.IsNullOrEmpty(this.mainFile))
			{
				throw new ArgumentException(string.Format("The specified path[{0}] is not valid.", this.mainFile));
			}
			return new Configuration(this.mainFile, this.patchFiles.ToArray(), this.cache);
		}
		public InMemoryConfigurer MainFile(string mainFile)
		{
			this.mainFile = mainFile;
			return this;
		}
		public InMemoryConfigurer PatchFile(string patchFile)
		{
			this.patchFiles.Add(patchFile);
			return this;
		}
		public IWURFLConfigurer Cache(ICache<object, IDevice> cache)
		{
			this.cache = cache;
			return this;
		}
	}
}
