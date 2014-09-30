using System;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
namespace ImageResizer.Caching
{
	[ComVisible(true)]
	public interface IResponseArgs
	{
		string RequestKey
		{
			get;
		}
		NameValueCollection RewrittenQuerystring
		{
			get;
		}
		string SuggestedExtension
		{
			get;
		}
		IResponseHeaders ResponseHeaders
		{
			get;
			set;
		}
		ModifiedDateDelegate GetModifiedDateUTC
		{
			get;
		}
		bool HasModifiedDate
		{
			get;
		}
		ResizeImageDelegate ResizeImageToStream
		{
			get;
		}
	}
}
