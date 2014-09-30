using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
namespace NCalc.Domain
{
	public class EvaluationVisitor : LogicalExpressionVisitor
	{
		private readonly EvaluateOptions _options = EvaluateOptions.None;
		private static Type[] CommonTypes = new Type[]
		{
			typeof(long),
			typeof(double),
			typeof(bool),
			typeof(string),
			typeof(decimal)
		};
		public event EvaluateFunctionHandler EvaluateFunction
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.EvaluateFunction = (EvaluateFunctionHandler)Delegate.Combine(this.EvaluateFunction, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.EvaluateFunction = (EvaluateFunctionHandler)Delegate.Remove(this.EvaluateFunction, value);
			}
		}
		public event EvaluateParameterHandler EvaluateParameter
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.EvaluateParameter = (EvaluateParameterHandler)Delegate.Combine(this.EvaluateParameter, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.EvaluateParameter = (EvaluateParameterHandler)Delegate.Remove(this.EvaluateParameter, value);
			}
		}
		private bool IgnoreCase
		{
			get
			{
				return (this._options & EvaluateOptions.IgnoreCase) == EvaluateOptions.IgnoreCase;
			}
		}
		public object Result
		{
			get;
			private set;
		}
		public Dictionary<string, object> Parameters
		{
			get;
			set;
		}
		public EvaluationVisitor(EvaluateOptions options)
		{
			this._options = options;
		}
		private object Evaluate(LogicalExpression expression)
		{
			expression.Accept(this);
			return this.Result;
		}
		public override void Visit(LogicalExpression expression)
		{
			throw new Exception("The method or operation is not implemented.");
		}
		private static Type GetMostPreciseType(Type a, Type b)
		{
			Type[] commonTypes = EvaluationVisitor.CommonTypes;
			Type result;
			for (int i = 0; i < commonTypes.Length; i++)
			{
				Type type = commonTypes[i];
				if (a == type || b == type)
				{
					result = type;
					return result;
				}
			}
			result = a;
			return result;
		}
		public int CompareUsingMostPreciseType(object a, object b)
		{
			Type mostPreciseType = EvaluationVisitor.GetMostPreciseType(a.GetType(), b.GetType());
			return Comparer.Default.Compare(Convert.ChangeType(a, mostPreciseType), Convert.ChangeType(b, mostPreciseType));
		}
		public override void Visit(TernaryExpression expression)
		{
			expression.LeftExpression.Accept(this);
			bool flag = Convert.ToBoolean(this.Result);
			if (flag)
			{
				expression.MiddleExpression.Accept(this);
			}
			else
			{
				expression.RightExpression.Accept(this);
			}
		}
		private static bool IsReal(object value)
		{
			TypeCode typeCode = Type.GetTypeCode(value.GetType());
			return typeCode == TypeCode.Decimal || typeCode == TypeCode.Double || typeCode == TypeCode.Single;
		}
		public override void Visit(BinaryExpression expression)
		{
			expression.LeftExpression.Accept(this);
			object result = this.Result;
			expression.RightExpression.Accept(this);
			object result2 = this.Result;
			switch (expression.Type)
			{
			case BinaryExpressionType.And:
				this.Result = (Convert.ToBoolean(result) && Convert.ToBoolean(result2));
				break;
			case BinaryExpressionType.Or:
				this.Result = (Convert.ToBoolean(result) || Convert.ToBoolean(result2));
				break;
			case BinaryExpressionType.NotEqual:
				this.Result = (this.CompareUsingMostPreciseType(result, result2) != 0);
				break;
			case BinaryExpressionType.LesserOrEqual:
				this.Result = (this.CompareUsingMostPreciseType(result, result2) <= 0);
				break;
			case BinaryExpressionType.GreaterOrEqual:
				this.Result = (this.CompareUsingMostPreciseType(result, result2) >= 0);
				break;
			case BinaryExpressionType.Lesser:
				this.Result = (this.CompareUsingMostPreciseType(result, result2) < 0);
				break;
			case BinaryExpressionType.Greater:
				this.Result = (this.CompareUsingMostPreciseType(result, result2) > 0);
				break;
			case BinaryExpressionType.Equal:
				this.Result = (this.CompareUsingMostPreciseType(result, result2) == 0);
				break;
			case BinaryExpressionType.Minus:
				this.Result = Numbers.Soustract(result, result2);
				break;
			case BinaryExpressionType.Plus:
				if (result is string)
				{
					this.Result = result + result2;
				}
				else
				{
					this.Result = Numbers.Add(result, result2);
				}
				break;
			case BinaryExpressionType.Modulo:
				this.Result = Numbers.Modulo(result, result2);
				break;
			case BinaryExpressionType.Div:
				this.Result = ((EvaluationVisitor.IsReal(result) || EvaluationVisitor.IsReal(result2)) ? Numbers.Divide(result, result2) : Numbers.Divide(Convert.ToDouble(result), result2));
				break;
			case BinaryExpressionType.Times:
				this.Result = Numbers.Multiply(result, result2);
				break;
			case BinaryExpressionType.BitwiseOr:
				this.Result = (int)(Convert.ToUInt16(result) | Convert.ToUInt16(result2));
				break;
			case BinaryExpressionType.BitwiseAnd:
				this.Result = (int)(Convert.ToUInt16(result) & Convert.ToUInt16(result2));
				break;
			case BinaryExpressionType.BitwiseXOr:
				this.Result = (int)(Convert.ToUInt16(result) ^ Convert.ToUInt16(result2));
				break;
			case BinaryExpressionType.LeftShift:
				this.Result = (int)Convert.ToUInt16(result) << (int)Convert.ToUInt16(result2);
				break;
			case BinaryExpressionType.RightShift:
				this.Result = Convert.ToUInt16(result) >> (int)Convert.ToUInt16(result2);
				break;
			}
		}
		public override void Visit(UnaryExpression expression)
		{
			expression.Expression.Accept(this);
			switch (expression.Type)
			{
			case UnaryExpressionType.Not:
				this.Result = !Convert.ToBoolean(this.Result);
				break;
			case UnaryExpressionType.Negate:
				this.Result = Numbers.Soustract(0, this.Result);
				break;
			case UnaryExpressionType.BitwiseNot:
				this.Result = (int)(~(int)Convert.ToUInt16(this.Result));
				break;
			}
		}
		public override void Visit(ValueExpression expression)
		{
			this.Result = expression.Value;
		}
		public override void Visit(Function function)
		{
			FunctionArgs functionArgs = new FunctionArgs
			{
				Parameters = new Expression[function.Expressions.Length]
			};
			for (int i = 0; i < function.Expressions.Length; i++)
			{
				functionArgs.Parameters[i] = new Expression(function.Expressions[i], this._options);
				functionArgs.Parameters[i].EvaluateFunction += this.EvaluateFunction;
				functionArgs.Parameters[i].EvaluateParameter += this.EvaluateParameter;
				functionArgs.Parameters[i].Parameters = this.Parameters;
			}
			this.OnEvaluateFunction(this.IgnoreCase ? function.Identifier.Name.ToLower() : function.Identifier.Name, functionArgs);
			if (!functionArgs.HasResult)
			{
				string text = function.Identifier.Name.ToLower();
				switch (text)
				{
				case "abs":
					this.CheckCase("Abs", function.Identifier.Name);
					if (function.Expressions.Length != 1)
					{
						throw new ArgumentException("Abs() takes exactly 1 argument");
					}
					this.Result = Math.Abs(Convert.ToDecimal(this.Evaluate(function.Expressions[0])));
					return;
				case "acos":
					this.CheckCase("Acos", function.Identifier.Name);
					if (function.Expressions.Length != 1)
					{
						throw new ArgumentException("Acos() takes exactly 1 argument");
					}
					this.Result = Math.Acos(Convert.ToDouble(this.Evaluate(function.Expressions[0])));
					return;
				case "asin":
					this.CheckCase("Asin", function.Identifier.Name);
					if (function.Expressions.Length != 1)
					{
						throw new ArgumentException("Asin() takes exactly 1 argument");
					}
					this.Result = Math.Asin(Convert.ToDouble(this.Evaluate(function.Expressions[0])));
					return;
				case "atan":
					this.CheckCase("Atan", function.Identifier.Name);
					if (function.Expressions.Length != 1)
					{
						throw new ArgumentException("Atan() takes exactly 1 argument");
					}
					this.Result = Math.Atan(Convert.ToDouble(this.Evaluate(function.Expressions[0])));
					return;
				case "ceiling":
					this.CheckCase("Ceiling", function.Identifier.Name);
					if (function.Expressions.Length != 1)
					{
						throw new ArgumentException("Ceiling() takes exactly 1 argument");
					}
					this.Result = Math.Ceiling(Convert.ToDouble(this.Evaluate(function.Expressions[0])));
					return;
				case "cos":
					this.CheckCase("Cos", function.Identifier.Name);
					if (function.Expressions.Length != 1)
					{
						throw new ArgumentException("Cos() takes exactly 1 argument");
					}
					this.Result = Math.Cos(Convert.ToDouble(this.Evaluate(function.Expressions[0])));
					return;
				case "exp":
					this.CheckCase("Exp", function.Identifier.Name);
					if (function.Expressions.Length != 1)
					{
						throw new ArgumentException("Exp() takes exactly 1 argument");
					}
					this.Result = Math.Exp(Convert.ToDouble(this.Evaluate(function.Expressions[0])));
					return;
				case "floor":
					this.CheckCase("Floor", function.Identifier.Name);
					if (function.Expressions.Length != 1)
					{
						throw new ArgumentException("Floor() takes exactly 1 argument");
					}
					this.Result = Math.Floor(Convert.ToDouble(this.Evaluate(function.Expressions[0])));
					return;
				case "ieeeremainder":
					this.CheckCase("IEEERemainder", function.Identifier.Name);
					if (function.Expressions.Length != 2)
					{
						throw new ArgumentException("IEEERemainder() takes exactly 2 arguments");
					}
					this.Result = Math.IEEERemainder(Convert.ToDouble(this.Evaluate(function.Expressions[0])), Convert.ToDouble(this.Evaluate(function.Expressions[1])));
					return;
				case "log":
					this.CheckCase("Log", function.Identifier.Name);
					if (function.Expressions.Length != 2)
					{
						throw new ArgumentException("Log() takes exactly 2 arguments");
					}
					this.Result = Math.Log(Convert.ToDouble(this.Evaluate(function.Expressions[0])), Convert.ToDouble(this.Evaluate(function.Expressions[1])));
					return;
				case "log10":
					this.CheckCase("Log10", function.Identifier.Name);
					if (function.Expressions.Length != 1)
					{
						throw new ArgumentException("Log10() takes exactly 1 argument");
					}
					this.Result = Math.Log10(Convert.ToDouble(this.Evaluate(function.Expressions[0])));
					return;
				case "pow":
					this.CheckCase("Pow", function.Identifier.Name);
					if (function.Expressions.Length != 2)
					{
						throw new ArgumentException("Pow() takes exactly 2 arguments");
					}
					this.Result = Math.Pow(Convert.ToDouble(this.Evaluate(function.Expressions[0])), Convert.ToDouble(this.Evaluate(function.Expressions[1])));
					return;
				case "round":
				{
					this.CheckCase("Round", function.Identifier.Name);
					if (function.Expressions.Length != 2)
					{
						throw new ArgumentException("Round() takes exactly 2 arguments");
					}
					MidpointRounding mode = ((this._options & EvaluateOptions.RoundAwayFromZero) == EvaluateOptions.RoundAwayFromZero) ? MidpointRounding.AwayFromZero : MidpointRounding.ToEven;
					this.Result = Math.Round(Convert.ToDouble(this.Evaluate(function.Expressions[0])), (int)Convert.ToInt16(this.Evaluate(function.Expressions[1])), mode);
					return;
				}
				case "sign":
					this.CheckCase("Sign", function.Identifier.Name);
					if (function.Expressions.Length != 1)
					{
						throw new ArgumentException("Sign() takes exactly 1 argument");
					}
					this.Result = Math.Sign(Convert.ToDouble(this.Evaluate(function.Expressions[0])));
					return;
				case "sin":
					this.CheckCase("Sin", function.Identifier.Name);
					if (function.Expressions.Length != 1)
					{
						throw new ArgumentException("Sin() takes exactly 1 argument");
					}
					this.Result = Math.Sin(Convert.ToDouble(this.Evaluate(function.Expressions[0])));
					return;
				case "sqrt":
					this.CheckCase("Sqrt", function.Identifier.Name);
					if (function.Expressions.Length != 1)
					{
						throw new ArgumentException("Sqrt() takes exactly 1 argument");
					}
					this.Result = Math.Sqrt(Convert.ToDouble(this.Evaluate(function.Expressions[0])));
					return;
				case "tan":
					this.CheckCase("Tan", function.Identifier.Name);
					if (function.Expressions.Length != 1)
					{
						throw new ArgumentException("Tan() takes exactly 1 argument");
					}
					this.Result = Math.Tan(Convert.ToDouble(this.Evaluate(function.Expressions[0])));
					return;
				case "truncate":
					this.CheckCase("Truncate", function.Identifier.Name);
					if (function.Expressions.Length != 1)
					{
						throw new ArgumentException("Truncate() takes exactly 1 argument");
					}
					this.Result = Math.Truncate(Convert.ToDouble(this.Evaluate(function.Expressions[0])));
					return;
				case "max":
				{
					this.CheckCase("Max", function.Identifier.Name);
					if (function.Expressions.Length != 2)
					{
						throw new ArgumentException("Max() takes exactly 2 arguments");
					}
					object a = this.Evaluate(function.Expressions[0]);
					object b = this.Evaluate(function.Expressions[1]);
					this.Result = Numbers.Max(a, b);
					return;
				}
				case "min":
				{
					this.CheckCase("Min", function.Identifier.Name);
					if (function.Expressions.Length != 2)
					{
						throw new ArgumentException("Min() takes exactly 2 arguments");
					}
					object a2 = this.Evaluate(function.Expressions[0]);
					object b2 = this.Evaluate(function.Expressions[1]);
					this.Result = Numbers.Min(a2, b2);
					return;
				}
				case "if":
					this.CheckCase("if", function.Identifier.Name);
					if (function.Expressions.Length != 3)
					{
						throw new ArgumentException("if() takes exactly 3 arguments");
					}
					this.Result = (Convert.ToBoolean(this.Evaluate(function.Expressions[0])) ? this.Evaluate(function.Expressions[1]) : this.Evaluate(function.Expressions[2]));
					return;
				case "in":
				{
					this.CheckCase("in", function.Identifier.Name);
					if (function.Expressions.Length < 2)
					{
						throw new ArgumentException("in() takes at least 2 arguments");
					}
					object a3 = this.Evaluate(function.Expressions[0]);
					bool flag = false;
					for (int i = 1; i < function.Expressions.Length; i++)
					{
						object b3 = this.Evaluate(function.Expressions[i]);
						if (this.CompareUsingMostPreciseType(a3, b3) == 0)
						{
							flag = true;
							break;
						}
					}
					this.Result = flag;
					return;
				}
				}
				throw new ArgumentException("Function not found", function.Identifier.Name);
			}
			this.Result = functionArgs.Result;
		}
		private void CheckCase(string function, string called)
		{
			if (this.IgnoreCase)
			{
				if (!(function.ToLower() == called.ToLower()))
				{
					throw new ArgumentException("Function not found", called);
				}
			}
			else
			{
				if (function != called)
				{
					throw new ArgumentException(string.Format("Function not found {0}. Try {1} instead.", called, function));
				}
			}
		}
		private void OnEvaluateFunction(string name, FunctionArgs args)
		{
			if (this.EvaluateFunction != null)
			{
				this.EvaluateFunction(name, args);
			}
		}
		public override void Visit(Identifier parameter)
		{
			if (this.Parameters.ContainsKey(parameter.Name))
			{
				if (this.Parameters[parameter.Name] is Expression)
				{
					Expression expression = (Expression)this.Parameters[parameter.Name];
					foreach (KeyValuePair<string, object> current in this.Parameters)
					{
						expression.Parameters[current.Key] = current.Value;
					}
					expression.EvaluateFunction += this.EvaluateFunction;
					expression.EvaluateParameter += this.EvaluateParameter;
					this.Result = ((Expression)this.Parameters[parameter.Name]).Evaluate();
				}
				else
				{
					this.Result = this.Parameters[parameter.Name];
				}
			}
			else
			{
				ParameterArgs parameterArgs = new ParameterArgs();
				this.OnEvaluateParameter(parameter.Name, parameterArgs);
				if (!parameterArgs.HasResult)
				{
					throw new ArgumentException("Parameter was not defined", parameter.Name);
				}
				this.Result = parameterArgs.Result;
			}
		}
		private void OnEvaluateParameter(string name, ParameterArgs args)
		{
			if (this.EvaluateParameter != null)
			{
				this.EvaluateParameter(name, args);
			}
		}
	}
}
