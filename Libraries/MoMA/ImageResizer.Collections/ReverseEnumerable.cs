using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
namespace ImageResizer.Collections
{
	[ComVisible(true)]
	public class ReverseEnumerable<T> : IEnumerable<T>, IEnumerable
	{
		private ReadOnlyCollection<T> _collection;
		public ReverseEnumerable(ReadOnlyCollection<T> collection)
		{
			this._collection = collection;
		}
		public IEnumerator<T> GetEnumerator()
		{
			return new ReverseEnumerator<T>(this._collection);
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new ReverseEnumerator<T>(this._collection);
		}
	}
}
