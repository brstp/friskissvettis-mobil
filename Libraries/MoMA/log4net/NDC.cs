using log4net.Util;
using System;
using System.Collections;
namespace log4net
{
	public sealed class NDC
	{
		public static int Depth
		{
			get
			{
				return ThreadContext.Stacks["NDC"].Count;
			}
		}
		private NDC()
		{
		}
		public static void Clear()
		{
			ThreadContext.Stacks["NDC"].Clear();
		}
		public static Stack CloneStack()
		{
			return ThreadContext.Stacks["NDC"].InternalStack;
		}
		public static void Inherit(Stack stack)
		{
			ThreadContext.Stacks["NDC"].InternalStack = stack;
		}
		public static string Pop()
		{
			return ThreadContext.Stacks["NDC"].Pop();
		}
		public static IDisposable Push(string message)
		{
			return ThreadContext.Stacks["NDC"].Push(message);
		}
		public static void Remove()
		{
		}
		public static void SetMaxDepth(int maxDepth)
		{
			if (maxDepth >= 0)
			{
				ThreadContextStack threadContextStack = ThreadContext.Stacks["NDC"];
				if (maxDepth == 0)
				{
					threadContextStack.Clear();
				}
				else
				{
					while (threadContextStack.Count > maxDepth)
					{
						threadContextStack.Pop();
					}
				}
			}
		}
	}
}
