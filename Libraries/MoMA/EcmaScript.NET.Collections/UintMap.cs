using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET.Collections
{
	[ComVisible(true)]
	public class UintMap
	{
		private const int A = -1640531527;
		private const int EMPTY = -1;
		private const int DELETED = -2;
		private int[] keys;
		private object[] values;
		private int power;
		private int keyCount;
		private int occupiedCount;
		private int ivaluesShift;
		private static readonly bool check = false;
		public virtual bool Empty
		{
			get
			{
				return this.keyCount == 0;
			}
		}
		public virtual int[] Keys
		{
			get
			{
				int[] array = this.keys;
				int num = this.keyCount;
				int[] array2 = new int[num];
				int num2 = 0;
				while (num != 0)
				{
					int num3 = array[num2];
					if (num3 != -1 && num3 != -2)
					{
						array2[--num] = num3;
					}
					num2++;
				}
				return array2;
			}
		}
		public UintMap() : this(4)
		{
		}
		public UintMap(int initialCapacity)
		{
			if (initialCapacity < 0)
			{
				Context.CodeBug();
			}
			int num = initialCapacity * 4 / 3;
			int num2 = 2;
			while (1 << num2 < num)
			{
				num2++;
			}
			this.power = num2;
			if (UintMap.check && this.power < 2)
			{
				Context.CodeBug();
			}
		}
		public virtual int size()
		{
			return this.keyCount;
		}
		public virtual bool has(int key)
		{
			if (key < 0)
			{
				Context.CodeBug();
			}
			return 0 <= this.findIndex(key);
		}
		public virtual object getObject(int key)
		{
			if (key < 0)
			{
				Context.CodeBug();
			}
			object result;
			if (this.values != null)
			{
				int num = this.findIndex(key);
				if (0 <= num)
				{
					result = this.values[num];
					return result;
				}
			}
			result = null;
			return result;
		}
		public virtual int getInt(int key, int defaultValue)
		{
			if (key < 0)
			{
				Context.CodeBug();
			}
			int num = this.findIndex(key);
			int result;
			if (0 <= num)
			{
				if (this.ivaluesShift != 0)
				{
					result = this.keys[this.ivaluesShift + num];
				}
				else
				{
					result = 0;
				}
			}
			else
			{
				result = defaultValue;
			}
			return result;
		}
		public virtual int getExistingInt(int key)
		{
			if (key < 0)
			{
				Context.CodeBug();
			}
			int num = this.findIndex(key);
			int result;
			if (0 <= num)
			{
				if (this.ivaluesShift != 0)
				{
					result = this.keys[this.ivaluesShift + num];
				}
				else
				{
					result = 0;
				}
			}
			else
			{
				Context.CodeBug();
				result = 0;
			}
			return result;
		}
		public virtual void put(int key, object value)
		{
			if (key < 0)
			{
				Context.CodeBug();
			}
			int num = this.ensureIndex(key, false);
			if (this.values == null)
			{
				this.values = new object[1 << this.power];
			}
			this.values[num] = value;
		}
		public virtual void put(int key, int value)
		{
			if (key < 0)
			{
				Context.CodeBug();
			}
			int num = this.ensureIndex(key, true);
			if (this.ivaluesShift == 0)
			{
				int num2 = 1 << this.power;
				if (this.keys.Length != num2 * 2)
				{
					int[] destinationArray = new int[num2 * 2];
					Array.Copy(this.keys, 0, destinationArray, 0, num2);
					this.keys = destinationArray;
				}
				this.ivaluesShift = num2;
			}
			this.keys[this.ivaluesShift + num] = value;
		}
		public virtual void remove(int key)
		{
			if (key < 0)
			{
				Context.CodeBug();
			}
			int num = this.findIndex(key);
			if (0 <= num)
			{
				this.keys[num] = -2;
				this.keyCount--;
				if (this.values != null)
				{
					this.values[num] = null;
				}
				if (this.ivaluesShift != 0)
				{
					this.keys[this.ivaluesShift + num] = 0;
				}
			}
		}
		public virtual void clear()
		{
			int num = 1 << this.power;
			if (this.keys != null)
			{
				for (int num2 = 0; num2 != num; num2++)
				{
					this.keys[num2] = -1;
				}
				if (this.values != null)
				{
					for (int num2 = 0; num2 != num; num2++)
					{
						this.values[num2] = null;
					}
				}
			}
			this.ivaluesShift = 0;
			this.keyCount = 0;
			this.occupiedCount = 0;
		}
		private static int tableLookupStep(int fraction, int mask, int power)
		{
			int num = 32 - 2 * power;
			int result;
			if (num >= 0)
			{
				result = (int)(((uint)fraction >> num & (uint)mask) | 1u);
			}
			else
			{
				result = ((fraction & (int)((uint)mask >> -num)) | 1);
			}
			return result;
		}
		private int findIndex(int key)
		{
			int[] array = this.keys;
			int result;
			if (array != null)
			{
				int num = key * -1640531527;
				int num2 = (int)((uint)num >> 32 - this.power);
				int num3 = array[num2];
				if (num3 == key)
				{
					result = num2;
					return result;
				}
				if (num3 != -1)
				{
					int num4 = (1 << this.power) - 1;
					int num5 = UintMap.tableLookupStep(num, num4, this.power);
					int num6 = 0;
					while (true)
					{
						if (UintMap.check)
						{
							if (num6 >= this.occupiedCount)
							{
								Context.CodeBug();
							}
							num6++;
						}
						num2 = (num2 + num5 & num4);
						num3 = array[num2];
						if (num3 == key)
						{
							break;
						}
						if (num3 == -1)
						{
							goto IL_D9;
						}
					}
					result = num2;
					return result;
				}
				IL_D9:;
			}
			result = -1;
			return result;
		}
		private int insertNewKey(int key)
		{
			if (UintMap.check && this.occupiedCount != this.keyCount)
			{
				Context.CodeBug();
			}
			if (UintMap.check && this.keyCount == 1 << this.power)
			{
				Context.CodeBug();
			}
			int[] array = this.keys;
			int num = key * -1640531527;
			int num2 = (int)((uint)num >> 32 - this.power);
			if (array[num2] != -1)
			{
				int num3 = (1 << this.power) - 1;
				int num4 = UintMap.tableLookupStep(num, num3, this.power);
				int num5 = num2;
				do
				{
					if (UintMap.check && array[num2] == -2)
					{
						Context.CodeBug();
					}
					num2 = (num2 + num4 & num3);
					if (UintMap.check && num5 == num2)
					{
						Context.CodeBug();
					}
				}
				while (array[num2] != -1);
			}
			array[num2] = key;
			this.occupiedCount++;
			this.keyCount++;
			return num2;
		}
		private void rehashTable(bool ensureIntSpace)
		{
			if (this.keys != null)
			{
				if (this.keyCount * 2 >= this.occupiedCount)
				{
					this.power++;
				}
			}
			int num = 1 << this.power;
			int[] array = this.keys;
			int num2 = this.ivaluesShift;
			if (num2 == 0 && !ensureIntSpace)
			{
				this.keys = new int[num];
			}
			else
			{
				this.ivaluesShift = num;
				this.keys = new int[num * 2];
			}
			for (int num3 = 0; num3 != num; num3++)
			{
				this.keys[num3] = -1;
			}
			object[] array2 = this.values;
			if (array2 != null)
			{
				this.values = new object[num];
			}
			int num4 = this.keyCount;
			this.occupiedCount = 0;
			if (num4 != 0)
			{
				this.keyCount = 0;
				int num3 = 0;
				int num5 = num4;
				while (num5 != 0)
				{
					int num6 = array[num3];
					if (num6 != -1 && num6 != -2)
					{
						int num7 = this.insertNewKey(num6);
						if (array2 != null)
						{
							this.values[num7] = array2[num3];
						}
						if (num2 != 0)
						{
							this.keys[this.ivaluesShift + num7] = array[num2 + num3];
						}
						num5--;
					}
					num3++;
				}
			}
		}
		private int ensureIndex(int key, bool intType)
		{
			int num = -1;
			int num2 = -1;
			int[] array = this.keys;
			int result;
			if (array != null)
			{
				int num3 = key * -1640531527;
				num = (int)((uint)num3 >> 32 - this.power);
				int num4 = array[num];
				if (num4 == key)
				{
					result = num;
					return result;
				}
				if (num4 != -1)
				{
					if (num4 == -2)
					{
						num2 = num;
					}
					int num5 = (1 << this.power) - 1;
					int num6 = UintMap.tableLookupStep(num3, num5, this.power);
					int num7 = 0;
					while (true)
					{
						if (UintMap.check)
						{
							if (num7 >= this.occupiedCount)
							{
								Context.CodeBug();
							}
							num7++;
						}
						num = (num + num6 & num5);
						num4 = array[num];
						if (num4 == key)
						{
							break;
						}
						if (num4 == -2 && num2 < 0)
						{
							num2 = num;
						}
						if (num4 == -1)
						{
							goto IL_118;
						}
					}
					result = num;
					return result;
				}
				IL_118:;
			}
			if (UintMap.check && array != null && array[num] != -1)
			{
				Context.CodeBug();
			}
			if (num2 >= 0)
			{
				num = num2;
			}
			else
			{
				if (array == null || this.occupiedCount * 4 >= (1 << this.power) * 3)
				{
					this.rehashTable(intType);
					array = this.keys;
					result = this.insertNewKey(key);
					return result;
				}
				this.occupiedCount++;
			}
			array[num] = key;
			this.keyCount++;
			result = num;
			return result;
		}
	}
}
