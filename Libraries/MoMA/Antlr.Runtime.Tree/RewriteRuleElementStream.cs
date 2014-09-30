using System;
using System.Collections;
using System.Collections.Generic;
namespace Antlr.Runtime.Tree
{
	public abstract class RewriteRuleElementStream<T>
	{
		protected int cursor;
		protected T singleElement;
		protected IList<T> elements;
		protected bool dirty;
		protected string elementDescription;
		protected ITreeAdaptor adaptor;
		public int Count
		{
			get
			{
				if (this.singleElement != null)
				{
					return 1;
				}
				if (this.elements != null)
				{
					return this.elements.Count;
				}
				return 0;
			}
		}
		public string Description
		{
			get
			{
				return this.elementDescription;
			}
		}
		public RewriteRuleElementStream(ITreeAdaptor adaptor, string elementDescription)
		{
			this.elementDescription = elementDescription;
			this.adaptor = adaptor;
		}
		public RewriteRuleElementStream(ITreeAdaptor adaptor, string elementDescription, T oneElement) : this(adaptor, elementDescription)
		{
			this.Add(oneElement);
		}
		public RewriteRuleElementStream(ITreeAdaptor adaptor, string elementDescription, IList<T> elements) : this(adaptor, elementDescription)
		{
			this.singleElement = default(T);
			this.elements = elements;
		}
		[Obsolete("This constructor is for internal use only and might be phased out soon. Use instead the one with IList<T>.")]
		public RewriteRuleElementStream(ITreeAdaptor adaptor, string elementDescription, IList elements) : this(adaptor, elementDescription)
		{
			this.singleElement = default(T);
			this.elements = new List<T>();
			if (elements != null)
			{
				IEnumerator enumerator = elements.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						T item = (T)((object)enumerator.Current);
						this.elements.Add(item);
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
		}
		public void Add(T el)
		{
			if (el == null)
			{
				return;
			}
			if (this.elements != null)
			{
				this.elements.Add(el);
				return;
			}
			if (this.singleElement == null)
			{
				this.singleElement = el;
				return;
			}
			this.elements = new List<T>(5);
			this.elements.Add(this.singleElement);
			this.singleElement = default(T);
			this.elements.Add(el);
		}
		public virtual void Reset()
		{
			this.cursor = 0;
			this.dirty = true;
		}
		public bool HasNext()
		{
			return (this.singleElement != null && this.cursor < 1) || (this.elements != null && this.cursor < this.elements.Count);
		}
		public virtual object NextTree()
		{
			return this._Next();
		}
		protected object _Next()
		{
			int count = this.Count;
			if (count == 0)
			{
				throw new RewriteEmptyStreamException(this.elementDescription);
			}
			if (this.cursor >= count)
			{
				if (count == 1)
				{
					return this.ToTree(this.singleElement);
				}
				throw new RewriteCardinalityException(this.elementDescription);
			}
			else
			{
				if (this.singleElement != null)
				{
					this.cursor++;
					return this.ToTree(this.singleElement);
				}
				return this.ToTree(this.elements[this.cursor++]);
			}
		}
		protected virtual object ToTree(T el)
		{
			return el;
		}
		[Obsolete("Please use property Count instead.")]
		public int Size()
		{
			return this.Count;
		}
	}
}
