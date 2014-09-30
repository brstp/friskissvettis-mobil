using log4net.Appender;
using log4net.Layout;
using log4net.Repository;
using log4net.Util;
using System;
using System.Reflection;
namespace log4net.Config
{
	public sealed class BasicConfigurator
	{
		private BasicConfigurator()
		{
		}
		public static void Configure()
		{
			BasicConfigurator.Configure(LogManager.GetRepository(Assembly.GetCallingAssembly()));
		}
		public static void Configure(IAppender appender)
		{
			BasicConfigurator.Configure(LogManager.GetRepository(Assembly.GetCallingAssembly()), appender);
		}
		public static void Configure(ILoggerRepository repository)
		{
			PatternLayout patternLayout = new PatternLayout();
			patternLayout.ConversionPattern = "%timestamp [%thread] %level %logger %ndc - %message%newline";
			patternLayout.ActivateOptions();
			ConsoleAppender consoleAppender = new ConsoleAppender();
			consoleAppender.Layout = patternLayout;
			consoleAppender.ActivateOptions();
			BasicConfigurator.Configure(repository, consoleAppender);
		}
		public static void Configure(ILoggerRepository repository, IAppender appender)
		{
			IBasicRepositoryConfigurator basicRepositoryConfigurator = repository as IBasicRepositoryConfigurator;
			if (basicRepositoryConfigurator != null)
			{
				basicRepositoryConfigurator.Configure(appender);
			}
			else
			{
				LogLog.Warn("BasicConfigurator: Repository [" + repository + "] does not support the BasicConfigurator");
			}
		}
	}
}
