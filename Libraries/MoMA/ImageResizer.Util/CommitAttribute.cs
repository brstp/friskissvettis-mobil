using System;
using System.Runtime.InteropServices;
namespace ImageResizer.Util
{
	[AttributeUsage(AttributeTargets.Assembly), ComVisible(true)]
	public class CommitAttribute : Attribute
	{
		private string guid;
		public string Value
		{
			get
			{
				return this.guid;
			}
		}
		public CommitAttribute()
		{
			this.guid = string.Empty;
		}
		public CommitAttribute(string txt)
		{
			this.guid = txt;
		}
		public override string ToString()
		{
			return this.guid;
		}
	}
}
