using log4net.Core;
using log4net.Util;
using System;
using System.IO;
namespace log4net.Layout.Pattern
{
	internal abstract class NamedPatternConverter : PatternLayoutConverter, IOptionHandler
	{
		protected int m_precision = 0;
		public void ActivateOptions()
		{
			this.m_precision = 0;
			if (this.Option != null)
			{
				string text = this.Option.Trim();
				if (text.Length > 0)
				{
					int num;
					if (SystemInfo.TryParse(text, out num))
					{
						if (num <= 0)
						{
							LogLog.Error("NamedPatternConverter: Precision option (" + text + ") isn't a positive integer.");
						}
						else
						{
							this.m_precision = num;
						}
					}
					else
					{
						LogLog.Error("NamedPatternConverter: Precision option \"" + text + "\" not a decimal integer.");
					}
				}
			}
		}
		protected abstract string GetFullyQualifiedName(LoggingEvent loggingEvent);
		protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
		{
			string fullyQualifiedName = this.GetFullyQualifiedName(loggingEvent);
			if (this.m_precision <= 0)
			{
				writer.Write(fullyQualifiedName);
			}
			else
			{
				int length = fullyQualifiedName.Length;
				int num = length - 1;
				for (int i = this.m_precision; i > 0; i--)
				{
					num = fullyQualifiedName.LastIndexOf('.', num - 1);
					if (num == -1)
					{
						writer.Write(fullyQualifiedName);
						return;
					}
				}
				writer.Write(fullyQualifiedName.Substring(num + 1, length - num - 1));
			}
		}
	}
}
