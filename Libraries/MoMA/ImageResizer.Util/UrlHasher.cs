using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
namespace ImageResizer.Util
{
	[ComVisible(true)]
	public class UrlHasher
	{
		public string hash(string data, int subfolders, string dirSeparator)
		{
			SHA256 sHA = SHA256.Create();
			byte[] array = sHA.ComputeHash(new UTF8Encoding().GetBytes(data));
			string str = "";
			if (subfolders > 0)
			{
				str = this.getSubfolder(array, subfolders) + dirSeparator;
			}
			return str + this.Base16Encode(array);
		}
		protected string getSubfolder(byte[] hash, int subfolders)
		{
			int num = (int)Math.Ceiling(Math.Log((double)subfolders, 2.0));
			byte[] array = new byte[(int)Math.Ceiling((double)num / 8.0)];
			Array.Copy(hash, hash.Length - array.Length, array, 0, array.Length);
			array[0] = (byte)(array[0] >> array.Length * 8 - num);
			return this.Base16Encode(array);
		}
		protected string Base16Encode(byte[] bytes)
		{
			StringBuilder stringBuilder = new StringBuilder(bytes.Length * 2);
			for (int i = 0; i < bytes.Length; i++)
			{
				byte b = bytes[i];
				stringBuilder.Append(b.ToString("x").PadLeft(2, '0'));
			}
			return stringBuilder.ToString();
		}
	}
}
