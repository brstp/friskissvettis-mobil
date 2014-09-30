using log4net.Core;
using System;
using System.Collections;
namespace log4net.Util
{
	public sealed class ThreadContextStack : IFixingRequired
	{
		private sealed class StackFrame
		{
			private readonly string m_message;
			private readonly ThreadContextStack.StackFrame m_parent;
			private string m_fullMessage = null;
			internal string Message
			{
				get
				{
					return this.m_message;
				}
			}
			internal string FullMessage
			{
				get
				{
					if (this.m_fullMessage == null && this.m_parent != null)
					{
						this.m_fullMessage = this.m_parent.FullMessage + " " + this.m_message;
					}
					return this.m_fullMessage;
				}
			}
			internal StackFrame(string message, ThreadContextStack.StackFrame parent)
			{
				this.m_message = message;
				this.m_parent = parent;
				if (parent == null)
				{
					this.m_fullMessage = message;
				}
			}
		}
		private struct AutoPopStackFrame : IDisposable
		{
			private Stack m_frameStack;
			private int m_frameDepth;
			internal AutoPopStackFrame(Stack frameStack, int frameDepth)
			{
				this.m_frameStack = frameStack;
				this.m_frameDepth = frameDepth;
			}
			public void Dispose()
			{
				if (this.m_frameDepth >= 0 && this.m_frameStack != null)
				{
					while (this.m_frameStack.Count > this.m_frameDepth)
					{
						this.m_frameStack.Pop();
					}
				}
			}
		}
		private Stack m_stack = new Stack();
		public int Count
		{
			get
			{
				return this.m_stack.Count;
			}
		}
		internal Stack InternalStack
		{
			get
			{
				return this.m_stack;
			}
			set
			{
				this.m_stack = value;
			}
		}
		internal ThreadContextStack()
		{
		}
		public void Clear()
		{
			this.m_stack.Clear();
		}
		public string Pop()
		{
			Stack stack = this.m_stack;
			string result;
			if (stack.Count > 0)
			{
				result = ((ThreadContextStack.StackFrame)stack.Pop()).Message;
			}
			else
			{
				result = "";
			}
			return result;
		}
		public IDisposable Push(string message)
		{
			Stack stack = this.m_stack;
			stack.Push(new ThreadContextStack.StackFrame(message, (stack.Count > 0) ? ((ThreadContextStack.StackFrame)stack.Peek()) : null));
			return new ThreadContextStack.AutoPopStackFrame(stack, stack.Count - 1);
		}
		internal string GetFullMessage()
		{
			Stack stack = this.m_stack;
			string result;
			if (stack.Count > 0)
			{
				result = ((ThreadContextStack.StackFrame)stack.Peek()).FullMessage;
			}
			else
			{
				result = null;
			}
			return result;
		}
		public override string ToString()
		{
			return this.GetFullMessage();
		}
		object IFixingRequired.GetFixedObject()
		{
			return this.GetFullMessage();
		}
	}
}
