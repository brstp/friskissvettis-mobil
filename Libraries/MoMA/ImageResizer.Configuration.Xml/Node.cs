using ImageResizer.Configuration.Issues;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
namespace ImageResizer.Configuration.Xml
{
	[ComVisible(true)]
	public class Node
	{
		private NameValueCollection attrs = new NameValueCollection();
		private string name;
		private List<Node> children = new List<Node>();
		protected Dictionary<string, ICollection<Node>> _cachedResults = new Dictionary<string, ICollection<Node>>(StringComparer.OrdinalIgnoreCase);
		public NameValueCollection Attrs
		{
			get
			{
				return this.attrs;
			}
			set
			{
				this.attrs = value;
			}
		}
		public string this[string name]
		{
			get
			{
				return this.attrs[name];
			}
			set
			{
				this.attrs[name] = value;
			}
		}
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}
		public List<Node> Children
		{
			get
			{
				return this.children;
			}
			set
			{
				this.children = value;
			}
		}
		public bool IsEmpty
		{
			get
			{
				return (this.attrs == null || this.attrs.Count == 0) && (this.children == null || this.children.Count == 0);
			}
		}
		public Node(string localName)
		{
			this.name = localName;
		}
		public Node(XmlElement e, IIssueReceiver ir)
		{
			this.name = e.LocalName;
			foreach (XmlAttribute xmlAttribute in e.Attributes)
			{
				if (this.attrs[xmlAttribute.LocalName] != null)
				{
					ir.AcceptIssue(new Issue((string.Concat(new object[]
					{
						"Two or more attributes named ",
						xmlAttribute.LocalName,
						" found on element ",
						this.name,
						" in ",
						e.ParentNode
					}) != null) ? e.ParentNode.Name : "(unknown node)"));
				}
				this.attrs[xmlAttribute.LocalName] = xmlAttribute.Value;
			}
			if (e.HasChildNodes)
			{
				foreach (XmlNode xmlNode in e.ChildNodes)
				{
					if (xmlNode.NodeType == XmlNodeType.Element)
					{
						XmlElement xmlElement = xmlNode as XmlElement;
						if (xmlElement != null)
						{
							this.children.Add(new Node(xmlElement, ir));
						}
					}
				}
			}
		}
		public IList<Node> childrenByName(string elementName)
		{
			if (this.children == null || this.children.Count == 0)
			{
				return null;
			}
			List<Node> list = null;
			foreach (Node current in this.children)
			{
				if (current.Name.Equals(elementName, StringComparison.OrdinalIgnoreCase))
				{
					if (list == null)
					{
						list = new List<Node>();
					}
					list.Add(current);
				}
			}
			return list;
		}
		public string queryAttr(string selector)
		{
			KeyValuePair<string, string> keyValuePair = this.parseAttributeName(selector);
			return this.queryAttr(keyValuePair.Key, keyValuePair.Value);
		}
		protected KeyValuePair<string, string> parseAttributeName(string selector)
		{
			selector = selector.Trim(new char[]
			{
				'.'
			});
			int num = selector.LastIndexOf('.');
			if (num < 0)
			{
				throw new ArgumentException("Selector must include an attribute name, like element.attrname. Was given '" + selector + "'");
			}
			string key = selector.Substring(0, num);
			string value = selector.Substring(num + 1);
			return new KeyValuePair<string, string>(key, value);
		}
		public string queryAttr(string nodeSelector, string attrName)
		{
			Node node = this.queryFirst(nodeSelector);
			if (node != null)
			{
				return node.Attrs[attrName];
			}
			return null;
		}
		public Node queryFirst(string selector)
		{
			ICollection<Node> collection = this.query(selector);
			if (collection != null && collection.Count > 0)
			{
				using (IEnumerator<Node> enumerator = collection.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						return enumerator.Current;
					}
				}
			}
			return null;
		}
		public void setAttr(string selector, string attrValue)
		{
			KeyValuePair<string, string> keyValuePair = this.parseAttributeName(selector);
			this.setAttr(keyValuePair.Key, keyValuePair.Value, attrValue);
		}
		public void setAttr(string nodeSelector, string attrName, string attrValue)
		{
			Node node = this.queryFirst(nodeSelector);
			if (node == null)
			{
				node = this.makeNodeTree(nodeSelector);
			}
			node.Attrs[attrName] = attrValue;
			this.clearQueryCache();
		}
		public Node makeNodeTree(string selector)
		{
			Node node = this;
			Selector selector2 = new Selector(selector);
			foreach (string current in selector2)
			{
				IList<Node> list = node.childrenByName(current);
				if (list == null || list.Count == 0)
				{
					Node node2 = new Node(current);
					if (node.children == null)
					{
						node.children = new List<Node>();
					}
					node.Children.Add(node2);
					node = node2;
				}
				else
				{
					node = list[0];
				}
			}
			return node;
		}
		public ICollection<Node> query(string selector)
		{
			if (this._cachedResults.ContainsKey(selector))
			{
				return this._cachedResults[selector];
			}
			IList<Node> list = this.queryUncached(selector) as IList<Node>;
			return this._cachedResults[selector] = ((list != null) ? new ReadOnlyCollection<Node>(list) : null);
		}
		public void clearQueryCache()
		{
			this._cachedResults = new Dictionary<string, ICollection<Node>>(StringComparer.OrdinalIgnoreCase);
		}
		public ICollection<Node> queryUncached(string selector)
		{
			if (this.children == null || this.children.Count == 0)
			{
				return null;
			}
			selector = selector.Trim(new char[]
			{
				'.'
			});
			int num = selector.IndexOf('.');
			string value = (num > -1) ? selector.Substring(0, num) : selector;
			string text = (num > -1) ? selector.Substring(num + 1) : null;
			List<Node> list = null;
			foreach (Node current in this.children)
			{
				if (current.Name.Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					if (string.IsNullOrEmpty(text))
					{
						if (list == null)
						{
							list = new List<Node>();
						}
						list.Add(current);
					}
					else
					{
						ICollection<Node> collection = current.queryUncached(text);
						if (collection != null)
						{
							if (list == null)
							{
								list = new List<Node>();
							}
							list.AddRange(collection);
						}
					}
				}
			}
			return list;
		}
		public Node deepCopy()
		{
			Node node = new Node(this.name);
			foreach (string text in this.Attrs.Keys)
			{
				node[text] = this[text];
			}
			if (this.children != null)
			{
				foreach (Node current in this.Children)
				{
					node.Children.Add(current.deepCopy());
				}
			}
			return node;
		}
		public XmlElement ToXmlElement()
		{
			return this.ToXmlElement(new XmlDocument());
		}
		public XmlElement ToXmlElement(XmlDocument doc)
		{
			XmlElement xmlElement = doc.CreateElement(this.Name);
			if (this.attrs != null)
			{
				foreach (string text in this.attrs)
				{
					XmlAttribute xmlAttribute = doc.CreateAttribute(text);
					xmlAttribute.Value = this[text];
					xmlElement.Attributes.Append(xmlAttribute);
				}
			}
			if (this.children != null)
			{
				foreach (Node current in this.children)
				{
					xmlElement.AppendChild(current.ToXmlElement(doc));
				}
			}
			return xmlElement;
		}
		public override string ToString()
		{
			return this.ToXmlElement(new XmlDocument()).OuterXml.Replace(">", ">\n");
		}
		public static Node FromXmlFragment(string xml, IssueSink sink)
		{
			NameTable nameTable = new NameTable();
			XmlNamespaceManager nsMgr = new XmlNamespaceManager(nameTable);
			XmlParserContext context = new XmlParserContext(nameTable, nsMgr, "elem", XmlSpace.None, Encoding.UTF8);
			XmlTextReader xmlTextReader = new XmlTextReader(xml, XmlNodeType.Element, context);
			Node result = new Node(new XmlDocument().ReadNode(xmlTextReader) as XmlElement, sink);
			xmlTextReader.Close();
			return result;
		}
	}
}
