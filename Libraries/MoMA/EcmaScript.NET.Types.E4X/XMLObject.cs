using System;
namespace EcmaScript.NET.Types.E4X
{
	internal abstract class XMLObject : IdScriptableObject
	{
		private static readonly object XMLOBJECT_TAG = new object();
		internal XMLLib lib = null;
		internal bool isPrototype = false;
		public override string ClassName
		{
			get
			{
				return "XMLObject";
			}
		}
		public XMLObject()
		{
		}
		public XMLObject(IScriptable scope, IScriptable prototype) : base(scope, prototype)
		{
		}
		protected internal XMLObject(XMLLib lib, XMLObject prototype) : base(lib.GlobalScope, prototype)
		{
			this.lib = lib;
		}
		public virtual object AddValues(Context cx, bool thisIsLeft, object value)
		{
			return UniqueTag.NotFound;
		}
		public bool EcmaHas(Context cx, object id)
		{
			throw new NotImplementedException();
		}
		public object EcmaGet(Context cx, object id)
		{
			XMLName xMLName = XMLName.Parse(this.lib, cx, id);
			object result;
			if (xMLName == null)
			{
				long num = ScriptRuntime.lastUint32Result(cx);
				object obj = base.Get((int)num, this);
				if (obj == UniqueTag.NotFound)
				{
					result = Undefined.Value;
				}
				else
				{
					result = obj;
				}
			}
			else
			{
				result = this.GetXMLProperty(xMLName);
			}
			return result;
		}
		public void EcmaPut(Context cx, object id, object value)
		{
			if (cx == null)
			{
				cx = Context.CurrentContext;
			}
			XMLName xMLName = XMLName.Parse(this.lib, cx, id);
			if (xMLName == null)
			{
				long num = ScriptRuntime.lastUint32Result(cx);
				this.Put((int)num, this, value);
			}
			else
			{
				this.PutXMLProperty(xMLName, value);
			}
		}
		public bool EcmaDelete(Context cx, object id)
		{
			throw new NotImplementedException();
		}
		public IRef MemberRef(Context cx, object elem, int memberTypeFlags)
		{
			XMLName xMLName = XMLName.Parse(this.lib, cx, elem);
			if ((memberTypeFlags & 2) != 0)
			{
				xMLName.IsAttributeName = true;
			}
			if ((memberTypeFlags & 4) != 0)
			{
				xMLName.IsDescendants = true;
			}
			xMLName.BindTo(this);
			return xMLName;
		}
		public IRef MemberRef(Context cx, object ns, object elem, int memberTypeFlags)
		{
			XMLName xMLName = this.lib.toQualifiedName(cx, ns, elem);
			if ((memberTypeFlags & 2) != 0)
			{
				xMLName.IsAttributeName = true;
			}
			if ((memberTypeFlags & 4) != 0)
			{
				xMLName.IsDescendants = true;
			}
			xMLName.BindTo(this);
			return xMLName;
		}
		public BuiltinWith EnterWith(IScriptable scope)
		{
			throw new NotImplementedException();
		}
		public BuiltinWith EnterDotQuery(IScriptable scope)
		{
			throw new NotImplementedException();
		}
		protected internal override object EquivalentValues(object value)
		{
			return this.EquivalentXml(value);
		}
		protected object GetArgSafe(object[] args, int index)
		{
			object result;
			if (index >= 0 && index < args.Length)
			{
				result = args[index];
			}
			else
			{
				result = null;
			}
			return result;
		}
		protected internal abstract IScriptable GetExtraMethodSource(Context cx);
		protected internal abstract bool EquivalentXml(object value);
		protected internal abstract object GetXMLProperty(XMLName name);
		protected internal abstract void PutXMLProperty(XMLName xmlName, object value);
		protected internal abstract string ToXMLString();
	}
}
