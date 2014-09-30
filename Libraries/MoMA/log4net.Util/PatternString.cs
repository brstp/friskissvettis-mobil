using log4net.Core;
using log4net.Util.PatternStringConverters;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
namespace log4net.Util
{
	public class PatternString : IOptionHandler
	{
		public sealed class ConverterInfo
		{
			private string m_name;
			private Type m_type;
			public string Name
			{
				get
				{
					return this.m_name;
				}
				set
				{
					this.m_name = value;
				}
			}
			public Type Type
			{
				get
				{
					return this.m_type;
				}
				set
				{
					this.m_type = value;
				}
			}
		}
		private static Hashtable s_globalRulesRegistry;
		private string m_pattern;
		private PatternConverter m_head;
		private Hashtable m_instanceRulesRegistry = new Hashtable();
		public string ConversionPattern
		{
			get
			{
				return this.m_pattern;
			}
			set
			{
				this.m_pattern = value;
			}
		}
		static PatternString()
		{
			PatternString.s_globalRulesRegistry = new Hashtable(15);
			PatternString.s_globalRulesRegistry.Add("appdomain", typeof(AppDomainPatternConverter));
			PatternString.s_globalRulesRegistry.Add("date", typeof(DatePatternConverter));
			PatternString.s_globalRulesRegistry.Add("env", typeof(EnvironmentPatternConverter));
			PatternString.s_globalRulesRegistry.Add("identity", typeof(IdentityPatternConverter));
			PatternString.s_globalRulesRegistry.Add("literal", typeof(LiteralPatternConverter));
			PatternString.s_globalRulesRegistry.Add("newline", typeof(NewLinePatternConverter));
			PatternString.s_globalRulesRegistry.Add("processid", typeof(ProcessIdPatternConverter));
			PatternString.s_globalRulesRegistry.Add("property", typeof(PropertyPatternConverter));
			PatternString.s_globalRulesRegistry.Add("random", typeof(RandomStringPatternConverter));
			PatternString.s_globalRulesRegistry.Add("username", typeof(UserNamePatternConverter));
			PatternString.s_globalRulesRegistry.Add("utcdate", typeof(UtcDatePatternConverter));
			PatternString.s_globalRulesRegistry.Add("utcDate", typeof(UtcDatePatternConverter));
			PatternString.s_globalRulesRegistry.Add("UtcDate", typeof(UtcDatePatternConverter));
		}
		public PatternString()
		{
		}
		public PatternString(string pattern)
		{
			this.m_pattern = pattern;
			this.ActivateOptions();
		}
		public virtual void ActivateOptions()
		{
			this.m_head = this.CreatePatternParser(this.m_pattern).Parse();
		}
		private PatternParser CreatePatternParser(string pattern)
		{
			PatternParser patternParser = new PatternParser(pattern);
			foreach (DictionaryEntry dictionaryEntry in PatternString.s_globalRulesRegistry)
			{
				patternParser.PatternConverters.Add(dictionaryEntry.Key, dictionaryEntry.Value);
			}
			foreach (DictionaryEntry dictionaryEntry in this.m_instanceRulesRegistry)
			{
				patternParser.PatternConverters[dictionaryEntry.Key] = dictionaryEntry.Value;
			}
			return patternParser;
		}
		public void Format(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			for (PatternConverter patternConverter = this.m_head; patternConverter != null; patternConverter = patternConverter.Next)
			{
				patternConverter.Format(writer, null);
			}
		}
		public string Format()
		{
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			this.Format(stringWriter);
			return stringWriter.ToString();
		}
		public void AddConverter(PatternString.ConverterInfo converterInfo)
		{
			this.AddConverter(converterInfo.Name, converterInfo.Type);
		}
		public void AddConverter(string name, Type type)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (!typeof(PatternConverter).IsAssignableFrom(type))
			{
				throw new ArgumentException("The converter type specified [" + type + "] must be a subclass of log4net.Util.PatternConverter", "type");
			}
			this.m_instanceRulesRegistry[name] = type;
		}
	}
}
