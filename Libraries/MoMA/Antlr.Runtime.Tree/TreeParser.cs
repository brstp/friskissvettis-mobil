using System;
using System.Text.RegularExpressions;
namespace Antlr.Runtime.Tree
{
	public class TreeParser : BaseRecognizer
	{
		public const int DOWN = 2;
		public const int UP = 3;
		private static readonly string dotdot = ".*[^.]\\.\\.[^.].*";
		private static readonly string doubleEtc = ".*\\.\\.\\.\\s+\\.\\.\\..*";
		private static readonly string spaces = "\\s+";
		private static readonly Regex dotdotPattern = new Regex(TreeParser.dotdot, RegexOptions.Compiled);
		private static readonly Regex doubleEtcPattern = new Regex(TreeParser.doubleEtc, RegexOptions.Compiled);
		private static readonly Regex spacesPattern = new Regex(TreeParser.spaces, RegexOptions.Compiled);
		protected internal ITreeNodeStream input;
		public virtual ITreeNodeStream TreeNodeStream
		{
			get
			{
				return this.input;
			}
			set
			{
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
		public TreeParser(ITreeNodeStream input)
		{
			this.TreeNodeStream = input;
		}
		public TreeParser(ITreeNodeStream input, RecognizerSharedState state) : base(state)
		{
			this.TreeNodeStream = input;
		}
		protected override object GetCurrentInputSymbol(IIntStream input)
		{
			return ((ITreeNodeStream)input).LT(1);
		}
		protected override object GetMissingSymbol(IIntStream input, RecognitionException e, int expectedTokenType, BitSet follow)
		{
			string text = "<missing " + this.TokenNames[expectedTokenType] + ">";
			return new CommonTree(new CommonToken(expectedTokenType, text));
		}
		public override void Reset()
		{
			base.Reset();
			if (this.input != null)
			{
				this.input.Seek(0);
			}
		}
		public override void MatchAny(IIntStream ignore)
		{
			this.state.errorRecovery = false;
			this.state.failed = false;
			object t = this.input.LT(1);
			if (this.input.TreeAdaptor.GetChildCount(t) == 0)
			{
				this.input.Consume();
				return;
			}
			int num = 0;
			int nodeType = this.input.TreeAdaptor.GetNodeType(t);
			while (nodeType != Token.EOF && (nodeType != 3 || num != 0))
			{
				this.input.Consume();
				t = this.input.LT(1);
				nodeType = this.input.TreeAdaptor.GetNodeType(t);
				if (nodeType == 2)
				{
					num++;
				}
				else
				{
					if (nodeType == 3)
					{
						num--;
					}
				}
			}
			this.input.Consume();
		}
		protected internal override object RecoverFromMismatchedToken(IIntStream input, int ttype, BitSet follow)
		{
			throw new MismatchedTreeNodeException(ttype, (ITreeNodeStream)input);
		}
		public override string GetErrorHeader(RecognitionException e)
		{
			return string.Concat(new object[]
			{
				this.GrammarFileName,
				": node from ",
				e.approximateLineInfo ? "after " : "",
				"line ",
				e.Line,
				":",
				e.CharPositionInLine
			});
		}
		public override string GetErrorMessage(RecognitionException e, string[] tokenNames)
		{
			if (this != null)
			{
				ITreeAdaptor treeAdaptor = ((ITreeNodeStream)e.Input).TreeAdaptor;
				e.Token = treeAdaptor.GetToken(e.Node);
				if (e.Token == null)
				{
					e.Token = new CommonToken(treeAdaptor.GetNodeType(e.Node), treeAdaptor.GetNodeText(e.Node));
				}
			}
			return base.GetErrorMessage(e, tokenNames);
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
