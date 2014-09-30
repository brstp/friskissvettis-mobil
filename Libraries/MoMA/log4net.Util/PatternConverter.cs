using log4net.Repository;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
namespace log4net.Util
{
	public abstract class PatternConverter
	{
		private const int c_renderBufferSize = 256;
		private const int c_renderBufferMaxCapacity = 1024;
		private static readonly string[] SPACES = new string[]
		{
			" ",
			"  ",
			"    ",
			"        ",
			"                ",
			"                                "
		};
		private PatternConverter m_next;
		private int m_min = -1;
		private int m_max = 2147483647;
		private bool m_leftAlign = false;
		private string m_option = null;
		private ReusableStringWriter m_formatWriter = new ReusableStringWriter(CultureInfo.InvariantCulture);
		public virtual PatternConverter Next
		{
			get
			{
				return this.m_next;
			}
		}
		public virtual FormattingInfo FormattingInfo
		{
			get
			{
				return new FormattingInfo(this.m_min, this.m_max, this.m_leftAlign);
			}
			set
			{
				this.m_min = value.Min;
				this.m_max = value.Max;
				this.m_leftAlign = value.LeftAlign;
			}
		}
		public virtual string Option
		{
			get
			{
				return this.m_option;
			}
			set
			{
				this.m_option = value;
			}
		}
		protected abstract void Convert(TextWriter writer, object state);
		public virtual PatternConverter SetNext(PatternConverter patternConverter)
		{
			this.m_next = patternConverter;
			return this.m_next;
		}
		public virtual void Format(TextWriter writer, object state)
		{
			if (this.m_min < 0 && this.m_max == 2147483647)
			{
				this.Convert(writer, state);
			}
			else
			{
				this.m_formatWriter.Reset(1024, 256);
				this.Convert(this.m_formatWriter, state);
				StringBuilder stringBuilder = this.m_formatWriter.GetStringBuilder();
				int length = stringBuilder.Length;
				if (length > this.m_max)
				{
					writer.Write(stringBuilder.ToString(length - this.m_max, this.m_max));
				}
				else
				{
					if (length < this.m_min)
					{
						if (this.m_leftAlign)
						{
							writer.Write(stringBuilder.ToString());
							PatternConverter.SpacePad(writer, this.m_min - length);
						}
						else
						{
							PatternConverter.SpacePad(writer, this.m_min - length);
							writer.Write(stringBuilder.ToString());
						}
					}
					else
					{
						writer.Write(stringBuilder.ToString());
					}
				}
			}
		}
		protected static void SpacePad(TextWriter writer, int length)
		{
			while (length >= 32)
			{
				writer.Write(PatternConverter.SPACES[5]);
				length -= 32;
			}
			for (int i = 4; i >= 0; i--)
			{
				if ((length & 1 << i) != 0)
				{
					writer.Write(PatternConverter.SPACES[i]);
				}
			}
		}
		protected static void WriteDictionary(TextWriter writer, ILoggerRepository repository, IDictionary value)
		{
			writer.Write("{");
			bool flag = true;
			foreach (DictionaryEntry dictionaryEntry in value)
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					writer.Write(", ");
				}
				PatternConverter.WriteObject(writer, repository, dictionaryEntry.Key);
				writer.Write("=");
				PatternConverter.WriteObject(writer, repository, dictionaryEntry.Value);
			}
			writer.Write("}");
		}
		protected static void WriteObject(TextWriter writer, ILoggerRepository repository, object value)
		{
			if (repository != null)
			{
				repository.RendererMap.FindAndRender(value, writer);
			}
			else
			{
				if (value == null)
				{
					writer.Write(SystemInfo.NullText);
				}
				else
				{
					writer.Write(value.ToString());
				}
			}
		}
	}
}
