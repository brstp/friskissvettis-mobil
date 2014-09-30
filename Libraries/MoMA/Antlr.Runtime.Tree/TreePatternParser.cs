using System;
namespace Antlr.Runtime.Tree
{
	public class TreePatternParser
	{
		protected TreePatternLexer tokenizer;
		protected int ttype;
		protected TreeWizard wizard;
		protected ITreeAdaptor adaptor;
		public TreePatternParser(TreePatternLexer tokenizer, TreeWizard wizard, ITreeAdaptor adaptor)
		{
			this.tokenizer = tokenizer;
			this.wizard = wizard;
			this.adaptor = adaptor;
			this.ttype = tokenizer.NextToken();
		}
		public object Pattern()
		{
			if (this.ttype == 1)
			{
				return this.ParseTree();
			}
			if (this.ttype != 3)
			{
				return null;
			}
			object result = this.ParseNode();
			if (this.ttype == -1)
			{
				return result;
			}
			return null;
		}
		public object ParseTree()
		{
			if (this.ttype != 1)
			{
				Console.Out.WriteLine("no BEGIN");
				return null;
			}
			this.ttype = this.tokenizer.NextToken();
			object obj = this.ParseNode();
			if (obj == null)
			{
				return null;
			}
			while (this.ttype == 1 || this.ttype == 3 || this.ttype == 5 || this.ttype == 7)
			{
				if (this.ttype == 1)
				{
					object child = this.ParseTree();
					this.adaptor.AddChild(obj, child);
				}
				else
				{
					object obj2 = this.ParseNode();
					if (obj2 == null)
					{
						return null;
					}
					this.adaptor.AddChild(obj, obj2);
				}
			}
			if (this.ttype != 2)
			{
				Console.Out.WriteLine("no END");
				return null;
			}
			this.ttype = this.tokenizer.NextToken();
			return obj;
		}
		public object ParseNode()
		{
			string text = null;
			if (this.ttype == 5)
			{
				this.ttype = this.tokenizer.NextToken();
				if (this.ttype != 3)
				{
					return null;
				}
				text = this.tokenizer.sval.ToString();
				this.ttype = this.tokenizer.NextToken();
				if (this.ttype != 6)
				{
					return null;
				}
				this.ttype = this.tokenizer.NextToken();
			}
			if (this.ttype == 7)
			{
				this.ttype = this.tokenizer.NextToken();
				IToken payload = new CommonToken(0, ".");
				TreeWizard.TreePattern treePattern = new TreeWizard.WildcardTreePattern(payload);
				if (text != null)
				{
					treePattern.label = text;
				}
				return treePattern;
			}
			if (this.ttype != 3)
			{
				return null;
			}
			string text2 = this.tokenizer.sval.ToString();
			this.ttype = this.tokenizer.NextToken();
			if (text2.Equals("nil"))
			{
				return this.adaptor.GetNilNode();
			}
			string text3 = text2;
			string text4 = null;
			if (this.ttype == 4)
			{
				text4 = this.tokenizer.sval.ToString();
				text3 = text4;
				this.ttype = this.tokenizer.NextToken();
			}
			int tokenType = this.wizard.GetTokenType(text2);
			if (tokenType == 0)
			{
				return null;
			}
			object obj = this.adaptor.Create(tokenType, text3);
			if (text != null && obj.GetType() == typeof(TreeWizard.TreePattern))
			{
				((TreeWizard.TreePattern)obj).label = text;
			}
			if (text4 != null && obj.GetType() == typeof(TreeWizard.TreePattern))
			{
				((TreeWizard.TreePattern)obj).hasTextArg = true;
			}
			return obj;
		}
	}
}
