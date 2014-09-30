using System;
using System.Web;
namespace WURFL.Request
{
	public interface IUserAgentResolver
	{
		string Resolve(HttpRequest httpRequest);
	}
}
