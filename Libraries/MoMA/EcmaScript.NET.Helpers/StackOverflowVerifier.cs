using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET.Helpers
{
	[ComVisible(true)]
	public class StackOverflowVerifier : IDisposable
	{
		[ThreadStatic]
		private static long m_Counter = -9223372036854775808L;
		private int m_MaxStackSize = 0;
		public StackOverflowVerifier(int maxStackSize)
		{
			this.m_MaxStackSize = maxStackSize;
			this.ChangeStackDepth(1);
		}
		public void Dispose()
		{
			this.ChangeStackDepth(-1);
		}
		private void ChangeStackDepth(int offset)
		{
			if (StackOverflowVerifier.m_Counter == -9223372036854775808L)
			{
				StackOverflowVerifier.m_Counter = 0L;
			}
			StackOverflowVerifier.m_Counter += (long)offset;
			if (StackOverflowVerifier.m_Counter > (long)this.m_MaxStackSize)
			{
				throw new StackOverflowVerifierException();
			}
		}
	}
}
