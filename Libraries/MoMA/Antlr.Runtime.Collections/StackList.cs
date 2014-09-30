using System;
using System.Collections.Generic;
namespace Antlr.Runtime.Collections
{
	public class StackList : List<object>
	{
		public void Push(object item)
		{
			base.Add(item);
		}
		public object Pop()
		{
			object result = base[base.Count - 1];
			base.RemoveAt(base.Count - 1);
			return result;
		}
		public object Peek()
		{
			return base[base.Count - 1];
		}
	}
}
