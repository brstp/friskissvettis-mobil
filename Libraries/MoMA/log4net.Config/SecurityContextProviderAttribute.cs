using log4net.Core;
using log4net.Repository;
using log4net.Util;
using System;
using System.Reflection;
namespace log4net.Config
{
	[AttributeUsage(AttributeTargets.Assembly)]
	[Serializable]
	public sealed class SecurityContextProviderAttribute : ConfiguratorAttribute
	{
		private Type m_providerType = null;
		public Type ProviderType
		{
			get
			{
				return this.m_providerType;
			}
			set
			{
				this.m_providerType = value;
			}
		}
		public SecurityContextProviderAttribute(Type providerType) : base(100)
		{
			this.m_providerType = providerType;
		}
		public override void Configure(Assembly sourceAssembly, ILoggerRepository targetRepository)
		{
			if (this.m_providerType == null)
			{
				LogLog.Error("SecurityContextProviderAttribute: Attribute specified on assembly [" + sourceAssembly.FullName + "] with null ProviderType.");
			}
			else
			{
				LogLog.Debug("SecurityContextProviderAttribute: Creating provider of type [" + this.m_providerType.FullName + "]");
				SecurityContextProvider securityContextProvider = Activator.CreateInstance(this.m_providerType) as SecurityContextProvider;
				if (securityContextProvider == null)
				{
					LogLog.Error("SecurityContextProviderAttribute: Failed to create SecurityContextProvider instance of type [" + this.m_providerType.Name + "].");
				}
				else
				{
					SecurityContextProvider.DefaultProvider = securityContextProvider;
				}
			}
		}
	}
}
