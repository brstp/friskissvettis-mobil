using log4net.Repository;
using log4net.Util;
using System;
using System.IO;
using System.Reflection;
namespace log4net.Config
{
	[AttributeUsage(AttributeTargets.Assembly)]
	[Serializable]
	public class XmlConfiguratorAttribute : ConfiguratorAttribute
	{
		private string m_configFile = null;
		private string m_configFileExtension = null;
		private bool m_configureAndWatch = false;
		public string ConfigFile
		{
			get
			{
				return this.m_configFile;
			}
			set
			{
				this.m_configFile = value;
			}
		}
		public string ConfigFileExtension
		{
			get
			{
				return this.m_configFileExtension;
			}
			set
			{
				this.m_configFileExtension = value;
			}
		}
		public bool Watch
		{
			get
			{
				return this.m_configureAndWatch;
			}
			set
			{
				this.m_configureAndWatch = value;
			}
		}
		public XmlConfiguratorAttribute() : base(0)
		{
		}
		public override void Configure(Assembly sourceAssembly, ILoggerRepository targetRepository)
		{
			string text = null;
			try
			{
				text = SystemInfo.ApplicationBaseDirectory;
			}
			catch
			{
			}
			if (text == null || new Uri(text).IsFile)
			{
				this.ConfigureFromFile(sourceAssembly, targetRepository);
			}
			else
			{
				this.ConfigureFromUri(sourceAssembly, targetRepository);
			}
		}
		private void ConfigureFromFile(Assembly sourceAssembly, ILoggerRepository targetRepository)
		{
			string text = null;
			if (this.m_configFile == null || this.m_configFile.Length == 0)
			{
				if (this.m_configFileExtension == null || this.m_configFileExtension.Length == 0)
				{
					try
					{
						text = SystemInfo.ConfigurationFileLocation;
					}
					catch (Exception exception)
					{
						LogLog.Error("XmlConfiguratorAttribute: Exception getting ConfigurationFileLocation. Must be able to resolve ConfigurationFileLocation when ConfigFile and ConfigFileExtension properties are not set.", exception);
					}
				}
				else
				{
					if (this.m_configFileExtension[0] != '.')
					{
						this.m_configFileExtension = "." + this.m_configFileExtension;
					}
					string text2 = null;
					try
					{
						text2 = SystemInfo.ApplicationBaseDirectory;
					}
					catch (Exception exception)
					{
						LogLog.Error("XmlConfiguratorAttribute: Exception getting ApplicationBaseDirectory. Must be able to resolve ApplicationBaseDirectory and AssemblyFileName when ConfigFileExtension property is set.", exception);
					}
					if (text2 != null)
					{
						text = Path.Combine(text2, SystemInfo.AssemblyFileName(sourceAssembly) + this.m_configFileExtension);
					}
				}
			}
			else
			{
				string text2 = null;
				try
				{
					text2 = SystemInfo.ApplicationBaseDirectory;
				}
				catch (Exception exception)
				{
					LogLog.Warn("XmlConfiguratorAttribute: Exception getting ApplicationBaseDirectory. ConfigFile property path [" + this.m_configFile + "] will be treated as an absolute path.", exception);
				}
				if (text2 != null)
				{
					text = Path.Combine(text2, this.m_configFile);
				}
				else
				{
					text = this.m_configFile;
				}
			}
			if (text != null)
			{
				this.ConfigureFromFile(targetRepository, new FileInfo(text));
			}
		}
		private void ConfigureFromFile(ILoggerRepository targetRepository, FileInfo configFile)
		{
			if (this.m_configureAndWatch)
			{
				XmlConfigurator.ConfigureAndWatch(targetRepository, configFile);
			}
			else
			{
				XmlConfigurator.Configure(targetRepository, configFile);
			}
		}
		private void ConfigureFromUri(Assembly sourceAssembly, ILoggerRepository targetRepository)
		{
			Uri uri = null;
			if (this.m_configFile == null || this.m_configFile.Length == 0)
			{
				if (this.m_configFileExtension == null || this.m_configFileExtension.Length == 0)
				{
					string text = null;
					try
					{
						text = SystemInfo.ConfigurationFileLocation;
					}
					catch (Exception exception)
					{
						LogLog.Error("XmlConfiguratorAttribute: Exception getting ConfigurationFileLocation. Must be able to resolve ConfigurationFileLocation when ConfigFile and ConfigFileExtension properties are not set.", exception);
					}
					if (text != null)
					{
						Uri uri2 = new Uri(text);
						uri = uri2;
					}
				}
				else
				{
					if (this.m_configFileExtension[0] != '.')
					{
						this.m_configFileExtension = "." + this.m_configFileExtension;
					}
					string text = null;
					try
					{
						text = SystemInfo.ConfigurationFileLocation;
					}
					catch (Exception exception)
					{
						LogLog.Error("XmlConfiguratorAttribute: Exception getting ConfigurationFileLocation. Must be able to resolve ConfigurationFileLocation when the ConfigFile property are not set.", exception);
					}
					if (text != null)
					{
						UriBuilder uriBuilder = new UriBuilder(new Uri(text));
						string text2 = uriBuilder.Path;
						int num = text2.LastIndexOf(".");
						if (num >= 0)
						{
							text2 = text2.Substring(0, num);
						}
						text2 += this.m_configFileExtension;
						uriBuilder.Path = text2;
						uri = uriBuilder.Uri;
					}
				}
			}
			else
			{
				string text3 = null;
				try
				{
					text3 = SystemInfo.ApplicationBaseDirectory;
				}
				catch (Exception exception)
				{
					LogLog.Warn("XmlConfiguratorAttribute: Exception getting ApplicationBaseDirectory. ConfigFile property path [" + this.m_configFile + "] will be treated as an absolute URI.", exception);
				}
				if (text3 != null)
				{
					uri = new Uri(new Uri(text3), this.m_configFile);
				}
				else
				{
					uri = new Uri(this.m_configFile);
				}
			}
			if (uri != null)
			{
				if (uri.IsFile)
				{
					this.ConfigureFromFile(targetRepository, new FileInfo(uri.LocalPath));
				}
				else
				{
					if (this.m_configureAndWatch)
					{
						LogLog.Warn("XmlConfiguratorAttribute: Unable to watch config file loaded from a URI");
					}
					XmlConfigurator.Configure(targetRepository, uri);
				}
			}
		}
	}
}
