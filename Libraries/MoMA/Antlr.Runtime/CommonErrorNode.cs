using Antlr.Runtime.Tree;
using System;
namespace Antlr.Runtime
{
	[Serializable]
	public class CommonErrorNode : CommonTree
	{
		public IIntStream input;
		public IToken start;
		public IToken stop;
		[NonSerialized]
		public RecognitionException trappedException;
		public override bool IsNil
		{
			get
			{
				return false;
			}
		}
		public override int Type
		{
			get
			{
				return 0;
			}
		}
		public override string Text
		{
			get
			{
				string result;
				if (this.start != null)
				{
					int tokenIndex = this.start.TokenIndex;
					int num = this.stop.TokenIndex;
					if (this.stop.Type == Antlr.Runtime.Token.EOF)
					{
						num = ((ITokenStream)this.input).Count;
					}
					result = ((ITokenStream)this.input).ToString(tokenIndex, num);
				}
				else
				{
					if (this.start is ITree)
					{
						result = ((ITreeNodeStream)this.input).ToString(this.start, this.stop);
					}
					else
					{
						result = "<unknown>";
					}
				}
				return result;
			}
		}
		public CommonErrorNode(ITokenStream input, IToken start, IToken stop, RecognitionException e)
		{
			if (stop == null || (stop.TokenIndex < start.TokenIndex && stop.Type != Antlr.Runtime.Token.EOF))
			{
				stop = start;
			}
			this.input = input;
			this.start = start;
			this.stop = stop;
			this.trappedException = e;
		}
		public override string ToString()
		{
			if (this.trappedException is MissingTokenException)
			{
				return "<missing type: " + ((MissingTokenException)this.trappedException).MissingType + ">";
			}
			if (this.trappedException is UnwantedTokenException)
			{
				return string.Concat(new object[]
				{
					"<extraneous: ",
					((UnwantedTokenException)this.trappedException).UnexpectedToken,
					", resync=",
					this.Text,
					">"
				});
			}
			if (this.trappedException is MismatchedTokenException)
			{
				return string.Concat(new object[]
				{
					"<mismatched token: ",
					this.trappedException.Token,
					", resync=",
					this.Text,
					">"
				});
			}
			if (this.trappedException is NoViableAltException)
			{
				return string.Concat(new object[]
				{
					"<unexpected: ",
					this.trappedException.Token,
					", resync=",
					this.Text,
					">"
				});
			}
			return "<error: " + this.Text + ">";
		}
	}
}
