using System;
using System.Reflection;
using System.Runtime.InteropServices;
namespace EcmaScript.NET.Attributes
{
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method), ComVisible(true)]
	public class EcmaScriptFunctionAttribute : Attribute
	{
		private string m_Name = string.Empty;
		internal MethodInfo MethodInfo;
		public string Name
		{
			get
			{
				return this.m_Name;
			}
		}
		public EcmaScriptFunctionAttribute() : this(string.Empty)
		{
		}
		public EcmaScriptFunctionAttribute(string name)
		{
			this.m_Name = name;
		}
	}
}
