using log4net.Repository;
using log4net.Util;
using System;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
namespace log4net.Core
{
	public class CompactRepositorySelector : IRepositorySelector
	{
		private const string DefaultRepositoryName = "log4net-default-repository";
		private readonly Hashtable m_name2repositoryMap = new Hashtable();
		private readonly Type m_defaultRepositoryType;
		private event LoggerRepositoryCreationEventHandler m_loggerRepositoryCreatedEvent
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.m_loggerRepositoryCreatedEvent = (LoggerRepositoryCreationEventHandler)Delegate.Combine(this.m_loggerRepositoryCreatedEvent, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.m_loggerRepositoryCreatedEvent = (LoggerRepositoryCreationEventHandler)Delegate.Remove(this.m_loggerRepositoryCreatedEvent, value);
			}
		}
		public event LoggerRepositoryCreationEventHandler LoggerRepositoryCreatedEvent
		{
			add
			{
				this.m_loggerRepositoryCreatedEvent = (LoggerRepositoryCreationEventHandler)Delegate.Combine(this.m_loggerRepositoryCreatedEvent, value);
			}
			remove
			{
				this.m_loggerRepositoryCreatedEvent = (LoggerRepositoryCreationEventHandler)Delegate.Remove(this.m_loggerRepositoryCreatedEvent, value);
			}
		}
		public CompactRepositorySelector(Type defaultRepositoryType)
		{
			if (defaultRepositoryType == null)
			{
				throw new ArgumentNullException("defaultRepositoryType");
			}
			if (!typeof(ILoggerRepository).IsAssignableFrom(defaultRepositoryType))
			{
				throw SystemInfo.CreateArgumentOutOfRangeException("defaultRepositoryType", defaultRepositoryType, "Parameter: defaultRepositoryType, Value: [" + defaultRepositoryType + "] out of range. Argument must implement the ILoggerRepository interface");
			}
			this.m_defaultRepositoryType = defaultRepositoryType;
			LogLog.Debug("CompactRepositorySelector: defaultRepositoryType [" + this.m_defaultRepositoryType + "]");
		}
		public ILoggerRepository GetRepository(Assembly assembly)
		{
			return this.CreateRepository(assembly, this.m_defaultRepositoryType);
		}
		public ILoggerRepository GetRepository(string repositoryName)
		{
			if (repositoryName == null)
			{
				throw new ArgumentNullException("repositoryName");
			}
			Monitor.Enter(this);
			ILoggerRepository result;
			try
			{
				ILoggerRepository loggerRepository = this.m_name2repositoryMap[repositoryName] as ILoggerRepository;
				if (loggerRepository == null)
				{
					throw new LogException("Repository [" + repositoryName + "] is NOT defined.");
				}
				result = loggerRepository;
			}
			finally
			{
				Monitor.Exit(this);
			}
			return result;
		}
		public ILoggerRepository CreateRepository(Assembly assembly, Type repositoryType)
		{
			if (repositoryType == null)
			{
				repositoryType = this.m_defaultRepositoryType;
			}
			Monitor.Enter(this);
			ILoggerRepository result;
			try
			{
				ILoggerRepository loggerRepository = this.m_name2repositoryMap["log4net-default-repository"] as ILoggerRepository;
				if (loggerRepository == null)
				{
					loggerRepository = this.CreateRepository("log4net-default-repository", repositoryType);
				}
				result = loggerRepository;
			}
			finally
			{
				Monitor.Exit(this);
			}
			return result;
		}
		public ILoggerRepository CreateRepository(string repositoryName, Type repositoryType)
		{
			if (repositoryName == null)
			{
				throw new ArgumentNullException("repositoryName");
			}
			if (repositoryType == null)
			{
				repositoryType = this.m_defaultRepositoryType;
			}
			Monitor.Enter(this);
			ILoggerRepository result;
			try
			{
				ILoggerRepository loggerRepository = this.m_name2repositoryMap[repositoryName] as ILoggerRepository;
				if (loggerRepository != null)
				{
					throw new LogException("Repository [" + repositoryName + "] is already defined. Repositories cannot be redefined.");
				}
				LogLog.Debug(string.Concat(new object[]
				{
					"DefaultRepositorySelector: Creating repository [",
					repositoryName,
					"] using type [",
					repositoryType,
					"]"
				}));
				loggerRepository = (ILoggerRepository)Activator.CreateInstance(repositoryType);
				loggerRepository.Name = repositoryName;
				this.m_name2repositoryMap[repositoryName] = loggerRepository;
				this.OnLoggerRepositoryCreatedEvent(loggerRepository);
				result = loggerRepository;
			}
			finally
			{
				Monitor.Exit(this);
			}
			return result;
		}
		public bool ExistsRepository(string repositoryName)
		{
			Monitor.Enter(this);
			bool result;
			try
			{
				result = this.m_name2repositoryMap.ContainsKey(repositoryName);
			}
			finally
			{
				Monitor.Exit(this);
			}
			return result;
		}
		public ILoggerRepository[] GetAllRepositories()
		{
			Monitor.Enter(this);
			ILoggerRepository[] result;
			try
			{
				ICollection values = this.m_name2repositoryMap.Values;
				ILoggerRepository[] array = new ILoggerRepository[values.Count];
				values.CopyTo(array, 0);
				result = array;
			}
			finally
			{
				Monitor.Exit(this);
			}
			return result;
		}
		protected virtual void OnLoggerRepositoryCreatedEvent(ILoggerRepository repository)
		{
			LoggerRepositoryCreationEventHandler loggerRepositoryCreatedEvent = this.m_loggerRepositoryCreatedEvent;
			if (loggerRepositoryCreatedEvent != null)
			{
				loggerRepositoryCreatedEvent(this, new LoggerRepositoryCreationEventArgs(repository));
			}
		}
	}
}
