using System;
using System.Text;
using System.Xml;
namespace EcmaScript.NET.Types.E4X
{
	internal class XMLName : IRef
	{
		internal string uri;
		internal string localName;
		private XMLObject xmlObject;
		internal bool IsAttributeName;
		internal bool IsDescendants;
		public XMLName(string uri, string localName)
		{
			this.uri = uri;
			this.localName = localName;
		}
		public void BindTo(XMLObject obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			if (this.xmlObject != null)
			{
				throw new ArgumentException("Already bound to an xml object.");
			}
			this.xmlObject = obj;
		}
		public object Get(Context cx)
		{
			if (this.xmlObject == null)
			{
				throw ScriptRuntime.UndefReadError(Undefined.Value, this.ToString());
			}
			return this.xmlObject.GetXMLProperty(this);
		}
		public object Set(Context cx, object value)
		{
			if (this.xmlObject == null)
			{
				throw ScriptRuntime.UndefWriteError(this, this.ToString(), value);
			}
			this.xmlObject.PutXMLProperty(this, value);
			return value;
		}
		public bool Has(Context cx)
		{
			throw new Exception("The method or operation is not implemented.");
		}
		public bool Delete(Context cx)
		{
			throw new Exception("The method or operation is not implemented.");
		}
		internal bool Matches(XmlNode node)
		{
			return (this.uri == null || this.uri.Equals(node.NamespaceURI)) && (this.localName == null || !(this.localName != "*") || this.localName.Equals(node.LocalName));
		}
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.IsDescendants)
			{
				stringBuilder.Append("..");
			}
			if (this.IsAttributeName)
			{
				stringBuilder.Append('@');
			}
			string result;
			if (this.uri == null)
			{
				stringBuilder.Append('*');
				if (this.localName.Equals("*"))
				{
					result = stringBuilder.ToString();
					return result;
				}
			}
			else
			{
				stringBuilder.Append('"').Append(this.uri).Append('"');
			}
			stringBuilder.Append(':').Append(this.localName);
			result = stringBuilder.ToString();
			return result;
		}
		internal static XMLName Parse(XMLLib lib, Context cx, object value)
		{
			XMLName result;
			if (value is XMLName)
			{
				result = (XMLName)value;
			}
			else
			{
				if (value is string)
				{
					string text = (string)value;
					long num = ScriptRuntime.testUint32String(text);
					if (num >= 0L)
					{
						ScriptRuntime.storeUint32Result(cx, num);
						result = null;
					}
					else
					{
						result = XMLName.Parse(lib, cx, text);
					}
				}
				else
				{
					if (CliHelper.IsNumber(value))
					{
						double num2 = ScriptConvert.ToNumber(value);
						long num3 = (long)num2;
						if ((double)num3 != num2 || 0L > num3 || num3 > (long)((ulong)-1))
						{
							throw XMLLib.BadXMLName(value);
						}
						ScriptRuntime.storeUint32Result(cx, num3);
						result = null;
					}
					else
					{
						if (value is QName)
						{
							QName qName = (QName)value;
							string text2 = qName.Uri;
							bool flag = false;
							result = null;
							if (text2 != null && text2.Length == 0)
							{
								long num = ScriptRuntime.testUint32String(text2);
								if (num >= 0L)
								{
									ScriptRuntime.storeUint32Result(cx, num);
									flag = true;
								}
							}
							if (!flag)
							{
								result = XMLName.FormProperty(text2, qName.LocalName);
							}
						}
						else
						{
							if (value is bool || value == Undefined.Value || value == null)
							{
								throw XMLLib.BadXMLName(value);
							}
							string text = ScriptConvert.ToString(value);
							long num = ScriptRuntime.testUint32String(text);
							if (num >= 0L)
							{
								ScriptRuntime.storeUint32Result(cx, num);
								result = null;
							}
							else
							{
								result = XMLName.Parse(lib, cx, text);
							}
						}
					}
				}
			}
			return result;
		}
		internal static XMLName FormStar()
		{
			return new XMLName(null, "*");
		}
		internal static XMLName FormProperty(string uri, string localName)
		{
			return new XMLName(uri, localName);
		}
		internal static XMLName Parse(XMLLib lib, Context cx, string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			int length = name.Length;
			XMLName result;
			if (length != 0)
			{
				char c = name[0];
				if (c == '*')
				{
					if (length == 1)
					{
						result = XMLName.FormStar();
						return result;
					}
				}
				else
				{
					if (c == '@')
					{
						XMLName xMLName = XMLName.FormProperty("", name.Substring(1));
						xMLName.IsAttributeName = true;
						result = xMLName;
						return result;
					}
				}
			}
			string defaultNamespaceURI = lib.GetDefaultNamespaceURI(cx);
			result = XMLName.FormProperty(defaultNamespaceURI, name);
			return result;
		}
	}
}
