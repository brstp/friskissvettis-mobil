using System;
using WURFL.Cache;
using WURFL.Config;
using WURFL.Matchers;
using WURFL.Request;
using WURFL.Request.Normalizers;
using WURFL.Resource;
using WURFL.Resource.WURFLResources;
namespace WURFL
{
	public class WURFLManagerBuilder
	{
		public static IWURFLManager Build(IWURFLConfigurer configurer)
		{
			return WURFLManagerBuilder.BuildFrom(configurer.Build());
		}
		private static IWURFLManager BuildFrom(Configuration configuration)
		{
			WURFLResourceFactory wurflResourceFactory = new WURFLResourceFactory();
			IWURFLParser wurflParser = new WURFLParser();
			IMatcherChainFactory matchersChainFactory = new MatchersChainFactory(new UserAgentNormalizerChain());
			DeviceRepositoryBuilder deviceRepositoryBuilder = new DeviceRepositoryBuilder(wurflResourceFactory, wurflParser, matchersChainFactory);
			IDeviceRepository deviceRepository = deviceRepositoryBuilder.Build(configuration.MainFile, configuration.PatchFiles);
			IWURFLRequestFactory wurflRequestFactory = new WURFLRequestFactory(new UserAgentResolver());
			ICache<object, IDevice> cache = (configuration.Cache != null) ? configuration.Cache : new LRUCache<object, IDevice>();
			return new WURFLManager(deviceRepository, wurflRequestFactory, cache);
		}
	}
}
