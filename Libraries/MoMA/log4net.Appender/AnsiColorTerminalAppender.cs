using log4net.Core;
using log4net.Util;
using System;
using System.Globalization;
using System.Text;
namespace log4net.Appender
{
	public class AnsiColorTerminalAppender : AppenderSkeleton
	{
		[Flags]
		public enum AnsiAttributes
		{
			Bright = 1,
			Dim = 2,
			Underscore = 4,
			Blink = 8,
			Reverse = 16,
			Hidden = 32,
			Strikethrough = 64
		}
		public enum AnsiColor
		{
			Black,
			Red,
			Green,
			Yellow,
			Blue,
			Magenta,
			Cyan,
			White
		}
		public class LevelColors : LevelMappingEntry
		{
			private AnsiColorTerminalAppender.AnsiColor m_foreColor;
			private AnsiColorTerminalAppender.AnsiColor m_backColor;
			private AnsiColorTerminalAppender.AnsiAttributes m_attributes;
			private string m_combinedColor = "";
			public AnsiColorTerminalAppender.AnsiColor ForeColor
			{
				get
				{
					return this.m_foreColor;
				}
				set
				{
					this.m_foreColor = value;
				}
			}
			public AnsiColorTerminalAppender.AnsiColor BackColor
			{
				get
				{
					return this.m_backColor;
				}
				set
				{
					this.m_backColor = value;
				}
			}
			public AnsiColorTerminalAppender.AnsiAttributes Attributes
			{
				get
				{
					return this.m_attributes;
				}
				set
				{
					this.m_attributes = value;
				}
			}
			internal string CombinedColor
			{
				get
				{
					return this.m_combinedColor;
				}
			}
			public override void ActivateOptions()
			{
				base.ActivateOptions();
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("\u001b[0;");
				stringBuilder.Append((int)(30 + this.m_foreColor));
				stringBuilder.Append(';');
				stringBuilder.Append((int)(40 + this.m_backColor));
				if ((this.m_attributes & AnsiColorTerminalAppender.AnsiAttributes.Bright) > (AnsiColorTerminalAppender.AnsiAttributes)0)
				{
					stringBuilder.Append(";1");
				}
				if ((this.m_attributes & AnsiColorTerminalAppender.AnsiAttributes.Dim) > (AnsiColorTerminalAppender.AnsiAttributes)0)
				{
					stringBuilder.Append(";2");
				}
				if ((this.m_attributes & AnsiColorTerminalAppender.AnsiAttributes.Underscore) > (AnsiColorTerminalAppender.AnsiAttributes)0)
				{
					stringBuilder.Append(";4");
				}
				if ((this.m_attributes & AnsiColorTerminalAppender.AnsiAttributes.Blink) > (AnsiColorTerminalAppender.AnsiAttributes)0)
				{
					stringBuilder.Append(";5");
				}
				if ((this.m_attributes & AnsiColorTerminalAppender.AnsiAttributes.Reverse) > (AnsiColorTerminalAppender.AnsiAttributes)0)
				{
					stringBuilder.Append(";7");
				}
				if ((this.m_attributes & AnsiColorTerminalAppender.AnsiAttributes.Hidden) > (AnsiColorTerminalAppender.AnsiAttributes)0)
				{
					stringBuilder.Append(";8");
				}
				if ((this.m_attributes & AnsiColorTerminalAppender.AnsiAttributes.Strikethrough) > (AnsiColorTerminalAppender.AnsiAttributes)0)
				{
					stringBuilder.Append(";9");
				}
				stringBuilder.Append('m');
				this.m_combinedColor = stringBuilder.ToString();
			}
		}
		public const string ConsoleOut = "Console.Out";
		public const string ConsoleError = "Console.Error";
		private const string PostEventCodes = "\u001b[0m";
		private bool m_writeToErrorStream = false;
		private LevelMapping m_levelMapping = new LevelMapping();
		public virtual string Target
		{
			get
			{
				return this.m_writeToErrorStream ? "Console.Error" : "Console.Out";
			}
			set
			{
				string strB = value.Trim();
				if (string.Compare("Console.Error", strB, true, CultureInfo.InvariantCulture) == 0)
				{
					this.m_writeToErrorStream = true;
				}
				else
				{
					this.m_writeToErrorStream = false;
				}
			}
		}
		protected override bool RequiresLayout
		{
			get
			{
				return true;
			}
		}
		public void AddMapping(AnsiColorTerminalAppender.LevelColors mapping)
		{
			this.m_levelMapping.Add(mapping);
		}
		protected override void Append(LoggingEvent loggingEvent)
		{
			string text = base.RenderLoggingEvent(loggingEvent);
			AnsiColorTerminalAppender.LevelColors levelColors = this.m_levelMapping.Lookup(loggingEvent.Level) as AnsiColorTerminalAppender.LevelColors;
			if (levelColors != null)
			{
				text = levelColors.CombinedColor + text;
			}
			if (text.Length > 1)
			{
				if (text.EndsWith("\r\n") || text.EndsWith("\n\r"))
				{
					text = text.Insert(text.Length - 2, "\u001b[0m");
				}
				else
				{
					if (text.EndsWith("\n") || text.EndsWith("\r"))
					{
						text = text.Insert(text.Length - 1, "\u001b[0m");
					}
					else
					{
						text += "\u001b[0m";
					}
				}
			}
			else
			{
				if (text[0] == '\n' || text[0] == '\r')
				{
					text = "\u001b[0m" + text;
				}
				else
				{
					text += "\u001b[0m";
				}
			}
			if (this.m_writeToErrorStream)
			{
				Console.Error.Write(text);
			}
			else
			{
				Console.Write(text);
			}
		}
		public override void ActivateOptions()
		{
			base.ActivateOptions();
			this.m_levelMapping.ActivateOptions();
		}
	}
}
