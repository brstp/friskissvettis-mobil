using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public class ContextFactory
	{
		private static volatile bool hasCustomGlobal;
		private static ContextFactory global = new ContextFactory();
		private volatile bool zealed;
		public event ContextEventHandler OnContextCreated;
		public event ContextEventHandler OnContextReleased;
		public static ContextFactory Global
		{
			get
			{
				return ContextFactory.global;
			}
		}
		public virtual bool Sealed
		{
			get
			{
				return this.zealed;
			}
		}
		public static bool HasExplicitGlobal
		{
			get
			{
				return ContextFactory.hasCustomGlobal;
			}
		}
		public static void InitGlobal(ContextFactory factory)
		{
			if (factory == null)
			{
				throw new ArgumentException();
			}
			if (ContextFactory.hasCustomGlobal)
			{
				throw new ApplicationException();
			}
			ContextFactory.hasCustomGlobal = true;
			ContextFactory.global = factory;
		}
		protected internal virtual object DoTopCall(ICallable callable, Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			return callable.Call(cx, scope, thisObj, args);
		}
		protected internal virtual void ObserveInstructionCount(Context cx, int instructionCount)
		{
		}
		protected internal virtual void FireOnContextCreated(Context cx)
		{
			if (this.OnContextCreated != null)
			{
				this.OnContextCreated(this, new ContextEventArgs(cx));
			}
		}
		protected internal virtual void FireOnContextReleased(Context cx)
		{
			if (this.OnContextReleased != null)
			{
				this.OnContextReleased(this, new ContextEventArgs(cx));
			}
		}
		public void Seal()
		{
			this.CheckNotSealed();
			this.zealed = true;
		}
		protected internal void CheckNotSealed()
		{
			if (this.zealed)
			{
				throw new ApplicationException();
			}
		}
	}
}
