using System;
namespace log4net.Util
{
	public sealed class ThreadContextStacks
	{
		private readonly ContextPropertiesBase m_properties;
		public ThreadContextStack this[string key]
		{
			get
			{
				object obj = this.m_properties[key];
				ThreadContextStack threadContextStack;
				if (obj == null)
				{
					threadContextStack = new ThreadContextStack();
					this.m_properties[key] = threadContextStack;
				}
				else
				{
					threadContextStack = (obj as ThreadContextStack);
					if (threadContextStack == null)
					{
						string text = SystemInfo.NullText;
						try
						{
							text = obj.ToString();
						}
						catch
						{
						}
						LogLog.Error(string.Concat(new string[]
						{
							"ThreadContextStacks: Request for stack named [",
							key,
							"] failed because a property with the same name exists which is a [",
							obj.GetType().Name,
							"] with value [",
							text,
							"]"
						}));
						threadContextStack = new ThreadContextStack();
					}
				}
				return threadContextStack;
			}
		}
		internal ThreadContextStacks(ContextPropertiesBase properties)
		{
			this.m_properties = properties;
		}
	}
}
