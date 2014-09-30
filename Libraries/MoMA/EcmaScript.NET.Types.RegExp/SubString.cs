using System;
namespace EcmaScript.NET.Types.RegExp
{
	internal class SubString
	{
		internal static readonly SubString EmptySubString = new SubString();
		internal char[] charArray;
		internal int index;
		internal int length;
		public SubString()
		{
		}
		public SubString(string str)
		{
			this.index = 0;
			this.charArray = str.ToCharArray();
			this.length = str.Length;
		}
		public SubString(char[] source, int start, int len)
		{
			this.index = 0;
			this.length = len;
			this.charArray = new char[len];
			Array.Copy(source, start, this.charArray, 0, len);
		}
		public override string ToString()
		{
			return (this.charArray == null) ? string.Empty : new string(this.charArray, this.index, this.length);
		}
	}
}
