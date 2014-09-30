using System;
using System.IO;
using System.Text;
namespace Antlr.Runtime
{
	public class ANTLRFileStream : ANTLRStringStream
	{
		protected string fileName;
		public override string SourceName
		{
			get
			{
				return this.fileName;
			}
		}
		protected ANTLRFileStream()
		{
		}
		public ANTLRFileStream(string fileName) : this(fileName, Encoding.Default)
		{
		}
		public ANTLRFileStream(string fileName, Encoding encoding)
		{
			this.fileName = fileName;
			this.Load(fileName, encoding);
		}
		public virtual void Load(string fileName, Encoding encoding)
		{
			if (fileName == null)
			{
				return;
			}
			StreamReader streamReader = null;
			try
			{
				FileInfo file = new FileInfo(fileName);
				int num = (int)this.GetFileLength(file);
				this.data = new char[num];
				if (encoding != null)
				{
					streamReader = new StreamReader(fileName, encoding);
				}
				else
				{
					streamReader = new StreamReader(fileName, Encoding.Default);
				}
				this.n = streamReader.Read(this.data, 0, this.data.Length);
			}
			finally
			{
				if (streamReader != null)
				{
					streamReader.Close();
				}
			}
		}
		private long GetFileLength(FileInfo file)
		{
			if (file.Exists)
			{
				return file.Length;
			}
			return 0L;
		}
	}
}
