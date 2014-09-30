using System;
namespace Antlr.Runtime
{
	[Serializable]
	public class MissingTokenException : MismatchedTokenException
	{
		private object inserted;
		public int MissingType
		{
			get
			{
				return base.Expecting;
			}
		}
		public object Inserted
		{
			get
			{
				return this.inserted;
			}
			set
			{
				this.inserted = value;
			}
		}
		public MissingTokenException()
		{
		}
		public MissingTokenException(int expecting, IIntStream input, object inserted) : base(expecting, input)
		{
			this.inserted = inserted;
		}
		public override string ToString()
		{
			if (this.inserted != null && this.token != null)
			{
				return string.Concat(new object[]
				{
					"MissingTokenException(inserted ",
					this.inserted,
					" at ",
					this.token.Text,
					")"
				});
			}
			if (this.token != null)
			{
				return "MissingTokenException(at " + this.token.Text + ")";
			}
			return "MissingTokenException";
		}
	}
}
