using System;
using System.IO;
namespace log4net.Util.PatternStringConverters
{
	internal class LiteralPatternConverter : PatternConverter
	{
		public override PatternConverter SetNext(PatternConverter pc)
		{
			LiteralPatternConverter literalPatternConverter = pc as LiteralPatternConverter;
			PatternConverter result;
			if (literalPatternConverter != null)
			{
				this.Option += literalPatternConverter.Option;
				result = this;
			}
			else
			{
				result = base.SetNext(pc);
			}
			return result;
		}
		public override void Format(TextWriter writer, object state)
		{
			writer.Write(this.Option);
		}
		protected override void Convert(TextWriter writer, object state)
		{
			throw new InvalidOperationException("Should never get here because of the overridden Format method");
		}
	}
}
