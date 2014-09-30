using System;
namespace Antlr.Runtime
{
	[Serializable]
	public class ClassicToken : IToken
	{
		protected internal string text;
		protected internal int type;
		protected internal int line;
		protected internal int charPositionInLine;
		protected internal int channel;
		protected internal int index;
		public virtual int Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}
		public virtual int Line
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
		public virtual int CharPositionInLine
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
		public virtual int Channel
		{
			get
			{
				return this.channel;
			}
			set
			{
				this.channel = value;
			}
		}
		public virtual int TokenIndex
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
		public virtual string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
			}
		}
		public virtual ICharStream InputStream
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public ClassicToken(int type)
		{
			this.type = type;
		}
		public ClassicToken(IToken oldToken)
		{
			this.text = oldToken.Text;
			this.type = oldToken.Type;
			this.line = oldToken.Line;
			this.charPositionInLine = oldToken.CharPositionInLine;
			this.channel = oldToken.Channel;
		}
		public ClassicToken(int type, string text)
		{
			this.type = type;
			this.text = text;
		}
		public ClassicToken(int type, string text, int channel)
		{
			this.type = type;
			this.text = text;
			this.channel = channel;
		}
		public override string ToString()
		{
			string text = "";
			if (this.channel > 0)
			{
				text = ",channel=" + this.channel;
			}
			string text2 = this.Text;
			if (text2 != null)
			{
				text2 = text2.Replace("\n", "\\\\n");
				text2 = text2.Replace("\r", "\\\\r");
				text2 = text2.Replace("\t", "\\\\t");
			}
			else
			{
				text2 = "<no text>";
			}
			return string.Concat(new object[]
			{
				"[@",
				this.TokenIndex,
				",'",
				text2,
				"',<",
				this.type,
				">",
				text,
				",",
				this.line,
				":",
				this.CharPositionInLine,
				"]"
			});
		}
	}
}
