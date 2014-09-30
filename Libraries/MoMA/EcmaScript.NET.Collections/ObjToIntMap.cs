using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET.Collections
{
	[ComVisible(true)]
	public class ObjToIntMap
	{
		public class Iterator
		{
			internal ObjToIntMap master;
			private int cursor;
			private int remaining;
			private object[] keys;
			private int[] values;
			public virtual object Key
			{
				get
				{
					object obj = this.keys[this.cursor];
					if (obj == UniqueTag.NullValue)
					{
						obj = null;
					}
					return obj;
				}
			}
			public virtual int Value
			{
				get
				{
					return this.values[this.cursor];
				}
				set
				{
					this.values[this.cursor] = value;
				}
			}
			internal Iterator(ObjToIntMap master)
			{
				this.master = master;
			}
			internal void Init(object[] keys, int[] values, int keyCount)
			{
				this.keys = keys;
				this.values = values;
				this.cursor = -1;
				this.remaining = keyCount;
			}
			public virtual void start()
			{
				this.master.initIterator(this);
				this.next();
			}
			public virtual bool done()
			{
				return this.remaining < 0;
			}
			public virtual void next()
			{
				if (this.remaining == -1)
				{
					Context.CodeBug();
				}
				if (this.remaining == 0)
				{
					this.remaining = -1;
					this.cursor = -1;
				}
				else
				{
					this.cursor++;
					while (true)
					{
						object obj = this.keys[this.cursor];
						if (obj != null && obj != ObjToIntMap.DELETED)
						{
							break;
						}
						this.cursor++;
					}
					this.remaining--;
				}
			}
		}
		private const int A = -1640531527;
		private static readonly object DELETED = new object();
		private object[] keys;
		private int[] values;
		private int power;
		private int keyCount;
		private int occupiedCount;
		private static readonly bool check = false;
		public virtual bool Empty
		{
			get
			{
				return this.keyCount == 0;
			}
		}
		public ObjToIntMap() : this(4)
		{
		}
		public ObjToIntMap(int keyCountHint)
		{
			if (keyCountHint < 0)
			{
				Context.CodeBug();
			}
			int num = keyCountHint * 4 / 3;
			int num2 = 2;
			while (1 << num2 < num)
			{
				num2++;
			}
			this.power = num2;
			if (ObjToIntMap.check && this.power < 2)
			{
				Context.CodeBug();
			}
		}
		public virtual int size()
		{
			return this.keyCount;
		}
		public virtual bool has(object key)
		{
			if (key == null)
			{
				key = UniqueTag.NullValue;
			}
			return 0 <= this.findIndex(key);
		}
		public virtual int Get(object key, int defaultValue)
		{
			if (key == null)
			{
				key = UniqueTag.NullValue;
			}
			int num = this.findIndex(key);
			int result;
			if (0 <= num)
			{
				result = this.values[num];
			}
			else
			{
				result = defaultValue;
			}
			return result;
		}
		public virtual int getExisting(object key)
		{
			if (key == null)
			{
				key = UniqueTag.NullValue;
			}
			int num = this.findIndex(key);
			int result;
			if (0 <= num)
			{
				result = this.values[num];
			}
			else
			{
				Context.CodeBug();
				result = 0;
			}
			return result;
		}
		public virtual void put(object key, int value)
		{
			if (key == null)
			{
				key = UniqueTag.NullValue;
			}
			int num = this.ensureIndex(key);
			this.values[num] = value;
		}
		public virtual object intern(object keyArg)
		{
			bool flag = false;
			if (keyArg == null)
			{
				flag = true;
				keyArg = UniqueTag.NullValue;
			}
			int num = this.ensureIndex(keyArg);
			this.values[num] = 0;
			return flag ? null : this.keys[num];
		}
		public virtual void remove(object key)
		{
			if (key == null)
			{
				key = UniqueTag.NullValue;
			}
			int num = this.findIndex(key);
			if (0 <= num)
			{
				this.keys[num] = ObjToIntMap.DELETED;
				this.keyCount--;
			}
		}
		public virtual void clear()
		{
			int num = this.keys.Length;
			while (num != 0)
			{
				this.keys[--num] = null;
			}
			this.keyCount = 0;
			this.occupiedCount = 0;
		}
		public virtual ObjToIntMap.Iterator newIterator()
		{
			return new ObjToIntMap.Iterator(this);
		}
		internal void initIterator(ObjToIntMap.Iterator i)
		{
			i.Init(this.keys, this.values, this.keyCount);
		}
		public virtual object[] getKeys()
		{
			object[] array = new object[this.keyCount];
			this.getKeys(array, 0);
			return array;
		}
		public virtual void getKeys(object[] array, int offset)
		{
			int num = this.keyCount;
			int num2 = 0;
			while (num != 0)
			{
				object obj = this.keys[num2];
				if (obj != null && obj != ObjToIntMap.DELETED)
				{
					if (obj == UniqueTag.NullValue)
					{
						obj = null;
					}
					array[offset] = obj;
					offset++;
					num--;
				}
				num2++;
			}
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
		private int findIndex(object key)
		{
			int result;
			if (this.keys != null)
			{
				int hashCode = key.GetHashCode();
				int num = hashCode * -1640531527;
				int num2 = (int)((uint)num >> 32 - this.power);
				object obj = this.keys[num2];
				if (obj != null)
				{
					int num3 = 1 << this.power;
					if (obj == key || (this.values[num3 + num2] == hashCode && obj.Equals(key)))
					{
						result = num2;
						return result;
					}
					int num4 = num3 - 1;
					int num5 = ObjToIntMap.tableLookupStep(num, num4, this.power);
					int num6 = 0;
					while (true)
					{
						if (ObjToIntMap.check)
						{
							if (num6 >= this.occupiedCount)
							{
								Context.CodeBug();
							}
							num6++;
						}
						num2 = (num2 + num5 & num4);
						obj = this.keys[num2];
						if (obj == null)
						{
							break;
						}
						if (obj == key || (this.values[num3 + num2] == hashCode && obj.Equals(key)))
						{
							goto Block_11;
						}
					}
					goto IL_151;
					Block_11:
					result = num2;
					return result;
				}
				IL_151:;
			}
			result = -1;
			return result;
		}
		private int insertNewKey(object key, int hash)
		{
			if (ObjToIntMap.check && this.occupiedCount != this.keyCount)
			{
				Context.CodeBug();
			}
			if (ObjToIntMap.check && this.keyCount == 1 << this.power)
			{
				Context.CodeBug();
			}
			int num = hash * -1640531527;
			int num2 = (int)((uint)num >> 32 - this.power);
			int num3 = 1 << this.power;
			if (this.keys[num2] != null)
			{
				int num4 = num3 - 1;
				int num5 = ObjToIntMap.tableLookupStep(num, num4, this.power);
				int num6 = num2;
				do
				{
					if (ObjToIntMap.check && this.keys[num2] == ObjToIntMap.DELETED)
					{
						Context.CodeBug();
					}
					num2 = (num2 + num5 & num4);
					if (ObjToIntMap.check && num6 == num2)
					{
						Context.CodeBug();
					}
				}
				while (this.keys[num2] != null);
			}
			this.keys[num2] = key;
			this.values[num3 + num2] = hash;
			this.occupiedCount++;
			this.keyCount++;
			return num2;
		}
		private void rehashTable()
		{
			if (this.keys == null)
			{
				if (ObjToIntMap.check && this.keyCount != 0)
				{
					Context.CodeBug();
				}
				if (ObjToIntMap.check && this.occupiedCount != 0)
				{
					Context.CodeBug();
				}
				int num = 1 << this.power;
				this.keys = new object[num];
				this.values = new int[2 * num];
			}
			else
			{
				if (this.keyCount * 2 >= this.occupiedCount)
				{
					this.power++;
				}
				int num = 1 << this.power;
				object[] array = this.keys;
				int[] array2 = this.values;
				int num2 = array.Length;
				this.keys = new object[num];
				this.values = new int[2 * num];
				int num3 = this.keyCount;
				this.occupiedCount = (this.keyCount = 0);
				int num4 = 0;
				while (num3 != 0)
				{
					object obj = array[num4];
					if (obj != null && obj != ObjToIntMap.DELETED)
					{
						int hash = array2[num2 + num4];
						int num5 = this.insertNewKey(obj, hash);
						this.values[num5] = array2[num4];
						num3--;
					}
					num4++;
				}
			}
		}
		private int ensureIndex(object key)
		{
			int hashCode = key.GetHashCode();
			int num = -1;
			int num2 = -1;
			int result;
			if (this.keys != null)
			{
				int num3 = hashCode * -1640531527;
				num = (int)((uint)num3 >> 32 - this.power);
				object obj = this.keys[num];
				if (obj != null)
				{
					int num4 = 1 << this.power;
					if (obj == key || (this.values[num4 + num] == hashCode && obj.Equals(key)))
					{
						result = num;
						return result;
					}
					if (obj == ObjToIntMap.DELETED)
					{
						num2 = num;
					}
					int num5 = num4 - 1;
					int num6 = ObjToIntMap.tableLookupStep(num3, num5, this.power);
					int num7 = 0;
					while (true)
					{
						if (ObjToIntMap.check)
						{
							if (num7 >= this.occupiedCount)
							{
								Context.CodeBug();
							}
							num7++;
						}
						num = (num + num6 & num5);
						obj = this.keys[num];
						if (obj == null)
						{
							break;
						}
						if (obj == key || (this.values[num4 + num] == hashCode && obj.Equals(key)))
						{
							goto Block_12;
						}
						if (obj == ObjToIntMap.DELETED && num2 < 0)
						{
							num2 = num;
						}
					}
					goto IL_193;
					Block_12:
					result = num;
					return result;
				}
				IL_193:;
			}
			if (ObjToIntMap.check && this.keys != null && this.keys[num] != null)
			{
				Context.CodeBug();
			}
			if (num2 >= 0)
			{
				num = num2;
			}
			else
			{
				if (this.keys == null || this.occupiedCount * 4 >= (1 << this.power) * 3)
				{
					this.rehashTable();
					result = this.insertNewKey(key, hashCode);
					return result;
				}
				this.occupiedCount++;
			}
			this.keys[num] = key;
			this.values[(1 << this.power) + num] = hashCode;
			this.keyCount++;
			result = num;
			return result;
		}
	}
}
