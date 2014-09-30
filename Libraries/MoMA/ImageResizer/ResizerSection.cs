using ImageResizer.Configuration.Issues;
using ImageResizer.Configuration.Xml;
using System;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml;
namespace ImageResizer
{
	[ComVisible(true)]
	public class ResizerSection : ConfigurationSection
	{
		protected object nSync = new object();
		protected volatile Node n = new Node("resizer");
		protected volatile XmlDocument xmlDoc = new XmlDocument();
		protected IssueSink sink = new IssueSink("resizer configuration");
		public IssueSink IssueSink
		{
			get
			{
				return this.sink;
			}
		}
		public ResizerSection()
		{
		}
		public ResizerSection(Node root)
		{
			this.n = root;
		}
		public ResizerSection(string xml)
		{
			this.n = Node.FromXmlFragment(xml, this.sink);
		}
		public Node getCopyOfNode(string selector)
		{
			object obj;
			Monitor.Enter(obj = this.nSync);
			Node result;
			try
			{
				Node node = this.n.queryFirst(selector);
				result = ((node != null) ? node.deepCopy() : null);
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return result;
		}
		public Node getCopyOfRootNode()
		{
			object obj;
			Monitor.Enter(obj = this.nSync);
			Node result;
			try
			{
				result = this.n.deepCopy();
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return result;
		}
		public void replaceRootNode(Node n)
		{
			object obj;
			Monitor.Enter(obj = this.nSync);
			try
			{
				this.n = n;
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		public string getAttr(string selector, string defaultValue)
		{
			object obj;
			Monitor.Enter(obj = this.nSync);
			string result;
			try
			{
				string text = this.n.queryAttr(selector);
				result = ((text != null) ? text : defaultValue);
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return result;
		}
		public void setAttr(string selector, string value)
		{
			object obj;
			Monitor.Enter(obj = this.nSync);
			try
			{
				this.n.setAttr(selector, value);
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
		{
			object obj;
			Monitor.Enter(obj = this.nSync);
			try
			{
				this.n.Children.Add(new Node(this.xmlDoc.ReadNode(reader) as XmlElement, this.sink));
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return true;
		}
		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			this.n.Attrs[name] = value;
			return true;
		}
		protected override bool SerializeToXmlElement(XmlWriter writer, string elementName)
		{
			if (this.n.IsEmpty)
			{
				return false;
			}
			XmlElement xmlElement = null;
			object obj;
			Monitor.Enter(obj = this.nSync);
			try
			{
				xmlElement = this.n.ToXmlElement();
			}
			finally
			{
				Monitor.Exit(obj);
			}
			writer.WriteRaw(xmlElement.OuterXml);
			return true;
		}
	}
}
