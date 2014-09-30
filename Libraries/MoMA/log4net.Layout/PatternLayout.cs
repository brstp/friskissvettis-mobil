using log4net.Core;
using log4net.Layout.Pattern;
using log4net.Util;
using log4net.Util.PatternStringConverters;
using System;
using System.Collections;
using System.IO;
namespace log4net.Layout
{
	public class PatternLayout : LayoutSkeleton
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
		public const string DefaultConversionPattern = "%message%newline";
		public const string DetailConversionPattern = "%timestamp [%thread] %level %logger %ndc - %message%newline";
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
		static PatternLayout()
		{
			PatternLayout.s_globalRulesRegistry = new Hashtable(45);
			PatternLayout.s_globalRulesRegistry.Add("literal", typeof(LiteralPatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("newline", typeof(NewLinePatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("n", typeof(NewLinePatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("c", typeof(LoggerPatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("logger", typeof(LoggerPatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("C", typeof(TypeNamePatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("class", typeof(TypeNamePatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("type", typeof(TypeNamePatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("d", typeof(log4net.Layout.Pattern.DatePatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("date", typeof(log4net.Layout.Pattern.DatePatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("exception", typeof(ExceptionPatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("F", typeof(FileLocationPatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("file", typeof(FileLocationPatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("l", typeof(FullLocationPatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("location", typeof(FullLocationPatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("L", typeof(LineLocationPatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("line", typeof(LineLocationPatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("m", typeof(MessagePatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("message", typeof(MessagePatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("M", typeof(MethodLocationPatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("method", typeof(MethodLocationPatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("p", typeof(LevelPatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("level", typeof(LevelPatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("P", typeof(log4net.Layout.Pattern.PropertyPatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("property", typeof(log4net.Layout.Pattern.PropertyPatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("properties", typeof(log4net.Layout.Pattern.PropertyPatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("r", typeof(RelativeTimePatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("timestamp", typeof(RelativeTimePatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("t", typeof(ThreadPatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("thread", typeof(ThreadPatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("x", typeof(NdcPatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("ndc", typeof(NdcPatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("X", typeof(log4net.Layout.Pattern.PropertyPatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("mdc", typeof(log4net.Layout.Pattern.PropertyPatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("a", typeof(log4net.Layout.Pattern.AppDomainPatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("appdomain", typeof(log4net.Layout.Pattern.AppDomainPatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("u", typeof(log4net.Layout.Pattern.IdentityPatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("identity", typeof(log4net.Layout.Pattern.IdentityPatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("utcdate", typeof(log4net.Layout.Pattern.UtcDatePatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("utcDate", typeof(log4net.Layout.Pattern.UtcDatePatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("UtcDate", typeof(log4net.Layout.Pattern.UtcDatePatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("w", typeof(log4net.Layout.Pattern.UserNamePatternConverter));
			PatternLayout.s_globalRulesRegistry.Add("username", typeof(log4net.Layout.Pattern.UserNamePatternConverter));
		}
		public PatternLayout() : this("%message%newline")
		{
		}
		public PatternLayout(string pattern)
		{
			this.IgnoresException = true;
			this.m_pattern = pattern;
			if (this.m_pattern == null)
			{
				this.m_pattern = "%message%newline";
			}
			this.ActivateOptions();
		}
		protected virtual PatternParser CreatePatternParser(string pattern)
		{
			PatternParser patternParser = new PatternParser(pattern);
			foreach (DictionaryEntry dictionaryEntry in PatternLayout.s_globalRulesRegistry)
			{
				patternParser.PatternConverters[dictionaryEntry.Key] = dictionaryEntry.Value;
			}
			foreach (DictionaryEntry dictionaryEntry in this.m_instanceRulesRegistry)
			{
				patternParser.PatternConverters[dictionaryEntry.Key] = dictionaryEntry.Value;
			}
			return patternParser;
		}
		public override void ActivateOptions()
		{
			this.m_head = this.CreatePatternParser(this.m_pattern).Parse();
			for (PatternConverter patternConverter = this.m_head; patternConverter != null; patternConverter = patternConverter.Next)
			{
				PatternLayoutConverter patternLayoutConverter = patternConverter as PatternLayoutConverter;
				if (patternLayoutConverter != null)
				{
					if (!patternLayoutConverter.IgnoresException)
					{
						this.IgnoresException = false;
						break;
					}
				}
			}
		}
		public override void Format(TextWriter writer, LoggingEvent loggingEvent)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (loggingEvent == null)
			{
				throw new ArgumentNullException("loggingEvent");
			}
			for (PatternConverter patternConverter = this.m_head; patternConverter != null; patternConverter = patternConverter.Next)
			{
				patternConverter.Format(writer, loggingEvent);
			}
		}
		public void AddConverter(PatternLayout.ConverterInfo converterInfo)
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
