using log4net.Core;
using System;
namespace log4net.Repository.Hierarchy
{
	internal class DefaultLoggerFactory : ILoggerFactory
	{
		internal sealed class LoggerImpl : Logger
		{
			internal LoggerImpl(string name) : base(name)
			{
			}
		}
		internal DefaultLoggerFactory()
		{
		}
		public Logger CreateLogger(string name)
		{
			Logger result;
			if (name == null)
			{
				result = new RootLogger(Level.Debug);
			}
			else
			{
				result = new DefaultLoggerFactory.LoggerImpl(name);
			}
			return result;
		}
	}
}
