using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
namespace Antlr.Runtime
{
	public abstract class BaseRecognizer
	{
		public const int MEMO_RULE_FAILED = -2;
		public const int MEMO_RULE_UNKNOWN = -1;
		public const int INITIAL_FOLLOW_STACK_SIZE = 100;
		public const int DEFAULT_TOKEN_CHANNEL = 0;
		public const int HIDDEN = 99;
		public static readonly string NEXT_TOKEN_RULE_NAME = "nextToken";
		protected internal RecognizerSharedState state;
		public abstract IIntStream Input
		{
			get;
		}
		public int BacktrackingLevel
		{
			get
			{
				return this.state.backtracking;
			}
			set
			{
				this.state.backtracking = value;
			}
		}
		public int NumberOfSyntaxErrors
		{
			get
			{
				return this.state.syntaxErrors;
			}
		}
		public virtual string GrammarFileName
		{
			get
			{
				return null;
			}
		}
		public abstract string SourceName
		{
			get;
		}
		public virtual string[] TokenNames
		{
			get
			{
				return null;
			}
		}
		public BaseRecognizer()
		{
			this.state = new RecognizerSharedState();
		}
		public BaseRecognizer(RecognizerSharedState state)
		{
			if (state == null)
			{
				state = new RecognizerSharedState();
			}
			this.state = state;
		}
		public virtual void BeginBacktrack(int level)
		{
		}
		public virtual void EndBacktrack(int level, bool successful)
		{
		}
		public bool Failed()
		{
			return this.state.failed;
		}
		public virtual void Reset()
		{
			if (this.state == null)
			{
				return;
			}
			this.state.followingStackPointer = -1;
			this.state.errorRecovery = false;
			this.state.lastErrorIndex = -1;
			this.state.failed = false;
			this.state.syntaxErrors = 0;
			this.state.backtracking = 0;
			int num = 0;
			while (this.state.ruleMemo != null && num < this.state.ruleMemo.Length)
			{
				this.state.ruleMemo[num] = null;
				num++;
			}
		}
		public virtual object Match(IIntStream input, int ttype, BitSet follow)
		{
			object currentInputSymbol = this.GetCurrentInputSymbol(input);
			if (input.LA(1) == ttype)
			{
				input.Consume();
				this.state.errorRecovery = false;
				this.state.failed = false;
				return currentInputSymbol;
			}
			if (this.state.backtracking > 0)
			{
				this.state.failed = true;
				return currentInputSymbol;
			}
			return this.RecoverFromMismatchedToken(input, ttype, follow);
		}
		public virtual void MatchAny(IIntStream input)
		{
			this.state.errorRecovery = false;
			this.state.failed = false;
			input.Consume();
		}
		public bool MismatchIsUnwantedToken(IIntStream input, int ttype)
		{
			return input.LA(2) == ttype;
		}
		public bool MismatchIsMissingToken(IIntStream input, BitSet follow)
		{
			if (follow == null)
			{
				return false;
			}
			if (follow.Member(1))
			{
				BitSet a = this.ComputeContextSensitiveRuleFOLLOW();
				follow = follow.Or(a);
				if (this.state.followingStackPointer >= 0)
				{
					follow.Remove(1);
				}
			}
			return follow.Member(input.LA(1)) || follow.Member(1);
		}
		public virtual void ReportError(RecognitionException e)
		{
			if (this.state.errorRecovery)
			{
				return;
			}
			this.state.syntaxErrors++;
			this.state.errorRecovery = true;
			this.DisplayRecognitionError(this.TokenNames, e);
		}
		public virtual void DisplayRecognitionError(string[] tokenNames, RecognitionException e)
		{
			string errorHeader = this.GetErrorHeader(e);
			string errorMessage = this.GetErrorMessage(e, tokenNames);
			this.EmitErrorMessage(errorHeader + " " + errorMessage);
		}
		public virtual string GetErrorMessage(RecognitionException e, string[] tokenNames)
		{
			string result = e.Message;
			if (e is UnwantedTokenException)
			{
				UnwantedTokenException ex = (UnwantedTokenException)e;
				string str;
				if (ex.Expecting == Token.EOF)
				{
					str = "EOF";
				}
				else
				{
					str = tokenNames[ex.Expecting];
				}
				result = "extraneous input " + this.GetTokenErrorDisplay(ex.UnexpectedToken) + " expecting " + str;
			}
			else
			{
				if (e is MissingTokenException)
				{
					MissingTokenException ex2 = (MissingTokenException)e;
					string str2;
					if (ex2.Expecting == Token.EOF)
					{
						str2 = "EOF";
					}
					else
					{
						str2 = tokenNames[ex2.Expecting];
					}
					result = "missing " + str2 + " at " + this.GetTokenErrorDisplay(e.Token);
				}
				else
				{
					if (e is MismatchedTokenException)
					{
						MismatchedTokenException ex3 = (MismatchedTokenException)e;
						string str3;
						if (ex3.Expecting == Token.EOF)
						{
							str3 = "EOF";
						}
						else
						{
							str3 = tokenNames[ex3.Expecting];
						}
						result = "mismatched input " + this.GetTokenErrorDisplay(e.Token) + " expecting " + str3;
					}
					else
					{
						if (e is MismatchedTreeNodeException)
						{
							MismatchedTreeNodeException ex4 = (MismatchedTreeNodeException)e;
							string text;
							if (ex4.expecting == Token.EOF)
							{
								text = "EOF";
							}
							else
							{
								text = tokenNames[ex4.expecting];
							}
							result = string.Concat(new object[]
							{
								"mismatched tree node: ",
								(ex4.Node != null && ex4.Node.ToString() != null) ? ex4.Node : string.Empty,
								" expecting ",
								text
							});
						}
						else
						{
							if (e is NoViableAltException)
							{
								result = "no viable alternative at input " + this.GetTokenErrorDisplay(e.Token);
							}
							else
							{
								if (e is EarlyExitException)
								{
									result = "required (...)+ loop did not match anything at input " + this.GetTokenErrorDisplay(e.Token);
								}
								else
								{
									if (e is MismatchedSetException)
									{
										MismatchedSetException ex5 = (MismatchedSetException)e;
										result = string.Concat(new object[]
										{
											"mismatched input ",
											this.GetTokenErrorDisplay(e.Token),
											" expecting set ",
											ex5.expecting
										});
									}
									else
									{
										if (e is MismatchedNotSetException)
										{
											MismatchedNotSetException ex6 = (MismatchedNotSetException)e;
											result = string.Concat(new object[]
											{
												"mismatched input ",
												this.GetTokenErrorDisplay(e.Token),
												" expecting set ",
												ex6.expecting
											});
										}
										else
										{
											if (e is FailedPredicateException)
											{
												FailedPredicateException ex7 = (FailedPredicateException)e;
												result = string.Concat(new string[]
												{
													"rule ",
													ex7.ruleName,
													" failed predicate: {",
													ex7.predicateText,
													"}?"
												});
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}
		public virtual string GetErrorHeader(RecognitionException e)
		{
			return string.Concat(new object[]
			{
				"line ",
				e.Line,
				":",
				e.CharPositionInLine
			});
		}
		public virtual string GetTokenErrorDisplay(IToken t)
		{
			string text = t.Text;
			if (text == null)
			{
				if (t.Type == Token.EOF)
				{
					text = "<EOF>";
				}
				else
				{
					text = "<" + t.Type + ">";
				}
			}
			text = text.Replace("\n", "\\\\n");
			text = text.Replace("\r", "\\\\r");
			text = text.Replace("\t", "\\\\t");
			return "'" + text + "'";
		}
		public virtual void EmitErrorMessage(string msg)
		{
			Console.Error.WriteLine(msg);
		}
		public virtual void Recover(IIntStream input, RecognitionException re)
		{
			if (this.state.lastErrorIndex == input.Index())
			{
				input.Consume();
			}
			this.state.lastErrorIndex = input.Index();
			BitSet set = this.ComputeErrorRecoverySet();
			this.BeginResync();
			this.ConsumeUntil(input, set);
			this.EndResync();
		}
		public virtual void BeginResync()
		{
		}
		public virtual void EndResync()
		{
		}
		protected internal virtual object RecoverFromMismatchedToken(IIntStream input, int ttype, BitSet follow)
		{
			RecognitionException ex = null;
			if (this.MismatchIsUnwantedToken(input, ttype))
			{
				ex = new UnwantedTokenException(ttype, input);
				this.BeginResync();
				input.Consume();
				this.EndResync();
				this.ReportError(ex);
				object currentInputSymbol = this.GetCurrentInputSymbol(input);
				input.Consume();
				return currentInputSymbol;
			}
			if (this.MismatchIsMissingToken(input, follow))
			{
				object missingSymbol = this.GetMissingSymbol(input, ex, ttype, follow);
				ex = new MissingTokenException(ttype, input, missingSymbol);
				this.ReportError(ex);
				return missingSymbol;
			}
			ex = new MismatchedTokenException(ttype, input);
			throw ex;
		}
		public virtual object RecoverFromMismatchedSet(IIntStream input, RecognitionException e, BitSet follow)
		{
			if (this.MismatchIsMissingToken(input, follow))
			{
				this.ReportError(e);
				return this.GetMissingSymbol(input, e, 0, follow);
			}
			throw e;
		}
		public virtual void ConsumeUntil(IIntStream input, int tokenType)
		{
			int num = input.LA(1);
			while (num != Token.EOF && num != tokenType)
			{
				input.Consume();
				num = input.LA(1);
			}
		}
		public virtual void ConsumeUntil(IIntStream input, BitSet set)
		{
			int num = input.LA(1);
			while (num != Token.EOF && !set.Member(num))
			{
				input.Consume();
				num = input.LA(1);
			}
		}
		public virtual IList GetRuleInvocationStack()
		{
			string fullName = base.GetType().FullName;
			return BaseRecognizer.GetRuleInvocationStack(new Exception(), fullName);
		}
		public static IList GetRuleInvocationStack(Exception e, string recognizerClassName)
		{
			IList list = new List<object>();
			StackTrace stackTrace = new StackTrace(e);
			for (int i = stackTrace.FrameCount - 1; i >= 0; i--)
			{
				StackFrame frame = stackTrace.GetFrame(i);
				if (!frame.GetMethod().DeclaringType.FullName.StartsWith("Antlr.Runtime.") && !frame.GetMethod().Name.Equals(BaseRecognizer.NEXT_TOKEN_RULE_NAME) && frame.GetMethod().DeclaringType.FullName.Equals(recognizerClassName))
				{
					list.Add(frame.GetMethod().Name);
				}
			}
			return list;
		}
		public virtual IList ToStrings(IList tokens)
		{
			if (tokens == null)
			{
				return null;
			}
			IList list = new List<object>(tokens.Count);
			for (int i = 0; i < tokens.Count; i++)
			{
				list.Add(((IToken)tokens[i]).Text);
			}
			return list;
		}
		public virtual int GetRuleMemoization(int ruleIndex, int ruleStartIndex)
		{
			if (this.state.ruleMemo[ruleIndex] == null)
			{
				this.state.ruleMemo[ruleIndex] = new Hashtable();
			}
			object obj = this.state.ruleMemo[ruleIndex][ruleStartIndex];
			if (obj == null)
			{
				return -1;
			}
			return (int)obj;
		}
		public virtual bool AlreadyParsedRule(IIntStream input, int ruleIndex)
		{
			int ruleMemoization = this.GetRuleMemoization(ruleIndex, input.Index());
			if (ruleMemoization == -1)
			{
				return false;
			}
			if (ruleMemoization == -2)
			{
				this.state.failed = true;
			}
			else
			{
				input.Seek(ruleMemoization + 1);
			}
			return true;
		}
		public virtual void Memoize(IIntStream input, int ruleIndex, int ruleStartIndex)
		{
			int num = this.state.failed ? -2 : (input.Index() - 1);
			if (this.state.ruleMemo[ruleIndex] != null)
			{
				this.state.ruleMemo[ruleIndex][ruleStartIndex] = num;
			}
		}
		public int GetRuleMemoizationCacheSize()
		{
			int num = 0;
			int num2 = 0;
			while (this.state.ruleMemo != null && num2 < this.state.ruleMemo.Length)
			{
				IDictionary dictionary = this.state.ruleMemo[num2];
				if (dictionary != null)
				{
					num += dictionary.Count;
				}
				num2++;
			}
			return num;
		}
		public virtual void TraceIn(string ruleName, int ruleIndex, object inputSymbol)
		{
			Console.Out.Write(string.Concat(new object[]
			{
				"enter ",
				ruleName,
				" ",
				inputSymbol
			}));
			if (this.state.backtracking > 0)
			{
				Console.Out.Write(" backtracking=" + this.state.backtracking);
			}
			Console.Out.WriteLine();
		}
		public virtual void TraceOut(string ruleName, int ruleIndex, object inputSymbol)
		{
			Console.Out.Write(string.Concat(new object[]
			{
				"exit ",
				ruleName,
				" ",
				inputSymbol
			}));
			if (this.state.backtracking > 0)
			{
				Console.Out.Write(" backtracking=" + this.state.backtracking);
				if (this.state.failed)
				{
					Console.Out.WriteLine(" failed" + this.state.failed);
				}
				else
				{
					Console.Out.WriteLine(" succeeded" + this.state.failed);
				}
			}
			Console.Out.WriteLine();
		}
		protected internal virtual BitSet ComputeErrorRecoverySet()
		{
			return this.CombineFollows(false);
		}
		protected internal virtual BitSet ComputeContextSensitiveRuleFOLLOW()
		{
			return this.CombineFollows(true);
		}
		protected internal virtual BitSet CombineFollows(bool exact)
		{
			int followingStackPointer = this.state.followingStackPointer;
			BitSet bitSet = new BitSet();
			for (int i = followingStackPointer; i >= 0; i--)
			{
				BitSet bitSet2 = this.state.following[i];
				bitSet.OrInPlace(bitSet2);
				if (exact)
				{
					if (!bitSet2.Member(1))
					{
						break;
					}
					if (i > 0)
					{
						bitSet.Remove(1);
					}
				}
			}
			return bitSet;
		}
		protected virtual object GetCurrentInputSymbol(IIntStream input)
		{
			return null;
		}
		protected virtual object GetMissingSymbol(IIntStream input, RecognitionException e, int expectedTokenType, BitSet follow)
		{
			return null;
		}
		protected void PushFollow(BitSet fset)
		{
			if (this.state.followingStackPointer + 1 >= this.state.following.Length)
			{
				BitSet[] array = new BitSet[this.state.following.Length * 2];
				Array.Copy(this.state.following, 0, array, 0, this.state.following.Length);
				this.state.following = array;
			}
			this.state.following[++this.state.followingStackPointer] = fset;
		}
	}
}
