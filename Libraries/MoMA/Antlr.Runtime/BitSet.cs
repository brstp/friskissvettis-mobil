using System;
using System.Collections;
using System.Text;
namespace Antlr.Runtime
{
	public class BitSet
	{
		protected internal const int BITS = 64;
		protected internal const int LOG_BITS = 6;
		protected internal static readonly int MOD_MASK = 63;
		protected internal ulong[] bits;
		public virtual bool Nil
		{
			get
			{
				for (int i = this.bits.Length - 1; i >= 0; i--)
				{
					if (this.bits[i] != 0uL)
					{
						return false;
					}
				}
				return true;
			}
		}
		public virtual int Count
		{
			get
			{
				int num = 0;
				for (int i = this.bits.Length - 1; i >= 0; i--)
				{
					ulong num2 = this.bits[i];
					if (num2 != 0uL)
					{
						for (int j = 63; j >= 0; j--)
						{
							if ((num2 & 1uL << j) != 0uL)
							{
								num++;
							}
						}
					}
				}
				return num;
			}
		}
		public BitSet() : this(64)
		{
		}
		public BitSet(ulong[] bits_)
		{
			this.bits = bits_;
		}
		public BitSet(IList items) : this(64)
		{
			for (int i = 0; i < items.Count; i++)
			{
				int el = (int)items[i];
				this.Add(el);
			}
		}
		public BitSet(int nbits)
		{
			this.bits = new ulong[(nbits - 1 >> 6) + 1];
		}
		public static BitSet Of(int el)
		{
			BitSet bitSet = new BitSet(el + 1);
			bitSet.Add(el);
			return bitSet;
		}
		public static BitSet Of(int a, int b)
		{
			BitSet bitSet = new BitSet(Math.Max(a, b) + 1);
			bitSet.Add(a);
			bitSet.Add(b);
			return bitSet;
		}
		public static BitSet Of(int a, int b, int c)
		{
			BitSet bitSet = new BitSet();
			bitSet.Add(a);
			bitSet.Add(b);
			bitSet.Add(c);
			return bitSet;
		}
		public static BitSet Of(int a, int b, int c, int d)
		{
			BitSet bitSet = new BitSet();
			bitSet.Add(a);
			bitSet.Add(b);
			bitSet.Add(c);
			bitSet.Add(d);
			return bitSet;
		}
		public virtual BitSet Or(BitSet a)
		{
			if (a == null)
			{
				return this;
			}
			BitSet bitSet = (BitSet)this.Clone();
			bitSet.OrInPlace(a);
			return bitSet;
		}
		public virtual void Add(int el)
		{
			int num = BitSet.WordNumber(el);
			if (num >= this.bits.Length)
			{
				this.GrowToInclude(el);
			}
			this.bits[num] |= BitSet.BitMask(el);
		}
		public virtual void GrowToInclude(int bit)
		{
			int num = Math.Max(this.bits.Length << 1, this.NumWordsToHold(bit));
			ulong[] destinationArray = new ulong[num];
			Array.Copy(this.bits, 0, destinationArray, 0, this.bits.Length);
			this.bits = destinationArray;
		}
		public virtual void OrInPlace(BitSet a)
		{
			if (a == null)
			{
				return;
			}
			if (a.bits.Length > this.bits.Length)
			{
				this.SetSize(a.bits.Length);
			}
			int num = Math.Min(this.bits.Length, a.bits.Length);
			for (int i = num - 1; i >= 0; i--)
			{
				this.bits[i] |= a.bits[i];
			}
		}
		public virtual object Clone()
		{
			BitSet bitSet;
			try
			{
				bitSet = (BitSet)base.MemberwiseClone();
				bitSet.bits = new ulong[this.bits.Length];
				Array.Copy(this.bits, 0, bitSet.bits, 0, this.bits.Length);
			}
			catch (Exception innerException)
			{
				throw new InvalidOperationException("Unable to clone BitSet", innerException);
			}
			return bitSet;
		}
		public virtual bool Member(int el)
		{
			if (el < 0)
			{
				return false;
			}
			int num = BitSet.WordNumber(el);
			return num < this.bits.Length && (this.bits[num] & BitSet.BitMask(el)) != 0uL;
		}
		public virtual void Remove(int el)
		{
			int num = BitSet.WordNumber(el);
			if (num < this.bits.Length)
			{
				this.bits[num] &= ~BitSet.BitMask(el);
			}
		}
		public virtual int NumBits()
		{
			return this.bits.Length << 6;
		}
		public virtual int LengthInLongWords()
		{
			return this.bits.Length;
		}
		public virtual int[] ToArray()
		{
			int[] array = new int[this.Count];
			int num = 0;
			for (int i = 0; i < this.bits.Length << 6; i++)
			{
				if (this.Member(i))
				{
					array[num++] = i;
				}
			}
			return array;
		}
		public virtual ulong[] ToPackedArray()
		{
			return this.bits;
		}
		private static int WordNumber(int bit)
		{
			return bit >> 6;
		}
		public override string ToString()
		{
			return this.ToString(null);
		}
		public virtual string ToString(string[] tokenNames)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string value = ",";
			bool flag = false;
			stringBuilder.Append('{');
			for (int i = 0; i < this.bits.Length << 6; i++)
			{
				if (this.Member(i))
				{
					if (i > 0 && flag)
					{
						stringBuilder.Append(value);
					}
					if (tokenNames != null)
					{
						stringBuilder.Append(tokenNames[i]);
					}
					else
					{
						stringBuilder.Append(i);
					}
					flag = true;
				}
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}
		public override bool Equals(object other)
		{
			if (other == null || !(other is BitSet))
			{
				return false;
			}
			BitSet bitSet = (BitSet)other;
			int num = Math.Min(this.bits.Length, bitSet.bits.Length);
			for (int i = 0; i < num; i++)
			{
				if (this.bits[i] != bitSet.bits[i])
				{
					return false;
				}
			}
			if (this.bits.Length > num)
			{
				for (int j = num + 1; j < this.bits.Length; j++)
				{
					if (this.bits[j] != 0uL)
					{
						return false;
					}
				}
			}
			else
			{
				if (bitSet.bits.Length > num)
				{
					for (int k = num + 1; k < bitSet.bits.Length; k++)
					{
						if (bitSet.bits[k] != 0uL)
						{
							return false;
						}
					}
				}
			}
			return true;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		private static ulong BitMask(int bitNumber)
		{
			int num = bitNumber & BitSet.MOD_MASK;
			return 1uL << num;
		}
		private void SetSize(int nwords)
		{
			ulong[] destinationArray = new ulong[nwords];
			int length = Math.Min(nwords, this.bits.Length);
			Array.Copy(this.bits, 0, destinationArray, 0, length);
			this.bits = destinationArray;
		}
		private int NumWordsToHold(int el)
		{
			return (el >> 6) + 1;
		}
	}
}
