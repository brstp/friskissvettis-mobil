using EcmaScript.NET.Attributes;
using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
namespace EcmaScript.NET.Types.Cli
{
	[ComVisible(true)]
	public class CliType : BaseFunction, Wrapper
	{
		private CliObject m_CliObject = null;
		private Type m_Type = null;
		private static Hashtable typeCache = new Hashtable();
		private MemberInfo[] m_IndexGetter = null;
		public EcmaScriptClassAttribute m_ClassAttribute = null;
		private EcmaScriptFunctionAttribute[] m_FunctionAttributes = null;
		private Hashtable functionCache = new Hashtable();
		private Hashtable functionsWithAttributeCache = new Hashtable();
		private MemberInfo[] m_IndexSetter = null;
		private Hashtable propertyCache = new Hashtable();
		private Hashtable fieldCache = new Hashtable();
		public override string ClassName
		{
			get
			{
				return this.m_Type.FullName;
			}
		}
		public Type UnderlyingType
		{
			get
			{
				return this.m_Type;
			}
		}
		public MemberInfo[] IndexGetter
		{
			get
			{
				if (this.m_IndexGetter == null)
				{
					this.m_IndexGetter = this.m_Type.GetMember("get_Item");
					if (this.m_IndexGetter == null)
					{
						this.m_IndexGetter = new MemberInfo[0];
					}
				}
				return this.m_IndexGetter;
			}
		}
		public EcmaScriptClassAttribute ClassAttribute
		{
			get
			{
				return this.m_ClassAttribute;
			}
		}
		public EcmaScriptFunctionAttribute[] FunctionAttributes
		{
			get
			{
				EcmaScriptFunctionAttribute[] result;
				if (this.m_FunctionAttributes != null)
				{
					result = this.m_FunctionAttributes;
				}
				else
				{
					ArrayList arrayList = new ArrayList();
					MethodInfo[] methods = this.m_Type.GetMethods();
					for (int i = 0; i < methods.Length; i++)
					{
						MethodInfo methodInfo = methods[i];
						EcmaScriptFunctionAttribute ecmaScriptFunctionAttribute = (EcmaScriptFunctionAttribute)CliHelper.GetCustomAttribute(typeof(EcmaScriptFunctionAttribute), methodInfo);
						if (ecmaScriptFunctionAttribute != null)
						{
							ecmaScriptFunctionAttribute.MethodInfo = methodInfo;
							arrayList.Add(ecmaScriptFunctionAttribute);
						}
					}
					result = (this.m_FunctionAttributes = (EcmaScriptFunctionAttribute[])arrayList.ToArray(typeof(EcmaScriptFunctionAttribute)));
				}
				return result;
			}
		}
		public MemberInfo[] IndexSetter
		{
			get
			{
				if (this.m_IndexSetter == null)
				{
					this.m_IndexSetter = this.m_Type.GetMember("set_Item");
					if (this.m_IndexSetter == null)
					{
						this.m_IndexSetter = new MemberInfo[0];
					}
				}
				return this.m_IndexSetter;
			}
		}
		public object Unwrap()
		{
			return this.m_CliObject;
		}
		private CliType(Type type)
		{
			this.m_Type = type;
			this.m_ClassAttribute = (EcmaScriptClassAttribute)CliHelper.GetCustomAttribute(this.m_Type, typeof(EcmaScriptClassAttribute));
		}
		private void Init()
		{
			this.m_CliObject = new CliObject(this.m_Type, this.m_Type);
		}
		public static CliType GetNativeCliType(Type type)
		{
			CliType result;
			if (CliType.typeCache.ContainsKey(type))
			{
				result = (CliType)CliType.typeCache[type];
			}
			else
			{
				CliType cliType = new CliType(type);
				Hashtable obj;
				Monitor.Enter(obj = CliType.typeCache);
				try
				{
					CliType.typeCache[type] = cliType;
					cliType.Init();
				}
				finally
				{
					Monitor.Exit(obj);
				}
				result = cliType;
			}
			return result;
		}
		public CliMethodInfo GetFunctions(string name)
		{
			CliMethodInfo result;
			if (this.functionCache.ContainsKey(name))
			{
				result = (CliMethodInfo)this.functionCache[name];
			}
			else
			{
				ArrayList arrayList = new ArrayList();
				MethodInfo[] methods = this.m_Type.GetMethods();
				for (int i = 0; i < methods.Length; i++)
				{
					MethodInfo methodInfo = methods[i];
					if (0 == string.Compare(name, methodInfo.Name))
					{
						arrayList.Add(methodInfo);
					}
				}
				CliMethodInfo cliMethodInfo = null;
				if (arrayList.Count > 0)
				{
					cliMethodInfo = new CliMethodInfo(name, (MethodInfo[])arrayList.ToArray(typeof(MethodInfo)), null);
				}
				Hashtable obj;
				Monitor.Enter(obj = this.functionCache);
				try
				{
					this.functionCache[name] = cliMethodInfo;
				}
				finally
				{
					Monitor.Exit(obj);
				}
				result = cliMethodInfo;
			}
			return result;
		}
		public CliMethodInfo GetFunctionsWithAttribute(string name)
		{
			CliMethodInfo result;
			if (this.functionsWithAttributeCache.ContainsKey(name))
			{
				result = (CliMethodInfo)this.functionsWithAttributeCache[name];
			}
			else
			{
				ArrayList arrayList = new ArrayList();
				EcmaScriptFunctionAttribute[] functionAttributes = this.FunctionAttributes;
				for (int i = 0; i < functionAttributes.Length; i++)
				{
					EcmaScriptFunctionAttribute ecmaScriptFunctionAttribute = functionAttributes[i];
					if (0 == string.Compare(ecmaScriptFunctionAttribute.Name, name))
					{
						arrayList.Add(ecmaScriptFunctionAttribute.MethodInfo);
					}
				}
				CliMethodInfo cliMethodInfo = null;
				if (arrayList.Count > 0)
				{
					cliMethodInfo = new CliMethodInfo(name, (MethodInfo[])arrayList.ToArray(typeof(MethodInfo)), null);
				}
				Hashtable obj;
				Monitor.Enter(obj = this.functionsWithAttributeCache);
				try
				{
					this.functionsWithAttributeCache[name] = cliMethodInfo;
				}
				finally
				{
					Monitor.Exit(obj);
				}
				result = cliMethodInfo;
			}
			return result;
		}
		public override object Call(Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			object result;
			try
			{
				result = cx.Wrap(scope, Activator.CreateInstance(this.m_Type, args), this.m_Type);
			}
			catch (Exception e)
			{
				throw Context.ThrowAsScriptRuntimeEx(e);
			}
			return result;
		}
		public override object Get(string name, IScriptable start)
		{
			object obj = base.Get(name, start);
			if (obj == UniqueTag.NotFound)
			{
				obj = this.m_CliObject.Get(name, start);
			}
			return obj;
		}
		public PropertyInfo GetCachedProperty(string name)
		{
			PropertyInfo result;
			if (this.propertyCache.Contains(name))
			{
				result = (PropertyInfo)this.propertyCache[name];
			}
			else
			{
				PropertyInfo property = this.m_Type.GetProperty(name);
				Hashtable obj;
				Monitor.Enter(obj = this.propertyCache);
				try
				{
					this.propertyCache[name] = property;
				}
				finally
				{
					Monitor.Exit(obj);
				}
				result = property;
			}
			return result;
		}
		public FieldInfo GetCachedField(string name)
		{
			FieldInfo result;
			if (this.fieldCache.Contains(name))
			{
				result = (FieldInfo)this.fieldCache[name];
			}
			else
			{
				FieldInfo field = this.m_Type.GetField(name);
				Hashtable obj;
				Monitor.Enter(obj = this.fieldCache);
				try
				{
					this.fieldCache[name] = field;
				}
				finally
				{
					Monitor.Exit(obj);
				}
				result = field;
			}
			return result;
		}
		internal EventInfo GetCachedEvent(string name)
		{
			return this.m_Type.GetEvent(name);
		}
		public override object GetDefaultValue(Type typeHint)
		{
			object result;
			if (typeHint == typeof(string))
			{
				result = this.ToString();
			}
			else
			{
				result = base.GetDefaultValue(typeHint);
			}
			return result;
		}
		public override string ToString()
		{
			string str = "function " + this.ClassName + "() \n";
			str += "{/*\n";
			ConstructorInfo[] constructors = this.m_Type.GetConstructors();
			for (int i = 0; i < constructors.Length; i++)
			{
				ConstructorInfo ci = constructors[i];
				str = str + CliHelper.ToSignature(ci) + "\n";
			}
			MemberInfo[] members = this.m_Type.GetMembers(BindingFlags.Static | BindingFlags.Public);
			for (int i = 0; i < members.Length; i++)
			{
				MemberInfo mi = members[i];
				str = str + CliHelper.ToSignature(mi) + "\n";
			}
			return str + "*/}";
		}
	}
}
