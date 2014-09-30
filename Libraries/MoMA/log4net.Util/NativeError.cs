using System;
using System.Globalization;
using System.Runtime.InteropServices;
namespace log4net.Util
{
	public sealed class NativeError
	{
		private int m_number;
		private string m_message;
		public int Number
		{
			get
			{
				return this.m_number;
			}
		}
		public string Message
		{
			get
			{
				return this.m_message;
			}
		}
		private NativeError(int number, string message)
		{
			this.m_number = number;
			this.m_message = message;
		}
		public static NativeError GetLastError()
		{
			int lastWin32Error = Marshal.GetLastWin32Error();
			return new NativeError(lastWin32Error, NativeError.GetErrorMessage(lastWin32Error));
		}
		public static NativeError GetError(int number)
		{
			return new NativeError(number, NativeError.GetErrorMessage(number));
		}
		public static string GetErrorMessage(int messageId)
		{
			int num = 256;
			int num2 = 512;
			int num3 = 4096;
			string text = "";
			IntPtr intPtr = 0;
			IntPtr arguments = 0;
			if (messageId != 0)
			{
				int num4 = NativeError.FormatMessage(num | num3 | num2, ref intPtr, messageId, 0, ref text, 255, arguments);
				if (num4 > 0)
				{
					text = text.TrimEnd(new char[]
					{
						'\r',
						'\n'
					});
				}
				else
				{
					text = null;
				}
			}
			else
			{
				text = null;
			}
			return text;
		}
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "0x{0:x8}", new object[]
			{
				this.Number
			}) + ((this.Message != null) ? (": " + this.Message) : "");
		}
		[DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int FormatMessage(int dwFlags, ref IntPtr lpSource, int dwMessageId, int dwLanguageId, ref string lpBuffer, int nSize, IntPtr Arguments);
	}
}
