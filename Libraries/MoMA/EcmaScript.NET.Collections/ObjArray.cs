using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET.Collections
{
	[ComVisible(true)]
	public class ObjArray
	{
		private const int FIELDS_STORE_SIZE = 5;
		private int m_Size;
		private bool zealed;
		private object f0;
		private object f1;
		private object f2;
		private object f3;
		private object f4;
		private object[] data;
		public virtual bool Sealed
		{
			get
			{
				return this.zealed;
			}
		}
		public virtual bool Empty
		{
			get
			{
				return this.m_Size == 0;
			}
		}
		public virtual int Size
		{
			set
			{
				if (value < 0)
				{
					throw new ArgumentException();
				}
				if (this.zealed)
				{
					throw ObjArray.onSeledMutation();
				}
				int size = this.m_Size;
				if (value < size)
				{
					for (int num = value; num != size; num++)
					{
						this.SetImpl(num, null);
					}
				}
				else
				{
					if (value > size)
					{
						if (value > 5)
						{
							this.ensureCapacity(value);
						}
					}
				}
				this.m_Size = value;
			}
		}
		public void seal()
		{
			this.zealed = true;
		}
		public int size()
		{
			return this.m_Size;
		}
		public object Get(int index)
		{
			if (0 > index || index >= this.m_Size)
			{
				throw ObjArray.onInvalidIndex(index, this.m_Size);
			}
			return this.GetImpl(index);
		}
		public void Set(int index, object value)
		{
			if (0 > index || index >= this.m_Size)
			{
				throw ObjArray.onInvalidIndex(index, this.m_Size);
			}
			if (this.zealed)
			{
				throw ObjArray.onSeledMutation();
			}
			this.SetImpl(index, value);
		}
		private object GetImpl(int index)
		{
			object result;
			switch (index)
			{
			case 0:
				result = this.f0;
				break;
			case 1:
				result = this.f1;
				break;
			case 2:
				result = this.f2;
				break;
			case 3:
				result = this.f3;
				break;
			case 4:
				result = this.f4;
				break;
			default:
				result = this.data[index - 5];
				break;
			}
			return result;
		}
		private void SetImpl(int index, object value)
		{
			switch (index)
			{
			case 0:
				this.f0 = value;
				break;
			case 1:
				this.f1 = value;
				break;
			case 2:
				this.f2 = value;
				break;
			case 3:
				this.f3 = value;
				break;
			case 4:
				this.f4 = value;
				break;
			default:
				this.data[index - 5] = value;
				break;
			}
		}
		public virtual int indexOf(object obj)
		{
			int size = this.m_Size;
			int result;
			for (int num = 0; num != size; num++)
			{
				object impl = this.GetImpl(num);
				if (impl == obj || (impl != null && impl.Equals(obj)))
				{
					result = num;
					return result;
				}
			}
			result = -1;
			return result;
		}
		public virtual int lastIndexOf(object obj)
		{
			int num = this.m_Size;
			int result;
			while (num != 0)
			{
				num--;
				object impl = this.GetImpl(num);
				if (impl == obj || (impl != null && impl.Equals(obj)))
				{
					result = num;
					return result;
				}
			}
			result = -1;
			return result;
		}
		public object peek()
		{
			int size = this.m_Size;
			if (size == 0)
			{
				throw ObjArray.onEmptyStackTopRead();
			}
			return this.GetImpl(size - 1);
		}
		public object pop()
		{
			if (this.zealed)
			{
				throw ObjArray.onSeledMutation();
			}
			int num = this.m_Size;
			num--;
			object result;
			switch (num)
			{
			case -1:
				throw ObjArray.onEmptyStackTopRead();
			case 0:
				result = this.f0;
				this.f0 = null;
				break;
			case 1:
				result = this.f1;
				this.f1 = null;
				break;
			case 2:
				result = this.f2;
				this.f2 = null;
				break;
			case 3:
				result = this.f3;
				this.f3 = null;
				break;
			case 4:
				result = this.f4;
				this.f4 = null;
				break;
			default:
				result = this.data[num - 5];
				this.data[num - 5] = null;
				break;
			}
			this.m_Size = num;
			return result;
		}
		public void push(object value)
		{
			this.add(value);
		}
		public void add(object value)
		{
			if (this.zealed)
			{
				throw ObjArray.onSeledMutation();
			}
			int size = this.m_Size;
			if (size >= 5)
			{
				this.ensureCapacity(size + 1);
			}
			this.m_Size = size + 1;
			this.SetImpl(size, value);
		}
		public void add(int index, object value)
		{
			int size = this.m_Size;
			if (0 > index || index > size)
			{
				throw ObjArray.onInvalidIndex(index, size + 1);
			}
			if (this.zealed)
			{
				throw ObjArray.onSeledMutation();
			}
			object obj;
			switch (index)
			{
			case 0:
				if (size == 0)
				{
					this.f0 = value;
					goto IL_1A0;
				}
				obj = this.f0;
				this.f0 = value;
				value = obj;
				break;
			case 1:
				break;
			case 2:
				goto IL_C6;
			case 3:
				goto IL_F7;
			case 4:
				goto IL_128;
			default:
				goto IL_15C;
			}
			if (size == 1)
			{
				this.f1 = value;
				goto IL_1A0;
			}
			obj = this.f1;
			this.f1 = value;
			value = obj;
			IL_C6:
			if (size == 2)
			{
				this.f2 = value;
				goto IL_1A0;
			}
			obj = this.f2;
			this.f2 = value;
			value = obj;
			IL_F7:
			if (size == 3)
			{
				this.f3 = value;
				goto IL_1A0;
			}
			obj = this.f3;
			this.f3 = value;
			value = obj;
			IL_128:
			if (size == 4)
			{
				this.f4 = value;
				goto IL_1A0;
			}
			obj = this.f4;
			this.f4 = value;
			value = obj;
			index = 5;
			IL_15C:
			this.ensureCapacity(size + 1);
			if (index != size)
			{
				Array.Copy(this.data, index - 5, this.data, index - 5 + 1, size - index);
			}
			this.data[index - 5] = value;
			IL_1A0:
			this.m_Size = size + 1;
		}
		public void remove(int index)
		{
			int num = this.m_Size;
			if (0 > index || index >= num)
			{
				throw ObjArray.onInvalidIndex(index, num);
			}
			if (this.zealed)
			{
				throw ObjArray.onSeledMutation();
			}
			num--;
			switch (index)
			{
			case 0:
				if (num == 0)
				{
					this.f0 = null;
					goto IL_17E;
				}
				this.f0 = this.f1;
				break;
			case 1:
				break;
			case 2:
				goto IL_BB;
			case 3:
				goto IL_E7;
			case 4:
				goto IL_113;
			default:
				goto IL_144;
			}
			if (num == 1)
			{
				this.f1 = null;
				goto IL_17E;
			}
			this.f1 = this.f2;
			IL_BB:
			if (num == 2)
			{
				this.f2 = null;
				goto IL_17E;
			}
			this.f2 = this.f3;
			IL_E7:
			if (num == 3)
			{
				this.f3 = null;
				goto IL_17E;
			}
			this.f3 = this.f4;
			IL_113:
			if (num == 4)
			{
				this.f4 = null;
				goto IL_17E;
			}
			this.f4 = this.data[0];
			index = 5;
			IL_144:
			if (index != num)
			{
				Array.Copy(this.data, index - 5 + 1, this.data, index - 5, num - index);
			}
			this.data[num - 5] = null;
			IL_17E:
			this.m_Size = num;
		}
		public void clear()
		{
			if (this.zealed)
			{
				throw ObjArray.onSeledMutation();
			}
			int size = this.m_Size;
			for (int num = 0; num != size; num++)
			{
				this.SetImpl(num, null);
			}
			this.m_Size = 0;
		}
		public object[] ToArray()
		{
			object[] array = new object[this.m_Size];
			this.ToArray(array, 0);
			return array;
		}
		public void ToArray(object[] array)
		{
			this.ToArray(array, 0);
		}
		public void ToArray(object[] array, int offset)
		{
			int size = this.m_Size;
			switch (size)
			{
			case 0:
				return;
			case 1:
				goto IL_86;
			case 2:
				goto IL_76;
			case 3:
				goto IL_66;
			case 4:
				goto IL_56;
			case 5:
				break;
			default:
				Array.Copy(this.data, 0, array, offset + 5, size - 5);
				break;
			}
			array[offset + 4] = this.f4;
			IL_56:
			array[offset + 3] = this.f3;
			IL_66:
			array[offset + 2] = this.f2;
			IL_76:
			array[offset + 1] = this.f1;
			IL_86:
			array[offset] = this.f0;
		}
		private void ensureCapacity(int minimalCapacity)
		{
			int num = minimalCapacity - 5;
			if (num <= 0)
			{
				throw new ArgumentException();
			}
			if (this.data == null)
			{
				int num2 = 10;
				if (num2 < num)
				{
					num2 = num;
				}
				this.data = new object[num2];
			}
			else
			{
				int num2 = this.data.Length;
				if (num2 < num)
				{
					if (num2 <= 5)
					{
						num2 = 10;
					}
					else
					{
						num2 *= 2;
					}
					if (num2 < num)
					{
						num2 = num;
					}
					object[] destinationArray = new object[num2];
					if (this.m_Size > 5)
					{
						Array.Copy(this.data, 0, destinationArray, 0, this.m_Size - 5);
					}
					this.data = destinationArray;
				}
			}
		}
		private static ApplicationException onInvalidIndex(int index, int upperBound)
		{
			string message = string.Concat(new object[]
			{
				index,
				" ∉ [0, ",
				upperBound,
				')'
			});
			throw new IndexOutOfRangeException(message);
		}
		private static ApplicationException onEmptyStackTopRead()
		{
			throw new ApplicationException("Empty stack");
		}
		private static ApplicationException onSeledMutation()
		{
			throw new ApplicationException("Attempt to modify sealed array");
		}
	}
}
