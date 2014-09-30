using System;
namespace log4net.Repository.Hierarchy
{
	public interface ILoggerFactory
	{
		Logger CreateLogger(string name);
	}
}
