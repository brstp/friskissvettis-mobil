using MoMA.Helpers;
using System;
using System.Configuration;
namespace MoMA.Mobile.Configuration
{
	internal class BaseConfiguration : ConfigurationSection
	{
		protected T GetDefaultValue<T>(string propertyName, T defaultValue)
		{
			if (ReflectionHelper.GetAttributeFromProperty<ConfigurationProperty>(this, "FilePath", null) == null)
			{
				return defaultValue;
			}
			defaultValue = ConversionHelper.Convert<T>(defaultValue, defaultValue);
			return defaultValue;
		}
	}
}
