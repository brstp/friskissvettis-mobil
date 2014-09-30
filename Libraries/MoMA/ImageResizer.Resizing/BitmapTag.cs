using System;
using System.IO;
using System.Runtime.InteropServices;
namespace ImageResizer.Resizing
{
	[ComVisible(true)]
	public class BitmapTag
	{
		private string _path;
		private Stream _source;
		public string Path
		{
			get
			{
				return this._path;
			}
			set
			{
				this._path = value;
			}
		}
		public Stream Source
		{
			get
			{
				return this._source;
			}
			set
			{
				this._source = value;
			}
		}
		public BitmapTag(object tag)
		{
			if (tag is string)
			{
				this._path = (string)tag;
			}
			if (tag is BitmapTag)
			{
				this._path = ((BitmapTag)tag).Path;
				this._source = ((BitmapTag)tag).Source;
			}
		}
		public BitmapTag(string path, Stream source)
		{
			this._path = path;
			this._source = source;
		}
	}
}
