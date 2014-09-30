using System;
namespace EcmaScript.NET.Types
{
	internal sealed class BuiltinMath : IdScriptableObject
	{
		private const double NET_WORKAROUND_1 = 2.35619449019234;
		private const double NET_WORKAROUND_2 = -2.35619449019234;
		private const double NET_WORKAROUND_3 = 0.785398163397448;
		private const double NET_WORKAROUND_4 = -0.785398163397448;
		private const int Id_toSource = 1;
		private const int Id_abs = 2;
		private const int Id_acos = 3;
		private const int Id_asin = 4;
		private const int Id_atan = 5;
		private const int Id_atan2 = 6;
		private const int Id_ceil = 7;
		private const int Id_cos = 8;
		private const int Id_exp = 9;
		private const int Id_floor = 10;
		private const int Id_log = 11;
		private const int Id_max = 12;
		private const int Id_min = 13;
		private const int Id_pow = 14;
		private const int Id_random = 15;
		private const int Id_round = 16;
		private const int Id_sin = 17;
		private const int Id_sqrt = 18;
		private const int Id_tan = 19;
		private const int LAST_METHOD_ID = 19;
		private const int Id_E = 20;
		private const int Id_PI = 21;
		private const int Id_LN10 = 22;
		private const int Id_LN2 = 23;
		private const int Id_LOG2E = 24;
		private const int Id_LOG10E = 25;
		private const int Id_SQRT1_2 = 26;
		private const int Id_SQRT2 = 27;
		private const int MAX_ID = 27;
		private static readonly object MATH_TAG = new object();
		public override string ClassName
		{
			get
			{
				return "Math";
			}
		}
		internal static void Init(IScriptable scope, bool zealed)
		{
			BuiltinMath builtinMath = new BuiltinMath();
			builtinMath.ActivatePrototypeMap(27);
			builtinMath.SetPrototype(ScriptableObject.GetObjectPrototype(scope));
			builtinMath.ParentScope = scope;
			if (zealed)
			{
				builtinMath.SealObject();
			}
			ScriptableObject.DefineProperty(scope, "Math", builtinMath, 7);
		}
		private BuiltinMath()
		{
		}
		protected internal override void InitPrototypeId(int id)
		{
			if (id <= 19)
			{
				int arity;
				string name;
				switch (id)
				{
				case 1:
					arity = 0;
					name = "toSource";
					break;
				case 2:
					arity = 1;
					name = "abs";
					break;
				case 3:
					arity = 1;
					name = "acos";
					break;
				case 4:
					arity = 1;
					name = "asin";
					break;
				case 5:
					arity = 1;
					name = "atan";
					break;
				case 6:
					arity = 2;
					name = "atan2";
					break;
				case 7:
					arity = 1;
					name = "ceil";
					break;
				case 8:
					arity = 1;
					name = "cos";
					break;
				case 9:
					arity = 1;
					name = "exp";
					break;
				case 10:
					arity = 1;
					name = "floor";
					break;
				case 11:
					arity = 1;
					name = "log";
					break;
				case 12:
					arity = 2;
					name = "max";
					break;
				case 13:
					arity = 2;
					name = "min";
					break;
				case 14:
					arity = 2;
					name = "pow";
					break;
				case 15:
					arity = 0;
					name = "random";
					break;
				case 16:
					arity = 1;
					name = "round";
					break;
				case 17:
					arity = 1;
					name = "sin";
					break;
				case 18:
					arity = 1;
					name = "sqrt";
					break;
				case 19:
					arity = 1;
					name = "tan";
					break;
				default:
					throw new ApplicationException(Convert.ToString(id));
				}
				base.InitPrototypeMethod(BuiltinMath.MATH_TAG, id, name, arity);
			}
			else
			{
				string name;
				double num;
				switch (id)
				{
				case 20:
					num = 2.7182818284590451;
					name = "E";
					break;
				case 21:
					num = 3.1415926535897931;
					name = "PI";
					break;
				case 22:
					num = 2.3025850929940459;
					name = "LN10";
					break;
				case 23:
					num = 0.69314718055994529;
					name = "LN2";
					break;
				case 24:
					num = 1.4426950408889634;
					name = "LOG2E";
					break;
				case 25:
					num = 0.43429448190325182;
					name = "LOG10E";
					break;
				case 26:
					num = 0.70710678118654757;
					name = "SQRT1_2";
					break;
				case 27:
					num = 1.4142135623730951;
					name = "SQRT2";
					break;
				default:
					throw new ApplicationException(Convert.ToString(id));
				}
				base.InitPrototypeValue(id, name, num, 7);
			}
		}
		public override object ExecIdCall(IdFunctionObject f, Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			object result;
			if (!f.HasTag(BuiltinMath.MATH_TAG))
			{
				result = base.ExecIdCall(f, cx, scope, thisObj, args);
			}
			else
			{
				int methodId = f.MethodId;
				double num;
				switch (methodId)
				{
				case 1:
					result = "Math";
					return result;
				case 2:
					num = ScriptConvert.ToNumber(args, 0);
					num = ((num == 0.0) ? 0.0 : ((num < 0.0) ? (-num) : num));
					break;
				case 3:
				case 4:
					num = ScriptConvert.ToNumber(args, 0);
					if (!double.IsNaN(num) && -1.0 <= num && num <= 1.0)
					{
						num = ((methodId == 3) ? Math.Acos(num) : Math.Asin(num));
					}
					else
					{
						num = double.NaN;
					}
					break;
				case 5:
					num = ScriptConvert.ToNumber(args, 0);
					num = Math.Atan(num);
					break;
				case 6:
				{
					num = ScriptConvert.ToNumber(args, 0);
					double num2 = ScriptConvert.ToNumber(args, 1);
					if (num == double.PositiveInfinity && num2 == double.PositiveInfinity)
					{
						num = 0.785398163397448;
					}
					else
					{
						if (num == double.PositiveInfinity && num2 == double.NegativeInfinity)
						{
							num = 2.35619449019234;
						}
						else
						{
							if (num == double.NegativeInfinity && num2 == double.PositiveInfinity)
							{
								num = -0.785398163397448;
							}
							else
							{
								if (num == double.NegativeInfinity && num2 == double.NegativeInfinity)
								{
									num = -2.35619449019234;
								}
								else
								{
									num = Math.Atan2(num, num2);
								}
							}
						}
					}
					break;
				}
				case 7:
					num = ScriptConvert.ToNumber(args, 0);
					num = Math.Ceiling(num);
					break;
				case 8:
					num = ScriptConvert.ToNumber(args, 0);
					num = ((num == double.PositiveInfinity || num == double.NegativeInfinity) ? double.NaN : Math.Cos(num));
					break;
				case 9:
					num = ScriptConvert.ToNumber(args, 0);
					num = ((num == double.PositiveInfinity) ? num : ((num == double.NegativeInfinity) ? 0.0 : Math.Exp(num)));
					break;
				case 10:
					num = ScriptConvert.ToNumber(args, 0);
					num = Math.Floor(num);
					break;
				case 11:
					num = ScriptConvert.ToNumber(args, 0);
					num = ((num < 0.0) ? double.NaN : Math.Log(num));
					break;
				case 12:
				case 13:
					num = ((methodId == 12) ? double.NegativeInfinity : double.PositiveInfinity);
					for (int num3 = 0; num3 != args.Length; num3++)
					{
						double num4 = ScriptConvert.ToNumber(args[num3]);
						if (double.IsNaN(num4))
						{
							num = num4;
							break;
						}
						if (methodId == 12)
						{
							num = Math.Max(num, num4);
						}
						else
						{
							num = Math.Min(num, num4);
						}
					}
					break;
				case 14:
					num = ScriptConvert.ToNumber(args, 0);
					num = this.js_pow(num, ScriptConvert.ToNumber(args, 1));
					break;
				case 15:
					num = new Random().NextDouble();
					break;
				case 16:
					num = ScriptConvert.ToNumber(args, 0);
					if (!double.IsNaN(num) && num != double.PositiveInfinity && num != double.NegativeInfinity)
					{
						long num5 = (long)Math.Floor(num + 0.5);
						if (num5 != 0L)
						{
							num = (double)num5;
						}
						else
						{
							if (num < 0.0)
							{
								num = BuiltinNumber.NegativeZero;
							}
							else
							{
								if (num != 0.0)
								{
									num = 0.0;
								}
							}
						}
					}
					break;
				case 17:
					num = ScriptConvert.ToNumber(args, 0);
					num = ((num == double.PositiveInfinity || num == double.NegativeInfinity) ? double.NaN : Math.Sin(num));
					break;
				case 18:
					num = ScriptConvert.ToNumber(args, 0);
					num = Math.Sqrt(num);
					break;
				case 19:
					num = ScriptConvert.ToNumber(args, 0);
					num = Math.Tan(num);
					break;
				default:
					throw new ApplicationException(Convert.ToString(methodId));
				}
				result = num;
			}
			return result;
		}
		private double js_pow(double x, double y)
		{
			double result;
			if (double.IsNaN(y))
			{
				result = y;
			}
			else
			{
				if (y == 0.0)
				{
					result = 1.0;
				}
				else
				{
					if (x == 0.0)
					{
						if (1.0 / x > 0.0)
						{
							result = ((y > 0.0) ? 0.0 : double.PositiveInfinity);
						}
						else
						{
							long num = (long)y;
							if ((double)num == y && (num & 1L) != 0L)
							{
								result = ((y > 0.0) ? -0.0 : double.NegativeInfinity);
							}
							else
							{
								result = ((y > 0.0) ? 0.0 : double.PositiveInfinity);
							}
						}
					}
					else
					{
						result = Math.Pow(x, y);
						if (!double.IsNaN(y))
						{
							if (y == double.PositiveInfinity)
							{
								if (x < -1.0 || 1.0 < x)
								{
									result = double.PositiveInfinity;
								}
								else
								{
									if (-1.0 < x && x < 1.0)
									{
										result = 0.0;
									}
									else
									{
										if (x == 1.0)
										{
											result = double.NaN;
										}
										else
										{
											if (x == -1.0)
											{
												result = double.NaN;
											}
										}
									}
								}
							}
							else
							{
								if (y == double.NegativeInfinity)
								{
									if (x < -1.0 || 1.0 < x)
									{
										result = 0.0;
									}
									else
									{
										if (-1.0 < x && x < 1.0)
										{
											result = double.PositiveInfinity;
										}
										else
										{
											if (x == 1.0)
											{
												result = double.NaN;
											}
											else
											{
												if (x == -1.0)
												{
													result = double.NaN;
												}
											}
										}
									}
								}
								else
								{
									if (x == double.PositiveInfinity)
									{
										result = ((y > 0.0) ? double.PositiveInfinity : 0.0);
									}
									else
									{
										if (x == double.NegativeInfinity)
										{
											long num = (long)y;
											if ((double)num == y && (num & 1L) != 0L)
											{
												result = ((y > 0.0) ? double.NegativeInfinity : -0.0);
											}
											else
											{
												result = ((y > 0.0) ? double.PositiveInfinity : 0.0);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}
		protected internal override int FindPrototypeId(string s)
		{
			int result = 0;
			string text = null;
			switch (s.Length)
			{
			case 1:
				if (s[0] == 'E')
				{
					result = 20;
					return result;
				}
				break;
			case 2:
				if (s[0] == 'P' && s[1] == 'I')
				{
					result = 21;
					return result;
				}
				break;
			case 3:
			{
				char c = s[0];
				if (c != 'L')
				{
					switch (c)
					{
					case 'a':
						if (s[2] == 's' && s[1] == 'b')
						{
							result = 2;
							return result;
						}
						break;
					case 'b':
					case 'd':
						break;
					case 'c':
						if (s[2] == 's' && s[1] == 'o')
						{
							result = 8;
							return result;
						}
						break;
					case 'e':
						if (s[2] == 'p' && s[1] == 'x')
						{
							result = 9;
							return result;
						}
						break;
					default:
						switch (c)
						{
						case 'l':
							if (s[2] == 'g' && s[1] == 'o')
							{
								result = 11;
								return result;
							}
							break;
						case 'm':
						{
							int num = (int)s[2];
							if (num == 110)
							{
								if (s[1] == 'i')
								{
									result = 13;
									return result;
								}
							}
							else
							{
								if (num == 120)
								{
									if (s[1] == 'a')
									{
										result = 12;
										return result;
									}
								}
							}
							break;
						}
						case 'p':
							if (s[2] == 'w' && s[1] == 'o')
							{
								result = 14;
								return result;
							}
							break;
						case 's':
							if (s[2] == 'n' && s[1] == 'i')
							{
								result = 17;
								return result;
							}
							break;
						case 't':
							if (s[2] == 'n' && s[1] == 'a')
							{
								result = 19;
								return result;
							}
							break;
						}
						break;
					}
				}
				else
				{
					if (s[2] == '2' && s[1] == 'N')
					{
						result = 23;
						return result;
					}
				}
				break;
			}
			case 4:
			{
				char c = s[1];
				if (c != 'N')
				{
					switch (c)
					{
					case 'c':
						text = "acos";
						result = 3;
						break;
					case 'd':
						break;
					case 'e':
						text = "ceil";
						result = 7;
						break;
					default:
						switch (c)
						{
						case 'q':
							text = "sqrt";
							result = 18;
							break;
						case 's':
							text = "asin";
							result = 4;
							break;
						case 't':
							text = "atan";
							result = 5;
							break;
						}
						break;
					}
				}
				else
				{
					text = "LN10";
					result = 22;
				}
				break;
			}
			case 5:
			{
				char c = s[0];
				if (c <= 'S')
				{
					if (c != 'L')
					{
						if (c == 'S')
						{
							text = "SQRT2";
							result = 27;
						}
					}
					else
					{
						text = "LOG2E";
						result = 24;
					}
				}
				else
				{
					if (c != 'a')
					{
						if (c != 'f')
						{
							if (c == 'r')
							{
								text = "round";
								result = 16;
							}
						}
						else
						{
							text = "floor";
							result = 10;
						}
					}
					else
					{
						text = "atan2";
						result = 6;
					}
				}
				break;
			}
			case 6:
			{
				int num = (int)s[0];
				if (num == 76)
				{
					text = "LOG10E";
					result = 25;
				}
				else
				{
					if (num == 114)
					{
						text = "random";
						result = 15;
					}
				}
				break;
			}
			case 7:
				text = "SQRT1_2";
				result = 26;
				break;
			case 8:
				text = "toSource";
				result = 1;
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
