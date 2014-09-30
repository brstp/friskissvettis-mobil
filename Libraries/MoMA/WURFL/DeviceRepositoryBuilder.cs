using System;
using WURFL.Matchers;
using WURFL.Request.Normalizers;
using WURFL.Resource;
using WURFL.Resource.WURFLResources;
namespace WURFL
{
	public class DeviceRepositoryBuilder
	{
		private readonly IMatcherChainFactory matchersChainFactory;
		private readonly IWURFLParser wurflParser;
		private readonly WURFLResourceFactory wurflResourceFactory;
		public DeviceRepositoryBuilder(WURFLResourceFactory wurflResourceFactory, IWURFLParser wurflParser) : this(wurflResourceFactory, wurflParser, new MatchersChainFactory(new UserAgentNormalizerChain()))
		{
		}
		public DeviceRepositoryBuilder(WURFLResourceFactory wurflResourceFactory, IWURFLParser wurflParser, IMatcherChainFactory matchersChainFactory)
		{
			this.wurflResourceFactory = wurflResourceFactory;
			this.wurflParser = wurflParser;
			this.matchersChainFactory = matchersChainFactory;
		}
		public IDeviceRepository Build(string wurflFile)
		{
			return this.Build(wurflFile, new string[0]);
		}
		public IDeviceRepository Build(string wurflFile, string[] patchFiles)
		{
			IWURFLResource wurflResource = this.wurflResourceFactory.Create(wurflFile);
			IWURFLResource[] patchResources = this.PatchResources(patchFiles);
			ResourceData resourceData = this.wurflParser.Parse(wurflResource, patchResources);
			IMatchersChain matchersChain = this.matchersChainFactory.Create(resourceData.ModelDevices);
			return new DeviceRepository(resourceData, matchersChain);
		}
		private IWURFLResource[] PatchResources(string[] patchFiles)
		{
			IWURFLResource[] array = new IWURFLResource[patchFiles.Length];
			for (int i = 0; i < patchFiles.Length; i++)
			{
				array[i] = this.wurflResourceFactory.Create(patchFiles[i]);
			}
			return array;
		}
	}
}
