using log4net.Repository;
using log4net.Util;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Threading;
namespace log4net.Core
{
	[Serializable]
	public class LoggingEvent : ISerializable
	{
		public const string HostNameProperty = "log4net:HostName";
		public const string IdentityProperty = "log4net:Identity";
		public const string UserNameProperty = "log4net:UserName";
		private LoggingEventData m_data;
		private CompositeProperties m_compositeProperties;
		private PropertiesDictionary m_eventProperties;
		private readonly Type m_callerStackBoundaryDeclaringType;
		private readonly object m_message;
		private readonly Exception m_thrownException;
		private ILoggerRepository m_repository = null;
		private FixFlags m_fixFlags = FixFlags.None;
		private bool m_cacheUpdatable = true;
		public static DateTime StartTime
		{
			get
			{
				return SystemInfo.ProcessStartTime;
			}
		}
		public Level Level
		{
			get
			{
				return this.m_data.Level;
			}
		}
		public DateTime TimeStamp
		{
			get
			{
				return this.m_data.TimeStamp;
			}
		}
		public string LoggerName
		{
			get
			{
				return this.m_data.LoggerName;
			}
		}
		public LocationInfo LocationInformation
		{
			get
			{
				if (this.m_data.LocationInfo == null && this.m_cacheUpdatable)
				{
					this.m_data.LocationInfo = new LocationInfo(this.m_callerStackBoundaryDeclaringType);
				}
				return this.m_data.LocationInfo;
			}
		}
		public object MessageObject
		{
			get
			{
				return this.m_message;
			}
		}
		public Exception ExceptionObject
		{
			get
			{
				return this.m_thrownException;
			}
		}
		public ILoggerRepository Repository
		{
			get
			{
				return this.m_repository;
			}
		}
		public string RenderedMessage
		{
			get
			{
				if (this.m_data.Message == null && this.m_cacheUpdatable)
				{
					if (this.m_message == null)
					{
						this.m_data.Message = "";
					}
					else
					{
						if (this.m_message is string)
						{
							this.m_data.Message = (this.m_message as string);
						}
						else
						{
							if (this.m_repository != null)
							{
								this.m_data.Message = this.m_repository.RendererMap.FindAndRender(this.m_message);
							}
							else
							{
								this.m_data.Message = this.m_message.ToString();
							}
						}
					}
				}
				return this.m_data.Message;
			}
		}
		public string ThreadName
		{
			get
			{
				if (this.m_data.ThreadName == null && this.m_cacheUpdatable)
				{
					this.m_data.ThreadName = Thread.CurrentThread.Name;
					if (this.m_data.ThreadName == null || this.m_data.ThreadName.Length == 0)
					{
						try
						{
							this.m_data.ThreadName = SystemInfo.CurrentThreadId.ToString(NumberFormatInfo.InvariantInfo);
						}
						catch (SecurityException)
						{
							LogLog.Debug("LoggingEvent: Security exception while trying to get current thread ID. Error Ignored. Empty thread name.");
							this.m_data.ThreadName = Thread.CurrentThread.GetHashCode().ToString(CultureInfo.InvariantCulture);
						}
					}
				}
				return this.m_data.ThreadName;
			}
		}
		public string UserName
		{
			get
			{
				if (this.m_data.UserName == null && this.m_cacheUpdatable)
				{
					try
					{
						WindowsIdentity current = WindowsIdentity.GetCurrent();
						if (current != null && current.Name != null)
						{
							this.m_data.UserName = current.Name;
						}
						else
						{
							this.m_data.UserName = "";
						}
					}
					catch (SecurityException)
					{
						LogLog.Debug("LoggingEvent: Security exception while trying to get current windows identity. Error Ignored. Empty user name.");
						this.m_data.UserName = "";
					}
				}
				return this.m_data.UserName;
			}
		}
		public string Identity
		{
			get
			{
				if (this.m_data.Identity == null && this.m_cacheUpdatable)
				{
					try
					{
						if (Thread.CurrentPrincipal != null && Thread.CurrentPrincipal.Identity != null && Thread.CurrentPrincipal.Identity.Name != null)
						{
							this.m_data.Identity = Thread.CurrentPrincipal.Identity.Name;
						}
						else
						{
							this.m_data.Identity = "";
						}
					}
					catch (SecurityException)
					{
						LogLog.Debug("LoggingEvent: Security exception while trying to get current thread principal. Error Ignored. Empty identity name.");
						this.m_data.Identity = "";
					}
				}
				return this.m_data.Identity;
			}
		}
		public string Domain
		{
			get
			{
				if (this.m_data.Domain == null && this.m_cacheUpdatable)
				{
					this.m_data.Domain = SystemInfo.ApplicationFriendlyName;
				}
				return this.m_data.Domain;
			}
		}
		public PropertiesDictionary Properties
		{
			get
			{
				PropertiesDictionary result;
				if (this.m_data.Properties != null)
				{
					result = this.m_data.Properties;
				}
				else
				{
					if (this.m_eventProperties == null)
					{
						this.m_eventProperties = new PropertiesDictionary();
					}
					result = this.m_eventProperties;
				}
				return result;
			}
		}
		public FixFlags Fix
		{
			get
			{
				return this.m_fixFlags;
			}
			set
			{
				this.FixVolatileData(value);
			}
		}
		public LoggingEvent(Type callerStackBoundaryDeclaringType, ILoggerRepository repository, string loggerName, Level level, object message, Exception exception)
		{
			this.m_callerStackBoundaryDeclaringType = callerStackBoundaryDeclaringType;
			this.m_message = message;
			this.m_repository = repository;
			this.m_thrownException = exception;
			this.m_data.LoggerName = loggerName;
			this.m_data.Level = level;
			this.m_data.TimeStamp = DateTime.Now;
		}
		public LoggingEvent(Type callerStackBoundaryDeclaringType, ILoggerRepository repository, LoggingEventData data, FixFlags fixedData)
		{
			this.m_callerStackBoundaryDeclaringType = callerStackBoundaryDeclaringType;
			this.m_repository = repository;
			this.m_data = data;
			this.m_fixFlags = fixedData;
		}
		public LoggingEvent(Type callerStackBoundaryDeclaringType, ILoggerRepository repository, LoggingEventData data) : this(callerStackBoundaryDeclaringType, repository, data, FixFlags.All)
		{
		}
		public LoggingEvent(LoggingEventData data) : this(null, null, data)
		{
		}
		protected LoggingEvent(SerializationInfo info, StreamingContext context)
		{
			this.m_data.LoggerName = info.GetString("LoggerName");
			this.m_data.Level = (Level)info.GetValue("Level", typeof(Level));
			this.m_data.Message = info.GetString("Message");
			this.m_data.ThreadName = info.GetString("ThreadName");
			this.m_data.TimeStamp = info.GetDateTime("TimeStamp");
			this.m_data.LocationInfo = (LocationInfo)info.GetValue("LocationInfo", typeof(LocationInfo));
			this.m_data.UserName = info.GetString("UserName");
			this.m_data.ExceptionString = info.GetString("ExceptionString");
			this.m_data.Properties = (PropertiesDictionary)info.GetValue("Properties", typeof(PropertiesDictionary));
			this.m_data.Domain = info.GetString("Domain");
			this.m_data.Identity = info.GetString("Identity");
			this.m_fixFlags = FixFlags.All;
		}
		internal void EnsureRepository(ILoggerRepository repository)
		{
			if (repository != null)
			{
				this.m_repository = repository;
			}
		}
		public void WriteRenderedMessage(TextWriter writer)
		{
			if (this.m_data.Message != null)
			{
				writer.Write(this.m_data.Message);
			}
			else
			{
				if (this.m_message != null)
				{
					if (this.m_message is string)
					{
						writer.Write(this.m_message as string);
					}
					else
					{
						if (this.m_repository != null)
						{
							this.m_repository.RendererMap.FindAndRender(this.m_message, writer);
						}
						else
						{
							writer.Write(this.m_message.ToString());
						}
					}
				}
			}
		}
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("LoggerName", this.m_data.LoggerName);
			info.AddValue("Level", this.m_data.Level);
			info.AddValue("Message", this.m_data.Message);
			info.AddValue("ThreadName", this.m_data.ThreadName);
			info.AddValue("TimeStamp", this.m_data.TimeStamp);
			info.AddValue("LocationInfo", this.m_data.LocationInfo);
			info.AddValue("UserName", this.m_data.UserName);
			info.AddValue("ExceptionString", this.m_data.ExceptionString);
			info.AddValue("Properties", this.m_data.Properties);
			info.AddValue("Domain", this.m_data.Domain);
			info.AddValue("Identity", this.m_data.Identity);
		}
		public LoggingEventData GetLoggingEventData()
		{
			return this.GetLoggingEventData(FixFlags.Partial);
		}
		public LoggingEventData GetLoggingEventData(FixFlags fixFlags)
		{
			this.Fix = fixFlags;
			return this.m_data;
		}
		[Obsolete("Use GetExceptionString instead")]
		public string GetExceptionStrRep()
		{
			return this.GetExceptionString();
		}
		public string GetExceptionString()
		{
			if (this.m_data.ExceptionString == null && this.m_cacheUpdatable)
			{
				if (this.m_thrownException != null)
				{
					if (this.m_repository != null)
					{
						this.m_data.ExceptionString = this.m_repository.RendererMap.FindAndRender(this.m_thrownException);
					}
					else
					{
						this.m_data.ExceptionString = this.m_thrownException.ToString();
					}
				}
				else
				{
					this.m_data.ExceptionString = "";
				}
			}
			return this.m_data.ExceptionString;
		}
		[Obsolete("Use Fix property")]
		public void FixVolatileData()
		{
			this.Fix = FixFlags.All;
		}
		[Obsolete("Use Fix property")]
		public void FixVolatileData(bool fastButLoose)
		{
			if (fastButLoose)
			{
				this.Fix = FixFlags.Partial;
			}
			else
			{
				this.Fix = FixFlags.All;
			}
		}
		protected void FixVolatileData(FixFlags flags)
		{
			object obj = null;
			this.m_cacheUpdatable = true;
			FixFlags fixFlags = (flags ^ this.m_fixFlags) & flags;
			if (fixFlags > FixFlags.None)
			{
				if ((fixFlags & FixFlags.Message) != FixFlags.None)
				{
					obj = this.RenderedMessage;
					this.m_fixFlags |= FixFlags.Message;
				}
				if ((fixFlags & FixFlags.ThreadName) != FixFlags.None)
				{
					obj = this.ThreadName;
					this.m_fixFlags |= FixFlags.ThreadName;
				}
				if ((fixFlags & FixFlags.LocationInfo) != FixFlags.None)
				{
					obj = this.LocationInformation;
					this.m_fixFlags |= FixFlags.LocationInfo;
				}
				if ((fixFlags & FixFlags.UserName) != FixFlags.None)
				{
					obj = this.UserName;
					this.m_fixFlags |= FixFlags.UserName;
				}
				if ((fixFlags & FixFlags.Domain) != FixFlags.None)
				{
					obj = this.Domain;
					this.m_fixFlags |= FixFlags.Domain;
				}
				if ((fixFlags & FixFlags.Identity) != FixFlags.None)
				{
					obj = this.Identity;
					this.m_fixFlags |= FixFlags.Identity;
				}
				if ((fixFlags & FixFlags.Exception) != FixFlags.None)
				{
					obj = this.GetExceptionString();
					this.m_fixFlags |= FixFlags.Exception;
				}
				if ((fixFlags & FixFlags.Properties) != FixFlags.None)
				{
					this.CacheProperties();
					this.m_fixFlags |= FixFlags.Properties;
				}
			}
			if (obj != null)
			{
			}
			this.m_cacheUpdatable = false;
		}
		private void CreateCompositeProperties()
		{
			this.m_compositeProperties = new CompositeProperties();
			if (this.m_eventProperties != null)
			{
				this.m_compositeProperties.Add(this.m_eventProperties);
			}
			PropertiesDictionary properties = LogicalThreadContext.Properties.GetProperties(false);
			if (properties != null)
			{
				this.m_compositeProperties.Add(properties);
			}
			PropertiesDictionary properties2 = ThreadContext.Properties.GetProperties(false);
			if (properties2 != null)
			{
				this.m_compositeProperties.Add(properties2);
			}
			this.m_compositeProperties.Add(GlobalContext.Properties.GetReadOnlyProperties());
		}
		private void CacheProperties()
		{
			if (this.m_data.Properties == null && this.m_cacheUpdatable)
			{
				if (this.m_compositeProperties == null)
				{
					this.CreateCompositeProperties();
				}
				PropertiesDictionary propertiesDictionary = this.m_compositeProperties.Flatten();
				PropertiesDictionary propertiesDictionary2 = new PropertiesDictionary();
				foreach (DictionaryEntry dictionaryEntry in (IEnumerable)propertiesDictionary)
				{
					string text = dictionaryEntry.Key as string;
					if (text != null)
					{
						object obj = dictionaryEntry.Value;
						IFixingRequired fixingRequired = obj as IFixingRequired;
						if (fixingRequired != null)
						{
							obj = fixingRequired.GetFixedObject();
						}
						if (obj != null)
						{
							propertiesDictionary2[text] = obj;
						}
					}
				}
				this.m_data.Properties = propertiesDictionary2;
			}
		}
		public object LookupProperty(string key)
		{
			object result;
			if (this.m_data.Properties != null)
			{
				result = this.m_data.Properties[key];
			}
			else
			{
				if (this.m_compositeProperties == null)
				{
					this.CreateCompositeProperties();
				}
				result = this.m_compositeProperties[key];
			}
			return result;
		}
		public PropertiesDictionary GetProperties()
		{
			PropertiesDictionary result;
			if (this.m_data.Properties != null)
			{
				result = this.m_data.Properties;
			}
			else
			{
				if (this.m_compositeProperties == null)
				{
					this.CreateCompositeProperties();
				}
				result = this.m_compositeProperties.Flatten();
			}
			return result;
		}
	}
}
