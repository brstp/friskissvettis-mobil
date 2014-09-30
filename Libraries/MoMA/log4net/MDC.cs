using System;
namespace log4net
{
	public sealed class MDC
	{
		private MDC()
		{
		}
		public static string Get(string key)
		{
			object obj = ThreadContext.Properties[key];
			string result;
			if (obj == null)
			{
				result = null;
			}
			else
			{
				result = obj.ToString();
			}
			return result;
		}
		public static void Set(string key, string value)
		{
			ThreadContext.Properties[key] = value;
		}
		public static void Remove(string key)
		{
			ThreadContext.Properties.Remove(key);
		}
		public static void Clear()
		{
			ThreadContext.Properties.Clear();
		}
	}
}
