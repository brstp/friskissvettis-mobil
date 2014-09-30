using System;
using System.Collections;
using System.Text;
namespace Antlr.Runtime.Tree
{
	public class ParseTree : BaseTree
	{
		public object payload;
		public IList hiddenTokens;
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
				return this.ToString();
			}
		}
		public override int TokenStartIndex
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}
		public override int TokenStopIndex
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}
		public ParseTree(object label)
		{
			this.payload = label;
		}
		public override ITree DupNode()
		{
			return null;
		}
		public override string ToString()
		{
			if (!(this.payload is IToken))
			{
				return this.payload.ToString();
			}
			IToken token = (IToken)this.payload;
			if (token.Type == Token.EOF)
			{
				return "<EOF>";
			}
			return token.Text;
		}
		public string ToStringWithHiddenTokens()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.hiddenTokens != null)
			{
				for (int i = 0; i < this.hiddenTokens.Count; i++)
				{
					IToken token = (IToken)this.hiddenTokens[i];
					stringBuilder.Append(token.Text);
				}
			}
			string text = this.ToString();
			if (text != "<EOF>")
			{
				stringBuilder.Append(text);
			}
			return stringBuilder.ToString();
		}
		public string ToInputString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this._ToStringLeaves(stringBuilder);
			return stringBuilder.ToString();
		}
		public void _ToStringLeaves(StringBuilder buf)
		{
			if (this.payload is IToken)
			{
				buf.Append(this.ToStringWithHiddenTokens());
				return;
			}
			int num = 0;
			while (this.children != null && num < this.children.Count)
			{
				ParseTree parseTree = (ParseTree)this.children[num];
				parseTree._ToStringLeaves(buf);
				num++;
			}
		}
	}
}
