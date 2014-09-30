using log4net.Repository;
using log4net.Util;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Xml;
namespace log4net.Config
{
	public sealed class XmlConfigurator
	{
		private sealed class ConfigureAndWatchHandler
		{
			private const int TimeoutMillis = 500;
			private FileInfo m_configFile;
			private ILoggerRepository m_repository;
			private Timer m_timer;
			internal static void StartWatching(ILoggerRepository repository, FileInfo configFile)
			{
				new XmlConfigurator.ConfigureAndWatchHandler(repository, configFile);
			}
			private ConfigureAndWatchHandler(ILoggerRepository repository, FileInfo configFile)
			{
				this.m_repository = repository;
				this.m_configFile = configFile;
				FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();
				fileSystemWatcher.Path = this.m_configFile.DirectoryName;
				fileSystemWatcher.Filter = this.m_configFile.Name;
				fileSystemWatcher.NotifyFilter = (NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime);
				fileSystemWatcher.Changed += new FileSystemEventHandler(this.ConfigureAndWatchHandler_OnChanged);
				fileSystemWatcher.Created += new FileSystemEventHandler(this.ConfigureAndWatchHandler_OnChanged);
				fileSystemWatcher.Deleted += new FileSystemEventHandler(this.ConfigureAndWatchHandler_OnChanged);
				fileSystemWatcher.Renamed += new RenamedEventHandler(this.ConfigureAndWatchHandler_OnRenamed);
				fileSystemWatcher.EnableRaisingEvents = true;
				this.m_timer = new Timer(new TimerCallback(this.OnWatchedFileChange), null, -1, -1);
			}
			private void ConfigureAndWatchHandler_OnChanged(object source, FileSystemEventArgs e)
			{
				LogLog.Debug(string.Concat(new object[]
				{
					"ConfigureAndWatchHandler: ",
					e.ChangeType,
					" [",
					this.m_configFile.FullName,
					"]"
				}));
				this.m_timer.Change(500, -1);
			}
			private void ConfigureAndWatchHandler_OnRenamed(object source, RenamedEventArgs e)
			{
				LogLog.Debug(string.Concat(new object[]
				{
					"ConfigureAndWatchHandler: ",
					e.ChangeType,
					" [",
					this.m_configFile.FullName,
					"]"
				}));
				this.m_timer.Change(500, -1);
			}
			private void OnWatchedFileChange(object state)
			{
				XmlConfigurator.Configure(this.m_repository, this.m_configFile);
			}
		}
		private XmlConfigurator()
		{
		}
		public static void Configure()
		{
			XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetCallingAssembly()));
		}
		public static void Configure(ILoggerRepository repository)
		{
			LogLog.Debug("XmlConfigurator: configuring repository [" + repository.Name + "] using .config file section");
			try
			{
				LogLog.Debug("XmlConfigurator: Application config file is [" + SystemInfo.ConfigurationFileLocation + "]");
			}
			catch
			{
				LogLog.Debug("XmlConfigurator: Application config file location unknown");
			}
			try
			{
				XmlElement xmlElement = ConfigurationManager.GetSection("log4net") as XmlElement;
				if (xmlElement == null)
				{
					LogLog.Error("XmlConfigurator: Failed to find configuration section 'log4net' in the application's .config file. Check your .config file for the <log4net> and <configSections> elements. The configuration section should look like: <section name=\"log4net\" type=\"log4net.Config.Log4NetConfigurationSectionHandler,log4net\" />");
				}
				else
				{
					XmlConfigurator.ConfigureFromXml(repository, xmlElement);
				}
			}
			catch (ConfigurationException ex)
			{
				if (ex.BareMessage.IndexOf("Unrecognized element") >= 0)
				{
					LogLog.Error("XmlConfigurator: Failed to parse config file. Check your .config file is well formed XML.", ex);
				}
				else
				{
					string str = "<section name=\"log4net\" type=\"log4net.Config.Log4NetConfigurationSectionHandler," + Assembly.GetExecutingAssembly().FullName + "\" />";
					LogLog.Error("XmlConfigurator: Failed to parse config file. Is the <configSections> specified as: " + str, ex);
				}
			}
		}
		public static void Configure(XmlElement element)
		{
			XmlConfigurator.ConfigureFromXml(LogManager.GetRepository(Assembly.GetCallingAssembly()), element);
		}
		public static void Configure(ILoggerRepository repository, XmlElement element)
		{
			LogLog.Debug("XmlConfigurator: configuring repository [" + repository.Name + "] using XML element");
			XmlConfigurator.ConfigureFromXml(repository, element);
		}
		public static void Configure(FileInfo configFile)
		{
			XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetCallingAssembly()), configFile);
		}
		public static void Configure(Uri configUri)
		{
			XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetCallingAssembly()), configUri);
		}
		public static void Configure(Stream configStream)
		{
			XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetCallingAssembly()), configStream);
		}
		public static void Configure(ILoggerRepository repository, FileInfo configFile)
		{
			LogLog.Debug(string.Concat(new object[]
			{
				"XmlConfigurator: configuring repository [",
				repository.Name,
				"] using file [",
				configFile,
				"]"
			}));
			if (configFile == null)
			{
				LogLog.Error("XmlConfigurator: Configure called with null 'configFile' parameter");
			}
			else
			{
				if (File.Exists(configFile.FullName))
				{
					FileStream fileStream = null;
					int num = 5;
					while (--num >= 0)
					{
						try
						{
							fileStream = configFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
							break;
						}
						catch (IOException exception)
						{
							if (num == 0)
							{
								LogLog.Error("XmlConfigurator: Failed to open XML config file [" + configFile.Name + "]", exception);
								fileStream = null;
							}
							Thread.Sleep(250);
						}
					}
					if (fileStream != null)
					{
						try
						{
							XmlConfigurator.Configure(repository, fileStream);
						}
						finally
						{
							fileStream.Close();
						}
					}
				}
				else
				{
					LogLog.Debug("XmlConfigurator: config file [" + configFile.FullName + "] not found. Configuration unchanged.");
				}
			}
		}
		public static void Configure(ILoggerRepository repository, Uri configUri)
		{
			LogLog.Debug(string.Concat(new object[]
			{
				"XmlConfigurator: configuring repository [",
				repository.Name,
				"] using URI [",
				configUri,
				"]"
			}));
			if (configUri == null)
			{
				LogLog.Error("XmlConfigurator: Configure called with null 'configUri' parameter");
			}
			else
			{
				if (configUri.IsFile)
				{
					XmlConfigurator.Configure(repository, new FileInfo(configUri.LocalPath));
				}
				else
				{
					WebRequest webRequest = null;
					try
					{
						webRequest = WebRequest.Create(configUri);
					}
					catch (Exception exception)
					{
						LogLog.Error("XmlConfigurator: Failed to create WebRequest for URI [" + configUri + "]", exception);
					}
					if (webRequest != null)
					{
						try
						{
							webRequest.Credentials = CredentialCache.DefaultCredentials;
						}
						catch
						{
						}
						try
						{
							WebResponse response = webRequest.GetResponse();
							if (response != null)
							{
								try
								{
									using (Stream responseStream = response.GetResponseStream())
									{
										XmlConfigurator.Configure(repository, responseStream);
									}
								}
								finally
								{
									response.Close();
								}
							}
						}
						catch (Exception exception)
						{
							LogLog.Error("XmlConfigurator: Failed to request config from URI [" + configUri + "]", exception);
						}
					}
				}
			}
		}
		public static void Configure(ILoggerRepository repository, Stream configStream)
		{
			LogLog.Debug("XmlConfigurator: configuring repository [" + repository.Name + "] using stream");
			if (configStream == null)
			{
				LogLog.Error("XmlConfigurator: Configure called with null 'configStream' parameter");
			}
			else
			{
				XmlDocument xmlDocument = new XmlDocument();
				try
				{
					XmlReader reader = XmlReader.Create(configStream, new XmlReaderSettings
					{
						ProhibitDtd = false
					});
					xmlDocument.Load(reader);
				}
				catch (Exception exception)
				{
					LogLog.Error("XmlConfigurator: Error while loading XML configuration", exception);
					xmlDocument = null;
				}
				if (xmlDocument != null)
				{
					LogLog.Debug("XmlConfigurator: loading XML configuration");
					XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("log4net");
					if (elementsByTagName.Count == 0)
					{
						LogLog.Debug("XmlConfigurator: XML configuration does not contain a <log4net> element. Configuration Aborted.");
					}
					else
					{
						if (elementsByTagName.Count > 1)
						{
							LogLog.Error("XmlConfigurator: XML configuration contains [" + elementsByTagName.Count + "] <log4net> elements. Only one is allowed. Configuration Aborted.");
						}
						else
						{
							XmlConfigurator.ConfigureFromXml(repository, elementsByTagName[0] as XmlElement);
						}
					}
				}
			}
		}
		public static void ConfigureAndWatch(FileInfo configFile)
		{
			XmlConfigurator.ConfigureAndWatch(LogManager.GetRepository(Assembly.GetCallingAssembly()), configFile);
		}
		public static void ConfigureAndWatch(ILoggerRepository repository, FileInfo configFile)
		{
			LogLog.Debug(string.Concat(new object[]
			{
				"XmlConfigurator: configuring repository [",
				repository.Name,
				"] using file [",
				configFile,
				"] watching for file updates"
			}));
			if (configFile == null)
			{
				LogLog.Error("XmlConfigurator: ConfigureAndWatch called with null 'configFile' parameter");
			}
			else
			{
				XmlConfigurator.Configure(repository, configFile);
				try
				{
					XmlConfigurator.ConfigureAndWatchHandler.StartWatching(repository, configFile);
				}
				catch (Exception exception)
				{
					LogLog.Error("XmlConfigurator: Failed to initialize configuration file watcher for file [" + configFile.FullName + "]", exception);
				}
			}
		}
		private static void ConfigureFromXml(ILoggerRepository repository, XmlElement element)
		{
			if (element == null)
			{
				LogLog.Error("XmlConfigurator: ConfigureFromXml called with null 'element' parameter");
			}
			else
			{
				if (repository == null)
				{
					LogLog.Error("XmlConfigurator: ConfigureFromXml called with null 'repository' parameter");
				}
				else
				{
					LogLog.Debug("XmlConfigurator: Configuring Repository [" + repository.Name + "]");
					IXmlRepositoryConfigurator xmlRepositoryConfigurator = repository as IXmlRepositoryConfigurator;
					if (xmlRepositoryConfigurator == null)
					{
						LogLog.Warn("XmlConfigurator: Repository [" + repository + "] does not support the XmlConfigurator");
					}
					else
					{
						XmlDocument xmlDocument = new XmlDocument();
						XmlElement element2 = (XmlElement)xmlDocument.AppendChild(xmlDocument.ImportNode(element, true));
						xmlRepositoryConfigurator.Configure(element2);
					}
				}
			}
		}
	}
}
