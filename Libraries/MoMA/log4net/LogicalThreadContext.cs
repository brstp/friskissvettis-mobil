using log4net.Util;
using System;
namespace log4net
{
	public sealed class LogicalThreadContext
	{
		private static readonly LogicalThreadContextProperties s_properties = new LogicalThreadContextProperties();
		private static readonly ThreadContextStacks s_stacks = new ThreadContextStacks(LogicalThreadContext.s_properties);
		public static LogicalThreadContextProperties Properties
		{
			get
			{
				return LogicalThreadContext.s_properties;
			}
		}
		public static ThreadContextStacks Stacks
		{
			get
			{
				return LogicalThreadContext.s_stacks;
			}
		}
		private LogicalThreadContext()
		{
		}
	}
}
