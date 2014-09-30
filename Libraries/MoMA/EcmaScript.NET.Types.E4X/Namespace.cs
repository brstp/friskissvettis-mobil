using System;
using System.Text;
namespace EcmaScript.NET.Types.E4X
{
	internal class Namespace : IdScriptableObject
	{
		private const int Id_prefix = 1;
		private const int Id_uri = 2;
		private const int MAX_INSTANCE_ID = 2;
		private const int Id_constructor = 1;
		private const int Id_toString = 2;
		private const int Id_toSource = 3;
		private const int MAX_PROTOTYPE_ID = 3;
		private static readonly object NAMESPACE_TAG = new object();
		private XMLLib lib;
		private string prefix;
		private string uri;
		public override string ClassName
		{
			get
			{
				return "Namespace";
			}
		}
		protected internal override int MaxInstanceId
		{
			get
			{
				return base.MaxInstanceId + 2;
			}
		}
		public virtual string Uri
		{
			get
			{
				return this.uri;
			}
		}
		public virtual string Prefix
		{
			get
			{
				return this.prefix;
			}
		}
		internal Namespace(XMLLib lib) : base(lib.GlobalScope, lib.namespacePrototype)
		{
			this.lib = lib;
		}
		public Namespace(XMLLib lib, string uri) : base(lib.GlobalScope, lib.namespacePrototype)
		{
			if (uri == null)
			{
				throw new ArgumentException();
			}
			this.lib = lib;
			this.prefix = ((uri.Length == 0) ? "" : null);
			this.uri = uri;
		}
		public Namespace(XMLLib lib, string prefix, string uri) : base(lib.GlobalScope, lib.namespacePrototype)
		{
			if (uri == null)
			{
				throw new ArgumentException();
			}
			if (uri.Length == 0)
			{
				if (prefix == null)
				{
					throw new ArgumentException();
				}
				if (prefix.Length != 0)
				{
					throw new ArgumentException();
				}
			}
			this.lib = lib;
			this.prefix = prefix;
			this.uri = uri;
		}
		public virtual void ExportAsJSClass(bool sealed_Renamed)
		{
			base.ExportAsJSClass(3, this.lib.GlobalScope, sealed_Renamed);
		}
		public override string ToString()
		{
			return this.Uri;
		}
		public virtual string toLocaleString()
		{
			return this.ToString();
		}
		public override bool Equals(object obj)
		{
			return obj is Namespace && this.equals((Namespace)obj);
		}
		protected internal override object EquivalentValues(object value_Renamed)
		{
			object result;
			if (!(value_Renamed is Namespace))
			{
				result = UniqueTag.NotFound;
			}
			else
			{
				result = this.equals((Namespace)value_Renamed);
			}
			return result;
		}
		private bool equals(Namespace n)
		{
			return this.Uri.Equals(n.Uri);
		}
		public override object GetDefaultValue(Type hint)
		{
			return this.Uri;
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
				if (length == 6)
				{
					text = "prefix";
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
				result = "prefix";
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
			object result;
			switch (id - base.MaxInstanceId)
			{
			case 1:
				if (this.prefix == null)
				{
					result = Undefined.Value;
				}
				else
				{
					result = this.prefix;
				}
				break;
			case 2:
				result = this.uri;
				break;
			default:
				result = base.GetInstanceIdValue(id);
				break;
			}
			return result;
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
			base.InitPrototypeMethod(Namespace.NAMESPACE_TAG, id, name, arity);
		}
		public override object ExecIdCall(IdFunctionObject f, Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			object result;
			if (!f.HasTag(Namespace.NAMESPACE_TAG))
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
		private Namespace realThis(IScriptable thisObj, IdFunctionObject f)
		{
			if (!(thisObj is Namespace))
			{
				throw IdScriptableObject.IncompatibleCallError(f);
			}
			return (Namespace)thisObj;
		}
		private object jsConstructor(Context cx, bool inNewExpr, object[] args)
		{
			object result;
			if (!inNewExpr && args.Length == 1)
			{
				result = Namespace.Parse(this.lib, cx, args[0]);
			}
			else
			{
				if (args.Length == 0)
				{
					result = Namespace.Parse(this.lib, cx);
				}
				else
				{
					if (args.Length == 1)
					{
						result = Namespace.Parse(this.lib, cx, args[0]);
					}
					else
					{
						result = Namespace.Parse(this.lib, cx, args[0], args[1]);
					}
				}
			}
			return result;
		}
		private string js_toSource()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('(');
			Namespace.toSourceImpl(this.prefix, this.uri, stringBuilder);
			stringBuilder.Append(')');
			return stringBuilder.ToString();
		}
		internal static void toSourceImpl(string prefix, string uri, StringBuilder sb)
		{
			sb.Append("new Namespace(");
			if (uri.Length == 0)
			{
				if (!"".Equals(prefix))
				{
					throw new ArgumentException(prefix);
				}
			}
			else
			{
				sb.Append('\'');
				if (prefix != null)
				{
					sb.Append(ScriptRuntime.escapeString(prefix, '\''));
					sb.Append("', '");
				}
				sb.Append(ScriptRuntime.escapeString(uri, '\''));
				sb.Append('\'');
			}
			sb.Append(')');
		}
		public static Namespace Parse(XMLLib lib, Context cx, object prefixValue, object uriValue)
		{
			string text;
			if (uriValue is QName)
			{
				QName qName = (QName)uriValue;
				text = qName.Uri;
				if (text == null)
				{
					text = qName.ToString();
				}
			}
			else
			{
				text = ScriptConvert.ToString(uriValue);
			}
			string text2;
			if (text.Length == 0)
			{
				if (prefixValue == Undefined.Value)
				{
					text2 = "";
				}
				else
				{
					text2 = ScriptConvert.ToString(prefixValue);
					if (text2.Length != 0)
					{
						throw ScriptRuntime.TypeError("Illegal prefix '" + text2 + "' for 'no namespace'.");
					}
				}
			}
			else
			{
				if (prefixValue == Undefined.Value)
				{
					text2 = "";
				}
				else
				{
					if (!lib.IsXMLName(cx, prefixValue))
					{
						text2 = "";
					}
					else
					{
						text2 = ScriptConvert.ToString(prefixValue);
					}
				}
			}
			return new Namespace(lib, text2, text);
		}
		internal static Namespace Parse(XMLLib lib, Context cx)
		{
			return new Namespace(lib, "", "");
		}
		internal static Namespace Parse(XMLLib lib, Context cx, object uriValue)
		{
			string text;
			string text2;
			if (uriValue is Namespace)
			{
				Namespace @namespace = (Namespace)uriValue;
				text = @namespace.Prefix;
				text2 = @namespace.Uri;
			}
			else
			{
				if (uriValue is QName)
				{
					QName qName = (QName)uriValue;
					text2 = qName.Uri;
					if (text2 != null)
					{
						text = qName.Prefix;
					}
					else
					{
						text2 = qName.ToString();
						text = null;
					}
				}
				else
				{
					text2 = ScriptConvert.ToString(uriValue);
					text = ((text2.Length == 0) ? "" : null);
				}
			}
			return new Namespace(lib, text, text2);
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
