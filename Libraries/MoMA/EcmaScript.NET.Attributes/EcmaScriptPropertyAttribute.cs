using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET.Attributes
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field), ComVisible(true)]
	public class EcmaScriptPropertyAttribute : Attribute
	{
		private string m_Name = string.Empty;
		private EcmaScriptPropertyAccess m_Access = EcmaScriptPropertyAccess.AsDeclared;
		public string Name
		{
			get
			{
				return this.m_Name;
			}
		}
		public EcmaScriptPropertyAccess Access
		{
			get
			{
				return this.m_Access;
			}
		}
		public EcmaScriptPropertyAttribute(string name)
		{
			this.m_Name = name;
		}
		public EcmaScriptPropertyAttribute(string name, EcmaScriptPropertyAccess access)
		{
			this.m_Name = name;
			this.m_Access = access;
		}
	}
}
