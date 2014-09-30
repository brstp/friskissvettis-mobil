using System;
using System.Globalization;
using System.Text;
namespace EcmaScript.NET.Types
{
	internal sealed class BuiltinDate : IdScriptableObject
	{
		private const string js_NaN_date_str = "Invalid Date";
		private const double HalfTimeDomain = 8.64E+15;
		private const double HoursPerDay = 24.0;
		private const double MinutesPerHour = 60.0;
		private const double SecondsPerMinute = 60.0;
		private const double msPerSecond = 1000.0;
		private const int MAXARGS = 7;
		private const int ConstructorId_now = -3;
		private const int ConstructorId_parse = -2;
		private const int ConstructorId_UTC = -1;
		private const int Id_constructor = 1;
		private const int Id_toString = 2;
		private const int Id_toTimeString = 3;
		private const int Id_toDateString = 4;
		private const int Id_toLocaleString = 5;
		private const int Id_toLocaleTimeString = 6;
		private const int Id_toLocaleDateString = 7;
		private const int Id_toUTCString = 8;
		private const int Id_toGMTString = 8;
		private const int Id_toSource = 9;
		private const int Id_valueOf = 10;
		private const int Id_getTime = 11;
		private const int Id_getYear = 12;
		private const int Id_getFullYear = 13;
		private const int Id_getUTCFullYear = 14;
		private const int Id_getMonth = 15;
		private const int Id_getUTCMonth = 16;
		private const int Id_getDate = 17;
		private const int Id_getUTCDate = 18;
		private const int Id_getDay = 19;
		private const int Id_getUTCDay = 20;
		private const int Id_getHours = 21;
		private const int Id_getUTCHours = 22;
		private const int Id_getMinutes = 23;
		private const int Id_getUTCMinutes = 24;
		private const int Id_getSeconds = 25;
		private const int Id_getUTCSeconds = 26;
		private const int Id_getMilliseconds = 27;
		private const int Id_getUTCMilliseconds = 28;
		private const int Id_getTimezoneOffset = 29;
		private const int Id_setTime = 30;
		private const int Id_setMilliseconds = 31;
		private const int Id_setUTCMilliseconds = 32;
		private const int Id_setSeconds = 33;
		private const int Id_setUTCSeconds = 34;
		private const int Id_setMinutes = 35;
		private const int Id_setUTCMinutes = 36;
		private const int Id_setHours = 37;
		private const int Id_setUTCHours = 38;
		private const int Id_setDate = 39;
		private const int Id_setUTCDate = 40;
		private const int Id_setMonth = 41;
		private const int Id_setUTCMonth = 42;
		private const int Id_setFullYear = 43;
		private const int Id_setUTCFullYear = 44;
		private const int Id_setYear = 45;
		private const int MAX_PROTOTYPE_ID = 45;
		private static readonly object DATE_TAG = new object();
		private static readonly double MinutesPerDay = 1440.0;
		private static readonly double SecondsPerDay = BuiltinDate.MinutesPerDay * 60.0;
		private static readonly double SecondsPerHour = 3600.0;
		private static readonly double msPerDay = BuiltinDate.SecondsPerDay * 1000.0;
		private static readonly double msPerHour = BuiltinDate.SecondsPerHour * 1000.0;
		private static readonly double msPerMinute = 60000.0;
		private static TimeZone thisTimeZone;
		private static double LocalTZA;
		private double date;
		private static readonly DateTime StandardBaseTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
		public override string ClassName
		{
			get
			{
				return "Date";
			}
		}
		internal double JSTimeValue
		{
			get
			{
				return this.date;
			}
		}
		internal static void Init(IScriptable scope, bool zealed)
		{
			new BuiltinDate
			{
				date = double.NaN
			}.ExportAsJSClass(45, scope, zealed, 7);
		}
		private BuiltinDate()
		{
			if (BuiltinDate.thisTimeZone == null)
			{
				BuiltinDate.thisTimeZone = TimeZone.CurrentTimeZone;
				BuiltinDate.LocalTZA = 1.0 * BuiltinDate.msPerHour;
			}
		}
		public override object GetDefaultValue(Type typeHint)
		{
			if (typeHint == null)
			{
				typeHint = typeof(string);
			}
			return base.GetDefaultValue(typeHint);
		}
		protected internal override void FillConstructorProperties(IdFunctionObject ctor)
		{
			this.AddIdFunctionProperty(ctor, BuiltinDate.DATE_TAG, -3, "now", 0);
			this.AddIdFunctionProperty(ctor, BuiltinDate.DATE_TAG, -2, "parse", 1);
			this.AddIdFunctionProperty(ctor, BuiltinDate.DATE_TAG, -1, "UTC", 1);
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
				arity = 0;
				name = "toString";
				break;
			case 3:
				arity = 0;
				name = "toTimeString";
				break;
			case 4:
				arity = 0;
				name = "toDateString";
				break;
			case 5:
				arity = 0;
				name = "toLocaleString";
				break;
			case 6:
				arity = 0;
				name = "toLocaleTimeString";
				break;
			case 7:
				arity = 0;
				name = "toLocaleDateString";
				break;
			case 8:
				arity = 0;
				name = "toUTCString";
				break;
			case 9:
				arity = 0;
				name = "toSource";
				break;
			case 10:
				arity = 0;
				name = "valueOf";
				break;
			case 11:
				arity = 0;
				name = "getTime";
				break;
			case 12:
				arity = 0;
				name = "getYear";
				break;
			case 13:
				arity = 0;
				name = "getFullYear";
				break;
			case 14:
				arity = 0;
				name = "getUTCFullYear";
				break;
			case 15:
				arity = 0;
				name = "getMonth";
				break;
			case 16:
				arity = 0;
				name = "getUTCMonth";
				break;
			case 17:
				arity = 0;
				name = "getDate";
				break;
			case 18:
				arity = 0;
				name = "getUTCDate";
				break;
			case 19:
				arity = 0;
				name = "getDay";
				break;
			case 20:
				arity = 0;
				name = "getUTCDay";
				break;
			case 21:
				arity = 0;
				name = "getHours";
				break;
			case 22:
				arity = 0;
				name = "getUTCHours";
				break;
			case 23:
				arity = 0;
				name = "getMinutes";
				break;
			case 24:
				arity = 0;
				name = "getUTCMinutes";
				break;
			case 25:
				arity = 0;
				name = "getSeconds";
				break;
			case 26:
				arity = 0;
				name = "getUTCSeconds";
				break;
			case 27:
				arity = 0;
				name = "getMilliseconds";
				break;
			case 28:
				arity = 0;
				name = "getUTCMilliseconds";
				break;
			case 29:
				arity = 0;
				name = "getTimezoneOffset";
				break;
			case 30:
				arity = 1;
				name = "setTime";
				break;
			case 31:
				arity = 1;
				name = "setMilliseconds";
				break;
			case 32:
				arity = 1;
				name = "setUTCMilliseconds";
				break;
			case 33:
				arity = 2;
				name = "setSeconds";
				break;
			case 34:
				arity = 2;
				name = "setUTCSeconds";
				break;
			case 35:
				arity = 3;
				name = "setMinutes";
				break;
			case 36:
				arity = 3;
				name = "setUTCMinutes";
				break;
			case 37:
				arity = 4;
				name = "setHours";
				break;
			case 38:
				arity = 4;
				name = "setUTCHours";
				break;
			case 39:
				arity = 1;
				name = "setDate";
				break;
			case 40:
				arity = 1;
				name = "setUTCDate";
				break;
			case 41:
				arity = 2;
				name = "setMonth";
				break;
			case 42:
				arity = 2;
				name = "setUTCMonth";
				break;
			case 43:
				arity = 3;
				name = "setFullYear";
				break;
			case 44:
				arity = 3;
				name = "setUTCFullYear";
				break;
			case 45:
				arity = 1;
				name = "setYear";
				break;
			default:
				throw new ArgumentException(Convert.ToString(id));
			}
			base.InitPrototypeMethod(BuiltinDate.DATE_TAG, id, name, arity);
		}
		public override object ExecIdCall(IdFunctionObject f, Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			object result;
			if (!f.HasTag(BuiltinDate.DATE_TAG))
			{
				result = base.ExecIdCall(f, cx, scope, thisObj, args);
			}
			else
			{
				int methodId = f.MethodId;
				switch (methodId)
				{
				case -3:
					result = BuiltinDate.now();
					return result;
				case -2:
				{
					string s = ScriptConvert.ToString(args, 0);
					result = BuiltinDate.date_parseString(s);
					return result;
				}
				case -1:
					result = BuiltinDate.jsStaticFunction_UTC(args);
					return result;
				case 1:
					if (thisObj != null)
					{
						result = BuiltinDate.date_format(BuiltinDate.now(), 2);
						return result;
					}
					result = BuiltinDate.jsConstructor(args);
					return result;
				}
				if (!(thisObj is BuiltinDate))
				{
					throw IdScriptableObject.IncompatibleCallError(f);
				}
				BuiltinDate builtinDate = (BuiltinDate)thisObj;
				double num = builtinDate.date;
				switch (methodId)
				{
				case 2:
				case 3:
				case 4:
					if (!double.IsNaN(num))
					{
						result = BuiltinDate.date_format(num, methodId);
					}
					else
					{
						result = "Invalid Date";
					}
					break;
				case 5:
				case 6:
				case 7:
					if (!double.IsNaN(num))
					{
						result = BuiltinDate.toLocale_helper(num, methodId);
					}
					else
					{
						result = "Invalid Date";
					}
					break;
				case 8:
					if (!double.IsNaN(num))
					{
						result = BuiltinDate.js_toUTCString(num);
					}
					else
					{
						result = "Invalid Date";
					}
					break;
				case 9:
					result = "(new Date(" + ScriptConvert.ToString(num) + "))";
					break;
				case 10:
				case 11:
					result = num;
					break;
				case 12:
				case 13:
				case 14:
					if (!double.IsNaN(num))
					{
						if (methodId != 14)
						{
							num = BuiltinDate.LocalTime(num);
						}
						num = (double)BuiltinDate.YearFromTime(num);
						if (methodId == 12)
						{
							if (cx.HasFeature(Context.Features.NonEcmaGetYear))
							{
								if (1900.0 <= num && num < 2000.0)
								{
									num -= 1900.0;
								}
							}
							else
							{
								num -= 1900.0;
							}
						}
					}
					result = num;
					break;
				case 15:
				case 16:
					if (!double.IsNaN(num))
					{
						if (methodId == 15)
						{
							num = BuiltinDate.LocalTime(num);
						}
						num = (double)BuiltinDate.MonthFromTime(num);
					}
					result = num;
					break;
				case 17:
				case 18:
					if (!double.IsNaN(num))
					{
						if (methodId == 17)
						{
							num = BuiltinDate.LocalTime(num);
						}
						num = (double)BuiltinDate.DateFromTime(num);
					}
					result = num;
					break;
				case 19:
				case 20:
					if (!double.IsNaN(num))
					{
						if (methodId == 19)
						{
							num = BuiltinDate.LocalTime(num);
						}
						num = (double)BuiltinDate.WeekDay(num);
					}
					result = num;
					break;
				case 21:
				case 22:
					if (!double.IsNaN(num))
					{
						if (methodId == 21)
						{
							num = BuiltinDate.LocalTime(num);
						}
						num = (double)BuiltinDate.HourFromTime(num);
					}
					result = num;
					break;
				case 23:
				case 24:
					if (!double.IsNaN(num))
					{
						if (methodId == 23)
						{
							num = BuiltinDate.LocalTime(num);
						}
						num = (double)BuiltinDate.MinFromTime(num);
					}
					result = num;
					break;
				case 25:
				case 26:
					if (!double.IsNaN(num))
					{
						if (methodId == 25)
						{
							num = BuiltinDate.LocalTime(num);
						}
						num = (double)BuiltinDate.SecFromTime(num);
					}
					result = num;
					break;
				case 27:
				case 28:
					if (!double.IsNaN(num))
					{
						if (methodId == 27)
						{
							num = BuiltinDate.LocalTime(num);
						}
						num = (double)BuiltinDate.msFromTime(num);
					}
					result = num;
					break;
				case 29:
					if (!double.IsNaN(num))
					{
						num = (num - BuiltinDate.LocalTime(num)) / BuiltinDate.msPerMinute;
					}
					result = num;
					break;
				case 30:
					num = BuiltinDate.TimeClip(ScriptConvert.ToNumber(args, 0));
					builtinDate.date = num;
					result = num;
					break;
				case 31:
				case 32:
				case 33:
				case 34:
				case 35:
				case 36:
				case 37:
				case 38:
					num = BuiltinDate.makeTime(num, args, methodId);
					builtinDate.date = num;
					result = num;
					break;
				case 39:
				case 40:
				case 41:
				case 42:
				case 43:
				case 44:
					num = BuiltinDate.makeDate(num, args, methodId);
					builtinDate.date = num;
					result = num;
					break;
				case 45:
				{
					double num2 = ScriptConvert.ToNumber(args, 0);
					if (double.IsNaN(num2) || double.IsInfinity(num2))
					{
						num = double.NaN;
					}
					else
					{
						if (double.IsNaN(num))
						{
							num = 0.0;
						}
						else
						{
							num = BuiltinDate.LocalTime(num);
						}
						if (num2 >= 0.0 && num2 <= 99.0)
						{
							num2 += 1900.0;
						}
						double day = BuiltinDate.MakeDay(num2, (double)BuiltinDate.MonthFromTime(num), (double)BuiltinDate.DateFromTime(num));
						num = BuiltinDate.MakeDate(day, BuiltinDate.TimeWithinDay(num));
						num = BuiltinDate.internalUTC(num);
						num = BuiltinDate.TimeClip(num);
					}
					builtinDate.date = num;
					result = num;
					break;
				}
				default:
					throw new ArgumentException(Convert.ToString(methodId));
				}
			}
			return result;
		}
		private static double Day(double t)
		{
			return Math.Floor(t / BuiltinDate.msPerDay);
		}
		private static double TimeWithinDay(double t)
		{
			double num = t % BuiltinDate.msPerDay;
			if (num < 0.0)
			{
				num += BuiltinDate.msPerDay;
			}
			return num;
		}
		private static bool IsLeapYear(int year)
		{
			return year % 4 == 0 && (year % 100 != 0 || year % 400 == 0);
		}
		private static double DayFromYear(double y)
		{
			return 365.0 * (y - 1970.0) + Math.Floor((y - 1969.0) / 4.0) - Math.Floor((y - 1901.0) / 100.0) + Math.Floor((y - 1601.0) / 400.0);
		}
		private static double TimeFromYear(double y)
		{
			return BuiltinDate.DayFromYear(y) * BuiltinDate.msPerDay;
		}
		private static int YearFromTime(double t)
		{
			int num = (int)Math.Floor(t / BuiltinDate.msPerDay / 366.0) + 1970;
			int i = (int)Math.Floor(t / BuiltinDate.msPerDay / 365.0) + 1970;
			if (i < num)
			{
				int num2 = num;
				num = i;
				i = num2;
			}
			int result;
			while (i > num)
			{
				int num3 = (i + num) / 2;
				if (BuiltinDate.TimeFromYear((double)num3) > t)
				{
					i = num3 - 1;
				}
				else
				{
					num = num3 + 1;
					if (BuiltinDate.TimeFromYear((double)num) > t)
					{
						result = num3;
						return result;
					}
				}
			}
			result = num;
			return result;
		}
		private static bool InLeapYear(double t)
		{
			return BuiltinDate.IsLeapYear(BuiltinDate.YearFromTime(t));
		}
		private static double DayFromMonth(int m, int year)
		{
			int num = m * 30;
			if (m >= 7)
			{
				num += m / 2 - 1;
			}
			else
			{
				if (m >= 2)
				{
					num += (m - 1) / 2 - 1;
				}
				else
				{
					num += m;
				}
			}
			if (m >= 2 && BuiltinDate.IsLeapYear(year))
			{
				num++;
			}
			return (double)num;
		}
		private static int MonthFromTime(double t)
		{
			int num = BuiltinDate.YearFromTime(t);
			int num2 = (int)(BuiltinDate.Day(t) - BuiltinDate.DayFromYear((double)num));
			num2 -= 59;
			int result;
			if (num2 < 0)
			{
				result = ((num2 < -28) ? 0 : 1);
			}
			else
			{
				if (BuiltinDate.IsLeapYear(num))
				{
					if (num2 == 0)
					{
						result = 1;
						return result;
					}
					num2--;
				}
				int num3 = num2 / 30;
				int num4;
				switch (num3)
				{
				case 0:
					result = 2;
					return result;
				case 1:
					num4 = 31;
					break;
				case 2:
					num4 = 61;
					break;
				case 3:
					num4 = 92;
					break;
				case 4:
					num4 = 122;
					break;
				case 5:
					num4 = 153;
					break;
				case 6:
					num4 = 184;
					break;
				case 7:
					num4 = 214;
					break;
				case 8:
					num4 = 245;
					break;
				case 9:
					num4 = 275;
					break;
				case 10:
					result = 11;
					return result;
				default:
					throw Context.CodeBug();
				}
				result = ((num2 >= num4) ? (num3 + 2) : (num3 + 1));
			}
			return result;
		}
		private static int DateFromTime(double t)
		{
			int num = BuiltinDate.YearFromTime(t);
			int num2 = (int)(BuiltinDate.Day(t) - BuiltinDate.DayFromYear((double)num));
			num2 -= 59;
			int result;
			if (num2 < 0)
			{
				result = ((num2 < -28) ? (num2 + 31 + 28 + 1) : (num2 + 28 + 1));
			}
			else
			{
				if (BuiltinDate.IsLeapYear(num))
				{
					if (num2 == 0)
					{
						result = 29;
						return result;
					}
					num2--;
				}
				int num3;
				int num4;
				switch (num2 / 30)
				{
				case 0:
					result = num2 + 1;
					return result;
				case 1:
					num3 = 31;
					num4 = 31;
					break;
				case 2:
					num3 = 30;
					num4 = 61;
					break;
				case 3:
					num3 = 31;
					num4 = 92;
					break;
				case 4:
					num3 = 30;
					num4 = 122;
					break;
				case 5:
					num3 = 31;
					num4 = 153;
					break;
				case 6:
					num3 = 31;
					num4 = 184;
					break;
				case 7:
					num3 = 30;
					num4 = 214;
					break;
				case 8:
					num3 = 31;
					num4 = 245;
					break;
				case 9:
					num3 = 30;
					num4 = 275;
					break;
				case 10:
					result = num2 - 275 + 1;
					return result;
				default:
					throw Context.CodeBug();
				}
				num2 -= num4;
				if (num2 < 0)
				{
					num2 += num3;
				}
				result = num2 + 1;
			}
			return result;
		}
		private static int WeekDay(double t)
		{
			double num = BuiltinDate.Day(t) + 4.0;
			num %= 7.0;
			if (num < 0.0)
			{
				num += 7.0;
			}
			return (int)num;
		}
		private static double now()
		{
			return (double)((DateTime.Now.Ticks - 621355968000000000L) / 10000L);
		}
		private static double DaylightSavingTA(double t)
		{
			if (t < 0.0 || t > 2145916800000.0)
			{
				int num = BuiltinDate.EquivalentYear(BuiltinDate.YearFromTime(t));
				double day = BuiltinDate.MakeDay((double)num, (double)BuiltinDate.MonthFromTime(t), (double)BuiltinDate.DateFromTime(t));
				t = BuiltinDate.MakeDate(day, BuiltinDate.TimeWithinDay(t));
			}
			DateTime time = BuiltinDate.FromMilliseconds(t);
			double result;
			if (BuiltinDate.thisTimeZone.IsDaylightSavingTime(time))
			{
				result = BuiltinDate.msPerHour;
			}
			else
			{
				result = 0.0;
			}
			return result;
		}
		private static int EquivalentYear(int year)
		{
			int num = (int)BuiltinDate.DayFromYear((double)year) + 4;
			num %= 7;
			if (num < 0)
			{
				num += 7;
			}
			if (BuiltinDate.IsLeapYear(year))
			{
				switch (num)
				{
				case 0:
				{
					int result = 1984;
					return result;
				}
				case 1:
				{
					int result = 1996;
					return result;
				}
				case 2:
				{
					int result = 1980;
					return result;
				}
				case 3:
				{
					int result = 1992;
					return result;
				}
				case 4:
				{
					int result = 1976;
					return result;
				}
				case 5:
				{
					int result = 1988;
					return result;
				}
				case 6:
				{
					int result = 1972;
					return result;
				}
				}
			}
			else
			{
				switch (num)
				{
				case 0:
				{
					int result = 1978;
					return result;
				}
				case 1:
				{
					int result = 1973;
					return result;
				}
				case 2:
				{
					int result = 1974;
					return result;
				}
				case 3:
				{
					int result = 1975;
					return result;
				}
				case 4:
				{
					int result = 1981;
					return result;
				}
				case 5:
				{
					int result = 1971;
					return result;
				}
				case 6:
				{
					int result = 1977;
					return result;
				}
				}
			}
			throw Context.CodeBug();
		}
		private static double LocalTime(double t)
		{
			return t + BuiltinDate.LocalTZA + BuiltinDate.DaylightSavingTA(t);
		}
		private static double internalUTC(double t)
		{
			return t - BuiltinDate.LocalTZA - BuiltinDate.DaylightSavingTA(t - BuiltinDate.LocalTZA);
		}
		private static int HourFromTime(double t)
		{
			double num = Math.Floor(t / BuiltinDate.msPerHour) % 24.0;
			if (num < 0.0)
			{
				num += 24.0;
			}
			return (int)num;
		}
		private static int MinFromTime(double t)
		{
			double num = Math.Floor(t / BuiltinDate.msPerMinute) % 60.0;
			if (num < 0.0)
			{
				num += 60.0;
			}
			return (int)num;
		}
		private static int SecFromTime(double t)
		{
			double num = Math.Floor(t / 1000.0) % 60.0;
			if (num < 0.0)
			{
				num += 60.0;
			}
			return (int)num;
		}
		private static int msFromTime(double t)
		{
			double num = t % 1000.0;
			if (num < 0.0)
			{
				num += 1000.0;
			}
			return (int)num;
		}
		private static double MakeTime(double hour, double min, double sec, double ms)
		{
			return ((hour * 60.0 + min) * 60.0 + sec) * 1000.0 + ms;
		}
		private static double MakeDay(double year, double month, double date)
		{
			year += Math.Floor(month / 12.0);
			month %= 12.0;
			if (month < 0.0)
			{
				month += 12.0;
			}
			double num = Math.Floor(BuiltinDate.TimeFromYear(year) / BuiltinDate.msPerDay);
			double num2 = BuiltinDate.DayFromMonth((int)month, (int)year);
			return num + num2 + date - 1.0;
		}
		private static double MakeDate(double day, double time)
		{
			return day * BuiltinDate.msPerDay + time;
		}
		private static double TimeClip(double d)
		{
			double result;
			if (double.IsNaN(d) || d == double.PositiveInfinity || d == double.NegativeInfinity || Math.Abs(d) > 8.64E+15)
			{
				result = double.NaN;
			}
			else
			{
				if (d > 0.0)
				{
					result = Math.Floor(d + 0.0);
				}
				else
				{
					result = Math.Ceiling(d + 0.0);
				}
			}
			return result;
		}
		private static double date_msecFromDate(double year, double mon, double mday, double hour, double min, double sec, double msec)
		{
			double day = BuiltinDate.MakeDay(year, mon, mday);
			double time = BuiltinDate.MakeTime(hour, min, sec, msec);
			return BuiltinDate.MakeDate(day, time);
		}
		private static double jsStaticFunction_UTC(object[] args)
		{
			double[] array = new double[7];
			double num;
			double result;
			for (int i = 0; i < 7; i++)
			{
				if (i < args.Length)
				{
					num = ScriptConvert.ToNumber(args[i]);
					if (double.IsNaN(num) || double.IsInfinity(num))
					{
						result = double.NaN;
						return result;
					}
					array[i] = ScriptConvert.ToInteger(args[i]);
				}
				else
				{
					array[i] = 0.0;
				}
			}
			if (array[0] >= 0.0 && array[0] <= 99.0)
			{
				array[0] += 1900.0;
			}
			if (array[2] < 1.0)
			{
				array[2] = 1.0;
			}
			num = BuiltinDate.date_msecFromDate(array[0], array[1], array[2], array[3], array[4], array[5], array[6]);
			num = BuiltinDate.TimeClip(num);
			result = num;
			return result;
		}
		public static double date_parseString(string s)
		{
			int num = -1;
			int num2 = -1;
			int num3 = -1;
			int num4 = -1;
			int num5 = -1;
			int num6 = -1;
			int i = 0;
			double num7 = -1.0;
			char c = '\0';
			bool flag = false;
			int length = s.Length;
			double result;
			while (i < length)
			{
				char c2 = s[i];
				i++;
				if (c2 <= ' ' || c2 == ',' || c2 == '-')
				{
					if (i < length)
					{
						char c3 = s[i];
						if (c2 == '-' && '0' <= c3 && c3 <= '9')
						{
							c = c2;
						}
					}
				}
				else
				{
					if (c2 != '(')
					{
						if ('0' <= c2 && c2 <= '9')
						{
							int num8 = (int)(c2 - '0');
							while (i < length && '0' <= (c2 = s[i]) && c2 <= '9')
							{
								num8 = num8 * 10 + (int)c2 - 48;
								i++;
							}
							if (c == '+' || c == '-')
							{
								flag = true;
								if (num8 < 24)
								{
									num8 *= 60;
								}
								else
								{
									num8 = num8 % 100 + num8 / 100 * 60;
								}
								if (c == '+')
								{
									num8 = -num8;
								}
								if (num7 != 0.0 && num7 != -1.0)
								{
									result = double.NaN;
									return result;
								}
								num7 = (double)num8;
							}
							else
							{
								if (num8 >= 70 || (c == '/' && num2 >= 0 && num3 >= 0 && num < 0))
								{
									if (num3 < 0)
									{
										num3 = num8;
									}
									else
									{
										if (num >= 0)
										{
											result = double.NaN;
											return result;
										}
										if (c2 > ' ' && c2 != ',' && c2 != '/' && i < length)
										{
											result = double.NaN;
											return result;
										}
										num = ((num8 < 100) ? (num8 + 1900) : num8);
									}
								}
								else
								{
									if (c2 == ':')
									{
										if (num4 < 0)
										{
											num4 = num8;
										}
										else
										{
											if (num5 >= 0)
											{
												result = double.NaN;
												return result;
											}
											num5 = num8;
										}
									}
									else
									{
										if (c2 == '/')
										{
											if (num2 < 0)
											{
												num2 = num8 - 1;
											}
											else
											{
												if (num3 >= 0)
												{
													result = double.NaN;
													return result;
												}
												num3 = num8;
											}
										}
										else
										{
											if (i < length && c2 != ',' && c2 > ' ' && c2 != '-')
											{
												result = double.NaN;
												return result;
											}
											if (flag && num8 < 60)
											{
												if (num7 < 0.0)
												{
													num7 -= (double)num8;
												}
												else
												{
													num7 += (double)num8;
												}
											}
											else
											{
												if (num4 >= 0 && num5 < 0)
												{
													num5 = num8;
												}
												else
												{
													if (num5 >= 0 && num6 < 0)
													{
														num6 = num8;
													}
													else
													{
														if (num3 >= 0)
														{
															result = double.NaN;
															return result;
														}
														num3 = num8;
													}
												}
											}
										}
									}
								}
							}
							c = '\0';
						}
						else
						{
							if (c2 == '/' || c2 == ':' || c2 == '+' || c2 == '-')
							{
								c = c2;
							}
							else
							{
								int num9 = i - 1;
								while (i < length)
								{
									c2 = s[i];
									if (('A' > c2 || c2 > 'Z') && ('a' > c2 || c2 > 'z'))
									{
										break;
									}
									i++;
								}
								int num10 = i - num9;
								if (num10 < 2)
								{
									result = double.NaN;
									return result;
								}
								string text = "am;pm;monday;tuesday;wednesday;thursday;friday;saturday;sunday;january;february;march;april;may;june;july;august;september;october;november;december;gmt;ut;utc;est;edt;cst;cdt;mst;mdt;pst;pdt;";
								int num11 = 0;
								int num12 = 0;
								while (true)
								{
									int num13 = text.IndexOf(';', num12);
									if (num13 < 0)
									{
										goto Block_61;
									}
									if (string.Compare(text, num12, s, num9, num10, true) == 0)
									{
										break;
									}
									num12 = num13 + 1;
									num11++;
								}
								if (num11 < 2)
								{
									if (num4 > 12 || num4 < 0)
									{
										result = double.NaN;
										return result;
									}
									if (num11 == 0)
									{
										if (num4 == 12)
										{
											num4 = 0;
										}
									}
									else
									{
										if (num4 != 12)
										{
											num4 += 12;
										}
									}
								}
								else
								{
									if ((num11 -= 2) >= 7)
									{
										if ((num11 -= 7) < 12)
										{
											if (num2 >= 0)
											{
												result = double.NaN;
												return result;
											}
											num2 = num11;
										}
										else
										{
											switch (num11)
											{
											case 12:
												num7 = 0.0;
												break;
											case 13:
												num7 = 0.0;
												break;
											case 14:
												num7 = 0.0;
												break;
											case 15:
												num7 = 300.0;
												break;
											case 16:
												num7 = 240.0;
												break;
											case 17:
												num7 = 360.0;
												break;
											case 18:
												num7 = 300.0;
												break;
											case 19:
												num7 = 420.0;
												break;
											case 20:
												num7 = 360.0;
												break;
											case 21:
												num7 = 480.0;
												break;
											case 22:
												num7 = 420.0;
												break;
											default:
												Context.CodeBug();
												break;
											}
										}
									}
								}
								continue;
								Block_61:
								result = double.NaN;
								return result;
							}
						}
						continue;
					}
					int num14 = 1;
					while (i < length)
					{
						c2 = s[i];
						i++;
						if (c2 == '(')
						{
							num14++;
						}
						else
						{
							if (c2 == ')' && --num14 <= 0)
							{
								break;
							}
						}
					}
				}
			}
			if (num < 0 || num2 < 0 || num3 < 0)
			{
				result = double.NaN;
				return result;
			}
			if (num6 < 0)
			{
				num6 = 0;
			}
			if (num5 < 0)
			{
				num5 = 0;
			}
			if (num4 < 0)
			{
				num4 = 0;
			}
			double num15 = BuiltinDate.date_msecFromDate((double)num, (double)num2, (double)num3, (double)num4, (double)num5, (double)num6, 0.0);
			if (num7 == -1.0)
			{
				result = BuiltinDate.internalUTC(num15);
				return result;
			}
			result = num15 + num7 * BuiltinDate.msPerMinute;
			return result;
		}
		private static string date_format(double t, int methodId)
		{
			StringBuilder stringBuilder = new StringBuilder(60);
			double t2 = BuiltinDate.LocalTime(t);
			if (methodId != 3)
			{
				BuiltinDate.appendWeekDayName(stringBuilder, BuiltinDate.WeekDay(t2));
				stringBuilder.Append(' ');
				BuiltinDate.appendMonthName(stringBuilder, BuiltinDate.MonthFromTime(t2));
				stringBuilder.Append(' ');
				BuiltinDate.append0PaddedUint(stringBuilder, BuiltinDate.DateFromTime(t2), 2);
				stringBuilder.Append(' ');
				int num = BuiltinDate.YearFromTime(t2);
				if (num < 0)
				{
					stringBuilder.Append('-');
					num = -num;
				}
				BuiltinDate.append0PaddedUint(stringBuilder, num, 4);
				if (methodId != 4)
				{
					stringBuilder.Append(' ');
				}
			}
			if (methodId != 4)
			{
				BuiltinDate.append0PaddedUint(stringBuilder, BuiltinDate.HourFromTime(t2), 2);
				stringBuilder.Append(':');
				BuiltinDate.append0PaddedUint(stringBuilder, BuiltinDate.MinFromTime(t2), 2);
				stringBuilder.Append(':');
				BuiltinDate.append0PaddedUint(stringBuilder, BuiltinDate.SecFromTime(t2), 2);
				int num2 = (int)Math.Floor((BuiltinDate.LocalTZA + BuiltinDate.DaylightSavingTA(t)) / BuiltinDate.msPerMinute);
				int num3 = num2 / 60 * 100 + num2 % 60;
				if (num3 > 0)
				{
					stringBuilder.Append(" GMT+");
				}
				else
				{
					stringBuilder.Append(" GMT-");
					num3 = -num3;
				}
				BuiltinDate.append0PaddedUint(stringBuilder, num3, 4);
				if (t < 0.0 || t > 2145916800000.0)
				{
					int num4 = BuiltinDate.EquivalentYear(BuiltinDate.YearFromTime(t2));
					double day = BuiltinDate.MakeDay((double)num4, (double)BuiltinDate.MonthFromTime(t), (double)BuiltinDate.DateFromTime(t));
					t = BuiltinDate.MakeDate(day, BuiltinDate.TimeWithinDay(t));
				}
				stringBuilder.Append(" (");
				stringBuilder.Append(BuiltinDate.FromMilliseconds(t).ToString("zzz"));
				stringBuilder.Append(')');
			}
			return stringBuilder.ToString();
		}
		private static object jsConstructor(object[] args)
		{
			BuiltinDate builtinDate = new BuiltinDate();
			object result;
			if (args.Length == 0)
			{
				builtinDate.date = BuiltinDate.now();
				result = builtinDate;
			}
			else
			{
				if (args.Length == 1)
				{
					object obj = args[0];
					if (obj is IScriptable)
					{
						obj = ((IScriptable)obj).GetDefaultValue(null);
					}
					double d;
					if (obj is string)
					{
						d = BuiltinDate.date_parseString((string)obj);
					}
					else
					{
						d = ScriptConvert.ToNumber(obj);
					}
					builtinDate.date = BuiltinDate.TimeClip(d);
					result = builtinDate;
				}
				else
				{
					double[] array = new double[7];
					for (int i = 0; i < 7; i++)
					{
						if (i < args.Length)
						{
							double d2 = ScriptConvert.ToNumber(args[i]);
							if (double.IsNaN(d2) || double.IsInfinity(d2))
							{
								builtinDate.date = double.NaN;
								result = builtinDate;
								return result;
							}
							array[i] = ScriptConvert.ToInteger(args[i]);
						}
						else
						{
							array[i] = 0.0;
						}
					}
					if (array[0] >= 0.0 && array[0] <= 99.0)
					{
						array[0] += 1900.0;
					}
					if (array[2] < 1.0)
					{
						array[2] = 1.0;
					}
					double day = BuiltinDate.MakeDay(array[0], array[1], array[2]);
					double num = BuiltinDate.MakeTime(array[3], array[4], array[5], array[6]);
					num = BuiltinDate.MakeDate(day, num);
					num = BuiltinDate.internalUTC(num);
					builtinDate.date = BuiltinDate.TimeClip(num);
					result = builtinDate;
				}
			}
			return result;
		}
		private static string toLocale_helper(double t, int methodId)
		{
			CultureInfo currentCulture = Context.CurrentContext.CurrentCulture;
			string result;
			switch (methodId)
			{
			case 5:
			{
				DateTime dateTime = BuiltinDate.FromMilliseconds(t);
				result = dateTime.ToString(currentCulture.DateTimeFormat.LongDatePattern) + " " + dateTime.ToString(currentCulture.DateTimeFormat.LongTimePattern);
				break;
			}
			case 6:
				result = BuiltinDate.FromMilliseconds(t).ToString(currentCulture.DateTimeFormat.LongTimePattern);
				break;
			case 7:
				result = BuiltinDate.FromMilliseconds(t).ToString(currentCulture.DateTimeFormat.LongDatePattern);
				break;
			default:
				throw Context.CodeBug();
			}
			return result;
		}
		private static string js_toUTCString(double date)
		{
			StringBuilder stringBuilder = new StringBuilder(60);
			BuiltinDate.appendWeekDayName(stringBuilder, BuiltinDate.WeekDay(date));
			stringBuilder.Append(", ");
			BuiltinDate.append0PaddedUint(stringBuilder, BuiltinDate.DateFromTime(date), 2);
			stringBuilder.Append(' ');
			BuiltinDate.appendMonthName(stringBuilder, BuiltinDate.MonthFromTime(date));
			stringBuilder.Append(' ');
			int num = BuiltinDate.YearFromTime(date);
			if (num < 0)
			{
				stringBuilder.Append('-');
				num = -num;
			}
			BuiltinDate.append0PaddedUint(stringBuilder, num, 4);
			stringBuilder.Append(' ');
			BuiltinDate.append0PaddedUint(stringBuilder, BuiltinDate.HourFromTime(date), 2);
			stringBuilder.Append(':');
			BuiltinDate.append0PaddedUint(stringBuilder, BuiltinDate.MinFromTime(date), 2);
			stringBuilder.Append(':');
			BuiltinDate.append0PaddedUint(stringBuilder, BuiltinDate.SecFromTime(date), 2);
			stringBuilder.Append(" GMT");
			return stringBuilder.ToString();
		}
		private static void append0PaddedUint(StringBuilder sb, int i, int minWidth)
		{
			if (i < 0)
			{
				Context.CodeBug();
			}
			int num = 1;
			minWidth--;
			if (i >= 10)
			{
				if (i < 1000000000)
				{
					while (true)
					{
						int num2 = num * 10;
						if (i < num2)
						{
							break;
						}
						minWidth--;
						num = num2;
					}
				}
				else
				{
					minWidth -= 9;
					num = 1000000000;
				}
			}
			while (minWidth > 0)
			{
				sb.Append('0');
				minWidth--;
			}
			while (num != 1)
			{
				sb.Append((char)(48 + i / num));
				i %= num;
				num /= 10;
			}
			sb.Append((char)(48 + i));
		}
		private static void appendMonthName(StringBuilder sb, int index)
		{
			string text = "JanFebMarAprMayJunJulAugSepOctNovDec";
			index *= 3;
			for (int num = 0; num != 3; num++)
			{
				sb.Append(text[index + num]);
			}
		}
		private static void appendWeekDayName(StringBuilder sb, int index)
		{
			string text = "SunMonTueWedThuFriSat";
			index *= 3;
			for (int num = 0; num != 3; num++)
			{
				sb.Append(text[index + num]);
			}
		}
		private static double makeTime(double date, object[] args, int methodId)
		{
			bool flag = true;
			int num;
			switch (methodId)
			{
			case 31:
				break;
			case 32:
				flag = false;
				break;
			case 33:
				goto IL_48;
			case 34:
				flag = false;
				goto IL_48;
			case 35:
				goto IL_56;
			case 36:
				flag = false;
				goto IL_56;
			case 37:
				goto IL_64;
			case 38:
				flag = false;
				goto IL_64;
			default:
				Context.CodeBug();
				num = 0;
				goto IL_78;
			}
			num = 1;
			goto IL_78;
			IL_48:
			num = 2;
			goto IL_78;
			IL_56:
			num = 3;
			goto IL_78;
			IL_64:
			num = 4;
			IL_78:
			double[] array = new double[4];
			double result;
			if (double.IsNaN(date))
			{
				result = date;
			}
			else
			{
				if (args.Length == 0)
				{
					args = ScriptRuntime.padArguments(args, 1);
				}
				int num2 = 0;
				while (num2 < args.Length && num2 < num)
				{
					array[num2] = ScriptConvert.ToNumber(args[num2]);
					if (array[num2] != array[num2] || double.IsInfinity(array[num2]))
					{
						result = double.NaN;
						return result;
					}
					array[num2] = ScriptConvert.ToInteger(array[num2]);
					num2++;
				}
				double t;
				if (flag)
				{
					t = BuiltinDate.LocalTime(date);
				}
				else
				{
					t = date;
				}
				num2 = 0;
				int num3 = args.Length;
				double hour;
				if (num >= 4 && num2 < num3)
				{
					hour = array[num2++];
				}
				else
				{
					hour = (double)BuiltinDate.HourFromTime(t);
				}
				double min;
				if (num >= 3 && num2 < num3)
				{
					min = array[num2++];
				}
				else
				{
					min = (double)BuiltinDate.MinFromTime(t);
				}
				double sec;
				if (num >= 2 && num2 < num3)
				{
					sec = array[num2++];
				}
				else
				{
					sec = (double)BuiltinDate.SecFromTime(t);
				}
				double ms;
				if (num >= 1 && num2 < num3)
				{
					ms = array[num2++];
				}
				else
				{
					ms = (double)BuiltinDate.msFromTime(t);
				}
				double time = BuiltinDate.MakeTime(hour, min, sec, ms);
				double num4 = BuiltinDate.MakeDate(BuiltinDate.Day(t), time);
				if (flag)
				{
					num4 = BuiltinDate.internalUTC(num4);
				}
				date = BuiltinDate.TimeClip(num4);
				result = date;
			}
			return result;
		}
		private static double makeDate(double date, object[] args, int methodId)
		{
			bool flag = true;
			int num;
			switch (methodId)
			{
			case 39:
				break;
			case 40:
				flag = false;
				break;
			case 41:
				goto IL_40;
			case 42:
				flag = false;
				goto IL_40;
			case 43:
				goto IL_4E;
			case 44:
				flag = false;
				goto IL_4E;
			default:
				Context.CodeBug();
				num = 0;
				goto IL_62;
			}
			num = 1;
			goto IL_62;
			IL_40:
			num = 2;
			goto IL_62;
			IL_4E:
			num = 3;
			IL_62:
			double[] array = new double[3];
			if (args.Length == 0)
			{
				args = ScriptRuntime.padArguments(args, 1);
			}
			int num2 = 0;
			double result;
			while (num2 < args.Length && num2 < num)
			{
				array[num2] = ScriptConvert.ToNumber(args[num2]);
				if (array[num2] != array[num2] || double.IsInfinity(array[num2]))
				{
					result = double.NaN;
					return result;
				}
				array[num2] = ScriptConvert.ToInteger(array[num2]);
				num2++;
			}
			double t;
			if (double.IsNaN(date))
			{
				if (args.Length < 3)
				{
					result = double.NaN;
					return result;
				}
				t = 0.0;
			}
			else
			{
				if (flag)
				{
					t = BuiltinDate.LocalTime(date);
				}
				else
				{
					t = date;
				}
			}
			num2 = 0;
			int num3 = args.Length;
			double year;
			if (num >= 3 && num2 < num3)
			{
				year = array[num2++];
			}
			else
			{
				year = (double)BuiltinDate.YearFromTime(t);
			}
			double month;
			if (num >= 2 && num2 < num3)
			{
				month = array[num2++];
			}
			else
			{
				month = (double)BuiltinDate.MonthFromTime(t);
			}
			double day;
			if (num >= 1 && num2 < num3)
			{
				day = array[num2++];
			}
			else
			{
				day = (double)BuiltinDate.DateFromTime(t);
			}
			day = BuiltinDate.MakeDay(year, month, day);
			double num4 = BuiltinDate.MakeDate(day, BuiltinDate.TimeWithinDay(t));
			if (flag)
			{
				num4 = BuiltinDate.internalUTC(num4);
			}
			date = BuiltinDate.TimeClip(num4);
			result = date;
			return result;
		}
		protected internal override int FindPrototypeId(string s)
		{
			int result = 0;
			string text = null;
			switch (s.Length)
			{
			case 6:
				text = "getDay";
				result = 19;
				break;
			case 7:
			{
				char c = s[3];
				if (c <= 'T')
				{
					if (c != 'D')
					{
						if (c == 'T')
						{
							int num = (int)s[0];
							if (num == 103)
							{
								text = "getTime";
								result = 11;
							}
							else
							{
								if (num == 115)
								{
									text = "setTime";
									result = 30;
								}
							}
						}
					}
					else
					{
						int num = (int)s[0];
						if (num == 103)
						{
							text = "getDate";
							result = 17;
						}
						else
						{
							if (num == 115)
							{
								text = "setDate";
								result = 39;
							}
						}
					}
				}
				else
				{
					if (c != 'Y')
					{
						if (c == 'u')
						{
							text = "valueOf";
							result = 10;
						}
					}
					else
					{
						int num = (int)s[0];
						if (num == 103)
						{
							text = "getYear";
							result = 12;
						}
						else
						{
							if (num == 115)
							{
								text = "setYear";
								result = 45;
							}
						}
					}
				}
				break;
			}
			case 8:
			{
				char c = s[3];
				if (c <= 'M')
				{
					if (c != 'H')
					{
						if (c == 'M')
						{
							int num = (int)s[0];
							if (num == 103)
							{
								text = "getMonth";
								result = 15;
							}
							else
							{
								if (num == 115)
								{
									text = "setMonth";
									result = 41;
								}
							}
						}
					}
					else
					{
						int num = (int)s[0];
						if (num == 103)
						{
							text = "getHours";
							result = 21;
						}
						else
						{
							if (num == 115)
							{
								text = "setHours";
								result = 37;
							}
						}
					}
				}
				else
				{
					if (c != 'o')
					{
						if (c == 't')
						{
							text = "toString";
							result = 2;
						}
					}
					else
					{
						text = "toSource";
						result = 9;
					}
				}
				break;
			}
			case 9:
				text = "getUTCDay";
				result = 20;
				break;
			case 10:
			{
				int num = (int)s[3];
				if (num == 77)
				{
					num = (int)s[0];
					if (num == 103)
					{
						text = "getMinutes";
						result = 23;
					}
					else
					{
						if (num == 115)
						{
							text = "setMinutes";
							result = 35;
						}
					}
				}
				else
				{
					if (num == 83)
					{
						num = (int)s[0];
						if (num == 103)
						{
							text = "getSeconds";
							result = 25;
						}
						else
						{
							if (num == 115)
							{
								text = "setSeconds";
								result = 33;
							}
						}
					}
					else
					{
						if (num == 85)
						{
							num = (int)s[0];
							if (num == 103)
							{
								text = "getUTCDate";
								result = 18;
							}
							else
							{
								if (num == 115)
								{
									text = "setUTCDate";
									result = 40;
								}
							}
						}
					}
				}
				break;
			}
			case 11:
			{
				char c = s[3];
				if (c <= 'M')
				{
					if (c != 'F')
					{
						if (c == 'M')
						{
							text = "toGMTString";
							result = 8;
						}
					}
					else
					{
						int num = (int)s[0];
						if (num == 103)
						{
							text = "getFullYear";
							result = 13;
						}
						else
						{
							if (num == 115)
							{
								text = "setFullYear";
								result = 43;
							}
						}
					}
				}
				else
				{
					switch (c)
					{
					case 'T':
						text = "toUTCString";
						result = 8;
						break;
					case 'U':
					{
						int num = (int)s[0];
						if (num == 103)
						{
							num = (int)s[9];
							if (num == 114)
							{
								text = "getUTCHours";
								result = 22;
							}
							else
							{
								if (num == 116)
								{
									text = "getUTCMonth";
									result = 16;
								}
							}
						}
						else
						{
							if (num == 115)
							{
								num = (int)s[9];
								if (num == 114)
								{
									text = "setUTCHours";
									result = 38;
								}
								else
								{
									if (num == 116)
									{
										text = "setUTCMonth";
										result = 42;
									}
								}
							}
						}
						break;
					}
					default:
						if (c == 's')
						{
							text = "constructor";
							result = 1;
						}
						break;
					}
				}
				break;
			}
			case 12:
			{
				int num = (int)s[2];
				if (num == 68)
				{
					text = "toDateString";
					result = 4;
				}
				else
				{
					if (num == 84)
					{
						text = "toTimeString";
						result = 3;
					}
				}
				break;
			}
			case 13:
			{
				int num = (int)s[0];
				if (num == 103)
				{
					num = (int)s[6];
					if (num == 77)
					{
						text = "getUTCMinutes";
						result = 24;
					}
					else
					{
						if (num == 83)
						{
							text = "getUTCSeconds";
							result = 26;
						}
					}
				}
				else
				{
					if (num == 115)
					{
						num = (int)s[6];
						if (num == 77)
						{
							text = "setUTCMinutes";
							result = 36;
						}
						else
						{
							if (num == 83)
							{
								text = "setUTCSeconds";
								result = 34;
							}
						}
					}
				}
				break;
			}
			case 14:
			{
				int num = (int)s[0];
				if (num == 103)
				{
					text = "getUTCFullYear";
					result = 14;
				}
				else
				{
					if (num == 115)
					{
						text = "setUTCFullYear";
						result = 44;
					}
					else
					{
						if (num == 116)
						{
							text = "toLocaleString";
							result = 5;
						}
					}
				}
				break;
			}
			case 15:
			{
				int num = (int)s[0];
				if (num == 103)
				{
					text = "getMilliseconds";
					result = 27;
				}
				else
				{
					if (num == 115)
					{
						text = "setMilliseconds";
						result = 31;
					}
				}
				break;
			}
			case 17:
				text = "getTimezoneOffset";
				result = 29;
				break;
			case 18:
			{
				int num = (int)s[0];
				if (num == 103)
				{
					text = "getUTCMilliseconds";
					result = 28;
				}
				else
				{
					if (num == 115)
					{
						text = "setUTCMilliseconds";
						result = 32;
					}
					else
					{
						if (num == 116)
						{
							num = (int)s[8];
							if (num == 68)
							{
								text = "toLocaleDateString";
								result = 7;
							}
							else
							{
								if (num == 84)
								{
									text = "toLocaleTimeString";
									result = 6;
								}
							}
						}
					}
				}
				break;
			}
			}
			if (text != null && text != s && !text.Equals(s))
			{
				result = 0;
			}
			return result;
		}
		internal static DateTime FromMilliseconds(double ms)
		{
			return BuiltinDate.StandardBaseTime.AddMilliseconds(ms);
		}
	}
}
