using System;
using System.Runtime.InteropServices;
using System.Security;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public abstract class SecurityController
	{
		private class AnonymousClassScript : IScript
		{
			private ICallable callable;
			private IScriptable thisObj;
			private object[] args;
			private SecurityController enclosingInstance;
			public SecurityController Enclosing_Instance
			{
				get
				{
					return this.enclosingInstance;
				}
			}
			public AnonymousClassScript(ICallable callable, IScriptable thisObj, object[] args, SecurityController enclosingInstance)
			{
				this.InitBlock(callable, thisObj, args, enclosingInstance);
			}
			private void InitBlock(ICallable callable, IScriptable thisObj, object[] args, SecurityController enclosingInstance)
			{
				this.callable = callable;
				this.thisObj = thisObj;
				this.args = args;
				this.enclosingInstance = enclosingInstance;
			}
			public virtual object Exec(Context cx, IScriptable scope)
			{
				return this.callable.Call(cx, scope, this.thisObj, this.args);
			}
		}
		private static SecurityController m_Global;
		internal static SecurityController Global
		{
			get
			{
				return SecurityController.m_Global;
			}
		}
		public static bool HasGlobal()
		{
			return SecurityController.m_Global != null;
		}
		public static void initGlobal(SecurityController controller)
		{
			if (controller == null)
			{
				throw new ArgumentException();
			}
			if (SecurityController.m_Global != null)
			{
				throw new SecurityException("Cannot overwrite already installed global SecurityController");
			}
			SecurityController.m_Global = controller;
		}
		public abstract object getDynamicSecurityDomain(object securityDomain);
		public abstract object callWithDomain(object securityDomain, Context cx, ICallable callable, IScriptable scope, IScriptable thisObj, object[] args);
	}
}
