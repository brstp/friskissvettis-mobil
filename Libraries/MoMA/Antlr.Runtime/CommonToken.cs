using System;
namespace Antlr.Runtime
{
	[Serializable]
	public class CommonToken : IToken
	{
		protected internal int type;
		protected internal int line;
		protected internal int charPositionInLine = -1;
		protected internal int channel;
		[NonSerialized]
		protected internal ICharStream input;
		protected internal string text;
		protected internal int index = -1;
		protected internal int start;
		protected internal int stop;
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
		public virtual int StartIndex
		{
			get
			{
				return this.start;
			}
			set
			{
				this.start = value;
			}
		}
		public virtual int StopIndex
		{
			get
			{
				return this.stop;
			}
			set
			{
				this.stop = value;
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
		public virtual ICharStream InputStream
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
		public virtual string Text
		{
			get
			{
				if (this.text != null)
				{
					return this.text;
				}
				if (this.input == null)
				{
					return null;
				}
				this.text = this.input.Substring(this.start, this.stop);
				return this.text;
			}
			set
			{
				this.text = value;
			}
		}
		public CommonToken(int type)
		{
			this.type = type;
		}
		public CommonToken(ICharStream input, int type, int channel, int start, int stop)
		{
			this.input = input;
			this.type = type;
			this.channel = channel;
			this.start = start;
			this.stop = stop;
		}
		public CommonToken(int type, string text)
		{
			this.type = type;
			this.channel = 0;
			this.text = text;
		}
		public CommonToken(IToken oldToken)
		{
			this.text = oldToken.Text;
			this.type = oldToken.Type;
			this.line = oldToken.Line;
			this.index = oldToken.TokenIndex;
			this.charPositionInLine = oldToken.CharPositionInLine;
			this.channel = oldToken.Channel;
			if (oldToken is CommonToken)
			{
				this.start = ((CommonToken)oldToken).start;
				this.stop = ((CommonToken)oldToken).stop;
			}
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
				",",
				this.start,
				":",
				this.stop,
				"='",
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
