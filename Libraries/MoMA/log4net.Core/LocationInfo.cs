using log4net.Util;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Security;
namespace log4net.Core
{
	[Serializable]
	public class LocationInfo
	{
		private const string NA = "?";
		private readonly string m_className;
		private readonly string m_fileName;
		private readonly string m_lineNumber;
		private readonly string m_methodName;
		private readonly string m_fullInfo;
		public string ClassName
		{
			get
			{
				return this.m_className;
			}
		}
		public string FileName
		{
			get
			{
				return this.m_fileName;
			}
		}
		public string LineNumber
		{
			get
			{
				return this.m_lineNumber;
			}
		}
		public string MethodName
		{
			get
			{
				return this.m_methodName;
			}
		}
		public string FullInfo
		{
			get
			{
				return this.m_fullInfo;
			}
		}
		public LocationInfo(Type callerStackBoundaryDeclaringType)
		{
			this.m_className = "?";
			this.m_fileName = "?";
			this.m_lineNumber = "?";
			this.m_methodName = "?";
			this.m_fullInfo = "?";
			if (callerStackBoundaryDeclaringType != null)
			{
				try
				{
					StackTrace stackTrace = new StackTrace(true);
					int i;
					for (i = 0; i < stackTrace.FrameCount; i++)
					{
						StackFrame frame = stackTrace.GetFrame(i);
						if (frame != null && frame.GetMethod().DeclaringType == callerStackBoundaryDeclaringType)
						{
							break;
						}
					}
					while (i < stackTrace.FrameCount)
					{
						StackFrame frame = stackTrace.GetFrame(i);
						if (frame != null && frame.GetMethod().DeclaringType != callerStackBoundaryDeclaringType)
						{
							break;
						}
						i++;
					}
					if (i < stackTrace.FrameCount)
					{
						StackFrame frame2 = stackTrace.GetFrame(i);
						if (frame2 != null)
						{
							MethodBase method = frame2.GetMethod();
							if (method != null)
							{
								this.m_methodName = method.Name;
								if (method.DeclaringType != null)
								{
									this.m_className = method.DeclaringType.FullName;
								}
							}
							this.m_fileName = frame2.GetFileName();
							this.m_lineNumber = frame2.GetFileLineNumber().ToString(NumberFormatInfo.InvariantInfo);
							this.m_fullInfo = string.Concat(new object[]
							{
								this.m_className,
								'.',
								this.m_methodName,
								'(',
								this.m_fileName,
								':',
								this.m_lineNumber,
								')'
							});
						}
					}
				}
				catch (SecurityException)
				{
					LogLog.Debug("LocationInfo: Security exception while trying to get caller stack frame. Error Ignored. Location Information Not Available.");
				}
			}
		}
		public LocationInfo(string className, string methodName, string fileName, string lineNumber)
		{
			this.m_className = className;
			this.m_fileName = fileName;
			this.m_lineNumber = lineNumber;
			this.m_methodName = methodName;
			this.m_fullInfo = string.Concat(new object[]
			{
				this.m_className,
				'.',
				this.m_methodName,
				'(',
				this.m_fileName,
				':',
				this.m_lineNumber,
				')'
			});
		}
	}
}
