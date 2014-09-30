using log4net.Util;
using System;
namespace log4net
{
	public sealed class GlobalContext
	{
		private static readonly GlobalContextProperties s_properties;
		public static GlobalContextProperties Properties
		{
			get
			{
				return GlobalContext.s_properties;
			}
		}
		private GlobalContext()
		{
		}
		static GlobalContext()
		{
			GlobalContext.s_properties = new GlobalContextProperties();
			GlobalContext.Properties["log4net:HostName"] = SystemInfo.HostName;
		}
	}
}
