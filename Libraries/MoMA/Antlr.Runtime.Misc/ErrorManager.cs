using System;
using System.Diagnostics;
using System.Text;
namespace Antlr.Runtime.Misc
{
	public class ErrorManager
	{
		public static void InternalError(object error, Exception e)
		{
			StackFrame lastNonErrorManagerCodeLocation = ErrorManager.GetLastNonErrorManagerCodeLocation(e);
			string arg = string.Concat(new object[]
			{
				"Exception ",
				e,
				"@",
				lastNonErrorManagerCodeLocation,
				": ",
				error
			});
			ErrorManager.Error(arg);
		}
		public static void InternalError(object error)
		{
			StackFrame lastNonErrorManagerCodeLocation = ErrorManager.GetLastNonErrorManagerCodeLocation(new Exception());
			string arg = lastNonErrorManagerCodeLocation + ": " + error;
			ErrorManager.Error(arg);
		}
		private static StackFrame GetLastNonErrorManagerCodeLocation(Exception e)
		{
			StackTrace stackTrace = new StackTrace(e);
			int i;
			for (i = 0; i < stackTrace.FrameCount; i++)
			{
				StackFrame frame = stackTrace.GetFrame(i);
				if (frame.ToString().IndexOf("ErrorManager") < 0)
				{
					break;
				}
			}
			return stackTrace.GetFrame(i);
		}
		public static void Error(object arg)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("internal error: {0} ", arg);
		}
	}
}
