using System;
using System.Globalization;
using System.Text;
namespace NCalc.Domain
{
	public class SerializationVisitor : LogicalExpressionVisitor
	{
		private readonly NumberFormatInfo _numberFormatInfo;
		public StringBuilder Result
		{
			get;
			protected set;
		}
		public SerializationVisitor()
		{
			this.Result = new StringBuilder();
			this._numberFormatInfo = new NumberFormatInfo
			{
				NumberDecimalSeparator = "."
			};
		}
		public override void Visit(LogicalExpression expression)
		{
			throw new Exception("The method or operation is not implemented.");
		}
		public override void Visit(TernaryExpression expression)
		{
			this.EncapsulateNoValue(expression.LeftExpression);
			this.Result.Append("? ");
			this.EncapsulateNoValue(expression.MiddleExpression);
			this.Result.Append(": ");
			this.EncapsulateNoValue(expression.RightExpression);
		}
		public override void Visit(BinaryExpression expression)
		{
			this.EncapsulateNoValue(expression.LeftExpression);
			switch (expression.Type)
			{
			case BinaryExpressionType.And:
				this.Result.Append("and ");
				break;
			case BinaryExpressionType.Or:
				this.Result.Append("or ");
				break;
			case BinaryExpressionType.NotEqual:
				this.Result.Append("!= ");
				break;
			case BinaryExpressionType.LesserOrEqual:
				this.Result.Append("<= ");
				break;
			case BinaryExpressionType.GreaterOrEqual:
				this.Result.Append(">= ");
				break;
			case BinaryExpressionType.Lesser:
				this.Result.Append("< ");
				break;
			case BinaryExpressionType.Greater:
				this.Result.Append("> ");
				break;
			case BinaryExpressionType.Equal:
				this.Result.Append("= ");
				break;
			case BinaryExpressionType.Minus:
				this.Result.Append("- ");
				break;
			case BinaryExpressionType.Plus:
				this.Result.Append("+ ");
				break;
			case BinaryExpressionType.Modulo:
				this.Result.Append("% ");
				break;
			case BinaryExpressionType.Div:
				this.Result.Append("/ ");
				break;
			case BinaryExpressionType.Times:
				this.Result.Append("* ");
				break;
			case BinaryExpressionType.BitwiseOr:
				this.Result.Append("| ");
				break;
			case BinaryExpressionType.BitwiseAnd:
				this.Result.Append("& ");
				break;
			case BinaryExpressionType.BitwiseXOr:
				this.Result.Append("~ ");
				break;
			case BinaryExpressionType.LeftShift:
				this.Result.Append("<< ");
				break;
			case BinaryExpressionType.RightShift:
				this.Result.Append(">> ");
				break;
			}
			this.EncapsulateNoValue(expression.RightExpression);
		}
		public override void Visit(UnaryExpression expression)
		{
			switch (expression.Type)
			{
			case UnaryExpressionType.Not:
				this.Result.Append("!");
				break;
			case UnaryExpressionType.Negate:
				this.Result.Append("-");
				break;
			case UnaryExpressionType.BitwiseNot:
				this.Result.Append("~");
				break;
			}
			this.EncapsulateNoValue(expression.Expression);
		}
		public override void Visit(ValueExpression expression)
		{
			switch (expression.Type)
			{
			case ValueType.Integer:
				this.Result.Append(expression.Value.ToString()).Append(" ");
				break;
			case ValueType.String:
				this.Result.Append("'").Append(expression.Value.ToString()).Append("'").Append(" ");
				break;
			case ValueType.DateTime:
				this.Result.Append("#").Append(expression.Value.ToString()).Append("#").Append(" ");
				break;
			case ValueType.Float:
				this.Result.Append(decimal.Parse(expression.Value.ToString()).ToString(this._numberFormatInfo)).Append(" ");
				break;
			case ValueType.Boolean:
				this.Result.Append(expression.Value.ToString()).Append(" ");
				break;
			}
		}
		public override void Visit(Function function)
		{
			this.Result.Append(function.Identifier.Name);
			this.Result.Append("(");
			for (int i = 0; i < function.Expressions.Length; i++)
			{
				function.Expressions[i].Accept(this);
				if (i < function.Expressions.Length - 1)
				{
					this.Result.Remove(this.Result.Length - 1, 1);
					this.Result.Append(", ");
				}
			}
			while (this.Result[this.Result.Length - 1] == ' ')
			{
				this.Result.Remove(this.Result.Length - 1, 1);
			}
			this.Result.Append(") ");
		}
		public override void Visit(Identifier parameter)
		{
			this.Result.Append("[").Append(parameter.Name).Append("] ");
		}
		protected void EncapsulateNoValue(LogicalExpression expression)
		{
			if (expression is ValueExpression)
			{
				expression.Accept(this);
			}
			else
			{
				this.Result.Append("(");
				expression.Accept(this);
				while (this.Result[this.Result.Length - 1] == ' ')
				{
					this.Result.Remove(this.Result.Length - 1, 1);
				}
				this.Result.Append(") ");
			}
		}
	}
}
