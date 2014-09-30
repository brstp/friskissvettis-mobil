using ImageResizer.Collections;
using ImageResizer.Configuration.Issues;
using ImageResizer.Configuration.Xml;
using ImageResizer.Plugins.Basic;
using ImageResizer.Resizing;
using System;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Web;
using System.Web.Hosting;
namespace ImageResizer.Configuration
{
	[ComVisible(true)]
	public class Config
	{
		private static volatile Config _singleton = null;
		private static object _singletonLock = new object();
		protected PluginConfig plugins;
		private PipelineConfig pipeline;
		protected volatile ImageBuilder _imageBuilder;
		protected volatile object _imageBuilderSync = new object();
		private volatile ResizerSection configuration;
		private object configurationLock = new object();
		public static Config Current
		{
			get
			{
				if (Config._singleton == null)
				{
					object singletonLock;
					Monitor.Enter(singletonLock = Config._singletonLock);
					try
					{
						if (Config._singleton == null)
						{
							Config._singleton = new Config(null);
						}
					}
					finally
					{
						Monitor.Exit(singletonLock);
					}
				}
				return Config._singleton;
			}
		}
		public PluginConfig Plugins
		{
			get
			{
				return this.plugins;
			}
		}
		public PipelineConfig Pipeline
		{
			get
			{
				return this.pipeline;
			}
		}
		public ImageBuilder CurrentImageBuilder
		{
			get
			{
				if (this._imageBuilder == null)
				{
					object imageBuilderSync;
					Monitor.Enter(imageBuilderSync = this._imageBuilderSync);
					try
					{
						if (this._imageBuilder == null)
						{
							this._imageBuilder = new ImageBuilder(this.plugins.ImageBuilderExtensions, this.plugins);
						}
					}
					finally
					{
						Monitor.Exit(imageBuilderSync);
					}
				}
				return this._imageBuilder;
			}
		}
		protected ResizerSection cs
		{
			get
			{
				if (this.configuration == null)
				{
					object obj;
					Monitor.Enter(obj = this.configurationLock);
					try
					{
						if (this.configuration == null)
						{
							ResizerSection resizerSection = ConfigurationManager.GetSection("resizer") as ResizerSection;
							this.configuration = ((resizerSection != null) ? resizerSection : new ResizerSection());
						}
					}
					finally
					{
						Monitor.Exit(obj);
					}
				}
				return this.configuration;
			}
		}
		public IssueSink configurationSectionIssues
		{
			get
			{
				return this.cs.IssueSink;
			}
		}
		public IIssueProvider AllIssues
		{
			get
			{
				return new IssueGatherer(this);
			}
		}
		public Config() : this(new ResizerSection())
		{
		}
		public Config(ResizerSection config)
		{
			this.configuration = config;
			this.plugins = new PluginConfig(this);
			this.plugins.ImageBuilderExtensions.Changed += delegate(SafeList<BuilderExtension> s)
			{
				this.InvalidateImageBuilder();
			};
			this.pipeline = new PipelineConfig(this);
			bool flag = HostingEnvironment.ApplicationPhysicalPath != null;
			new DefaultEncoder().Install(this);
			new NoCache().Install(this);
			new ClientCache().Install(this);
			new Diagnostic().Install(this);
			if (flag)
			{
				new SizeLimiting().Install(this);
			}
			if (flag)
			{
				this.plugins.LoadPlugins();
				return;
			}
			this.pipeline.OnFirstRequest += delegate(IHttpModule sender, HttpContext context)
			{
				this.Plugins.LoadPlugins();
			};
		}
		public void UpgradeImageBuilder(ImageBuilder replacement)
		{
			object imageBuilderSync;
			Monitor.Enter(imageBuilderSync = this._imageBuilderSync);
			try
			{
				this._imageBuilder = replacement.Create(this.plugins.ImageBuilderExtensions, this.plugins);
			}
			finally
			{
				Monitor.Exit(imageBuilderSync);
			}
		}
		public void BuildImage(object source, object dest, string settings)
		{
			this.CurrentImageBuilder.Build(source, dest, new ResizeSettings(settings));
		}
		protected void InvalidateImageBuilder()
		{
			object imageBuilderSync;
			Monitor.Enter(imageBuilderSync = this._imageBuilderSync);
			try
			{
				if (this._imageBuilder != null)
				{
					this._imageBuilder = this._imageBuilder.Create(this.plugins.ImageBuilderExtensions, this.plugins);
				}
			}
			finally
			{
				Monitor.Exit(imageBuilderSync);
			}
		}
		public string get(string selector, string defaultValue)
		{
			return this.cs.getAttr(selector, defaultValue);
		}
		public int get(string selector, int defaultValue)
		{
			string attr = this.cs.getAttr(selector, defaultValue.ToString());
			int result;
			if (int.TryParse(attr, out result))
			{
				return result;
			}
			this.configurationSectionIssues.AcceptIssue(new Issue("Invalid integer value in imageresizer configuration section, " + selector + ":" + attr, IssueSeverity.ConfigurationError));
			return defaultValue;
		}
		public bool get(string selector, bool defaultValue)
		{
			string attr = this.cs.getAttr(selector, defaultValue.ToString());
			if ("true".Equals(attr, StringComparison.OrdinalIgnoreCase) || "1".Equals(attr, StringComparison.OrdinalIgnoreCase) || "yes".Equals(attr, StringComparison.OrdinalIgnoreCase) || "on".Equals(attr, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			if ("false".Equals(attr, StringComparison.OrdinalIgnoreCase) || "0".Equals(attr, StringComparison.OrdinalIgnoreCase) || "no".Equals(attr, StringComparison.OrdinalIgnoreCase) || "off".Equals(attr, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			this.configurationSectionIssues.AcceptIssue(new Issue("Invalid boolean value in imageresizer configuration section, " + selector + ":" + attr, IssueSeverity.ConfigurationError));
			return defaultValue;
		}
		public T get<T>(string selector, T defaultValue) where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException("T must be an enumerated type");
			}
			string text = this.get(selector, null);
			if (text == null)
			{
				return defaultValue;
			}
			text = text.Trim();
			T result;
			try
			{
				result = (T)((object)Enum.Parse(typeof(T), text, true));
			}
			catch (ArgumentException)
			{
				string[] names = Enum.GetNames(typeof(T));
				string text2 = "";
				for (int i = 0; i < names.Length; i++)
				{
					text2 += ((i == names.Length - 1) ? (", and " + names[i]) : (((i != 0) ? ", " : "") + names[i]));
				}
				this.configurationSectionIssues.AcceptIssue(new Issue(string.Concat(new string[]
				{
					"Failed to parse ",
					selector,
					". Invalid value \"",
					text,
					"\"."
				}), "Valid values are " + text2, IssueSeverity.ConfigurationError));
				result = defaultValue;
			}
			return result;
		}
		public Node getNode(string selector)
		{
			return this.cs.getCopyOfNode(selector);
		}
		public Node getConfigXml()
		{
			return this.cs.getCopyOfRootNode();
		}
		public void setConfigXml(Node n)
		{
			this.cs.replaceRootNode(n);
		}
		public void setConfigXmlText(string xml)
		{
			this.cs.replaceRootNode(Node.FromXmlFragment(xml, this.cs.IssueSink));
		}
		public void WriteDiagnosticsTo(string path)
		{
			File.WriteAllText(path, this.GetDiagnosticsPage());
		}
		public string GetDiagnosticsPage()
		{
			return new DiagnosticPageHandler(this).GenerateOutput(HttpContext.Current, this);
		}
	}
}
