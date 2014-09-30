using System;
namespace Antlr.Runtime
{
	public class UnwantedTokenException : MismatchedTokenException
	{
		public IToken UnexpectedToken
		{
			get
			{
				return this.token;
			}
		}
		public UnwantedTokenException()
		{
		}
		public UnwantedTokenException(int expecting, IIntStream input) : base(expecting, input)
		{
		}
		public override string ToString()
		{
			string text = ", expected " + base.Expecting;
			if (base.Expecting == 0)
			{
				text = "";
			}
			if (this.token == null)
			{
				return "UnwantedTokenException(found=" + text + ")";
			}
			return "UnwantedTokenException(found=" + this.token.Text + text + ")";
		}
	}
}
