using System;
using System.IO;
using System.Security;
namespace log4net.Util.PatternStringConverters
{
	internal sealed class EnvironmentPatternConverter : PatternConverter
	{
		protected override void Convert(TextWriter writer, object state)
		{
			try
			{
				if (this.Option != null && this.Option.Length > 0)
				{
					string environmentVariable = Environment.GetEnvironmentVariable(this.Option);
					if (environmentVariable != null && environmentVariable.Length > 0)
					{
						writer.Write(environmentVariable);
					}
				}
			}
			catch (SecurityException exception)
			{
				LogLog.Debug("EnvironmentPatternConverter: Security exception while trying to expand environment variables. Error Ignored. No Expansion.", exception);
			}
			catch (Exception exception2)
			{
				LogLog.Error("EnvironmentPatternConverter: Error occurred while converting environment variable.", exception2);
			}
		}
	}
}
