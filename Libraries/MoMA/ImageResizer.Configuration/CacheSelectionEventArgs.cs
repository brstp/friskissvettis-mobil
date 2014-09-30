using ImageResizer.Caching;
using System;
using System.Web;
namespace ImageResizer.Configuration
{
	internal class CacheSelectionEventArgs : ICacheSelectionEventArgs
	{
		private HttpContext context;
		private IResponseArgs responseArgs;
		private ICache selectedCache;
		public HttpContext Context
		{
			get
			{
				return this.context;
			}
		}
		public IResponseArgs ResponseArgs
		{
			get
			{
				return this.responseArgs;
			}
		}
		public ICache SelectedCache
		{
			get
			{
				return this.selectedCache;
			}
			set
			{
				this.selectedCache = value;
			}
		}
		public CacheSelectionEventArgs(HttpContext context, IResponseArgs responseArgs, ICache defaultCache)
		{
			this.context = context;
			this.responseArgs = responseArgs;
			this.selectedCache = defaultCache;
		}
	}
}
