using System;
namespace NCalc.Domain
{
	public class BinaryExpression : LogicalExpression
	{
		public LogicalExpression LeftExpression
		{
			get;
			set;
		}
		public LogicalExpression RightExpression
		{
			get;
			set;
		}
		public BinaryExpressionType Type
		{
			get;
			set;
		}
		public BinaryExpression(BinaryExpressionType type, LogicalExpression leftExpression, LogicalExpression rightExpression)
		{
			this.Type = type;
			this.LeftExpression = leftExpression;
			this.RightExpression = rightExpression;
		}
		public override void Accept(LogicalExpressionVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
