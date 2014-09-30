using log4net.Core;
using System;
using System.IO;
using System.Threading;
namespace log4net.Util.PatternStringConverters
{
	internal sealed class RandomStringPatternConverter : PatternConverter, IOptionHandler
	{
		private static readonly Random s_random = new Random();
		private int m_length = 4;
		public void ActivateOptions()
		{
			string option = this.Option;
			if (option != null && option.Length > 0)
			{
				int length;
				if (SystemInfo.TryParse(option, out length))
				{
					this.m_length = length;
				}
				else
				{
					LogLog.Error("RandomStringPatternConverter: Could not convert Option [" + option + "] to Length Int32");
				}
			}
		}
		protected override void Convert(TextWriter writer, object state)
		{
			try
			{
				Random obj;
				Monitor.Enter(obj = RandomStringPatternConverter.s_random);
				try
				{
					for (int i = 0; i < this.m_length; i++)
					{
						int num = RandomStringPatternConverter.s_random.Next(36);
						if (num < 26)
						{
							char value = (char)(65 + num);
							writer.Write(value);
						}
						else
						{
							if (num < 36)
							{
								char value = (char)(48 + (num - 26));
								writer.Write(value);
							}
							else
							{
								writer.Write('X');
							}
						}
					}
				}
				finally
				{
					Monitor.Exit(obj);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error("RandomStringPatternConverter: Error occurred while converting.", exception);
			}
		}
	}
}
