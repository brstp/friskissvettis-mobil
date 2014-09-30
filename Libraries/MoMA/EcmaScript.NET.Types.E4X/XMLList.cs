using System;
using System.Collections;
using System.Text;
namespace EcmaScript.NET.Types.E4X
{
	internal class XMLList : XMLObject, IFunction, IScriptable, ICallable, IEnumerable
	{
		private const int Id_constructor = 1;
		private const int Id_attribute = 2;
		private const int Id_attributes = 3;
		private const int Id_child = 4;
		private const int Id_children = 5;
		private const int Id_comments = 6;
		private const int Id_contains = 7;
		private const int Id_copy = 8;
		private const int Id_descendants = 9;
		private const int Id_elements = 10;
		private const int Id_hasOwnProperty = 11;
		private const int Id_hasComplexContent = 12;
		private const int Id_hasSimpleContent = 13;
		private const int Id_length = 14;
		private const int Id_normalize = 15;
		private const int Id_parent = 16;
		private const int Id_processingInstructions = 17;
		private const int Id_propertyIsEnumerable = 18;
		private const int Id_text = 19;
		private const int Id_toString = 20;
		private const int Id_toXMLString = 21;
		private const int Id_valueOf = 22;
		private const int Id_domNode = 23;
		private const int Id_domNodeList = 24;
		private const int Id_xpath = 25;
		private const int Id_addNamespace = 26;
		private const int Id_appendChild = 27;
		private const int Id_childIndex = 28;
		private const int Id_inScopeNamespaces = 29;
		private const int Id_insertChildAfter = 30;
		private const int Id_insertChildBefore = 31;
		private const int Id_localName = 32;
		private const int Id_name = 33;
		private const int Id_namespace = 34;
		private const int Id_namespaceDeclarations = 35;
		private const int Id_nodeKind = 36;
		private const int Id_prependChild = 37;
		private const int Id_removeNamespace = 38;
		private const int Id_replace = 39;
		private const int Id_setChildren = 40;
		private const int Id_setLocalName = 41;
		private const int Id_setName = 42;
		private const int Id_setNamespace = 43;
		private const int MAX_PROTOTYPE_ID = 43;
		private static readonly object XMLOBJECT_TAG = new object();
		internal ArrayList m_Nodes = new ArrayList();
		public override string ClassName
		{
			get
			{
				return "XMLList";
			}
		}
		public XML this[int index]
		{
			get
			{
				return (XML)this.m_Nodes[index];
			}
		}
		public XMLList(XMLLib lib) : base(lib, lib.xmlListPrototype)
		{
			this.lib = lib;
		}
		public XMLList(XMLLib lib, object inputObject) : this(lib)
		{
			if (inputObject != null && !(inputObject is Undefined))
			{
				if (inputObject is XML)
				{
					XML node = (XML)inputObject;
					this.Add(node);
				}
				else
				{
					if (inputObject is XMLList)
					{
						XMLList list = (XMLList)inputObject;
						this.AddRange(list);
					}
					else
					{
						string text = ScriptConvert.ToString(inputObject).Trim();
						if (!text.StartsWith("<>"))
						{
							text = "<>" + text + "</>";
						}
						text = "<fragment>" + text.Substring(2);
						if (!text.EndsWith("</>"))
						{
							throw ScriptRuntime.TypeError("XML with anonymous tag missing end anonymous tag");
						}
						text = text.Substring(0, text.Length - 3) + "</fragment>";
						XML xML = XML.CreateFromJS(lib, text);
						XMLList list2 = xML.Children();
						this.AddRange(list2);
					}
				}
			}
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
					result = 8;
				}
				else
				{
					if (num == 110)
					{
						text = "name";
						result = 33;
					}
					else
					{
						if (num == 116)
						{
							text = "text";
							result = 19;
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
					result = 4;
				}
				else
				{
					if (num == 120)
					{
						text = "xpath";
						result = 25;
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
					result = 14;
				}
				else
				{
					if (num == 112)
					{
						text = "parent";
						result = 16;
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
						result = 39;
						break;
					case 's':
						text = "setName";
						result = 42;
						break;
					case 'v':
						text = "valueOf";
						result = 22;
						break;
					}
				}
				else
				{
					text = "domNode";
					result = 23;
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
							result = 36;
							break;
						case 'e':
							text = "elements";
							result = 10;
							break;
						}
					}
					else
					{
						text = "toString";
						result = 20;
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
							result = 6;
							break;
						case 'n':
							text = "contains";
							result = 7;
							break;
						}
					}
					else
					{
						text = "children";
						result = 5;
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
							result = 15;
							break;
						case 't':
							text = "attribute";
							result = 2;
							break;
						}
					}
					else
					{
						text = "namespace";
						result = 34;
					}
				}
				else
				{
					text = "localName";
					result = 32;
				}
				break;
			}
			case 10:
			{
				int num = (int)s[0];
				if (num == 97)
				{
					text = "attributes";
					result = 3;
				}
				else
				{
					if (num == 99)
					{
						text = "childIndex";
						result = 28;
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
						result = 24;
						break;
					case 'n':
						text = "constructor";
						result = 1;
						break;
					case 'p':
						text = "appendChild";
						result = 27;
						break;
					case 's':
						text = "descendants";
						result = 9;
						break;
					case 't':
						text = "setChildren";
						result = 40;
						break;
					}
				}
				else
				{
					text = "toXMLString";
					result = 21;
				}
				break;
			}
			case 12:
			{
				int num = (int)s[0];
				if (num == 97)
				{
					text = "addNamespace";
					result = 26;
				}
				else
				{
					if (num == 112)
					{
						text = "prependChild";
						result = 37;
					}
					else
					{
						if (num == 115)
						{
							num = (int)s[3];
							if (num == 76)
							{
								text = "setLocalName";
								result = 41;
							}
							else
							{
								if (num == 78)
								{
									text = "setNamespace";
									result = 43;
								}
							}
						}
					}
				}
				break;
			}
			case 14:
				text = "hasOwnProperty";
				result = 11;
				break;
			case 15:
				text = "removeNamespace";
				result = 38;
				break;
			case 16:
			{
				int num = (int)s[0];
				if (num == 104)
				{
					text = "hasSimpleContent";
					result = 13;
				}
				else
				{
					if (num == 105)
					{
						text = "insertChildAfter";
						result = 30;
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
					result = 12;
				}
				else
				{
					if (num == 99)
					{
						text = "inScopeNamespaces";
						result = 29;
					}
					else
					{
						if (num == 101)
						{
							text = "insertChildBefore";
							result = 31;
						}
					}
				}
				break;
			}
			case 20:
				text = "propertyIsEnumerable";
				result = 18;
				break;
			case 21:
				text = "namespaceDeclarations";
				result = 35;
				break;
			case 22:
				text = "processingInstructions";
				result = 17;
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
				arity = 1;
				name = "constructor";
				goto IL_2EA;
			case 2:
				arity = 1;
				name = "attribute";
				goto IL_2EA;
			case 3:
				arity = 0;
				name = "attributes";
				goto IL_2EA;
			case 4:
				arity = 1;
				name = "child";
				goto IL_2EA;
			case 5:
				arity = 0;
				name = "children";
				goto IL_2EA;
			case 6:
				arity = 0;
				name = "comments";
				goto IL_2EA;
			case 7:
				arity = 1;
				name = "contains";
				goto IL_2EA;
			case 8:
				arity = 0;
				name = "copy";
				goto IL_2EA;
			case 9:
				arity = 1;
				name = "descendants";
				goto IL_2EA;
			case 11:
				arity = 1;
				name = "hasOwnProperty";
				goto IL_2EA;
			case 12:
				arity = 0;
				name = "hasComplexContent";
				goto IL_2EA;
			case 13:
				arity = 0;
				name = "hasSimpleContent";
				goto IL_2EA;
			case 14:
				arity = 0;
				name = "length";
				goto IL_2EA;
			case 15:
				arity = 0;
				name = "normalize";
				goto IL_2EA;
			case 16:
				arity = 0;
				name = "parent";
				goto IL_2EA;
			case 17:
				arity = 1;
				name = "processingInstructions";
				goto IL_2EA;
			case 18:
				arity = 1;
				name = "propertyIsEnumerable";
				goto IL_2EA;
			case 19:
				arity = 0;
				name = "text";
				goto IL_2EA;
			case 20:
				arity = 0;
				name = "toString";
				goto IL_2EA;
			case 21:
				arity = 1;
				name = "toXMLString";
				goto IL_2EA;
			case 22:
				arity = 0;
				name = "valueOf";
				goto IL_2EA;
			case 23:
				arity = 0;
				name = "domNode";
				goto IL_2EA;
			case 24:
				arity = 0;
				name = "domNodeList";
				goto IL_2EA;
			case 25:
				arity = 0;
				name = "xpath";
				goto IL_2EA;
			case 26:
				arity = 1;
				name = "addNamespace";
				goto IL_2EA;
			case 27:
				arity = 1;
				name = "appendChild";
				goto IL_2EA;
			case 28:
				arity = 0;
				name = "childIndex";
				goto IL_2EA;
			case 29:
				arity = 0;
				name = "inScopeNamespaces";
				goto IL_2EA;
			case 30:
				arity = 2;
				name = "insertChildAfter";
				goto IL_2EA;
			case 31:
				arity = 2;
				name = "insertChildBefore";
				goto IL_2EA;
			case 32:
				arity = 0;
				name = "localName";
				goto IL_2EA;
			case 33:
				arity = 0;
				name = "name";
				goto IL_2EA;
			case 34:
				arity = 1;
				name = "namespace";
				goto IL_2EA;
			case 35:
				arity = 0;
				name = "namespaceDeclarations";
				goto IL_2EA;
			case 36:
				arity = 0;
				name = "nodeKind";
				goto IL_2EA;
			case 37:
				arity = 1;
				name = "prependChild";
				goto IL_2EA;
			case 38:
				arity = 1;
				name = "removeNamespace";
				goto IL_2EA;
			case 39:
				arity = 2;
				name = "replace";
				goto IL_2EA;
			case 40:
				arity = 1;
				name = "setChildren";
				goto IL_2EA;
			case 41:
				arity = 1;
				name = "setLocalName";
				goto IL_2EA;
			case 42:
				arity = 1;
				name = "setName";
				goto IL_2EA;
			case 43:
				arity = 1;
				name = "setNamespace";
				goto IL_2EA;
			}
			throw new ArgumentException(Convert.ToString(id));
			IL_2EA:
			base.InitPrototypeMethod(XMLList.XMLOBJECT_TAG, id, name, arity);
		}
		public override object ExecIdCall(IdFunctionObject f, Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			object result;
			if (!f.HasTag(XMLList.XMLOBJECT_TAG))
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
					if (!(thisObj is XMLList))
					{
						throw IdScriptableObject.IncompatibleCallError(f);
					}
					XMLList xMLList = (XMLList)thisObj;
					switch (methodId)
					{
					case 2:
					{
						XMLName xMLName = XMLName.Parse(this.lib, cx, base.GetArgSafe(args, 0));
						result = xMLList.Attribute(xMLName);
						return result;
					}
					case 3:
						result = xMLList.Attributes();
						return result;
					case 4:
					{
						XMLName xMLName = XMLName.Parse(this.lib, cx, base.GetArgSafe(args, 0));
						if (xMLName == null)
						{
							long index = ScriptRuntime.lastUint32Result(cx);
							result = xMLList.Child(index);
							return result;
						}
						result = xMLList.Child(xMLName);
						return result;
					}
					case 5:
						result = xMLList.Children();
						return result;
					case 7:
						result = xMLList.Contains(base.GetArgSafe(args, 0));
						return result;
					case 8:
						result = xMLList.Copy();
						return result;
					case 9:
					{
						XMLName xMLName = (args.Length == 0) ? XMLName.FormStar() : XMLName.Parse(this.lib, cx, base.GetArgSafe(args, 0));
						result = xMLList.Descendants(xMLName);
						return result;
					}
					case 11:
					{
						XMLName xMLName = XMLName.Parse(this.lib, cx, base.GetArgSafe(args, 0));
						result = xMLList.HasOwnProperty(xMLName);
						return result;
					}
					case 12:
						result = xMLList.HasComplexContent();
						return result;
					case 13:
						result = xMLList.HasSimpleContent();
						return result;
					case 14:
						result = xMLList.Length();
						return result;
					case 15:
						xMLList.Normalize();
						result = Undefined.Value;
						return result;
					case 16:
						result = xMLList.Parent();
						return result;
					case 17:
					{
						XMLName xMLName = (args.Length > 0) ? XMLName.Parse(this.lib, cx, args[0]) : XMLName.FormStar();
						result = xMLList.ProcessingInstructions(xMLName);
						return result;
					}
					case 18:
						result = xMLList.PropertyIsEnumerable(base.GetArgSafe(args, 0));
						return result;
					case 19:
						result = xMLList.Text();
						return result;
					case 20:
						result = xMLList.ToString();
						return result;
					case 21:
						result = xMLList.ToXMLString();
						return result;
					case 22:
						result = xMLList;
						return result;
					case 26:
						result = xMLList.DelegateTo("addNamespace").AddNamespace(base.GetArgSafe(args, 0));
						return result;
					case 27:
						result = xMLList.DelegateTo("appendChild").AppendChild(base.GetArgSafe(args, 0));
						return result;
					case 28:
						result = xMLList.DelegateTo("childIndex").ChildIndex();
						return result;
					case 29:
						result = xMLList.DelegateTo("inScopeNamespaces").InScopeNamespaces();
						return result;
					case 30:
						result = xMLList.DelegateTo("insertChildAfter").InsertChildAfter(base.GetArgSafe(args, 0), base.GetArgSafe(args, 1));
						return result;
					case 31:
						result = xMLList.DelegateTo("insertChildBefore").InsertChildBefore(base.GetArgSafe(args, 0), base.GetArgSafe(args, 1));
						return result;
					case 32:
						result = xMLList.DelegateTo("localName").LocalName();
						return result;
					case 33:
						result = xMLList.DelegateTo("name").Name();
						return result;
					case 34:
						result = xMLList.DelegateTo("namespace").Namespace(base.GetArgSafe(args, 0));
						return result;
					case 35:
						result = xMLList.DelegateTo("namespaceDeclarations").NamespaceDeclarations();
						return result;
					case 36:
						result = xMLList.DelegateTo("nodeKind").NodeKind();
						return result;
					case 37:
						result = xMLList.DelegateTo("prependChild").PrependChild(base.GetArgSafe(args, 0));
						return result;
					case 38:
						result = xMLList.DelegateTo("removeNamespace").RemoveNamespace(base.GetArgSafe(args, 0));
						return result;
					case 39:
						result = xMLList.DelegateTo("replace").Replace(base.GetArgSafe(args, 0), base.GetArgSafe(args, 1));
						return result;
					case 40:
						result = xMLList.DelegateTo("setChildren").SetChildren(base.GetArgSafe(args, 0));
						return result;
					case 41:
						xMLList.DelegateTo("setLocalName").SetLocalName(base.GetArgSafe(args, 0));
						result = Undefined.Value;
						return result;
					case 42:
						xMLList.DelegateTo("setName").SetName(base.GetArgSafe(args, 0));
						result = Undefined.Value;
						return result;
					case 43:
						xMLList.DelegateTo("setNamespace").SetNamespace(base.GetArgSafe(args, 0));
						result = Undefined.Value;
						return result;
					}
					throw new ArgumentException(Convert.ToString(methodId));
				}
			}
			return result;
		}
		private XML DelegateTo(string methodName)
		{
			if (this.Length() != 1)
			{
				throw ScriptRuntime.TypeError("The " + methodName + " method works only on lists containing one item");
			}
			return this[0];
		}
		internal void Add(XML node)
		{
			this.m_Nodes.Add(node);
		}
		internal void AddRange(XMLList list)
		{
			foreach (XML node in list.m_Nodes)
			{
				this.Add(node);
			}
		}
		internal int Length()
		{
			return this.m_Nodes.Count;
		}
		public object Call(Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			throw new Exception("The method or operation is not implemented.");
		}
		public IScriptable Construct(Context cx, IScriptable scope, object[] args)
		{
			throw new Exception("The method or operation is not implemented.");
		}
		private object JsConstructor(Context cx, bool inNewExpr, object[] args)
		{
			object result;
			if (args.Length == 0)
			{
				result = new XMLList(this.lib);
			}
			else
			{
				object obj = args[0];
				if (!inNewExpr && obj is XMLList)
				{
					result = obj;
				}
				else
				{
					result = new XMLList(this.lib, obj);
				}
			}
			return result;
		}
		protected internal override string ToXMLString()
		{
			return this.ToSource(0);
		}
		private bool HasSimpleContent()
		{
			return this.Length() == 1;
		}
		private bool HasComplexContent()
		{
			return !this.HasSimpleContent();
		}
		protected internal override IScriptable GetExtraMethodSource(Context cx)
		{
			IScriptable result;
			if (this.HasSimpleContent())
			{
				result = (this.m_Nodes[0] as XML).GetExtraMethodSource(cx);
			}
			else
			{
				result = null;
			}
			return result;
		}
		private string ToSource(int indent)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (XML xML in this.m_Nodes)
			{
				stringBuilder.Append(xML.ToSource(indent));
			}
			return stringBuilder.ToString();
		}
		public override bool Has(int index, IScriptable start)
		{
			return index >= 0 && index < this.Length();
		}
		public override object Get(int index, IScriptable start)
		{
			object result;
			if (!this.Has(index, start))
			{
				result = base.Get(index, start);
			}
			else
			{
				result = this.m_Nodes[index];
			}
			return result;
		}
		private string NodeKind()
		{
			if (this.Length() == 1)
			{
				return (this.m_Nodes[0] as XML).NodeKind();
			}
			throw ScriptRuntime.TypeError("The nodeKind method works only on lists containing one item");
		}
		private object Namespace(string prefix)
		{
			if (this.Length() == 1)
			{
				return (this.m_Nodes[0] as XML).Namespace(prefix);
			}
			throw ScriptRuntime.TypeError("The namespace method works only on lists containing one item");
		}
		private object Parent()
		{
			object result;
			if (this.Length() == 0)
			{
				result = Undefined.Value;
			}
			else
			{
				object obj = null;
				foreach (XML xML in this.m_Nodes)
				{
					if (obj == null)
					{
						obj = xML.Parent();
					}
					else
					{
						if (!obj.Equals(xML.Parent()))
						{
							result = Undefined.Value;
							return result;
						}
					}
				}
				result = obj;
			}
			return result;
		}
		private string Text()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (XML xML in this)
			{
				stringBuilder.Append(xML.Text());
			}
			return stringBuilder.ToString();
		}
		private XMLObject Normalize()
		{
			foreach (XML xML in this.m_Nodes)
			{
				xML.Normalize();
			}
			return this;
		}
		private XMLList Children()
		{
			XMLList xMLList = new XMLList(this.lib);
			foreach (XML xML in this.m_Nodes)
			{
				xMLList.AddRange(xML.Children());
			}
			return xMLList;
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
				object[] array = new object[this.Length()];
				for (int i = 0; i < this.Length(); i++)
				{
					array[i] = i;
				}
				result = array;
			}
			return result;
		}
		public override object GetDefaultValue(Type typeHint)
		{
			object result;
			if (this.m_Nodes.Count == 1)
			{
				result = ((XML)this.m_Nodes[0]).GetDefaultValue(typeHint);
			}
			else
			{
				result = this.ToSource(0);
			}
			return result;
		}
		private XMLList Attribute(XMLName xmlName)
		{
			XMLList xMLList = new XMLList(this.lib);
			foreach (XML xML in this)
			{
				xMLList.AddRange(xML.Attribute(xmlName));
			}
			return xMLList;
		}
		private XMLList Attributes()
		{
			XMLList xMLList = new XMLList(this.lib);
			foreach (XML xML in this)
			{
				xMLList.AddRange(xML.Attributes());
			}
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
				XMLList xMLList = new XMLList(this.lib);
				foreach (XML xML in this.m_Nodes)
				{
					xMLList.AddRange((XMLList)xML.GetXMLProperty(name));
				}
				result = xMLList;
			}
			return result;
		}
		private XMLList Descendants(XMLName xmlName)
		{
			XMLList xMLList = new XMLList(this.lib);
			foreach (XML xML in this)
			{
				xMLList.AddRange(xML.Descendants(xmlName));
			}
			return xMLList;
		}
		private XMLList Child(XMLName xmlName)
		{
			XMLList xMLList = new XMLList(this.lib);
			foreach (XML xML in this)
			{
				xMLList.AddRange(xML.Child(xmlName));
			}
			return xMLList;
		}
		private XMLList Child(long index)
		{
			XMLList xMLList = new XMLList(this.lib);
			foreach (XML xML in this)
			{
				xMLList.AddRange(xML.Child(index));
			}
			return xMLList;
		}
		protected internal override void PutXMLProperty(XMLName name, object value)
		{
			if (this.isPrototype)
			{
				return;
			}
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
			if (this.Length() > 1)
			{
				throw ScriptRuntime.TypeError("Assignment to lists with more that one item is not supported");
			}
			throw new NotImplementedException();
		}
		protected internal override bool EquivalentXml(object value)
		{
			bool result;
			if (this.Length() == 0 && value is Undefined)
			{
				result = true;
			}
			else
			{
				if (this.Length() == 1)
				{
					result = this[0].EquivalentXml(value);
				}
				else
				{
					result = (value is XMLList && value.ToString().Equals(this.ToSource(0)));
				}
			}
			return result;
		}
		private object Copy()
		{
			XMLList xMLList = new XMLList(this.lib);
			foreach (XML xML in this.m_Nodes)
			{
				xMLList.Add((XML)xML.Copy());
			}
			return xMLList;
		}
		private object[] NamespaceDeclarations()
		{
			ArrayList arrayList = new ArrayList();
			foreach (XML xML in this.m_Nodes)
			{
				object[] array = xML.NamespaceDeclarations();
				for (int i = 0; i < array.Length; i++)
				{
					Namespace @namespace = (Namespace)array[i];
					if (!arrayList.Contains(@namespace))
					{
						arrayList.Add(@namespace);
					}
				}
			}
			return arrayList.ToArray();
		}
		private bool HasOwnProperty(XMLName xmlName)
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
		internal XMLList GetPropertyList(XMLName xmlName)
		{
			XMLList xMLList = new XMLList(this.lib);
			foreach (XML xML in this.m_Nodes)
			{
				xMLList.AddRange(xML.GetPropertyList(xmlName));
			}
			return xMLList;
		}
		private void SetNamespace(object ns)
		{
		}
		private bool Contains(object value)
		{
			bool result;
			foreach (XML xML in this)
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
		private XMLList ProcessingInstructions(XMLName name)
		{
			XMLList xMLList = new XMLList(this.lib);
			foreach (XML xML in this)
			{
				xMLList.AddRange(xML.ProcessingInstructions(name));
			}
			return xMLList;
		}
		internal bool PropertyIsEnumerable(object p)
		{
			throw new NotImplementedException();
		}
		private XMLObject ValueOf()
		{
			return this;
		}
		public IEnumerator GetEnumerator()
		{
			return this.m_Nodes.GetEnumerator();
		}
	}
}
