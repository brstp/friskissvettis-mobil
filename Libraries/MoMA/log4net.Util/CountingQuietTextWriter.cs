using log4net.Core;
using System;
using System.IO;
namespace log4net.Util
{
	public class CountingQuietTextWriter : QuietTextWriter
	{
		private long m_countBytes;
		public long Count
		{
			get
			{
				return this.m_countBytes;
			}
			set
			{
				this.m_countBytes = value;
			}
		}
		public CountingQuietTextWriter(TextWriter writer, IErrorHandler errorHandler) : base(writer, errorHandler)
		{
			this.m_countBytes = 0L;
		}
		public override void Write(char value)
		{
			try
			{
				base.Write(value);
				this.m_countBytes += (long)this.Encoding.GetByteCount(new char[]
				{
					value
				});
			}
			catch (Exception e)
			{
				base.ErrorHandler.Error("Failed to write [" + value + "].", e, ErrorCode.WriteFailure);
			}
		}
		public override void Write(char[] buffer, int index, int count)
		{
			if (count > 0)
			{
				try
				{
					base.Write(buffer, index, count);
					this.m_countBytes += (long)this.Encoding.GetByteCount(buffer, index, count);
				}
				catch (Exception e)
				{
					base.ErrorHandler.Error("Failed to write buffer.", e, ErrorCode.WriteFailure);
				}
			}
		}
		public override void Write(string str)
		{
			if (str != null && str.Length > 0)
			{
				try
				{
					base.Write(str);
					this.m_countBytes += (long)this.Encoding.GetByteCount(str);
				}
				catch (Exception e)
				{
					base.ErrorHandler.Error("Failed to write [" + str + "].", e, ErrorCode.WriteFailure);
				}
			}
		}
	}
}
