using log4net.Repository;
using System;
using System.Collections;
using System.Threading;
namespace log4net.Core
{
	public class WrapperMap
	{
		private readonly Hashtable m_repositories = new Hashtable();
		private readonly WrapperCreationHandler m_createWrapperHandler;
		private readonly LoggerRepositoryShutdownEventHandler m_shutdownHandler;
		protected Hashtable Repositories
		{
			get
			{
				return this.m_repositories;
			}
		}
		public WrapperMap(WrapperCreationHandler createWrapperHandler)
		{
			this.m_createWrapperHandler = createWrapperHandler;
			this.m_shutdownHandler = new LoggerRepositoryShutdownEventHandler(this.ILoggerRepository_Shutdown);
		}
		public virtual ILoggerWrapper GetWrapper(ILogger logger)
		{
			ILoggerWrapper result;
			if (logger == null)
			{
				result = null;
			}
			else
			{
				Monitor.Enter(this);
				try
				{
					Hashtable hashtable = (Hashtable)this.m_repositories[logger.Repository];
					if (hashtable == null)
					{
						hashtable = new Hashtable();
						this.m_repositories[logger.Repository] = hashtable;
						logger.Repository.ShutdownEvent += this.m_shutdownHandler;
					}
					ILoggerWrapper loggerWrapper = hashtable[logger] as ILoggerWrapper;
					if (loggerWrapper == null)
					{
						loggerWrapper = this.CreateNewWrapperObject(logger);
						hashtable[logger] = loggerWrapper;
					}
					result = loggerWrapper;
				}
				finally
				{
					Monitor.Exit(this);
				}
			}
			return result;
		}
		protected virtual ILoggerWrapper CreateNewWrapperObject(ILogger logger)
		{
			ILoggerWrapper result;
			if (this.m_createWrapperHandler != null)
			{
				result = this.m_createWrapperHandler(logger);
			}
			else
			{
				result = null;
			}
			return result;
		}
		protected virtual void RepositoryShutdown(ILoggerRepository repository)
		{
			Monitor.Enter(this);
			try
			{
				this.m_repositories.Remove(repository);
				repository.ShutdownEvent -= this.m_shutdownHandler;
			}
			finally
			{
				Monitor.Exit(this);
			}
		}
		private void ILoggerRepository_Shutdown(object sender, EventArgs e)
		{
			ILoggerRepository loggerRepository = sender as ILoggerRepository;
			if (loggerRepository != null)
			{
				this.RepositoryShutdown(loggerRepository);
			}
		}
	}
}
