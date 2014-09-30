using EcmaScript.NET.Collections;
using EcmaScript.NET.Helpers;
using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public class Parser
	{
		private class ParserException : ApplicationException
		{
		}
		internal const int CLEAR_TI_MASK = 65535;
		internal const int TI_AFTER_EOL = 65536;
		internal const int TI_CHECK_LABEL = 131072;
		internal CompilerEnvirons compilerEnv;
		private ErrorReporter errorReporter;
		private string sourceURI;
		internal bool calledByCompileFunction;
		private TokenStream ts;
		private int currentFlaggedToken;
		private int syntaxErrorCount;
		private NodeFactory nf;
		private int nestingOfFunction;
		private Decompiler decompiler;
		private string encodedSource;
		internal ScriptOrFnNode currentScriptOrFn;
		private int nestingOfWith;
		private Hashtable labelSet;
		private ObjArray loopSet;
		private ObjArray loopAndSwitchSet;
		private int currentStackIndex = 0;
		public string EncodedSource
		{
			get
			{
				return this.encodedSource;
			}
		}
		public bool Eof
		{
			get
			{
				return this.ts.eof();
			}
		}
		public Parser(CompilerEnvirons compilerEnv, ErrorReporter errorReporter)
		{
			this.compilerEnv = compilerEnv;
			this.errorReporter = errorReporter;
		}
		private Decompiler CreateDecompiler(CompilerEnvirons compilerEnv)
		{
			return new Decompiler();
		}
		internal void AddWarning(string messageId, string messageArg)
		{
			string message = ScriptRuntime.GetMessage(messageId, new object[]
			{
				messageArg
			});
			this.errorReporter.Warning(message, this.sourceURI, this.ts.Lineno, this.ts.Line, this.ts.Offset);
		}
		internal void AddError(string messageId)
		{
			this.syntaxErrorCount++;
			string message = ScriptRuntime.GetMessage(messageId, new object[0]);
			this.errorReporter.Error(message, this.sourceURI, this.ts.Lineno, this.ts.Line, this.ts.Offset);
		}
		internal Exception ReportError(string messageId)
		{
			this.AddError(messageId);
			throw new Parser.ParserException();
		}
		private int peekToken()
		{
			int num = this.currentFlaggedToken;
			if (num == 0)
			{
				while ((num = this.ts.Token) == 154 || num == 155)
				{
					if (num == 154)
					{
						this.decompiler.AddJScriptConditionalComment(this.ts.String);
					}
					else
					{
						this.decompiler.AddPreservedComment(this.ts.String);
					}
				}
				if (num == 1)
				{
					do
					{
						num = this.ts.Token;
						if (num == 154)
						{
							this.decompiler.AddJScriptConditionalComment(this.ts.String);
						}
						else
						{
							if (num == 155)
							{
								this.decompiler.AddPreservedComment(this.ts.String);
							}
						}
					}
					while (num == 1 || num == 154 || num == 155);
					num |= 65536;
				}
				this.currentFlaggedToken = num;
			}
			return num & 65535;
		}
		private int peekFlaggedToken()
		{
			this.peekToken();
			return this.currentFlaggedToken;
		}
		private void consumeToken()
		{
			this.currentFlaggedToken = 0;
		}
		private int nextToken()
		{
			int result = this.peekToken();
			this.consumeToken();
			return result;
		}
		private int nextFlaggedToken()
		{
			this.peekToken();
			int result = this.currentFlaggedToken;
			this.consumeToken();
			return result;
		}
		private bool matchToken(int toMatch)
		{
			int num = this.peekToken();
			bool result;
			if (num != toMatch)
			{
				result = false;
			}
			else
			{
				this.consumeToken();
				result = true;
			}
			return result;
		}
		private int peekTokenOrEOL()
		{
			int result = this.peekToken();
			if ((this.currentFlaggedToken & 65536) != 0)
			{
				result = 1;
			}
			return result;
		}
		private void setCheckForLabel()
		{
			if ((this.currentFlaggedToken & 65535) != 38)
			{
				throw Context.CodeBug();
			}
			this.currentFlaggedToken |= 131072;
		}
		private void mustMatchToken(int toMatch, string messageId)
		{
			if (!this.matchToken(toMatch))
			{
				this.ReportError(messageId);
			}
		}
		private void mustHaveXML()
		{
			if (!this.compilerEnv.isXmlAvailable())
			{
				this.ReportError("msg.XML.not.available");
			}
		}
		internal bool insideFunction()
		{
			return this.nestingOfFunction != 0;
		}
		private Node enterLoop(Node loopLabel)
		{
			Node node = this.nf.CreateLoopNode(loopLabel, this.ts.Lineno);
			if (this.loopSet == null)
			{
				this.loopSet = new ObjArray();
				if (this.loopAndSwitchSet == null)
				{
					this.loopAndSwitchSet = new ObjArray();
				}
			}
			this.loopSet.push(node);
			this.loopAndSwitchSet.push(node);
			return node;
		}
		private void exitLoop()
		{
			this.loopSet.pop();
			this.loopAndSwitchSet.pop();
		}
		private Node enterSwitch(Node switchSelector, int lineno, Node switchLabel)
		{
			Node node = this.nf.CreateSwitch(switchSelector, lineno);
			if (this.loopAndSwitchSet == null)
			{
				this.loopAndSwitchSet = new ObjArray();
			}
			this.loopAndSwitchSet.push(node);
			return node;
		}
		private void exitSwitch()
		{
			this.loopAndSwitchSet.pop();
		}
		public ScriptOrFnNode Parse(string sourceString, string sourceURI, int lineno)
		{
			this.sourceURI = sourceURI;
			this.ts = new TokenStream(this, null, sourceString, lineno);
			ScriptOrFnNode result;
			try
			{
				result = this.Parse();
			}
			catch (IOException)
			{
				throw new ApplicationException();
			}
			return result;
		}
		public ScriptOrFnNode Parse(StreamReader sourceReader, string sourceURI, int lineno)
		{
			this.sourceURI = sourceURI;
			this.ts = new TokenStream(this, sourceReader, null, lineno);
			return this.Parse();
		}
		private ScriptOrFnNode Parse()
		{
			this.decompiler = this.CreateDecompiler(this.compilerEnv);
			this.nf = new NodeFactory(this);
			this.currentScriptOrFn = this.nf.CreateScript();
			int currentOffset = this.decompiler.CurrentOffset;
			this.encodedSource = null;
			this.decompiler.AddToken(134);
			this.currentFlaggedToken = 0;
			this.syntaxErrorCount = 0;
			int lineno = this.ts.Lineno;
			Node node = this.nf.CreateLeaf(127);
			while (true)
			{
				int num = this.peekToken();
				if (num <= 0)
				{
					break;
				}
				Node child;
				if (num == 107)
				{
					this.consumeToken();
					try
					{
						child = this.function(this.calledByCompileFunction ? 2 : 1);
					}
					catch (Parser.ParserException)
					{
						break;
					}
				}
				else
				{
					child = this.statement();
				}
				this.nf.addChildToBack(node, child);
			}
			if (this.syntaxErrorCount != 0)
			{
				string text = Convert.ToString(this.syntaxErrorCount);
				text = ScriptRuntime.GetMessage("msg.got.syntax.errors", new object[]
				{
					text
				});
				throw this.errorReporter.RuntimeError(text, this.sourceURI, lineno, null, 0);
			}
			this.currentScriptOrFn.SourceName = this.sourceURI;
			this.currentScriptOrFn.BaseLineno = lineno;
			this.currentScriptOrFn.EndLineno = this.ts.Lineno;
			int currentOffset2 = this.decompiler.CurrentOffset;
			this.currentScriptOrFn.setEncodedSourceBounds(currentOffset, currentOffset2);
			this.nf.initScript(this.currentScriptOrFn, node);
			if (this.compilerEnv.isGeneratingSource())
			{
				this.encodedSource = this.decompiler.EncodedSource;
			}
			this.decompiler = null;
			return this.currentScriptOrFn;
		}
		private Node parseFunctionBody()
		{
			this.nestingOfFunction++;
			Node node = this.nf.CreateBlock(this.ts.Lineno);
			try
			{
				while (true)
				{
					int num = this.peekToken();
					int num2 = num;
					switch (num2)
					{
					case -1:
					case 0:
						goto IL_5B;
					default:
					{
						if (num2 == 84)
						{
							goto IL_5B;
						}
						Node child;
						if (num2 != 107)
						{
							child = this.statement();
						}
						else
						{
							this.consumeToken();
							child = this.function(1);
						}
						this.nf.addChildToBack(node, child);
						break;
					}
					}
				}
				IL_5B:;
			}
			catch (Parser.ParserException)
			{
			}
			finally
			{
				this.nestingOfFunction--;
			}
			return node;
		}
		private Node function(int functionType)
		{
			Node result;
			using (new StackOverflowVerifier(1024))
			{
				int functionType2 = functionType;
				int lineno = this.ts.Lineno;
				int num = this.decompiler.MarkFunctionStart(functionType);
				Node node = null;
				string text;
				if (this.matchToken(38))
				{
					text = this.ts.String;
					this.decompiler.AddName(text);
					if (!this.matchToken(85))
					{
						if (this.compilerEnv.isAllowMemberExprAsFunctionName())
						{
							Node pn = this.nf.CreateName(text);
							text = "";
							node = this.memberExprTail(false, pn);
						}
						this.mustMatchToken(85, "msg.no.paren.parms");
					}
				}
				else
				{
					if (this.matchToken(85))
					{
						text = "";
					}
					else
					{
						text = "";
						if (this.compilerEnv.isAllowMemberExprAsFunctionName())
						{
							node = this.memberExpr(false);
						}
						this.mustMatchToken(85, "msg.no.paren.parms");
					}
				}
				if (node != null)
				{
					functionType2 = 2;
				}
				bool flag = this.insideFunction();
				FunctionNode functionNode = this.nf.CreateFunction(text);
				if (flag || this.nestingOfWith > 0)
				{
					functionNode.itsIgnoreDynamicScope = true;
				}
				int functionIndex = this.currentScriptOrFn.addFunction(functionNode);
				ScriptOrFnNode scriptOrFnNode = this.currentScriptOrFn;
				this.currentScriptOrFn = functionNode;
				int num2 = this.nestingOfWith;
				this.nestingOfWith = 0;
				Hashtable hashtable = this.labelSet;
				this.labelSet = null;
				ObjArray objArray = this.loopSet;
				this.loopSet = null;
				ObjArray objArray2 = this.loopAndSwitchSet;
				this.loopAndSwitchSet = null;
				Node statements;
				int end;
				try
				{
					this.decompiler.AddToken(85);
					if (!this.matchToken(86))
					{
						bool flag2 = true;
						do
						{
							if (!flag2)
							{
								this.decompiler.AddToken(87);
							}
							flag2 = false;
							this.mustMatchToken(38, "msg.no.parm");
							string @string = this.ts.String;
							if (functionNode.hasParamOrVar(@string))
							{
								this.AddWarning("msg.dup.parms", @string);
							}
							functionNode.addParam(@string);
							this.decompiler.AddName(@string);
						}
						while (this.matchToken(87));
						this.mustMatchToken(86, "msg.no.paren.after.parms");
					}
					this.decompiler.AddToken(86);
					this.mustMatchToken(83, "msg.no.brace.body");
					this.decompiler.AddEol(83);
					statements = this.parseFunctionBody();
					this.mustMatchToken(84, "msg.no.brace.after.body");
					this.decompiler.AddToken(84);
					end = this.decompiler.MarkFunctionEnd(num);
					if (functionType != 2)
					{
						if (this.compilerEnv.LanguageVersion >= Context.Versions.JS1_2)
						{
							int num3 = this.peekTokenOrEOL();
							if (num3 == 107)
							{
								this.ReportError("msg.no.semi.stmt");
							}
						}
						this.decompiler.AddToken(1);
					}
				}
				finally
				{
					this.loopAndSwitchSet = objArray2;
					this.loopSet = objArray;
					this.labelSet = hashtable;
					this.nestingOfWith = num2;
					this.currentScriptOrFn = scriptOrFnNode;
				}
				functionNode.setEncodedSourceBounds(num, end);
				functionNode.SourceName = this.sourceURI;
				functionNode.BaseLineno = lineno;
				functionNode.EndLineno = this.ts.Lineno;
				Node node2 = this.nf.initFunction(functionNode, functionIndex, statements, functionType2);
				if (node != null)
				{
					node2 = this.nf.CreateAssignment(88, node, node2);
					if (functionType != 2)
					{
						node2 = this.nf.CreateExprStatementNoReturn(node2, lineno);
					}
				}
				result = node2;
			}
			return result;
		}
		private Node statements()
		{
			Node node = this.nf.CreateBlock(this.ts.Lineno);
			int num;
			while ((num = this.peekToken()) > 0 && num != 84)
			{
				this.nf.addChildToBack(node, this.statement());
			}
			return node;
		}
		private Node condition()
		{
			this.mustMatchToken(85, "msg.no.paren.cond");
			this.decompiler.AddToken(85);
			Node result = this.expr(false);
			this.mustMatchToken(86, "msg.no.paren.after.cond");
			this.decompiler.AddToken(86);
			return result;
		}
		private Node matchJumpLabelName()
		{
			Node node = null;
			int num = this.peekTokenOrEOL();
			if (num == 38)
			{
				this.consumeToken();
				string @string = this.ts.String;
				this.decompiler.AddName(@string);
				if (this.labelSet != null)
				{
					node = (Node)this.labelSet[@string];
				}
				if (node == null)
				{
					this.ReportError("msg.undef.label");
				}
			}
			return node;
		}
		private Node statement()
		{
			Node result;
			using (new StackOverflowVerifier(512))
			{
				try
				{
					Node node = this.statementHelper(null);
					if (node != null)
					{
						result = node;
						return result;
					}
				}
				catch (Parser.ParserException)
				{
				}
			}
			int lineno = this.ts.Lineno;
			while (true)
			{
				int num = this.peekTokenOrEOL();
				this.consumeToken();
				int num2 = num;
				switch (num2)
				{
				case -1:
				case 0:
				case 1:
					goto IL_9B;
				default:
					if (num2 == 80)
					{
						goto IL_9B;
					}
					break;
				}
			}
			IL_9B:
			result = this.nf.CreateExprStatement(this.nf.CreateName("error"), lineno);
			return result;
		}
		private Node statementHelper(Node statementLabel)
		{
			Node node = null;
			int num = this.peekToken();
			int num2 = num;
			int lineno;
			Node result;
			if (num2 <= 38)
			{
				if (num2 != -1)
				{
					if (num2 == 4)
					{
						if (!this.insideFunction())
						{
							this.ReportError("msg.bad.return");
						}
						this.consumeToken();
						this.decompiler.AddToken(4);
						lineno = this.ts.Lineno;
						num = this.peekTokenOrEOL();
						num2 = num;
						Node expr;
						switch (num2)
						{
						case -1:
						case 0:
						case 1:
							break;
						default:
							if (num2 != 80 && num2 != 84)
							{
								expr = this.expr(false);
								goto IL_C95;
							}
							break;
						}
						expr = null;
						IL_C95:
						node = this.nf.CreateReturn(expr, lineno);
						goto IL_1110;
					}
					if (num2 != 38)
					{
						goto IL_10E8;
					}
					lineno = this.ts.Lineno;
					string @string = this.ts.String;
					this.setCheckForLabel();
					node = this.expr(false);
					if (node.Type != 128)
					{
						if (this.compilerEnv.getterAndSetterSupport)
						{
							num = this.peekToken();
							if (num == 38)
							{
								if (this.ts.String == "getter" || this.ts.String == "setter")
								{
									node.Type = ((this.ts.String[0] == 'g') ? 77 : 78);
									this.decompiler.AddName(" " + this.ts.String);
									this.consumeToken();
									this.matchToken(88);
									this.decompiler.AddToken(88);
									this.matchToken(107);
									Node child = this.function(2);
									node.addChildToBack(child);
								}
							}
						}
						node = this.nf.CreateExprStatement(node, lineno);
						goto IL_1110;
					}
					if (this.peekToken() != 101)
					{
						Context.CodeBug();
					}
					this.consumeToken();
					this.decompiler.AddName(@string);
					this.decompiler.AddEol(101);
					if (this.labelSet == null)
					{
						this.labelSet = Hashtable.Synchronized(new Hashtable());
					}
					else
					{
						if (this.labelSet.ContainsKey(@string))
						{
							this.ReportError("msg.dup.label");
						}
					}
					bool flag;
					if (statementLabel == null)
					{
						flag = true;
						statementLabel = node;
					}
					else
					{
						flag = false;
					}
					this.labelSet[@string] = statementLabel;
					try
					{
						node = this.statementHelper(statementLabel);
					}
					finally
					{
						this.labelSet.Remove(@string);
					}
					if (flag)
					{
						node = this.nf.CreateLabeledStatement(statementLabel, node);
					}
					result = node;
					return result;
				}
			}
			else
			{
				if (num2 <= 83)
				{
					if (num2 == 49)
					{
						this.consumeToken();
						if (this.peekTokenOrEOL() == 1)
						{
							this.ReportError("msg.bad.throw.eol");
						}
						lineno = this.ts.Lineno;
						this.decompiler.AddToken(49);
						node = this.nf.CreateThrow(this.expr(false), lineno);
						goto IL_1110;
					}
					switch (num2)
					{
					case 79:
					{
						this.consumeToken();
						lineno = this.ts.Lineno;
						Node finallyBlock = null;
						this.decompiler.AddToken(79);
						this.decompiler.AddEol(83);
						Node tryBlock = this.statement();
						this.decompiler.AddEol(84);
						Node node2 = this.nf.CreateLeaf(127);
						bool flag2 = false;
						int num3 = this.peekToken();
						if (num3 == 122)
						{
							while (this.matchToken(122))
							{
								if (flag2)
								{
									this.ReportError("msg.catch.unreachable");
								}
								this.decompiler.AddToken(122);
								this.mustMatchToken(85, "msg.no.paren.catch");
								this.decompiler.AddToken(85);
								this.mustMatchToken(38, "msg.bad.catchcond");
								string string2 = this.ts.String;
								this.decompiler.AddName(string2);
								Node catchCond = null;
								if (this.matchToken(110))
								{
									this.decompiler.AddToken(110);
									catchCond = this.expr(false);
								}
								else
								{
									flag2 = true;
								}
								this.mustMatchToken(86, "msg.bad.catchcond");
								this.decompiler.AddToken(86);
								this.mustMatchToken(83, "msg.no.brace.catchblock");
								this.decompiler.AddEol(83);
								this.nf.addChildToBack(node2, this.nf.CreateCatch(string2, catchCond, this.statements(), this.ts.Lineno));
								this.mustMatchToken(84, "msg.no.brace.after.body");
								this.decompiler.AddEol(84);
							}
						}
						else
						{
							if (num3 != 123)
							{
								this.mustMatchToken(123, "msg.try.no.catchfinally");
							}
						}
						if (this.matchToken(123))
						{
							this.decompiler.AddToken(123);
							this.decompiler.AddEol(83);
							finallyBlock = this.statement();
							this.decompiler.AddEol(84);
						}
						node = this.nf.CreateTryCatchFinally(tryBlock, node2, finallyBlock, lineno);
						result = node;
						return result;
					}
					case 80:
						break;
					case 81:
					case 82:
						goto IL_10E8;
					case 83:
						this.consumeToken();
						if (statementLabel != null)
						{
							this.decompiler.AddToken(83);
						}
						node = this.statements();
						this.mustMatchToken(84, "msg.no.brace.block");
						if (statementLabel != null)
						{
							this.decompiler.AddEol(84);
						}
						result = node;
						return result;
					default:
						goto IL_10E8;
					}
				}
				else
				{
					switch (num2)
					{
					case 107:
						this.consumeToken();
						node = this.function(3);
						result = node;
						return result;
					case 108:
					case 109:
					case 111:
					case 113:
						goto IL_10E8;
					case 110:
					{
						this.consumeToken();
						this.decompiler.AddToken(110);
						lineno = this.ts.Lineno;
						Node node3 = this.condition();
						this.decompiler.AddEol(83);
						Node ifTrue = this.statement();
						Node ifFalse = null;
						if (this.matchToken(111))
						{
							this.decompiler.AddToken(84);
							this.decompiler.AddToken(111);
							this.decompiler.AddEol(83);
							ifFalse = this.statement();
						}
						this.decompiler.AddEol(84);
						node = this.nf.CreateIf(node3, ifTrue, ifFalse, lineno);
						result = node;
						return result;
					}
					case 112:
						this.consumeToken();
						this.decompiler.AddToken(112);
						lineno = this.ts.Lineno;
						this.mustMatchToken(85, "msg.no.paren.switch");
						this.decompiler.AddToken(85);
						node = this.enterSwitch(this.expr(false), lineno, statementLabel);
						try
						{
							this.mustMatchToken(86, "msg.no.paren.after.switch");
							this.decompiler.AddToken(86);
							this.mustMatchToken(83, "msg.no.brace.switch");
							this.decompiler.AddEol(83);
							bool flag3 = false;
							while (true)
							{
								num = this.nextToken();
								num2 = num;
								if (num2 == 84)
								{
									goto IL_221;
								}
								Node caseExpression;
								switch (num2)
								{
								case 113:
									this.decompiler.AddToken(113);
									caseExpression = this.expr(false);
									this.mustMatchToken(101, "msg.no.colon.case");
									this.decompiler.AddEol(101);
									goto IL_2C0;
								case 114:
									if (flag3)
									{
										this.ReportError("msg.double.switch.default");
									}
									this.decompiler.AddToken(114);
									flag3 = true;
									caseExpression = null;
									this.mustMatchToken(101, "msg.no.colon.case");
									this.decompiler.AddEol(101);
									goto IL_2C0;
								}
								break;
								IL_2C0:
								Node node4 = this.nf.CreateLeaf(127);
								while ((num = this.peekToken()) != 84 && num != 113 && num != 114 && num != 0)
								{
									this.nf.addChildToBack(node4, this.statement());
								}
								this.nf.addSwitchCase(node, caseExpression, node4);
							}
							this.ReportError("msg.bad.switch");
							IL_221:
							this.decompiler.AddEol(84);
							this.nf.closeSwitch(node);
						}
						finally
						{
							this.exitSwitch();
						}
						result = node;
						return result;
					case 114:
					{
						this.consumeToken();
						this.mustHaveXML();
						this.decompiler.AddToken(114);
						int lineno2 = this.ts.Lineno;
						if (!this.matchToken(38) || !this.ts.String.Equals("xml"))
						{
							this.ReportError("msg.bad.namespace");
						}
						this.decompiler.AddName(this.ts.String);
						if (!this.matchToken(38) || !this.ts.String.Equals("namespace"))
						{
							this.ReportError("msg.bad.namespace");
						}
						this.decompiler.AddName(this.ts.String);
						if (!this.matchToken(88))
						{
							this.ReportError("msg.bad.namespace");
						}
						this.decompiler.AddToken(88);
						Node expr2 = this.expr(false);
						node = this.nf.CreateDefaultNamespace(expr2, lineno2);
						goto IL_1110;
					}
					case 115:
					{
						this.consumeToken();
						this.decompiler.AddToken(115);
						Node node5 = this.enterLoop(statementLabel);
						try
						{
							Node node3 = this.condition();
							this.decompiler.AddEol(83);
							Node body = this.statement();
							this.decompiler.AddEol(84);
							node = this.nf.CreateWhile(node5, node3, body);
						}
						finally
						{
							this.exitLoop();
						}
						result = node;
						return result;
					}
					case 116:
					{
						this.consumeToken();
						this.decompiler.AddToken(116);
						this.decompiler.AddEol(83);
						Node node5 = this.enterLoop(statementLabel);
						try
						{
							Node body = this.statement();
							this.decompiler.AddToken(84);
							this.mustMatchToken(115, "msg.no.while.do");
							this.decompiler.AddToken(115);
							Node node3 = this.condition();
							node = this.nf.CreateDoWhile(node5, body, node3);
						}
						finally
						{
							this.exitLoop();
						}
						this.matchToken(80);
						this.decompiler.AddEol(80);
						result = node;
						return result;
					}
					case 117:
					{
						this.consumeToken();
						bool isForEach = false;
						this.decompiler.AddToken(117);
						Node node5 = this.enterLoop(statementLabel);
						try
						{
							Node node6 = null;
							if (this.matchToken(38))
							{
								this.decompiler.AddName(this.ts.String);
								if (this.ts.String.Equals("each"))
								{
									isForEach = true;
								}
								else
								{
									this.ReportError("msg.no.paren.for");
								}
							}
							this.mustMatchToken(85, "msg.no.paren.for");
							this.decompiler.AddToken(85);
							num = this.peekToken();
							Node node7;
							if (num == 80)
							{
								node7 = this.nf.CreateLeaf(126);
							}
							else
							{
								if (num == 120)
								{
									this.consumeToken();
									node7 = this.variables(true);
								}
								else
								{
									node7 = this.expr(true);
								}
							}
							Node node3;
							if (this.matchToken(51))
							{
								this.decompiler.AddToken(51);
								node3 = this.expr(false);
							}
							else
							{
								this.mustMatchToken(80, "msg.no.semi.for");
								this.decompiler.AddToken(80);
								if (this.peekToken() == 80)
								{
									node3 = this.nf.CreateLeaf(126);
								}
								else
								{
									node3 = this.expr(false);
								}
								this.mustMatchToken(80, "msg.no.semi.for.cond");
								this.decompiler.AddToken(80);
								if (this.peekToken() == 86)
								{
									node6 = this.nf.CreateLeaf(126);
								}
								else
								{
									node6 = this.expr(false);
								}
							}
							this.mustMatchToken(86, "msg.no.paren.for.ctrl");
							this.decompiler.AddToken(86);
							this.decompiler.AddEol(83);
							Node body = this.statement();
							this.decompiler.AddEol(84);
							if (node6 == null)
							{
								node = this.nf.CreateForIn(node5, node7, node3, body, isForEach);
							}
							else
							{
								node = this.nf.CreateFor(node5, node7, node3, node6, body);
							}
						}
						finally
						{
							this.exitLoop();
						}
						result = node;
						return result;
					}
					case 118:
					{
						this.consumeToken();
						lineno = this.ts.Lineno;
						this.decompiler.AddToken(118);
						Node node8 = this.matchJumpLabelName();
						if (node8 == null)
						{
							if (this.loopAndSwitchSet == null || this.loopAndSwitchSet.size() == 0)
							{
								this.ReportError("msg.bad.break");
								result = null;
								return result;
							}
							node8 = (Node)this.loopAndSwitchSet.peek();
						}
						node = this.nf.CreateBreak(node8, lineno);
						goto IL_1110;
					}
					case 119:
					{
						this.consumeToken();
						lineno = this.ts.Lineno;
						this.decompiler.AddToken(119);
						Node node9 = this.matchJumpLabelName();
						Node node5;
						if (node9 == null)
						{
							if (this.loopSet == null || this.loopSet.size() == 0)
							{
								this.ReportError("msg.continue.outside");
								result = null;
								return result;
							}
							node5 = (Node)this.loopSet.peek();
						}
						else
						{
							node5 = this.nf.getLabelLoop(node9);
							if (node5 == null)
							{
								this.ReportError("msg.continue.nonloop");
								result = null;
								return result;
							}
						}
						node = this.nf.CreateContinue(node5, lineno);
						goto IL_1110;
					}
					case 120:
						this.consumeToken();
						node = this.variables(false);
						goto IL_1110;
					case 121:
					{
						this.consumeToken();
						this.decompiler.AddToken(121);
						lineno = this.ts.Lineno;
						this.mustMatchToken(85, "msg.no.paren.with");
						this.decompiler.AddToken(85);
						Node obj = this.expr(false);
						this.mustMatchToken(86, "msg.no.paren.after.with");
						this.decompiler.AddToken(86);
						this.decompiler.AddEol(83);
						this.nestingOfWith++;
						Node body;
						try
						{
							body = this.statement();
						}
						finally
						{
							this.nestingOfWith--;
						}
						this.decompiler.AddEol(84);
						node = this.nf.CreateWith(obj, body, lineno);
						result = node;
						return result;
					}
					default:
						if (num2 != 156)
						{
							goto IL_10E8;
						}
						this.consumeToken();
						this.decompiler.AddToken(156);
						node = this.nf.CreateDebugger(this.ts.Lineno);
						goto IL_1110;
					}
				}
			}
			this.consumeToken();
			node = this.nf.CreateLeaf(126);
			result = node;
			return result;
			IL_10E8:
			lineno = this.ts.Lineno;
			node = this.expr(false);
			node = this.nf.CreateExprStatement(node, lineno);
			IL_1110:
			int num4 = this.peekFlaggedToken();
			num2 = (num4 & 65535);
			switch (num2)
			{
			case -1:
			case 0:
				break;
			default:
				if (num2 != 80)
				{
					if (num2 != 84)
					{
						if ((num4 & 65536) == 0)
						{
							this.ReportError("msg.no.semi.stmt");
						}
					}
				}
				else
				{
					this.consumeToken();
				}
				break;
			}
			this.decompiler.AddEol(80);
			result = node;
			return result;
		}
		private Node variables(bool inForInit)
		{
			Node node = this.nf.CreateVariables(this.ts.Lineno);
			bool flag = true;
			this.decompiler.AddToken(120);
			do
			{
				this.mustMatchToken(38, "msg.bad.var");
				string @string = this.ts.String;
				if (!flag)
				{
					this.decompiler.AddToken(87);
				}
				flag = false;
				this.decompiler.AddName(@string);
				this.currentScriptOrFn.addVar(@string);
				Node node2 = this.nf.CreateName(@string);
				if (this.matchToken(88))
				{
					this.decompiler.AddToken(88);
					Node child = this.assignExpr(inForInit);
					this.nf.addChildToBack(node2, child);
				}
				this.nf.addChildToBack(node, node2);
			}
			while (this.matchToken(87));
			return node;
		}
		private Node expr(bool inForInit)
		{
			Node node = this.assignExpr(inForInit);
			while (this.matchToken(87))
			{
				this.decompiler.AddToken(87);
				node = this.nf.CreateBinary(87, node, this.assignExpr(inForInit));
			}
			return node;
		}
		private Node assignExpr(bool inForInit)
		{
			Node node = this.condExpr(inForInit);
			int num = this.peekToken();
			if (88 <= num && num <= 99)
			{
				this.consumeToken();
				this.decompiler.AddToken(num);
				node = this.nf.CreateAssignment(num, node, this.assignExpr(inForInit));
			}
			return node;
		}
		private Node condExpr(bool inForInit)
		{
			Node node = this.orExpr(inForInit);
			Node result;
			if (this.matchToken(100))
			{
				this.decompiler.AddToken(100);
				Node ifTrue = this.assignExpr(false);
				this.mustMatchToken(101, "msg.no.colon.cond");
				this.decompiler.AddToken(101);
				Node ifFalse = this.assignExpr(inForInit);
				result = this.nf.CreateCondExpr(node, ifTrue, ifFalse);
			}
			else
			{
				result = node;
			}
			return result;
		}
		private Node orExpr(bool inForInit)
		{
			Node node = this.andExpr(inForInit);
			if (this.matchToken(102))
			{
				this.decompiler.AddToken(102);
				node = this.nf.CreateBinary(102, node, this.orExpr(inForInit));
			}
			return node;
		}
		private Node andExpr(bool inForInit)
		{
			Node node = this.bitOrExpr(inForInit);
			if (this.matchToken(103))
			{
				this.decompiler.AddToken(103);
				node = this.nf.CreateBinary(103, node, this.andExpr(inForInit));
			}
			return node;
		}
		private Node bitOrExpr(bool inForInit)
		{
			Node node = this.bitXorExpr(inForInit);
			while (this.matchToken(9))
			{
				this.decompiler.AddToken(9);
				node = this.nf.CreateBinary(9, node, this.bitXorExpr(inForInit));
			}
			return node;
		}
		private Node bitXorExpr(bool inForInit)
		{
			Node node = this.bitAndExpr(inForInit);
			while (this.matchToken(10))
			{
				this.decompiler.AddToken(10);
				node = this.nf.CreateBinary(10, node, this.bitAndExpr(inForInit));
			}
			return node;
		}
		private Node bitAndExpr(bool inForInit)
		{
			Node node = this.eqExpr(inForInit);
			while (this.matchToken(11))
			{
				this.decompiler.AddToken(11);
				node = this.nf.CreateBinary(11, node, this.eqExpr(inForInit));
			}
			return node;
		}
		private Node eqExpr(bool inForInit)
		{
			Node node = this.relExpr(inForInit);
			while (true)
			{
				int num = this.peekToken();
				int num2 = num;
				switch (num2)
				{
				case 12:
				case 13:
					break;
				default:
					switch (num2)
					{
					case 45:
					case 46:
						goto IL_3F;
					}
					return node;
				}
				IL_3F:
				this.consumeToken();
				int token = num;
				int nodeType = num;
				if (this.compilerEnv.LanguageVersion == Context.Versions.JS1_2)
				{
					num2 = num;
					switch (num2)
					{
					case 12:
						nodeType = 45;
						break;
					case 13:
						nodeType = 46;
						break;
					default:
						switch (num2)
						{
						case 45:
							token = 12;
							break;
						case 46:
							token = 13;
							break;
						}
						break;
					}
				}
				this.decompiler.AddToken(token);
				node = this.nf.CreateBinary(nodeType, node, this.relExpr(inForInit));
			}
			return node;
		}
		private Node relExpr(bool inForInit)
		{
			Node node = this.shiftExpr();
			while (true)
			{
				int num = this.peekToken();
				int num2 = num;
				switch (num2)
				{
				case 14:
				case 15:
				case 16:
				case 17:
					break;
				default:
					switch (num2)
					{
					case 51:
						if (!inForInit)
						{
							goto IL_5B;
						}
						break;
					case 52:
						goto IL_5B;
					}
					return node;
				}
				IL_5B:
				this.consumeToken();
				this.decompiler.AddToken(num);
				node = this.nf.CreateBinary(num, node, this.shiftExpr());
			}
			return node;
		}
		private Node shiftExpr()
		{
			Node node = this.addExpr();
			while (true)
			{
				int num = this.peekToken();
				switch (num)
				{
				case 18:
				case 19:
				case 20:
					this.consumeToken();
					this.decompiler.AddToken(num);
					node = this.nf.CreateBinary(num, node, this.addExpr());
					continue;
				}
				break;
			}
			return node;
		}
		private Node addExpr()
		{
			Node node = this.mulExpr();
			while (true)
			{
				int num = this.peekToken();
				if (num != 21 && num != 22)
				{
					break;
				}
				this.consumeToken();
				this.decompiler.AddToken(num);
				node = this.nf.CreateBinary(num, node, this.mulExpr());
			}
			return node;
		}
		private Node mulExpr()
		{
			Node node = this.unaryExpr();
			while (true)
			{
				int num = this.peekToken();
				switch (num)
				{
				case 23:
				case 24:
				case 25:
					this.consumeToken();
					this.decompiler.AddToken(num);
					node = this.nf.CreateBinary(num, node, this.unaryExpr());
					continue;
				}
				break;
			}
			return node;
		}
		private Node unaryExpr()
		{
			Node result;
			using (new StackOverflowVerifier(4096))
			{
				int num = this.peekToken();
				int num2 = num;
				Node node;
				if (num2 <= 27)
				{
					if (num2 == -1)
					{
						this.consumeToken();
						result = this.nf.CreateName("err");
						return result;
					}
					if (num2 != 14)
					{
						switch (num2)
						{
						case 21:
							this.consumeToken();
							this.decompiler.AddToken(28);
							result = this.nf.CreateUnary(28, this.unaryExpr());
							return result;
						case 22:
							this.consumeToken();
							this.decompiler.AddToken(29);
							result = this.nf.CreateUnary(29, this.unaryExpr());
							return result;
						case 23:
						case 24:
						case 25:
							goto IL_1B1;
						case 26:
						case 27:
							break;
						default:
							goto IL_1B1;
						}
					}
					else
					{
						if (this.compilerEnv.isXmlAvailable())
						{
							this.consumeToken();
							node = this.xmlInitializer();
							result = this.memberExprTail(true, node);
							return result;
						}
						goto IL_1B1;
					}
				}
				else
				{
					switch (num2)
					{
					case 31:
						this.consumeToken();
						this.decompiler.AddToken(31);
						result = this.nf.CreateUnary(31, this.unaryExpr());
						return result;
					case 32:
						break;
					default:
						switch (num2)
						{
						case 104:
						case 105:
							this.consumeToken();
							this.decompiler.AddToken(num);
							result = this.nf.CreateIncDec(num, false, this.memberExpr(true));
							return result;
						default:
							if (num2 != 124)
							{
								goto IL_1B1;
							}
							break;
						}
						break;
					}
				}
				this.consumeToken();
				this.decompiler.AddToken(num);
				result = this.nf.CreateUnary(num, this.unaryExpr());
				return result;
				IL_1B1:
				node = this.memberExpr(true);
				num = this.peekTokenOrEOL();
				if (num == 104 || num == 105)
				{
					this.consumeToken();
					this.decompiler.AddToken(num);
					result = this.nf.CreateIncDec(num, true, node);
				}
				else
				{
					result = node;
				}
			}
			return result;
		}
		private Node xmlInitializer()
		{
			int num = this.ts.FirstXMLToken;
			Node result;
			if (num != 143 && num != 146)
			{
				this.ReportError("msg.syntax");
				result = null;
			}
			else
			{
				Node node = this.nf.CreateLeaf(30);
				this.decompiler.AddToken(30);
				this.decompiler.AddToken(106);
				string @string = this.ts.String;
				bool flag = @string.Trim().StartsWith("<>");
				this.decompiler.AddName(flag ? "XMLList" : "XML");
				Node node2 = this.nf.CreateName(flag ? "XMLList" : "XML");
				this.nf.addChildToBack(node, node2);
				node2 = null;
				int num2;
				while (true)
				{
					num2 = num;
					if (num2 != 143)
					{
						break;
					}
					@string = this.ts.String;
					this.decompiler.AddString(@string);
					this.mustMatchToken(83, "msg.syntax");
					this.decompiler.AddToken(83);
					Node node3 = (this.peekToken() == 84) ? this.nf.CreateString("") : this.expr(false);
					this.mustMatchToken(84, "msg.syntax");
					this.decompiler.AddToken(84);
					if (node2 == null)
					{
						node2 = this.nf.CreateString(@string);
					}
					else
					{
						node2 = this.nf.CreateBinary(21, node2, this.nf.CreateString(@string));
					}
					int nodeType;
					if (this.ts.XMLAttribute)
					{
						nodeType = 71;
					}
					else
					{
						nodeType = 72;
					}
					node3 = this.nf.CreateUnary(nodeType, node3);
					node2 = this.nf.CreateBinary(21, node2, node3);
					num = this.ts.NextXMLToken;
				}
				if (num2 != 146)
				{
					this.ReportError("msg.syntax");
					result = null;
				}
				else
				{
					@string = this.ts.String;
					this.decompiler.AddString(@string);
					if (node2 == null)
					{
						node2 = this.nf.CreateString(@string);
					}
					else
					{
						node2 = this.nf.CreateBinary(21, node2, this.nf.CreateString(@string));
					}
					this.nf.addChildToBack(node, node2);
					result = node;
				}
			}
			return result;
		}
		private void argumentList(Node listNode)
		{
			if (!this.matchToken(86))
			{
				bool flag = true;
				do
				{
					if (!flag)
					{
						this.decompiler.AddToken(87);
					}
					flag = false;
					this.nf.addChildToBack(listNode, this.assignExpr(false));
				}
				while (this.matchToken(87));
				this.mustMatchToken(86, "msg.no.paren.arg");
			}
			this.decompiler.AddToken(86);
		}
		private Node memberExpr(bool allowCallSyntax)
		{
			int num = this.peekToken();
			Node node;
			if (num == 30)
			{
				this.consumeToken();
				this.decompiler.AddToken(30);
				node = this.nf.CreateCallOrNew(30, this.memberExpr(false));
				if (this.matchToken(85))
				{
					this.decompiler.AddToken(85);
					this.argumentList(node);
				}
				num = this.peekToken();
				if (num == 83)
				{
					this.nf.addChildToBack(node, this.primaryExpr());
				}
			}
			else
			{
				node = this.primaryExpr();
			}
			return this.memberExprTail(allowCallSyntax, node);
		}
		private Node memberExprTail(bool allowCallSyntax, Node pn)
		{
			while (true)
			{
				int num = this.peekToken();
				int num2 = num;
				if (num2 <= 85)
				{
					if (num2 != 81)
					{
						if (num2 != 85)
						{
							break;
						}
						if (!allowCallSyntax)
						{
							break;
						}
						this.consumeToken();
						this.decompiler.AddToken(85);
						pn = this.nf.CreateCallOrNew(37, pn);
						this.argumentList(pn);
					}
					else
					{
						this.consumeToken();
						this.decompiler.AddToken(81);
						pn = this.nf.CreateElementGet(pn, null, this.expr(false), 0);
						this.mustMatchToken(82, "msg.no.bracket.index");
						this.decompiler.AddToken(82);
					}
				}
				else
				{
					if (num2 != 106 && num2 != 141)
					{
						if (num2 != 144)
						{
							break;
						}
						this.consumeToken();
						this.mustHaveXML();
						this.decompiler.AddToken(144);
						pn = this.nf.CreateDotQuery(pn, this.expr(false), this.ts.Lineno);
						this.mustMatchToken(86, "msg.no.paren");
						this.decompiler.AddToken(86);
					}
					else
					{
						this.consumeToken();
						this.decompiler.AddToken(num);
						int memberTypeFlags = 0;
						if (num == 141)
						{
							this.mustHaveXML();
							memberTypeFlags = 4;
						}
						if (!this.compilerEnv.isXmlAvailable())
						{
							this.mustMatchToken(38, "msg.no.name.after.dot");
							string @string = this.ts.String;
							this.decompiler.AddName(@string);
							pn = this.nf.CreatePropertyGet(pn, null, @string, memberTypeFlags);
						}
						else
						{
							num = this.nextToken();
							num2 = num;
							if (num2 != 23)
							{
								if (num2 != 38)
								{
									if (num2 != 145)
									{
										this.ReportError("msg.no.name.after.dot");
									}
									else
									{
										this.decompiler.AddToken(145);
										pn = this.attributeAccess(pn, memberTypeFlags);
									}
								}
								else
								{
									string @string = this.ts.String;
									this.decompiler.AddName(@string);
									pn = this.propertyName(pn, @string, memberTypeFlags);
								}
							}
							else
							{
								this.decompiler.AddName("*");
								pn = this.propertyName(pn, "*", memberTypeFlags);
							}
						}
					}
				}
			}
			return pn;
		}
		private Node attributeAccess(Node pn, int memberTypeFlags)
		{
			memberTypeFlags |= 2;
			int num = this.nextToken();
			int num2 = num;
			if (num2 != 23)
			{
				if (num2 != 38)
				{
					if (num2 != 81)
					{
						this.ReportError("msg.no.name.after.xmlAttr");
						pn = this.nf.CreatePropertyGet(pn, null, "?", memberTypeFlags);
					}
					else
					{
						this.decompiler.AddToken(81);
						pn = this.nf.CreateElementGet(pn, null, this.expr(false), memberTypeFlags);
						this.mustMatchToken(82, "msg.no.bracket.index");
						this.decompiler.AddToken(82);
					}
				}
				else
				{
					string @string = this.ts.String;
					this.decompiler.AddName(@string);
					pn = this.propertyName(pn, @string, memberTypeFlags);
				}
			}
			else
			{
				this.decompiler.AddName("*");
				pn = this.propertyName(pn, "*", memberTypeFlags);
			}
			return pn;
		}
		private Node propertyName(Node pn, string name, int memberTypeFlags)
		{
			string ns = null;
			Node result;
			if (this.matchToken(142))
			{
				this.decompiler.AddToken(142);
				ns = name;
				int num = this.nextToken();
				int num2 = num;
				if (num2 != 23)
				{
					if (num2 != 38)
					{
						if (num2 == 81)
						{
							this.decompiler.AddToken(81);
							pn = this.nf.CreateElementGet(pn, ns, this.expr(false), memberTypeFlags);
							this.mustMatchToken(82, "msg.no.bracket.index");
							this.decompiler.AddToken(82);
							result = pn;
							return result;
						}
						this.ReportError("msg.no.name.after.coloncolon");
						name = "?";
					}
					else
					{
						name = this.ts.String;
						this.decompiler.AddName(name);
					}
				}
				else
				{
					this.decompiler.AddName("*");
					name = "*";
				}
			}
			pn = this.nf.CreatePropertyGet(pn, ns, name, memberTypeFlags);
			result = pn;
			return result;
		}
		private Node primaryExpr()
		{
			Node result;
			try
			{
				if (this.currentStackIndex++ > 1000)
				{
					this.currentStackIndex = 0;
					throw Context.ReportRuntimeError(ScriptRuntime.GetMessage("mag.too.deep.parser.recursion", new object[0]), this.sourceURI, this.ts.Lineno, null, 0);
				}
				int num = this.nextFlaggedToken();
				int num2 = num & 65535;
				int num3 = num2;
				if (num3 <= 85)
				{
					if (num3 <= 24)
					{
						switch (num3)
						{
						case -1:
							goto IL_618;
						case 0:
							this.ReportError("msg.unexpected.eof");
							goto IL_618;
						default:
							if (num3 != 24)
							{
								goto IL_607;
							}
							break;
						}
					}
					else
					{
						switch (num3)
						{
						case 38:
						{
							string @string = this.ts.String;
							if ((num & 131072) != 0)
							{
								if (this.peekToken() == 101)
								{
									result = this.nf.CreateLabel(this.ts.Lineno);
									return result;
								}
							}
							this.decompiler.AddName(@string);
							Node node;
							if (this.compilerEnv.isXmlAvailable())
							{
								node = this.propertyName(null, @string, 0);
							}
							else
							{
								node = this.nf.CreateName(@string);
							}
							result = node;
							return result;
						}
						case 39:
						{
							double number = this.ts.Number;
							this.decompiler.AddNumber(number);
							result = this.nf.CreateNumber(number);
							return result;
						}
						case 40:
						{
							string string2 = this.ts.String;
							this.decompiler.AddString(string2);
							result = this.nf.CreateString(string2);
							return result;
						}
						case 41:
						case 42:
						case 43:
						case 44:
							this.decompiler.AddToken(num2);
							result = this.nf.CreateLeaf(num2);
							return result;
						default:
							switch (num3)
							{
							case 81:
							{
								ObjArray objArray = new ObjArray();
								int num4 = 0;
								this.decompiler.AddToken(81);
								bool flag = true;
								while (true)
								{
									num2 = this.peekToken();
									if (num2 == 87)
									{
										this.consumeToken();
										this.decompiler.AddToken(87);
										if (!flag)
										{
											flag = true;
										}
										else
										{
											objArray.add(null);
											num4++;
										}
									}
									else
									{
										if (num2 == 82)
										{
											break;
										}
										if (!flag)
										{
											this.ReportError("msg.no.bracket.arg");
										}
										objArray.add(this.assignExpr(false));
										flag = false;
									}
								}
								this.consumeToken();
								this.decompiler.AddToken(82);
								result = this.nf.CreateArrayLiteral(objArray, num4);
								return result;
							}
							case 82:
							case 84:
								goto IL_607;
							case 83:
							{
								ObjArray objArray = new ObjArray();
								this.decompiler.AddToken(83);
								if (!this.matchToken(84))
								{
									bool flag2 = true;
									while (true)
									{
										if (!flag2)
										{
											this.decompiler.AddToken(87);
										}
										else
										{
											flag2 = false;
										}
										num2 = this.peekToken();
										num3 = num2;
										object indexObject;
										switch (num3)
										{
										case 38:
										case 40:
										{
											this.consumeToken();
											if (this.compilerEnv.getterAndSetterSupport)
											{
												if (num2 == 38 && (this.CheckForGetOrSet(objArray) || this.CheckForGetterOrSetter(objArray)))
												{
													goto IL_3AC;
												}
											}
											string string2 = this.ts.String;
											if (num2 == 38)
											{
												this.decompiler.AddName(string2);
											}
											else
											{
												this.decompiler.AddString(string2);
											}
											indexObject = ScriptRuntime.getIndexObject(string2);
											goto IL_377;
										}
										case 39:
										{
											this.consumeToken();
											double number = this.ts.Number;
											this.decompiler.AddNumber(number);
											indexObject = ScriptRuntime.getIndexObject(number);
											goto IL_377;
										}
										}
										break;
										IL_3AC:
										if (!this.matchToken(87))
										{
											goto IL_3BC;
										}
										continue;
										IL_377:
										this.mustMatchToken(101, "msg.no.colon.prop");
										this.decompiler.AddToken(64);
										objArray.add(indexObject);
										objArray.add(this.assignExpr(false));
										goto IL_3AC;
									}
									if (num3 != 84)
									{
										this.ReportError("msg.bad.prop");
									}
									IL_3BC:
									this.mustMatchToken(84, "msg.no.brace.prop");
								}
								this.decompiler.AddToken(84);
								result = this.nf.CreateObjectLiteral(objArray);
								return result;
							}
							case 85:
							{
								this.decompiler.AddToken(85);
								Node node = this.expr(false);
								this.decompiler.AddToken(86);
								this.mustMatchToken(86, "msg.no.paren");
								result = node;
								return result;
							}
							default:
								goto IL_607;
							}
							break;
						}
					}
				}
				else
				{
					if (num3 <= 107)
					{
						if (num3 != 98)
						{
							if (num3 != 107)
							{
								goto IL_607;
							}
							result = this.function(2);
							return result;
						}
					}
					else
					{
						if (num3 == 125)
						{
							this.ReportError("msg.reserved.id");
							goto IL_618;
						}
						if (num3 != 145)
						{
							goto IL_607;
						}
						this.mustHaveXML();
						this.decompiler.AddToken(145);
						Node node = this.attributeAccess(null, 0);
						result = node;
						return result;
					}
				}
				this.ts.readRegExp(num2);
				string regExpFlags = this.ts.regExpFlags;
				this.ts.regExpFlags = null;
				string string3 = this.ts.String;
				this.decompiler.AddRegexp(string3, regExpFlags);
				int regexpIndex = this.currentScriptOrFn.addRegexp(string3, regExpFlags);
				result = this.nf.CreateRegExp(regexpIndex);
				return result;
				IL_607:
				this.ReportError("msg.syntax");
				IL_618:
				result = null;
			}
			finally
			{
				this.currentStackIndex--;
			}
			return result;
		}
		private bool CheckForGetOrSet(ObjArray elems)
		{
			string @string = this.ts.String;
			bool result;
			if (@string != "get" && @string != "set")
			{
				result = false;
			}
			else
			{
				int num = this.peekToken();
				if (num != 38)
				{
					result = false;
				}
				else
				{
					this.consumeToken();
					string string2 = this.ts.String;
					this.decompiler.AddName(string2);
					Node value = this.function(2);
					object indexObject = ScriptRuntime.getIndexObject(string2);
					elems.add((@string[0] == 'g') ? new Node.GetterPropertyLiteral(indexObject) : new Node.SetterPropertyLiteral(indexObject));
					elems.add(value);
					result = true;
				}
			}
			return result;
		}
		private bool CheckForGetterOrSetter(ObjArray elems)
		{
			string @string = this.ts.String;
			this.consumeToken();
			int num = this.peekToken();
			bool result;
			if (num != 38)
			{
				result = false;
			}
			else
			{
				string string2 = this.ts.String;
				if (string2 != "getter" && string2 != "setter")
				{
					result = false;
				}
				else
				{
					this.consumeToken();
					this.matchToken(101);
					this.matchToken(107);
					Node value = this.function(2);
					object indexObject = ScriptRuntime.getIndexObject(@string);
					elems.add((string2[0] == 'g') ? new Node.GetterPropertyLiteral(indexObject) : new Node.SetterPropertyLiteral(indexObject));
					elems.add(value);
					result = true;
				}
			}
			return result;
		}
	}
}
