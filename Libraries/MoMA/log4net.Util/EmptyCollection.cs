using System;
using System.Collections;
namespace log4net.Util
{
	[Serializable]
	public sealed class EmptyCollection : ICollection, IEnumerable
	{
		private static readonly EmptyCollection s_instance = new EmptyCollection();
		public static EmptyCollection Instance
		{
			get
			{
				return EmptyCollection.s_instance;
			}
		}
		public bool IsSynchronized
		{
			get
			{
				return true;
			}
		}
		public int Count
		{
			get
			{
				return 0;
			}
		}
		public object SyncRoot
		{
			get
			{
				return this;
			}
		}
		private EmptyCollection()
		{
		}
		public void CopyTo(Array array, int index)
		{
		}
		public IEnumerator GetEnumerator()
		{
			return NullEnumerator.Instance;
		}
	}
}
