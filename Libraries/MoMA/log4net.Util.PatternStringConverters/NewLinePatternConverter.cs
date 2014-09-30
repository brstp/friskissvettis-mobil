using log4net.Core;
using System;
using System.Globalization;
namespace log4net.Util.PatternStringConverters
{
	internal sealed class NewLinePatternConverter : LiteralPatternConverter, IOptionHandler
	{
		public void ActivateOptions()
		{
			if (string.Compare(this.Option, "DOS", true, CultureInfo.InvariantCulture) == 0)
			{
				this.Option = "\r\n";
			}
			else
			{
				if (string.Compare(this.Option, "UNIX", true, CultureInfo.InvariantCulture) == 0)
				{
					this.Option = "\n";
				}
				else
				{
					this.Option = SystemInfo.NewLine;
				}
			}
		}
	}
}
