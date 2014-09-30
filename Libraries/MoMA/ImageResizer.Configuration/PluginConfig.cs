using ImageResizer.Caching;
using ImageResizer.Collections;
using ImageResizer.Configuration.Issues;
using ImageResizer.Configuration.Xml;
using ImageResizer.Encoding;
using ImageResizer.Plugins;
using ImageResizer.Plugins.Basic;
using ImageResizer.Resizing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
namespace ImageResizer.Configuration
{
	[ComVisible(true)]
	public class PluginConfig : IssueSink, IEncoderProvider
	{
		protected Config c;
		protected volatile bool _pluginsLoaded;
		protected object _loadPluginsSync = new object();
		protected SafeList<BuilderExtension> imageBuilderExtensions;
		protected SafeList<IEncoder> imageEncoders;
		protected SafeList<ICache> cachingSystems;
		protected SafeList<IQuerystringPlugin> querystringPlugins;
		protected SafeList<IFileExtensionPlugin> fileExtensionPlugins;
		protected SafeList<IVirtualImageProvider> virtualProviderPlugins;
		protected SafeList<IPlugin> allPlugins;
		public bool PluginsLoaded
		{
			get
			{
				object loadPluginsSync;
				Monitor.Enter(loadPluginsSync = this._loadPluginsSync);
				bool pluginsLoaded;
				try
				{
					pluginsLoaded = this._pluginsLoaded;
				}
				finally
				{
					Monitor.Exit(loadPluginsSync);
				}
				return pluginsLoaded;
			}
		}
		public SafeList<BuilderExtension> ImageBuilderExtensions
		{
			get
			{
				return this.imageBuilderExtensions;
			}
		}
		public SafeList<IEncoder> ImageEncoders
		{
			get
			{
				return this.imageEncoders;
			}
		}
		public SafeList<ICache> CachingSystems
		{
			get
			{
				return this.cachingSystems;
			}
		}
		public SafeList<IQuerystringPlugin> QuerystringPlugins
		{
			get
			{
				return this.querystringPlugins;
			}
		}
		public SafeList<IFileExtensionPlugin> FileExtensionPlugins
		{
			get
			{
				return this.fileExtensionPlugins;
			}
		}
		public SafeList<IVirtualImageProvider> VirtualProviderPlugins
		{
			get
			{
				return this.virtualProviderPlugins;
			}
		}
		public SafeList<IPlugin> AllPlugins
		{
			get
			{
				return this.allPlugins;
			}
		}
		public IEncoderProvider EncoderProvider
		{
			get
			{
				return this;
			}
		}
		public PluginConfig(Config c) : base("Plugins")
		{
			this.c = c;
			this.imageBuilderExtensions = new SafeList<BuilderExtension>();
			this.imageEncoders = new SafeList<IEncoder>();
			this.cachingSystems = new SafeList<ICache>();
			this.querystringPlugins = new SafeList<IQuerystringPlugin>();
			this.fileExtensionPlugins = new SafeList<IFileExtensionPlugin>();
			this.allPlugins = new SafeList<IPlugin>();
			this.virtualProviderPlugins = new SafeList<IVirtualImageProvider>();
		}
		public void LoadPlugins()
		{
			object loadPluginsSync;
			Monitor.Enter(loadPluginsSync = this._loadPluginsSync);
			try
			{
				if (!this._pluginsLoaded)
				{
					this.loadPluginsInternal();
					this._pluginsLoaded = true;
				}
			}
			finally
			{
				Monitor.Exit(loadPluginsSync);
			}
		}
		protected void loadPluginsInternal()
		{
			Node node = this.c.getNode("plugins");
			if (node == null)
			{
				return;
			}
			foreach (Node current in node.Children)
			{
				if (current.Name.Equals("add", StringComparison.OrdinalIgnoreCase))
				{
					this.add_plugin_by_name(current["name"], (current.Attrs.Count > 1) ? current.Attrs : null);
				}
				else
				{
					if (current.Name.Equals("remove", StringComparison.OrdinalIgnoreCase))
					{
						this.remove_plugins_by_name(current["name"]);
					}
					else
					{
						if (current.Name.Equals("clear", StringComparison.OrdinalIgnoreCase))
						{
							this.clear_plugins_by_type(current["type"]);
						}
						else
						{
							this.AcceptIssue(new Issue("Plugins", "Unexpected element <" + current.Name + "> in <plugins></plugins>.", "Element XML: " + current.ToXmlElement().OuterXml, IssueSeverity.Warning));
						}
					}
				}
			}
		}
		public IList<IPlugin> GetPlugins(Type type)
		{
			List<IPlugin> list = new List<IPlugin>();
			foreach (IPlugin current in this.AllPlugins)
			{
				if (type.IsAssignableFrom(current.GetType()))
				{
					list.Add(current);
				}
			}
			return list;
		}
		public bool HasPlugin(Type type)
		{
			foreach (IPlugin current in this.AllPlugins)
			{
				if (type.IsAssignableFrom(current.GetType()))
				{
					return true;
				}
			}
			return false;
		}
		public IList<T> GetAll<T>()
		{
			List<T> list = new List<T>();
			foreach (IPlugin current in this.AllPlugins)
			{
				if (typeof(T).IsAssignableFrom(current.GetType()))
				{
					list.Add((T)((object)current));
				}
			}
			return list;
		}
		public T Get<T>()
		{
			foreach (IPlugin current in this.AllPlugins)
			{
				if (typeof(T).IsAssignableFrom(current.GetType()))
				{
					return (T)((object)current);
				}
			}
			return default(T);
		}
		public bool Has<T>()
		{
			return Comparer<T>.Default.Compare(this.Get<T>(), default(T)) != 0;
		}
		public IPlugin Install(IPlugin plugin)
		{
			return plugin.Install(this.c);
		}
		public bool Uninstall(IPlugin plugin)
		{
			return plugin.Uninstall(this.c);
		}
		public IEncoder GetEncoder(ResizeSettings settings, object original)
		{
			foreach (IEncoder current in this.ImageEncoders)
			{
				IEncoder encoder = current.CreateIfSuitable(settings, original);
				if (encoder != null)
				{
					return encoder;
				}
			}
			return null;
		}
		protected void remove_plugins_by_name(string name)
		{
			Type type = this.FindPluginType(name);
			if (type == null)
			{
				return;
			}
			foreach (IPlugin current in this.GetPlugins(type))
			{
				if (!current.Uninstall(this.c))
				{
					this.AcceptIssue(new Issue(string.Concat(new string[]
					{
						"Plugin ",
						type.FullName,
						" reported a failed uninstall attempt triggered by a <remove name=\"",
						name,
						"\" />."
					}), IssueSeverity.Error));
				}
			}
		}
		protected void add_plugin_by_name(string name, NameValueCollection args)
		{
			IPlugin plugin = this.CreatePluginByName(name, args);
			if (plugin == null)
			{
				return;
			}
			if (!(plugin is IMultiInstancePlugin) && this.HasPlugin(plugin.GetType()))
			{
				this.AcceptIssue(new Issue("An instance of the specified plugin (" + plugin.GetType().ToString() + ") has already been added. Implement IMultiInstancePlugin if the plugin supports multiple instances.", IssueSeverity.ConfigurationError));
				return;
			}
			plugin.Install(this.c);
		}
		protected void clear_plugins_by_type(string type)
		{
			Type type2 = null;
			if ("encoders".Equals(type, StringComparison.OrdinalIgnoreCase))
			{
				type2 = typeof(IEncoder);
			}
			if ("caches".Equals(type, StringComparison.OrdinalIgnoreCase))
			{
				type2 = typeof(ICache);
			}
			if ("extensions".Equals(type, StringComparison.OrdinalIgnoreCase))
			{
				type2 = typeof(BuilderExtension);
			}
			if ("all".Equals(type, StringComparison.OrdinalIgnoreCase) || type == null)
			{
				type2 = typeof(IPlugin);
			}
			if (type2 == null)
			{
				this.AcceptIssue(new Issue(string.Concat(new string[]
				{
					"Unrecognized type value \"",
					type,
					"\" in <clear type=\"",
					type,
					"\" />."
				}), IssueSeverity.ConfigurationError));
				return;
			}
			IList<IPlugin> plugins = this.GetPlugins(type2);
			foreach (IPlugin current in plugins)
			{
				if (!current.Uninstall(this.c))
				{
					this.AcceptIssue(new Issue(string.Concat(new string[]
					{
						"Plugin ",
						current.GetType().FullName,
						" reported a failed uninstall attempt triggered by a <clear type=\"",
						type,
						"\" />."
					}), IssueSeverity.Error));
				}
			}
		}
		public Type FindPluginType(string searchName)
		{
			Type type = null;
			int num = searchName.IndexOf(',');
			int num2 = searchName.IndexOf('.');
			if (num2 > -1 || num > -1)
			{
				type = Type.GetType(searchName, false, true);
			}
			if (type != null)
			{
				return type;
			}
			string text = (num > -1) ? searchName.Substring(num) : null;
			string text2 = (num > -1) ? searchName.Substring(0, num) : searchName;
			bool flag = text2.IndexOf('.') > -1;
			List<string> list = new List<string>();
			if (flag)
			{
				list.Add(text2);
			}
			list.Add("ImageResizer.Plugins.Basic." + text2.TrimStart(new char[]
			{
				'.'
			}));
			list.Add("ImageResizer.Plugins.Pro." + text2.TrimStart(new char[]
			{
				'.'
			}));
			if (!flag)
			{
				list.Add(string.Concat(new string[]
				{
					"ImageResizer.Plugins.",
					text2.Trim(new char[]
					{
						'.'
					}),
					".",
					text2.Trim(new char[]
					{
						'.'
					}),
					"Plugin"
				}));
			}
			if (!flag && text2.EndsWith("Plugin"))
			{
				list.Add("ImageResizer.Plugins." + text2.Substring(0, text2.Length - 6).Trim(new char[]
				{
					'.'
				}) + "." + text2.Trim(new char[]
				{
					'.'
				}));
			}
			list.Add("ImageResizer.Plugins." + text2.TrimStart(new char[]
			{
				'.'
			}));
			if (!flag)
			{
				list.Add("ImageResizer.Plugins." + text2.Trim(new char[]
				{
					'.'
				}) + "." + text2.Trim(new char[]
				{
					'.'
				}));
			}
			list.Add("ImageResizer." + text2.TrimStart(new char[]
			{
				'.'
			}));
			if (!flag)
			{
				list.Add(text2);
			}
			List<string> list2 = new List<string>();
			if (text != null)
			{
				list2.Add(text);
			}
			list2.Add("");
			List<string> list3 = new List<string>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				Assembly assembly = assemblies[i];
				string fullName = assembly.FullName;
				if (fullName.IndexOf("ImageResizer", StringComparison.OrdinalIgnoreCase) > -1)
				{
					list2.Add(", " + fullName);
				}
				else
				{
					list3.Add(", " + fullName);
				}
			}
			list2.AddRange(list3);
			List<string> list4 = new List<string>(list2.Count * list.Count);
			foreach (string current in list2)
			{
				foreach (string current2 in list)
				{
					list4.Add(current2 + current);
				}
			}
			foreach (string current3 in list4)
			{
				if (type != null)
				{
					return type;
				}
				type = Type.GetType(current3, false, true);
			}
			if (type == null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string current4 in list4)
				{
					stringBuilder.Append(", \"" + current4 + "\"");
				}
				this.AcceptIssue(new Issue("Failed to load plugin by name \"" + searchName + "\"", string.Concat(new string[]
				{
					"Verify the plugin DLL is located in /bin, and that the name is spelled correctly. \nAttempted using \"",
					searchName,
					"\"",
					stringBuilder.ToString(),
					"."
				}), IssueSeverity.ConfigurationError));
			}
			return type;
		}
		protected IPlugin CreatePluginByName(string name, NameValueCollection args)
		{
			Type type = this.FindPluginType(name);
			if (type == null)
			{
				return null;
			}
			if (args != null && type.GetConstructor(new Type[]
			{
				typeof(NameValueCollection)
			}) == null)
			{
				args = null;
				this.AcceptIssue(new Issue("Plugins", "The plugin " + type.ToString() + " doesn't support constructor arguments.", "To support arguments in the <add> element, the plugin must have a public constructor that accepts a NameValueCollection argument.", IssueSeverity.ConfigurationError));
			}
			else
			{
				if (args == null && type.GetConstructor(Type.EmptyTypes) == null)
				{
					bool flag = type.GetConstructor(new Type[]
					{
						typeof(NameValueCollection)
					}) == null;
					if (flag)
					{
						this.AcceptIssue(new Issue("Plugins", "The plugin " + type.ToString() + " requires arguments in the <add> element. Refer to the plugin documentation for details.", null, IssueSeverity.ConfigurationError));
					}
					else
					{
						this.AcceptIssue(new Issue("Plugins", "The plugin " + type.ToString() + " does not have a constuctor   Constructor() or Constructor(NameValueCollection args).", "To be compatible with the <plugins> section, a plugin must implement IPlugin and define one or more of the above constructors publicly.", IssueSeverity.Critical));
					}
				}
			}
			object obj;
			if (args == null)
			{
				obj = Activator.CreateInstance(type, false);
			}
			else
			{
				obj = Activator.CreateInstance(type, new object[]
				{
					args
				});
			}
			if (!(obj is IPlugin))
			{
				this.AcceptIssue(new Issue("Plugins", "Specified plugin doesn't implement IPlugin as required: " + type.ToString(), null, IssueSeverity.ConfigurationError));
			}
			return obj as IPlugin;
		}
		public void remove_plugin(object plugin)
		{
			if (plugin is IPlugin)
			{
				this.AllPlugins.Remove(plugin as IPlugin);
			}
			if (plugin is IQuerystringPlugin)
			{
				this.QuerystringPlugins.Remove(plugin as IQuerystringPlugin);
			}
			if (plugin is IFileExtensionPlugin)
			{
				this.FileExtensionPlugins.Remove(plugin as IFileExtensionPlugin);
			}
			if (plugin is ICache)
			{
				this.CachingSystems.Remove(plugin as ICache);
			}
			if (plugin is IEncoder)
			{
				this.ImageEncoders.Remove(plugin as IEncoder);
			}
			if (plugin is BuilderExtension)
			{
				this.ImageBuilderExtensions.Remove(plugin as BuilderExtension);
			}
			if (plugin is IVirtualImageProvider)
			{
				this.VirtualProviderPlugins.Remove(plugin as IVirtualImageProvider);
			}
		}
		public void add_plugin(IPlugin plugin)
		{
			if (!(plugin is IMultiInstancePlugin) && this.HasPlugin(plugin.GetType()))
			{
				this.AcceptIssue(new Issue("An instance of the specified plugin (" + plugin.GetType().ToString() + ") has already been registered.", "The plugin should implement IMultiInstancePlugin to support multiple instances.", IssueSeverity.Error));
				return;
			}
			this.AllPlugins.Add(plugin);
			if (plugin is IQuerystringPlugin)
			{
				this.QuerystringPlugins.Add(plugin as IQuerystringPlugin);
			}
			if (plugin is IFileExtensionPlugin)
			{
				this.FileExtensionPlugins.Add(plugin as IFileExtensionPlugin);
			}
			if (plugin is ICache)
			{
				this.CachingSystems.AddFirst(plugin as ICache);
			}
			if (plugin is IEncoder)
			{
				this.ImageEncoders.AddFirst(plugin as IEncoder);
			}
			if (plugin is BuilderExtension)
			{
				this.ImageBuilderExtensions.Add(plugin as BuilderExtension);
			}
			if (plugin is IVirtualImageProvider)
			{
				this.VirtualProviderPlugins.Add(plugin as IVirtualImageProvider);
			}
		}
		public void RemoveAll()
		{
			foreach (IPlugin current in this.AllPlugins)
			{
				if (!current.Uninstall(this.c))
				{
					this.AcceptIssue(new Issue("Uninstall of " + current.ToString() + " reported failure.", IssueSeverity.Error));
				}
			}
			IList[] array = new IList[]
			{
				this.AllPlugins.GetCollection(),
				this.QuerystringPlugins.GetCollection(),
				this.FileExtensionPlugins.GetCollection(),
				this.CachingSystems.GetCollection(),
				this.ImageEncoders.GetCollection(),
				this.ImageBuilderExtensions.GetCollection()
			};
			IList[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				IList list = array2[i];
				if (list.Count > 0)
				{
					string text = "";
					foreach (object current2 in list)
					{
						text = text + current2.ToString() + ", ";
					}
					this.AcceptIssue(new Issue("Collection " + list.ToString() + " was not empty after RemoveAllPlugins() executed!", "Remaining items: " + text, IssueSeverity.Error));
				}
			}
		}
		public override IEnumerable<IIssue> GetIssues()
		{
			List<IIssue> list = new List<IIssue>(base.GetIssues());
			if (this.c.Plugins.CachingSystems.First is NoCache)
			{
				list.Add(new Issue("NoCache is only for development usage, and cannot scale to production use.", "Add DiskCache or S3Cache for production use", IssueSeverity.Warning));
			}
			if (!this.c.Plugins.Has<NoCache>())
			{
				list.Add(new Issue("NoCache should not be removed from the plugins collection.", "Simply add the new ICache plugin later for it to take precedence. NoCache is still required as a fallback by most caching plugins.", IssueSeverity.Error));
			}
			if (this.c.Plugins.ImageEncoders.First == null)
			{
				list.Add(new Issue("No encoders are registered! Without an image encoder, the pipeline cannot function.", IssueSeverity.Error));
			}
			return list;
		}
	}
}
