using System;
namespace Antlr.Runtime.Tree
{
	[Serializable]
	public class CommonTree : BaseTree
	{
		public int startIndex = -1;
		public int stopIndex = -1;
		protected IToken token;
		public CommonTree parent;
		public int childIndex = -1;
		public virtual IToken Token
		{
			get
			{
				return this.token;
			}
		}
		public override bool IsNil
		{
			get
			{
				return this.token == null;
			}
		}
		public override int Type
		{
			get
			{
				if (this.token == null)
				{
					return 0;
				}
				return this.token.Type;
			}
		}
		public override string Text
		{
			get
			{
				if (this.token == null)
				{
					return null;
				}
				return this.token.Text;
			}
		}
		public override int Line
		{
			get
			{
				if (this.token != null && this.token.Line != 0)
				{
					return this.token.Line;
				}
				if (this.ChildCount > 0)
				{
					return this.GetChild(0).Line;
				}
				return 0;
			}
		}
		public override int CharPositionInLine
		{
			get
			{
				if (this.token != null && this.token.CharPositionInLine != -1)
				{
					return this.token.CharPositionInLine;
				}
				if (this.ChildCount > 0)
				{
					return this.GetChild(0).CharPositionInLine;
				}
				return 0;
			}
		}
		public override int TokenStartIndex
		{
			get
			{
				if (this.startIndex == -1 && this.token != null)
				{
					return this.token.TokenIndex;
				}
				return this.startIndex;
			}
			set
			{
				this.startIndex = value;
			}
		}
		public override int TokenStopIndex
		{
			get
			{
				if (this.stopIndex == -1 && this.token != null)
				{
					return this.token.TokenIndex;
				}
				return this.stopIndex;
			}
			set
			{
				this.stopIndex = value;
			}
		}
		public override int ChildIndex
		{
			get
			{
				return this.childIndex;
			}
			set
			{
				this.childIndex = value;
			}
		}
		public override ITree Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				this.parent = (CommonTree)value;
			}
		}
		public CommonTree()
		{
		}
		public CommonTree(CommonTree node) : base(node)
		{
			this.token = node.token;
			this.startIndex = node.startIndex;
			this.stopIndex = node.stopIndex;
		}
		public CommonTree(IToken t)
		{
			this.token = t;
		}
		public void SetUnknownTokenBoundaries()
		{
			if (this.children == null)
			{
				if (this.startIndex < 0 || this.stopIndex < 0)
				{
					this.startIndex = (this.stopIndex = this.token.TokenIndex);
				}
				return;
			}
			for (int i = 0; i < this.children.Count; i++)
			{
				((CommonTree)this.children[i]).SetUnknownTokenBoundaries();
			}
			if (this.startIndex >= 0 && this.stopIndex >= 0)
			{
				return;
			}
			if (this.children.Count > 0)
			{
				CommonTree commonTree = (CommonTree)this.children[0];
				CommonTree commonTree2 = (CommonTree)this.children[this.children.Count - 1];
				this.startIndex = commonTree.TokenStartIndex;
				this.stopIndex = commonTree2.TokenStopIndex;
			}
		}
		public override ITree DupNode()
		{
			return new CommonTree(this);
		}
		public override string ToString()
		{
			if (this.IsNil)
			{
				return "nil";
			}
			if (this.Type == 0)
			{
				return "<errornode>";
			}
			if (this.token == null)
			{
				return null;
			}
			return this.token.Text;
		}
	}
}
