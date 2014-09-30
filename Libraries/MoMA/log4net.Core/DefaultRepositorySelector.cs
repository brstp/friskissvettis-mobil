using log4net.Config;
using log4net.Plugin;
using log4net.Repository;
using log4net.Util;
using System;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
namespace log4net.Core
{
	public class DefaultRepositorySelector : IRepositorySelector
	{
		private const string DefaultRepositoryName = "log4net-default-repository";
		private readonly Hashtable m_name2repositoryMap = new Hashtable();
		private readonly Hashtable m_assembly2repositoryMap = new Hashtable();
		private readonly Hashtable m_alias2repositoryMap = new Hashtable();
		private readonly Type m_defaultRepositoryType;
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
		public DefaultRepositorySelector(Type defaultRepositoryType)
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
			LogLog.Debug("DefaultRepositorySelector: defaultRepositoryType [" + this.m_defaultRepositoryType + "]");
		}
		public ILoggerRepository GetRepository(Assembly repositoryAssembly)
		{
			if (repositoryAssembly == null)
			{
				throw new ArgumentNullException("repositoryAssembly");
			}
			return this.CreateRepository(repositoryAssembly, this.m_defaultRepositoryType);
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
		public ILoggerRepository CreateRepository(Assembly repositoryAssembly, Type repositoryType)
		{
			return this.CreateRepository(repositoryAssembly, repositoryType, "log4net-default-repository", true);
		}
		public ILoggerRepository CreateRepository(Assembly repositoryAssembly, Type repositoryType, string repositoryName, bool readAssemblyAttributes)
		{
			if (repositoryAssembly == null)
			{
				throw new ArgumentNullException("repositoryAssembly");
			}
			if (repositoryType == null)
			{
				repositoryType = this.m_defaultRepositoryType;
			}
			Monitor.Enter(this);
			ILoggerRepository result;
			try
			{
				ILoggerRepository loggerRepository = this.m_assembly2repositoryMap[repositoryAssembly] as ILoggerRepository;
				if (loggerRepository == null)
				{
					LogLog.Debug("DefaultRepositorySelector: Creating repository for assembly [" + repositoryAssembly + "]");
					string text = repositoryName;
					Type type = repositoryType;
					if (readAssemblyAttributes)
					{
						this.GetInfoForAssembly(repositoryAssembly, ref text, ref type);
					}
					LogLog.Debug(string.Concat(new object[]
					{
						"DefaultRepositorySelector: Assembly [",
						repositoryAssembly,
						"] using repository [",
						text,
						"] and repository type [",
						type,
						"]"
					}));
					loggerRepository = (this.m_name2repositoryMap[text] as ILoggerRepository);
					if (loggerRepository == null)
					{
						loggerRepository = this.CreateRepository(text, type);
						if (readAssemblyAttributes)
						{
							try
							{
								this.LoadAliases(repositoryAssembly, loggerRepository);
								this.LoadPlugins(repositoryAssembly, loggerRepository);
								this.ConfigureRepository(repositoryAssembly, loggerRepository);
							}
							catch (Exception exception)
							{
								LogLog.Error("DefaultRepositorySelector: Failed to configure repository [" + text + "] from assembly attributes.", exception);
							}
						}
					}
					else
					{
						LogLog.Debug(string.Concat(new string[]
						{
							"DefaultRepositorySelector: repository [",
							text,
							"] already exists, using repository type [",
							loggerRepository.GetType().FullName,
							"]"
						}));
						if (readAssemblyAttributes)
						{
							try
							{
								this.LoadPlugins(repositoryAssembly, loggerRepository);
							}
							catch (Exception exception)
							{
								LogLog.Error("DefaultRepositorySelector: Failed to configure repository [" + text + "] from assembly attributes.", exception);
							}
						}
					}
					this.m_assembly2repositoryMap[repositoryAssembly] = loggerRepository;
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
				ILoggerRepository loggerRepository2 = this.m_alias2repositoryMap[repositoryName] as ILoggerRepository;
				if (loggerRepository2 != null)
				{
					if (loggerRepository2.GetType() == repositoryType)
					{
						LogLog.Debug(string.Concat(new string[]
						{
							"DefaultRepositorySelector: Aliasing repository [",
							repositoryName,
							"] to existing repository [",
							loggerRepository2.Name,
							"]"
						}));
						loggerRepository = loggerRepository2;
						this.m_name2repositoryMap[repositoryName] = loggerRepository;
					}
					else
					{
						LogLog.Error(string.Concat(new string[]
						{
							"DefaultRepositorySelector: Failed to alias repository [",
							repositoryName,
							"] to existing repository [",
							loggerRepository2.Name,
							"]. Requested repository type [",
							repositoryType.FullName,
							"] is not compatible with existing type [",
							loggerRepository2.GetType().FullName,
							"]"
						}));
					}
				}
				if (loggerRepository == null)
				{
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
				}
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
		public void AliasRepository(string repositoryAlias, ILoggerRepository repositoryTarget)
		{
			if (repositoryAlias == null)
			{
				throw new ArgumentNullException("repositoryAlias");
			}
			if (repositoryTarget == null)
			{
				throw new ArgumentNullException("repositoryTarget");
			}
			Monitor.Enter(this);
			try
			{
				if (this.m_alias2repositoryMap.Contains(repositoryAlias))
				{
					if (repositoryTarget != (ILoggerRepository)this.m_alias2repositoryMap[repositoryAlias])
					{
						throw new InvalidOperationException(string.Concat(new string[]
						{
							"Repository [",
							repositoryAlias,
							"] is already aliased to repository [",
							((ILoggerRepository)this.m_alias2repositoryMap[repositoryAlias]).Name,
							"]. Aliases cannot be redefined."
						}));
					}
				}
				else
				{
					if (this.m_name2repositoryMap.Contains(repositoryAlias))
					{
						if (repositoryTarget != (ILoggerRepository)this.m_name2repositoryMap[repositoryAlias])
						{
							throw new InvalidOperationException(string.Concat(new string[]
							{
								"Repository [",
								repositoryAlias,
								"] already exists and cannot be aliased to repository [",
								repositoryTarget.Name,
								"]."
							}));
						}
					}
					else
					{
						this.m_alias2repositoryMap[repositoryAlias] = repositoryTarget;
					}
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
		}
		protected virtual void OnLoggerRepositoryCreatedEvent(ILoggerRepository repository)
		{
			LoggerRepositoryCreationEventHandler loggerRepositoryCreatedEvent = this.m_loggerRepositoryCreatedEvent;
			if (loggerRepositoryCreatedEvent != null)
			{
				loggerRepositoryCreatedEvent(this, new LoggerRepositoryCreationEventArgs(repository));
			}
		}
		private void GetInfoForAssembly(Assembly assembly, ref string repositoryName, ref Type repositoryType)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			try
			{
				LogLog.Debug(string.Concat(new string[]
				{
					"DefaultRepositorySelector: Assembly [",
					assembly.FullName,
					"] Loaded From [",
					SystemInfo.AssemblyLocationInfo(assembly),
					"]"
				}));
			}
			catch
			{
			}
			try
			{
				object[] customAttributes = Attribute.GetCustomAttributes(assembly, typeof(RepositoryAttribute), false);
				if (customAttributes == null || customAttributes.Length == 0)
				{
					LogLog.Debug("DefaultRepositorySelector: Assembly [" + assembly + "] does not have a RepositoryAttribute specified.");
				}
				else
				{
					if (customAttributes.Length > 1)
					{
						LogLog.Error("DefaultRepositorySelector: Assembly [" + assembly + "] has multiple log4net.Config.RepositoryAttribute assembly attributes. Only using first occurrence.");
					}
					RepositoryAttribute repositoryAttribute = customAttributes[0] as RepositoryAttribute;
					if (repositoryAttribute == null)
					{
						LogLog.Error("DefaultRepositorySelector: Assembly [" + assembly + "] has a RepositoryAttribute but it does not!.");
					}
					else
					{
						if (repositoryAttribute.Name != null)
						{
							repositoryName = repositoryAttribute.Name;
						}
						if (repositoryAttribute.RepositoryType != null)
						{
							if (typeof(ILoggerRepository).IsAssignableFrom(repositoryAttribute.RepositoryType))
							{
								repositoryType = repositoryAttribute.RepositoryType;
							}
							else
							{
								LogLog.Error("DefaultRepositorySelector: Repository Type [" + repositoryAttribute.RepositoryType + "] must implement the ILoggerRepository interface.");
							}
						}
					}
				}
			}
			catch (Exception exception)
			{
				LogLog.Error("DefaultRepositorySelector: Unhandled exception in GetInfoForAssembly", exception);
			}
		}
		private void ConfigureRepository(Assembly assembly, ILoggerRepository repository)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			if (repository == null)
			{
				throw new ArgumentNullException("repository");
			}
			object[] customAttributes = Attribute.GetCustomAttributes(assembly, typeof(ConfiguratorAttribute), false);
			if (customAttributes != null && customAttributes.Length > 0)
			{
				Array.Sort<object>(customAttributes);
				object[] array = customAttributes;
				for (int i = 0; i < array.Length; i++)
				{
					ConfiguratorAttribute configuratorAttribute = (ConfiguratorAttribute)array[i];
					if (configuratorAttribute != null)
					{
						try
						{
							configuratorAttribute.Configure(assembly, repository);
						}
						catch (Exception exception)
						{
							LogLog.Error("DefaultRepositorySelector: Exception calling [" + configuratorAttribute.GetType().FullName + "] .Configure method.", exception);
						}
					}
				}
			}
			if (repository.Name == "log4net-default-repository")
			{
				string appSetting = SystemInfo.GetAppSetting("log4net.Config");
				if (appSetting != null && appSetting.Length > 0)
				{
					string text = null;
					try
					{
						text = SystemInfo.ApplicationBaseDirectory;
					}
					catch (Exception exception)
					{
						LogLog.Warn("DefaultRepositorySelector: Exception getting ApplicationBaseDirectory. appSettings log4net.Config path [" + appSetting + "] will be treated as an absolute URI", exception);
					}
					Uri uri = null;
					try
					{
						if (text != null)
						{
							uri = new Uri(new Uri(text), appSetting);
						}
						else
						{
							uri = new Uri(appSetting);
						}
					}
					catch (Exception exception)
					{
						LogLog.Error("DefaultRepositorySelector: Exception while parsing log4net.Config file path [" + appSetting + "]", exception);
					}
					if (uri != null)
					{
						LogLog.Debug("DefaultRepositorySelector: Loading configuration for default repository from AppSettings specified Config URI [" + uri.ToString() + "]");
						try
						{
							XmlConfigurator.Configure(repository, uri);
						}
						catch (Exception exception)
						{
							LogLog.Error("DefaultRepositorySelector: Exception calling XmlConfigurator.Configure method with ConfigUri [" + uri + "]", exception);
						}
					}
				}
			}
		}
		private void LoadPlugins(Assembly assembly, ILoggerRepository repository)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			if (repository == null)
			{
				throw new ArgumentNullException("repository");
			}
			object[] customAttributes = Attribute.GetCustomAttributes(assembly, typeof(PluginAttribute), false);
			if (customAttributes != null && customAttributes.Length > 0)
			{
				object[] array = customAttributes;
				for (int i = 0; i < array.Length; i++)
				{
					IPluginFactory pluginFactory = (IPluginFactory)array[i];
					try
					{
						repository.PluginMap.Add(pluginFactory.CreatePlugin());
					}
					catch (Exception exception)
					{
						LogLog.Error("DefaultRepositorySelector: Failed to create plugin. Attribute [" + pluginFactory.ToString() + "]", exception);
					}
				}
			}
		}
		private void LoadAliases(Assembly assembly, ILoggerRepository repository)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			if (repository == null)
			{
				throw new ArgumentNullException("repository");
			}
			object[] customAttributes = Attribute.GetCustomAttributes(assembly, typeof(AliasRepositoryAttribute), false);
			if (customAttributes != null && customAttributes.Length > 0)
			{
				object[] array = customAttributes;
				for (int i = 0; i < array.Length; i++)
				{
					AliasRepositoryAttribute aliasRepositoryAttribute = (AliasRepositoryAttribute)array[i];
					try
					{
						this.AliasRepository(aliasRepositoryAttribute.Name, repository);
					}
					catch (Exception exception)
					{
						LogLog.Error("DefaultRepositorySelector: Failed to alias repository [" + aliasRepositoryAttribute.Name + "]", exception);
					}
				}
			}
		}
	}
}
