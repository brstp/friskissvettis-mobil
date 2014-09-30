using System;
using System.Diagnostics;
namespace log4net.Util
{
	public sealed class LogLog
	{
		private const string PREFIX = "log4net: ";
		private const string ERR_PREFIX = "log4net:ERROR ";
		private const string WARN_PREFIX = "log4net:WARN ";
		private static bool s_debugEnabled;
		private static bool s_quietMode;
		public static bool InternalDebugging
		{
			get
			{
				return LogLog.s_debugEnabled;
			}
			set
			{
				LogLog.s_debugEnabled = value;
			}
		}
		public static bool QuietMode
		{
			get
			{
				return LogLog.s_quietMode;
			}
			set
			{
				LogLog.s_quietMode = value;
			}
		}
		public static bool IsDebugEnabled
		{
			get
			{
				return LogLog.s_debugEnabled && !LogLog.s_quietMode;
			}
		}
		public static bool IsWarnEnabled
		{
			get
			{
				return !LogLog.s_quietMode;
			}
		}
		public static bool IsErrorEnabled
		{
			get
			{
				return !LogLog.s_quietMode;
			}
		}
		private LogLog()
		{
		}
		static LogLog()
		{
			LogLog.s_debugEnabled = false;
			LogLog.s_quietMode = false;
			try
			{
				LogLog.InternalDebugging = OptionConverter.ToBoolean(SystemInfo.GetAppSetting("log4net.Internal.Debug"), false);
				LogLog.QuietMode = OptionConverter.ToBoolean(SystemInfo.GetAppSetting("log4net.Internal.Quiet"), false);
			}
			catch (Exception exception)
			{
				LogLog.Error("LogLog: Exception while reading ConfigurationSettings. Check your .config file is well formed XML.", exception);
			}
		}
		public static void Debug(string message)
		{
			if (LogLog.IsDebugEnabled)
			{
				LogLog.EmitOutLine("log4net: " + message);
			}
		}
		public static void Debug(string message, Exception exception)
		{
			if (LogLog.IsDebugEnabled)
			{
				LogLog.EmitOutLine("log4net: " + message);
				if (exception != null)
				{
					LogLog.EmitOutLine(exception.ToString());
				}
			}
		}
		public static void Warn(string message)
		{
			if (LogLog.IsWarnEnabled)
			{
				LogLog.EmitErrorLine("log4net:WARN " + message);
			}
		}
		public static void Warn(string message, Exception exception)
		{
			if (LogLog.IsWarnEnabled)
			{
				LogLog.EmitErrorLine("log4net:WARN " + message);
				if (exception != null)
				{
					LogLog.EmitErrorLine(exception.ToString());
				}
			}
		}
		public static void Error(string message)
		{
			if (LogLog.IsErrorEnabled)
			{
				LogLog.EmitErrorLine("log4net:ERROR " + message);
			}
		}
		public static void Error(string message, Exception exception)
		{
			if (LogLog.IsErrorEnabled)
			{
				LogLog.EmitErrorLine("log4net:ERROR " + message);
				if (exception != null)
				{
					LogLog.EmitErrorLine(exception.ToString());
				}
			}
		}
		private static void EmitOutLine(string message)
		{
			try
			{
				Console.Out.WriteLine(message);
				Trace.WriteLine(message);
			}
			catch
			{
			}
		}
		private static void EmitErrorLine(string message)
		{
			try
			{
				Console.Error.WriteLine(message);
				Trace.WriteLine(message);
			}
			catch
			{
			}
		}
	}
}
