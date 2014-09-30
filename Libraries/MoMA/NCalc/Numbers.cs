using System;
namespace NCalc
{
	public class Numbers
	{
		private static object ConvertIfString(object s)
		{
			object result;
			if (s is string || s is char)
			{
				result = decimal.Parse(s.ToString());
			}
			else
			{
				result = s;
			}
			return result;
		}
		public static object Add(object a, object b)
		{
			a = Numbers.ConvertIfString(a);
			b = Numbers.ConvertIfString(b);
			TypeCode typeCode = Type.GetTypeCode(a.GetType());
			TypeCode typeCode2 = Type.GetTypeCode(b.GetType());
			object result;
			switch (typeCode)
			{
			case TypeCode.Boolean:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'bool' and 'bool'");
				case TypeCode.SByte:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'bool' and 'byte'");
				case TypeCode.Byte:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'bool' and 'byte'");
				case TypeCode.Int16:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'bool' and 'byte'");
				case TypeCode.UInt16:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'bool' and 'byte'");
				case TypeCode.Int32:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'bool' and 'byte'");
				case TypeCode.UInt32:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'bool' and 'byte'");
				case TypeCode.Int64:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'bool' and 'byte'");
				case TypeCode.UInt64:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'bool' and 'byte'");
				case TypeCode.Single:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'bool' and 'byte'");
				case TypeCode.Double:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'bool' and 'byte'");
				case TypeCode.Decimal:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'bool' and 'byte'");
				}
				break;
			case TypeCode.SByte:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'sbyte' and 'bool'");
				case TypeCode.SByte:
					result = (int)((sbyte)a + (sbyte)b);
					return result;
				case TypeCode.Byte:
					result = (int)((sbyte)a + (sbyte)((byte)b));
					return result;
				case TypeCode.Int16:
					result = (int)((short)((sbyte)a) + (short)b);
					return result;
				case TypeCode.UInt16:
					result = (int)((ushort)((sbyte)a) + (ushort)b);
					return result;
				case TypeCode.Int32:
					result = (int)((sbyte)a) + (int)b;
					return result;
				case TypeCode.UInt32:
					result = (long)((sbyte)a) + (long)((ulong)((uint)b));
					return result;
				case TypeCode.Int64:
					result = (long)((sbyte)a) + (long)b;
					return result;
				case TypeCode.UInt64:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'sbyte' and 'ulong'");
				case TypeCode.Single:
					result = (float)((sbyte)a) + (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((sbyte)a) + (double)b;
					return result;
				case TypeCode.Decimal:
					result = (sbyte)a + (decimal)b;
					return result;
				}
				break;
			case TypeCode.Byte:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'byte' and 'bool'");
				case TypeCode.SByte:
					result = (int)((byte)a + (byte)((sbyte)b));
					return result;
				case TypeCode.Byte:
					result = (int)((byte)a + (byte)b);
					return result;
				case TypeCode.Int16:
					result = (int)((short)((byte)a) + (short)b);
					return result;
				case TypeCode.UInt16:
					result = (int)((ushort)((byte)a) + (ushort)b);
					return result;
				case TypeCode.Int32:
					result = (int)((byte)a) + (int)b;
					return result;
				case TypeCode.UInt32:
					result = (uint)((byte)a) + (uint)b;
					return result;
				case TypeCode.Int64:
					result = (long)((ulong)((byte)a) + (ulong)((long)b));
					return result;
				case TypeCode.UInt64:
					result = (ulong)((byte)a) + (ulong)b;
					return result;
				case TypeCode.Single:
					result = (float)((byte)a) + (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((byte)a) + (double)b;
					return result;
				case TypeCode.Decimal:
					result = (byte)a + (decimal)b;
					return result;
				}
				break;
			case TypeCode.Int16:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'short' and 'bool'");
				case TypeCode.SByte:
					result = (int)((short)a + (short)((sbyte)b));
					return result;
				case TypeCode.Byte:
					result = (int)((short)a + (short)((byte)b));
					return result;
				case TypeCode.Int16:
					result = (int)((short)a + (short)b);
					return result;
				case TypeCode.UInt16:
					result = (int)((short)a + (short)((ushort)b));
					return result;
				case TypeCode.Int32:
					result = (int)((short)a) + (int)b;
					return result;
				case TypeCode.UInt32:
					result = (long)((short)a) + (long)((ulong)((uint)b));
					return result;
				case TypeCode.Int64:
					result = (long)((short)a) + (long)b;
					return result;
				case TypeCode.UInt64:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'short' and 'ulong'");
				case TypeCode.Single:
					result = (float)((short)a) + (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((short)a) + (double)b;
					return result;
				case TypeCode.Decimal:
					result = (short)a + (decimal)b;
					return result;
				}
				break;
			case TypeCode.UInt16:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'ushort' and 'bool'");
				case TypeCode.SByte:
					result = (int)((ushort)a + (ushort)((sbyte)b));
					return result;
				case TypeCode.Byte:
					result = (int)((ushort)a + (ushort)((byte)b));
					return result;
				case TypeCode.Int16:
					result = (int)((ushort)a + (ushort)((short)b));
					return result;
				case TypeCode.UInt16:
					result = (int)((ushort)a + (ushort)b);
					return result;
				case TypeCode.Int32:
					result = (int)((ushort)a) + (int)b;
					return result;
				case TypeCode.UInt32:
					result = (uint)((ushort)a) + (uint)b;
					return result;
				case TypeCode.Int64:
					result = (long)((ulong)((ushort)a) + (ulong)((long)b));
					return result;
				case TypeCode.UInt64:
					result = (ulong)((ushort)a) + (ulong)b;
					return result;
				case TypeCode.Single:
					result = (float)((ushort)a) + (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((ushort)a) + (double)b;
					return result;
				case TypeCode.Decimal:
					result = (ushort)a + (decimal)b;
					return result;
				}
				break;
			case TypeCode.Int32:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'int' and 'bool'");
				case TypeCode.SByte:
					result = (int)a + (int)((sbyte)b);
					return result;
				case TypeCode.Byte:
					result = (int)a + (int)((byte)b);
					return result;
				case TypeCode.Int16:
					result = (int)a + (int)((short)b);
					return result;
				case TypeCode.UInt16:
					result = (int)a + (int)((ushort)b);
					return result;
				case TypeCode.Int32:
					result = (int)a + (int)b;
					return result;
				case TypeCode.UInt32:
					result = (long)((int)a) + (long)((ulong)((uint)b));
					return result;
				case TypeCode.Int64:
					result = (long)((int)a) + (long)b;
					return result;
				case TypeCode.UInt64:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'int' and 'ulong'");
				case TypeCode.Single:
					result = (float)((int)a) + (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((int)a) + (double)b;
					return result;
				case TypeCode.Decimal:
					result = (int)a + (decimal)b;
					return result;
				}
				break;
			case TypeCode.UInt32:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'unit' and 'bool'");
				case TypeCode.SByte:
					result = (long)((ulong)((uint)a) + (ulong)((long)((sbyte)b)));
					return result;
				case TypeCode.Byte:
					result = (uint)a + (uint)((byte)b);
					return result;
				case TypeCode.Int16:
					result = (long)((ulong)((uint)a) + (ulong)((long)((short)b)));
					return result;
				case TypeCode.UInt16:
					result = (uint)a + (uint)((ushort)b);
					return result;
				case TypeCode.Int32:
					result = (long)((ulong)((uint)a) + (ulong)((long)((int)b)));
					return result;
				case TypeCode.UInt32:
					result = (uint)a + (uint)b;
					return result;
				case TypeCode.Int64:
					result = (long)((ulong)((uint)a) + (ulong)((long)b));
					return result;
				case TypeCode.UInt64:
					result = (ulong)((uint)a) + (ulong)b;
					return result;
				case TypeCode.Single:
					result = (uint)a + (float)b;
					return result;
				case TypeCode.Double:
					result = (uint)a + (double)b;
					return result;
				case TypeCode.Decimal:
					result = (uint)a + (decimal)b;
					return result;
				}
				break;
			case TypeCode.Int64:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'long' and 'bool'");
				case TypeCode.SByte:
					result = (long)a + (long)((sbyte)b);
					return result;
				case TypeCode.Byte:
					result = (long)a + (long)((ulong)((byte)b));
					return result;
				case TypeCode.Int16:
					result = (long)a + (long)((short)b);
					return result;
				case TypeCode.UInt16:
					result = (long)a + (long)((ulong)((ushort)b));
					return result;
				case TypeCode.Int32:
					result = (long)a + (long)((int)b);
					return result;
				case TypeCode.UInt32:
					result = (long)a + (long)((ulong)((uint)b));
					return result;
				case TypeCode.Int64:
					result = (long)a + (long)b;
					return result;
				case TypeCode.UInt64:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'long' and 'ulong'");
				case TypeCode.Single:
					result = (float)((long)a) + (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((long)a) + (double)b;
					return result;
				case TypeCode.Decimal:
					result = (long)a + (decimal)b;
					return result;
				}
				break;
			case TypeCode.UInt64:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'ulong' and 'bool'");
				case TypeCode.SByte:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'ulong' and 'sbyte'");
				case TypeCode.Byte:
					result = (ulong)a + (ulong)((byte)b);
					return result;
				case TypeCode.Int16:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'ulong' and 'short'");
				case TypeCode.UInt16:
					result = (ulong)a + (ulong)((ushort)b);
					return result;
				case TypeCode.Int32:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'ulong' and 'int'");
				case TypeCode.UInt32:
					result = (ulong)a + (ulong)((uint)b);
					return result;
				case TypeCode.Int64:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'ulong' and 'ulong'");
				case TypeCode.UInt64:
					result = (ulong)a + (ulong)b;
					return result;
				case TypeCode.Single:
					result = (ulong)a + (float)b;
					return result;
				case TypeCode.Double:
					result = (ulong)a + (double)b;
					return result;
				case TypeCode.Decimal:
					result = (ulong)a + (decimal)b;
					return result;
				}
				break;
			case TypeCode.Single:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'float' and 'bool'");
				case TypeCode.SByte:
					result = (float)a + (float)((sbyte)b);
					return result;
				case TypeCode.Byte:
					result = (float)a + (float)((byte)b);
					return result;
				case TypeCode.Int16:
					result = (float)a + (float)((short)b);
					return result;
				case TypeCode.UInt16:
					result = (float)a + (float)((ushort)b);
					return result;
				case TypeCode.Int32:
					result = (float)a + (float)((int)b);
					return result;
				case TypeCode.UInt32:
					result = (float)a + (uint)b;
					return result;
				case TypeCode.Int64:
					result = (float)a + (float)((long)b);
					return result;
				case TypeCode.UInt64:
					result = (float)a + (ulong)b;
					return result;
				case TypeCode.Single:
					result = (float)a + (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((float)a) + (double)b;
					return result;
				case TypeCode.Decimal:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'float' and 'decimal'");
				}
				break;
			case TypeCode.Double:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'double' and 'bool'");
				case TypeCode.SByte:
					result = (double)a + (double)((sbyte)b);
					return result;
				case TypeCode.Byte:
					result = (double)a + (double)((byte)b);
					return result;
				case TypeCode.Int16:
					result = (double)a + (double)((short)b);
					return result;
				case TypeCode.UInt16:
					result = (double)a + (double)((ushort)b);
					return result;
				case TypeCode.Int32:
					result = (double)a + (double)((int)b);
					return result;
				case TypeCode.UInt32:
					result = (double)a + (uint)b;
					return result;
				case TypeCode.Int64:
					result = (double)a + (double)((long)b);
					return result;
				case TypeCode.UInt64:
					result = (double)a + (ulong)b;
					return result;
				case TypeCode.Single:
					result = (double)a + (double)((float)b);
					return result;
				case TypeCode.Double:
					result = (double)a + (double)b;
					return result;
				case TypeCode.Decimal:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'double' and 'decimal'");
				}
				break;
			case TypeCode.Decimal:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'decimal' and 'bool'");
				case TypeCode.SByte:
					result = (decimal)a + (sbyte)b;
					return result;
				case TypeCode.Byte:
					result = (decimal)a + (byte)b;
					return result;
				case TypeCode.Int16:
					result = (decimal)a + (short)b;
					return result;
				case TypeCode.UInt16:
					result = (decimal)a + (ushort)b;
					return result;
				case TypeCode.Int32:
					result = (decimal)a + (int)b;
					return result;
				case TypeCode.UInt32:
					result = (decimal)a + (uint)b;
					return result;
				case TypeCode.Int64:
					result = (decimal)a + (long)b;
					return result;
				case TypeCode.UInt64:
					result = (decimal)a + (ulong)b;
					return result;
				case TypeCode.Single:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'decimal' and 'float'");
				case TypeCode.Double:
					throw new InvalidOperationException("Operator '+' can't be applied to operands of types 'decimal' and 'double'");
				case TypeCode.Decimal:
					result = (decimal)a + (decimal)b;
					return result;
				}
				break;
			}
			result = null;
			return result;
		}
		public static object Soustract(object a, object b)
		{
			a = Numbers.ConvertIfString(a);
			b = Numbers.ConvertIfString(b);
			TypeCode typeCode = Type.GetTypeCode(a.GetType());
			TypeCode typeCode2 = Type.GetTypeCode(b.GetType());
			object result;
			switch (typeCode)
			{
			case TypeCode.Boolean:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'bool' and 'bool'");
				case TypeCode.SByte:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'bool' and 'byte'");
				case TypeCode.Byte:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'bool' and 'byte'");
				case TypeCode.Int16:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'bool' and 'byte'");
				case TypeCode.UInt16:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'bool' and 'byte'");
				case TypeCode.Int32:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'bool' and 'byte'");
				case TypeCode.UInt32:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'bool' and 'byte'");
				case TypeCode.Int64:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'bool' and 'byte'");
				case TypeCode.UInt64:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'bool' and 'byte'");
				case TypeCode.Single:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'bool' and 'byte'");
				case TypeCode.Double:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'bool' and 'byte'");
				case TypeCode.Decimal:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'bool' and 'byte'");
				}
				break;
			case TypeCode.SByte:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'sbyte' and 'bool'");
				case TypeCode.SByte:
					result = (int)((sbyte)a - (sbyte)b);
					return result;
				case TypeCode.Int16:
					result = (int)((short)((sbyte)a) - (short)b);
					return result;
				case TypeCode.UInt16:
					result = (int)((ushort)((sbyte)a) - (ushort)b);
					return result;
				case TypeCode.Int32:
					result = (int)((sbyte)a) - (int)b;
					return result;
				case TypeCode.UInt32:
					result = (long)((sbyte)a) - (long)((ulong)((uint)b));
					return result;
				case TypeCode.Int64:
					result = (long)((sbyte)a) - (long)b;
					return result;
				case TypeCode.UInt64:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'sbyte' and 'ulong'");
				case TypeCode.Single:
					result = (float)((sbyte)a) - (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((sbyte)a) - (double)b;
					return result;
				case TypeCode.Decimal:
					result = (sbyte)a - (decimal)b;
					return result;
				}
				break;
			case TypeCode.Byte:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'byte' and 'bool'");
				case TypeCode.SByte:
					result = (int)((byte)a - (byte)((sbyte)b));
					return result;
				case TypeCode.Int16:
					result = (int)((short)((byte)a) - (short)b);
					return result;
				case TypeCode.UInt16:
					result = (int)((ushort)((byte)a) - (ushort)b);
					return result;
				case TypeCode.Int32:
					result = (int)((byte)a) - (int)b;
					return result;
				case TypeCode.UInt32:
					result = (uint)((byte)a) - (uint)b;
					return result;
				case TypeCode.Int64:
					result = (long)((ulong)((byte)a) - (ulong)((long)b));
					return result;
				case TypeCode.UInt64:
					result = (ulong)((byte)a) - (ulong)b;
					return result;
				case TypeCode.Single:
					result = (float)((byte)a) - (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((byte)a) - (double)b;
					return result;
				case TypeCode.Decimal:
					result = (byte)a - (decimal)b;
					return result;
				}
				break;
			case TypeCode.Int16:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'short' and 'bool'");
				case TypeCode.SByte:
					result = (int)((short)a - (short)((sbyte)b));
					return result;
				case TypeCode.Int16:
					result = (int)((short)a - (short)b);
					return result;
				case TypeCode.UInt16:
					result = (int)((short)a - (short)((ushort)b));
					return result;
				case TypeCode.Int32:
					result = (int)((short)a) - (int)b;
					return result;
				case TypeCode.UInt32:
					result = (long)((short)a) - (long)((ulong)((uint)b));
					return result;
				case TypeCode.Int64:
					result = (long)((short)a) - (long)b;
					return result;
				case TypeCode.UInt64:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'short' and 'ulong'");
				case TypeCode.Single:
					result = (float)((short)a) - (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((short)a) - (double)b;
					return result;
				case TypeCode.Decimal:
					result = (short)a - (decimal)b;
					return result;
				}
				break;
			case TypeCode.UInt16:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'ushort' and 'bool'");
				case TypeCode.SByte:
					result = (int)((ushort)a - (ushort)((sbyte)b));
					return result;
				case TypeCode.Int16:
					result = (int)((ushort)a - (ushort)((short)b));
					return result;
				case TypeCode.UInt16:
					result = (int)((ushort)a - (ushort)b);
					return result;
				case TypeCode.Int32:
					result = (int)((ushort)a) - (int)b;
					return result;
				case TypeCode.UInt32:
					result = (uint)((ushort)a) - (uint)b;
					return result;
				case TypeCode.Int64:
					result = (long)((ulong)((ushort)a) - (ulong)((long)b));
					return result;
				case TypeCode.UInt64:
					result = (ulong)((ushort)a) - (ulong)b;
					return result;
				case TypeCode.Single:
					result = (float)((ushort)a) - (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((ushort)a) - (double)b;
					return result;
				case TypeCode.Decimal:
					result = (ushort)a - (decimal)b;
					return result;
				}
				break;
			case TypeCode.Int32:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'int' and 'bool'");
				case TypeCode.SByte:
					result = (int)a - (int)((sbyte)b);
					return result;
				case TypeCode.Int16:
					result = (int)a - (int)((short)b);
					return result;
				case TypeCode.UInt16:
					result = (int)a - (int)((ushort)b);
					return result;
				case TypeCode.Int32:
					result = (int)a - (int)b;
					return result;
				case TypeCode.UInt32:
					result = (long)((int)a) - (long)((ulong)((uint)b));
					return result;
				case TypeCode.Int64:
					result = (long)((int)a) - (long)b;
					return result;
				case TypeCode.UInt64:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'int' and 'ulong'");
				case TypeCode.Single:
					result = (float)((int)a) - (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((int)a) - (double)b;
					return result;
				case TypeCode.Decimal:
					result = (int)a - (decimal)b;
					return result;
				}
				break;
			case TypeCode.UInt32:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'uint' and 'bool'");
				case TypeCode.SByte:
					result = (long)((ulong)((uint)a) - (ulong)((long)((sbyte)b)));
					return result;
				case TypeCode.Int16:
					result = (long)((ulong)((uint)a) - (ulong)((long)((short)b)));
					return result;
				case TypeCode.UInt16:
					result = (uint)a - (uint)((ushort)b);
					return result;
				case TypeCode.Int32:
					result = (long)((ulong)((uint)a) - (ulong)((long)((int)b)));
					return result;
				case TypeCode.UInt32:
					result = (uint)a - (uint)b;
					return result;
				case TypeCode.Int64:
					result = (long)((ulong)((uint)a) - (ulong)((long)b));
					return result;
				case TypeCode.UInt64:
					result = (ulong)((uint)a) - (ulong)b;
					return result;
				case TypeCode.Single:
					result = (uint)a - (float)b;
					return result;
				case TypeCode.Double:
					result = (uint)a - (double)b;
					return result;
				case TypeCode.Decimal:
					result = (uint)a - (decimal)b;
					return result;
				}
				break;
			case TypeCode.Int64:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'long' and 'bool'");
				case TypeCode.SByte:
					result = (long)a - (long)((sbyte)b);
					return result;
				case TypeCode.Int16:
					result = (long)a - (long)((short)b);
					return result;
				case TypeCode.UInt16:
					result = (long)a - (long)((ulong)((ushort)b));
					return result;
				case TypeCode.Int32:
					result = (long)a - (long)((int)b);
					return result;
				case TypeCode.UInt32:
					result = (long)a - (long)((ulong)((uint)b));
					return result;
				case TypeCode.Int64:
					result = (long)a - (long)b;
					return result;
				case TypeCode.UInt64:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'long' and 'ulong'");
				case TypeCode.Single:
					result = (float)((long)a) - (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((long)a) - (double)b;
					return result;
				case TypeCode.Decimal:
					result = (long)a - (decimal)b;
					return result;
				}
				break;
			case TypeCode.UInt64:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'ulong' and 'bool'");
				case TypeCode.SByte:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'ulong' and 'double'");
				case TypeCode.Int16:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'ulong' and 'short'");
				case TypeCode.UInt16:
					result = (ulong)a - (ulong)((ushort)b);
					return result;
				case TypeCode.Int32:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'ulong' and 'int'");
				case TypeCode.UInt32:
					result = (ulong)a - (ulong)((uint)b);
					return result;
				case TypeCode.Int64:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'ulong' and 'long'");
				case TypeCode.UInt64:
					result = (ulong)a - (ulong)b;
					return result;
				case TypeCode.Single:
					result = (ulong)a - (float)b;
					return result;
				case TypeCode.Double:
					result = (ulong)a - (double)b;
					return result;
				case TypeCode.Decimal:
					result = (ulong)a - (decimal)b;
					return result;
				}
				break;
			case TypeCode.Single:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'float' and 'bool'");
				case TypeCode.SByte:
					result = (float)a - (float)((sbyte)b);
					return result;
				case TypeCode.Int16:
					result = (float)a - (float)((short)b);
					return result;
				case TypeCode.UInt16:
					result = (float)a - (float)((ushort)b);
					return result;
				case TypeCode.Int32:
					result = (float)a - (float)((int)b);
					return result;
				case TypeCode.UInt32:
					result = (float)a - (uint)b;
					return result;
				case TypeCode.Int64:
					result = (float)a - (float)((long)b);
					return result;
				case TypeCode.UInt64:
					result = (float)a - (ulong)b;
					return result;
				case TypeCode.Single:
					result = (float)a - (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((float)a) - (double)b;
					return result;
				case TypeCode.Decimal:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'float' and 'decimal'");
				}
				break;
			case TypeCode.Double:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'double' and 'bool'");
				case TypeCode.SByte:
					result = (double)a - (double)((sbyte)b);
					return result;
				case TypeCode.Int16:
					result = (double)a - (double)((short)b);
					return result;
				case TypeCode.UInt16:
					result = (double)a - (double)((ushort)b);
					return result;
				case TypeCode.Int32:
					result = (double)a - (double)((int)b);
					return result;
				case TypeCode.UInt32:
					result = (double)a - (uint)b;
					return result;
				case TypeCode.Int64:
					result = (double)a - (double)((long)b);
					return result;
				case TypeCode.UInt64:
					result = (double)a - (ulong)b;
					return result;
				case TypeCode.Single:
					result = (double)a - (double)((float)b);
					return result;
				case TypeCode.Double:
					result = (double)a - (double)b;
					return result;
				case TypeCode.Decimal:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'double' and 'decimal'");
				}
				break;
			case TypeCode.Decimal:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'decimal' and 'bool'");
				case TypeCode.SByte:
					result = (decimal)a - (sbyte)b;
					return result;
				case TypeCode.Int16:
					result = (decimal)a - (short)b;
					return result;
				case TypeCode.UInt16:
					result = (decimal)a - (ushort)b;
					return result;
				case TypeCode.Int32:
					result = (decimal)a - (int)b;
					return result;
				case TypeCode.UInt32:
					result = (decimal)a - (uint)b;
					return result;
				case TypeCode.Int64:
					result = (decimal)a - (long)b;
					return result;
				case TypeCode.UInt64:
					result = (decimal)a - (ulong)b;
					return result;
				case TypeCode.Single:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'decimal' and 'float'");
				case TypeCode.Double:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'decimal' and 'double'");
				case TypeCode.Decimal:
					result = (decimal)a - (decimal)b;
					return result;
				}
				break;
			}
			result = null;
			return result;
		}
		public static object Multiply(object a, object b)
		{
			a = Numbers.ConvertIfString(a);
			b = Numbers.ConvertIfString(b);
			TypeCode typeCode = Type.GetTypeCode(a.GetType());
			TypeCode typeCode2 = Type.GetTypeCode(b.GetType());
			object result;
			switch (typeCode)
			{
			case TypeCode.SByte:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'sbyte' and 'bool'");
				case TypeCode.SByte:
					result = (int)((sbyte)a * (sbyte)b);
					return result;
				case TypeCode.Int16:
					result = (int)((short)((sbyte)a) * (short)b);
					return result;
				case TypeCode.UInt16:
					result = (int)((ushort)((sbyte)a) * (ushort)b);
					return result;
				case TypeCode.Int32:
					result = (int)((sbyte)a) * (int)b;
					return result;
				case TypeCode.UInt32:
					result = (long)((sbyte)a) * (long)((ulong)((uint)b));
					return result;
				case TypeCode.Int64:
					result = (long)((sbyte)a) * (long)b;
					return result;
				case TypeCode.UInt64:
					throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'sbyte' and 'ulong'");
				case TypeCode.Single:
					result = (float)((sbyte)a) * (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((sbyte)a) * (double)b;
					return result;
				case TypeCode.Decimal:
					result = (sbyte)a * (decimal)b;
					return result;
				}
				break;
			case TypeCode.Byte:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'byte' and 'bool'");
				case TypeCode.SByte:
					result = (int)((byte)a * (byte)((sbyte)b));
					return result;
				case TypeCode.Int16:
					result = (int)((short)((byte)a) * (short)b);
					return result;
				case TypeCode.UInt16:
					result = (int)((ushort)((byte)a) * (ushort)b);
					return result;
				case TypeCode.Int32:
					result = (int)((byte)a) * (int)b;
					return result;
				case TypeCode.UInt32:
					result = (uint)((byte)a) * (uint)b;
					return result;
				case TypeCode.Int64:
					result = (long)((ulong)((byte)a) * (ulong)((long)b));
					return result;
				case TypeCode.UInt64:
					result = (ulong)((byte)a) * (ulong)b;
					return result;
				case TypeCode.Single:
					result = (float)((byte)a) * (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((byte)a) * (double)b;
					return result;
				case TypeCode.Decimal:
					result = (byte)a * (decimal)b;
					return result;
				}
				break;
			case TypeCode.Int16:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'short' and 'bool'");
				case TypeCode.SByte:
					result = (int)((short)a * (short)((sbyte)b));
					return result;
				case TypeCode.Int16:
					result = (int)((short)a * (short)b);
					return result;
				case TypeCode.UInt16:
					result = (int)((short)a * (short)((ushort)b));
					return result;
				case TypeCode.Int32:
					result = (int)((short)a) * (int)b;
					return result;
				case TypeCode.UInt32:
					result = (long)((short)a) * (long)((ulong)((uint)b));
					return result;
				case TypeCode.Int64:
					result = (long)((short)a) * (long)b;
					return result;
				case TypeCode.UInt64:
					throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'short' and 'ulong'");
				case TypeCode.Single:
					result = (float)((short)a) * (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((short)a) * (double)b;
					return result;
				case TypeCode.Decimal:
					result = (short)a * (decimal)b;
					return result;
				}
				break;
			case TypeCode.UInt16:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'ushort' and 'bool'");
				case TypeCode.SByte:
					result = (int)((ushort)a * (ushort)((sbyte)b));
					return result;
				case TypeCode.Int16:
					result = (int)((ushort)a * (ushort)((short)b));
					return result;
				case TypeCode.UInt16:
					result = (int)((ushort)a * (ushort)b);
					return result;
				case TypeCode.Int32:
					result = (int)((ushort)a) * (int)b;
					return result;
				case TypeCode.UInt32:
					result = (uint)((ushort)a) * (uint)b;
					return result;
				case TypeCode.Int64:
					result = (long)((ulong)((ushort)a) * (ulong)((long)b));
					return result;
				case TypeCode.UInt64:
					result = (ulong)((ushort)a) * (ulong)b;
					return result;
				case TypeCode.Single:
					result = (float)((ushort)a) * (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((ushort)a) * (double)b;
					return result;
				case TypeCode.Decimal:
					result = (ushort)a * (decimal)b;
					return result;
				}
				break;
			case TypeCode.Int32:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'int' and 'bool'");
				case TypeCode.SByte:
					result = (int)a * (int)((sbyte)b);
					return result;
				case TypeCode.Int16:
					result = (int)a * (int)((short)b);
					return result;
				case TypeCode.UInt16:
					result = (int)a * (int)((ushort)b);
					return result;
				case TypeCode.Int32:
					result = (int)a * (int)b;
					return result;
				case TypeCode.UInt32:
					result = (long)((int)a) * (long)((ulong)((uint)b));
					return result;
				case TypeCode.Int64:
					result = (long)((int)a) * (long)b;
					return result;
				case TypeCode.UInt64:
					throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'int' and 'ulong'");
				case TypeCode.Single:
					result = (float)((int)a) * (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((int)a) * (double)b;
					return result;
				case TypeCode.Decimal:
					result = (int)a * (decimal)b;
					return result;
				}
				break;
			case TypeCode.UInt32:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'uint' and 'bool'");
				case TypeCode.SByte:
					result = (long)((ulong)((uint)a) * (ulong)((long)((sbyte)b)));
					return result;
				case TypeCode.Int16:
					result = (long)((ulong)((uint)a) * (ulong)((long)((short)b)));
					return result;
				case TypeCode.UInt16:
					result = (uint)a * (uint)((ushort)b);
					return result;
				case TypeCode.Int32:
					result = (long)((ulong)((uint)a) * (ulong)((long)((int)b)));
					return result;
				case TypeCode.UInt32:
					result = (uint)a * (uint)b;
					return result;
				case TypeCode.Int64:
					result = (long)((ulong)((uint)a) * (ulong)((long)b));
					return result;
				case TypeCode.UInt64:
					result = (ulong)((uint)a) * (ulong)b;
					return result;
				case TypeCode.Single:
					result = (uint)a * (float)b;
					return result;
				case TypeCode.Double:
					result = (uint)a * (double)b;
					return result;
				case TypeCode.Decimal:
					result = (uint)a * (decimal)b;
					return result;
				}
				break;
			case TypeCode.Int64:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'long' and 'bool'");
				case TypeCode.SByte:
					result = (long)a * (long)((sbyte)b);
					return result;
				case TypeCode.Int16:
					result = (long)a * (long)((short)b);
					return result;
				case TypeCode.UInt16:
					result = (long)a * (long)((ulong)((ushort)b));
					return result;
				case TypeCode.Int32:
					result = (long)a * (long)((int)b);
					return result;
				case TypeCode.UInt32:
					result = (long)a * (long)((ulong)((uint)b));
					return result;
				case TypeCode.Int64:
					result = (long)a * (long)b;
					return result;
				case TypeCode.UInt64:
					throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'long' and 'ulong'");
				case TypeCode.Single:
					result = (float)((long)a) * (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((long)a) * (double)b;
					return result;
				case TypeCode.Decimal:
					result = (long)a * (decimal)b;
					return result;
				}
				break;
			case TypeCode.UInt64:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'ulong' and 'bool'");
				case TypeCode.SByte:
					throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'ulong' and 'sbyte'");
				case TypeCode.Int16:
					throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'ulong' and 'short'");
				case TypeCode.UInt16:
					result = (ulong)a * (ulong)((ushort)b);
					return result;
				case TypeCode.Int32:
					throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'ulong' and 'int'");
				case TypeCode.UInt32:
					result = (ulong)a * (ulong)((uint)b);
					return result;
				case TypeCode.Int64:
					throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'ulong' and 'long'");
				case TypeCode.UInt64:
					result = (ulong)a * (ulong)b;
					return result;
				case TypeCode.Single:
					result = (ulong)a * (float)b;
					return result;
				case TypeCode.Double:
					result = (ulong)a * (double)b;
					return result;
				case TypeCode.Decimal:
					result = (ulong)a * (decimal)b;
					return result;
				}
				break;
			case TypeCode.Single:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'float' and 'bool'");
				case TypeCode.SByte:
					result = (float)a * (float)((sbyte)b);
					return result;
				case TypeCode.Int16:
					result = (float)a * (float)((short)b);
					return result;
				case TypeCode.UInt16:
					result = (float)a * (float)((ushort)b);
					return result;
				case TypeCode.Int32:
					result = (float)a * (float)((int)b);
					return result;
				case TypeCode.UInt32:
					result = (float)a * (uint)b;
					return result;
				case TypeCode.Int64:
					result = (float)a * (float)((long)b);
					return result;
				case TypeCode.UInt64:
					result = (float)a * (ulong)b;
					return result;
				case TypeCode.Single:
					result = (float)a * (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((float)a) * (double)b;
					return result;
				case TypeCode.Decimal:
					throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'float' and 'decimal'");
				}
				break;
			case TypeCode.Double:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'double' and 'bool'");
				case TypeCode.SByte:
					result = (double)a * (double)((sbyte)b);
					return result;
				case TypeCode.Int16:
					result = (double)a * (double)((short)b);
					return result;
				case TypeCode.UInt16:
					result = (double)a * (double)((ushort)b);
					return result;
				case TypeCode.Int32:
					result = (double)a * (double)((int)b);
					return result;
				case TypeCode.UInt32:
					result = (double)a * (uint)b;
					return result;
				case TypeCode.Int64:
					result = (double)a * (double)((long)b);
					return result;
				case TypeCode.UInt64:
					result = (double)a * (ulong)b;
					return result;
				case TypeCode.Single:
					result = (double)a * (double)((float)b);
					return result;
				case TypeCode.Double:
					result = (double)a * (double)b;
					return result;
				case TypeCode.Decimal:
					throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'double' and 'decimal'");
				}
				break;
			case TypeCode.Decimal:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'decimal' and 'bool'");
				case TypeCode.SByte:
					result = (decimal)a * (sbyte)b;
					return result;
				case TypeCode.Int16:
					result = (decimal)a * (short)b;
					return result;
				case TypeCode.UInt16:
					result = (decimal)a * (ushort)b;
					return result;
				case TypeCode.Int32:
					result = (decimal)a * (int)b;
					return result;
				case TypeCode.UInt32:
					result = (decimal)a * (uint)b;
					return result;
				case TypeCode.Int64:
					result = (decimal)a * (long)b;
					return result;
				case TypeCode.UInt64:
					result = (decimal)a * (ulong)b;
					return result;
				case TypeCode.Single:
					throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'decimal' and 'float'");
				case TypeCode.Double:
					throw new InvalidOperationException("Operator '*' can't be applied to operands of types 'decimal' and 'double'");
				case TypeCode.Decimal:
					result = (decimal)a * (decimal)b;
					return result;
				}
				break;
			}
			result = null;
			return result;
		}
		public static object Divide(object a, object b)
		{
			a = Numbers.ConvertIfString(a);
			b = Numbers.ConvertIfString(b);
			TypeCode typeCode = Type.GetTypeCode(a.GetType());
			TypeCode typeCode2 = Type.GetTypeCode(b.GetType());
			object result;
			switch (typeCode)
			{
			case TypeCode.SByte:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'sbyte' and 'bool'");
				case TypeCode.SByte:
					result = (int)((sbyte)a / (sbyte)b);
					return result;
				case TypeCode.Int16:
					result = (int)((short)((sbyte)a) / (short)b);
					return result;
				case TypeCode.UInt16:
					result = (int)((ushort)((sbyte)a) / (ushort)b);
					return result;
				case TypeCode.Int32:
					result = (int)((sbyte)a) / (int)b;
					return result;
				case TypeCode.UInt32:
					result = (long)((sbyte)a) / (long)((ulong)((uint)b));
					return result;
				case TypeCode.Int64:
					result = (long)((sbyte)a) / (long)b;
					return result;
				case TypeCode.UInt64:
					throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'sbyte' and 'ulong'");
				case TypeCode.Single:
					result = (float)((sbyte)a) / (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((sbyte)a) / (double)b;
					return result;
				case TypeCode.Decimal:
					result = (sbyte)a / (decimal)b;
					return result;
				}
				break;
			case TypeCode.Byte:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'byte' and 'bool'");
				case TypeCode.SByte:
					result = (int)((byte)a / (byte)((sbyte)b));
					return result;
				case TypeCode.Int16:
					result = (int)((short)((byte)a) / (short)b);
					return result;
				case TypeCode.UInt16:
					result = (int)((ushort)((byte)a) / (ushort)b);
					return result;
				case TypeCode.Int32:
					result = (int)((byte)a) / (int)b;
					return result;
				case TypeCode.UInt32:
					result = (uint)((byte)a) / (uint)b;
					return result;
				case TypeCode.Int64:
					result = (long)((ulong)((byte)a) / (ulong)((long)b));
					return result;
				case TypeCode.UInt64:
					result = (ulong)((byte)a) / (ulong)b;
					return result;
				case TypeCode.Single:
					result = (float)((byte)a) / (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((byte)a) / (double)b;
					return result;
				case TypeCode.Decimal:
					result = (byte)a / (decimal)b;
					return result;
				}
				break;
			case TypeCode.Int16:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'short' and 'bool'");
				case TypeCode.SByte:
					result = (int)((short)a / (short)((sbyte)b));
					return result;
				case TypeCode.Int16:
					result = (int)((short)a / (short)b);
					return result;
				case TypeCode.UInt16:
					result = (int)((short)a / (short)((ushort)b));
					return result;
				case TypeCode.Int32:
					result = (int)((short)a) / (int)b;
					return result;
				case TypeCode.UInt32:
					result = (long)((short)a) / (long)((ulong)((uint)b));
					return result;
				case TypeCode.Int64:
					result = (long)((short)a) / (long)b;
					return result;
				case TypeCode.UInt64:
					throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'short' and 'ulong'");
				case TypeCode.Single:
					result = (float)((short)a) / (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((short)a) / (double)b;
					return result;
				case TypeCode.Decimal:
					result = (short)a / (decimal)b;
					return result;
				}
				break;
			case TypeCode.UInt16:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'ushort' and 'bool'");
				case TypeCode.SByte:
					result = (int)((ushort)a / (ushort)((sbyte)b));
					return result;
				case TypeCode.Int16:
					result = (int)((ushort)a / (ushort)((short)b));
					return result;
				case TypeCode.UInt16:
					result = (int)((ushort)a / (ushort)b);
					return result;
				case TypeCode.Int32:
					result = (int)((ushort)a) / (int)b;
					return result;
				case TypeCode.UInt32:
					result = (uint)((ushort)a) / (uint)b;
					return result;
				case TypeCode.Int64:
					result = (long)((ulong)((ushort)a) / (ulong)((long)b));
					return result;
				case TypeCode.UInt64:
					result = (ulong)((ushort)a) / (ulong)b;
					return result;
				case TypeCode.Single:
					result = (float)((ushort)a) / (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((ushort)a) / (double)b;
					return result;
				case TypeCode.Decimal:
					result = (ushort)a / (decimal)b;
					return result;
				}
				break;
			case TypeCode.Int32:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'int' and 'bool'");
				case TypeCode.SByte:
					result = (int)a / (int)((sbyte)b);
					return result;
				case TypeCode.Int16:
					result = (int)a / (int)((short)b);
					return result;
				case TypeCode.UInt16:
					result = (int)a / (int)((ushort)b);
					return result;
				case TypeCode.Int32:
					result = (int)a / (int)b;
					return result;
				case TypeCode.UInt32:
					result = (long)((int)a) / (long)((ulong)((uint)b));
					return result;
				case TypeCode.Int64:
					result = (long)((int)a) / (long)b;
					return result;
				case TypeCode.UInt64:
					throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'int' and 'ulong'");
				case TypeCode.Single:
					result = (float)((int)a) / (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((int)a) / (double)b;
					return result;
				case TypeCode.Decimal:
					result = (int)a / (decimal)b;
					return result;
				}
				break;
			case TypeCode.UInt32:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'uint' and 'bool'");
				case TypeCode.SByte:
					result = (long)((ulong)((uint)a) / (ulong)((long)((sbyte)b)));
					return result;
				case TypeCode.Int16:
					result = (long)((ulong)((uint)a) / (ulong)((long)((short)b)));
					return result;
				case TypeCode.UInt16:
					result = (uint)a / (uint)((ushort)b);
					return result;
				case TypeCode.Int32:
					result = (long)((ulong)((uint)a) / (ulong)((long)((int)b)));
					return result;
				case TypeCode.UInt32:
					result = (uint)a / (uint)b;
					return result;
				case TypeCode.Int64:
					result = (long)((ulong)((uint)a) / (ulong)((long)b));
					return result;
				case TypeCode.UInt64:
					result = (ulong)((uint)a) / (ulong)b;
					return result;
				case TypeCode.Single:
					result = (uint)a / (float)b;
					return result;
				case TypeCode.Double:
					result = (uint)a / (double)b;
					return result;
				case TypeCode.Decimal:
					result = (uint)a / (decimal)b;
					return result;
				}
				break;
			case TypeCode.Int64:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'long' and 'bool'");
				case TypeCode.SByte:
					result = (long)a / (long)((sbyte)b);
					return result;
				case TypeCode.Int16:
					result = (long)a / (long)((short)b);
					return result;
				case TypeCode.UInt16:
					result = (long)a / (long)((ulong)((ushort)b));
					return result;
				case TypeCode.Int32:
					result = (long)a / (long)((int)b);
					return result;
				case TypeCode.UInt32:
					result = (long)a / (long)((ulong)((uint)b));
					return result;
				case TypeCode.Int64:
					result = (long)a / (long)b;
					return result;
				case TypeCode.UInt64:
					throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'long' and 'ulong'");
				case TypeCode.Single:
					result = (float)((long)a) / (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((long)a) / (double)b;
					return result;
				case TypeCode.Decimal:
					result = (long)a / (decimal)b;
					return result;
				}
				break;
			case TypeCode.UInt64:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '-' can't be applied to operands of types 'ulong' and 'bool'");
				case TypeCode.SByte:
					throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'ulong' and 'sbyte'");
				case TypeCode.Int16:
					throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'ulong' and 'short'");
				case TypeCode.UInt16:
					result = (ulong)a / (ulong)((ushort)b);
					return result;
				case TypeCode.Int32:
					throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'ulong' and 'int'");
				case TypeCode.UInt32:
					result = (ulong)a / (ulong)((uint)b);
					return result;
				case TypeCode.Int64:
					throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'ulong' and 'long'");
				case TypeCode.UInt64:
					result = (ulong)a / (ulong)b;
					return result;
				case TypeCode.Single:
					result = (ulong)a / (float)b;
					return result;
				case TypeCode.Double:
					result = (ulong)a / (double)b;
					return result;
				case TypeCode.Decimal:
					result = (ulong)a / (decimal)b;
					return result;
				}
				break;
			case TypeCode.Single:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'float' and 'bool'");
				case TypeCode.SByte:
					result = (float)a / (float)((sbyte)b);
					return result;
				case TypeCode.Int16:
					result = (float)a / (float)((short)b);
					return result;
				case TypeCode.UInt16:
					result = (float)a / (float)((ushort)b);
					return result;
				case TypeCode.Int32:
					result = (float)a / (float)((int)b);
					return result;
				case TypeCode.UInt32:
					result = (float)a / (uint)b;
					return result;
				case TypeCode.Int64:
					result = (float)a / (float)((long)b);
					return result;
				case TypeCode.UInt64:
					result = (float)a / (ulong)b;
					return result;
				case TypeCode.Single:
					result = (float)a / (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((float)a) / (double)b;
					return result;
				case TypeCode.Decimal:
					throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'float' and 'decimal'");
				}
				break;
			case TypeCode.Double:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'double' and 'bool'");
				case TypeCode.SByte:
					result = (double)a / (double)((sbyte)b);
					return result;
				case TypeCode.Int16:
					result = (double)a / (double)((short)b);
					return result;
				case TypeCode.UInt16:
					result = (double)a / (double)((ushort)b);
					return result;
				case TypeCode.Int32:
					result = (double)a / (double)((int)b);
					return result;
				case TypeCode.UInt32:
					result = (double)a / (uint)b;
					return result;
				case TypeCode.Int64:
					result = (double)a / (double)((long)b);
					return result;
				case TypeCode.UInt64:
					result = (double)a / (ulong)b;
					return result;
				case TypeCode.Single:
					result = (double)a / (double)((float)b);
					return result;
				case TypeCode.Double:
					result = (double)a / (double)b;
					return result;
				case TypeCode.Decimal:
					throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'double' and 'decimal'");
				}
				break;
			case TypeCode.Decimal:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'decimal' and 'bool'");
				case TypeCode.SByte:
					result = (decimal)a / (sbyte)b;
					return result;
				case TypeCode.Int16:
					result = (decimal)a / (short)b;
					return result;
				case TypeCode.UInt16:
					result = (decimal)a / (ushort)b;
					return result;
				case TypeCode.Int32:
					result = (decimal)a / (int)b;
					return result;
				case TypeCode.UInt32:
					result = (decimal)a / (uint)b;
					return result;
				case TypeCode.Int64:
					result = (decimal)a / (long)b;
					return result;
				case TypeCode.UInt64:
					result = (decimal)a / (ulong)b;
					return result;
				case TypeCode.Single:
					throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'decimal' and 'float'");
				case TypeCode.Double:
					throw new InvalidOperationException("Operator '/' can't be applied to operands of types 'decimal' and 'double'");
				case TypeCode.Decimal:
					result = (decimal)a / (decimal)b;
					return result;
				}
				break;
			}
			result = null;
			return result;
		}
		public static object Modulo(object a, object b)
		{
			a = Numbers.ConvertIfString(a);
			b = Numbers.ConvertIfString(b);
			TypeCode typeCode = Type.GetTypeCode(a.GetType());
			TypeCode typeCode2 = Type.GetTypeCode(b.GetType());
			object result;
			switch (typeCode)
			{
			case TypeCode.SByte:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'sbyte' and 'bool'");
				case TypeCode.SByte:
					result = (int)((sbyte)a % (sbyte)b);
					return result;
				case TypeCode.Int16:
					result = (int)((short)((sbyte)a) % (short)b);
					return result;
				case TypeCode.UInt16:
					result = (int)((ushort)((sbyte)a) % (ushort)b);
					return result;
				case TypeCode.Int32:
					result = (int)((sbyte)a) % (int)b;
					return result;
				case TypeCode.UInt32:
					result = (long)((sbyte)a) % (long)((ulong)((uint)b));
					return result;
				case TypeCode.Int64:
					result = (long)((sbyte)a) % (long)b;
					return result;
				case TypeCode.UInt64:
					throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'sbyte' and 'ulong'");
				case TypeCode.Single:
					result = (float)((sbyte)a) % (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((sbyte)a) % (double)b;
					return result;
				case TypeCode.Decimal:
					result = (sbyte)a % (decimal)b;
					return result;
				}
				break;
			case TypeCode.Byte:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'byte' and 'bool'");
				case TypeCode.SByte:
					result = (int)((byte)a % (byte)((sbyte)b));
					return result;
				case TypeCode.Int16:
					result = (int)((short)((byte)a) % (short)b);
					return result;
				case TypeCode.UInt16:
					result = (int)((ushort)((byte)a) % (ushort)b);
					return result;
				case TypeCode.Int32:
					result = (int)((byte)a) % (int)b;
					return result;
				case TypeCode.UInt32:
					result = (uint)((byte)a) % (uint)b;
					return result;
				case TypeCode.Int64:
					result = (long)((ulong)((byte)a) % (ulong)((long)b));
					return result;
				case TypeCode.UInt64:
					result = (ulong)((byte)a) % (ulong)b;
					return result;
				case TypeCode.Single:
					result = (float)((byte)a) % (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((byte)a) % (double)b;
					return result;
				case TypeCode.Decimal:
					result = (byte)a % (decimal)b;
					return result;
				}
				break;
			case TypeCode.Int16:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'short' and 'bool'");
				case TypeCode.SByte:
					result = (int)((short)a % (short)((sbyte)b));
					return result;
				case TypeCode.Int16:
					result = (int)((short)a % (short)b);
					return result;
				case TypeCode.UInt16:
					result = (int)((short)a % (short)((ushort)b));
					return result;
				case TypeCode.Int32:
					result = (int)((short)a) % (int)b;
					return result;
				case TypeCode.UInt32:
					result = (long)((short)a) % (long)((ulong)((uint)b));
					return result;
				case TypeCode.Int64:
					result = (long)((short)a) % (long)b;
					return result;
				case TypeCode.UInt64:
					throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'short' and 'ulong'");
				case TypeCode.Single:
					result = (float)((short)a) % (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((short)a) % (double)b;
					return result;
				case TypeCode.Decimal:
					result = (short)a % (decimal)b;
					return result;
				}
				break;
			case TypeCode.UInt16:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'ushort' and 'bool'");
				case TypeCode.SByte:
					result = (int)((ushort)a % (ushort)((sbyte)b));
					return result;
				case TypeCode.Int16:
					result = (int)((ushort)a % (ushort)((short)b));
					return result;
				case TypeCode.UInt16:
					result = (int)((ushort)a % (ushort)b);
					return result;
				case TypeCode.Int32:
					result = (int)((ushort)a) % (int)b;
					return result;
				case TypeCode.UInt32:
					result = (uint)((ushort)a) % (uint)b;
					return result;
				case TypeCode.Int64:
					result = (long)((ulong)((ushort)a) % (ulong)((long)b));
					return result;
				case TypeCode.UInt64:
					result = (ulong)((ushort)a) % (ulong)b;
					return result;
				case TypeCode.Single:
					result = (float)((ushort)a) % (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((ushort)a) % (double)b;
					return result;
				case TypeCode.Decimal:
					result = (ushort)a % (decimal)b;
					return result;
				}
				break;
			case TypeCode.Int32:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'int' and 'bool'");
				case TypeCode.SByte:
					result = (int)a % (int)((sbyte)b);
					return result;
				case TypeCode.Int16:
					result = (int)a % (int)((short)b);
					return result;
				case TypeCode.UInt16:
					result = (int)a % (int)((ushort)b);
					return result;
				case TypeCode.Int32:
					result = (int)a % (int)b;
					return result;
				case TypeCode.UInt32:
					result = (long)((int)a) % (long)((ulong)((uint)b));
					return result;
				case TypeCode.Int64:
					result = (long)((int)a) % (long)b;
					return result;
				case TypeCode.UInt64:
					throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'int' and 'ulong'");
				case TypeCode.Single:
					result = (float)((int)a) % (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((int)a) % (double)b;
					return result;
				case TypeCode.Decimal:
					result = (int)a % (decimal)b;
					return result;
				}
				break;
			case TypeCode.UInt32:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'uint' and 'bool'");
				case TypeCode.SByte:
					result = (long)((ulong)((uint)a) % (ulong)((long)((sbyte)b)));
					return result;
				case TypeCode.Int16:
					result = (long)((ulong)((uint)a) % (ulong)((long)((short)b)));
					return result;
				case TypeCode.UInt16:
					result = (uint)a % (uint)((ushort)b);
					return result;
				case TypeCode.Int32:
					result = (long)((ulong)((uint)a) % (ulong)((long)((int)b)));
					return result;
				case TypeCode.UInt32:
					result = (uint)a % (uint)b;
					return result;
				case TypeCode.Int64:
					result = (long)((ulong)((uint)a) % (ulong)((long)b));
					return result;
				case TypeCode.UInt64:
					result = (ulong)((uint)a) % (ulong)b;
					return result;
				case TypeCode.Single:
					result = (uint)a % (float)b;
					return result;
				case TypeCode.Double:
					result = (uint)a % (double)b;
					return result;
				case TypeCode.Decimal:
					result = (uint)a % (decimal)b;
					return result;
				}
				break;
			case TypeCode.Int64:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'long' and 'bool'");
				case TypeCode.SByte:
					result = (long)a % (long)((sbyte)b);
					return result;
				case TypeCode.Int16:
					result = (long)a % (long)((short)b);
					return result;
				case TypeCode.UInt16:
					result = (long)a % (long)((ulong)((ushort)b));
					return result;
				case TypeCode.Int32:
					result = (long)a % (long)((int)b);
					return result;
				case TypeCode.UInt32:
					result = (long)a % (long)((ulong)((uint)b));
					return result;
				case TypeCode.Int64:
					result = (long)a % (long)b;
					return result;
				case TypeCode.UInt64:
					throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'long' and 'ulong'");
				case TypeCode.Single:
					result = (float)((long)a) % (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((long)a) % (double)b;
					return result;
				case TypeCode.Decimal:
					result = (long)a % (decimal)b;
					return result;
				}
				break;
			case TypeCode.UInt64:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'ulong' and 'bool'");
				case TypeCode.SByte:
					throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'ulong' and 'sbyte'");
				case TypeCode.Int16:
					throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'ulong' and 'short'");
				case TypeCode.UInt16:
					result = (ulong)a % (ulong)((ushort)b);
					return result;
				case TypeCode.Int32:
					throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'ulong' and 'int'");
				case TypeCode.UInt32:
					result = (ulong)a % (ulong)((uint)b);
					return result;
				case TypeCode.Int64:
					throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'ulong' and 'long'");
				case TypeCode.UInt64:
					result = (ulong)a % (ulong)b;
					return result;
				case TypeCode.Single:
					result = (ulong)a % (float)b;
					return result;
				case TypeCode.Double:
					result = (ulong)a % (double)b;
					return result;
				case TypeCode.Decimal:
					result = (ulong)a % (decimal)b;
					return result;
				}
				break;
			case TypeCode.Single:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'float' and 'bool'");
				case TypeCode.SByte:
					result = (float)a % (float)((sbyte)b);
					return result;
				case TypeCode.Int16:
					result = (float)a % (float)((short)b);
					return result;
				case TypeCode.UInt16:
					result = (float)a % (float)((ushort)b);
					return result;
				case TypeCode.Int32:
					result = (float)a % (float)((int)b);
					return result;
				case TypeCode.UInt32:
					result = (float)a % (uint)b;
					return result;
				case TypeCode.Int64:
					result = (float)a % (float)((long)b);
					return result;
				case TypeCode.UInt64:
					result = (float)a % (ulong)b;
					return result;
				case TypeCode.Single:
					result = (float)a % (float)b;
					return result;
				case TypeCode.Double:
					result = (double)((float)a) % (double)b;
					return result;
				case TypeCode.Decimal:
					throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'float' and 'decimal'");
				}
				break;
			case TypeCode.Double:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'double' and 'bool'");
				case TypeCode.SByte:
					result = (double)a % (double)((sbyte)b);
					return result;
				case TypeCode.Int16:
					result = (double)a % (double)((short)b);
					return result;
				case TypeCode.UInt16:
					result = (double)a % (double)((ushort)b);
					return result;
				case TypeCode.Int32:
					result = (double)a % (double)((int)b);
					return result;
				case TypeCode.UInt32:
					result = (double)a % (uint)b;
					return result;
				case TypeCode.Int64:
					result = (double)a % (double)((long)b);
					return result;
				case TypeCode.UInt64:
					result = (double)a % (ulong)b;
					return result;
				case TypeCode.Single:
					result = (double)a % (double)((float)b);
					return result;
				case TypeCode.Double:
					result = (double)a % (double)b;
					return result;
				case TypeCode.Decimal:
					throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'double' and 'decimal'");
				}
				break;
			case TypeCode.Decimal:
				switch (typeCode2)
				{
				case TypeCode.Boolean:
					throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'decimal' and 'bool'");
				case TypeCode.SByte:
					result = (decimal)a % (sbyte)b;
					return result;
				case TypeCode.Int16:
					result = (decimal)a % (short)b;
					return result;
				case TypeCode.UInt16:
					result = (decimal)a % (ushort)b;
					return result;
				case TypeCode.Int32:
					result = (decimal)a % (int)b;
					return result;
				case TypeCode.UInt32:
					result = (decimal)a % (uint)b;
					return result;
				case TypeCode.Int64:
					result = (decimal)a % (long)b;
					return result;
				case TypeCode.UInt64:
					result = (decimal)a % (ulong)b;
					return result;
				case TypeCode.Single:
					throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'decimal' and 'float'");
				case TypeCode.Double:
					throw new InvalidOperationException("Operator '%' can't be applied to operands of types 'decimal' and 'decimal'");
				case TypeCode.Decimal:
					result = (decimal)a % (decimal)b;
					return result;
				}
				break;
			}
			result = null;
			return result;
		}
		public static object Max(object a, object b)
		{
			a = Numbers.ConvertIfString(a);
			b = Numbers.ConvertIfString(b);
			object result;
			if (a == null && b == null)
			{
				result = null;
			}
			else
			{
				if (a == null)
				{
					result = b;
				}
				else
				{
					if (b == null)
					{
						result = a;
					}
					else
					{
						switch (Type.GetTypeCode(a.GetType()))
						{
						case TypeCode.SByte:
							result = Math.Max((sbyte)a, Convert.ToSByte(b));
							break;
						case TypeCode.Byte:
							result = Math.Max((byte)a, Convert.ToByte(b));
							break;
						case TypeCode.Int16:
							result = Math.Max((short)a, Convert.ToInt16(b));
							break;
						case TypeCode.UInt16:
							result = Math.Max((ushort)a, Convert.ToUInt16(b));
							break;
						case TypeCode.Int32:
							result = Math.Max((int)a, Convert.ToInt32(b));
							break;
						case TypeCode.UInt32:
							result = Math.Max((uint)a, Convert.ToUInt32(b));
							break;
						case TypeCode.Int64:
							result = Math.Max((long)a, Convert.ToInt64(b));
							break;
						case TypeCode.UInt64:
							result = Math.Max((ulong)a, Convert.ToUInt64(b));
							break;
						case TypeCode.Single:
							result = Math.Max((float)a, Convert.ToSingle(b));
							break;
						case TypeCode.Double:
							result = Math.Max((double)a, Convert.ToDouble(b));
							break;
						case TypeCode.Decimal:
							result = Math.Max((decimal)a, Convert.ToDecimal(b));
							break;
						default:
							result = null;
							break;
						}
					}
				}
			}
			return result;
		}
		public static object Min(object a, object b)
		{
			a = Numbers.ConvertIfString(a);
			b = Numbers.ConvertIfString(b);
			object result;
			if (a == null && b == null)
			{
				result = null;
			}
			else
			{
				if (a == null)
				{
					result = b;
				}
				else
				{
					if (b == null)
					{
						result = a;
					}
					else
					{
						switch (Type.GetTypeCode(a.GetType()))
						{
						case TypeCode.SByte:
							result = Math.Min((sbyte)a, Convert.ToSByte(b));
							break;
						case TypeCode.Byte:
							result = Math.Min((byte)a, Convert.ToByte(b));
							break;
						case TypeCode.Int16:
							result = Math.Min((short)a, Convert.ToInt16(b));
							break;
						case TypeCode.UInt16:
							result = Math.Min((ushort)a, Convert.ToUInt16(b));
							break;
						case TypeCode.Int32:
							result = Math.Min((int)a, Convert.ToInt32(b));
							break;
						case TypeCode.UInt32:
							result = Math.Min((uint)a, Convert.ToUInt32(b));
							break;
						case TypeCode.Int64:
							result = Math.Min((long)a, Convert.ToInt64(b));
							break;
						case TypeCode.UInt64:
							result = Math.Min((ulong)a, Convert.ToUInt64(b));
							break;
						case TypeCode.Single:
							result = Math.Min((float)a, Convert.ToSingle(b));
							break;
						case TypeCode.Double:
							result = Math.Min((double)a, Convert.ToDouble(b));
							break;
						case TypeCode.Decimal:
							result = Math.Min((decimal)a, Convert.ToDecimal(b));
							break;
						default:
							result = null;
							break;
						}
					}
				}
			}
			return result;
		}
	}
}
