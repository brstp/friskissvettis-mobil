using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET.Attributes
{
	[AttributeUsage(AttributeTargets.Class), ComVisible(true)]
	public class EcmaScriptClassAttribute : Attribute
	{
		private string m_Name = string.Empty;
		public string Name
		{
			get
			{
				return this.m_Name;
			}
		}
		public EcmaScriptClassAttribute(string name)
		{
			this.m_Name = name;
		}
	}
}
