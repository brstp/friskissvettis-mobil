using System;
using System.Collections;
namespace log4net.Util
{
	public sealed class NullDictionaryEnumerator : IDictionaryEnumerator, IEnumerator
	{
		private static readonly NullDictionaryEnumerator s_instance = new NullDictionaryEnumerator();
		public static NullDictionaryEnumerator Instance
		{
			get
			{
				return NullDictionaryEnumerator.s_instance;
			}
		}
		public object Current
		{
			get
			{
				throw new InvalidOperationException();
			}
		}
		public object Key
		{
			get
			{
				throw new InvalidOperationException();
			}
		}
		public object Value
		{
			get
			{
				throw new InvalidOperationException();
			}
		}
		public DictionaryEntry Entry
		{
			get
			{
				throw new InvalidOperationException();
			}
		}
		private NullDictionaryEnumerator()
		{
		}
		public bool MoveNext()
		{
			return false;
		}
		public void Reset()
		{
		}
	}
}
