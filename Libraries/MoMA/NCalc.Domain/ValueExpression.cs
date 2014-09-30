using System;
namespace NCalc.Domain
{
	public class ValueExpression : LogicalExpression
	{
		public object Value
		{
			get;
			set;
		}
		public ValueType Type
		{
			get;
			set;
		}
		public ValueExpression(object value, ValueType type)
		{
			this.Value = value;
			this.Type = type;
		}
		public ValueExpression(object value)
		{
			switch (System.Type.GetTypeCode(value.GetType()))
			{
			case TypeCode.Boolean:
				this.Type = ValueType.Boolean;
				goto IL_B3;
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
				this.Type = ValueType.Integer;
				goto IL_B3;
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
				this.Type = ValueType.Float;
				goto IL_B3;
			case TypeCode.DateTime:
				this.Type = ValueType.DateTime;
				goto IL_B3;
			case TypeCode.String:
				this.Type = ValueType.String;
				goto IL_B3;
			}
			throw new EvaluationException("This value could not be handled: " + value);
			IL_B3:
			this.Value = value;
		}
		public ValueExpression(string value)
		{
			this.Value = value;
			this.Type = ValueType.String;
		}
		public ValueExpression(int value)
		{
			this.Value = value;
			this.Type = ValueType.Integer;
		}
		public ValueExpression(float value)
		{
			this.Value = value;
			this.Type = ValueType.Float;
		}
		public ValueExpression(DateTime value)
		{
			this.Value = value;
			this.Type = ValueType.DateTime;
		}
		public ValueExpression(bool value)
		{
			this.Value = value;
			this.Type = ValueType.Boolean;
		}
		public override void Accept(LogicalExpressionVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
