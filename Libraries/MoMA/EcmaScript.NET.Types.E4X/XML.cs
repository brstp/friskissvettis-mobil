using System;
using System.Collections;
using System.IO;
using System.Xml;
namespace EcmaScript.NET.Types.E4X
{
	internal class XML : XMLObject
	{
		private class XMLTextReader : XmlTextReader
		{
			private XMLLib lib;
			public XMLTextReader(XMLLib lib, StringReader sr) : base(sr)
			{
				this.lib = lib;
			}
			public override bool Read()
			{
				bool result;
				if (!base.Read())
				{
					result = false;
				}
				else
				{
					if (this.lib.ignoreComments && this.NodeType == XmlNodeType.Comment)
					{
						result = this.Read();
					}
					else
					{
						if (this.lib.ignoreProcessingInstructions && this.NodeType == XmlNodeType.ProcessingInstruction)
						{
							result = this.Read();
						}
						else
						{
							result = (!this.lib.ignoreWhitespace || (this.NodeType != XmlNodeType.Whitespace && this.NodeType != XmlNodeType.SignificantWhitespace) || this.Read());
						}
					}
				}
				return result;
			}
		}
		private const int Id_constructor = 1;
		private const int Id_addNamespace = 2;
		private const int Id_appendChild = 3;
		private const int Id_attribute = 4;
		private const int Id_attributes = 5;
		private const int Id_child = 6;
		private const int Id_childIndex = 7;
		private const int Id_children = 8;
		private const int Id_comments = 9;
		private const int Id_contains = 10;
		private const int Id_copy = 11;
		private const int Id_descendants = 12;
		private const int Id_elements = 13;
		private const int Id_hasOwnProperty = 14;
		private const int Id_hasComplexContent = 15;
		private const int Id_hasSimpleContent = 16;
		private const int Id_inScopeNamespaces = 17;
		private const int Id_insertChildAfter = 18;
		private const int Id_insertChildBefore = 19;
		private const int Id_length = 20;
		private const int Id_localName = 21;
		private const int Id_name = 22;
		private const int Id_namespace = 23;
		private const int Id_namespaceDeclarations = 24;
		private const int Id_nodeKind = 25;
		private const int Id_normalize = 26;
		private const int Id_parent = 27;
		private const int Id_processingInstructions = 28;
		private const int Id_prependChild = 29;
		private const int Id_propertyIsEnumerable = 30;
		private const int Id_removeNamespace = 31;
		private const int Id_replace = 32;
		private const int Id_setChildren = 33;
		private const int Id_setLocalName = 34;
		private const int Id_setName = 35;
		private const int Id_setNamespace = 36;
		private const int Id_text = 37;
		private const int Id_toString = 38;
		private const int Id_toXMLString = 39;
		private const int Id_valueOf = 40;
		private const int Id_domNode = 41;
		private const int Id_domNodeList = 42;
		private const int Id_xpath = 43;
		private const int MAX_PROTOTYPE_ID = 43;
		private static readonly object XMLOBJECT_TAG = new object();
		private XmlNode underlyingNode = null;
		protected virtual XmlNode UnderlyingNode
		{
			get
			{
				return this.underlyingNode;
			}
			set
			{
				this.underlyingNode = value;
			}
		}
		public override string ClassName
		{
			get
			{
				return "XML";
			}
		}
		public XML(XMLLib lib) : base(lib, lib.xmlPrototype)
		{
			this.lib = lib;
		}
		public XML(XMLLib lib, XmlNode underlyingNode) : base(lib, lib.xmlPrototype)
		{
			this.lib = lib;
			this.underlyingNode = underlyingNode;
		}
		public void ExportAsJSClass(bool zealed)
		{
			base.ExportAsJSClass(43, this.lib.GlobalScope, zealed);
			this.isPrototype = true;
		}
		protected internal override int FindPrototypeId(string s)
		{
			int result = 0;
			string text = null;
			switch (s.Length)
			{
			case 4:
			{
				int num = (int)s[0];
				if (num == 99)
				{
					text = "copy";
					result = 11;
				}
				else
				{
					if (num == 110)
					{
						text = "name";
						result = 22;
					}
					else
					{
						if (num == 116)
						{
							text = "text";
							result = 37;
						}
					}
				}
				break;
			}
			case 5:
			{
				int num = (int)s[0];
				if (num == 99)
				{
					text = "child";
					result = 6;
				}
				else
				{
					if (num == 120)
					{
						text = "xpath";
						result = 43;
					}
				}
				break;
			}
			case 6:
			{
				int num = (int)s[0];
				if (num == 108)
				{
					text = "length";
					result = 20;
				}
				else
				{
					if (num == 112)
					{
						text = "parent";
						result = 27;
					}
				}
				break;
			}
			case 7:
			{
				char c = s[0];
				if (c != 'd')
				{
					switch (c)
					{
					case 'r':
						text = "replace";
						result = 32;
						break;
					case 's':
						text = "setName";
						result = 35;
						break;
					case 'v':
						text = "valueOf";
						result = 40;
						break;
					}
				}
				else
				{
					text = "domNode";
					result = 41;
				}
				break;
			}
			case 8:
			{
				char c = s[2];
				if (c <= 'e')
				{
					if (c != 'S')
					{
						switch (c)
						{
						case 'd':
							text = "nodeKind";
							result = 25;
							break;
						case 'e':
							text = "elements";
							result = 13;
							break;
						}
					}
					else
					{
						text = "toString";
						result = 38;
					}
				}
				else
				{
					if (c != 'i')
					{
						switch (c)
						{
						case 'm':
							text = "comments";
							result = 9;
							break;
						case 'n':
							text = "contains";
							result = 10;
							break;
						}
					}
					else
					{
						text = "children";
						result = 8;
					}
				}
				break;
			}
			case 9:
			{
				char c = s[2];
				if (c != 'c')
				{
					if (c != 'm')
					{
						switch (c)
						{
						case 'r':
							text = "normalize";
							result = 26;
							break;
						case 't':
							text = "attribute";
							result = 4;
							break;
						}
					}
					else
					{
						text = "namespace";
						result = 23;
					}
				}
				else
				{
					text = "localName";
					result = 21;
				}
				break;
			}
			case 10:
			{
				int num = (int)s[0];
				if (num == 97)
				{
					text = "attributes";
					result = 5;
				}
				else
				{
					if (num == 99)
					{
						text = "childIndex";
						result = 7;
					}
				}
				break;
			}
			case 11:
			{
				char c = s[2];
				if (c != 'X')
				{
					switch (c)
					{
					case 'm':
						text = "domNodeList";
						result = 42;
						break;
					case 'n':
						text = "constructor";
						result = 1;
						break;
					case 'p':
						text = "appendChild";
						result = 3;
						break;
					case 's':
						text = "descendants";
						result = 12;
						break;
					case 't':
						text = "setChildren";
						result = 33;
						break;
					}
				}
				else
				{
					text = "toXMLString";
					result = 39;
				}
				break;
			}
			case 12:
			{
				int num = (int)s[0];
				if (num == 97)
				{
					text = "addNamespace";
					result = 2;
				}
				else
				{
					if (num == 112)
					{
						text = "prependChild";
						result = 29;
					}
					else
					{
						if (num == 115)
						{
							num = (int)s[3];
							if (num == 76)
							{
								text = "setLocalName";
								result = 34;
							}
							else
							{
								if (num == 78)
								{
									text = "setNamespace";
									result = 36;
								}
							}
						}
					}
				}
				break;
			}
			case 14:
				text = "hasOwnProperty";
				result = 14;
				break;
			case 15:
				text = "removeNamespace";
				result = 31;
				break;
			case 16:
			{
				int num = (int)s[0];
				if (num == 104)
				{
					text = "hasSimpleContent";
					result = 16;
				}
				else
				{
					if (num == 105)
					{
						text = "insertChildAfter";
						result = 18;
					}
				}
				break;
			}
			case 17:
			{
				int num = (int)s[3];
				if (num == 67)
				{
					text = "hasComplexContent";
					result = 15;
				}
				else
				{
					if (num == 99)
					{
						text = "inScopeNamespaces";
						result = 17;
					}
					else
					{
						if (num == 101)
						{
							text = "insertChildBefore";
							result = 19;
						}
					}
				}
				break;
			}
			case 20:
				text = "propertyIsEnumerable";
				result = 30;
				break;
			case 21:
				text = "namespaceDeclarations";
				result = 24;
				break;
			case 22:
				text = "processingInstructions";
				result = 28;
				break;
			}
			if (text != null && text != s && !text.Equals(s))
			{
				result = 0;
			}
			return result;
		}
		protected internal override void InitPrototypeId(int id)
		{
			int arity;
			string name;
			switch (id)
			{
			case 1:
			{
				IdFunctionObject f;
				if (this != null)
				{
					f = new XMLCtor(this, XML.XMLOBJECT_TAG, id, 1);
				}
				else
				{
					f = new IdFunctionObject(this, XML.XMLOBJECT_TAG, id, 1);
				}
				base.InitPrototypeConstructor(f);
				return;
			}
			case 2:
				arity = 1;
				name = "addNamespace";
				goto IL_344;
			case 3:
				arity = 1;
				name = "appendChild";
				goto IL_344;
			case 4:
				arity = 1;
				name = "attribute";
				goto IL_344;
			case 5:
				arity = 0;
				name = "attributes";
				goto IL_344;
			case 6:
				arity = 1;
				name = "child";
				goto IL_344;
			case 7:
				arity = 0;
				name = "childIndex";
				goto IL_344;
			case 8:
				arity = 0;
				name = "children";
				goto IL_344;
			case 9:
				arity = 0;
				name = "comments";
				goto IL_344;
			case 10:
				arity = 1;
				name = "contains";
				goto IL_344;
			case 11:
				arity = 0;
				name = "copy";
				goto IL_344;
			case 12:
				arity = 1;
				name = "descendants";
				goto IL_344;
			case 14:
				arity = 1;
				name = "hasOwnProperty";
				goto IL_344;
			case 15:
				arity = 0;
				name = "hasComplexContent";
				goto IL_344;
			case 16:
				arity = 0;
				name = "hasSimpleContent";
				goto IL_344;
			case 17:
				arity = 0;
				name = "inScopeNamespaces";
				goto IL_344;
			case 18:
				arity = 2;
				name = "insertChildAfter";
				goto IL_344;
			case 19:
				arity = 2;
				name = "insertChildBefore";
				goto IL_344;
			case 20:
				arity = 0;
				name = "length";
				goto IL_344;
			case 21:
				arity = 0;
				name = "localName";
				goto IL_344;
			case 22:
				arity = 0;
				name = "name";
				goto IL_344;
			case 23:
				arity = 1;
				name = "namespace";
				goto IL_344;
			case 24:
				arity = 0;
				name = "namespaceDeclarations";
				goto IL_344;
			case 25:
				arity = 0;
				name = "nodeKind";
				goto IL_344;
			case 26:
				arity = 0;
				name = "normalize";
				goto IL_344;
			case 27:
				arity = 0;
				name = "parent";
				goto IL_344;
			case 28:
				arity = 1;
				name = "processingInstructions";
				goto IL_344;
			case 29:
				arity = 1;
				name = "prependChild";
				goto IL_344;
			case 30:
				arity = 1;
				name = "propertyIsEnumerable";
				goto IL_344;
			case 31:
				arity = 1;
				name = "removeNamespace";
				goto IL_344;
			case 32:
				arity = 2;
				name = "replace";
				goto IL_344;
			case 33:
				arity = 1;
				name = "setChildren";
				goto IL_344;
			case 34:
				arity = 1;
				name = "setLocalName";
				goto IL_344;
			case 35:
				arity = 1;
				name = "setName";
				goto IL_344;
			case 36:
				arity = 1;
				name = "setNamespace";
				goto IL_344;
			case 37:
				arity = 0;
				name = "text";
				goto IL_344;
			case 38:
				arity = 0;
				name = "toString";
				goto IL_344;
			case 39:
				arity = 1;
				name = "toXMLString";
				goto IL_344;
			case 40:
				arity = 0;
				name = "valueOf";
				goto IL_344;
			case 41:
				arity = 0;
				name = "domNode";
				goto IL_344;
			case 42:
				arity = 0;
				name = "domNodeList";
				goto IL_344;
			case 43:
				arity = 0;
				name = "xpath";
				goto IL_344;
			}
			throw new ArgumentException(Convert.ToString(id));
			IL_344:
			base.InitPrototypeMethod(XML.XMLOBJECT_TAG, id, name, arity);
		}
		public override object ExecIdCall(IdFunctionObject f, Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			object result;
			if (!f.HasTag(XML.XMLOBJECT_TAG))
			{
				result = base.ExecIdCall(f, cx, scope, thisObj, args);
			}
			else
			{
				int methodId = f.MethodId;
				if (methodId == 1)
				{
					result = this.JsConstructor(cx, thisObj == null, args);
				}
				else
				{
					if (!(thisObj is XML))
					{
						throw IdScriptableObject.IncompatibleCallError(f);
					}
					XML xML = (XML)thisObj;
					switch (methodId)
					{
					case 2:
						result = xML.AddNamespace(base.GetArgSafe(args, 0));
						return result;
					case 3:
						result = xML.AppendChild(base.GetArgSafe(args, 0));
						return result;
					case 4:
					{
						XMLName xMLName = XMLName.Parse(this.lib, cx, base.GetArgSafe(args, 0));
						result = xML.Attribute(xMLName);
						return result;
					}
					case 5:
						result = xML.Attributes();
						return result;
					case 6:
					{
						XMLName xMLName = XMLName.Parse(this.lib, cx, base.GetArgSafe(args, 0));
						if (xMLName == null)
						{
							long num = ScriptRuntime.lastUint32Result(cx);
							result = xML.Child(num);
							return result;
						}
						result = xML.Child(xMLName);
						return result;
					}
					case 7:
						result = xML.ChildIndex();
						return result;
					case 8:
						result = xML.Children();
						return result;
					case 9:
						result = xML.Comments();
						return result;
					case 10:
						result = xML.Contains(base.GetArgSafe(args, 0));
						return result;
					case 11:
						result = xML.Copy();
						return result;
					case 12:
					{
						XMLName xMLName = (args.Length == 0) ? XMLName.FormStar() : XMLName.Parse(this.lib, cx, base.GetArgSafe(args, 0));
						result = xML.Descendants(xMLName);
						return result;
					}
					case 14:
					{
						XMLName xMLName = XMLName.Parse(this.lib, cx, base.GetArgSafe(args, 0));
						result = xML.HasOwnProperty(xMLName);
						return result;
					}
					case 15:
						result = xML.HasComplexContent();
						return result;
					case 16:
						result = xML.HasSimpleContent();
						return result;
					case 17:
					{
						object[] elements = xML.InScopeNamespaces();
						result = cx.NewArray(scope, elements);
						return result;
					}
					case 18:
						result = xML.InsertChildAfter(base.GetArgSafe(args, 0), base.GetArgSafe(args, 1));
						return result;
					case 19:
						result = xML.InsertChildBefore(base.GetArgSafe(args, 0), base.GetArgSafe(args, 1));
						return result;
					case 20:
						result = xML.Length();
						return result;
					case 21:
						result = xML.LocalName();
						return result;
					case 22:
						result = xML.Name();
						return result;
					case 23:
						result = xML.Namespace(base.GetArgSafe(args, 0));
						return result;
					case 24:
						result = cx.NewArray(scope, xML.NamespaceDeclarations());
						return result;
					case 25:
						result = xML.NodeKind();
						return result;
					case 26:
						xML.Normalize();
						result = Undefined.Value;
						return result;
					case 27:
						result = xML.Parent();
						return result;
					case 28:
					{
						XMLName xMLName = (args.Length > 0) ? XMLName.Parse(this.lib, cx, args[0]) : XMLName.FormStar();
						result = xML.ProcessingInstructions(xMLName);
						return result;
					}
					case 29:
						result = xML.PrependChild(base.GetArgSafe(args, 0));
						return result;
					case 30:
						result = xML.PropertyIsEnumerable(base.GetArgSafe(args, 0));
						return result;
					case 31:
					{
						Namespace @namespace = EcmaScript.NET.Types.E4X.Namespace.Parse(this.lib, cx, base.GetArgSafe(args, 0));
						result = xML.RemoveNamespace(@namespace);
						return result;
					}
					case 32:
					{
						XMLName xMLName = XMLName.Parse(this.lib, cx, base.GetArgSafe(args, 0));
						object argSafe = base.GetArgSafe(args, 1);
						if (xMLName == null)
						{
							long num = ScriptRuntime.lastUint32Result(cx);
							result = xML.Replace(num, argSafe);
							return result;
						}
						result = xML.Replace(xMLName, argSafe);
						return result;
					}
					case 33:
						result = xML.SetChildren(base.GetArgSafe(args, 0));
						return result;
					case 34:
					{
						object obj = base.GetArgSafe(args, 0);
						string localName;
						if (obj is QName)
						{
							localName = ((QName)obj).LocalName;
						}
						else
						{
							localName = ScriptConvert.ToString(obj);
						}
						xML.SetLocalName(localName);
						result = Undefined.Value;
						return result;
					}
					case 35:
					{
						object obj = (args.Length != 0) ? args[0] : Undefined.Value;
						QName qName;
						if (obj is QName)
						{
							qName = (QName)obj;
							if (qName.Uri == null)
							{
								qName = QName.Parse(this.lib, cx, qName.LocalName);
							}
							else
							{
								qName = QName.Parse(this.lib, cx, qName);
							}
						}
						else
						{
							qName = QName.Parse(this.lib, cx, obj);
						}
						xML.SetName(qName);
						result = Undefined.Value;
						return result;
					}
					case 36:
					{
						Namespace @namespace = EcmaScript.NET.Types.E4X.Namespace.Parse(this.lib, cx, base.GetArgSafe(args, 0));
						xML.SetNamespace(@namespace);
						result = Undefined.Value;
						return result;
					}
					case 37:
						result = xML.Text();
						return result;
					case 38:
						result = xML.ToString();
						return result;
					case 39:
						result = xML.ToXMLString();
						return result;
					case 40:
						result = xML;
						return result;
					}
					throw new ArgumentException(Convert.ToString(methodId));
				}
			}
			return result;
		}
		internal XMLObject AddNamespace(object value)
		{
			if (value == null || value is Undefined)
			{
				throw ScriptRuntime.TypeErrorById("value may be not be null or undefined.", new string[0]);
			}
			Namespace @namespace = value as Namespace;
			if (@namespace == null)
			{
				throw ScriptRuntime.TypeErrorById("value may be a Namespace, not {0}", new string[]
				{
					ScriptRuntime.Typeof(value)
				});
			}
			throw new NotImplementedException();
		}
		private object JsConstructor(Context cx, bool inNewExpr, object[] args)
		{
			object result;
			if (args.Length == 0)
			{
				result = XML.CreateFromJS(this.lib, string.Empty);
			}
			else
			{
				object obj = args[0];
				if (!inNewExpr && obj is XML)
				{
					result = obj;
				}
				else
				{
					result = XML.CreateFromJS(this.lib, obj);
				}
			}
			return result;
		}
		internal static XML CreateFromJS(XMLLib lib, object inputObject)
		{
			string text;
			if (inputObject == null || inputObject == Undefined.Value)
			{
				text = "";
			}
			else
			{
				if (inputObject is XMLObject)
				{
					text = ((XMLObject)inputObject).ToXMLString();
				}
				else
				{
					text = ScriptConvert.ToString(inputObject);
				}
			}
			if (text.Trim().StartsWith("<>"))
			{
				throw ScriptRuntime.TypeError("Invalid use of XML object anonymous tags <></>.");
			}
			XmlDocument xmlDocument = null;
			XML result;
			if (text.IndexOf("<") == -1)
			{
				xmlDocument = new XmlDocument();
				XmlNode xmlNode = xmlDocument.CreateTextNode(text);
				result = new XML(lib, xmlNode);
			}
			else
			{
				xmlDocument = new XmlDocument();
				try
				{
					using (StringReader stringReader = new StringReader(text))
					{
						XmlTextReader reader = new XML.XMLTextReader(lib, stringReader);
						xmlDocument.Load(reader);
					}
				}
				catch (XmlException ex)
				{
					throw ScriptRuntime.TypeError(ex.Message);
				}
				result = new XML(lib, xmlDocument);
			}
			return result;
		}
		protected internal override IScriptable GetExtraMethodSource(Context cx)
		{
			IScriptable result;
			if (this.HasSimpleContent())
			{
				result = ScriptConvert.ToObjectOrNull(cx, this.ToString());
			}
			else
			{
				result = null;
			}
			return result;
		}
		internal static XML CreateEmptyXml(XMLLib impl)
		{
			return new XML(impl);
		}
		protected internal override string ToXMLString()
		{
			string result;
			using (StringWriter stringWriter = new StringWriter())
			{
				XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
				this.UnderlyingNode.WriteTo(xmlTextWriter);
				xmlTextWriter.Flush();
				xmlTextWriter.Close();
				result = stringWriter.ToString();
			}
			return result;
		}
		internal XMLObject ValueOf()
		{
			return this;
		}
		internal bool HasSimpleContent()
		{
			return this.UnderlyingNode is XmlAttribute || this.UnderlyingNode is XmlText || this.UnderlyingNode.SelectNodes("*").Count < 1;
		}
		internal bool HasComplexContent()
		{
			return !this.HasSimpleContent();
		}
		internal object[] InScopeNamespaces()
		{
			return this.NamespaceDeclarations();
		}
		internal int Length()
		{
			return 1;
		}
		public override object GetDefaultValue(Type typeHint)
		{
			object result;
			if (this.HasSimpleContent())
			{
				result = this.UnderlyingNode.InnerText;
			}
			else
			{
				result = this.ToSource(0);
			}
			return result;
		}
		internal string ToSource(int indent)
		{
			return this.ToXMLString();
		}
		internal object Namespace(object value)
		{
			string prefix = ScriptConvert.ToString(value);
			string namespaceOfPrefix = this.UnderlyingNode.GetNamespaceOfPrefix(prefix);
			object result;
			if (namespaceOfPrefix == null)
			{
				result = Undefined.Value;
			}
			else
			{
				result = new Namespace(this.lib, prefix, namespaceOfPrefix);
			}
			return result;
		}
		internal object Parent()
		{
			object result;
			if (this.UnderlyingNode.ParentNode == null)
			{
				result = null;
			}
			else
			{
				result = new XML(this.lib, this.UnderlyingNode.ParentNode);
			}
			return result;
		}
		internal int ChildIndex()
		{
			int result;
			if (this.UnderlyingNode.ParentNode == null)
			{
				result = -1;
			}
			else
			{
				for (int i = 0; i < this.UnderlyingNode.ParentNode.ChildNodes.Count; i++)
				{
					if (this.UnderlyingNode.ParentNode.ChildNodes[i] == this.UnderlyingNode)
					{
						result = i;
						return result;
					}
				}
				result = -1;
			}
			return result;
		}
		internal XMLList Children()
		{
			XMLList xMLList = new XMLList(this.lib);
			foreach (XmlNode xmlNode in this.UnderlyingNode.ChildNodes)
			{
				xMLList.Add(new XML(this.lib, xmlNode));
			}
			return xMLList;
		}
		private XMLList Comments()
		{
			XMLList xMLList = new XMLList(this.lib);
			foreach (XmlNode xmlNode in this.UnderlyingNode.ChildNodes)
			{
				if (xmlNode is XmlComment)
				{
					xMLList.Add(new XML(this.lib, xmlNode));
				}
			}
			return xMLList;
		}
		internal XMLList ProcessingInstructions(XMLName xmlName)
		{
			XMLList xMLList = new XMLList(this.lib);
			foreach (XmlNode xmlNode in this.UnderlyingNode.ChildNodes)
			{
				if (xmlNode is XmlProcessingInstruction)
				{
					if (xmlName == null || xmlName.Matches(xmlNode))
					{
						xMLList.Add(new XML(this.lib, xmlNode));
					}
				}
			}
			return xMLList;
		}
		private bool Contains(object value)
		{
			bool result;
			foreach (XML xML in this.Children())
			{
				if (xML.EquivalentXml(value))
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
		protected internal override bool EquivalentXml(object value)
		{
			object defaultValue = this.GetDefaultValue(typeof(string));
			bool result;
			if (value is XML)
			{
				XML xML = (XML)value;
				result = defaultValue.Equals(xML.GetDefaultValue(typeof(string)));
			}
			else
			{
				if (value is XMLList)
				{
					XMLList xMLList = (XMLList)value;
					if (xMLList.Length() == 1)
					{
						result = this.EquivalentXml(xMLList[0]);
						return result;
					}
				}
				result = (this.HasSimpleContent() && defaultValue.Equals(value.ToString()));
			}
			return result;
		}
		public override bool Equals(object obj)
		{
			XML xML = obj as XML;
			return xML != null && ((xML.UnderlyingNode == null && this.UnderlyingNode == null) || (xML.UnderlyingNode != null && this.UnderlyingNode != null && xML.UnderlyingNode.Equals(this.UnderlyingNode)));
		}
		public override int GetHashCode()
		{
			int hashCode;
			if (this.UnderlyingNode == null)
			{
				hashCode = base.GetHashCode();
			}
			else
			{
				hashCode = this.UnderlyingNode.GetHashCode();
			}
			return hashCode;
		}
		internal XMLList Attribute(XMLName xmlName)
		{
			XMLList xMLList = new XMLList(this.lib);
			this.MatchAttributes(xMLList, xmlName, this.UnderlyingNode, false);
			return xMLList;
		}
		internal void MatchAttributes(XMLList list, XMLName xmlName, XmlNode parent, bool recursive)
		{
			if (parent is XmlDocument)
			{
				parent = ((XmlDocument)parent).DocumentElement;
			}
			if (parent is XmlElement)
			{
				foreach (XmlAttribute node in parent.Attributes)
				{
					if (xmlName == null || xmlName.Matches(node))
					{
						list.Add(new XML(this.lib, node));
					}
				}
				if (recursive)
				{
					foreach (XmlNode parent2 in parent.ChildNodes)
					{
						this.MatchAttributes(list, xmlName, parent2, recursive);
					}
				}
			}
		}
		internal XMLList Attributes()
		{
			XMLList xMLList = new XMLList(this.lib);
			this.MatchAttributes(xMLList, null, this.UnderlyingNode, false);
			return xMLList;
		}
		internal XMLList Descendants(XMLName xmlName)
		{
			XMLList xMLList = new XMLList(this.lib);
			if (xmlName.IsAttributeName)
			{
				this.MatchAttributes(xMLList, xmlName, this.UnderlyingNode, true);
			}
			else
			{
				this.MatchChildren(xMLList, xmlName, this.UnderlyingNode, true);
			}
			return xMLList;
		}
		private XMLList Elements(XMLName xmlName)
		{
			XMLList result;
			if (xmlName == null)
			{
				result = this.Children();
			}
			else
			{
				XMLList xMLList = new XMLList(this.lib);
				this.MatchChildren(xMLList, xmlName, this.UnderlyingNode, false);
				result = xMLList;
			}
			return result;
		}
		public void MatchChildren(XMLList list, XMLName xmlName, XmlNode parent, bool recursive)
		{
			foreach (XmlNode xmlNode in parent.ChildNodes)
			{
				if (xmlName.Matches(xmlNode))
				{
					list.Add(new XML(this.lib, xmlNode));
				}
				if (recursive)
				{
					this.MatchChildren(list, xmlName, xmlNode, recursive);
				}
			}
		}
		public override object[] GetIds()
		{
			object[] result;
			if (this.isPrototype)
			{
				result = new object[0];
			}
			else
			{
				result = new object[]
				{
					0
				};
			}
			return result;
		}
		internal QName Name()
		{
			QName result;
			if (this.UnderlyingNode is XmlText)
			{
				result = null;
			}
			else
			{
				result = new QName(this.lib, this.UnderlyingNode.NamespaceURI, this.UnderlyingNode.LocalName, this.UnderlyingNode.Prefix);
			}
			return result;
		}
		internal string LocalName()
		{
			return this.UnderlyingNode.LocalName;
		}
		internal string NodeKind()
		{
			string result;
			if (this.UnderlyingNode is XmlElement)
			{
				result = "element";
			}
			else
			{
				if (this.UnderlyingNode is XmlAttribute)
				{
					result = "attribute";
				}
				else
				{
					if (this.UnderlyingNode is XmlText)
					{
						result = "text";
					}
					else
					{
						if (this.UnderlyingNode is XmlComment)
						{
							result = "comment";
						}
						else
						{
							if (this.UnderlyingNode is XmlProcessingInstruction)
							{
								result = "processing-instruction";
							}
							else
							{
								if (this.UnderlyingNode is XmlDocument)
								{
									result = "element";
								}
								else
								{
									result = "text";
								}
							}
						}
					}
				}
			}
			return result;
		}
		internal XMLList Child(long index)
		{
			XMLList xMLList = new XMLList(this.lib);
			if (index > 0L || index < (long)this.UnderlyingNode.ChildNodes.Count)
			{
				xMLList.Add(new XML(this.lib, this.UnderlyingNode.ChildNodes[(int)index]));
			}
			return xMLList;
		}
		internal XMLList Child(XMLName xmlName)
		{
			XMLList xMLList = new XMLList(this.lib);
			this.MatchChildren(xMLList, xmlName, this.UnderlyingNode, false);
			return xMLList;
		}
		protected internal override object GetXMLProperty(XMLName name)
		{
			object result;
			if (this.isPrototype)
			{
				result = base.Get(name.localName, this);
			}
			else
			{
				if (name.IsDescendants)
				{
					result = this.Descendants(name);
				}
				else
				{
					if (name.IsAttributeName)
					{
						result = this.Attribute(name);
					}
					else
					{
						result = this.Child(name);
					}
				}
			}
			return result;
		}
		internal string Text()
		{
			return this.UnderlyingNode.InnerText;
		}
		internal XMLObject Normalize()
		{
			this.UnderlyingNode.Normalize();
			return this;
		}
		internal object Copy()
		{
			return new XML(this.lib, this.UnderlyingNode.Clone());
		}
		internal object[] NamespaceDeclarations()
		{
			ArrayList arrayList = new ArrayList();
			foreach (XmlAttribute xmlAttribute in this.UnderlyingNode.Attributes)
			{
				if (xmlAttribute.LocalName == "xmlns")
				{
					Namespace @namespace = new Namespace(this.lib, xmlAttribute.Prefix, xmlAttribute.InnerText);
					if (!arrayList.Contains(@namespace))
					{
						arrayList.Add(@namespace);
					}
				}
			}
			return arrayList.ToArray();
		}
		internal bool HasOwnProperty(XMLName xmlName)
		{
			bool result;
			if (this.isPrototype)
			{
				result = (this.FindPrototypeId(xmlName.localName) != 0);
			}
			else
			{
				result = (this.GetPropertyList(xmlName).Length() > 0);
			}
			return result;
		}
		protected internal override void PutXMLProperty(XMLName xmlName, object value)
		{
			if (!this.isPrototype)
			{
				if (value == null)
				{
					value = "null";
				}
				else
				{
					if (value is Undefined)
					{
						value = "undefined";
					}
				}
				if (xmlName.IsAttributeName)
				{
					this.SetAttribute(xmlName, value);
				}
				else
				{
					if (value is XMLList)
					{
						XMLList xMLList = value as XMLList;
						XMLList propertyList = this.GetPropertyList(xmlName);
						if (propertyList.Length() == 0)
						{
							foreach (XML xML in xMLList)
							{
								this.AppendChild(xML);
							}
						}
						else
						{
							for (int i = 1; i < propertyList.Length(); i++)
							{
								this.UnderlyingNode.RemoveChild(propertyList[i].UnderlyingNode);
							}
							for (int i = 1; i < xMLList.Length(); i++)
							{
								this.UnderlyingNode.InsertAfter(this.ImportNode(xMLList[i].UnderlyingNode), propertyList[0].UnderlyingNode);
							}
							this.UnderlyingNode.ReplaceChild(this.ImportNode(xMLList[0].UnderlyingNode), propertyList[0].UnderlyingNode);
						}
					}
					else
					{
						XML xML = value as XML;
						if (xML == null)
						{
							xML = XML.CreateFromJS(this.lib, value);
						}
						XMLList propertyList = this.GetPropertyList(xmlName);
						if (propertyList.Length() == 0)
						{
							this.AppendChild(xML);
						}
						else
						{
							for (int i = 1; i < propertyList.Length(); i++)
							{
								this.UnderlyingNode.RemoveChild(propertyList[i].UnderlyingNode);
							}
							if (xML.UnderlyingNode is XmlText)
							{
								propertyList[0].RemoveAllChildren();
								propertyList[0].AppendChild(xML);
							}
							else
							{
								this.UnderlyingNode.ReplaceChild(this.ImportNode(xML.UnderlyingNode), propertyList[0].UnderlyingNode);
							}
						}
					}
				}
			}
		}
		internal void SetAttribute(XMLName xmlName, object value)
		{
			if (xmlName.uri == null && xmlName.localName.Equals("*"))
			{
				throw ScriptRuntime.TypeError("@* assignment not supported.");
			}
			XmlNode documentElement = this.UnderlyingNode;
			if (documentElement is XmlDocument)
			{
				documentElement = ((XmlDocument)documentElement).DocumentElement;
			}
			if (documentElement is XmlElement)
			{
				((XmlElement)documentElement).SetAttribute(xmlName.localName, xmlName.uri, ScriptConvert.ToString(value));
			}
		}
		internal XML AppendChild(object child)
		{
			XML result;
			if (this.underlyingNode is XmlDocument)
			{
				XML xML = new XML(this.lib, ((XmlDocument)this.underlyingNode).DocumentElement);
				xML.AppendChild(child);
				result = this;
			}
			else
			{
				result = this.InsertChildBefore(child, null);
			}
			return result;
		}
		internal XML PrependChild(object child)
		{
			return this.InsertChildAfter(child, null);
		}
		internal bool PropertyIsEnumerable(object p)
		{
			throw new NotImplementedException();
		}
		internal XMLObject RemoveNamespace(object ns)
		{
			throw new NotImplementedException();
		}
		internal XMLObject Replace(object name, object value)
		{
			throw new NotImplementedException();
		}
		internal XMLObject SetChildren(object value)
		{
			if (!(value is XML) && !(value is XMLList))
			{
				throw ScriptRuntime.TypeErrorById("value may be a single XML object or an XMLList, not {0}", new string[]
				{
					ScriptRuntime.Typeof(value)
				});
			}
			foreach (XmlNode xmlNode in this.UnderlyingNode.ChildNodes)
			{
				xmlNode.ParentNode.RemoveChild(xmlNode);
			}
			XML xML = value as XML;
			if (xML != null)
			{
				this.UnderlyingNode.AppendChild(this.ImportNode(xML.UnderlyingNode));
			}
			else
			{
				XMLList xMLList = (XMLList)value;
				foreach (XML xML2 in xMLList)
				{
					this.UnderlyingNode.AppendChild(this.ImportNode(xML2.UnderlyingNode));
				}
			}
			return this;
		}
		internal void SetLocalName(object name)
		{
			if (name == null || name is Undefined)
			{
				throw ScriptRuntime.TypeError("name may not be null or undefined");
			}
			XmlQualifiedName xmlQualifiedName = null;
			try
			{
				xmlQualifiedName = new XmlQualifiedName(ScriptConvert.ToString(name));
			}
			catch (XmlException ex)
			{
				throw ScriptRuntime.TypeErrorById("invalid name: {0}", new string[]
				{
					ex.Message
				});
			}
			this.UnderlyingNode = this.Rename(this.UnderlyingNode, this.UnderlyingNode.Prefix, xmlQualifiedName.Name, this.UnderlyingNode.NamespaceURI);
		}
		internal void SetName(object name)
		{
			if (name == null || name is Undefined)
			{
				throw ScriptRuntime.TypeError("name may not be null or undefined");
			}
			string prefix = this.UnderlyingNode.Prefix;
			string namespaceURI = this.UnderlyingNode.NamespaceURI;
			string localName = this.UnderlyingNode.LocalName;
			if (name is QName)
			{
				QName qName = (QName)name;
				this.UnderlyingNode = this.Rename(this.UnderlyingNode, qName.Prefix, qName.LocalName, qName.Uri);
			}
			else
			{
				this.SetLocalName(name);
			}
		}
		internal void SetNamespace(object ns)
		{
			if (ns == null || ns is Undefined)
			{
				throw ScriptRuntime.TypeError("name may not be null or undefined");
			}
			if (!(ns is Namespace))
			{
				throw ScriptRuntime.TypeErrorById("name must be typeof Namespace, not {0}", new string[]
				{
					ScriptRuntime.Typeof(ns)
				});
			}
			Namespace @namespace = (Namespace)ns;
			this.UnderlyingNode = this.Rename(this.UnderlyingNode, @namespace.Prefix, this.UnderlyingNode.LocalName, @namespace.Prefix);
		}
		internal XML InsertChildAfter(object newChild, object refChild)
		{
			XML xML = this.lib.ToXML(refChild);
			XML xML2 = this.lib.ToXML(newChild);
			if (xML2 != null)
			{
				XmlNode newChild2 = xML2.UnderlyingNode;
				if (this.UnderlyingNode.OwnerDocument != xML2.UnderlyingNode.OwnerDocument)
				{
					xML2.UnderlyingNode = this.UnderlyingNode.OwnerDocument.ImportNode(xML2.UnderlyingNode, true);
				}
				newChild2 = xML2.UnderlyingNode;
				XmlNode refChild2 = null;
				if (xML != null)
				{
					if (xML.UnderlyingNode.OwnerDocument == this.UnderlyingNode.OwnerDocument)
					{
						if (this.UnderlyingNode.OwnerDocument != xML.UnderlyingNode.OwnerDocument)
						{
							xML.UnderlyingNode = this.UnderlyingNode.OwnerDocument.ImportNode(xML.UnderlyingNode, true);
						}
						refChild2 = xML.UnderlyingNode;
					}
				}
				this.UnderlyingNode.InsertAfter(newChild2, refChild2);
			}
			return this;
		}
		internal XML InsertChildBefore(object newChild, object refChild)
		{
			XML xML = this.lib.ToXML(refChild);
			XML xML2 = this.lib.ToXML(newChild);
			if (xML2 != null)
			{
				XmlNode xmlNode = xML2.UnderlyingNode;
				if (xML2.UnderlyingNode.OwnerDocument == this.UnderlyingNode.OwnerDocument)
				{
					if (this.UnderlyingNode.OwnerDocument != xML2.UnderlyingNode.OwnerDocument)
					{
						xML2.UnderlyingNode = this.UnderlyingNode.OwnerDocument.ImportNode(xML2.UnderlyingNode, true);
					}
					xmlNode = xML2.UnderlyingNode;
				}
				XmlNode refChild2 = null;
				if (xML != null)
				{
					refChild2 = xML.UnderlyingNode;
				}
				this.UnderlyingNode.InsertBefore(this.ImportNode(xML2.UnderlyingNode), refChild2);
			}
			return this;
		}
		internal XMLList GetPropertyList(XMLName name)
		{
			XMLList result;
			if (name.IsDescendants)
			{
				result = this.Descendants(name);
			}
			else
			{
				if (name.IsAttributeName)
				{
					result = this.Attribute(name);
				}
				else
				{
					result = this.Child(name);
				}
			}
			return result;
		}
		public override string ToString()
		{
			return (string)this.GetDefaultValue(typeof(string));
		}
		private XmlNode ImportNode(XmlNode node)
		{
			XmlNode result;
			if (node.OwnerDocument != this.UnderlyingNode.OwnerDocument)
			{
				if (node is XmlDocument)
				{
					node = node.SelectSingleNode("*");
				}
				result = this.UnderlyingNode.OwnerDocument.ImportNode(node, true);
			}
			else
			{
				result = node;
			}
			return result;
		}
		private XmlNode Rename(XmlNode node, string prefix, string localName, string namespaceUri)
		{
			if (node is XmlElement)
			{
				return this.Rename((XmlElement)node, prefix, localName, namespaceUri);
			}
			throw ScriptRuntime.TypeErrorById("Renaming of xml node type {0} is not supported.", new string[]
			{
				node.NodeType.ToString()
			});
		}
		private XmlNode Rename(XmlElement source, string prefix, string localName, string namespaceUri)
		{
			XmlElement xmlElement = source.OwnerDocument.CreateElement(prefix, localName, namespaceUri);
			foreach (XmlAttribute node in source.Attributes)
			{
				xmlElement.Attributes.Append(node);
			}
			foreach (XmlNode newChild in source.ChildNodes)
			{
				xmlElement.AppendChild(newChild);
			}
			source.ParentNode.ReplaceChild(xmlElement, source);
			return xmlElement;
		}
		private void RemoveAllChildren()
		{
			foreach (XmlNode oldChild in this.underlyingNode.ChildNodes)
			{
				this.underlyingNode.RemoveChild(oldChild);
			}
		}
	}
}
