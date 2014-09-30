using System;
using System.Collections.Generic;
using WURFL.Request;
using WURFL.Request.Normalizers.Specific;
using WURFL.Resource;
namespace WURFL.Matchers
{
	internal class MatchersChainFactory : IMatcherChainFactory
	{
		private readonly IUserAgentNormalizerChain _genericUserAgentNormalizerChain;
		private IUserAgentNormalizer GenericNormalizer
		{
			get
			{
				return this._genericUserAgentNormalizerChain;
			}
		}
		public MatchersChainFactory(IUserAgentNormalizerChain genericUserAgentNormalizerChain)
		{
			this._genericUserAgentNormalizerChain = genericUserAgentNormalizerChain;
		}
		public IMatchersChain Create(ICollection<IModelDevice> devices)
		{
			IEnumerable<IMatcher> matchers = this.GetMatchers();
			foreach (IModelDevice current in devices)
			{
				foreach (IMatcher current2 in matchers)
				{
					if (current2.CanMatch(RequestFactory.FromUserAgent(current.UserAgent)))
					{
						current2.Add(current.UserAgent, current.Id);
						break;
					}
				}
			}
			return new MatchersChain(matchers);
		}
		private IEnumerable<IMatcher> GetMatchers()
		{
			return new List<IMatcher>
			{
				new NokiaMatcher(this.GenericNormalizer),
				new LGUPLUSMatcher(this.GenericNormalizer),
				new AndroidMatcher(this.GenericNormalizer),
				new SonyEricssonMatcher(this.GenericNormalizer),
				new MotorolaMatcher(this.GenericNormalizer),
				new BlackBerryMatcher(this.GenericNormalizer),
				new SiemensMatcher(this.GenericNormalizer),
				new SagemMatcher(this.GenericNormalizer),
				new SamsungMatcher(this.GenericNormalizer),
				new PanasonicMatcher(this.GenericNormalizer),
				new NecMatcher(this.GenericNormalizer),
				new QtekMatcher(this.GenericNormalizer),
				new MitsubishiMatcher(this.GenericNormalizer),
				new PhilipsMatcher(this.GenericNormalizer),
				new LGMatcher(this.GenericNormalizer),
				new AppleMatcher(this.GenericNormalizer),
				new KyoceraMatcher(this.GenericNormalizer),
				new AlcatelMatcher(this.GenericNormalizer),
				new SharpMatcher(this.GenericNormalizer),
				new SanyoMatcher(this.GenericNormalizer),
				new BenQMatcher(this.GenericNormalizer),
				new PantechMatcher(this.GenericNormalizer),
				new ToshibaMatcher(this.GenericNormalizer),
				new GrundingMatcher(this.GenericNormalizer),
				new HTCMatcher(this.GenericNormalizer),
				new BotMatcher(this.GenericNormalizer),
				new SPVMatcher(this.GenericNormalizer),
				new WindowsCEMatcher(this.GenericNormalizer),
				new PortalmmmMatcher(this.GenericNormalizer),
				new DoCoMoMatcher(this.GenericNormalizer),
				new KDDIMatcher(this.GenericNormalizer),
				new VodafoneMatcher(this.GenericNormalizer),
				new OperaMiniMatcher(this.GenericNormalizer),
				new MaemoMatcher(this.ComposeNormalizer(new MaemoNormalizer())),
				new ChromeMatcher(this.ComposeNormalizer(new ChromeNormalizer())),
				new AOLMatcher(this.GenericNormalizer),
				new OperaMatcher(this.GenericNormalizer),
				new SafariMatcher(this.ComposeNormalizer(new SafariNormalizer())),
				new FirefoxMatcher(this.ComposeNormalizer(new FirefoxNormalizer())),
				new MSIEMatcher(this.ComposeNormalizer(new MSIENormalizer())),
				new CatchAllMatcher(this.GenericNormalizer)
			};
		}
		private IUserAgentNormalizerChain ComposeNormalizer(IUserAgentNormalizer userAgentNormalizer)
		{
			return this._genericUserAgentNormalizerChain.Add(userAgentNormalizer);
		}
	}
}
