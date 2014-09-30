using System;
using System.Web;
namespace WURFL.Request
{
	public interface IWURFLRequestFactory
	{
		WURFLRequest CreateRequest(string userAgent);
		WURFLRequest CreateRequest(HttpRequest httpRequest);
	}
}
