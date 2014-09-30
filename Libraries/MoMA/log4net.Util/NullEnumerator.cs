using System;
using System.Collections;
namespace log4net.Util
{
	public sealed class NullEnumerator : IEnumerator
	{
		private static readonly NullEnumerator s_instance = new NullEnumerator();
		public static NullEnumerator Instance
		{
			get
			{
				return NullEnumerator.s_instance;
			}
		}
		public object Current
		{
			get
			{
				throw new InvalidOperationException();
			}
		}
		private NullEnumerator()
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
