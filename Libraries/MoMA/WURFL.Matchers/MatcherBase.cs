using log4net;
using System;
using System.Collections.Generic;
using WURFL.Commons;
using WURFL.Matchers.StringMatcher;
using WURFL.Request;
namespace WURFL.Matchers
{
	public abstract class MatcherBase : IMatcher
	{
		private static class CatchAllRecoveryMap
		{
			private static readonly IDictionary<string, string> CathAllRecoveryMap;
			static CatchAllRecoveryMap()
			{
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap = new Dictionary<string, string>();
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("UP.Browser/7.2", "opwv_v72_generic");
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("UP.Browser/7", "opwv_v7_generic");
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("UP.Browser/6.2", "opwv_v62_generic");
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("UP.Browser/6", "opwv_v6_generic");
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("UP.Browser/5", "upgui_generic");
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("UP.Browser/4", "uptext_generic");
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("UP.Browser/3", "uptext_generic");
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("Series60", "nokia_generic_series60");
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("NetFront/3.0", "generic_netfront_ver3");
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("ACS-NF/3.0", "generic_netfront_ver3");
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("NetFront/3.1", "generic_netfront_ver3_1");
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("ACS-NF/3.1", "generic_netfront_ver3_1");
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("NetFront/3.2", "generic_netfront_ver3_2");
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("ACS-NF/3.2", "generic_netfront_ver3_2");
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("NetFront/3.3", "generic_netfront_ver3_3");
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("ACS-NF/3.3", "generic_netfront_ver3_3");
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("NetFront/3.4", "generic_netfront_ver3_4");
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("NetFront/3.5", "generic_netfront_ver3_5");
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("NetFront/4.0", "generic_netfront_ver4");
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("NetFront/4.1", "generic_netfront_ver4_1");
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("Windows CE", "generic_ms_mobile_browser_ver1");
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("Mozilla/", Constants.GenericXhtml);
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("ObigoInternetBrowser/Q03C", Constants.GenericXhtml);
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("AU-MIC/2", Constants.GenericXhtml);
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("AU-MIC-", Constants.GenericXhtml);
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("AU-OBIGO/", Constants.GenericXhtml);
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("Obigo/Q03", Constants.GenericXhtml);
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("Obigo/Q04", Constants.GenericXhtml);
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("ObigoInternetBrowser/2", Constants.GenericXhtml);
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("Teleca Q03B1", Constants.GenericXhtml);
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("Opera Mini/1", "browser_opera_mini_release1");
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("Opera Mini/2", "browser_opera_mini_release2");
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("Opera Mini/3", "browser_opera_mini_release3");
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("Opera Mini/4", "browser_opera_mini_release4");
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("DoCoMo", "docomo_generic_jap_ver1");
				MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Add("KDDI", "docomo_generic_jap_ver1");
			}
			public static string DeviceIdOf(string userAgent)
			{
				string text = Collections.Find<string>(MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap.Keys, Predicates.ContainedIn(userAgent));
				if (text != null)
				{
					return MatcherBase.CatchAllRecoveryMap.CathAllRecoveryMap[text];
				}
				return Constants.Generic;
			}
		}
		private readonly IUserAgentNormalizer userAgentNormalizer;
		private readonly IDictionary<string, string> userAgentsDeviceIdMap;
		protected readonly ILog Logger;
		protected readonly bool IsDebugEnabled;
		protected IDictionary<string, string> UserAgentsDeviceIdMap
		{
			get
			{
				return this.userAgentsDeviceIdMap;
			}
		}
		protected ICollection<string> UserAgents
		{
			get
			{
				return this.userAgentsDeviceIdMap.Keys;
			}
		}
		protected MatcherBase() : this(NoOpNormalizer.Instance)
		{
		}
		protected MatcherBase(IUserAgentNormalizer userAgentNormalizer)
		{
			this.userAgentsDeviceIdMap = new SortedDictionary<string, string>(StringComparer.Ordinal);
			this.userAgentNormalizer = userAgentNormalizer;
			this.Logger = LogManager.GetLogger(base.GetType());
			this.IsDebugEnabled = this.Logger.IsDebugEnabled;
		}
		public void Add(string userAgent, string deviceId)
		{
			this.userAgentsDeviceIdMap[this.NormalizeUserAgent(userAgent)] = deviceId;
		}
		public abstract bool CanMatch(WURFLRequest request);
		public virtual string Match(WURFLRequest wurflRequest)
		{
			WURFLRequest wURFLRequest = this.NormalizeRequest(wurflRequest);
			string userAgent = wURFLRequest.UserAgent;
			if (string.IsNullOrEmpty(userAgent))
			{
				return Constants.Generic;
			}
			if (this.IsDebugEnabled)
			{
				this.Logger.Debug("Applying Direct Match on UA: " + userAgent);
			}
			string text;
			this.userAgentsDeviceIdMap.TryGetValue(userAgent, out text);
			if (!MatcherBase.IsBlankOrGeneric(text))
			{
				return text;
			}
			text = this.ApplyConclusiveMatch(wURFLRequest);
			if (!MatcherBase.IsBlankOrGeneric(text))
			{
				return text;
			}
			if (this.IsDebugEnabled)
			{
				this.Logger.Debug("Applying Recovery Match on UA: " + userAgent);
			}
			text = this.ApplyRecoveryMatch(wURFLRequest);
			if (!MatcherBase.IsBlankOrGeneric(text))
			{
				return text;
			}
			if (this.IsDebugEnabled)
			{
				this.Logger.Debug("Applying CatchAll Recovery Match on UA: " + userAgent);
			}
			text = this.ApplyCatchAllRecoveryMatch(wURFLRequest);
			if (string.IsNullOrEmpty(text))
			{
				return Constants.Generic;
			}
			return text;
		}
		private string NormalizeUserAgent(string userAgent)
		{
			return this.userAgentNormalizer.Normalize(userAgent);
		}
		protected WURFLRequest NormalizeRequest(WURFLRequest wurflRequest)
		{
			return wurflRequest.NormalizedRequest(this.userAgentNormalizer);
		}
		protected virtual string ApplyConclusiveMatch(WURFLRequest request)
		{
			string userAgent = request.UserAgent;
			string text = this.LookForMatchingUserAgent(userAgent);
			if (text != null)
			{
				return this.userAgentsDeviceIdMap[text];
			}
			return null;
		}
		protected virtual string LookForMatchingUserAgent(string userAgent)
		{
			if (this.IsDebugEnabled)
			{
				this.Logger.Debug("Applying RIS(FS) UA: " + userAgent);
			}
			return StringMatchers.LongestCommonPrefix(this.UserAgents, userAgent, StringUtils.FirstSlash(userAgent));
		}
		protected virtual string ApplyRecoveryMatch(WURFLRequest request)
		{
			return Constants.Generic;
		}
		protected virtual string ApplyCatchAllRecoveryMatch(WURFLRequest normalizedRequest)
		{
			return MatcherBase.CatchAllRecoveryMap.DeviceIdOf(normalizedRequest.UserAgent);
		}
		protected bool DeviceExist(string deviceId)
		{
			return this.userAgentsDeviceIdMap.Values.Contains(deviceId);
		}
		private static bool IsBlankOrGeneric(string deviceId)
		{
			return string.IsNullOrEmpty(deviceId) || Constants.Generic.Equals(deviceId);
		}
	}
}
