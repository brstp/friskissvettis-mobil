using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
namespace ImageResizer.Collections
{
	[ComVisible(true)]
	public class ReverseEnumerator<T> : IEnumerator<T>, IDisposable, IEnumerator
	{
		private ReadOnlyCollection<T> _collection;
		private int curIndex;
		private T curItem;
		public T Current
		{
			get
			{
				return this.curItem;
			}
		}
		object IEnumerator.Current
		{
			get
			{
				return this.Current;
			}
		}
		public ReverseEnumerator(ReadOnlyCollection<T> collection)
		{
			this._collection = collection;
			this.curIndex = this._collection.Count;
			this.curItem = default(T);
		}
		public bool MoveNext()
		{
			this.curIndex--;
			if (this.curIndex < 0)
			{
				this.curItem = default(T);
				return false;
			}
			this.curItem = this._collection[this.curIndex];
			return true;
		}
		public void Reset()
		{
			this.curIndex = this._collection.Count;
			this.curItem = default(T);
		}
		void IDisposable.Dispose()
		{
		}
	}
}
