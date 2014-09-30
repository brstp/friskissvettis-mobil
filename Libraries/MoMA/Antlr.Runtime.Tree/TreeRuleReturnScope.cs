using System;
namespace Antlr.Runtime.Tree
{
	public class TreeRuleReturnScope : RuleReturnScope
	{
		private object start;
		public override object Start
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
	}
}
