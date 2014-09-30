using System;
using System.IO;
using System.Security;
using System.Threading;
namespace log4net.Util.PatternStringConverters
{
	internal sealed class IdentityPatternConverter : PatternConverter
	{
		protected override void Convert(TextWriter writer, object state)
		{
			try
			{
				if (Thread.CurrentPrincipal != null && Thread.CurrentPrincipal.Identity != null && Thread.CurrentPrincipal.Identity.Name != null)
				{
					writer.Write(Thread.CurrentPrincipal.Identity.Name);
				}
			}
			catch (SecurityException)
			{
				LogLog.Debug("IdentityPatternConverter: Security exception while trying to get current thread principal. Error Ignored.");
				writer.Write(SystemInfo.NotAvailableText);
			}
		}
	}
}
