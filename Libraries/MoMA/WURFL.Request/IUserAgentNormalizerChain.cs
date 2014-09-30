using System;
namespace WURFL.Request
{
	internal interface IUserAgentNormalizerChain : IUserAgentNormalizer
	{
		IUserAgentNormalizerChain Add(IUserAgentNormalizer userAgentNormalizer);
	}
}
