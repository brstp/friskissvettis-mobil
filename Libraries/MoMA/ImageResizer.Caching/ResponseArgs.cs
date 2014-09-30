using System;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
namespace ImageResizer.Caching
{
	[ComVisible(true)]
	public class ResponseArgs : IResponseArgs
	{
		protected ModifiedDateDelegate getModifiedDateUTC;
		protected ResizeImageDelegate resizeImageToStream;
		protected string requestKey;
		protected string suggestedExtension;
		protected bool hasModifiedDate;
		protected IResponseHeaders responseHeaders = new ResponseHeaders();
		protected NameValueCollection rewrittenQuerystring;
		public ModifiedDateDelegate GetModifiedDateUTC
		{
			get
			{
				return this.getModifiedDateUTC;
			}
			set
			{
				this.getModifiedDateUTC = value;
			}
		}
		public ResizeImageDelegate ResizeImageToStream
		{
			get
			{
				return this.resizeImageToStream;
			}
			set
			{
				this.resizeImageToStream = value;
			}
		}
		public string RequestKey
		{
			get
			{
				return this.requestKey;
			}
			set
			{
				this.requestKey = value;
			}
		}
		public string SuggestedExtension
		{
			get
			{
				return this.suggestedExtension;
			}
			set
			{
				this.suggestedExtension = value;
			}
		}
		public bool HasModifiedDate
		{
			get
			{
				return this.hasModifiedDate;
			}
			set
			{
				this.hasModifiedDate = value;
			}
		}
		public IResponseHeaders ResponseHeaders
		{
			get
			{
				return this.responseHeaders;
			}
			set
			{
				this.responseHeaders = value;
			}
		}
		public NameValueCollection RewrittenQuerystring
		{
			get
			{
				return this.rewrittenQuerystring;
			}
			set
			{
				this.rewrittenQuerystring = value;
			}
		}
	}
}
