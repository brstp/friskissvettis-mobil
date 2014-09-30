using MoMA.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
namespace MoMA.Mobile.Cache
{
	public class CachedOutput
	{
		private static List<CachedOutput> Cache = new List<CachedOutput>();
		public string Hash
		{
			get;
			set;
		}
		public int Width
		{
			get;
			set;
		}
		public string Mode
		{
			get;
			set;
		}
		public string Content
		{
			get;
			set;
		}
		public static void ClearAll()
		{
			CachedOutput.Cache.Clear();
		}
		public static void Set(string OriginalContent, int Width, string Mode, string Content)
		{
			string md5Sum = EncryptHelper.GetMd5Sum(OriginalContent + Width.ToString() + Mode);
			CachedOutput cachedOutput = (
				from c in CachedOutput.Cache
				where c.Width == Width && c.Mode.Equals(Mode)
				select c).FirstOrDefault<CachedOutput>();
			if (cachedOutput == null)
			{
				cachedOutput = new CachedOutput();
				cachedOutput.Hash = md5Sum;
				cachedOutput.Mode = Mode;
				cachedOutput.Width = Width;
				CachedOutput.Cache.Add(cachedOutput);
			}
			cachedOutput.Content = Content;
		}
		public static CachedOutput Get(string OriginalContent, int Width, string Mode)
		{
			string hash = EncryptHelper.GetMd5Sum(OriginalContent + Width.ToString() + Mode);
			return (
				from c in CachedOutput.Cache
				where c.Hash.Equals(hash)
				select c).FirstOrDefault<CachedOutput>();
		}
	}
}
