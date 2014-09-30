using System;
namespace NCalc.Domain
{
	public class Identifier : LogicalExpression
	{
		public string Name
		{
			get;
			set;
		}
		public Identifier(string name)
		{
			this.Name = name;
		}
		public override void Accept(LogicalExpressionVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
