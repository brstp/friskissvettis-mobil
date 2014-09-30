using System;
namespace NCalc.Domain
{
	public class UnaryExpression : LogicalExpression
	{
		public LogicalExpression Expression
		{
			get;
			set;
		}
		public UnaryExpressionType Type
		{
			get;
			set;
		}
		public UnaryExpression(UnaryExpressionType type, LogicalExpression expression)
		{
			this.Type = type;
			this.Expression = expression;
		}
		public override void Accept(LogicalExpressionVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
