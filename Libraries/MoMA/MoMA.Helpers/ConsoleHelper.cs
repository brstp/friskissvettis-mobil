using System;
using System.Diagnostics;
using System.IO;
namespace MoMA.Helpers
{
	public class ConsoleHelper
	{
		public static string RunCmd(string command)
		{
			string result = string.Empty;
			using (Process process = Process.Start(new ProcessStartInfo(command)
			{
				UseShellExecute = false,
				RedirectStandardOutput = true
			}))
			{
				StreamReader standardOutput = process.StandardOutput;
				result = standardOutput.ReadToEnd();
			}
			return result;
		}
	}
}
