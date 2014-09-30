using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public sealed class ScriptConvert
	{
		internal static int XDigitToInt(int c, int accumulator)
		{
			if (c <= 57)
			{
				c -= 48;
				if (0 <= c)
				{
					goto IL_84;
				}
			}
			else
			{
				if (c <= 70)
				{
					if (65 <= c)
					{
						c -= 55;
						goto IL_84;
					}
				}
				else
				{
					if (c <= 102)
					{
						if (97 <= c)
						{
							c -= 87;
							goto IL_84;
						}
					}
				}
			}
			int result = -1;
			return result;
			IL_84:
			result = (accumulator << 4 | c);
			return result;
		}
		public static IScriptable ToObject(IScriptable scope, object val)
		{
			IScriptable result;
			if (val is IScriptable)
			{
				result = (IScriptable)val;
			}
			else
			{
				result = ScriptConvert.ToObject(null, scope, val);
			}
			return result;
		}
		public static IScriptable ToObjectOrNull(Context cx, object obj)
		{
			IScriptable result;
			if (obj is IScriptable)
			{
				result = (IScriptable)obj;
			}
			else
			{
				if (obj != null && obj != Undefined.Value)
				{
					result = ScriptConvert.ToObject(cx, ScriptRuntime.getTopCallScope(cx), obj);
				}
				else
				{
					result = null;
				}
			}
			return result;
		}
		public static IScriptable ToObject(Context cx, IScriptable scope, object val)
		{
			IScriptable result;
			if (val is IScriptable)
			{
				result = (IScriptable)val;
			}
			else
			{
				if (val == null)
				{
					throw ScriptRuntime.TypeErrorById("msg.null.to.object", new string[0]);
				}
				if (val == Undefined.Value)
				{
					throw ScriptRuntime.TypeErrorById("msg.undef.to.object", new string[0]);
				}
				string text = (val is string) ? "String" : (CliHelper.IsNumber(val) ? "Number" : ((val is bool) ? "Boolean" : null));
				if (text != null)
				{
					object[] args = new object[]
					{
						val
					};
					scope = ScriptableObject.GetTopLevelScope(scope);
					result = ScriptRuntime.NewObject((cx == null) ? Context.CurrentContext : cx, scope, text, args);
				}
				else
				{
					object obj = cx.Wrap(scope, val, null);
					if (!(obj is IScriptable))
					{
						throw ScriptRuntime.errorWithClassName("msg.invalid.type", val);
					}
					result = (IScriptable)obj;
				}
			}
			return result;
		}
		public static double ToInteger(object val)
		{
			return ScriptConvert.ToInteger(ScriptConvert.ToNumber(val));
		}
		public static double ToInteger(double d)
		{
			double result;
			if (double.IsNaN(d))
			{
				result = 0.0;
			}
			else
			{
				if (d == 0.0 || d == double.PositiveInfinity || d == double.NegativeInfinity)
				{
					result = d;
				}
				else
				{
					if (d > 0.0)
					{
						result = Math.Floor(d);
					}
					else
					{
						result = Math.Ceiling(d);
					}
				}
			}
			return result;
		}
		public static double ToInteger(object[] args, int index)
		{
			return (index < args.Length) ? ScriptConvert.ToInteger(args[index]) : 0.0;
		}
		public static int ToInt32(object val)
		{
			int result;
			if (val is int)
			{
				result = (int)val;
			}
			else
			{
				result = ScriptConvert.ToInt32(ScriptConvert.ToNumber(val));
			}
			return result;
		}
		public static int ToInt32(object[] args, int index)
		{
			return (index < args.Length) ? ScriptConvert.ToInt32(args[index]) : 0;
		}
		public static int ToInt32(double d)
		{
			int num = (int)d;
			int result;
			if ((double)num == d)
			{
				result = num;
			}
			else
			{
				if (double.IsNaN(d) || d == double.PositiveInfinity || d == double.NegativeInfinity)
				{
					result = 0;
				}
				else
				{
					d = ((d >= 0.0) ? Math.Floor(d) : Math.Ceiling(d));
					double y = 4294967296.0;
					d = Math.IEEERemainder(d, y);
					long num2 = (long)d;
					result = (int)num2;
				}
			}
			return result;
		}
		public static long ToUint32(double d)
		{
			long num = (long)d;
			long result;
			if ((double)num == d)
			{
				result = (num & (long)((ulong)-1));
			}
			else
			{
				if (double.IsNaN(d) || d == double.PositiveInfinity || d == double.NegativeInfinity)
				{
					result = 0L;
				}
				else
				{
					d = ((d >= 0.0) ? Math.Floor(d) : Math.Ceiling(d));
					double y = 4294967296.0;
					num = (long)Math.IEEERemainder(d, y);
					result = (num & -1L);
				}
			}
			return result;
		}
		public static long ToUint32(object val)
		{
			return ScriptConvert.ToUint32(ScriptConvert.ToNumber(val));
		}
		public static bool ToBoolean(object val)
		{
			bool result;
			while (!(val is bool))
			{
				if (val == null || val == Undefined.Value)
				{
					result = false;
				}
				else
				{
					if (val is string)
					{
						result = (((string)val).Length != 0);
					}
					else
					{
						if (CliHelper.IsNumber(val))
						{
							double num = Convert.ToDouble(val);
							result = (!double.IsNaN(num) && num != 0.0);
						}
						else
						{
							if (val is IScriptable)
							{
								if (Context.CurrentContext.VersionECMA1)
								{
									result = true;
								}
								else
								{
									val = ((IScriptable)val).GetDefaultValue(typeof(bool));
									if (val is IScriptable)
									{
										throw ScriptRuntime.errorWithClassName("msg.primitive.expected", val);
									}
									continue;
								}
							}
							else
							{
								ScriptRuntime.WarnAboutNonJSObject(val);
								result = true;
							}
						}
					}
				}
				return result;
			}
			result = (bool)val;
			return result;
		}
		public static bool ToBoolean(object[] args, int index)
		{
			return index < args.Length && ScriptConvert.ToBoolean(args[index]);
		}
		public static double ToNumber(object val)
		{
			double result;
			while (!(val is double))
			{
				if (CliHelper.IsNumber(val))
				{
					result = Convert.ToDouble(val);
				}
				else
				{
					if (val == null)
					{
						result = 0.0;
					}
					else
					{
						if (val == Undefined.Value)
						{
							result = double.NaN;
						}
						else
						{
							if (val is string)
							{
								result = ScriptConvert.ToNumber((string)val);
							}
							else
							{
								if (val is bool)
								{
									result = (((bool)val) ? 1.0 : 0.0);
								}
								else
								{
									if (val is IScriptable)
									{
										val = ((IScriptable)val).GetDefaultValue(typeof(long));
										if (val is IScriptable)
										{
											throw ScriptRuntime.errorWithClassName("msg.primitive.expected", val);
										}
										continue;
									}
									else
									{
										ScriptRuntime.WarnAboutNonJSObject(val);
										result = double.NaN;
									}
								}
							}
						}
					}
				}
				return result;
			}
			result = (double)val;
			return result;
		}
		public static double ToNumber(object[] args, int index)
		{
			return (index < args.Length) ? ScriptConvert.ToNumber(args[index]) : double.NaN;
		}
		internal static double ToNumber(string s, int start, int radix)
		{
			char c = '9';
			char c2 = 'a';
			char c3 = 'A';
			int length = s.Length;
			if (radix < 10)
			{
				c = (char)(48 + radix - 1);
			}
			if (radix > 10)
			{
				c2 = (char)(97 + radix - 10);
				c3 = (char)(65 + radix - 10);
			}
			double num = 0.0;
			int i;
			for (i = start; i < length; i++)
			{
				char c4 = s[i];
				int num2;
				if ('0' <= c4 && c4 <= c)
				{
					num2 = (int)(c4 - '0');
				}
				else
				{
					if ('a' <= c4 && c4 < c2)
					{
						num2 = (int)(c4 - 'a' + '\n');
					}
					else
					{
						if ('A' > c4 || c4 >= c3)
						{
							break;
						}
						num2 = (int)(c4 - 'A' + '\n');
					}
				}
				num = num * (double)radix + (double)num2;
			}
			double result;
			if (start == i)
			{
				result = double.NaN;
			}
			else
			{
				if (num >= 9007199254740992.0)
				{
					if (radix == 10)
					{
						try
						{
							result = double.Parse(s.Substring(start, i - start));
							return result;
						}
						catch (FormatException)
						{
							result = double.NaN;
							return result;
						}
					}
					if (radix == 2 || radix == 4 || radix == 8 || radix == 16 || radix == 32)
					{
						int num3 = 1;
						int num4 = 0;
						int num5 = 0;
						int num6 = 53;
						double num7 = 0.0;
						bool flag = false;
						bool flag2 = false;
						while (true)
						{
							if (num3 == 1)
							{
								if (start == i)
								{
									break;
								}
								num4 = (int)s[start++];
								if (48 <= num4 && num4 <= 57)
								{
									num4 -= 48;
								}
								else
								{
									if (97 <= num4 && num4 <= 122)
									{
										num4 -= 87;
									}
									else
									{
										num4 -= 55;
									}
								}
								num3 = radix;
							}
							num3 >>= 1;
							bool flag3 = (num4 & num3) != 0;
							switch (num5)
							{
							case 0:
								if (flag3)
								{
									num6--;
									num = 1.0;
									num5 = 1;
								}
								break;
							case 1:
								num *= 2.0;
								if (flag3)
								{
									num += 1.0;
								}
								num6--;
								if (num6 == 0)
								{
									flag = flag3;
									num5 = 2;
								}
								break;
							case 2:
								flag2 = flag3;
								num7 = 2.0;
								num5 = 3;
								break;
							case 3:
								if (flag3)
								{
									num5 = 4;
								}
								goto IL_35C;
							case 4:
								goto IL_35C;
							}
							continue;
							IL_35C:
							num7 *= 2.0;
						}
						switch (num5)
						{
						case 0:
							num = 0.0;
							break;
						case 3:
							if (flag2 & flag)
							{
								num += 1.0;
							}
							num *= num7;
							break;
						case 4:
							if (flag2)
							{
								num += 1.0;
							}
							num *= num7;
							break;
						}
					}
				}
				result = num;
			}
			return result;
		}
		public static string ToString(object[] args, int index)
		{
			return (index < args.Length) ? ScriptConvert.ToString(args[index]) : "undefined";
		}
		internal static object ToPrimitive(object val)
		{
			object result;
			if (!(val is IScriptable))
			{
				result = val;
			}
			else
			{
				IScriptable scriptable = (IScriptable)val;
				object defaultValue = scriptable.GetDefaultValue(null);
				if (defaultValue is IScriptable)
				{
					throw ScriptRuntime.TypeErrorById("msg.bad.default.value", new string[0]);
				}
				result = defaultValue;
			}
			return result;
		}
		public static string ToString(object val)
		{
			string result;
			while (val != null)
			{
				if (val == Undefined.Value)
				{
					result = "undefined";
				}
				else
				{
					if (val is string)
					{
						result = (string)val;
					}
					else
					{
						if (val is bool)
						{
							result = (((bool)val) ? "true" : "false");
						}
						else
						{
							if (CliHelper.IsNumber(val))
							{
								result = ScriptConvert.ToString(Convert.ToDouble(val), 10);
							}
							else
							{
								if (val is IScriptable)
								{
									val = ((IScriptable)val).GetDefaultValue(typeof(string));
									if (val is IScriptable)
									{
										throw ScriptRuntime.errorWithClassName("msg.primitive.expected", val);
									}
									continue;
								}
								else
								{
									result = val.ToString();
								}
							}
						}
					}
				}
				return result;
			}
			result = "null";
			return result;
		}
		public static double ToNumber(string input)
		{
			int length = input.Length;
			int num = 0;
			char[] array = input.ToCharArray();
			double result;
			while (num != length)
			{
				char c = array[num];
				if (!char.IsWhiteSpace(c))
				{
					if (c == '0')
					{
						if (num + 2 < length)
						{
							int num2 = (int)array[num + 1];
							if (num2 == 120 || num2 == 88)
							{
								result = ScriptConvert.ToNumber(input, num + 2, 16);
								return result;
							}
						}
					}
					else
					{
						if (c == '+' || c == '-')
						{
							if (num + 3 < length && array[num + 1] == '0')
							{
								int num3 = (int)array[num + 2];
								if (num3 == 120 || num3 == 88)
								{
									double num4 = ScriptConvert.ToNumber(input, num + 3, 16);
									result = ((c == '-') ? (-num4) : num4);
									return result;
								}
							}
						}
					}
					int num5 = length - 1;
					char c2;
					while (char.IsWhiteSpace(c2 = array[num5]))
					{
						num5--;
					}
					if (c2 == 'y')
					{
						if (c == '+' || c == '-')
						{
							num++;
						}
						if (num + 7 == num5 && string.Compare(input, num, "Infinity", 0, 8) == 0)
						{
							result = ((c == '-') ? double.NegativeInfinity : double.PositiveInfinity);
						}
						else
						{
							result = double.NaN;
						}
					}
					else
					{
						string text = input.Substring(num, num5 + 1 - num);
						for (int i = text.Length - 1; i >= 0; i--)
						{
							char c3 = text[i];
							if (('0' > c3 || c3 > '9') && c3 != '.' && c3 != 'e' && c3 != 'E' && c3 != '+' && c3 != '-')
							{
								result = double.NaN;
								return result;
							}
						}
						try
						{
							double num6 = double.Parse(text);
							if (num6 == 0.0)
							{
								if (text[0] == '-')
								{
									num6 = -num6;
								}
							}
							result = num6;
						}
						catch (OverflowException)
						{
							if (text[0] == '-')
							{
								result = double.NegativeInfinity;
							}
							else
							{
								result = double.PositiveInfinity;
							}
						}
						catch (Exception)
						{
							result = double.NaN;
						}
					}
					return result;
				}
				num++;
			}
			result = 0.0;
			return result;
		}
		public static char ToUint16(object val)
		{
			double num = ScriptConvert.ToNumber(val);
			int num2 = (int)num;
			char result;
			if ((double)num2 == num)
			{
				result = (char)num2;
			}
			else
			{
				if (double.IsNaN(num) || num == double.PositiveInfinity || num == double.NegativeInfinity)
				{
					result = '\0';
				}
				else
				{
					num = ((num >= 0.0) ? Math.Floor(num) : Math.Ceiling(num));
					int num3 = 65536;
					num2 = (int)Math.IEEERemainder(num, (double)num3);
					result = (char)num2;
				}
			}
			return result;
		}
		public static string ToString(double val)
		{
			return ScriptConvert.ToString(val, 10);
		}
		public static string ToString(double d, int toBase)
		{
			string result;
			if (double.IsNaN(d))
			{
				result = "NaN";
			}
			else
			{
				if (d == double.PositiveInfinity)
				{
					result = "Infinity";
				}
				else
				{
					if (d == double.NegativeInfinity)
					{
						result = "-Infinity";
					}
					else
					{
						if (d == 0.0)
						{
							result = "0";
						}
						else
						{
							if (toBase < 2 || toBase > 36)
							{
								throw Context.ReportRuntimeErrorById("msg.bad.radix", new object[]
								{
									Convert.ToString(toBase)
								});
							}
							if (double.IsNaN(d))
							{
								result = "NaN";
							}
							else
							{
								if (double.IsPositiveInfinity(d))
								{
									result = "Infinity";
								}
								else
								{
									if (double.IsNegativeInfinity(d))
									{
										result = "-Infinity";
									}
									else
									{
										string text = d.ToString("g");
										result = text;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}
	}
}
