using log4net.Util;
using System;
namespace log4net
{
	public sealed class ThreadContext
	{
		private static readonly ThreadContextProperties s_properties = new ThreadContextProperties();
		private static readonly ThreadContextStacks s_stacks = new ThreadContextStacks(ThreadContext.s_properties);
		public static ThreadContextProperties Properties
		{
			get
			{
				return ThreadContext.s_properties;
			}
		}
		public static ThreadContextStacks Stacks
		{
			get
			{
				return ThreadContext.s_stacks;
			}
		}
		private ThreadContext()
		{
		}
	}
}
