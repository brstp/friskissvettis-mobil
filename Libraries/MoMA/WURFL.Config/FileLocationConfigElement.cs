using System;
using System.Configuration;
namespace WURFL.Config
{
	public class FileLocationConfigElement : ConfigurationElement
	{
		[ConfigurationProperty("path", IsRequired = true, IsKey = true)]
		public string Path
		{
			get
			{
				return (string)(base["path"] ?? base["path"]);
			}
		}
	}
}
