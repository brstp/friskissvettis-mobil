using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET.Debugging
{
	[ComVisible(true)]
	public interface DebuggableScript
	{
		bool TopLevel
		{
			get;
		}
		string FunctionName
		{
			get;
		}
		int ParamCount
		{
			get;
		}
		int ParamAndVarCount
		{
			get;
		}
		string SourceName
		{
			get;
		}
		bool GeneratedScript
		{
			get;
		}
		int[] LineNumbers
		{
			get;
		}
		int FunctionCount
		{
			get;
		}
		DebuggableScript Parent
		{
			get;
		}
		bool IsFunction();
		string GetParamOrVarName(int index);
		DebuggableScript GetFunction(int index);
	}
}
