using System;
using System.Text;
namespace EcmaScript.NET.Types.E4X
{
	[Serializable]
	internal sealed class QName : IdScriptableObject
	{
		private const int Id_localName = 1;
		private const int Id_uri = 2;
		private const int MAX_INSTANCE_ID = 2;
		private const int Id_constructor = 1;
		private const int Id_toString = 2;
		private const int Id_toSource = 3;
		private const int MAX_PROTOTYPE_ID = 3;
		private static readonly object QNAME_TAG = new object();
		internal XMLLib lib;
		private string prefix;
		private string localName;
		private string uri;
		public override string ClassName
		{
			get
			{
				return "QName";
			}
		}
		protected internal override int MaxInstanceId
		{
			get
			{
				return base.MaxInstanceId + 2;
			}
		}
		public string LocalName
		{
			get
			{
				return this.localName;
			}
		}
		internal string Prefix
		{
			get
			{
				return (this.prefix == null) ? this.prefix : "";
			}
		}
		internal string Uri
		{
			get
			{
				return this.uri;
			}
		}
		internal QName(XMLLib lib) : base(lib.GlobalScope, lib.qnamePrototype)
		{
			this.lib = lib;
		}
		internal QName(XMLLib lib, QName qname) : this(lib, qname.Uri, qname.LocalName, qname.Prefix)
		{
		}
		internal QName(XMLLib lib, string uri, string localName, string prefix) : base(lib.GlobalScope, lib.qnamePrototype)
		{
			if (localName == null)
			{
				throw new ArgumentException();
			}
			this.lib = lib;
			this.uri = uri;
			this.prefix = prefix;
			this.localName = localName;
		}
		internal void ExportAsJSClass(bool zealed)
		{
			base.ExportAsJSClass(3, this.lib.GlobalScope, zealed);
		}
		public override string ToString()
		{
			string result;
			if (this.uri == null)
			{
				result = "*::" + this.localName;
			}
			else
			{
				if (this.uri.Length == 0)
				{
					result = this.localName;
				}
				else
				{
					result = this.uri + "::" + this.localName;
				}
			}
			return result;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			QName qName = obj as QName;
			return qName != null && this.Equals(qName);
		}
		protected internal override object EquivalentValues(object value)
		{
			QName qName = value as QName;
			object result;
			if (qName == null)
			{
				result = UniqueTag.NotFound;
			}
			else
			{
				result = this.Equals(qName);
			}
			return result;
		}
		private bool Equals(QName q)
		{
			return CliHelper.Equals(this.LocalName, q.LocalName) && CliHelper.Equals(this.Uri, q.Uri);
		}
		public override object GetDefaultValue(Type hint)
		{
			return this.ToString();
		}
		protected internal override int FindInstanceIdInfo(string s)
		{
			int num = 0;
			string text = null;
			int length = s.Length;
			if (length == 3)
			{
				text = "uri";
				num = 2;
			}
			else
			{
				if (length == 9)
				{
					text = "localName";
					num = 1;
				}
			}
			if (text != null && text != s && !text.Equals(s))
			{
				num = 0;
			}
			int result;
			if (num == 0)
			{
				result = base.FindInstanceIdInfo(s);
			}
			else
			{
				switch (num)
				{
				case 1:
				case 2:
				{
					int attributes = 5;
					result = IdScriptableObject.InstanceIdInfo(attributes, base.MaxInstanceId + num);
					break;
				}
				default:
					throw new SystemException();
				}
			}
			return result;
		}
		protected internal override string GetInstanceIdName(int id)
		{
			string result;
			switch (id - base.MaxInstanceId)
			{
			case 1:
				result = "localName";
				break;
			case 2:
				result = "uri";
				break;
			default:
				result = base.GetInstanceIdName(id);
				break;
			}
			return result;
		}
		protected internal override object GetInstanceIdValue(int id)
		{
			object instanceIdValue;
			switch (id - base.MaxInstanceId)
			{
			case 1:
				instanceIdValue = this.localName;
				break;
			case 2:
				instanceIdValue = this.uri;
				break;
			default:
				instanceIdValue = base.GetInstanceIdValue(id);
				break;
			}
			return instanceIdValue;
		}
		protected internal override int FindPrototypeId(string s)
		{
			int result = 0;
			string text = null;
			int length = s.Length;
			if (length == 8)
			{
				int num = (int)s[3];
				if (num == 111)
				{
					text = "toSource";
					result = 3;
				}
				else
				{
					if (num == 116)
					{
						text = "toString";
						result = 2;
					}
				}
			}
			else
			{
				if (length == 11)
				{
					text = "constructor";
					result = 1;
				}
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
				arity = 2;
				name = "constructor";
				break;
			case 2:
				arity = 0;
				name = "toString";
				break;
			case 3:
				arity = 0;
				name = "toSource";
				break;
			default:
				throw new ArgumentException(Convert.ToString(id));
			}
			base.InitPrototypeMethod(QName.QNAME_TAG, id, name, arity);
		}
		public override object ExecIdCall(IdFunctionObject f, Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			object result;
			if (!f.HasTag(QName.QNAME_TAG))
			{
				result = base.ExecIdCall(f, cx, scope, thisObj, args);
			}
			else
			{
				int methodId = f.MethodId;
				switch (methodId)
				{
				case 1:
					result = this.jsConstructor(cx, thisObj == null, args);
					break;
				case 2:
					result = this.realThis(thisObj, f).ToString();
					break;
				case 3:
					result = this.realThis(thisObj, f).js_toSource();
					break;
				default:
					throw new ArgumentException(Convert.ToString(methodId));
				}
			}
			return result;
		}
		private QName realThis(IScriptable thisObj, IdFunctionObject f)
		{
			if (!(thisObj is QName))
			{
				throw IdScriptableObject.IncompatibleCallError(f);
			}
			return (QName)thisObj;
		}
		private object jsConstructor(Context cx, bool inNewExpr, object[] args)
		{
			object result;
			if (!inNewExpr && args.Length == 1)
			{
				result = QName.Parse(this.lib, cx, args[0]);
			}
			else
			{
				if (args.Length == 0)
				{
					result = QName.Parse(this.lib, cx, Undefined.Value);
				}
				else
				{
					if (args.Length == 1)
					{
						result = QName.Parse(this.lib, cx, args[0]);
					}
					else
					{
						result = QName.Parse(this.lib, cx, args[0], args[1]);
					}
				}
			}
			return result;
		}
		internal static QName Parse(XMLLib lib, Context cx, object value)
		{
			QName result;
			if (value is QName)
			{
				QName qName = (QName)value;
				result = new QName(lib, qName.Uri, qName.LocalName, qName.Prefix);
			}
			else
			{
				result = QName.Parse(lib, cx, ScriptConvert.ToString(value));
			}
			return result;
		}
		internal static QName Parse(XMLLib lib, Context cx, string localName)
		{
			if (localName == null)
			{
				throw new ArgumentNullException("localName");
			}
			string text;
			string text2;
			if ("*".Equals(localName))
			{
				text = null;
				text2 = null;
			}
			else
			{
				Namespace defaultNamespace = lib.GetDefaultNamespace(cx);
				text = defaultNamespace.Uri;
				text2 = defaultNamespace.Prefix;
			}
			return new QName(lib, text, localName, text2);
		}
		internal static QName Parse(XMLLib lib, Context cx, object namespaceValue, object nameValue)
		{
			string value;
			if (nameValue is QName)
			{
				QName qName = (QName)nameValue;
				value = qName.LocalName;
			}
			else
			{
				value = ScriptConvert.ToString(nameValue);
			}
			Namespace @namespace;
			if (namespaceValue == Undefined.Value)
			{
				if ("*".Equals(value))
				{
					@namespace = null;
				}
				else
				{
					@namespace = lib.GetDefaultNamespace(cx);
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
						@namespace = Namespace.Parse(lib, cx, namespaceValue);
					}
				}
			}
			string text;
			string text2;
			if (@namespace == null)
			{
				text = null;
				text2 = null;
			}
			else
			{
				text = @namespace.Uri;
				text2 = @namespace.Prefix;
			}
			return new QName(lib, text, value, text2);
		}
		private string js_toSource()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('(');
			QName.toSourceImpl(this.uri, this.localName, this.prefix, stringBuilder);
			stringBuilder.Append(')');
			return stringBuilder.ToString();
		}
		private static void toSourceImpl(string uri, string localName, string prefix, StringBuilder sb)
		{
			sb.Append("new QName(");
			if (uri == null && prefix == null)
			{
				if (!"*".Equals(localName))
				{
					sb.Append("null, ");
				}
			}
			else
			{
				Namespace.toSourceImpl(prefix, uri, sb);
				sb.Append(", ");
			}
			sb.Append('\'');
			sb.Append(ScriptRuntime.escapeString(localName, '\''));
			sb.Append("')");
		}
	}
}
