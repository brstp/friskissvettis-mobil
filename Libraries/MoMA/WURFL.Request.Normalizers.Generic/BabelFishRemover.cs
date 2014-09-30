using System;
using System.Text.RegularExpressions;
namespace WURFL.Request.Normalizers.Generic
{
	public class BabelFishRemover : IUserAgentNormalizer
	{
		private static readonly Regex BabelFishRegex = new Regex("\\s*\\(via babelfish.yahoo.com\\)\\s*", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		public string Normalize(string userAgent)
		{
			return BabelFishRemover.BabelFishRegex.Replace(userAgent, "");
		}
	}
}
