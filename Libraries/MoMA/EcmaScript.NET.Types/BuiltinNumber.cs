using System;
using System.Globalization;
namespace EcmaScript.NET.Types
{
	internal sealed class BuiltinNumber : IdScriptableObject
	{
		internal const int DTOSTR_STANDARD = 0;
		internal const int DTOSTR_STANDARD_EXPONENTIAL = 1;
		internal const int DTOSTR_FIXED = 2;
		internal const int DTOSTR_EXPONENTIAL = 3;
		internal const int DTOSTR_PRECISION = 4;
		public const double NaN = double.NaN;
		public const double POSITIVE_INFINITY = double.PositiveInfinity;
		public const double NEGATIVE_INFINITY = double.NegativeInfinity;
		public const double MAX_VALUE = 1.7976931348623157E+308;
		public const double MIN_VALUE = 4.94065645841247E-324;
		private const int MAX_PRECISION = 100;
		private const int Id_constructor = 1;
		private const int Id_toString = 2;
		private const int Id_toLocaleString = 3;
		private const int Id_toSource = 4;
		private const int Id_valueOf = 5;
		private const int Id_toFixed = 6;
		private const int Id_toExponential = 7;
		private const int Id_toPrecision = 8;
		private const int MAX_PROTOTYPE_ID = 8;
		public static readonly double NegativeZero = BitConverter.Int64BitsToDouble(-9223372036854775808L);
		private static readonly object NUMBER_TAG = new object();
		private static NumberFormatInfo m_NumberFormatter = null;
		private double doubleValue;
		public override string ClassName
		{
			get
			{
				return "Number";
			}
		}
		public static NumberFormatInfo NumberFormatter
		{
			get
			{
				if (BuiltinNumber.m_NumberFormatter == null)
				{
					BuiltinNumber.m_NumberFormatter = new NumberFormatInfo();
					BuiltinNumber.m_NumberFormatter.PercentGroupSeparator = ",";
					BuiltinNumber.m_NumberFormatter.NumberDecimalSeparator = ".";
				}
				return BuiltinNumber.m_NumberFormatter;
			}
		}
		internal static void Init(IScriptable scope, bool zealed)
		{
			BuiltinNumber builtinNumber = new BuiltinNumber(0.0);
			builtinNumber.ExportAsJSClass(8, scope, zealed, 7);
		}
		private BuiltinNumber(double number)
		{
			this.doubleValue = number;
		}
		protected internal override void FillConstructorProperties(IdFunctionObject ctor)
		{
			ctor.DefineProperty("NaN", double.NaN, 7);
			ctor.DefineProperty("POSITIVE_INFINITY", double.PositiveInfinity, 7);
			ctor.DefineProperty("NEGATIVE_INFINITY", double.NegativeInfinity, 7);
			ctor.DefineProperty("MAX_VALUE", 1.7976931348623157E+308, 7);
			ctor.DefineProperty("MIN_VALUE", 4.94065645841247E-324, 7);
			base.FillConstructorProperties(ctor);
		}
		protected internal override void InitPrototypeId(int id)
		{
			int arity;
			string name;
			switch (id)
			{
			case 1:
				arity = 1;
				name = "constructor";
				break;
			case 2:
				arity = 1;
				name = "toString";
				break;
			case 3:
				arity = 1;
				name = "toLocaleString";
				break;
			case 4:
				arity = 0;
				name = "toSource";
				break;
			case 5:
				arity = 0;
				name = "valueOf";
				break;
			case 6:
				arity = 1;
				name = "toFixed";
				break;
			case 7:
				arity = 1;
				name = "toExponential";
				break;
			case 8:
				arity = 1;
				name = "toPrecision";
				break;
			default:
				throw new ArgumentException(Convert.ToString(id));
			}
			base.InitPrototypeMethod(BuiltinNumber.NUMBER_TAG, id, name, arity);
		}
		public override object ExecIdCall(IdFunctionObject f, Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			object result;
			if (!f.HasTag(BuiltinNumber.NUMBER_TAG))
			{
				result = base.ExecIdCall(f, cx, scope, thisObj, args);
			}
			else
			{
				int methodId = f.MethodId;
				if (methodId == 1)
				{
					double num = (args.Length >= 1) ? ScriptConvert.ToNumber(args[0]) : 0.0;
					if (thisObj == null)
					{
						result = new BuiltinNumber(num);
					}
					else
					{
						result = num;
					}
				}
				else
				{
					BuiltinNumber builtinNumber = thisObj as BuiltinNumber;
					if (builtinNumber == null)
					{
						throw IdScriptableObject.IncompatibleCallError(f);
					}
					double num2 = builtinNumber.doubleValue;
					switch (methodId)
					{
					case 2:
					{
						int toBase = (args.Length == 0) ? 10 : ScriptConvert.ToInt32(args[0]);
						result = ScriptConvert.ToString(num2, toBase);
						break;
					}
					case 3:
					{
						int toBase = (args.Length == 0) ? 10 : ScriptConvert.ToInt32(args[0]);
						result = ScriptConvert.ToString(num2, toBase);
						break;
					}
					case 4:
						result = "(new Number(" + ScriptConvert.ToString(num2) + "))";
						break;
					case 5:
						result = num2;
						break;
					case 6:
						result = BuiltinNumber.num_to(num2, args, 2, 2, -20, 0);
						break;
					case 7:
						result = BuiltinNumber.num_to(num2, args, 1, 3, 0, 1);
						break;
					case 8:
						if (args.Length < 0 || args[0] == Undefined.Value)
						{
							result = ScriptConvert.ToString(num2);
						}
						else
						{
							int num3 = ScriptConvert.ToInt32(args[0]);
							if (num3 < 0 || num3 > 100)
							{
								throw ScriptRuntime.ConstructError("RangeError", ScriptRuntime.GetMessage("msg.bad.precision", new object[]
								{
									ScriptConvert.ToString(args[0])
								}));
							}
							result = num2.ToString(BuiltinNumber.GetFormatString(num3));
						}
						break;
					default:
						throw new ArgumentException(Convert.ToString(methodId));
					}
				}
			}
			return result;
		}
		private static string GetFormatString(int precision)
		{
			string text = "0.";
			for (int i = 0; i < precision; i++)
			{
				text += "#";
			}
			return text;
		}
		public override string ToString()
		{
			return ScriptConvert.ToString(this.doubleValue, 10);
		}
		private static string num_to(double val, object[] args, int zeroArgMode, int oneArgMode, int precisionMin, int precisionOffset)
		{
			int num;
			if (args.Length == 0)
			{
				num = 0;
				oneArgMode = zeroArgMode;
			}
			else
			{
				num = ScriptConvert.ToInt32(args[0]);
				if (num < precisionMin || num > 100)
				{
					string message = ScriptRuntime.GetMessage("msg.bad.precision", new object[]
					{
						ScriptConvert.ToString(args[0])
					});
					throw ScriptRuntime.ConstructError("RangeError", message);
				}
			}
			string result;
			switch (zeroArgMode)
			{
			case 0:
				if (oneArgMode == 4)
				{
					result = val.ToString(num.ToString(), BuiltinNumber.NumberFormatter);
				}
				else
				{
					result = val.ToString(BuiltinNumber.NumberFormatter);
				}
				break;
			case 1:
				result = val.ToString("e" + (num + precisionOffset), BuiltinNumber.NumberFormatter);
				break;
			case 2:
				result = val.ToString("F" + (num + precisionOffset), BuiltinNumber.NumberFormatter);
				break;
			default:
				Context.CodeBug();
				result = string.Empty;
				break;
			}
			return result;
		}
		protected internal override int FindPrototypeId(string s)
		{
			int result = 0;
			string text = null;
			switch (s.Length)
			{
			case 7:
			{
				int num = (int)s[0];
				if (num == 116)
				{
					text = "toFixed";
					result = 6;
				}
				else
				{
					if (num == 118)
					{
						text = "valueOf";
						result = 5;
					}
				}
				break;
			}
			case 8:
			{
				int num = (int)s[3];
				if (num == 111)
				{
					text = "toSource";
					result = 4;
				}
				else
				{
					if (num == 116)
					{
						text = "toString";
						result = 2;
					}
				}
				break;
			}
			case 11:
			{
				int num = (int)s[0];
				if (num == 99)
				{
					text = "constructor";
					result = 1;
				}
				else
				{
					if (num == 116)
					{
						text = "toPrecision";
						result = 8;
					}
				}
				break;
			}
			case 13:
				text = "toExponential";
				result = 7;
				break;
			case 14:
				text = "toLocaleString";
				result = 3;
				break;
			}
			if (text != null && text != s && !text.Equals(s))
			{
				result = 0;
			}
			return result;
		}
	}
}
