using log4net.Appender;
using log4net.Core;
using log4net.Util;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;
namespace log4net.Repository.Hierarchy
{
	public class Hierarchy : LoggerRepositorySkeleton, IBasicRepositoryConfigurator, IXmlRepositoryConfigurator
	{
		internal class LevelEntry
		{
			private int m_levelValue = -1;
			private string m_levelName = null;
			private string m_levelDisplayName = null;
			public int Value
			{
				get
				{
					return this.m_levelValue;
				}
				set
				{
					this.m_levelValue = value;
				}
			}
			public string Name
			{
				get
				{
					return this.m_levelName;
				}
				set
				{
					this.m_levelName = value;
				}
			}
			public string DisplayName
			{
				get
				{
					return this.m_levelDisplayName;
				}
				set
				{
					this.m_levelDisplayName = value;
				}
			}
			public override string ToString()
			{
				return string.Concat(new object[]
				{
					"LevelEntry(Value=",
					this.m_levelValue,
					", Name=",
					this.m_levelName,
					", DisplayName=",
					this.m_levelDisplayName,
					")"
				});
			}
		}
		internal class PropertyEntry
		{
			private string m_key = null;
			private object m_value = null;
			public string Key
			{
				get
				{
					return this.m_key;
				}
				set
				{
					this.m_key = value;
				}
			}
			public object Value
			{
				get
				{
					return this.m_value;
				}
				set
				{
					this.m_value = value;
				}
			}
			public override string ToString()
			{
				return string.Concat(new object[]
				{
					"PropertyEntry(Key=",
					this.m_key,
					", Value=",
					this.m_value,
					")"
				});
			}
		}
		private ILoggerFactory m_defaultFactory;
		private Hashtable m_ht;
		private Logger m_root;
		private bool m_emittedNoAppenderWarning = false;
		public event LoggerCreationEventHandler LoggerCreatedEvent
		{
			add
			{
				this.m_loggerCreatedEvent = (LoggerCreationEventHandler)Delegate.Combine(this.m_loggerCreatedEvent, value);
			}
			remove
			{
				this.m_loggerCreatedEvent = (LoggerCreationEventHandler)Delegate.Remove(this.m_loggerCreatedEvent, value);
			}
		}
		private event LoggerCreationEventHandler m_loggerCreatedEvent
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.m_loggerCreatedEvent = (LoggerCreationEventHandler)Delegate.Combine(this.m_loggerCreatedEvent, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.m_loggerCreatedEvent = (LoggerCreationEventHandler)Delegate.Remove(this.m_loggerCreatedEvent, value);
			}
		}
		public bool EmittedNoAppenderWarning
		{
			get
			{
				return this.m_emittedNoAppenderWarning;
			}
			set
			{
				this.m_emittedNoAppenderWarning = value;
			}
		}
		public Logger Root
		{
			get
			{
				if (this.m_root == null)
				{
					Monitor.Enter(this);
					try
					{
						if (this.m_root == null)
						{
							Logger logger = this.m_defaultFactory.CreateLogger(null);
							logger.Hierarchy = this;
							this.m_root = logger;
						}
					}
					finally
					{
						Monitor.Exit(this);
					}
				}
				return this.m_root;
			}
		}
		public ILoggerFactory LoggerFactory
		{
			get
			{
				return this.m_defaultFactory;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.m_defaultFactory = value;
			}
		}
		public Hierarchy() : this(new DefaultLoggerFactory())
		{
		}
		public Hierarchy(PropertiesDictionary properties) : this(properties, new DefaultLoggerFactory())
		{
		}
		public Hierarchy(ILoggerFactory loggerFactory) : this(new PropertiesDictionary(), loggerFactory)
		{
		}
		public Hierarchy(PropertiesDictionary properties, ILoggerFactory loggerFactory) : base(properties)
		{
			if (loggerFactory == null)
			{
				throw new ArgumentNullException("loggerFactory");
			}
			this.m_defaultFactory = loggerFactory;
			this.m_ht = Hashtable.Synchronized(new Hashtable());
		}
		public override ILogger Exists(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			return this.m_ht[new LoggerKey(name)] as Logger;
		}
		public override ILogger[] GetCurrentLoggers()
		{
			ArrayList arrayList = new ArrayList(this.m_ht.Count);
			foreach (object current in this.m_ht.Values)
			{
				if (current is Logger)
				{
					arrayList.Add(current);
				}
			}
			return (Logger[])arrayList.ToArray(typeof(Logger));
		}
		public override ILogger GetLogger(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			return this.GetLogger(name, this.m_defaultFactory);
		}
		public override void Shutdown()
		{
			LogLog.Debug("Hierarchy: Shutdown called on Hierarchy [" + this.Name + "]");
			this.Root.CloseNestedAppenders();
			Hashtable ht;
			Monitor.Enter(ht = this.m_ht);
			try
			{
				ILogger[] currentLoggers = this.GetCurrentLoggers();
				ILogger[] array = currentLoggers;
				for (int i = 0; i < array.Length; i++)
				{
					Logger logger = (Logger)array[i];
					logger.CloseNestedAppenders();
				}
				this.Root.RemoveAllAppenders();
				array = currentLoggers;
				for (int i = 0; i < array.Length; i++)
				{
					Logger logger = (Logger)array[i];
					logger.RemoveAllAppenders();
				}
			}
			finally
			{
				Monitor.Exit(ht);
			}
			base.Shutdown();
		}
		public override void ResetConfiguration()
		{
			this.Root.Level = Level.Debug;
			this.Threshold = Level.All;
			Hashtable ht;
			Monitor.Enter(ht = this.m_ht);
			try
			{
				this.Shutdown();
				ILogger[] currentLoggers = this.GetCurrentLoggers();
				for (int i = 0; i < currentLoggers.Length; i++)
				{
					Logger logger = (Logger)currentLoggers[i];
					logger.Level = null;
					logger.Additivity = true;
				}
			}
			finally
			{
				Monitor.Exit(ht);
			}
			base.ResetConfiguration();
			this.OnConfigurationChanged(null);
		}
		public override void Log(LoggingEvent logEvent)
		{
			if (logEvent == null)
			{
				throw new ArgumentNullException("logEvent");
			}
			this.GetLogger(logEvent.LoggerName, this.m_defaultFactory).Log(logEvent);
		}
		public override IAppender[] GetAppenders()
		{
			ArrayList arrayList = new ArrayList();
			Hierarchy.CollectAppenders(arrayList, this.Root);
			ILogger[] currentLoggers = this.GetCurrentLoggers();
			for (int i = 0; i < currentLoggers.Length; i++)
			{
				Logger container = (Logger)currentLoggers[i];
				Hierarchy.CollectAppenders(arrayList, container);
			}
			return (IAppender[])arrayList.ToArray(typeof(IAppender));
		}
		private static void CollectAppender(ArrayList appenderList, IAppender appender)
		{
			if (!appenderList.Contains(appender))
			{
				appenderList.Add(appender);
				IAppenderAttachable appenderAttachable = appender as IAppenderAttachable;
				if (appenderAttachable != null)
				{
					Hierarchy.CollectAppenders(appenderList, appenderAttachable);
				}
			}
		}
		private static void CollectAppenders(ArrayList appenderList, IAppenderAttachable container)
		{
			foreach (IAppender current in container.Appenders)
			{
				Hierarchy.CollectAppender(appenderList, current);
			}
		}
		void IBasicRepositoryConfigurator.Configure(IAppender appender)
		{
			this.BasicRepositoryConfigure(appender);
		}
		protected void BasicRepositoryConfigure(IAppender appender)
		{
			this.Root.AddAppender(appender);
			this.Configured = true;
			this.OnConfigurationChanged(null);
		}
		void IXmlRepositoryConfigurator.Configure(XmlElement element)
		{
			this.XmlRepositoryConfigure(element);
		}
		protected void XmlRepositoryConfigure(XmlElement element)
		{
			XmlHierarchyConfigurator xmlHierarchyConfigurator = new XmlHierarchyConfigurator(this);
			xmlHierarchyConfigurator.Configure(element);
			this.Configured = true;
			this.OnConfigurationChanged(null);
		}
		public bool IsDisabled(Level level)
		{
			if (level == null)
			{
				throw new ArgumentNullException("level");
			}
			return !this.Configured || this.Threshold > level;
		}
		public void Clear()
		{
			this.m_ht.Clear();
		}
		public Logger GetLogger(string name, ILoggerFactory factory)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			LoggerKey key = new LoggerKey(name);
			Hashtable ht;
			Monitor.Enter(ht = this.m_ht);
			Logger result;
			try
			{
				object obj = this.m_ht[key];
				if (obj == null)
				{
					Logger logger = factory.CreateLogger(name);
					logger.Hierarchy = this;
					this.m_ht[key] = logger;
					this.UpdateParents(logger);
					this.OnLoggerCreationEvent(logger);
					result = logger;
				}
				else
				{
					Logger logger2 = obj as Logger;
					if (logger2 != null)
					{
						result = logger2;
					}
					else
					{
						ProvisionNode provisionNode = obj as ProvisionNode;
						if (provisionNode != null)
						{
							Logger logger = factory.CreateLogger(name);
							logger.Hierarchy = this;
							this.m_ht[key] = logger;
							this.UpdateChildren(provisionNode, logger);
							this.UpdateParents(logger);
							this.OnLoggerCreationEvent(logger);
							result = logger;
						}
						else
						{
							result = null;
						}
					}
				}
			}
			finally
			{
				Monitor.Exit(ht);
			}
			return result;
		}
		protected virtual void OnLoggerCreationEvent(Logger logger)
		{
			LoggerCreationEventHandler loggerCreatedEvent = this.m_loggerCreatedEvent;
			if (loggerCreatedEvent != null)
			{
				loggerCreatedEvent(this, new LoggerCreationEventArgs(logger));
			}
		}
		private void UpdateParents(Logger log)
		{
			string name = log.Name;
			int length = name.Length;
			bool flag = false;
			for (int i = name.LastIndexOf('.', length - 1); i >= 0; i = name.LastIndexOf('.', i - 1))
			{
				string name2 = name.Substring(0, i);
				LoggerKey key = new LoggerKey(name2);
				object obj = this.m_ht[key];
				if (obj == null)
				{
					ProvisionNode value = new ProvisionNode(log);
					this.m_ht[key] = value;
				}
				else
				{
					Logger logger = obj as Logger;
					if (logger != null)
					{
						flag = true;
						log.Parent = logger;
						break;
					}
					ProvisionNode provisionNode = obj as ProvisionNode;
					if (provisionNode != null)
					{
						provisionNode.Add(log);
					}
					else
					{
						LogLog.Error("Hierarchy: Unexpected object type [" + obj.GetType() + "] in ht.", new LogException());
					}
				}
			}
			if (!flag)
			{
				log.Parent = this.Root;
			}
		}
		private void UpdateChildren(ProvisionNode pn, Logger log)
		{
			for (int i = 0; i < pn.Count; i++)
			{
				Logger logger = (Logger)pn[i];
				if (!logger.Parent.Name.StartsWith(log.Name))
				{
					log.Parent = logger.Parent;
					logger.Parent = log;
				}
			}
		}
		internal void AddLevel(Hierarchy.LevelEntry levelEntry)
		{
			if (levelEntry == null)
			{
				throw new ArgumentNullException("levelEntry");
			}
			if (levelEntry.Name == null)
			{
				throw new ArgumentNullException("levelEntry.Name");
			}
			if (levelEntry.Value == -1)
			{
				Level level = this.LevelMap[levelEntry.Name];
				if (level == null)
				{
					throw new InvalidOperationException("Cannot redefine level [" + levelEntry.Name + "] because it is not defined in the LevelMap. To define the level supply the level value.");
				}
				levelEntry.Value = level.Value;
			}
			this.LevelMap.Add(levelEntry.Name, levelEntry.Value, levelEntry.DisplayName);
		}
		internal void AddProperty(Hierarchy.PropertyEntry propertyEntry)
		{
			if (propertyEntry == null)
			{
				throw new ArgumentNullException("propertyEntry");
			}
			if (propertyEntry.Key == null)
			{
				throw new ArgumentNullException("propertyEntry.Key");
			}
			base.Properties[propertyEntry.Key] = propertyEntry.Value;
		}
	}
}
