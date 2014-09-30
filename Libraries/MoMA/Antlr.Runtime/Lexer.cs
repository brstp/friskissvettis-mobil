using System;
namespace Antlr.Runtime
{
	public abstract class Lexer : BaseRecognizer, ITokenSource
	{
		private const int TOKEN_dot_EOF = -1;
		protected internal ICharStream input;
		public virtual ICharStream CharStream
		{
			get
			{
				return this.input;
			}
			set
			{
				this.input = null;
				this.Reset();
				this.input = value;
			}
		}
		public override string SourceName
		{
			get
			{
				return this.input.SourceName;
			}
		}
		public override IIntStream Input
		{
			get
			{
				return this.input;
			}
		}
		public virtual int Line
		{
			get
			{
				return this.input.Line;
			}
		}
		public virtual int CharPositionInLine
		{
			get
			{
				return this.input.CharPositionInLine;
			}
		}
		public virtual int CharIndex
		{
			get
			{
				return this.input.Index();
			}
		}
		public virtual string Text
		{
			get
			{
				if (this.state.text != null)
				{
					return this.state.text;
				}
				return this.input.Substring(this.state.tokenStartCharIndex, this.CharIndex - 1);
			}
			set
			{
				this.state.text = value;
			}
		}
		public Lexer()
		{
		}
		public Lexer(ICharStream input)
		{
			this.input = input;
		}
		public Lexer(ICharStream input, RecognizerSharedState state) : base(state)
		{
			this.input = input;
		}
		public override void Reset()
		{
			base.Reset();
			if (this.input != null)
			{
				this.input.Seek(0);
			}
			if (this.state == null)
			{
				return;
			}
			this.state.token = null;
			this.state.type = 0;
			this.state.channel = 0;
			this.state.tokenStartCharIndex = -1;
			this.state.tokenStartCharPositionInLine = -1;
			this.state.tokenStartLine = -1;
			this.state.text = null;
		}
		public virtual IToken NextToken()
		{
			while (true)
			{
				this.state.token = null;
				this.state.channel = 0;
				this.state.tokenStartCharIndex = this.input.Index();
				this.state.tokenStartCharPositionInLine = this.input.CharPositionInLine;
				this.state.tokenStartLine = this.input.Line;
				this.state.text = null;
				if (this.input.LA(1) == -1)
				{
					break;
				}
				IToken token;
				try
				{
					this.mTokens();
					if (this.state.token == null)
					{
						this.Emit();
					}
					else
					{
						if (this.state.token == Token.SKIP_TOKEN)
						{
							continue;
						}
					}
					token = this.state.token;
				}
				catch (NoViableAltException ex)
				{
					this.ReportError(ex);
					this.Recover(ex);
					continue;
				}
				catch (RecognitionException e)
				{
					this.ReportError(e);
					continue;
				}
				return token;
			}
			return Token.EOF_TOKEN;
		}
		public void Skip()
		{
			this.state.token = Token.SKIP_TOKEN;
		}
		public abstract void mTokens();
		public virtual void Emit(IToken token)
		{
			this.state.token = token;
		}
		public virtual IToken Emit()
		{
			IToken token = new CommonToken(this.input, this.state.type, this.state.channel, this.state.tokenStartCharIndex, this.CharIndex - 1);
			token.Line = this.state.tokenStartLine;
			token.Text = this.state.text;
			token.CharPositionInLine = this.state.tokenStartCharPositionInLine;
			this.Emit(token);
			return token;
		}
		public virtual void Match(string s)
		{
			int i = 0;
			while (i < s.Length)
			{
				if (this.input.LA(1) != (int)s[i])
				{
					if (this.state.backtracking > 0)
					{
						this.state.failed = true;
						return;
					}
					MismatchedTokenException ex = new MismatchedTokenException((int)s[i], this.input);
					this.Recover(ex);
					throw ex;
				}
				else
				{
					i++;
					this.input.Consume();
					this.state.failed = false;
				}
			}
		}
		public virtual void MatchAny()
		{
			this.input.Consume();
		}
		public virtual void Match(int c)
		{
			if (this.input.LA(1) == c)
			{
				this.input.Consume();
				this.state.failed = false;
				return;
			}
			if (this.state.backtracking > 0)
			{
				this.state.failed = true;
				return;
			}
			MismatchedTokenException ex = new MismatchedTokenException(c, this.input);
			this.Recover(ex);
			throw ex;
		}
		public virtual void MatchRange(int a, int b)
		{
			if (this.input.LA(1) >= a && this.input.LA(1) <= b)
			{
				this.input.Consume();
				this.state.failed = false;
				return;
			}
			if (this.state.backtracking > 0)
			{
				this.state.failed = true;
				return;
			}
			MismatchedRangeException ex = new MismatchedRangeException(a, b, this.input);
			this.Recover(ex);
			throw ex;
		}
		public virtual void Recover(RecognitionException re)
		{
			this.input.Consume();
		}
		public override void ReportError(RecognitionException e)
		{
			this.DisplayRecognitionError(this.TokenNames, e);
		}
		public override string GetErrorMessage(RecognitionException e, string[] tokenNames)
		{
			string result;
			if (e is MismatchedTokenException)
			{
				MismatchedTokenException ex = (MismatchedTokenException)e;
				result = "mismatched character " + this.GetCharErrorDisplay(e.Char) + " expecting " + this.GetCharErrorDisplay(ex.Expecting);
			}
			else
			{
				if (e is NoViableAltException)
				{
					NoViableAltException ex2 = (NoViableAltException)e;
					result = "no viable alternative at character " + this.GetCharErrorDisplay(ex2.Char);
				}
				else
				{
					if (e is EarlyExitException)
					{
						EarlyExitException ex3 = (EarlyExitException)e;
						result = "required (...)+ loop did not match anything at character " + this.GetCharErrorDisplay(ex3.Char);
					}
					else
					{
						if (e is MismatchedNotSetException)
						{
							MismatchedSetException ex4 = (MismatchedSetException)e;
							result = string.Concat(new object[]
							{
								"mismatched character ",
								this.GetCharErrorDisplay(ex4.Char),
								" expecting set ",
								ex4.expecting
							});
						}
						else
						{
							if (e is MismatchedSetException)
							{
								MismatchedSetException ex5 = (MismatchedSetException)e;
								result = string.Concat(new object[]
								{
									"mismatched character ",
									this.GetCharErrorDisplay(ex5.Char),
									" expecting set ",
									ex5.expecting
								});
							}
							else
							{
								if (e is MismatchedRangeException)
								{
									MismatchedRangeException ex6 = (MismatchedRangeException)e;
									result = string.Concat(new string[]
									{
										"mismatched character ",
										this.GetCharErrorDisplay(ex6.Char),
										" expecting set ",
										this.GetCharErrorDisplay(ex6.A),
										"..",
										this.GetCharErrorDisplay(ex6.B)
									});
								}
								else
								{
									result = base.GetErrorMessage(e, tokenNames);
								}
							}
						}
					}
				}
			}
			return result;
		}
		public string GetCharErrorDisplay(int c)
		{
			string str;
			if (c != -1)
			{
				switch (c)
				{
				case 9:
					str = "\\t";
					goto IL_5F;
				case 10:
					str = "\\n";
					goto IL_5F;
				case 13:
					str = "\\r";
					goto IL_5F;
				}
				str = Convert.ToString((char)c);
			}
			else
			{
				str = "<EOF>";
			}
			IL_5F:
			return "'" + str + "'";
		}
		public virtual void TraceIn(string ruleName, int ruleIndex)
		{
			string inputSymbol = string.Concat(new object[]
			{
				(char)this.input.LT(1),
				" line=",
				this.Line,
				":",
				this.CharPositionInLine
			});
			base.TraceIn(ruleName, ruleIndex, inputSymbol);
		}
		public virtual void TraceOut(string ruleName, int ruleIndex)
		{
			string inputSymbol = string.Concat(new object[]
			{
				(char)this.input.LT(1),
				" line=",
				this.Line,
				":",
				this.CharPositionInLine
			});
			base.TraceOut(ruleName, ruleIndex, inputSymbol);
		}
	}
}
