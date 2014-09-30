using log4net.Util;
using System;
namespace log4net.Core
{
	public class SecurityContextProvider
	{
		private static SecurityContextProvider s_defaultProvider = new SecurityContextProvider();
		public static SecurityContextProvider DefaultProvider
		{
			get
			{
				return SecurityContextProvider.s_defaultProvider;
			}
			set
			{
				SecurityContextProvider.s_defaultProvider = value;
			}
		}
		protected SecurityContextProvider()
		{
		}
		public virtual SecurityContext CreateSecurityContext(object consumer)
		{
			return NullSecurityContext.Instance;
		}
	}
}
