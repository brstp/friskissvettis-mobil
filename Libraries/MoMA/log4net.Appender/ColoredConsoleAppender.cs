using log4net.Core;
using log4net.Layout;
using log4net.Util;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
namespace log4net.Appender
{
	public class ColoredConsoleAppender : AppenderSkeleton
	{
		[Flags]
		public enum Colors
		{
			Blue = 1,
			Green = 2,
			Red = 4,
			White = 7,
			Yellow = 6,
			Purple = 5,
			Cyan = 3,
			HighIntensity = 8
		}
		private struct COORD
		{
			public ushort x;
			public ushort y;
		}
		private struct SMALL_RECT
		{
			public ushort Left;
			public ushort Top;
			public ushort Right;
			public ushort Bottom;
		}
		private struct CONSOLE_SCREEN_BUFFER_INFO
		{
			public ColoredConsoleAppender.COORD dwSize;
			public ColoredConsoleAppender.COORD dwCursorPosition;
			public ushort wAttributes;
			public ColoredConsoleAppender.SMALL_RECT srWindow;
			public ColoredConsoleAppender.COORD dwMaximumWindowSize;
		}
		public class LevelColors : LevelMappingEntry
		{
			private ColoredConsoleAppender.Colors m_foreColor;
			private ColoredConsoleAppender.Colors m_backColor;
			private ushort m_combinedColor = 0;
			public ColoredConsoleAppender.Colors ForeColor
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
			public ColoredConsoleAppender.Colors BackColor
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
			internal ushort CombinedColor
			{
				get
				{
					return this.m_combinedColor;
				}
			}
			public override void ActivateOptions()
			{
				base.ActivateOptions();
				this.m_combinedColor = (ushort)(this.m_foreColor + (int)((int)this.m_backColor << 4));
			}
		}
		public const string ConsoleOut = "Console.Out";
		public const string ConsoleError = "Console.Error";
		private const uint STD_OUTPUT_HANDLE = 4294967285u;
		private const uint STD_ERROR_HANDLE = 4294967284u;
		private static readonly char[] s_windowsNewline = new char[]
		{
			'\r',
			'\n'
		};
		private bool m_writeToErrorStream = false;
		private LevelMapping m_levelMapping = new LevelMapping();
		private StreamWriter m_consoleOutputWriter = null;
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
		public ColoredConsoleAppender()
		{
		}
		[Obsolete("Instead use the default constructor and set the Layout property")]
		public ColoredConsoleAppender(ILayout layout) : this(layout, false)
		{
		}
		[Obsolete("Instead use the default constructor and set the Layout & Target properties")]
		public ColoredConsoleAppender(ILayout layout, bool writeToErrorStream)
		{
			this.Layout = layout;
			this.m_writeToErrorStream = writeToErrorStream;
		}
		public void AddMapping(ColoredConsoleAppender.LevelColors mapping)
		{
			this.m_levelMapping.Add(mapping);
		}
		protected override void Append(LoggingEvent loggingEvent)
		{
			if (this.m_consoleOutputWriter != null)
			{
				IntPtr consoleHandle = IntPtr.Zero;
				if (this.m_writeToErrorStream)
				{
					consoleHandle = ColoredConsoleAppender.GetStdHandle(4294967284u);
				}
				else
				{
					consoleHandle = ColoredConsoleAppender.GetStdHandle(4294967285u);
				}
				ushort attributes = 7;
				ColoredConsoleAppender.LevelColors levelColors = this.m_levelMapping.Lookup(loggingEvent.Level) as ColoredConsoleAppender.LevelColors;
				if (levelColors != null)
				{
					attributes = levelColors.CombinedColor;
				}
				string text = base.RenderLoggingEvent(loggingEvent);
				ColoredConsoleAppender.CONSOLE_SCREEN_BUFFER_INFO cONSOLE_SCREEN_BUFFER_INFO;
				ColoredConsoleAppender.GetConsoleScreenBufferInfo(consoleHandle, out cONSOLE_SCREEN_BUFFER_INFO);
				ColoredConsoleAppender.SetConsoleTextAttribute(consoleHandle, attributes);
				char[] array = text.ToCharArray();
				int num = array.Length;
				bool flag = false;
				if (num > 1 && array[num - 2] == '\r' && array[num - 1] == '\n')
				{
					num -= 2;
					flag = true;
				}
				this.m_consoleOutputWriter.Write(array, 0, num);
				ColoredConsoleAppender.SetConsoleTextAttribute(consoleHandle, cONSOLE_SCREEN_BUFFER_INFO.wAttributes);
				if (flag)
				{
					this.m_consoleOutputWriter.Write(ColoredConsoleAppender.s_windowsNewline, 0, 2);
				}
			}
		}
		public override void ActivateOptions()
		{
			base.ActivateOptions();
			this.m_levelMapping.ActivateOptions();
			Stream stream;
			if (this.m_writeToErrorStream)
			{
				stream = Console.OpenStandardError();
			}
			else
			{
				stream = Console.OpenStandardOutput();
			}
			Encoding encoding = Encoding.GetEncoding(ColoredConsoleAppender.GetConsoleOutputCP());
			this.m_consoleOutputWriter = new StreamWriter(stream, encoding, 256);
			this.m_consoleOutputWriter.AutoFlush = true;
			GC.SuppressFinalize(this.m_consoleOutputWriter);
		}
		[DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int GetConsoleOutputCP();
		[DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool SetConsoleTextAttribute(IntPtr consoleHandle, ushort attributes);
		[DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool GetConsoleScreenBufferInfo(IntPtr consoleHandle, out ColoredConsoleAppender.CONSOLE_SCREEN_BUFFER_INFO bufferInfo);
		[DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr GetStdHandle(uint type);
	}
}
