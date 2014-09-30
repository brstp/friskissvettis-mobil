using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

public static class XmlHelper
{
    public static T GetValue<T>(XmlDocument xmlDoc, string xpath, T defaultValue)
    {
        try
        {
            defaultValue = Convert<T>(xmlDoc.SelectSingleNode(xpath).InnerText, defaultValue);
        }
        catch { }

        return defaultValue;
    }

    public static T GetValue<T>(XmlNode xmlNode, string xpath, T defaultValue)
    {
        try
        {
            defaultValue = Convert<T>(xmlNode.SelectSingleNode(xpath).InnerText, defaultValue);
        }
        catch { }

        return defaultValue;
    }


    /// <summary>
    /// Helper to convert a object to a type. Defaults to 
    /// a default value if the conversion was unsuccessfull.
    /// </summary>
    /// <typeparam name="T">Type to convert to</typeparam>
    /// <param name="value">Original value</param>
    /// <param name="defaultValue">Default value</param>
    /// <returns></returns>
    private static T Convert<T>(object value, T defaultValue)
    {

        T convertedValue = defaultValue;
        try
        {
            convertedValue = (T)System.Convert.ChangeType(value, typeof(T));
        }
        catch { }
        return convertedValue;
    }

    public static T GetAttr<T>(XmlNode xmlNode, string xpath, string name, T defaultValue)
    {
        try
        {
            XmlNode node = xmlNode.SelectSingleNode(xpath);
            return GetAttr<T>(xmlNode, name, defaultValue);
        }
        catch { }

        return defaultValue;
    }

    public static T GetAttr<T>(XmlNode xmlNode, string name, T defaultValue)
    {
        try
        {

            if (xmlNode != null)
            {
                XmlAttribute attr = xmlNode.Attributes[name];

                if (attr != null)
                {
                    defaultValue = Convert<T>(attr.Value, defaultValue);
                }
            }
        }
        catch { }

        return defaultValue;
    }

    public static bool Save(string path)
    {
        return false;
    }

    public static XmlDocument Load(string path)
    {
        return new XmlDocument();
    }
}