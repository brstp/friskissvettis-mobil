using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security;
using System.Threading;
namespace log4net.Util
{
	public sealed class SystemInfo
	{
		private const string DEFAULT_NULL_TEXT = "(null)";
		private const string DEFAULT_NOT_AVAILABLE_TEXT = "NOT AVAILABLE";
		public static readonly Type[] EmptyTypes;
		private static string s_hostName;
		private static string s_appFriendlyName;
		private static string s_nullText;
		private static string s_notAvailableText;
		private static DateTime s_processStartTime;
		public static string NewLine
		{
			get
			{
				return Environment.NewLine;
			}
		}
		public static string ApplicationBaseDirectory
		{
			get
			{
				return AppDomain.CurrentDomain.BaseDirectory;
			}
		}
		public static string ConfigurationFileLocation
		{
			get
			{
				return AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
			}
		}
		public static string EntryAssemblyLocation
		{
			get
			{
				return Assembly.GetEntryAssembly().Location;
			}
		}
		public static int CurrentThreadId
		{
			get
			{
				return Thread.CurrentThread.ManagedThreadId;
			}
		}
		public static string HostName
		{
			get
			{
				if (SystemInfo.s_hostName == null)
				{
					try
					{
						SystemInfo.s_hostName = Dns.GetHostName();
					}
					catch (SocketException)
					{
					}
					catch (SecurityException)
					{
					}
					if (SystemInfo.s_hostName == null || SystemInfo.s_hostName.Length == 0)
					{
						try
						{
							SystemInfo.s_hostName = Environment.MachineName;
						}
						catch (InvalidOperationException)
						{
						}
						catch (SecurityException)
						{
						}
					}
					if (SystemInfo.s_hostName == null || SystemInfo.s_hostName.Length == 0)
					{
						SystemInfo.s_hostName = SystemInfo.s_notAvailableText;
					}
				}
				return SystemInfo.s_hostName;
			}
		}
		public static string ApplicationFriendlyName
		{
			get
			{
				if (SystemInfo.s_appFriendlyName == null)
				{
					try
					{
						SystemInfo.s_appFriendlyName = AppDomain.CurrentDomain.FriendlyName;
					}
					catch (SecurityException)
					{
						LogLog.Debug("SystemInfo: Security exception while trying to get current domain friendly name. Error Ignored.");
					}
					if (SystemInfo.s_appFriendlyName == null || SystemInfo.s_appFriendlyName.Length == 0)
					{
						try
						{
							string entryAssemblyLocation = SystemInfo.EntryAssemblyLocation;
							SystemInfo.s_appFriendlyName = Path.GetFileName(entryAssemblyLocation);
						}
						catch (SecurityException)
						{
						}
					}
					if (SystemInfo.s_appFriendlyName == null || SystemInfo.s_appFriendlyName.Length == 0)
					{
						SystemInfo.s_appFriendlyName = SystemInfo.s_notAvailableText;
					}
				}
				return SystemInfo.s_appFriendlyName;
			}
		}
		public static DateTime ProcessStartTime
		{
			get
			{
				return SystemInfo.s_processStartTime;
			}
		}
		public static string NullText
		{
			get
			{
				return SystemInfo.s_nullText;
			}
			set
			{
				SystemInfo.s_nullText = value;
			}
		}
		public static string NotAvailableText
		{
			get
			{
				return SystemInfo.s_notAvailableText;
			}
			set
			{
				SystemInfo.s_notAvailableText = value;
			}
		}
		private SystemInfo()
		{
		}
		static SystemInfo()
		{
			SystemInfo.EmptyTypes = new Type[0];
			SystemInfo.s_processStartTime = DateTime.Now;
			string text = "(null)";
			string text2 = "NOT AVAILABLE";
			string appSetting = SystemInfo.GetAppSetting("log4net.NullText");
			if (appSetting != null && appSetting.Length > 0)
			{
				LogLog.Debug("SystemInfo: Initializing NullText value to [" + appSetting + "].");
				text = appSetting;
			}
			string appSetting2 = SystemInfo.GetAppSetting("log4net.NotAvailableText");
			if (appSetting2 != null && appSetting2.Length > 0)
			{
				LogLog.Debug("SystemInfo: Initializing NotAvailableText value to [" + appSetting2 + "].");
				text2 = appSetting2;
			}
			SystemInfo.s_notAvailableText = text2;
			SystemInfo.s_nullText = text;
		}
		public static string AssemblyLocationInfo(Assembly myAssembly)
		{
			string result;
			if (myAssembly.GlobalAssemblyCache)
			{
				result = "Global Assembly Cache";
			}
			else
			{
				try
				{
					result = myAssembly.Location;
				}
				catch (SecurityException)
				{
					result = "Location Permission Denied";
				}
			}
			return result;
		}
		public static string AssemblyQualifiedName(Type type)
		{
			return type.FullName + ", " + type.Assembly.FullName;
		}
		public static string AssemblyShortName(Assembly myAssembly)
		{
			string text = myAssembly.FullName;
			int num = text.IndexOf(',');
			if (num > 0)
			{
				text = text.Substring(0, num);
			}
			return text.Trim();
		}
		public static string AssemblyFileName(Assembly myAssembly)
		{
			return Path.GetFileName(myAssembly.Location);
		}
		public static Type GetTypeFromString(Type relativeType, string typeName, bool throwOnError, bool ignoreCase)
		{
			return SystemInfo.GetTypeFromString(relativeType.Assembly, typeName, throwOnError, ignoreCase);
		}
		public static Type GetTypeFromString(string typeName, bool throwOnError, bool ignoreCase)
		{
			return SystemInfo.GetTypeFromString(Assembly.GetCallingAssembly(), typeName, throwOnError, ignoreCase);
		}
		public static Type GetTypeFromString(Assembly relativeAssembly, string typeName, bool throwOnError, bool ignoreCase)
		{
			Type result;
			if (typeName.IndexOf(',') == -1)
			{
				Type type = relativeAssembly.GetType(typeName, false, ignoreCase);
				if (type != null)
				{
					result = type;
				}
				else
				{
					Assembly[] array = null;
					try
					{
						array = AppDomain.CurrentDomain.GetAssemblies();
					}
					catch (SecurityException)
					{
					}
					if (array != null)
					{
						Assembly[] array2 = array;
						for (int i = 0; i < array2.Length; i++)
						{
							Assembly assembly = array2[i];
							type = assembly.GetType(typeName, false, ignoreCase);
							if (type != null)
							{
								LogLog.Debug(string.Concat(new string[]
								{
									"SystemInfo: Loaded type [",
									typeName,
									"] from assembly [",
									assembly.FullName,
									"] by searching loaded assemblies."
								}));
								result = type;
								return result;
							}
						}
					}
					if (throwOnError)
					{
						throw new TypeLoadException(string.Concat(new string[]
						{
							"Could not load type [",
							typeName,
							"]. Tried assembly [",
							relativeAssembly.FullName,
							"] and all loaded assemblies"
						}));
					}
					result = null;
				}
			}
			else
			{
				result = Type.GetType(typeName, throwOnError, ignoreCase);
			}
			return result;
		}
		public static Guid NewGuid()
		{
			return Guid.NewGuid();
		}
		public static ArgumentOutOfRangeException CreateArgumentOutOfRangeException(string parameterName, object actualValue, string message)
		{
			return new ArgumentOutOfRangeException(parameterName, actualValue, message);
		}
		public static bool TryParse(string s, out int val)
		{
			val = 0;
			bool result;
			try
			{
				double value;
				if (double.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
				{
					val = Convert.ToInt32(value);
					result = true;
					return result;
				}
			}
			catch
			{
			}
			result = false;
			return result;
		}
		public static bool TryParse(string s, out long val)
		{
			val = 0L;
			bool result;
			try
			{
				double value;
				if (double.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
				{
					val = Convert.ToInt64(value);
					result = true;
					return result;
				}
			}
			catch
			{
			}
			result = false;
			return result;
		}
		public static string GetAppSetting(string key)
		{
			string result;
			try
			{
				result = ConfigurationManager.AppSettings[key];
				return result;
			}
			catch (Exception exception)
			{
				LogLog.Error("DefaultRepositorySelector: Exception while reading ConfigurationSettings. Check your .config file is well formed XML.", exception);
			}
			result = null;
			return result;
		}
		public static string ConvertToFullPath(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			string text = "";
			try
			{
				string applicationBaseDirectory = SystemInfo.ApplicationBaseDirectory;
				if (applicationBaseDirectory != null)
				{
					Uri uri = new Uri(applicationBaseDirectory);
					if (uri.IsFile)
					{
						text = uri.LocalPath;
					}
				}
			}
			catch
			{
			}
			string fullPath;
			if (text != null && text.Length > 0)
			{
				fullPath = Path.GetFullPath(Path.Combine(text, path));
			}
			else
			{
				fullPath = Path.GetFullPath(path);
			}
			return fullPath;
		}
		public static Hashtable CreateCaseInsensitiveHashtable()
		{
			return CollectionsUtil.CreateCaseInsensitiveHashtable();
		}
	}
}
