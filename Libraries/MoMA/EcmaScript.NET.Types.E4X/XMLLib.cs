using System;
using System.Xml;
namespace EcmaScript.NET.Types.E4X
{
	internal class XMLLib
	{
		private IScriptable globalScope;
		internal bool ignoreComments;
		internal bool ignoreProcessingInstructions;
		internal bool ignoreWhitespace;
		internal bool prettyPrinting;
		internal int prettyIndent;
		internal XML xmlPrototype;
		internal XMLList xmlListPrototype;
		internal Namespace namespacePrototype;
		internal QName qnamePrototype;
		private static readonly object XML_LIB_KEY = new object();
		public IScriptable GlobalScope
		{
			get
			{
				return this.globalScope;
			}
		}
		private XMLLib(IScriptable globalScope)
		{
			this.globalScope = globalScope;
		}
		internal void SetDefaultSettings()
		{
			this.ignoreComments = true;
			this.ignoreProcessingInstructions = true;
			this.ignoreWhitespace = true;
			this.prettyPrinting = true;
			this.prettyIndent = 2;
		}
		public static XMLLib ExtractFromScopeOrNull(IScriptable scope)
		{
			ScriptableObject libraryScopeOrNull = ScriptRuntime.getLibraryScopeOrNull(scope);
			XMLLib result;
			if (libraryScopeOrNull == null)
			{
				result = null;
			}
			else
			{
				result = (XMLLib)libraryScopeOrNull.GetAssociatedValue(XMLLib.XML_LIB_KEY);
			}
			return result;
		}
		public static XMLLib ExtractFromScope(IScriptable scope)
		{
			XMLLib xMLLib = XMLLib.ExtractFromScopeOrNull(scope);
			if (xMLLib != null)
			{
				return xMLLib;
			}
			string message = ScriptRuntime.GetMessage("msg.XML.not.available", new object[0]);
			throw Context.ReportRuntimeError(message);
		}
		internal XMLLib BindToScope(IScriptable scope)
		{
			ScriptableObject libraryScopeOrNull = ScriptRuntime.getLibraryScopeOrNull(scope);
			if (libraryScopeOrNull == null)
			{
				throw new ApplicationException();
			}
			return (XMLLib)libraryScopeOrNull.AssociateValue(XMLLib.XML_LIB_KEY, this);
		}
		public static void Init(IScriptable scope, bool zealed)
		{
			XMLLib xMLLib = new XMLLib(scope);
			xMLLib.SetDefaultSettings();
			xMLLib.BindToScope(scope);
			xMLLib.xmlPrototype = XML.CreateEmptyXml(xMLLib);
			xMLLib.xmlPrototype.ExportAsJSClass(zealed);
			xMLLib.xmlListPrototype = new XMLList(xMLLib);
			xMLLib.xmlListPrototype.ExportAsJSClass(zealed);
			xMLLib.qnamePrototype = new QName(xMLLib);
			xMLLib.qnamePrototype.ExportAsJSClass(zealed);
			xMLLib.namespacePrototype = new Namespace(xMLLib);
			xMLLib.namespacePrototype.ExportAsJSClass(zealed);
		}
		public bool IsXMLName(Context cx, object value)
		{
			string text;
			bool result;
			try
			{
				text = ScriptConvert.ToString(value);
			}
			catch (EcmaScriptError ecmaScriptError)
			{
				if ("TypeError".Equals(ecmaScriptError.Name))
				{
					result = false;
					return result;
				}
				throw ecmaScriptError;
			}
			int length = text.Length;
			if (length != 0)
			{
				if (XMLLib.IsNCNameStartChar((int)text[0]))
				{
					for (int num = 1; num != length; num++)
					{
						if (!XMLLib.IsNCNameChar((int)text[num]))
						{
							result = false;
							return result;
						}
					}
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
		private static bool IsNCNameStartChar(int c)
		{
			bool result;
			if ((c & -128) == 0)
			{
				if (c >= 97)
				{
					result = (c <= 122);
					return result;
				}
				if (c >= 65)
				{
					result = (c <= 90 || c == 95);
					return result;
				}
			}
			else
			{
				if ((c & -8192) == 0)
				{
					result = ((192 <= c && c <= 214) || (216 <= c && c <= 246) || (248 <= c && c <= 767) || (880 <= c && c <= 893) || 895 <= c);
					return result;
				}
			}
			result = ((8204 <= c && c <= 8205) || (8304 <= c && c <= 8591) || (11264 <= c && c <= 12271) || (12289 <= c && c <= 55295) || (63744 <= c && c <= 64975) || (65008 <= c && c <= 65533) || (65536 <= c && c <= 983039));
			return result;
		}
		private static bool IsNCNameChar(int c)
		{
			bool result;
			if ((c & -128) == 0)
			{
				if (c >= 97)
				{
					result = (c <= 122);
				}
				else
				{
					if (c >= 65)
					{
						result = (c <= 90 || c == 95);
					}
					else
					{
						if (c >= 48)
						{
							result = (c <= 57);
						}
						else
						{
							result = (c == 45 || c == 46);
						}
					}
				}
			}
			else
			{
				if ((c & -8192) == 0)
				{
					result = (XMLLib.IsNCNameStartChar(c) || c == 183 || (768 <= c && c <= 879));
				}
				else
				{
					result = (XMLLib.IsNCNameStartChar(c) || (8255 <= c && c <= 8256));
				}
			}
			return result;
		}
		public string GetDefaultNamespaceURI(Context cx)
		{
			string result = "";
			if (cx == null)
			{
				cx = Context.CurrentContext;
			}
			if (cx != null)
			{
				object obj = ScriptRuntime.searchDefaultNamespace(cx);
				if (obj != null)
				{
					if (obj is Namespace)
					{
						result = ((Namespace)obj).Uri;
					}
				}
			}
			return result;
		}
		public Namespace GetDefaultNamespace(Context cx)
		{
			Namespace result;
			if (cx == null)
			{
				cx = Context.CurrentContext;
				if (cx == null)
				{
					result = this.namespacePrototype;
					return result;
				}
			}
			object obj = ScriptRuntime.searchDefaultNamespace(cx);
			Namespace @namespace;
			if (obj == null)
			{
				@namespace = this.namespacePrototype;
			}
			else
			{
				if (obj is Namespace)
				{
					@namespace = (Namespace)obj;
				}
				else
				{
					@namespace = this.namespacePrototype;
				}
			}
			result = @namespace;
			return result;
		}
		public IRef NameRef(Context cx, object name, IScriptable scope, int memberTypeFlags)
		{
			XMLName xMLName = XMLName.Parse(this, cx, name);
			IRef result;
			if (xMLName == null)
			{
				result = null;
			}
			else
			{
				result = xMLName;
			}
			return result;
		}
		public IRef NameRef(Context cx, object ns, object name, IScriptable scope, int memberTypeFlags)
		{
			throw new NotImplementedException();
		}
		public string EscapeAttributeValue(object value)
		{
			throw new NotImplementedException();
		}
		public string EscapeTextValue(object value)
		{
			throw new NotImplementedException();
		}
		public object ToDefaultXmlNamespace(Context cx, object uriValue)
		{
			return Namespace.Parse(this, cx, uriValue);
		}
		internal static EcmaScriptError BadXMLName(object value)
		{
			string str;
			if (CliHelper.IsNumber(value))
			{
				str = "Can not construct XML name from number: ";
			}
			else
			{
				if (value is bool)
				{
					str = "Can not construct XML name from boolean: ";
				}
				else
				{
					if (value != Undefined.Value && value != null)
					{
						throw new ArgumentException(value.ToString());
					}
					str = "Can not construct XML name from ";
				}
			}
			return ScriptRuntime.TypeError(str + ScriptConvert.ToString(value));
		}
		internal XMLName toQualifiedName(Context cx, object namespaceValue, object nameValue)
		{
			string text;
			if (nameValue is QName)
			{
				QName qName = (QName)nameValue;
				text = qName.LocalName;
			}
			else
			{
				text = ScriptConvert.ToString(nameValue);
			}
			Namespace @namespace;
			if (namespaceValue == Undefined.Value)
			{
				if ("*".Equals(text))
				{
					@namespace = null;
				}
				else
				{
					@namespace = this.GetDefaultNamespace(cx);
				}
			}
			else
			{
				if (namespaceValue == null)
				{
					@namespace = null;
				}
				else
				{
					if (namespaceValue is Namespace)
					{
						@namespace = (Namespace)namespaceValue;
					}
					else
					{
						@namespace = Namespace.Parse(this, cx, namespaceValue);
					}
				}
			}
			string uri;
			if (@namespace == null)
			{
				uri = null;
			}
			else
			{
				uri = @namespace.Uri;
			}
			return XMLName.FormProperty(uri, text);
		}
		internal XMLList ToXMLList(object value)
		{
			XMLList result;
			if (value == null || value is Undefined)
			{
				result = null;
			}
			else
			{
				if (value is XMLList)
				{
					result = (XMLList)value;
				}
				else
				{
					if (value is XML)
					{
						result = null;
					}
					else
					{
						if (value is XmlNode)
						{
							result = null;
						}
						else
						{
							result = new XMLList(this, value);
						}
					}
				}
			}
			return result;
		}
		internal XML ToXML(object value)
		{
			XML result;
			if (value == null || value is Undefined)
			{
				result = null;
			}
			else
			{
				if (value is XML)
				{
					result = (XML)value;
				}
				else
				{
					if (value is XMLList)
					{
						result = null;
					}
					else
					{
						if (value is XmlNode)
						{
							result = new XML(this, (XmlNode)value);
						}
						else
						{
							result = XML.CreateFromJS(this, value);
						}
					}
				}
			}
			return result;
		}
	}
}
