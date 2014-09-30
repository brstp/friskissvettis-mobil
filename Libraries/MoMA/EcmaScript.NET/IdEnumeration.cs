using EcmaScript.NET.Collections;
using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public class IdEnumeration
	{
		private IScriptable obj;
		private object[] ids;
		private int index;
		private ObjToIntMap used;
		private string currentId;
		private bool enumValues;
		protected IdEnumeration()
		{
		}
		public IdEnumeration(object value, Context cx, bool enumValues)
		{
			this.obj = ScriptConvert.ToObjectOrNull(cx, value);
			if (this.obj != null)
			{
				this.enumValues = enumValues;
				this.ChangeObject();
			}
		}
		public virtual bool MoveNext()
		{
			bool result;
			while (this.obj != null)
			{
				if (this.index == this.ids.Length)
				{
					this.obj = this.obj.GetPrototype();
					this.ChangeObject();
				}
				else
				{
					object obj = this.ids[this.index++];
					if (this.used == null || !this.used.has(obj))
					{
						if (obj is string)
						{
							string name = (string)obj;
							if (!this.obj.Has(name, this.obj))
							{
								continue;
							}
							this.currentId = name;
						}
						else
						{
							int value = Convert.ToInt32(obj);
							if (!this.obj.Has(value, this.obj))
							{
								continue;
							}
							this.currentId = Convert.ToString(value);
						}
						result = true;
						return result;
					}
				}
			}
			result = false;
			return result;
		}
		public virtual object Current(Context cx)
		{
			object result;
			if (!this.enumValues)
			{
				result = this.currentId;
			}
			else
			{
				string text = ScriptRuntime.ToStringIdOrIndex(cx, this.currentId);
				object obj;
				if (text == null)
				{
					int num = ScriptRuntime.lastIndexResult(cx);
					obj = this.obj.Get(num, this.obj);
				}
				else
				{
					obj = this.obj.Get(text, this.obj);
				}
				result = obj;
			}
			return result;
		}
		private void ChangeObject()
		{
			object[] array = null;
			while (this.obj != null)
			{
				array = this.obj.GetIds();
				if (array.Length != 0)
				{
					break;
				}
				this.obj = this.obj.GetPrototype();
			}
			if (this.obj != null && this.ids != null)
			{
				object[] array2 = this.ids;
				int num = array2.Length;
				if (this.used == null)
				{
					this.used = new ObjToIntMap(num);
				}
				for (int num2 = 0; num2 != num; num2++)
				{
					this.used.intern(array2[num2]);
				}
			}
			this.ids = array;
			this.index = 0;
		}
	}
}
