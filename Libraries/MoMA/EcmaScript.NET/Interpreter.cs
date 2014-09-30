using EcmaScript.NET.Collections;
using EcmaScript.NET.Debugging;
using EcmaScript.NET.Helpers;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public class Interpreter
	{
		internal class CallFrame : ICloneable
		{
			internal Interpreter.CallFrame parentFrame;
			internal int frameIndex;
			internal bool frozen;
			internal InterpretedFunction fnOrScript;
			internal InterpreterData idata;
			internal object[] stack;
			internal double[] sDbl;
			internal Interpreter.CallFrame varSource;
			internal int localShift;
			internal int emptyStackTop;
			internal DebugFrame debuggerFrame;
			internal bool useActivation;
			internal IScriptable thisObj;
			internal IScriptable[] scriptRegExps;
			internal object result;
			internal double resultDbl;
			internal int pc;
			internal int pcPrevBranch;
			internal int pcSourceLineStart;
			internal IScriptable scope;
			internal int savedStackTop;
			internal int savedCallOp;
			internal virtual Interpreter.CallFrame cloneFrozen()
			{
				if (!this.frozen)
				{
					Context.CodeBug();
				}
				Interpreter.CallFrame callFrame = (Interpreter.CallFrame)this.Clone();
				callFrame.stack = new object[this.stack.Length];
				this.stack.CopyTo(callFrame.stack, 0);
				callFrame.sDbl = new double[this.sDbl.Length];
				this.sDbl.CopyTo(callFrame.sDbl, 0);
				callFrame.frozen = false;
				return callFrame;
			}
			public virtual object Clone()
			{
				return base.MemberwiseClone();
			}
		}
		private sealed class ContinuationJump
		{
			internal Interpreter.CallFrame capturedFrame;
			internal Interpreter.CallFrame branchFrame;
			internal object result;
			internal double resultDbl;
			internal ContinuationJump(Continuation c, Interpreter.CallFrame current)
			{
				this.capturedFrame = (Interpreter.CallFrame)c.Implementation;
				if (this.capturedFrame == null || current == null)
				{
					this.branchFrame = null;
				}
				else
				{
					Interpreter.CallFrame callFrame = this.capturedFrame;
					Interpreter.CallFrame callFrame2 = current;
					int num = callFrame.frameIndex - callFrame2.frameIndex;
					if (num != 0)
					{
						if (num < 0)
						{
							callFrame = current;
							callFrame2 = this.capturedFrame;
							num = -num;
						}
						do
						{
							callFrame = callFrame.parentFrame;
						}
						while (--num != 0);
						if (callFrame.frameIndex != callFrame2.frameIndex)
						{
							Context.CodeBug();
						}
					}
					while (callFrame != callFrame2 && callFrame != null)
					{
						callFrame = callFrame.parentFrame;
						callFrame2 = callFrame2.parentFrame;
					}
					this.branchFrame = callFrame;
					if (this.branchFrame != null && !this.branchFrame.frozen)
					{
						Context.CodeBug();
					}
				}
			}
		}
		private const int Icode_DUP = -1;
		private const int Icode_DUP2 = -2;
		private const int Icode_SWAP = -3;
		private const int Icode_POP = -4;
		private const int Icode_POP_RESULT = -5;
		private const int Icode_IFEQ_POP = -6;
		private const int Icode_VAR_INC_DEC = -7;
		private const int Icode_NAME_INC_DEC = -8;
		private const int Icode_PROP_INC_DEC = -9;
		private const int Icode_ELEM_INC_DEC = -10;
		private const int Icode_REF_INC_DEC = -11;
		private const int Icode_SCOPE_LOAD = -12;
		private const int Icode_SCOPE_SAVE = -13;
		private const int Icode_TYPEOFNAME = -14;
		private const int Icode_NAME_AND_THIS = -15;
		private const int Icode_PROP_AND_THIS = -16;
		private const int Icode_ELEM_AND_THIS = -17;
		private const int Icode_VALUE_AND_THIS = -18;
		private const int Icode_CLOSURE_EXPR = -19;
		private const int Icode_CLOSURE_STMT = -20;
		private const int Icode_CALLSPECIAL = -21;
		private const int Icode_RETUNDEF = -22;
		private const int Icode_GOSUB = -23;
		private const int Icode_STARTSUB = -24;
		private const int Icode_RETSUB = -25;
		private const int Icode_LINE = -26;
		private const int Icode_SHORTNUMBER = -27;
		private const int Icode_INTNUMBER = -28;
		private const int Icode_LITERAL_NEW = -29;
		private const int Icode_LITERAL_SET = -30;
		private const int Icode_SPARE_ARRAYLIT = -31;
		private const int Icode_REG_IND_C0 = -32;
		private const int Icode_REG_IND_C1 = -33;
		private const int Icode_REG_IND_C2 = -34;
		private const int Icode_REG_IND_C3 = -35;
		private const int Icode_REG_IND_C4 = -36;
		private const int Icode_REG_IND_C5 = -37;
		private const int Icode_REG_IND1 = -38;
		private const int Icode_REG_IND2 = -39;
		private const int Icode_REG_IND4 = -40;
		private const int Icode_REG_STR_C0 = -41;
		private const int Icode_REG_STR_C1 = -42;
		private const int Icode_REG_STR_C2 = -43;
		private const int Icode_REG_STR_C3 = -44;
		private const int Icode_REG_STR1 = -45;
		private const int Icode_REG_STR2 = -46;
		private const int Icode_REG_STR4 = -47;
		private const int Icode_GETVAR1 = -48;
		private const int Icode_SETVAR1 = -49;
		private const int Icode_UNDEF = -50;
		private const int Icode_ZERO = -51;
		private const int Icode_ONE = -52;
		private const int Icode_ENTERDQ = -53;
		private const int Icode_LEAVEDQ = -54;
		private const int Icode_TAIL_CALL = -55;
		private const int Icode_LOCAL_CLEAR = -56;
		private const int Icode_DEBUGGER = -57;
		private const int MIN_ICODE = -57;
		private const int MIN_LABEL_TABLE_SIZE = 32;
		private const int MIN_FIXUP_TABLE_SIZE = 40;
		private const int EXCEPTION_TRY_START_SLOT = 0;
		private const int EXCEPTION_TRY_END_SLOT = 1;
		private const int EXCEPTION_HANDLER_SLOT = 2;
		private const int EXCEPTION_TYPE_SLOT = 3;
		private const int EXCEPTION_LOCAL_SLOT = 4;
		private const int EXCEPTION_SCOPE_SLOT = 5;
		private const int EXCEPTION_SLOT_SIZE = 6;
		private const int ECF_TAIL = 1;
		private CompilerEnvirons compilerEnv;
		private bool itsInFunctionFlag;
		private InterpreterData itsData;
		private ScriptOrFnNode scriptOrFn;
		private int itsICodeTop;
		private int itsStackDepth;
		private int itsLineNumber;
		private int itsDoubleTableTop;
		private ObjToIntMap itsStrings = new ObjToIntMap(20);
		private int itsLocalTop;
		private int[] itsLabelTable;
		private int itsLabelTableTop;
		private long[] itsFixupTable;
		private int itsFixupTableTop;
		private ObjArray itsLiteralIds = new ObjArray();
		private int itsExceptionTableTop;
		private static string bytecodeName(int bytecode)
		{
			if (!Interpreter.validBytecode(bytecode))
			{
				throw new ArgumentException(Convert.ToString(bytecode));
			}
			string result;
			if (!Token.printICode)
			{
				result = Convert.ToString(bytecode);
			}
			else
			{
				if (Interpreter.ValidTokenCode(bytecode))
				{
					result = Token.name(bytecode);
				}
				else
				{
					switch (bytecode)
					{
					case -57:
						result = "DEBUGGER";
						break;
					case -56:
						result = "LOCAL_CLEAR";
						break;
					case -55:
						result = "TAIL_CALL";
						break;
					case -54:
						result = "LEAVEDQ";
						break;
					case -53:
						result = "ENTERDQ";
						break;
					case -52:
						result = "ONE";
						break;
					case -51:
						result = "ZERO";
						break;
					case -50:
						result = "UNDEF";
						break;
					case -49:
						result = "SETVAR1";
						break;
					case -48:
						result = "GETVAR1";
						break;
					case -47:
						result = "LOAD_STR4";
						break;
					case -46:
						result = "LOAD_STR2";
						break;
					case -45:
						result = "LOAD_STR1";
						break;
					case -44:
						result = "REG_STR_C3";
						break;
					case -43:
						result = "REG_STR_C2";
						break;
					case -42:
						result = "REG_STR_C1";
						break;
					case -41:
						result = "REG_STR_C0";
						break;
					case -40:
						result = "LOAD_IND4";
						break;
					case -39:
						result = "LOAD_IND2";
						break;
					case -38:
						result = "LOAD_IND1";
						break;
					case -37:
						result = "REG_IND_C5";
						break;
					case -36:
						result = "REG_IND_C4";
						break;
					case -35:
						result = "REG_IND_C3";
						break;
					case -34:
						result = "REG_IND_C2";
						break;
					case -33:
						result = "REG_IND_C1";
						break;
					case -32:
						result = "REG_IND_C0";
						break;
					case -31:
						result = "SPARE_ARRAYLIT";
						break;
					case -30:
						result = "LITERAL_SET";
						break;
					case -29:
						result = "LITERAL_NEW";
						break;
					case -28:
						result = "INTNUMBER";
						break;
					case -27:
						result = "SHORTNUMBER";
						break;
					case -26:
						result = "LINE";
						break;
					case -25:
						result = "RETSUB";
						break;
					case -24:
						result = "STARTSUB";
						break;
					case -23:
						result = "GOSUB";
						break;
					case -22:
						result = "RETUNDEF";
						break;
					case -21:
						result = "CALLSPECIAL";
						break;
					case -20:
						result = "CLOSURE_STMT";
						break;
					case -19:
						result = "CLOSURE_EXPR";
						break;
					case -18:
						result = "VALUE_AND_THIS";
						break;
					case -17:
						result = "ELEM_AND_THIS";
						break;
					case -16:
						result = "PROP_AND_THIS";
						break;
					case -15:
						result = "NAME_AND_THIS";
						break;
					case -14:
						result = "TYPEOFNAME";
						break;
					case -13:
						result = "SCOPE_SAVE";
						break;
					case -12:
						result = "SCOPE_LOAD";
						break;
					case -11:
						result = "REF_INC_DEC";
						break;
					case -10:
						result = "ELEM_INC_DEC";
						break;
					case -9:
						result = "PROP_INC_DEC";
						break;
					case -8:
						result = "NAME_INC_DEC";
						break;
					case -7:
						result = "VAR_INC_DEC";
						break;
					case -6:
						result = "IFEQ_POP";
						break;
					case -5:
						result = "POP_RESULT";
						break;
					case -4:
						result = "POP";
						break;
					case -3:
						result = "SWAP";
						break;
					case -2:
						result = "DUP2";
						break;
					case -1:
						result = "DUP";
						break;
					default:
						throw new ApplicationException(Convert.ToString(bytecode));
					}
				}
			}
			return result;
		}
		private static bool validIcode(int icode)
		{
			return -57 <= icode && icode <= -1;
		}
		private static bool ValidTokenCode(int token)
		{
			return 2 <= token && token <= 78;
		}
		private static bool validBytecode(int bytecode)
		{
			return Interpreter.validIcode(bytecode) || Interpreter.ValidTokenCode(bytecode);
		}
		public virtual object Compile(CompilerEnvirons compilerEnv, ScriptOrFnNode tree, string encodedSource, bool returnFunction)
		{
			this.compilerEnv = compilerEnv;
			new NodeTransformer().transform(tree);
			if (Token.printTrees)
			{
				Console.Out.WriteLine(tree.toStringTree(tree));
			}
			if (returnFunction)
			{
				tree = tree.getFunctionNode(0);
			}
			this.scriptOrFn = tree;
			this.itsData = new InterpreterData(compilerEnv.LanguageVersion, this.scriptOrFn.SourceName, encodedSource);
			this.itsData.topLevel = true;
			if (returnFunction)
			{
				this.generateFunctionICode();
			}
			else
			{
				this.generateICodeFromTree(this.scriptOrFn);
			}
			return this.itsData;
		}
		public virtual IScript CreateScriptObject(object bytecode, object staticSecurityDomain)
		{
			InterpreterData interpreterData = (InterpreterData)bytecode;
			return InterpretedFunction.createScript(this.itsData, staticSecurityDomain);
		}
		public virtual IFunction CreateFunctionObject(Context cx, IScriptable scope, object bytecode, object staticSecurityDomain)
		{
			InterpreterData interpreterData = (InterpreterData)bytecode;
			return InterpretedFunction.createFunction(cx, scope, this.itsData, staticSecurityDomain);
		}
		private void generateFunctionICode()
		{
			this.itsInFunctionFlag = true;
			FunctionNode functionNode = (FunctionNode)this.scriptOrFn;
			this.itsData.itsFunctionType = functionNode.FunctionType;
			this.itsData.itsNeedsActivation = functionNode.RequiresActivation;
			this.itsData.itsName = functionNode.FunctionName;
			if (!functionNode.IgnoreDynamicScope)
			{
				if (this.compilerEnv.UseDynamicScope)
				{
					this.itsData.useDynamicScope = true;
				}
			}
			this.generateICodeFromTree(functionNode.LastChild);
		}
		private void generateICodeFromTree(Node tree)
		{
			this.generateNestedFunctions();
			this.generateRegExpLiterals();
			this.VisitStatement(tree);
			this.fixLabelGotos();
			if (this.itsData.itsFunctionType == 0)
			{
				this.addToken(62);
			}
			if (this.itsData.itsICode.Length != this.itsICodeTop)
			{
				sbyte[] array = new sbyte[this.itsICodeTop];
				Array.Copy(this.itsData.itsICode, 0, array, 0, this.itsICodeTop);
				this.itsData.itsICode = array;
			}
			if (this.itsStrings.size() == 0)
			{
				this.itsData.itsStringTable = null;
			}
			else
			{
				this.itsData.itsStringTable = new string[this.itsStrings.size()];
				ObjToIntMap.Iterator iterator = this.itsStrings.newIterator();
				iterator.start();
				while (!iterator.done())
				{
					string text = (string)iterator.Key;
					int value = iterator.Value;
					if (this.itsData.itsStringTable[value] != null)
					{
						Context.CodeBug();
					}
					this.itsData.itsStringTable[value] = text;
					iterator.next();
				}
			}
			if (this.itsDoubleTableTop == 0)
			{
				this.itsData.itsDoubleTable = null;
			}
			else
			{
				if (this.itsData.itsDoubleTable.Length != this.itsDoubleTableTop)
				{
					double[] array2 = new double[this.itsDoubleTableTop];
					Array.Copy(this.itsData.itsDoubleTable, 0, array2, 0, this.itsDoubleTableTop);
					this.itsData.itsDoubleTable = array2;
				}
			}
			if (this.itsExceptionTableTop != 0 && this.itsData.itsExceptionTable.Length != this.itsExceptionTableTop)
			{
				int[] array3 = new int[this.itsExceptionTableTop];
				Array.Copy(this.itsData.itsExceptionTable, 0, array3, 0, this.itsExceptionTableTop);
				this.itsData.itsExceptionTable = array3;
			}
			this.itsData.itsMaxVars = this.scriptOrFn.ParamAndVarCount;
			this.itsData.itsMaxFrameArray = this.itsData.itsMaxVars + this.itsData.itsMaxLocals + this.itsData.itsMaxStack;
			this.itsData.argNames = this.scriptOrFn.ParamAndVarNames;
			this.itsData.argCount = this.scriptOrFn.ParamCount;
			this.itsData.encodedSourceStart = this.scriptOrFn.EncodedSourceStart;
			this.itsData.encodedSourceEnd = this.scriptOrFn.EncodedSourceEnd;
			if (this.itsLiteralIds.size() != 0)
			{
				this.itsData.literalIds = this.itsLiteralIds.ToArray();
			}
			if (Token.printICode)
			{
				Interpreter.dumpICode(this.itsData);
			}
		}
		private void generateNestedFunctions()
		{
			int functionCount = this.scriptOrFn.FunctionCount;
			if (functionCount != 0)
			{
				InterpreterData[] array = new InterpreterData[functionCount];
				for (int num = 0; num != functionCount; num++)
				{
					FunctionNode functionNode = this.scriptOrFn.getFunctionNode(num);
					Interpreter interpreter = new Interpreter();
					interpreter.compilerEnv = this.compilerEnv;
					interpreter.scriptOrFn = functionNode;
					interpreter.itsData = new InterpreterData(this.itsData);
					interpreter.generateFunctionICode();
					array[num] = interpreter.itsData;
				}
				this.itsData.itsNestedFunctions = array;
			}
		}
		private void generateRegExpLiterals()
		{
			int regexpCount = this.scriptOrFn.RegexpCount;
			if (regexpCount != 0)
			{
				Context currentContext = Context.CurrentContext;
				RegExpProxy regExpProxy = currentContext.RegExpProxy;
				object[] array = new object[regexpCount];
				for (int num = 0; num != regexpCount; num++)
				{
					string regexpString = this.scriptOrFn.getRegexpString(num);
					string regexpFlags = this.scriptOrFn.getRegexpFlags(num);
					array[num] = regExpProxy.Compile(currentContext, regexpString, regexpFlags);
				}
				this.itsData.itsRegExpLiterals = array;
			}
		}
		private void updateLineNumber(Node node)
		{
			int lineno = node.Lineno;
			if (lineno != this.itsLineNumber && lineno >= 0)
			{
				if (this.itsData.firstLinePC < 0)
				{
					this.itsData.firstLinePC = lineno;
				}
				this.itsLineNumber = lineno;
				this.addIcode(-26);
				this.addUint16(lineno & 65535);
			}
		}
		private ApplicationException badTree(Node node)
		{
			throw new ApplicationException(node.ToString());
		}
		private void VisitStatement(Node node)
		{
			int type = node.Type;
			Node node2 = node.FirstChild;
			int num = type;
			if (num <= 79)
			{
				if (num <= 50)
				{
					switch (num)
					{
					case 2:
						this.VisitExpression(node2, 0);
						this.addToken(2);
						this.stackChange(-1);
						goto IL_649;
					case 3:
						this.addToken(3);
						goto IL_649;
					case 4:
						this.updateLineNumber(node);
						if (node2 != null)
						{
							this.VisitExpression(node2, 1);
							this.addToken(4);
							this.stackChange(-1);
						}
						else
						{
							this.addIcode(-22);
						}
						goto IL_649;
					case 5:
					{
						Node target = ((Node.Jump)node).target;
						this.addGoto(target, type);
						goto IL_649;
					}
					case 6:
					case 7:
					{
						Node target = ((Node.Jump)node).target;
						this.VisitExpression(node2, 0);
						this.addGoto(target, type);
						this.stackChange(-1);
						goto IL_649;
					}
					default:
						switch (num)
						{
						case 49:
							this.updateLineNumber(node);
							this.VisitExpression(node2, 0);
							this.addToken(49);
							this.addUint16(this.itsLineNumber & 65535);
							this.stackChange(-1);
							goto IL_649;
						case 50:
							this.updateLineNumber(node);
							this.addIndexOp(50, this.getLocalBlockRef(node));
							goto IL_649;
						}
						break;
					}
				}
				else
				{
					switch (num)
					{
					case 56:
					{
						int localBlockRef = this.getLocalBlockRef(node);
						int existingIntProp = node.getExistingIntProp(14);
						string @string = node2.String;
						node2 = node2.Next;
						this.VisitExpression(node2, 0);
						this.addStringPrefix(@string);
						this.addIndexPrefix(localBlockRef);
						this.addToken(56);
						this.addUint8((existingIntProp != 0) ? 1 : 0);
						this.stackChange(-1);
						goto IL_649;
					}
					case 57:
					case 58:
						this.VisitExpression(node2, 0);
						this.addIndexOp(type, this.getLocalBlockRef(node));
						this.stackChange(-1);
						goto IL_649;
					case 59:
					case 60:
					case 61:
						break;
					case 62:
						this.updateLineNumber(node);
						this.addToken(62);
						goto IL_649;
					default:
						if (num == 79)
						{
							Node.Jump jump = (Node.Jump)node;
							int localBlockRef2 = this.getLocalBlockRef(jump);
							int num2 = this.allocLocal();
							this.addIndexOp(-13, num2);
							int icodeStart = this.itsICodeTop;
							while (node2 != null)
							{
								this.VisitStatement(node2);
								node2 = node2.Next;
							}
							Node target2 = jump.target;
							if (target2 != null)
							{
								int num3 = this.itsLabelTable[this.getTargetLabel(target2)];
								this.addExceptionHandler(icodeStart, num3, num3, false, localBlockRef2, num2);
							}
							Node @finally = jump.Finally;
							if (@finally != null)
							{
								int num4 = this.itsLabelTable[this.getTargetLabel(@finally)];
								this.addExceptionHandler(icodeStart, num4, num4, true, localBlockRef2, num2);
							}
							this.addIndexOp(-56, num2);
							this.releaseLocal(num2);
							goto IL_649;
						}
						break;
					}
				}
			}
			else
			{
				if (num <= 134)
				{
					if (num == 107)
					{
						int existingIntProp2 = node.getExistingIntProp(1);
						int functionType = this.scriptOrFn.getFunctionNode(existingIntProp2).FunctionType;
						if (functionType == 3)
						{
							this.addIndexOp(-20, existingIntProp2);
						}
						else
						{
							if (functionType != 1)
							{
								throw Context.CodeBug();
							}
						}
						goto IL_649;
					}
					switch (num)
					{
					case 112:
					{
						this.updateLineNumber(node);
						Node node3 = (Node.Jump)node;
						this.VisitExpression(node2, 0);
						for (Node.Jump jump2 = (Node.Jump)node2.Next; jump2 != null; jump2 = (Node.Jump)jump2.Next)
						{
							if (jump2.Type != 113)
							{
								throw this.badTree(jump2);
							}
							Node firstChild = jump2.FirstChild;
							this.addIcode(-1);
							this.stackChange(1);
							this.VisitExpression(firstChild, 0);
							this.addToken(45);
							this.stackChange(-1);
							this.addGoto(jump2.target, -6);
							this.stackChange(-1);
						}
						this.addIcode(-4);
						this.stackChange(-1);
						goto IL_649;
					}
					case 121:
					case 126:
					case 127:
					case 128:
					case 130:
					case 134:
						this.updateLineNumber(node);
						while (node2 != null)
						{
							this.VisitStatement(node2);
							node2 = node2.Next;
						}
						goto IL_649;
					case 123:
					{
						this.stackChange(1);
						int localBlockRef3 = this.getLocalBlockRef(node);
						this.addIndexOp(-24, localBlockRef3);
						this.stackChange(-1);
						while (node2 != null)
						{
							this.VisitStatement(node2);
							node2 = node2.Next;
						}
						this.addIndexOp(-25, localBlockRef3);
						goto IL_649;
					}
					case 129:
						this.markTargetLabel(node);
						goto IL_649;
					case 131:
					case 132:
						this.updateLineNumber(node);
						this.VisitExpression(node2, 0);
						this.addIcode((type == 131) ? -4 : -5);
						this.stackChange(-1);
						goto IL_649;
					case 133:
					{
						Node target = ((Node.Jump)node).target;
						this.addGoto(target, -23);
						goto IL_649;
					}
					}
				}
				else
				{
					if (num == 139)
					{
						int num5 = this.allocLocal();
						node.putIntProp(2, num5);
						this.updateLineNumber(node);
						while (node2 != null)
						{
							this.VisitStatement(node2);
							node2 = node2.Next;
						}
						this.addIndexOp(-56, num5);
						this.releaseLocal(num5);
						goto IL_649;
					}
					if (num == 156)
					{
						this.updateLineNumber(node);
						this.addIcode(-57);
						goto IL_649;
					}
				}
			}
			throw this.badTree(node);
			IL_649:
			if (this.itsStackDepth != 0)
			{
				throw Context.CodeBug();
			}
		}
		private bool VisitExpressionOptimized(Node node, int contextFlags)
		{
			return false;
		}
		private void VisitExpression(Node node, int contextFlags)
		{
			if (!this.VisitExpressionOptimized(node, contextFlags))
			{
				int num = node.Type;
				Node node2 = node.FirstChild;
				int num2 = this.itsStackDepth;
				switch (num)
				{
				case 8:
				{
					string @string = node2.String;
					this.VisitExpression(node2, 0);
					node2 = node2.Next;
					this.VisitExpression(node2, 0);
					this.addStringOp(8, @string);
					this.stackChange(-1);
					return;
				}
				case 9:
				case 10:
				case 11:
				case 12:
				case 13:
				case 14:
				case 15:
				case 16:
				case 17:
				case 18:
				case 19:
				case 20:
				case 21:
				case 22:
				case 23:
				case 24:
				case 25:
				case 31:
				case 35:
				case 45:
				case 46:
				case 51:
				case 52:
					this.VisitExpression(node2, 0);
					this.VisitExpression(node2.Next, 0);
					this.addToken(num);
					this.stackChange(-1);
					return;
				case 26:
				case 27:
				case 28:
				case 29:
				case 32:
				case 124:
					this.VisitExpression(node2, 0);
					if (num == 124)
					{
						this.addIcode(-4);
						this.addIcode(-50);
					}
					else
					{
						this.addToken(num);
					}
					return;
				case 30:
				case 37:
				case 68:
				{
					if (num == 30)
					{
						this.VisitExpression(node2, 0);
					}
					else
					{
						this.generateCallFunAndThis(node2);
					}
					int num3 = 0;
					while ((node2 = node2.Next) != null)
					{
						this.VisitExpression(node2, 0);
						num3++;
					}
					int intProp = node.getIntProp(10, 0);
					if (intProp != 0)
					{
						this.addIndexOp(-21, num3);
						this.addUint8(intProp);
						this.addUint8((num == 30) ? 1 : 0);
						this.addUint16(this.itsLineNumber & 65535);
					}
					else
					{
						if (num == 37)
						{
							if ((contextFlags & 1) != 0)
							{
								num = -55;
							}
						}
						this.addIndexOp(num, num3);
					}
					if (num == 30)
					{
						this.stackChange(-num3);
					}
					else
					{
						this.stackChange(-1 - num3);
					}
					if (num3 > this.itsData.itsMaxCalleeArgs)
					{
						this.itsData.itsMaxCalleeArgs = num3;
					}
					return;
				}
				case 33:
					this.VisitExpression(node2, 0);
					node2 = node2.Next;
					this.addStringOp(33, node2.String);
					return;
				case 34:
				case 77:
				case 78:
				case 137:
				{
					this.VisitExpression(node2, 0);
					node2 = node2.Next;
					string string2 = node2.String;
					node2 = node2.Next;
					if (num == 137)
					{
						this.addIcode(-1);
						this.stackChange(1);
						this.addStringOp(33, string2);
						this.stackChange(-1);
					}
					this.VisitExpression(node2, 0);
					this.addStringOp((num == 137) ? 34 : num, string2);
					this.stackChange(-1);
					return;
				}
				case 36:
				case 138:
					this.VisitExpression(node2, 0);
					node2 = node2.Next;
					this.VisitExpression(node2, 0);
					node2 = node2.Next;
					if (num == 138)
					{
						this.addIcode(-2);
						this.stackChange(2);
						this.addToken(35);
						this.stackChange(-1);
						this.stackChange(-1);
					}
					this.VisitExpression(node2, 0);
					this.addToken(36);
					this.stackChange(-2);
					return;
				case 38:
				case 40:
				case 48:
					this.addStringOp(num, node.String);
					this.stackChange(1);
					return;
				case 39:
				{
					double @double = node.Double;
					int num4 = (int)@double;
					if ((double)num4 == @double)
					{
						if (num4 == 0)
						{
							this.addIcode(-51);
							if (1.0 / @double < 0.0)
							{
								this.addToken(29);
							}
						}
						else
						{
							if (num4 == 1)
							{
								this.addIcode(-52);
							}
							else
							{
								if ((int)((short)num4) == num4)
								{
									this.addIcode(-27);
									this.addUint16(num4 & 65535);
								}
								else
								{
									this.addIcode(-28);
									this.addInt(num4);
								}
							}
						}
					}
					else
					{
						int num5 = this.GetDoubleIndex(@double);
						this.addIndexOp(39, num5);
					}
					this.stackChange(1);
					return;
				}
				case 41:
				case 42:
				case 43:
				case 44:
				case 61:
					this.addToken(num);
					this.stackChange(1);
					return;
				case 47:
				{
					int num5 = node.getExistingIntProp(4);
					this.addIndexOp(47, num5);
					this.stackChange(1);
					return;
				}
				case 53:
				{
					int localBlockRef = this.getLocalBlockRef(node);
					this.addIndexOp(53, localBlockRef);
					this.stackChange(1);
					return;
				}
				case 54:
				{
					if (this.itsData.itsNeedsActivation)
					{
						Context.CodeBug();
					}
					string @string = node.String;
					int num5 = this.scriptOrFn.getParamOrVarIndex(@string);
					this.addVarOp(54, num5);
					this.stackChange(1);
					return;
				}
				case 55:
				{
					if (this.itsData.itsNeedsActivation)
					{
						Context.CodeBug();
					}
					string @string = node2.String;
					node2 = node2.Next;
					this.VisitExpression(node2, 0);
					int num5 = this.scriptOrFn.getParamOrVarIndex(@string);
					this.addVarOp(55, num5);
					return;
				}
				case 59:
				case 60:
					this.addIndexOp(num, this.getLocalBlockRef(node));
					this.stackChange(1);
					return;
				case 63:
				case 64:
					this.VisitLiteral(node, node2);
					return;
				case 65:
				case 67:
					this.VisitExpression(node2, 0);
					this.addToken(num);
					return;
				case 66:
				case 140:
					this.VisitExpression(node2, 0);
					node2 = node2.Next;
					if (num == 140)
					{
						this.addIcode(-1);
						this.stackChange(1);
						this.addToken(65);
						this.stackChange(-1);
					}
					this.VisitExpression(node2, 0);
					this.addToken(66);
					this.stackChange(-1);
					return;
				case 69:
					this.VisitExpression(node2, 0);
					this.addStringOp(num, (string)node.getProp(17));
					return;
				case 70:
				case 71:
				case 72:
					this.VisitExpression(node2, 0);
					this.addToken(num);
					return;
				case 73:
				case 74:
				case 75:
				case 76:
				{
					int intProp2 = node.getIntProp(16, 0);
					int num6 = 0;
					do
					{
						this.VisitExpression(node2, 0);
						num6++;
						node2 = node2.Next;
					}
					while (node2 != null);
					this.addIndexOp(num, intProp2);
					this.stackChange(1 - num6);
					return;
				}
				case 87:
				{
					Node lastChild = node.LastChild;
					while (node2 != lastChild)
					{
						this.VisitExpression(node2, 0);
						this.addIcode(-4);
						this.stackChange(-1);
						node2 = node2.Next;
					}
					this.VisitExpression(node2, contextFlags & 1);
					return;
				}
				case 100:
				{
					Node next = node2.Next;
					Node next2 = next.Next;
					this.VisitExpression(node2, 0);
					int fromPC = this.itsICodeTop;
					this.addGotoOp(7);
					this.stackChange(-1);
					this.VisitExpression(next, contextFlags & 1);
					int fromPC2 = this.itsICodeTop;
					this.addGotoOp(5);
					this.resolveForwardGoto(fromPC);
					this.itsStackDepth = num2;
					this.VisitExpression(next2, contextFlags & 1);
					this.resolveForwardGoto(fromPC2);
					return;
				}
				case 102:
				case 103:
				{
					this.VisitExpression(node2, 0);
					this.addIcode(-1);
					this.stackChange(1);
					int fromPC3 = this.itsICodeTop;
					int gotoOp = (num == 103) ? 7 : 6;
					this.addGotoOp(gotoOp);
					this.stackChange(-1);
					this.addIcode(-4);
					this.stackChange(-1);
					node2 = node2.Next;
					this.VisitExpression(node2, contextFlags & 1);
					this.resolveForwardGoto(fromPC3);
					return;
				}
				case 104:
				case 105:
					this.VisitIncDec(node, node2);
					return;
				case 107:
				{
					int existingIntProp = node.getExistingIntProp(1);
					FunctionNode functionNode = this.scriptOrFn.getFunctionNode(existingIntProp);
					int functionType = functionNode.FunctionType;
					if (functionType != 2)
					{
						throw Context.CodeBug();
					}
					this.addIndexOp(-19, existingIntProp);
					this.stackChange(1);
					return;
				}
				case 135:
				{
					string @string = node.String;
					int num5 = -1;
					if (this.itsInFunctionFlag && !this.itsData.itsNeedsActivation)
					{
						num5 = this.scriptOrFn.getParamOrVarIndex(@string);
					}
					if (num5 == -1)
					{
						this.addStringOp(-14, @string);
						this.stackChange(1);
					}
					else
					{
						this.addVarOp(54, num5);
						this.stackChange(1);
						this.addToken(32);
					}
					return;
				}
				case 136:
					this.stackChange(1);
					return;
				case 144:
				{
					this.updateLineNumber(node);
					this.VisitExpression(node2, 0);
					this.addIcode(-53);
					this.stackChange(-1);
					int jumpPC = this.itsICodeTop;
					this.VisitExpression(node2.Next, 0);
					this.addBackwardGoto(-54, jumpPC);
					return;
				}
				}
				throw this.badTree(node);
			}
		}
		private void generateCallFunAndThis(Node left)
		{
			int type = left.Type;
			int num = type;
			switch (num)
			{
			case 33:
			case 35:
			{
				Node firstChild = left.FirstChild;
				this.VisitExpression(firstChild, 0);
				Node next = firstChild.Next;
				if (type == 33)
				{
					string @string = next.String;
					this.addStringOp(-16, @string);
					this.stackChange(1);
				}
				else
				{
					this.VisitExpression(next, 0);
					this.addIcode(-17);
				}
				return;
			}
			case 34:
				break;
			default:
				if (num == 38)
				{
					string string2 = left.String;
					this.addStringOp(-15, string2);
					this.stackChange(2);
					return;
				}
				break;
			}
			this.VisitExpression(left, 0);
			this.addIcode(-18);
			this.stackChange(1);
		}
		private void VisitIncDec(Node node, Node child)
		{
			int existingIntProp = node.getExistingIntProp(13);
			int type = child.Type;
			int num = type;
			if (num <= 38)
			{
				switch (num)
				{
				case 33:
				{
					Node firstChild = child.FirstChild;
					this.VisitExpression(firstChild, 0);
					string @string = firstChild.Next.String;
					this.addStringOp(-9, @string);
					this.addUint8(existingIntProp);
					return;
				}
				case 34:
					break;
				case 35:
				{
					Node firstChild = child.FirstChild;
					this.VisitExpression(firstChild, 0);
					Node next = firstChild.Next;
					this.VisitExpression(next, 0);
					this.addIcode(-10);
					this.addUint8(existingIntProp);
					this.stackChange(-1);
					return;
				}
				default:
					if (num == 38)
					{
						string string2 = child.String;
						this.addStringOp(-8, string2);
						this.addUint8(existingIntProp);
						this.stackChange(1);
						return;
					}
					break;
				}
			}
			else
			{
				if (num == 54)
				{
					if (this.itsData.itsNeedsActivation)
					{
						Context.CodeBug();
					}
					string string2 = child.String;
					int paramOrVarIndex = this.scriptOrFn.getParamOrVarIndex(string2);
					this.addVarOp(-7, paramOrVarIndex);
					this.addUint8(existingIntProp);
					this.stackChange(1);
					return;
				}
				if (num == 65)
				{
					Node firstChild2 = child.FirstChild;
					this.VisitExpression(firstChild2, 0);
					this.addIcode(-11);
					this.addUint8(existingIntProp);
					return;
				}
			}
			throw this.badTree(node);
		}
		private void VisitLiteral(Node node, Node child)
		{
			int type = node.Type;
			object[] array = null;
			int num;
			if (type == 63)
			{
				num = 0;
				for (Node node2 = child; node2 != null; node2 = node2.Next)
				{
					num++;
				}
			}
			else
			{
				if (type != 64)
				{
					throw this.badTree(node);
				}
				array = (object[])node.getProp(12);
				num = array.Length;
			}
			this.addIndexOp(-29, num);
			this.stackChange(1);
			while (child != null)
			{
				this.VisitExpression(child, 0);
				this.addIcode(-30);
				this.stackChange(-1);
				child = child.Next;
			}
			if (type == 63)
			{
				int[] array2 = (int[])node.getProp(11);
				if (array2 == null)
				{
					this.addToken(63);
				}
				else
				{
					int index = this.itsLiteralIds.size();
					this.itsLiteralIds.add(array2);
					this.addIndexOp(-31, index);
				}
			}
			else
			{
				int index = this.itsLiteralIds.size();
				this.itsLiteralIds.add(array);
				this.addIndexOp(64, index);
			}
		}
		private int getLocalBlockRef(Node node)
		{
			Node node2 = (Node)node.getProp(3);
			return node2.getExistingIntProp(2);
		}
		private int getTargetLabel(Node target)
		{
			int num = target.labelId();
			int result;
			if (num != -1)
			{
				result = num;
			}
			else
			{
				num = this.itsLabelTableTop;
				if (this.itsLabelTable == null || num == this.itsLabelTable.Length)
				{
					if (this.itsLabelTable == null)
					{
						this.itsLabelTable = new int[32];
					}
					else
					{
						int[] destinationArray = new int[this.itsLabelTable.Length * 2];
						Array.Copy(this.itsLabelTable, 0, destinationArray, 0, num);
						this.itsLabelTable = destinationArray;
					}
				}
				this.itsLabelTableTop = num + 1;
				this.itsLabelTable[num] = -1;
				target.labelId(num);
				result = num;
			}
			return result;
		}
		private void markTargetLabel(Node target)
		{
			int targetLabel = this.getTargetLabel(target);
			if (this.itsLabelTable[targetLabel] != -1)
			{
				Context.CodeBug();
			}
			this.itsLabelTable[targetLabel] = this.itsICodeTop;
		}
		private void addGoto(Node target, int gotoOp)
		{
			int targetLabel = this.getTargetLabel(target);
			if (targetLabel >= this.itsLabelTableTop)
			{
				Context.CodeBug();
			}
			int num = this.itsLabelTable[targetLabel];
			if (num != -1)
			{
				this.addBackwardGoto(gotoOp, num);
			}
			else
			{
				int num2 = this.itsICodeTop;
				this.addGotoOp(gotoOp);
				int num3 = this.itsFixupTableTop;
				if (this.itsFixupTable == null || num3 == this.itsFixupTable.Length)
				{
					if (this.itsFixupTable == null)
					{
						this.itsFixupTable = new long[40];
					}
					else
					{
						long[] destinationArray = new long[this.itsFixupTable.Length * 2];
						Array.Copy(this.itsFixupTable, 0, destinationArray, 0, num3);
						this.itsFixupTable = destinationArray;
					}
				}
				this.itsFixupTableTop = num3 + 1;
				this.itsFixupTable[num3] = ((long)targetLabel << 32 | (long)((ulong)num2));
			}
		}
		private void fixLabelGotos()
		{
			for (int i = 0; i < this.itsFixupTableTop; i++)
			{
				long num = this.itsFixupTable[i];
				int num2 = (int)(num >> 32);
				int fromPC = (int)num;
				int num3 = this.itsLabelTable[num2];
				if (num3 == -1)
				{
					throw Context.CodeBug();
				}
				this.resolveGoto(fromPC, num3);
			}
			this.itsFixupTableTop = 0;
		}
		private void addBackwardGoto(int gotoOp, int jumpPC)
		{
			int num = this.itsICodeTop;
			if (num <= jumpPC)
			{
				throw Context.CodeBug();
			}
			this.addGotoOp(gotoOp);
			this.resolveGoto(num, jumpPC);
		}
		private void resolveForwardGoto(int fromPC)
		{
			if (this.itsICodeTop < fromPC + 3)
			{
				throw Context.CodeBug();
			}
			this.resolveGoto(fromPC, this.itsICodeTop);
		}
		private void resolveGoto(int fromPC, int jumpPC)
		{
			int num = jumpPC - fromPC;
			if (0 <= num && num <= 2)
			{
				throw Context.CodeBug();
			}
			int num2 = fromPC + 1;
			if (num != (int)((short)num))
			{
				if (this.itsData.longJumps == null)
				{
					this.itsData.longJumps = new UintMap();
				}
				this.itsData.longJumps.put(num2, jumpPC);
				num = 0;
			}
			sbyte[] itsICode = this.itsData.itsICode;
			itsICode[num2] = (sbyte)(num >> 8);
			itsICode[num2 + 1] = (sbyte)num;
		}
		private void addToken(int token)
		{
			if (!Interpreter.ValidTokenCode(token))
			{
				throw Context.CodeBug();
			}
			this.addUint8(token);
		}
		private void addIcode(int icode)
		{
			if (!Interpreter.validIcode(icode))
			{
				throw Context.CodeBug();
			}
			this.addUint8(icode & 255);
		}
		private void addUint8(int value)
		{
			if ((value & -256) != 0)
			{
				throw Context.CodeBug();
			}
			sbyte[] array = this.itsData.itsICode;
			int num = this.itsICodeTop;
			if (num == array.Length)
			{
				array = this.increaseICodeCapasity(1);
			}
			array[num] = (sbyte)value;
			this.itsICodeTop = num + 1;
		}
		private void addUint16(int value)
		{
			if ((value & -65536) != 0)
			{
				throw Context.CodeBug();
			}
			sbyte[] array = this.itsData.itsICode;
			int num = this.itsICodeTop;
			if (num + 2 > array.Length)
			{
				array = this.increaseICodeCapasity(2);
			}
			array[num] = (sbyte)((uint)value >> 8);
			array[num + 1] = (sbyte)value;
			this.itsICodeTop = num + 2;
		}
		private void addInt(int i)
		{
			sbyte[] array = this.itsData.itsICode;
			int num = this.itsICodeTop;
			if (num + 4 > array.Length)
			{
				array = this.increaseICodeCapasity(4);
			}
			array[num] = (sbyte)((uint)i >> 24);
			array[num + 1] = (sbyte)((uint)i >> 16);
			array[num + 2] = (sbyte)((uint)i >> 8);
			array[num + 3] = (sbyte)i;
			this.itsICodeTop = num + 4;
		}
		private int GetDoubleIndex(double num)
		{
			int num2 = this.itsDoubleTableTop;
			if (num2 == 0)
			{
				this.itsData.itsDoubleTable = new double[64];
			}
			else
			{
				if (this.itsData.itsDoubleTable.Length == num2)
				{
					double[] array = new double[num2 * 2];
					Array.Copy(this.itsData.itsDoubleTable, 0, array, 0, num2);
					this.itsData.itsDoubleTable = array;
				}
			}
			this.itsData.itsDoubleTable[num2] = num;
			this.itsDoubleTableTop = num2 + 1;
			return num2;
		}
		private void addVarOp(int op, int varIndex)
		{
			if (op != -7)
			{
				switch (op)
				{
				case 54:
				case 55:
					if (varIndex < 128)
					{
						this.addIcode((op == 54) ? -48 : -49);
						this.addUint8(varIndex);
						return;
					}
					break;
				default:
					throw Context.CodeBug();
				}
			}
			this.addIndexOp(op, varIndex);
		}
		private void addStringOp(int op, string str)
		{
			this.addStringPrefix(str);
			if (Interpreter.validIcode(op))
			{
				this.addIcode(op);
			}
			else
			{
				this.addToken(op);
			}
		}
		private void addIndexOp(int op, int index)
		{
			this.addIndexPrefix(index);
			if (Interpreter.validIcode(op))
			{
				this.addIcode(op);
			}
			else
			{
				this.addToken(op);
			}
		}
		private void addStringPrefix(string str)
		{
			int num = this.itsStrings.Get(str, -1);
			if (num == -1)
			{
				num = this.itsStrings.size();
				this.itsStrings.put(str, num);
			}
			if (num < 4)
			{
				this.addIcode(-41 - num);
			}
			else
			{
				if (num <= 255)
				{
					this.addIcode(-45);
					this.addUint8(num);
				}
				else
				{
					if (num <= 65535)
					{
						this.addIcode(-46);
						this.addUint16(num);
					}
					else
					{
						this.addIcode(-47);
						this.addInt(num);
					}
				}
			}
		}
		private void addIndexPrefix(int index)
		{
			if (index < 0)
			{
				Context.CodeBug();
			}
			if (index < 6)
			{
				this.addIcode(-32 - index);
			}
			else
			{
				if (index <= 255)
				{
					this.addIcode(-38);
					this.addUint8(index);
				}
				else
				{
					if (index <= 65535)
					{
						this.addIcode(-39);
						this.addUint16(index);
					}
					else
					{
						this.addIcode(-40);
						this.addInt(index);
					}
				}
			}
		}
		private void addExceptionHandler(int icodeStart, int icodeEnd, int handlerStart, bool isFinally, int exceptionObjectLocal, int scopeLocal)
		{
			int num = this.itsExceptionTableTop;
			int[] array = this.itsData.itsExceptionTable;
			if (array == null)
			{
				if (num != 0)
				{
					Context.CodeBug();
				}
				array = new int[12];
				this.itsData.itsExceptionTable = array;
			}
			else
			{
				if (array.Length == num)
				{
					array = new int[array.Length * 2];
					Array.Copy(this.itsData.itsExceptionTable, 0, array, 0, num);
					this.itsData.itsExceptionTable = array;
				}
			}
			array[num] = icodeStart;
			array[num + 1] = icodeEnd;
			array[num + 2] = handlerStart;
			array[num + 3] = (isFinally ? 1 : 0);
			array[num + 4] = exceptionObjectLocal;
			array[num + 5] = scopeLocal;
			this.itsExceptionTableTop = num + 6;
		}
		private sbyte[] increaseICodeCapasity(int extraSize)
		{
			int num = this.itsData.itsICode.Length;
			int num2 = this.itsICodeTop;
			if (num2 + extraSize <= num)
			{
				throw Context.CodeBug();
			}
			num *= 2;
			if (num2 + extraSize > num)
			{
				num = num2 + extraSize;
			}
			sbyte[] array = new sbyte[num];
			Array.Copy(this.itsData.itsICode, 0, array, 0, num2);
			this.itsData.itsICode = array;
			return array;
		}
		private void stackChange(int change)
		{
			if (change <= 0)
			{
				this.itsStackDepth += change;
			}
			else
			{
				int num = this.itsStackDepth + change;
				if (num > this.itsData.itsMaxStack)
				{
					this.itsData.itsMaxStack = num;
				}
				this.itsStackDepth = num;
			}
		}
		private int allocLocal()
		{
			int result = this.itsLocalTop;
			this.itsLocalTop++;
			if (this.itsLocalTop > this.itsData.itsMaxLocals)
			{
				this.itsData.itsMaxLocals = this.itsLocalTop;
			}
			return result;
		}
		private void releaseLocal(int localSlot)
		{
			this.itsLocalTop--;
			if (localSlot != this.itsLocalTop)
			{
				Context.CodeBug();
			}
		}
		private static int GetShort(sbyte[] iCode, int pc)
		{
			return (int)iCode[pc] << 8 | ((int)iCode[pc + 1] & 255);
		}
		private static int GetIndex(sbyte[] iCode, int pc)
		{
			return ((int)iCode[pc] & 255) << 8 | ((int)iCode[pc + 1] & 255);
		}
		private static int GetInt(sbyte[] iCode, int pc)
		{
			return (int)iCode[pc] << 24 | ((int)iCode[pc + 1] & 255) << 16 | ((int)iCode[pc + 2] & 255) << 8 | ((int)iCode[pc + 3] & 255);
		}
		private static int getExceptionHandler(Interpreter.CallFrame frame, bool onlyFinally)
		{
			int[] itsExceptionTable = frame.idata.itsExceptionTable;
			int result;
			if (itsExceptionTable == null)
			{
				result = -1;
			}
			else
			{
				int num = frame.pc - 1;
				int num2 = -1;
				int num3 = 0;
				int num4 = 0;
				for (int num5 = 0; num5 != itsExceptionTable.Length; num5 += 6)
				{
					int num6 = itsExceptionTable[num5];
					int num7 = itsExceptionTable[num5 + 1];
					if (num6 <= num && num < num7)
					{
						if (!onlyFinally || itsExceptionTable[num5 + 3] == 1)
						{
							if (num2 >= 0)
							{
								if (num4 < num7)
								{
									goto IL_EB;
								}
								if (num3 > num6)
								{
									Context.CodeBug();
								}
								if (num4 == num7)
								{
									Context.CodeBug();
								}
							}
							num2 = num5;
							num3 = num6;
							num4 = num7;
						}
					}
					IL_EB:;
				}
				result = num2;
			}
			return result;
		}
		private static void dumpICode(InterpreterData idata)
		{
			if (Token.printICode)
			{
				sbyte[] itsICode = idata.itsICode;
				int num = itsICode.Length;
				string[] itsStringTable = idata.itsStringTable;
				TextWriter @out = Console.Out;
				@out.WriteLine(string.Concat(new object[]
				{
					"ICode dump, for ",
					idata.itsName,
					", length = ",
					num
				}));
				@out.WriteLine("MaxStack = " + idata.itsMaxStack);
				int num2 = 0;
				int i = 0;
				while (i < num)
				{
					@out.Flush();
					@out.Write(" [" + i + "] ");
					int num3 = (int)itsICode[i];
					int num4 = Interpreter.bytecodeSpan(num3);
					string text = Interpreter.bytecodeName(num3);
					int num5 = i;
					i++;
					int num6 = num3;
					if (num6 <= 30)
					{
						if (num6 <= -6)
						{
							switch (num6)
							{
							case -55:
								goto IL_3F8;
							case -54:
								break;
							case -53:
							case -52:
							case -51:
							case -50:
							case -44:
							case -43:
							case -42:
							case -41:
								goto IL_236;
							case -49:
							case -48:
								num2 = (int)itsICode[i];
								@out.WriteLine(text + " " + num2);
								i++;
								goto IL_6CC;
							case -47:
							{
								string text2 = itsStringTable[Interpreter.GetInt(itsICode, i)];
								@out.WriteLine(string.Concat(new object[]
								{
									text,
									" \"",
									text2,
									'"'
								}));
								i += 4;
								goto IL_6CC;
							}
							case -46:
							{
								string text2 = itsStringTable[Interpreter.GetIndex(itsICode, i)];
								@out.WriteLine(string.Concat(new object[]
								{
									text,
									" \"",
									text2,
									'"'
								}));
								i += 2;
								goto IL_6CC;
							}
							case -45:
							{
								string text2 = itsStringTable[255 & (int)itsICode[i]];
								@out.WriteLine(string.Concat(new object[]
								{
									text,
									" \"",
									text2,
									'"'
								}));
								i++;
								goto IL_6CC;
							}
							case -40:
								num2 = Interpreter.GetInt(itsICode, i);
								@out.WriteLine(text + " " + num2);
								i += 4;
								goto IL_6CC;
							case -39:
								num2 = Interpreter.GetIndex(itsICode, i);
								@out.WriteLine(text + " " + num2);
								i += 2;
								goto IL_6CC;
							case -38:
								num2 = (255 & (int)itsICode[i]);
								@out.WriteLine(text + " " + num2);
								i++;
								goto IL_6CC;
							default:
								switch (num6)
								{
								case -31:
									goto IL_3B0;
								case -30:
								case -29:
								case -25:
								case -24:
								case -22:
								case -18:
								case -17:
								case -16:
								case -15:
								case -14:
								case -13:
								case -12:
									goto IL_236;
								case -28:
								{
									int num7 = Interpreter.GetInt(itsICode, i);
									@out.WriteLine(text + " " + num7);
									i += 4;
									goto IL_6CC;
								}
								case -27:
								{
									int num7 = Interpreter.GetShort(itsICode, i);
									@out.WriteLine(text + " " + num7);
									i += 2;
									goto IL_6CC;
								}
								case -26:
								{
									int index = Interpreter.GetIndex(itsICode, i);
									@out.WriteLine(text + " : " + index);
									i += 2;
									goto IL_6CC;
								}
								case -23:
								case -6:
									break;
								case -21:
								{
									int num8 = (int)itsICode[i] & 255;
									bool flag = itsICode[i + 1] != 0;
									int index = Interpreter.GetIndex(itsICode, i + 2);
									@out.WriteLine(string.Concat(new object[]
									{
										text,
										" ",
										num8,
										" ",
										flag,
										" ",
										num2,
										" ",
										index
									}));
									i += 4;
									goto IL_6CC;
								}
								case -20:
								case -19:
									@out.WriteLine(text + " " + idata.itsNestedFunctions[num2]);
									goto IL_6CC;
								case -11:
								case -10:
								case -9:
								case -8:
								case -7:
								{
									int num9 = (int)itsICode[i];
									@out.WriteLine(text + " " + num9);
									i++;
									goto IL_6CC;
								}
								default:
									goto IL_236;
								}
								break;
							}
						}
						else
						{
							switch (num6)
							{
							case 5:
							case 6:
							case 7:
								break;
							default:
								if (num6 != 30)
								{
									goto IL_236;
								}
								goto IL_3F8;
							}
						}
						int num10 = i + Interpreter.GetShort(itsICode, i) - 1;
						@out.WriteLine(text + " " + num10);
						i += 2;
					}
					else
					{
						if (num6 <= 49)
						{
							switch (num6)
							{
							case 37:
								goto IL_3F8;
							case 38:
								goto IL_236;
							case 39:
							{
								double num11 = idata.itsDoubleTable[num2];
								@out.WriteLine(text + " " + num11);
								i += 2;
								break;
							}
							default:
								switch (num6)
								{
								case 47:
									@out.WriteLine(text + " " + idata.itsRegExpLiterals[num2]);
									break;
								case 48:
									goto IL_236;
								case 49:
								{
									int index = Interpreter.GetIndex(itsICode, i);
									@out.WriteLine(text + " : " + index);
									i += 2;
									break;
								}
								default:
									goto IL_236;
								}
								break;
							}
						}
						else
						{
							if (num6 != 56)
							{
								if (num6 == 64)
								{
									goto IL_3B0;
								}
								if (num6 != 68)
								{
									goto IL_236;
								}
								goto IL_3F8;
							}
							else
							{
								bool flag2 = itsICode[i] != 0;
								@out.WriteLine(text + " " + flag2);
								i++;
							}
						}
					}
					IL_6CC:
					if (num5 + num4 != i)
					{
						Context.CodeBug();
					}
					continue;
					IL_236:
					if (num4 != 1)
					{
						Context.CodeBug();
					}
					@out.WriteLine(text);
					goto IL_6CC;
					IL_3B0:
					@out.WriteLine(text + " " + idata.literalIds[num2]);
					goto IL_6CC;
					IL_3F8:
					@out.WriteLine(text + ' ' + num2);
					goto IL_6CC;
				}
				int[] itsExceptionTable = idata.itsExceptionTable;
				if (itsExceptionTable != null)
				{
					@out.WriteLine("Exception handlers: " + itsExceptionTable.Length / 6);
					for (int num12 = 0; num12 != itsExceptionTable.Length; num12 += 6)
					{
						int num13 = itsExceptionTable[num12];
						int num14 = itsExceptionTable[num12 + 1];
						int num15 = itsExceptionTable[num12 + 2];
						int num16 = itsExceptionTable[num12 + 3];
						int num17 = itsExceptionTable[num12 + 4];
						int num18 = itsExceptionTable[num12 + 5];
						@out.WriteLine(string.Concat(new object[]
						{
							" tryStart=",
							num13,
							" tryEnd=",
							num14,
							" handlerStart=",
							num15,
							" type=",
							(num16 == 0) ? "catch" : "finally",
							" exceptionLocal=",
							num17
						}));
					}
				}
				@out.Flush();
			}
		}
		private static int bytecodeSpan(int bytecode)
		{
			int result;
			if (bytecode <= -6)
			{
				switch (bytecode)
				{
				case -54:
					break;
				case -53:
				case -52:
				case -51:
				case -50:
				case -44:
				case -43:
				case -42:
				case -41:
					goto IL_139;
				case -49:
				case -48:
					result = 2;
					return result;
				case -47:
					result = 5;
					return result;
				case -46:
					result = 3;
					return result;
				case -45:
					result = 2;
					return result;
				case -40:
					result = 5;
					return result;
				case -39:
					result = 3;
					return result;
				case -38:
					result = 2;
					return result;
				default:
					switch (bytecode)
					{
					case -28:
						result = 5;
						return result;
					case -27:
						result = 3;
						return result;
					case -26:
						result = 3;
						return result;
					case -25:
					case -24:
					case -22:
						goto IL_139;
					case -23:
						break;
					case -21:
						result = 5;
						return result;
					default:
						switch (bytecode)
						{
						case -11:
						case -10:
						case -9:
						case -8:
						case -7:
							result = 2;
							return result;
						case -6:
							break;
						default:
							goto IL_139;
						}
						break;
					}
					break;
				}
			}
			else
			{
				switch (bytecode)
				{
				case 5:
				case 6:
				case 7:
					break;
				default:
					if (bytecode == 49)
					{
						result = 3;
						return result;
					}
					if (bytecode != 56)
					{
						goto IL_139;
					}
					result = 2;
					return result;
				}
			}
			result = 3;
			return result;
			IL_139:
			if (!Interpreter.validBytecode(bytecode))
			{
				throw Context.CodeBug();
			}
			result = 1;
			return result;
		}
		internal static int[] getLineNumbers(InterpreterData data)
		{
			UintMap uintMap = new UintMap();
			sbyte[] itsICode = data.itsICode;
			int num = itsICode.Length;
			int num4;
			for (int num2 = 0; num2 != num; num2 += num4)
			{
				int num3 = (int)itsICode[num2];
				num4 = Interpreter.bytecodeSpan(num3);
				if (num3 == -26)
				{
					if (num4 != 3)
					{
						Context.CodeBug();
					}
					int index = Interpreter.GetIndex(itsICode, num2 + 1);
					uintMap.put(index, 0);
				}
			}
			return uintMap.Keys;
		}
		internal static void captureInterpreterStackInfo(EcmaScriptException ex)
		{
			Context currentContext = Context.CurrentContext;
			if (currentContext == null || currentContext.lastInterpreterFrame == null)
			{
				ex.m_InterpreterStackInfo = null;
				ex.m_InterpreterLineData = null;
			}
			else
			{
				Interpreter.CallFrame[] array;
				if (currentContext.previousInterpreterInvocations == null || currentContext.previousInterpreterInvocations.size() == 0)
				{
					array = new Interpreter.CallFrame[1];
				}
				else
				{
					int num = currentContext.previousInterpreterInvocations.size();
					if (currentContext.previousInterpreterInvocations.peek() == currentContext.lastInterpreterFrame)
					{
						num--;
					}
					array = new Interpreter.CallFrame[num + 1];
					currentContext.previousInterpreterInvocations.ToArray(array);
				}
				array[array.Length - 1] = (Interpreter.CallFrame)currentContext.lastInterpreterFrame;
				int num2 = 0;
				int num3;
				for (num3 = 0; num3 != array.Length; num3++)
				{
					num2 += 1 + array[num3].frameIndex;
				}
				int[] array2 = new int[num2];
				int num4 = num2;
				num3 = array.Length;
				while (num3 != 0)
				{
					num3--;
					for (Interpreter.CallFrame callFrame = array[num3]; callFrame != null; callFrame = callFrame.parentFrame)
					{
						num4--;
						array2[num4] = callFrame.pcSourceLineStart;
					}
				}
				if (num4 != 0)
				{
					Context.CodeBug();
				}
				ex.m_InterpreterStackInfo = array;
				ex.m_InterpreterLineData = array2;
			}
		}
		internal static string GetSourcePositionFromStack(Context cx, int[] linep)
		{
			Interpreter.CallFrame callFrame = (Interpreter.CallFrame)cx.lastInterpreterFrame;
			InterpreterData idata = callFrame.idata;
			if (callFrame.pcSourceLineStart >= 0)
			{
				linep[0] = Interpreter.GetIndex(idata.itsICode, callFrame.pcSourceLineStart);
			}
			else
			{
				linep[0] = 0;
			}
			return idata.itsSourceFile;
		}
		internal static string GetStack(EcmaScriptException ex)
		{
			StringBuilder stringBuilder = new StringBuilder();
			Interpreter.CallFrame[] array = (Interpreter.CallFrame[])ex.m_InterpreterStackInfo;
			string result;
			if (array == null)
			{
				result = stringBuilder.ToString();
			}
			else
			{
				int[] interpreterLineData = ex.m_InterpreterLineData;
				int num = array.Length;
				int num2 = interpreterLineData.Length;
				while (num != 0)
				{
					num--;
					for (Interpreter.CallFrame callFrame = array[num]; callFrame != null; callFrame = callFrame.parentFrame)
					{
						if (num2 == 0)
						{
							Context.CodeBug();
						}
						num2--;
						InterpreterData idata = callFrame.idata;
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Append(Environment.NewLine);
						}
						stringBuilder.Append("\tat script");
						if (idata.itsName != null && idata.itsName.Length != 0)
						{
							stringBuilder.Append('.');
							stringBuilder.Append(idata.itsName);
						}
						stringBuilder.Append('(');
						stringBuilder.Append(idata.itsSourceFile);
						int num3 = interpreterLineData[num2];
						if (num3 >= 0)
						{
							stringBuilder.Append(':');
							stringBuilder.Append(Interpreter.GetIndex(idata.itsICode, num3));
						}
						stringBuilder.Append(')');
					}
				}
				result = stringBuilder.ToString();
			}
			return result;
		}
		internal static string getPatchedStack(EcmaScriptException ex, string nativeStackTrace)
		{
			string text = "EcmaScript.NET.Interpreter.interpretLoop";
			StringBuilder stringBuilder = new StringBuilder(nativeStackTrace.Length + 1000);
			string newLine = Environment.NewLine;
			Interpreter.CallFrame[] array = (Interpreter.CallFrame[])ex.m_InterpreterStackInfo;
			string result;
			if (array == null)
			{
				result = stringBuilder.ToString();
			}
			else
			{
				int[] interpreterLineData = ex.m_InterpreterLineData;
				int num = array.Length;
				int num2 = interpreterLineData.Length;
				int num3 = 0;
				while (num != 0)
				{
					num--;
					int num4 = nativeStackTrace.IndexOf(text, num3);
					if (num4 < 0)
					{
						break;
					}
					for (num4 += text.Length; num4 != nativeStackTrace.Length; num4++)
					{
						char c = nativeStackTrace[num4];
						if (c == '\n' || c == '\r')
						{
							break;
						}
					}
					stringBuilder.Append(nativeStackTrace.Substring(num3, num4 - num3));
					num3 = num4;
					for (Interpreter.CallFrame callFrame = array[num]; callFrame != null; callFrame = callFrame.parentFrame)
					{
						if (num2 == 0)
						{
							Context.CodeBug();
						}
						num2--;
						InterpreterData idata = callFrame.idata;
						stringBuilder.Append(newLine);
						stringBuilder.Append("\tat script");
						if (idata.itsName != null && idata.itsName.Length != 0)
						{
							stringBuilder.Append('.');
							stringBuilder.Append(idata.itsName);
						}
						stringBuilder.Append('(');
						stringBuilder.Append(idata.itsSourceFile);
						int num5 = interpreterLineData[num2];
						if (num5 >= 0)
						{
							stringBuilder.Append(':');
							stringBuilder.Append(Interpreter.GetIndex(idata.itsICode, num5));
						}
						stringBuilder.Append(')');
					}
				}
				stringBuilder.Append(nativeStackTrace.Substring(num3));
				result = stringBuilder.ToString();
			}
			return result;
		}
		internal static string GetEncodedSource(InterpreterData idata)
		{
			string result;
			if (idata.encodedSource == null)
			{
				result = null;
			}
			else
			{
				result = idata.encodedSource.Substring(idata.encodedSourceStart, idata.encodedSourceEnd - idata.encodedSourceStart);
			}
			return result;
		}
		private static void initFunction(Context cx, IScriptable scope, InterpretedFunction parent, int index)
		{
			InterpretedFunction interpretedFunction = InterpretedFunction.createFunction(cx, scope, parent, index);
			ScriptRuntime.initFunction(cx, scope, interpretedFunction, interpretedFunction.idata.itsFunctionType, parent.idata.evalScriptFlag);
		}
		internal static object Interpret(InterpretedFunction ifun, Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			object result;
			using (new StackOverflowVerifier(128))
			{
				if (!ScriptRuntime.hasTopCall(cx))
				{
					Context.CodeBug();
				}
				if (cx.interpreterSecurityDomain != ifun.securityDomain)
				{
					object interpreterSecurityDomain = cx.interpreterSecurityDomain;
					cx.interpreterSecurityDomain = ifun.securityDomain;
					try
					{
						result = ifun.securityController.callWithDomain(ifun.securityDomain, cx, ifun, scope, thisObj, args);
						return result;
					}
					finally
					{
						cx.interpreterSecurityDomain = interpreterSecurityDomain;
					}
				}
				Interpreter.CallFrame frame = new Interpreter.CallFrame();
				Interpreter.initFrame(cx, scope, thisObj, args, null, 0, args.Length, ifun, null, frame);
				result = Interpreter.InterpretLoop(cx, frame, null);
			}
			return result;
		}
		public static object restartContinuation(Continuation c, Context cx, IScriptable scope, object[] args)
		{
			object result;
			if (!ScriptRuntime.hasTopCall(cx))
			{
				result = ScriptRuntime.DoTopCall(c, cx, scope, null, args);
			}
			else
			{
				object obj;
				if (args.Length == 0)
				{
					obj = Undefined.Value;
				}
				else
				{
					obj = args[0];
				}
				Interpreter.CallFrame callFrame = (Interpreter.CallFrame)c.Implementation;
				if (callFrame == null)
				{
					result = obj;
				}
				else
				{
					result = Interpreter.InterpretLoop(cx, null, new Interpreter.ContinuationJump(c, null)
					{
						result = obj
					});
				}
			}
			return result;
		}
		private static object InterpretLoop(Context cx, Interpreter.CallFrame frame, object throwable)
		{
			object doubleMark = UniqueTag.DoubleMark;
			object value = Undefined.Value;
			bool flag = cx.instructionThreshold != 0;
			string text = null;
			int num = -1;
			if (cx.lastInterpreterFrame != null)
			{
				if (cx.previousInterpreterInvocations == null)
				{
					cx.previousInterpreterInvocations = new ObjArray();
				}
				cx.previousInterpreterInvocations.push(cx.lastInterpreterFrame);
			}
			if (throwable != null)
			{
				if (!(throwable is Interpreter.ContinuationJump))
				{
					Context.CodeBug();
				}
			}
			object obj = null;
			double num2 = 0.0;
			Interpreter.ContinuationJump continuationJump2;
			while (true)
			{
				try
				{
					if (throwable != null)
					{
						if (num >= 0)
						{
							if (frame.frozen)
							{
								frame = frame.cloneFrozen();
							}
							int[] itsExceptionTable = frame.idata.itsExceptionTable;
							frame.pc = itsExceptionTable[num + 2];
							if (flag)
							{
								frame.pcPrevBranch = frame.pc;
							}
							frame.savedStackTop = frame.emptyStackTop;
							int num3 = frame.localShift + itsExceptionTable[num + 5];
							int num4 = frame.localShift + itsExceptionTable[num + 4];
							frame.scope = (IScriptable)frame.stack[num3];
							frame.stack[num4] = throwable;
							throwable = null;
						}
						else
						{
							Interpreter.ContinuationJump continuationJump = (Interpreter.ContinuationJump)throwable;
							throwable = null;
							if (continuationJump.branchFrame != frame)
							{
								Context.CodeBug();
							}
							if (continuationJump.capturedFrame == null)
							{
								Context.CodeBug();
							}
							int num5 = continuationJump.capturedFrame.frameIndex + 1;
							if (continuationJump.branchFrame != null)
							{
								num5 -= continuationJump.branchFrame.frameIndex;
							}
							int num6 = 0;
							Interpreter.CallFrame[] array = null;
							Interpreter.CallFrame callFrame = continuationJump.capturedFrame;
							for (int num7 = 0; num7 != num5; num7++)
							{
								if (!callFrame.frozen)
								{
									Context.CodeBug();
								}
								if (Interpreter.isFrameEnterExitRequired(callFrame))
								{
									if (array == null)
									{
										array = new Interpreter.CallFrame[num5 - num7];
									}
									array[num6] = callFrame;
									num6++;
								}
								callFrame = callFrame.parentFrame;
							}
							while (num6 != 0)
							{
								num6--;
								callFrame = array[num6];
								Interpreter.EnterFrame(cx, callFrame, ScriptRuntime.EmptyArgs);
							}
							frame = continuationJump.capturedFrame.cloneFrozen();
							Interpreter.setCallResult(frame, continuationJump.result, continuationJump.resultDbl);
						}
						if (throwable != null)
						{
							Context.CodeBug();
						}
					}
					else
					{
						if (frame.frozen)
						{
							Context.CodeBug();
						}
					}
					object[] stack = frame.stack;
					double[] sDbl = frame.sDbl;
					object[] stack2 = frame.varSource.stack;
					double[] sDbl2 = frame.varSource.sDbl;
					sbyte[] itsICode = frame.idata.itsICode;
					string[] itsStringTable = frame.idata.itsStringTable;
					int num8 = frame.savedStackTop;
					cx.lastInterpreterFrame = frame;
					int num9;
					IScriptable thisObj;
					IScriptable scriptable;
					InterpretedFunction interpretedFunction;
					object obj3;
					object obj5;
					int index2;
					InterpretedFunction interpretedFunction3;
					while (true)
					{
						num9 = (int)itsICode[frame.pc++];
						switch (num9)
						{
						case -57:
							if (frame.debuggerFrame != null)
							{
								frame.debuggerFrame.OnDebuggerStatement(cx);
							}
							goto IL_2577;
						case -56:
							num += frame.localShift;
							stack[num] = null;
							continue;
						case -55:
						case 37:
						case 68:
						{
							if (flag)
							{
								cx.instructionCount += 100;
							}
							num8 -= 1 + num;
							ICallable callable = (ICallable)stack[num8];
							thisObj = (IScriptable)stack[num8 + 1];
							if (num9 == 68)
							{
								object[] argsArray = Interpreter.GetArgsArray(stack, sDbl, num8 + 2, num);
								stack[num8] = ScriptRuntime.callRef(callable, thisObj, argsArray, cx);
								continue;
							}
							scriptable = frame.scope;
							if (frame.useActivation)
							{
								scriptable = ScriptableObject.GetTopLevelScope(frame.scope);
							}
							if (callable is InterpretedFunction)
							{
								interpretedFunction = (InterpretedFunction)callable;
								if (frame.fnOrScript.securityDomain == interpretedFunction.securityDomain)
								{
									goto Block_101;
								}
							}
							if (callable is Continuation)
							{
								Interpreter.ContinuationJump continuationJump = new Interpreter.ContinuationJump((Continuation)callable, frame);
								if (num == 0)
								{
									continuationJump.result = value;
								}
								else
								{
									continuationJump.result = stack[num8 + 2];
									continuationJump.resultDbl = sDbl[num8 + 2];
								}
								throwable = continuationJump;
								goto IL_2577;
							}
							if (callable is IdFunctionObject)
							{
								IdFunctionObject f = (IdFunctionObject)callable;
								if (Continuation.IsContinuationConstructor(f))
								{
									Interpreter.captureContinuation(cx, frame, num8);
									continue;
								}
							}
							object[] argsArray2 = Interpreter.GetArgsArray(stack, sDbl, num8 + 2, num);
							stack[num8] = callable.Call(cx, scriptable, thisObj, argsArray2);
							continue;
						}
						case -54:
						{
							bool flag2 = Interpreter.stack_boolean(frame, num8);
							object obj2 = ScriptRuntime.updateDotQuery(flag2, frame.scope);
							if (obj2 != null)
							{
								stack[num8] = obj2;
								frame.scope = ScriptRuntime.leaveDotQuery(frame.scope);
								frame.pc += 2;
								continue;
							}
							num8--;
							goto IL_2578;
						}
						case -53:
							obj3 = stack[num8];
							if (obj3 == doubleMark)
							{
								obj3 = sDbl[num8];
							}
							num8--;
							frame.scope = ScriptRuntime.enterDotQuery(obj3, frame.scope);
							continue;
						case -52:
							num8++;
							stack[num8] = doubleMark;
							sDbl[num8] = 1.0;
							continue;
						case -51:
							num8++;
							stack[num8] = doubleMark;
							sDbl[num8] = 0.0;
							continue;
						case -50:
							stack[++num8] = value;
							continue;
						case -49:
							num = (int)itsICode[frame.pc++];
							goto IL_1A74;
						case -48:
							num = (int)itsICode[frame.pc++];
							goto IL_1B0E;
						case -47:
							text = itsStringTable[Interpreter.GetInt(itsICode, frame.pc)];
							frame.pc += 4;
							continue;
						case -46:
							text = itsStringTable[Interpreter.GetIndex(itsICode, frame.pc)];
							frame.pc += 2;
							continue;
						case -45:
							text = itsStringTable[255 & (int)itsICode[frame.pc]];
							frame.pc++;
							continue;
						case -44:
							text = itsStringTable[3];
							continue;
						case -43:
							text = itsStringTable[2];
							continue;
						case -42:
							text = itsStringTable[1];
							continue;
						case -41:
							text = itsStringTable[0];
							continue;
						case -40:
							num = Interpreter.GetInt(itsICode, frame.pc);
							frame.pc += 4;
							continue;
						case -39:
							num = Interpreter.GetIndex(itsICode, frame.pc);
							frame.pc += 2;
							continue;
						case -38:
							num = (255 & (int)itsICode[frame.pc]);
							frame.pc++;
							continue;
						case -37:
							num = 5;
							continue;
						case -36:
							num = 4;
							continue;
						case -35:
							num = 3;
							continue;
						case -34:
							num = 2;
							continue;
						case -33:
							num = 1;
							continue;
						case -32:
							num = 0;
							continue;
						case -31:
						case 63:
						case 64:
						{
							object[] array2 = (object[])stack[num8];
							object obj4;
							if (num9 == 64)
							{
								object[] propertyIds = (object[])frame.idata.literalIds[num];
								obj4 = ScriptRuntime.newObjectLiteral(propertyIds, array2, cx, frame.scope);
							}
							else
							{
								int[] skipIndexces = null;
								if (num9 == -31)
								{
									skipIndexces = (int[])frame.idata.literalIds[num];
								}
								obj4 = ScriptRuntime.newArrayLiteral(array2, skipIndexces, cx, frame.scope);
							}
							stack[num8] = obj4;
							continue;
						}
						case -30:
						{
							obj5 = stack[num8];
							if (obj5 == doubleMark)
							{
								obj5 = sDbl[num8];
							}
							num8--;
							int num7 = (int)sDbl[num8];
							((object[])stack[num8])[num7] = obj5;
							sDbl[num8] = (double)(num7 + 1);
							continue;
						}
						case -29:
							num8++;
							stack[num8] = new object[num];
							sDbl[num8] = 0.0;
							continue;
						case -28:
							num8++;
							stack[num8] = doubleMark;
							sDbl[num8] = (double)Interpreter.GetInt(itsICode, frame.pc);
							frame.pc += 4;
							continue;
						case -27:
							num8++;
							stack[num8] = doubleMark;
							sDbl[num8] = (double)Interpreter.GetShort(itsICode, frame.pc);
							frame.pc += 2;
							continue;
						case -26:
							frame.pcSourceLineStart = frame.pc;
							if (frame.debuggerFrame != null)
							{
								int index = Interpreter.GetIndex(itsICode, frame.pc);
								frame.debuggerFrame.OnLineChange(cx, index);
							}
							frame.pc += 2;
							continue;
						case -25:
							if (flag)
							{
								Interpreter.addInstructionCount(cx, frame, 0);
							}
							num += frame.localShift;
							obj5 = stack[num];
							if (obj5 != doubleMark)
							{
								goto Block_68;
							}
							frame.pc = (int)sDbl[num];
							if (flag)
							{
								frame.pcPrevBranch = frame.pc;
							}
							continue;
						case -24:
							if (num8 == frame.emptyStackTop + 1)
							{
								num += frame.localShift;
								stack[num] = stack[num8];
								sDbl[num] = sDbl[num8];
								num8--;
							}
							else
							{
								if (num8 != frame.emptyStackTop)
								{
									Context.CodeBug();
								}
							}
							continue;
						case -23:
							num8++;
							stack[num8] = doubleMark;
							sDbl[num8] = (double)(frame.pc + 2);
							goto IL_2578;
						case -22:
							goto IL_C5F;
						case -21:
						{
							if (flag)
							{
								cx.instructionCount += 100;
							}
							int callType = (int)itsICode[frame.pc] & 255;
							bool flag3 = itsICode[frame.pc + 1] != 0;
							index2 = Interpreter.GetIndex(itsICode, frame.pc + 2);
							if (flag3)
							{
								num8 -= num;
								object obj6 = stack[num8];
								if (obj6 == doubleMark)
								{
									obj6 = sDbl[num8];
								}
								object[] argsArray = Interpreter.GetArgsArray(stack, sDbl, num8 + 1, num);
								stack[num8] = ScriptRuntime.newSpecial(cx, obj6, argsArray, frame.scope, callType);
							}
							else
							{
								num8 -= 1 + num;
								IScriptable thisObj2 = (IScriptable)stack[num8 + 1];
								ICallable fun = (ICallable)stack[num8];
								object[] argsArray = Interpreter.GetArgsArray(stack, sDbl, num8 + 2, num);
								stack[num8] = ScriptRuntime.callSpecial(cx, fun, thisObj2, argsArray, frame.scope, frame.thisObj, callType, frame.idata.itsSourceFile, index2);
							}
							frame.pc += 4;
							continue;
						}
						case -20:
							Interpreter.initFunction(cx, frame.scope, frame.fnOrScript, num);
							continue;
						case -19:
						{
							InterpretedFunction interpretedFunction2 = InterpretedFunction.createFunction(cx, frame.scope, frame.fnOrScript, num);
							stack[++num8] = interpretedFunction2;
							continue;
						}
						case -18:
							obj5 = stack[num8];
							if (obj5 == doubleMark)
							{
								obj5 = sDbl[num8];
							}
							stack[num8] = ScriptRuntime.getValueFunctionAndThis(obj5, cx);
							num8++;
							stack[num8] = ScriptRuntime.lastStoredScriptable(cx);
							continue;
						case -17:
						{
							object obj7 = stack[num8 - 1];
							if (obj7 == doubleMark)
							{
								obj7 = sDbl[num8 - 1];
							}
							object obj8 = stack[num8];
							if (obj8 == doubleMark)
							{
								obj8 = sDbl[num8];
							}
							stack[num8 - 1] = ScriptRuntime.GetElemFunctionAndThis(obj7, obj8, cx);
							stack[num8] = ScriptRuntime.lastStoredScriptable(cx);
							continue;
						}
						case -16:
						{
							object obj7 = stack[num8];
							if (obj7 == doubleMark)
							{
								obj7 = sDbl[num8];
							}
							stack[num8] = ScriptRuntime.getPropFunctionAndThis(obj7, text, cx);
							num8++;
							stack[num8] = ScriptRuntime.lastStoredScriptable(cx);
							continue;
						}
						case -15:
							num8++;
							stack[num8] = ScriptRuntime.getNameFunctionAndThis(text, cx, frame.scope);
							num8++;
							stack[num8] = ScriptRuntime.lastStoredScriptable(cx);
							continue;
						case -14:
							stack[++num8] = ScriptRuntime.TypeofName(frame.scope, text);
							continue;
						case -13:
							num += frame.localShift;
							stack[num] = frame.scope;
							continue;
						case -12:
							num += frame.localShift;
							frame.scope = (IScriptable)stack[num];
							continue;
						case -11:
						{
							IRef rf = (IRef)stack[num8];
							stack[num8] = ScriptRuntime.refIncrDecr(rf, cx, (int)itsICode[frame.pc]);
							frame.pc++;
							continue;
						}
						case -10:
						{
							object obj9 = stack[num8];
							if (obj9 == doubleMark)
							{
								obj9 = sDbl[num8];
							}
							num8--;
							obj3 = stack[num8];
							if (obj3 == doubleMark)
							{
								obj3 = sDbl[num8];
							}
							stack[num8] = ScriptRuntime.elemIncrDecr(obj3, obj9, cx, (int)itsICode[frame.pc]);
							frame.pc++;
							continue;
						}
						case -9:
							obj3 = stack[num8];
							if (obj3 == doubleMark)
							{
								obj3 = sDbl[num8];
							}
							stack[num8] = ScriptRuntime.propIncrDecr(obj3, text, cx, (int)itsICode[frame.pc]);
							frame.pc++;
							continue;
						case -8:
							stack[++num8] = ScriptRuntime.nameIncrDecr(frame.scope, text, (int)itsICode[frame.pc]);
							frame.pc++;
							continue;
						case -7:
						{
							num8++;
							int num10 = (int)itsICode[frame.pc];
							if (!frame.useActivation)
							{
								stack[num8] = doubleMark;
								object obj10 = stack2[num];
								double num11;
								if (obj10 == doubleMark)
								{
									num11 = sDbl2[num];
								}
								else
								{
									num11 = ScriptConvert.ToNumber(obj10);
									stack2[num] = doubleMark;
								}
								double num12 = ((num10 & 1) == 0) ? (num11 + 1.0) : (num11 - 1.0);
								sDbl2[num] = num12;
								sDbl[num8] = (((num10 & 2) == 0) ? num12 : num11);
							}
							else
							{
								string id = frame.idata.argNames[num];
								stack[num8] = ScriptRuntime.nameIncrDecr(frame.scope, id, num10);
							}
							frame.pc++;
							continue;
						}
						case -6:
							if (!Interpreter.stack_boolean(frame, num8--))
							{
								frame.pc += 2;
								continue;
							}
							stack[num8--] = null;
							goto IL_2578;
						case -5:
							frame.result = stack[num8];
							frame.resultDbl = sDbl[num8];
							stack[num8] = null;
							num8--;
							continue;
						case -4:
							stack[num8] = null;
							num8--;
							continue;
						case -3:
						{
							object obj11 = stack[num8];
							stack[num8] = stack[num8 - 1];
							stack[num8 - 1] = obj11;
							double num11 = sDbl[num8];
							sDbl[num8] = sDbl[num8 - 1];
							sDbl[num8 - 1] = num11;
							continue;
						}
						case -2:
							stack[num8 + 1] = stack[num8 - 1];
							sDbl[num8 + 1] = sDbl[num8 - 1];
							stack[num8 + 2] = stack[num8];
							sDbl[num8 + 2] = sDbl[num8];
							num8 += 2;
							continue;
						case -1:
							stack[num8 + 1] = stack[num8];
							sDbl[num8 + 1] = sDbl[num8];
							num8++;
							continue;
						case 2:
							obj3 = stack[num8];
							if (obj3 == doubleMark)
							{
								obj3 = sDbl[num8];
							}
							num8--;
							frame.scope = ScriptRuntime.enterWith(obj3, cx, frame.scope);
							continue;
						case 3:
							frame.scope = ScriptRuntime.leaveWith(frame.scope);
							continue;
						case 4:
							goto IL_C39;
						case 5:
							goto IL_2578;
						case 6:
							if (!Interpreter.stack_boolean(frame, num8--))
							{
								frame.pc += 2;
								continue;
							}
							goto IL_2578;
						case 7:
							if (Interpreter.stack_boolean(frame, num8--))
							{
								frame.pc += 2;
								continue;
							}
							goto IL_2578;
						case 8:
						{
							object obj9 = stack[num8];
							if (obj9 == doubleMark)
							{
								obj9 = sDbl[num8];
							}
							num8--;
							IScriptable bound = (IScriptable)stack[num8];
							stack[num8] = ScriptRuntime.setName(bound, obj9, cx, frame.scope, text);
							continue;
						}
						case 9:
						case 10:
						case 11:
						case 18:
						case 19:
						{
							int num13 = Interpreter.stack_int32(frame, num8);
							num8--;
							int num14 = Interpreter.stack_int32(frame, num8);
							stack[num8] = doubleMark;
							int num15 = num9;
							switch (num15)
							{
							case 9:
								num14 |= num13;
								break;
							case 10:
								num14 ^= num13;
								break;
							case 11:
								num14 &= num13;
								break;
							default:
								switch (num15)
								{
								case 18:
									num14 <<= num13;
									break;
								case 19:
									num14 >>= num13;
									break;
								}
								break;
							}
							sDbl[num8] = (double)num14;
							continue;
						}
						case 12:
						case 13:
						{
							num8--;
							object obj9 = stack[num8 + 1];
							obj3 = stack[num8];
							bool flag2;
							if (obj9 == doubleMark)
							{
								if (obj3 == doubleMark)
								{
									flag2 = (sDbl[num8] == sDbl[num8 + 1]);
								}
								else
								{
									flag2 = ScriptRuntime.eqNumber(sDbl[num8 + 1], obj3);
								}
							}
							else
							{
								if (obj3 == doubleMark)
								{
									flag2 = ScriptRuntime.eqNumber(sDbl[num8], obj9);
								}
								else
								{
									flag2 = ScriptRuntime.eq(obj3, obj9);
								}
							}
							flag2 ^= (num9 == 13);
							stack[num8] = flag2;
							continue;
						}
						case 14:
						case 15:
						case 16:
						case 17:
						{
							num8--;
							object obj9 = stack[num8 + 1];
							obj3 = stack[num8];
							if (obj9 == doubleMark)
							{
								double num16 = sDbl[num8 + 1];
								double num17 = Interpreter.stack_double(frame, num8);
								goto IL_67E;
							}
							if (obj3 == doubleMark)
							{
								double num16 = ScriptConvert.ToNumber(obj9);
								double num17 = sDbl[num8];
								goto IL_67E;
							}
							bool flag2;
							switch (num9)
							{
							case 14:
								flag2 = ScriptRuntime.cmp_LT(obj3, obj9);
								goto IL_74A;
							case 15:
								flag2 = ScriptRuntime.cmp_LE(obj3, obj9);
								goto IL_74A;
							case 16:
								flag2 = ScriptRuntime.cmp_LT(obj9, obj3);
								goto IL_74A;
							case 17:
								flag2 = ScriptRuntime.cmp_LE(obj9, obj3);
								goto IL_74A;
							}
							goto Block_49;
							IL_74A:
							IL_74B:
							stack[num8] = flag2;
							continue;
							IL_67E:
							switch (num9)
							{
							case 14:
							{
								double num16;
								double num17;
								flag2 = (num17 < num16);
								goto IL_74B;
							}
							case 15:
							{
								double num16;
								double num17;
								flag2 = (num17 <= num16);
								goto IL_74B;
							}
							case 16:
							{
								double num16;
								double num17;
								flag2 = (num17 > num16);
								goto IL_74B;
							}
							case 17:
							{
								double num16;
								double num17;
								flag2 = (num17 >= num16);
								goto IL_74B;
							}
							}
							goto Block_48;
						}
						case 20:
						{
							int num13 = Interpreter.stack_int32(frame, num8) & 31;
							num8--;
							double num17 = Interpreter.stack_double(frame, num8);
							stack[num8] = doubleMark;
							uint num18 = (uint)ScriptConvert.ToUint32(num17);
							sDbl[num8] = num18 >> num13;
							continue;
						}
						case 21:
							num8--;
							Interpreter.DoAdd(stack, sDbl, num8, cx);
							continue;
						case 22:
						case 23:
						case 24:
						case 25:
						{
							double num16 = Interpreter.stack_double(frame, num8);
							num8--;
							double num17 = Interpreter.stack_double(frame, num8);
							stack[num8] = doubleMark;
							switch (num9)
							{
							case 22:
								num17 -= num16;
								break;
							case 23:
								num17 *= num16;
								break;
							case 24:
								num17 /= num16;
								break;
							case 25:
								num17 %= num16;
								break;
							}
							sDbl[num8] = num17;
							continue;
						}
						case 26:
							stack[num8] = !Interpreter.stack_boolean(frame, num8);
							continue;
						case 27:
						{
							int num13 = Interpreter.stack_int32(frame, num8);
							stack[num8] = doubleMark;
							sDbl[num8] = (double)(~(double)num13);
							continue;
						}
						case 28:
						case 29:
						{
							double num16 = Interpreter.stack_double(frame, num8);
							stack[num8] = doubleMark;
							if (num9 == 29)
							{
								num16 = -num16;
							}
							sDbl[num8] = num16;
							continue;
						}
						case 30:
						{
							if (flag)
							{
								cx.instructionCount += 100;
							}
							num8 -= num;
							obj3 = stack[num8];
							if (obj3 is InterpretedFunction)
							{
								interpretedFunction3 = (InterpretedFunction)obj3;
								if (frame.fnOrScript.securityDomain == interpretedFunction3.securityDomain)
								{
									goto Block_110;
								}
							}
							if (!(obj3 is IFunction))
							{
								goto Block_111;
							}
							IFunction function = (IFunction)obj3;
							if (function is IdFunctionObject)
							{
								IdFunctionObject f = (IdFunctionObject)function;
								if (Continuation.IsContinuationConstructor(f))
								{
									Interpreter.captureContinuation(cx, frame, num8);
									continue;
								}
							}
							object[] argsArray = Interpreter.GetArgsArray(stack, sDbl, num8 + 1, num);
							stack[num8] = function.Construct(cx, frame.scope, argsArray);
							continue;
						}
						case 31:
						{
							object obj9 = stack[num8];
							if (obj9 == doubleMark)
							{
								obj9 = sDbl[num8];
							}
							num8--;
							obj3 = stack[num8];
							if (obj3 == doubleMark)
							{
								obj3 = sDbl[num8];
							}
							stack[num8] = ScriptRuntime.delete(obj3, obj9, cx);
							continue;
						}
						case 32:
							obj3 = stack[num8];
							if (obj3 == doubleMark)
							{
								obj3 = sDbl[num8];
							}
							stack[num8] = ScriptRuntime.Typeof(obj3);
							continue;
						case 33:
							obj3 = stack[num8];
							if (obj3 == doubleMark)
							{
								obj3 = sDbl[num8];
							}
							stack[num8] = ScriptRuntime.getObjectProp(obj3, text, cx);
							continue;
						case 34:
						case 77:
						case 78:
						{
							object obj9 = stack[num8];
							if (obj9 == doubleMark)
							{
								obj9 = sDbl[num8];
							}
							num8--;
							obj3 = stack[num8];
							if (obj3 == doubleMark)
							{
								obj3 = sDbl[num8];
							}
							switch (num9)
							{
							case 77:
								((ScriptableObject)obj3).DefineGetter(text, (ICallable)obj9);
								stack[num8] = obj9;
								break;
							case 78:
								((ScriptableObject)obj3).DefineSetter(text, (ICallable)obj9);
								stack[num8] = obj9;
								break;
							default:
								stack[num8] = ScriptRuntime.setObjectProp(obj3, text, obj9, cx);
								break;
							}
							continue;
						}
						case 35:
						{
							num8--;
							obj3 = stack[num8];
							if (obj3 == doubleMark)
							{
								obj3 = sDbl[num8];
							}
							object obj8 = stack[num8 + 1];
							if (obj8 != doubleMark)
							{
								obj5 = ScriptRuntime.getObjectElem(obj3, obj8, cx);
							}
							else
							{
								double num11 = sDbl[num8 + 1];
								obj5 = ScriptRuntime.getObjectIndex(obj3, num11, cx);
							}
							stack[num8] = obj5;
							continue;
						}
						case 36:
						{
							num8 -= 2;
							object obj9 = stack[num8 + 2];
							if (obj9 == doubleMark)
							{
								obj9 = sDbl[num8 + 2];
							}
							obj3 = stack[num8];
							if (obj3 == doubleMark)
							{
								obj3 = sDbl[num8];
							}
							object obj8 = stack[num8 + 1];
							if (obj8 != doubleMark)
							{
								obj5 = ScriptRuntime.setObjectElem(obj3, obj8, obj9, cx);
							}
							else
							{
								double num11 = sDbl[num8 + 1];
								obj5 = ScriptRuntime.setObjectIndex(obj3, num11, obj9, cx);
							}
							stack[num8] = obj5;
							continue;
						}
						case 38:
							stack[++num8] = ScriptRuntime.name(cx, frame.scope, text);
							continue;
						case 39:
							num8++;
							stack[num8] = doubleMark;
							sDbl[num8] = frame.idata.itsDoubleTable[num];
							continue;
						case 40:
							stack[++num8] = text;
							continue;
						case 41:
							stack[++num8] = null;
							continue;
						case 42:
							stack[++num8] = frame.thisObj;
							continue;
						case 43:
							stack[++num8] = false;
							continue;
						case 44:
							stack[++num8] = true;
							continue;
						case 45:
						case 46:
						{
							num8--;
							object obj9 = stack[num8 + 1];
							obj3 = stack[num8];
							bool flag2;
							double num19;
							double num20;
							if (obj9 == doubleMark)
							{
								num19 = sDbl[num8 + 1];
								if (obj3 == doubleMark)
								{
									num20 = sDbl[num8];
								}
								else
								{
									if (!CliHelper.IsNumber(obj3))
									{
										flag2 = false;
										goto IL_9B3;
									}
									num20 = Convert.ToDouble(obj3);
								}
								goto IL_9AA;
							}
							if (obj3 == doubleMark)
							{
								num20 = sDbl[num8];
								if (obj9 == doubleMark)
								{
									num19 = sDbl[num8 + 1];
								}
								else
								{
									if (!CliHelper.IsNumber(obj9))
									{
										flag2 = false;
										goto IL_9B3;
									}
									num19 = Convert.ToDouble(obj9);
								}
								goto IL_9AA;
							}
							flag2 = ScriptRuntime.shallowEq(obj3, obj9);
							IL_9B3:
							flag2 ^= (num9 == 46);
							stack[num8] = flag2;
							continue;
							IL_9AA:
							flag2 = (num20 == num19);
							goto IL_9B3;
						}
						case 47:
							stack[++num8] = frame.scriptRegExps[num];
							continue;
						case 48:
							stack[++num8] = ScriptRuntime.bind(cx, frame.scope, text);
							continue;
						case 49:
							goto IL_59F;
						case 50:
							num += frame.localShift;
							throwable = stack[num];
							goto IL_2577;
						case 51:
						case 52:
						{
							object obj9 = stack[num8];
							if (obj9 == doubleMark)
							{
								obj9 = sDbl[num8];
							}
							num8--;
							obj3 = stack[num8];
							if (obj3 == doubleMark)
							{
								obj3 = sDbl[num8];
							}
							bool flag2;
							if (num9 == 51)
							{
								flag2 = ScriptRuntime.In(obj3, obj9, cx);
							}
							else
							{
								flag2 = ScriptRuntime.InstanceOf(obj3, obj9, cx);
							}
							stack[num8] = flag2;
							continue;
						}
						case 53:
							num8++;
							num += frame.localShift;
							stack[num8] = stack[num];
							sDbl[num8] = sDbl[num];
							continue;
						case 54:
							goto IL_1B0E;
						case 55:
							goto IL_1A74;
						case 56:
						{
							num8--;
							num += frame.localShift;
							bool flag4 = frame.idata.itsICode[frame.pc] != 0;
							Exception t = (Exception)stack[num8 + 1];
							IScriptable lastCatchScope;
							if (!flag4)
							{
								lastCatchScope = null;
							}
							else
							{
								lastCatchScope = (IScriptable)stack[num];
							}
							stack[num] = ScriptRuntime.NewCatchScope(t, lastCatchScope, text, cx, frame.scope);
							frame.pc++;
							continue;
						}
						case 57:
						case 58:
							obj3 = stack[num8];
							if (obj3 == doubleMark)
							{
								obj3 = sDbl[num8];
							}
							num8--;
							num += frame.localShift;
							if (obj3 is IIdEnumerable)
							{
								stack[num] = ((IIdEnumerable)obj3).GetEnumeration(cx, num9 == 58);
							}
							else
							{
								stack[num] = new IdEnumeration(obj3, cx, num9 == 58);
							}
							continue;
						case 59:
						case 60:
						{
							num += frame.localShift;
							IdEnumeration idEnumeration = (IdEnumeration)stack[num];
							num8++;
							stack[num8] = ((num9 == 59) ? idEnumeration.MoveNext() : idEnumeration.Current(cx));
							continue;
						}
						case 61:
							stack[++num8] = frame.fnOrScript;
							continue;
						case 62:
							goto IL_C5A;
						case 65:
						{
							IRef rf = (IRef)stack[num8];
							stack[num8] = ScriptRuntime.refGet(rf, cx);
							continue;
						}
						case 66:
						{
							obj5 = stack[num8];
							if (obj5 == doubleMark)
							{
								obj5 = sDbl[num8];
							}
							num8--;
							IRef rf = (IRef)stack[num8];
							stack[num8] = ScriptRuntime.refSet(rf, obj5, cx);
							continue;
						}
						case 67:
						{
							IRef rf = (IRef)stack[num8];
							stack[num8] = ScriptRuntime.refDel(rf, cx);
							continue;
						}
						case 69:
						{
							object obj7 = stack[num8];
							if (obj7 == doubleMark)
							{
								obj7 = sDbl[num8];
							}
							stack[num8] = ScriptRuntime.specialRef(obj7, text, cx);
							continue;
						}
						case 70:
							obj5 = stack[num8];
							if (obj5 == doubleMark)
							{
								obj5 = sDbl[num8];
							}
							stack[num8] = ScriptRuntime.setDefaultNamespace(obj5, cx);
							continue;
						case 71:
							obj5 = stack[num8];
							if (obj5 != doubleMark)
							{
								stack[num8] = ScriptRuntime.escapeAttributeValue(obj5, cx);
							}
							continue;
						case 72:
							obj5 = stack[num8];
							if (obj5 != doubleMark)
							{
								stack[num8] = ScriptRuntime.escapeTextValue(obj5, cx);
							}
							continue;
						case 73:
						{
							object obj12 = stack[num8];
							if (obj12 == doubleMark)
							{
								obj12 = sDbl[num8];
							}
							num8--;
							object obj7 = stack[num8];
							if (obj7 == doubleMark)
							{
								obj7 = sDbl[num8];
							}
							stack[num8] = ScriptRuntime.memberRef(obj7, obj12, cx, num);
							continue;
						}
						case 74:
						{
							object obj12 = stack[num8];
							if (obj12 == doubleMark)
							{
								obj12 = sDbl[num8];
							}
							num8--;
							object obj13 = stack[num8];
							if (obj13 == doubleMark)
							{
								obj13 = sDbl[num8];
							}
							num8--;
							object obj7 = stack[num8];
							if (obj7 == doubleMark)
							{
								obj7 = sDbl[num8];
							}
							stack[num8] = ScriptRuntime.memberRef(obj7, obj13, obj12, cx, num);
							continue;
						}
						case 75:
						{
							object obj14 = stack[num8];
							if (obj14 == doubleMark)
							{
								obj14 = sDbl[num8];
							}
							stack[num8] = ScriptRuntime.nameRef(obj14, cx, frame.scope, num);
							continue;
						}
						case 76:
						{
							object obj14 = stack[num8];
							if (obj14 == doubleMark)
							{
								obj14 = sDbl[num8];
							}
							num8--;
							object obj13 = stack[num8];
							if (obj13 == doubleMark)
							{
								obj13 = sDbl[num8];
							}
							stack[num8] = ScriptRuntime.nameRef(obj13, obj14, cx, frame.scope, num);
							continue;
						}
						}
						break;
						continue;
						IL_1A74:
						if (!frame.useActivation)
						{
							stack2[num] = stack[num8];
							sDbl2[num] = sDbl[num8];
						}
						else
						{
							object obj4 = stack[num8];
							if (obj4 == doubleMark)
							{
								obj4 = sDbl[num8];
							}
							text = frame.idata.argNames[num];
							frame.scope.Put(text, frame.scope, obj4);
						}
						continue;
						IL_1B0E:
						num8++;
						if (!frame.useActivation)
						{
							stack[num8] = stack2[num];
							sDbl[num8] = sDbl2[num];
						}
						else
						{
							text = frame.idata.argNames[num];
							stack[num8] = frame.scope.Get(text, frame.scope);
						}
						continue;
						IL_2578:
						if (flag)
						{
							Interpreter.addInstructionCount(cx, frame, 2);
						}
						int @short = Interpreter.GetShort(itsICode, frame.pc);
						if (@short != 0)
						{
							frame.pc += @short - 1;
						}
						else
						{
							frame.pc = frame.idata.longJumps.getExistingInt(frame.pc);
						}
						if (flag)
						{
							frame.pcPrevBranch = frame.pc;
						}
						continue;
						IL_2577:
						goto IL_2578;
					}
					goto IL_2528;
					IL_59F:
					obj5 = stack[num8];
					if (obj5 == doubleMark)
					{
						obj5 = sDbl[num8];
					}
					num8--;
					index2 = Interpreter.GetIndex(itsICode, frame.pc);
					throwable = new EcmaScriptThrow(obj5, frame.idata.itsSourceFile, index2);
					goto IL_2696;
					Block_48:
					throw Context.CodeBug();
					Block_49:
					throw Context.CodeBug();
					Block_68:
					throwable = obj5;
					goto IL_2696;
					IL_C39:
					frame.result = stack[num8];
					frame.resultDbl = sDbl[num8];
					num8--;
					IL_C5A:
					goto IL_260C;
					IL_C5F:
					frame.result = value;
					goto IL_260C;
					Block_101:
					Interpreter.CallFrame parentFrame = frame;
					Interpreter.CallFrame callFrame2 = new Interpreter.CallFrame();
					if (num9 == -55)
					{
						parentFrame = frame.parentFrame;
					}
					Interpreter.initFrame(cx, scriptable, thisObj, stack, sDbl, num8 + 2, num, interpretedFunction, parentFrame, callFrame2);
					if (num9 == -55)
					{
						Interpreter.ExitFrame(cx, frame, null);
					}
					else
					{
						frame.savedStackTop = num8;
						frame.savedCallOp = num9;
					}
					frame = callFrame2;
					continue;
					Block_110:
					IScriptable scriptable2 = interpretedFunction3.CreateObject(cx, frame.scope);
					callFrame2 = new Interpreter.CallFrame();
					Interpreter.initFrame(cx, frame.scope, scriptable2, stack, sDbl, num8 + 1, num, interpretedFunction3, frame, callFrame2);
					stack[num8] = scriptable2;
					frame.savedStackTop = num8;
					frame.savedCallOp = num9;
					frame = callFrame2;
					continue;
					Block_111:
					if (obj3 == doubleMark)
					{
						obj3 = sDbl[num8];
					}
					throw ScriptRuntime.NotFunctionError(obj3);
					IL_2528:
					Interpreter.dumpICode(frame.idata);
					throw new ApplicationException(string.Concat(new object[]
					{
						"Unknown icode : ",
						num9,
						" @ pc : ",
						frame.pc - 1
					}));
					IL_260C:
					Interpreter.ExitFrame(cx, frame, null);
					obj = frame.result;
					num2 = frame.resultDbl;
					if (frame.parentFrame != null)
					{
						frame = frame.parentFrame;
						if (frame.frozen)
						{
							frame = frame.cloneFrozen();
						}
						Interpreter.setCallResult(frame, obj, num2);
						obj = null;
						continue;
					}
					goto IL_28EB;
				}
				catch (Exception ex)
				{
					if (throwable != null)
					{
						throw new ApplicationException();
					}
					throwable = ex;
				}
				goto IL_2695;
				continue;
				IL_2696:
				if (throwable == null)
				{
					Context.CodeBug();
				}
				continuationJump2 = null;
				int num21;
				if (throwable is EcmaScriptThrow)
				{
					num21 = 2;
				}
				else
				{
					if (throwable is EcmaScriptError)
					{
						num21 = 2;
					}
					else
					{
						if (throwable is EcmaScriptRuntimeException)
						{
							num21 = 2;
						}
						else
						{
							if (throwable is EcmaScriptException)
							{
								num21 = 1;
							}
							else
							{
								if (throwable is Exception)
								{
									num21 = 0;
								}
								else
								{
									num21 = 1;
									continuationJump2 = (Interpreter.ContinuationJump)throwable;
								}
							}
						}
					}
				}
				if (flag)
				{
					try
					{
						Interpreter.addInstructionCount(cx, frame, 100);
					}
					catch (ApplicationException ex2)
					{
						throwable = ex2;
						continuationJump2 = null;
						num21 = 0;
					}
				}
				if (frame.debuggerFrame != null && throwable is ApplicationException)
				{
					ApplicationException ex3 = (ApplicationException)throwable;
					try
					{
						frame.debuggerFrame.OnExceptionThrown(cx, ex3);
					}
					catch (Exception ex)
					{
						throwable = ex;
						continuationJump2 = null;
						num21 = 0;
					}
				}
				while (true)
				{
					if (num21 != 0)
					{
						bool onlyFinally = num21 != 2;
						num = Interpreter.getExceptionHandler(frame, onlyFinally);
						if (num >= 0)
						{
							break;
						}
					}
					Interpreter.ExitFrame(cx, frame, throwable);
					frame = frame.parentFrame;
					if (frame == null)
					{
						goto Block_18;
					}
					if (continuationJump2 != null && continuationJump2.branchFrame == frame)
					{
						goto Block_20;
					}
				}
				continue;
				Block_20:
				num = -1;
				continue;
				Block_18:
				if (continuationJump2 == null)
				{
					goto IL_28DC;
				}
				if (continuationJump2.branchFrame != null)
				{
					Context.CodeBug();
				}
				if (continuationJump2.capturedFrame != null)
				{
					num = -1;
					continue;
				}
				break;
				IL_2695:
				goto IL_2696;
			}
			obj = continuationJump2.result;
			num2 = continuationJump2.resultDbl;
			throwable = null;
			IL_28DC:
			IL_28EB:
			if (cx.previousInterpreterInvocations != null && cx.previousInterpreterInvocations.size() != 0)
			{
				cx.lastInterpreterFrame = cx.previousInterpreterInvocations.pop();
			}
			else
			{
				cx.lastInterpreterFrame = null;
				cx.previousInterpreterInvocations = null;
			}
			if (throwable == null)
			{
				return (obj != doubleMark) ? obj : num2;
			}
			if (throwable is StackOverflowVerifierException)
			{
				throw Context.ReportRuntimeError(ScriptRuntime.GetMessage("mag.too.deep.parser.recursion", new object[0]));
			}
			throw (Exception)throwable;
		}
		private static void initFrame(Context cx, IScriptable callerScope, IScriptable thisObj, object[] args, double[] argsDbl, int argShift, int argCount, InterpretedFunction fnOrScript, Interpreter.CallFrame parentFrame, Interpreter.CallFrame frame)
		{
			InterpreterData idata = fnOrScript.idata;
			bool flag = idata.itsNeedsActivation;
			DebugFrame debugFrame = null;
			if (cx.m_Debugger != null)
			{
				debugFrame = cx.m_Debugger.GetFrame(cx, idata);
				if (debugFrame != null)
				{
					flag = true;
				}
			}
			if (flag)
			{
				if (argsDbl != null)
				{
					args = Interpreter.GetArgsArray(args, argsDbl, argShift, argCount);
				}
				argShift = 0;
				argsDbl = null;
			}
			IScriptable scope;
			if (idata.itsFunctionType != 0)
			{
				if (!idata.useDynamicScope)
				{
					scope = fnOrScript.ParentScope;
				}
				else
				{
					scope = callerScope;
				}
				if (flag)
				{
					scope = ScriptRuntime.createFunctionActivation(fnOrScript, scope, args);
				}
			}
			else
			{
				scope = callerScope;
				ScriptRuntime.initScript(fnOrScript, thisObj, cx, scope, fnOrScript.idata.evalScriptFlag);
			}
			if (idata.itsNestedFunctions != null)
			{
				if (idata.itsFunctionType != 0 && !idata.itsNeedsActivation)
				{
					Context.CodeBug();
				}
				for (int i = 0; i < idata.itsNestedFunctions.Length; i++)
				{
					InterpreterData interpreterData = idata.itsNestedFunctions[i];
					if (interpreterData.itsFunctionType == 1)
					{
						Interpreter.initFunction(cx, scope, fnOrScript, i);
					}
				}
			}
			IScriptable[] scriptRegExps = null;
			if (idata.itsRegExpLiterals != null)
			{
				if (idata.itsFunctionType != 0)
				{
					scriptRegExps = fnOrScript.functionRegExps;
				}
				else
				{
					scriptRegExps = fnOrScript.createRegExpWraps(cx, scope);
				}
			}
			int num = idata.itsMaxVars + idata.itsMaxLocals - 1;
			int itsMaxFrameArray = idata.itsMaxFrameArray;
			if (itsMaxFrameArray != num + idata.itsMaxStack + 1)
			{
				Context.CodeBug();
			}
			bool flag2;
			object[] array;
			double[] array2;
			if (frame.stack != null && itsMaxFrameArray <= frame.stack.Length)
			{
				flag2 = true;
				array = frame.stack;
				array2 = frame.sDbl;
			}
			else
			{
				flag2 = false;
				array = new object[itsMaxFrameArray];
				array2 = new double[itsMaxFrameArray];
			}
			int num2 = idata.argCount;
			if (num2 > argCount)
			{
				num2 = argCount;
			}
			frame.parentFrame = parentFrame;
			frame.frameIndex = ((parentFrame == null) ? 0 : (parentFrame.frameIndex + 1));
			if (frame.frameIndex > cx.MaximumInterpreterStackDepth)
			{
				throw ScriptRuntime.TypeErrorById("msg.stackoverflow", new string[0]);
			}
			frame.frozen = false;
			frame.fnOrScript = fnOrScript;
			frame.idata = idata;
			frame.stack = array;
			frame.sDbl = array2;
			frame.varSource = frame;
			frame.localShift = idata.itsMaxVars;
			frame.emptyStackTop = num;
			frame.debuggerFrame = debugFrame;
			frame.useActivation = flag;
			frame.thisObj = thisObj;
			frame.scriptRegExps = scriptRegExps;
			frame.result = Undefined.Value;
			frame.pc = 0;
			frame.pcPrevBranch = 0;
			frame.pcSourceLineStart = idata.firstLinePC;
			frame.scope = scope;
			frame.savedStackTop = num;
			frame.savedCallOp = 0;
			Array.Copy(args, argShift, array, 0, num2);
			if (argsDbl != null)
			{
				Array.Copy(argsDbl, argShift, array2, 0, num2);
			}
			for (int i = num2; i != idata.itsMaxVars; i++)
			{
				array[i] = Undefined.Value;
			}
			if (flag2)
			{
				for (int i = num + 1; i != array.Length; i++)
				{
					array[i] = null;
				}
			}
			Interpreter.EnterFrame(cx, frame, args);
		}
		private static bool isFrameEnterExitRequired(Interpreter.CallFrame frame)
		{
			return frame.debuggerFrame != null || frame.idata.itsNeedsActivation;
		}
		private static void EnterFrame(Context cx, Interpreter.CallFrame frame, object[] args)
		{
			if (frame.debuggerFrame != null)
			{
				frame.debuggerFrame.OnEnter(cx, frame.scope, frame.thisObj, args);
			}
			if (frame.idata.itsNeedsActivation)
			{
				ScriptRuntime.enterActivationFunction(cx, frame.scope);
			}
		}
		private static void ExitFrame(Context cx, Interpreter.CallFrame frame, object throwable)
		{
			if (frame.idata.itsNeedsActivation)
			{
				ScriptRuntime.exitActivationFunction(cx);
			}
			if (frame.debuggerFrame != null)
			{
				try
				{
					if (throwable is Exception)
					{
						frame.debuggerFrame.OnExit(cx, true, throwable);
					}
					else
					{
						Interpreter.ContinuationJump continuationJump = (Interpreter.ContinuationJump)throwable;
						object obj;
						if (continuationJump == null)
						{
							obj = frame.result;
						}
						else
						{
							obj = continuationJump.result;
						}
						if (obj == UniqueTag.DoubleMark)
						{
							double resultDbl;
							if (continuationJump == null)
							{
								resultDbl = frame.resultDbl;
							}
							else
							{
								resultDbl = continuationJump.resultDbl;
							}
							obj = resultDbl;
						}
						frame.debuggerFrame.OnExit(cx, false, obj);
					}
				}
				catch (Exception ex)
				{
					Console.Error.WriteLine("USAGE WARNING: onExit terminated with exception");
					Console.Error.WriteLine(ex.ToString());
				}
			}
		}
		private static void setCallResult(Interpreter.CallFrame frame, object callResult, double callResultDbl)
		{
			if (frame.savedCallOp == 37)
			{
				frame.stack[frame.savedStackTop] = callResult;
				frame.sDbl[frame.savedStackTop] = callResultDbl;
			}
			else
			{
				if (frame.savedCallOp == 30)
				{
					if (callResult is IScriptable)
					{
						frame.stack[frame.savedStackTop] = callResult;
					}
				}
				else
				{
					Context.CodeBug();
				}
			}
			frame.savedCallOp = 0;
		}
		private static void captureContinuation(Context cx, Interpreter.CallFrame frame, int stackTop)
		{
			Continuation continuation = new Continuation();
			ScriptRuntime.setObjectProtoAndParent(continuation, ScriptRuntime.getTopCallScope(cx));
			Interpreter.CallFrame parentFrame = frame.parentFrame;
			while (parentFrame != null && !parentFrame.frozen)
			{
				parentFrame.frozen = true;
				for (int num = parentFrame.savedStackTop + 1; num != parentFrame.stack.Length; num++)
				{
					parentFrame.stack[num] = null;
				}
				if (parentFrame.savedCallOp == 37)
				{
					parentFrame.stack[parentFrame.savedStackTop] = null;
				}
				else
				{
					if (parentFrame.savedCallOp != 30)
					{
						Context.CodeBug();
					}
				}
				parentFrame = parentFrame.parentFrame;
			}
			continuation.initImplementation(frame.parentFrame);
			frame.stack[stackTop] = continuation;
		}
		private static int stack_int32(Interpreter.CallFrame frame, int i)
		{
			object obj = frame.stack[i];
			double d;
			if (obj == UniqueTag.DoubleMark)
			{
				d = frame.sDbl[i];
			}
			else
			{
				d = ScriptConvert.ToNumber(obj);
			}
			return ScriptConvert.ToInt32(d);
		}
		private static double stack_double(Interpreter.CallFrame frame, int i)
		{
			object obj = frame.stack[i];
			double result;
			if (obj != UniqueTag.DoubleMark)
			{
				result = ScriptConvert.ToNumber(obj);
			}
			else
			{
				result = frame.sDbl[i];
			}
			return result;
		}
		private static bool stack_boolean(Interpreter.CallFrame frame, int i)
		{
			object obj = frame.stack[i];
			bool result;
			if (obj is bool)
			{
				result = (bool)obj;
			}
			else
			{
				if (obj == UniqueTag.DoubleMark)
				{
					double num = frame.sDbl[i];
					result = (!double.IsNaN(num) && num != 0.0);
				}
				else
				{
					if (obj == null || obj == Undefined.Value)
					{
						result = false;
					}
					else
					{
						if (CliHelper.IsNumber(obj))
						{
							double num = Convert.ToDouble(obj);
							result = (!double.IsNaN(num) && num != 0.0);
						}
						else
						{
							result = ScriptConvert.ToBoolean(obj);
						}
					}
				}
			}
			return result;
		}
		private static void DoAdd(object[] stack, double[] sDbl, int stackTop, Context cx)
		{
			object obj = stack[stackTop + 1];
			object obj2 = stack[stackTop];
			double num;
			bool flag;
			if (obj == UniqueTag.DoubleMark)
			{
				num = sDbl[stackTop + 1];
				if (obj2 == UniqueTag.DoubleMark)
				{
					sDbl[stackTop] += num;
					return;
				}
				flag = true;
			}
			else
			{
				if (obj2 != UniqueTag.DoubleMark)
				{
					if (obj2 is IScriptable || obj is IScriptable)
					{
						stack[stackTop] = ScriptRuntime.Add(obj2, obj, cx);
					}
					else
					{
						if (obj2 is string)
						{
							string text = (string)obj2;
							string text2 = ScriptConvert.ToString(obj);
							stack[stackTop] = text + text2;
						}
						else
						{
							if (obj is string)
							{
								string text = ScriptConvert.ToString(obj2);
								string text2 = (string)obj;
								stack[stackTop] = text + text2;
							}
							else
							{
								double num2 = CliHelper.IsNumber(obj2) ? Convert.ToDouble(obj2) : ScriptConvert.ToNumber(obj2);
								double num3 = CliHelper.IsNumber(obj) ? Convert.ToDouble(obj) : ScriptConvert.ToNumber(obj);
								stack[stackTop] = UniqueTag.DoubleMark;
								sDbl[stackTop] = num2 + num3;
							}
						}
					}
					return;
				}
				num = sDbl[stackTop];
				obj2 = obj;
				flag = false;
			}
			if (obj2 is IScriptable)
			{
				obj = num;
				if (!flag)
				{
					object obj3 = obj2;
					obj2 = obj;
					obj = obj3;
				}
				stack[stackTop] = ScriptRuntime.Add(obj2, obj, cx);
			}
			else
			{
				if (obj2 is string)
				{
					string text = (string)obj2;
					string text2 = ScriptConvert.ToString(num);
					if (flag)
					{
						stack[stackTop] = text + text2;
					}
					else
					{
						stack[stackTop] = text2 + text;
					}
				}
				else
				{
					double num2 = CliHelper.IsNumber(obj2) ? Convert.ToDouble(obj2) : ScriptConvert.ToNumber(obj2);
					stack[stackTop] = UniqueTag.DoubleMark;
					sDbl[stackTop] = num2 + num;
				}
			}
		}
		private void addGotoOp(int gotoOp)
		{
			sbyte[] array = this.itsData.itsICode;
			int num = this.itsICodeTop;
			if (num + 3 > array.Length)
			{
				array = this.increaseICodeCapasity(3);
			}
			array[num] = (sbyte)gotoOp;
			this.itsICodeTop = num + 1 + 2;
		}
		private static object[] GetArgsArray(object[] stack, double[] sDbl, int shift, int count)
		{
			object[] result;
			if (count == 0)
			{
				result = ScriptRuntime.EmptyArgs;
			}
			else
			{
				object[] array = new object[count];
				int num = 0;
				while (num != count)
				{
					object obj = stack[shift];
					if (obj == UniqueTag.DoubleMark)
					{
						obj = sDbl[shift];
					}
					array[num] = obj;
					num++;
					shift++;
				}
				result = array;
			}
			return result;
		}
		private static void addInstructionCount(Context cx, Interpreter.CallFrame frame, int extra)
		{
			cx.instructionCount += frame.pc - frame.pcPrevBranch + extra;
			if (cx.instructionCount > cx.instructionThreshold)
			{
				cx.ObserveInstructionCount(cx.instructionCount);
				cx.instructionCount = 0;
			}
		}
	}
}
