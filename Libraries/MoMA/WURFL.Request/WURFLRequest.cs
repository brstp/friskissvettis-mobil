using System;
using WURFL.Matchers.Utils;
namespace WURFL.Request
{
	public class WURFLRequest
	{
		private readonly string userAgent;
		private readonly string userAgentProfile;
		private bool? isMobileRequest;
		public string UserAgent
		{
			get
			{
				return this.userAgent;
			}
		}
		public bool IsMobileRequest
		{
			get
			{
				if (!this.isMobileRequest.HasValue)
				{
					this.isMobileRequest = new bool?(MatchersUtil.IsMobileBrowser(this.userAgent));
				}
				return this.isMobileRequest.Value;
			}
		}
		public string UserAgentProfile
		{
			get
			{
				return this.userAgentProfile;
			}
		}
		public WURFLRequest(string userAgent) : this(userAgent, null)
		{
		}
		public WURFLRequest(string userAgent, string userAgentProfile)
		{
			this.userAgent = userAgent;
			this.userAgentProfile = userAgentProfile;
		}
		public WURFLRequest NormalizedRequest(IUserAgentNormalizer userAgentNormalizer)
		{
			return new WURFLRequest(userAgentNormalizer.Normalize(this.UserAgent));
		}
	}
}
