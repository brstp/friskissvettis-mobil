using System;
using System.Net;
namespace log4net.Util.TypeConverters
{
	internal class IPAddressConverter : IConvertFrom
	{
		private static readonly char[] validIpAddressChars = new char[]
		{
			'0',
			'1',
			'2',
			'3',
			'4',
			'5',
			'6',
			'7',
			'8',
			'9',
			'a',
			'b',
			'c',
			'd',
			'e',
			'f',
			'A',
			'B',
			'C',
			'D',
			'E',
			'F',
			'x',
			'X',
			'.',
			':',
			'%'
		};
		public bool CanConvertFrom(Type sourceType)
		{
			return sourceType == typeof(string);
		}
		public object ConvertFrom(object source)
		{
			string text = source as string;
			if (text != null && text.Length > 0)
			{
				try
				{
					IPHostEntry hostEntry = Dns.GetHostEntry(text);
					if (hostEntry != null && hostEntry.AddressList != null && hostEntry.AddressList.Length > 0 && hostEntry.AddressList[0] != null)
					{
						return hostEntry.AddressList[0];
					}
				}
				catch (Exception innerException)
				{
					throw ConversionNotSupportedException.Create(typeof(IPAddress), source, innerException);
				}
			}
			throw ConversionNotSupportedException.Create(typeof(IPAddress), source);
		}
	}
}
