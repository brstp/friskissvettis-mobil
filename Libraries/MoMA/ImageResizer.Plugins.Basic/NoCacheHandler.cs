using ImageResizer.Caching;
using System;
using System.Runtime.InteropServices;
using System.Web;
namespace ImageResizer.Plugins.Basic
{
	[ComVisible(true)]
	public class NoCacheHandler : IHttpHandler
	{
		private IResponseArgs e;
		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
		public NoCacheHandler(IResponseArgs e)
		{
			this.e = e;
		}
		public void ProcessRequest(HttpContext context)
		{
			context.Response.StatusCode = 200;
			context.Response.BufferOutput = true;
			this.e.ResponseHeaders.ApplyDuringPreSendRequestHeaders = false;
			this.e.ResponseHeaders.ApplyToResponse(this.e.ResponseHeaders, context);
			this.e.ResizeImageToStream(context.Response.OutputStream);
		}
	}
}
