using log4net.Core;
using log4net.Util;
using System;
using System.Collections;
using System.Xml;
namespace log4net.Layout
{
	public class XmlLayoutSchemaLog4j : XmlLayoutBase
	{
		private static readonly DateTime s_date1970 = new DateTime(1970, 1, 1);
		public string Version
		{
			get
			{
				return "1.2";
			}
			set
			{
				if (value != "1.2")
				{
					throw new ArgumentException("Only version 1.2 of the log4j schema is currently supported");
				}
			}
		}
		public XmlLayoutSchemaLog4j()
		{
		}
		public XmlLayoutSchemaLog4j(bool locationInfo) : base(locationInfo)
		{
		}
		protected override void FormatXml(XmlWriter writer, LoggingEvent loggingEvent)
		{
			if (loggingEvent.LookupProperty("log4net:HostName") != null && loggingEvent.LookupProperty("log4jmachinename") == null)
			{
				loggingEvent.GetProperties()["log4jmachinename"] = loggingEvent.LookupProperty("log4net:HostName");
			}
			if (loggingEvent.LookupProperty("log4japp") == null && loggingEvent.Domain != null && loggingEvent.Domain.Length > 0)
			{
				loggingEvent.GetProperties()["log4japp"] = loggingEvent.Domain;
			}
			if (loggingEvent.Identity != null && loggingEvent.Identity.Length > 0 && loggingEvent.LookupProperty("log4net:Identity") == null)
			{
				loggingEvent.GetProperties()["log4net:Identity"] = loggingEvent.Identity;
			}
			if (loggingEvent.UserName != null && loggingEvent.UserName.Length > 0 && loggingEvent.LookupProperty("log4net:UserName") == null)
			{
				loggingEvent.GetProperties()["log4net:UserName"] = loggingEvent.UserName;
			}
			writer.WriteStartElement("log4j:event");
			writer.WriteAttributeString("logger", loggingEvent.LoggerName);
			writer.WriteAttributeString("timestamp", XmlConvert.ToString((long)(loggingEvent.TimeStamp.ToUniversalTime() - XmlLayoutSchemaLog4j.s_date1970).TotalMilliseconds));
			writer.WriteAttributeString("level", loggingEvent.Level.DisplayName);
			writer.WriteAttributeString("thread", loggingEvent.ThreadName);
			writer.WriteStartElement("log4j:message");
			Transform.WriteEscapedXmlString(writer, loggingEvent.RenderedMessage, base.InvalidCharReplacement);
			writer.WriteEndElement();
			object obj = loggingEvent.LookupProperty("NDC");
			if (obj != null)
			{
				string text = loggingEvent.Repository.RendererMap.FindAndRender(obj);
				if (text != null && text.Length > 0)
				{
					writer.WriteStartElement("log4j:NDC");
					Transform.WriteEscapedXmlString(writer, text, base.InvalidCharReplacement);
					writer.WriteEndElement();
				}
			}
			PropertiesDictionary properties = loggingEvent.GetProperties();
			if (properties.Count > 0)
			{
				writer.WriteStartElement("log4j:properties");
				foreach (DictionaryEntry dictionaryEntry in (IEnumerable)properties)
				{
					writer.WriteStartElement("log4j:data");
					writer.WriteAttributeString("name", (string)dictionaryEntry.Key);
					string text = loggingEvent.Repository.RendererMap.FindAndRender(dictionaryEntry.Value);
					writer.WriteAttributeString("value", text);
					writer.WriteEndElement();
				}
				writer.WriteEndElement();
			}
			string exceptionString = loggingEvent.GetExceptionString();
			if (exceptionString != null && exceptionString.Length > 0)
			{
				writer.WriteStartElement("log4j:throwable");
				Transform.WriteEscapedXmlString(writer, exceptionString, base.InvalidCharReplacement);
				writer.WriteEndElement();
			}
			if (base.LocationInfo)
			{
				LocationInfo locationInformation = loggingEvent.LocationInformation;
				writer.WriteStartElement("log4j:locationInfo");
				writer.WriteAttributeString("class", locationInformation.ClassName);
				writer.WriteAttributeString("method", locationInformation.MethodName);
				writer.WriteAttributeString("file", locationInformation.FileName);
				writer.WriteAttributeString("line", locationInformation.LineNumber);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}
	}
}
