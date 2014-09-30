using System;
using System.IO;
using System.Runtime.InteropServices;
namespace ImageResizer.Util
{
	[ComVisible(true)]
	public class StreamUtils
	{
		public static MemoryStream CopyStream(Stream s)
		{
			MemoryStream memoryStream = new MemoryStream((int)s.Length + 8);
			StreamUtils.CopyTo(s, memoryStream);
			memoryStream.Position = 0L;
			return memoryStream;
		}
		public static void CopyTo(Stream src, Stream dest)
		{
			int num = src.CanSeek ? Math.Min((int)(src.Length - src.Position), 8192) : 8192;
			byte[] array = new byte[num];
			int num2;
			do
			{
				num2 = src.Read(array, 0, array.Length);
				dest.Write(array, 0, num2);
			}
			while (num2 != 0);
		}
		public static void CopyTo(MemoryStream src, Stream dest)
		{
			dest.Write(src.GetBuffer(), (int)src.Position, (int)(src.Length - src.Position));
		}
		public static void CopyTo(Stream src, MemoryStream dest)
		{
			if (src.CanSeek)
			{
				int i = (int)dest.Position;
				int num = (int)(src.Length - src.Position) + i;
				dest.SetLength((long)num);
				while (i < num)
				{
					i += src.Read(dest.GetBuffer(), i, num - i);
				}
				return;
			}
			StreamUtils.CopyTo(src, dest);
		}
	}
}
