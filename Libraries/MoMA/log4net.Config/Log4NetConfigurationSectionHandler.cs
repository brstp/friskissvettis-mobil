using System;
using System.Configuration;
using System.Xml;
namespace log4net.Config
{
	public class Log4NetConfigurationSectionHandler : IConfigurationSectionHandler
	{
		public object Create(object parent, object configContext, XmlNode section)
		{
			return section;
		}
	}
}
