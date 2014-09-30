using System;
using System.Reflection;
namespace MoMA.Helpers
{
	public static class ObjectExtensions
	{
		public static string GetCurrentAssemblyLocation(this object obj)
		{
			return Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", "");
		}
	}
}
