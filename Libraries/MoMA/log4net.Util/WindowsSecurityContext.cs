using log4net.Core;
using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
namespace log4net.Util
{
	public class WindowsSecurityContext : SecurityContext, IOptionHandler
	{
		public enum ImpersonationMode
		{
			User,
			Process
		}
		private sealed class DisposableImpersonationContext : IDisposable
		{
			private readonly WindowsImpersonationContext m_impersonationContext;
			public DisposableImpersonationContext(WindowsImpersonationContext impersonationContext)
			{
				this.m_impersonationContext = impersonationContext;
			}
			public void Dispose()
			{
				this.m_impersonationContext.Undo();
			}
		}
		private WindowsSecurityContext.ImpersonationMode m_impersonationMode = WindowsSecurityContext.ImpersonationMode.User;
		private string m_userName;
		private string m_domainName = Environment.MachineName;
		private string m_password;
		private WindowsIdentity m_identity;
		public WindowsSecurityContext.ImpersonationMode Credentials
		{
			get
			{
				return this.m_impersonationMode;
			}
			set
			{
				this.m_impersonationMode = value;
			}
		}
		public string UserName
		{
			get
			{
				return this.m_userName;
			}
			set
			{
				this.m_userName = value;
			}
		}
		public string DomainName
		{
			get
			{
				return this.m_domainName;
			}
			set
			{
				this.m_domainName = value;
			}
		}
		public string Password
		{
			set
			{
				this.m_password = value;
			}
		}
		public void ActivateOptions()
		{
			if (this.m_impersonationMode == WindowsSecurityContext.ImpersonationMode.User)
			{
				if (this.m_userName == null)
				{
					throw new ArgumentNullException("m_userName");
				}
				if (this.m_domainName == null)
				{
					throw new ArgumentNullException("m_domainName");
				}
				if (this.m_password == null)
				{
					throw new ArgumentNullException("m_password");
				}
				this.m_identity = WindowsSecurityContext.LogonUser(this.m_userName, this.m_domainName, this.m_password);
			}
		}
		public override IDisposable Impersonate(object state)
		{
			IDisposable result;
			if (this.m_impersonationMode == WindowsSecurityContext.ImpersonationMode.User)
			{
				if (this.m_identity != null)
				{
					result = new WindowsSecurityContext.DisposableImpersonationContext(this.m_identity.Impersonate());
					return result;
				}
			}
			else
			{
				if (this.m_impersonationMode == WindowsSecurityContext.ImpersonationMode.Process)
				{
					result = new WindowsSecurityContext.DisposableImpersonationContext(WindowsIdentity.Impersonate(IntPtr.Zero));
					return result;
				}
			}
			result = null;
			return result;
		}
		private static WindowsIdentity LogonUser(string userName, string domainName, string password)
		{
			IntPtr zero = IntPtr.Zero;
			if (!WindowsSecurityContext.LogonUser(userName, domainName, password, 2, 0, ref zero))
			{
				NativeError lastError = NativeError.GetLastError();
				throw new Exception(string.Concat(new string[]
				{
					"Failed to LogonUser [",
					userName,
					"] in Domain [",
					domainName,
					"]. Error: ",
					lastError.ToString()
				}));
			}
			IntPtr zero2 = IntPtr.Zero;
			if (!WindowsSecurityContext.DuplicateToken(zero, 2, ref zero2))
			{
				NativeError lastError = NativeError.GetLastError();
				if (zero != IntPtr.Zero)
				{
					WindowsSecurityContext.CloseHandle(zero);
				}
				throw new Exception("Failed to DuplicateToken after LogonUser. Error: " + lastError.ToString());
			}
			WindowsIdentity result = new WindowsIdentity(zero2);
			if (zero2 != IntPtr.Zero)
			{
				WindowsSecurityContext.CloseHandle(zero2);
			}
			if (zero != IntPtr.Zero)
			{
				WindowsSecurityContext.CloseHandle(zero);
			}
			return result;
		}
		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);
		[DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
		private static extern bool CloseHandle(IntPtr handle);
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool DuplicateToken(IntPtr ExistingTokenHandle, int SECURITY_IMPERSONATION_LEVEL, ref IntPtr DuplicateTokenHandle);
	}
}
