using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public class UniqueTag
	{
		public static readonly UniqueTag NotFound = new UniqueTag("NotFound");
		public static readonly UniqueTag NullValue = new UniqueTag("NullValue");
		public static readonly UniqueTag DoubleMark = new UniqueTag("DoubleMark");
		public static readonly UniqueTag LongMark = new UniqueTag("LongMark");
		private string tagName;
		private UniqueTag(string tagName)
		{
			this.tagName = tagName;
		}
		public override string ToString()
		{
			return "UniqueTag." + this.tagName;
		}
	}
}
