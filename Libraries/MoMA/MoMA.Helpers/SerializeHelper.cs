using System;
using System.IO;
using System.Xml.Serialization;
namespace MoMA.Helpers
{
	public class SerializeHelper
	{
		public static void SerializeToFile(object item, string path)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(item.GetType());
			TextWriter textWriter = null;
			try
			{
				textWriter = new StreamWriter(path);
				xmlSerializer.Serialize(textWriter, item);
			}
			catch
			{
			}
			finally
			{
				if (textWriter != null)
				{
					textWriter.Close();
				}
			}
		}
		public static T DeserializeFromFile<T>(string path) where T : new()
		{
			T result = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
			XmlSerializer xmlSerializer = new XmlSerializer(result.GetType());
			TextReader textReader = null;
			try
			{
				xmlSerializer = new XmlSerializer(result.GetType());
				textReader = new StreamReader(path);
				result = (T)((object)xmlSerializer.Deserialize(textReader));
			}
			catch
			{
			}
			finally
			{
				if (textReader != null)
				{
					textReader.Close();
				}
			}
			return result;
		}
	}
}
