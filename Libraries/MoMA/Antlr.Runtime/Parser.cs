using System;
namespace Antlr.Runtime
{
	public class Parser : BaseRecognizer
	{
		protected internal ITokenStream input;
		public virtual ITokenStream TokenStream
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
		public Parser(ITokenStream input)
		{
			this.TokenStream = input;
		}
		public Parser(ITokenStream input, RecognizerSharedState state) : base(state)
		{
			this.TokenStream = input;
		}
		public override void Reset()
		{
			base.Reset();
			if (this.input != null)
			{
				this.input.Seek(0);
			}
		}
		protected override object GetCurrentInputSymbol(IIntStream input)
		{
			return ((ITokenStream)input).LT(1);
		}
		protected override object GetMissingSymbol(IIntStream input, RecognitionException e, int expectedTokenType, BitSet follow)
		{
			string text;
			if (expectedTokenType == Token.EOF)
			{
				text = "<missing EOF>";
			}
			else
			{
				text = "<missing " + this.TokenNames[expectedTokenType] + ">";
			}
			CommonToken commonToken = new CommonToken(expectedTokenType, text);
			IToken token = ((ITokenStream)input).LT(1);
			if (token.Type == Token.EOF)
			{
				token = ((ITokenStream)input).LT(-1);
			}
			commonToken.line = token.Line;
			commonToken.CharPositionInLine = token.CharPositionInLine;
			commonToken.Channel = 0;
			return commonToken;
		}
		public virtual void TraceIn(string ruleName, int ruleIndex)
		{
			base.TraceIn(ruleName, ruleIndex, this.input.LT(1));
		}
		public virtual void TraceOut(string ruleName, int ruleIndex)
		{
			base.TraceOut(ruleName, ruleIndex, this.input.LT(1));
		}
	}
}
