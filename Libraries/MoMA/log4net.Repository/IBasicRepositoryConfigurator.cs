using log4net.Appender;
using System;
namespace log4net.Repository
{
	public interface IBasicRepositoryConfigurator
	{
		void Configure(IAppender appender);
	}
}
