using ImageResizer.Caching;
using System;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Web;
namespace ImageResizer.Configuration
{
	[ComVisible(true)]
	public interface IPipelineConfig
	{
		string ModifiedQueryStringKey
		{
			get;
		}
		string ResponseArgsKey
		{
			get;
		}
		string ModifiedPathKey
		{
			get;
		}
		VppUsageOption VppUsage
		{
			get;
		}
		string SkipFileTypeCheckKey
		{
			get;
		}
		bool SkipFileTypeCheck
		{
			get;
		}
		string PreRewritePath
		{
			get;
		}
		NameValueCollection ModifiedQueryString
		{
			get;
			set;
		}
		bool IsAcceptedImageType(string filePath);
		bool HasPipelineDirective(NameValueCollection q);
		string TrimFakeExtensions(string path);
		ImageBuilder GetImageBuilder();
		ICacheProvider GetCacheProvider();
		object GetFile(string virtualPath, NameValueCollection queryString);
		bool FileExists(string virtualPath, NameValueCollection queryString);
		void FirePostAuthorizeRequest(IHttpModule sender, HttpContext httpContext);
		void FireRewritingEvents(IHttpModule sender, HttpContext context, IUrlEventArgs ue);
		void FireAuthorizeImage(IHttpModule sender, HttpContext context, IUrlAuthorizationEventArgs urlEventArgs);
		void FirePreHandleImage(IHttpModule sender, HttpContext context, IResponseArgs e);
		void FireImageMissing(IHttpModule sender, HttpContext context, IUrlEventArgs urlEventArgs);
	}
}
