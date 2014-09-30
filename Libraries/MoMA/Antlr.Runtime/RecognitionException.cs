using Antlr.Runtime.Tree;
using System;
namespace Antlr.Runtime
{
	[Serializable]
	public class RecognitionException : Exception
	{
		[NonSerialized]
		protected IIntStream input;
		protected int index;
		protected IToken token;
		protected object node;
		protected int c;
		protected int line;
		protected int charPositionInLine;
		public bool approximateLineInfo;
		public IIntStream Input
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
		public int Index
		{
			get
			{
				return this.index;
			}
			set
			{
				this.index = value;
			}
		}
		public IToken Token
		{
			get
			{
				return this.token;
			}
			set
			{
				this.token = value;
			}
		}
		public object Node
		{
			get
			{
				return this.node;
			}
			set
			{
				this.node = value;
			}
		}
		public int Char
		{
			get
			{
				return this.c;
			}
			set
			{
				this.c = value;
			}
		}
		public int CharPositionInLine
		{
			get
			{
				return this.charPositionInLine;
			}
			set
			{
				this.charPositionInLine = value;
			}
		}
		public int Line
		{
			get
			{
				return this.line;
			}
			set
			{
				this.line = value;
			}
		}
		public virtual int UnexpectedType
		{
			get
			{
				if (this.input is ITokenStream)
				{
					return this.token.Type;
				}
				if (this.input is ITreeNodeStream)
				{
					ITreeNodeStream treeNodeStream = (ITreeNodeStream)this.input;
					ITreeAdaptor treeAdaptor = treeNodeStream.TreeAdaptor;
					return treeAdaptor.GetNodeType(this.node);
				}
				return this.c;
			}
		}
		public RecognitionException() : this(null, null, null)
		{
		}
		public RecognitionException(string message) : this(message, null, null)
		{
		}
		public RecognitionException(string message, Exception inner) : this(message, inner, null)
		{
		}
		public RecognitionException(IIntStream input) : this(null, null, input)
		{
		}
		public RecognitionException(string message, IIntStream input) : this(message, null, input)
		{
		}
		public RecognitionException(string message, Exception inner, IIntStream input) : base(message, inner)
		{
			this.input = input;
			this.index = input.Index();
			if (input is ITokenStream)
			{
				this.token = ((ITokenStream)input).LT(1);
				this.line = this.token.Line;
				this.charPositionInLine = this.token.CharPositionInLine;
			}
			if (input is ITreeNodeStream)
			{
				this.ExtractInformationFromTreeNodeStream(input);
				return;
			}
			if (input is ICharStream)
			{
				this.c = input.LA(1);
				this.line = ((ICharStream)input).Line;
				this.charPositionInLine = ((ICharStream)input).CharPositionInLine;
				return;
			}
			this.c = input.LA(1);
		}
		protected void ExtractInformationFromTreeNodeStream(IIntStream input)
		{
			ITreeNodeStream treeNodeStream = (ITreeNodeStream)input;
			this.node = treeNodeStream.LT(1);
			ITreeAdaptor treeAdaptor = treeNodeStream.TreeAdaptor;
			IToken token = treeAdaptor.GetToken(this.node);
			if (token == null)
			{
				if (this.node is ITree)
				{
					this.line = ((ITree)this.node).Line;
					this.charPositionInLine = ((ITree)this.node).CharPositionInLine;
					if (this.node is CommonTree)
					{
						this.token = ((CommonTree)this.node).Token;
						return;
					}
				}
				else
				{
					int nodeType = treeAdaptor.GetNodeType(this.node);
					string nodeText = treeAdaptor.GetNodeText(this.node);
					this.token = new CommonToken(nodeType, nodeText);
				}
				return;
			}
			this.token = token;
			if (token.Line <= 0)
			{
				int num = -1;
				for (object treeNode = treeNodeStream.LT(num); treeNode != null; treeNode = treeNodeStream.LT(num))
				{
					IToken token2 = treeAdaptor.GetToken(treeNode);
					if (token2 != null && token2.Line > 0)
					{
						this.line = token2.Line;
						this.charPositionInLine = token2.CharPositionInLine;
						this.approximateLineInfo = true;
						return;
					}
					num--;
				}
				return;
			}
			this.line = token.Line;
			this.charPositionInLine = token.CharPositionInLine;
		}
	}
}