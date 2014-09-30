using System;
using System.IO;
namespace Antlr.Runtime
{
	public class ANTLRReaderStream : ANTLRStringStream
	{
		public static readonly int READ_BUFFER_SIZE = 1024;
		public static readonly int INITIAL_BUFFER_SIZE = 1024;
		protected ANTLRReaderStream()
		{
		}
		public ANTLRReaderStream(TextReader reader) : this(reader, ANTLRReaderStream.INITIAL_BUFFER_SIZE, ANTLRReaderStream.READ_BUFFER_SIZE)
		{
		}
		public ANTLRReaderStream(TextReader reader, int size) : this(reader, size, ANTLRReaderStream.READ_BUFFER_SIZE)
		{
		}
		public ANTLRReaderStream(TextReader reader, int size, int readChunkSize)
		{
			this.Load(reader, size, readChunkSize);
		}
		public virtual void Load(TextReader reader, int size, int readChunkSize)
		{
			if (reader == null)
			{
				return;
			}
			if (size <= 0)
			{
				size = ANTLRReaderStream.INITIAL_BUFFER_SIZE;
			}
			if (readChunkSize <= 0)
			{
				readChunkSize = ANTLRReaderStream.READ_BUFFER_SIZE;
			}
			try
			{
				this.data = new char[size];
				int num = 0;
				int num2;
				do
				{
					if (num + readChunkSize > this.data.Length)
					{
						char[] array = new char[this.data.Length * 2];
						Array.Copy(this.data, 0, array, 0, this.data.Length);
						this.data = array;
					}
					num2 = reader.Read(this.data, num, readChunkSize);
					num += num2;
				}
				while (num2 != 0);
				this.n = num;
			}
			finally
			{
				reader.Close();
			}
		}
	}
}
