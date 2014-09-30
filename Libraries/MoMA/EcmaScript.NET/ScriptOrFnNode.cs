using EcmaScript.NET.Collections;
using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public class ScriptOrFnNode : Node
	{
		private int encodedSourceStart;
		private int encodedSourceEnd;
		private string sourceName;
		private int baseLineno = -1;
		private int endLineno = -1;
		private ObjArray functions;
		private ObjArray regexps;
		private ObjArray itsVariables = new ObjArray();
		private ObjToIntMap itsVariableNames = new ObjToIntMap(11);
		private int varStart;
		private object compilerData;
		public virtual string SourceName
		{
			get
			{
				return this.sourceName;
			}
			set
			{
				this.sourceName = value;
			}
		}
		public virtual int EncodedSourceStart
		{
			get
			{
				return this.encodedSourceStart;
			}
		}
		public virtual int EncodedSourceEnd
		{
			get
			{
				return this.encodedSourceEnd;
			}
		}
		public virtual int BaseLineno
		{
			get
			{
				return this.baseLineno;
			}
			set
			{
				if (value < 0 || this.baseLineno >= 0)
				{
					Context.CodeBug();
				}
				this.baseLineno = value;
			}
		}
		public virtual int EndLineno
		{
			get
			{
				return this.baseLineno;
			}
			set
			{
				if (value < 0 || this.endLineno >= 0)
				{
					Context.CodeBug();
				}
				this.endLineno = value;
			}
		}
		public virtual int FunctionCount
		{
			get
			{
				int result;
				if (this.functions == null)
				{
					result = 0;
				}
				else
				{
					result = this.functions.size();
				}
				return result;
			}
		}
		public virtual int RegexpCount
		{
			get
			{
				int result;
				if (this.regexps == null)
				{
					result = 0;
				}
				else
				{
					result = this.regexps.size() / 2;
				}
				return result;
			}
		}
		public virtual int ParamCount
		{
			get
			{
				return this.varStart;
			}
		}
		public virtual int ParamAndVarCount
		{
			get
			{
				return this.itsVariables.size();
			}
		}
		public virtual string[] ParamAndVarNames
		{
			get
			{
				int num = this.itsVariables.size();
				string[] result;
				if (num == 0)
				{
					result = ScriptRuntime.EmptyStrings;
				}
				else
				{
					string[] array = new string[num];
					this.itsVariables.ToArray(array);
					result = array;
				}
				return result;
			}
		}
		public virtual object CompilerData
		{
			get
			{
				return this.compilerData;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				if (this.compilerData != null)
				{
					throw new ApplicationException();
				}
				this.compilerData = value;
			}
		}
		public ScriptOrFnNode(int nodeType) : base(nodeType)
		{
		}
		public void setEncodedSourceBounds(int start, int end)
		{
			this.encodedSourceStart = start;
			this.encodedSourceEnd = end;
		}
		public FunctionNode getFunctionNode(int i)
		{
			return (FunctionNode)this.functions.Get(i);
		}
		public int addFunction(FunctionNode fnNode)
		{
			if (fnNode == null)
			{
				Context.CodeBug();
			}
			if (this.functions == null)
			{
				this.functions = new ObjArray();
			}
			this.functions.add(fnNode);
			return this.functions.size() - 1;
		}
		public string getRegexpString(int index)
		{
			return (string)this.regexps.Get(index * 2);
		}
		public string getRegexpFlags(int index)
		{
			return (string)this.regexps.Get(index * 2 + 1);
		}
		public int addRegexp(string str, string flags)
		{
			if (str == null)
			{
				Context.CodeBug();
			}
			if (this.regexps == null)
			{
				this.regexps = new ObjArray();
			}
			this.regexps.add(str);
			this.regexps.add(flags);
			return this.regexps.size() / 2 - 1;
		}
		public bool hasParamOrVar(string name)
		{
			return this.itsVariableNames.has(name);
		}
		public int getParamOrVarIndex(string name)
		{
			return this.itsVariableNames.Get(name, -1);
		}
		public string getParamOrVarName(int index)
		{
			return (string)this.itsVariables.Get(index);
		}
		public void addParam(string name)
		{
			if (this.varStart != this.itsVariables.size())
			{
				Context.CodeBug();
			}
			int value = this.varStart++;
			this.itsVariables.add(name);
			this.itsVariableNames.put(name, value);
		}
		public void addVar(string name)
		{
			int num = this.itsVariableNames.Get(name, -1);
			if (num == -1)
			{
				int value = this.itsVariables.size();
				this.itsVariables.add(name);
				this.itsVariableNames.put(name, value);
			}
		}
		public void removeParamOrVar(string name)
		{
			int num = this.itsVariableNames.Get(name, -1);
			if (num != -1)
			{
				this.itsVariables.remove(num);
				this.itsVariableNames.remove(name);
				ObjToIntMap.Iterator iterator = this.itsVariableNames.newIterator();
				iterator.start();
				while (!iterator.done())
				{
					int value = iterator.Value;
					if (value > num)
					{
						iterator.Value = value - 1;
					}
					iterator.next();
				}
			}
		}
	}
}
