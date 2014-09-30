using System;
using System.Configuration;
using System.Web.Configuration;
namespace MoMA.Helpers
{
	public class DebugHelper
	{
		public static bool IsWebConfigCompilationDebugMode
		{
			get
			{
				CompilationSection compilationSection = (CompilationSection)ConfigurationManager.GetSection("system.web/compilation");
				return compilationSection.Debug;
			}
		}
	}
}
