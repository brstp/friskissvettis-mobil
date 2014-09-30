using System;
using System.Collections.Generic;
using WURFL.Request.Normalizers.Generic;
namespace WURFL.Request.Normalizers
{
	internal class UserAgentNormalizerChain : IUserAgentNormalizerChain, IUserAgentNormalizer
	{
		private readonly ICollection<IUserAgentNormalizer> userAgentNormalizers;
		public UserAgentNormalizerChain()
		{
			this.userAgentNormalizers = new List<IUserAgentNormalizer>();
			this.userAgentNormalizers.Add(new SerialNumberRemover());
			this.userAgentNormalizers.Add(new BlackBerryNormalizer());
			this.userAgentNormalizers.Add(new YesWAPRemover());
			this.userAgentNormalizers.Add(new UPLinkRemover());
			this.userAgentNormalizers.Add(new BabelFishRemover());
			this.userAgentNormalizers.Add(new LocaleRemover());
		}
		public UserAgentNormalizerChain(IEnumerable<IUserAgentNormalizer> userAgentNormalizers)
		{
			this.userAgentNormalizers = new List<IUserAgentNormalizer>(userAgentNormalizers);
		}
		public string Normalize(string userAgent)
		{
			string text = userAgent;
			foreach (IUserAgentNormalizer current in this.userAgentNormalizers)
			{
				text = current.Normalize(text);
			}
			return text;
		}
		public IUserAgentNormalizerChain Add(IUserAgentNormalizer userAgentNormalizer)
		{
			return new UserAgentNormalizerChain(new List<IUserAgentNormalizer>(this.userAgentNormalizers)
			{
				userAgentNormalizer
			});
		}
	}
}
