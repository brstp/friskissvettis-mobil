using EcmaScript.NET.Collections;
using EcmaScript.NET.Debugging;
using System;
namespace EcmaScript.NET
{
	internal sealed class InterpreterData : DebuggableScript
	{
		internal const int INITIAL_MAX_ICODE_LENGTH = 1024;
		internal const int INITIAL_STRINGTABLE_SIZE = 64;
		internal const int INITIAL_NUMBERTABLE_SIZE = 64;
		internal string itsName;
		internal string itsSourceFile;
		internal bool itsNeedsActivation;
		internal int itsFunctionType;
		internal string[] itsStringTable;
		internal double[] itsDoubleTable;
		internal InterpreterData[] itsNestedFunctions;
		internal object[] itsRegExpLiterals;
		internal sbyte[] itsICode;
		internal int[] itsExceptionTable;
		internal int itsMaxVars;
		internal int itsMaxLocals;
		internal int itsMaxStack;
		internal int itsMaxFrameArray;
		internal string[] argNames;
		internal int argCount;
		internal int itsMaxCalleeArgs;
		internal string encodedSource;
		internal int encodedSourceStart;
		internal int encodedSourceEnd;
		internal Context.Versions languageVersion;
		internal bool useDynamicScope;
		internal bool topLevel;
		internal object[] literalIds;
		internal UintMap longJumps;
		internal int firstLinePC = -1;
		internal InterpreterData parentData;
		internal bool evalScriptFlag;
		public bool TopLevel
		{
			get
			{
				return this.topLevel;
			}
		}
		public string FunctionName
		{
			get
			{
				return this.itsName;
			}
		}
		public int ParamCount
		{
			get
			{
				return this.argCount;
			}
		}
		public int ParamAndVarCount
		{
			get
			{
				return this.argNames.Length;
			}
		}
		public string SourceName
		{
			get
			{
				return this.itsSourceFile;
			}
		}
		public bool GeneratedScript
		{
			get
			{
				return ScriptRuntime.isGeneratedScript(this.itsSourceFile);
			}
		}
		public int[] LineNumbers
		{
			get
			{
				return Interpreter.getLineNumbers(this);
			}
		}
		public int FunctionCount
		{
			get
			{
				return (this.itsNestedFunctions == null) ? 0 : this.itsNestedFunctions.Length;
			}
		}
		public DebuggableScript Parent
		{
			get
			{
				return this.parentData;
			}
		}
		internal InterpreterData(Context.Versions languageVersion, string sourceFile, string encodedSource)
		{
			this.languageVersion = languageVersion;
			this.itsSourceFile = sourceFile;
			this.encodedSource = encodedSource;
			this.Init();
		}
		internal InterpreterData(InterpreterData parent)
		{
			this.parentData = parent;
			this.languageVersion = parent.languageVersion;
			this.itsSourceFile = parent.itsSourceFile;
			this.encodedSource = parent.encodedSource;
			this.Init();
		}
		private void Init()
		{
			this.itsICode = new sbyte[1024];
			this.itsStringTable = new string[64];
		}
		public bool IsFunction()
		{
			return this.itsFunctionType != 0;
		}
		public string GetParamOrVarName(int index)
		{
			return this.argNames[index];
		}
		public DebuggableScript GetFunction(int index)
		{
			return this.itsNestedFunctions[index];
		}
	}
}
