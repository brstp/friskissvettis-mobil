using Antlr.Runtime.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
namespace Antlr.Runtime.Tree
{
	public class CommonTreeNodeStream : ITreeNodeStream, IIntStream, IEnumerable
	{
		protected sealed class CommonTreeNodeStreamEnumerator : IEnumerator
		{
			private CommonTreeNodeStream _nodeStream;
			private int _index;
			private object _currentItem;
			public object Current
			{
				get
				{
					if (this._currentItem == null)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					return this._currentItem;
				}
			}
			internal CommonTreeNodeStreamEnumerator()
			{
			}
			internal CommonTreeNodeStreamEnumerator(CommonTreeNodeStream nodeStream)
			{
				this._nodeStream = nodeStream;
				this.Reset();
			}
			public void Reset()
			{
				this._index = 0;
				this._currentItem = null;
			}
			public bool MoveNext()
			{
				if (this._index >= this._nodeStream.nodes.Count)
				{
					int index = this._index;
					this._index++;
					if (index < this._nodeStream.nodes.Count)
					{
						this._currentItem = this._nodeStream.nodes[index];
					}
					this._currentItem = this._nodeStream.eof;
					return true;
				}
				this._currentItem = null;
				return false;
			}
		}
		public const int DEFAULT_INITIAL_BUFFER_SIZE = 100;
		public const int INITIAL_CALL_STACK_SIZE = 10;
		protected object down;
		protected object up;
		protected object eof;
		protected IList nodes;
		protected internal object root;
		protected ITokenStream tokens;
		private ITreeAdaptor adaptor;
		protected bool uniqueNavigationNodes;
		protected int p = -1;
		protected int lastMarker;
		protected StackList calls;
		public virtual object CurrentSymbol
		{
			get
			{
				return this.LT(1);
			}
		}
		public virtual object TreeSource
		{
			get
			{
				return this.root;
			}
		}
		public virtual string SourceName
		{
			get
			{
				return this.TokenStream.SourceName;
			}
		}
		public virtual ITokenStream TokenStream
		{
			get
			{
				return this.tokens;
			}
			set
			{
				this.tokens = value;
			}
		}
		public ITreeAdaptor TreeAdaptor
		{
			get
			{
				return this.adaptor;
			}
			set
			{
				this.adaptor = value;
			}
		}
		public bool HasUniqueNavigationNodes
		{
			get
			{
				return this.uniqueNavigationNodes;
			}
			set
			{
				this.uniqueNavigationNodes = value;
			}
		}
		public virtual int Count
		{
			get
			{
				if (this.p == -1)
				{
					this.FillBuffer();
				}
				return this.nodes.Count;
			}
		}
		public IEnumerator GetEnumerator()
		{
			if (this.p == -1)
			{
				this.FillBuffer();
			}
			return new CommonTreeNodeStream.CommonTreeNodeStreamEnumerator(this);
		}
		public CommonTreeNodeStream(object tree) : this(new CommonTreeAdaptor(), tree)
		{
		}
		public CommonTreeNodeStream(ITreeAdaptor adaptor, object tree) : this(adaptor, tree, 100)
		{
		}
		public CommonTreeNodeStream(ITreeAdaptor adaptor, object tree, int initialBufferSize)
		{
			this.root = tree;
			this.adaptor = adaptor;
			this.nodes = new List<object>(initialBufferSize);
			this.down = adaptor.Create(2, "DOWN");
			this.up = adaptor.Create(3, "UP");
			this.eof = adaptor.Create(Token.EOF, "EOF");
		}
		protected void FillBuffer()
		{
			this.FillBuffer(this.root);
			this.p = 0;
		}
		public void FillBuffer(object t)
		{
			bool flag = this.adaptor.IsNil(t);
			if (!flag)
			{
				this.nodes.Add(t);
			}
			int childCount = this.adaptor.GetChildCount(t);
			if (!flag && childCount > 0)
			{
				this.AddNavigationNode(2);
			}
			for (int i = 0; i < childCount; i++)
			{
				object child = this.adaptor.GetChild(t, i);
				this.FillBuffer(child);
			}
			if (!flag && childCount > 0)
			{
				this.AddNavigationNode(3);
			}
		}
		protected int GetNodeIndex(object node)
		{
			if (this.p == -1)
			{
				this.FillBuffer();
			}
			for (int i = 0; i < this.nodes.Count; i++)
			{
				object obj = this.nodes[i];
				if (obj == node)
				{
					return i;
				}
			}
			return -1;
		}
		protected void AddNavigationNode(int ttype)
		{
			object value;
			if (ttype == 2)
			{
				if (this.HasUniqueNavigationNodes)
				{
					value = this.adaptor.Create(2, "DOWN");
				}
				else
				{
					value = this.down;
				}
			}
			else
			{
				if (this.HasUniqueNavigationNodes)
				{
					value = this.adaptor.Create(3, "UP");
				}
				else
				{
					value = this.up;
				}
			}
			this.nodes.Add(value);
		}
		public object Get(int i)
		{
			if (this.p == -1)
			{
				this.FillBuffer();
			}
			return this.nodes[i];
		}
		public object LT(int k)
		{
			if (this.p == -1)
			{
				this.FillBuffer();
			}
			if (k == 0)
			{
				return null;
			}
			if (k < 0)
			{
				return this.LB(-k);
			}
			if (this.p + k - 1 >= this.nodes.Count)
			{
				return this.eof;
			}
			return this.nodes[this.p + k - 1];
		}
		protected object LB(int k)
		{
			if (k == 0)
			{
				return null;
			}
			if (this.p - k < 0)
			{
				return null;
			}
			return this.nodes[this.p - k];
		}
		public void Push(int index)
		{
			if (this.calls == null)
			{
				this.calls = new StackList();
			}
			this.calls.Push(this.p);
			this.Seek(index);
		}
		public int Pop()
		{
			int num = (int)this.calls.Pop();
			this.Seek(num);
			return num;
		}
		public void Reset()
		{
			this.p = -1;
			this.lastMarker = 0;
			if (this.calls != null)
			{
				this.calls.Clear();
			}
		}
		public void ReplaceChildren(object parent, int startChildIndex, int stopChildIndex, object t)
		{
			if (parent != null)
			{
				this.adaptor.ReplaceChildren(parent, startChildIndex, stopChildIndex, t);
			}
		}
		public virtual void Consume()
		{
			if (this.p == -1)
			{
				this.FillBuffer();
			}
			this.p++;
		}
		public virtual int LA(int i)
		{
			return this.adaptor.GetNodeType(this.LT(i));
		}
		public virtual int Mark()
		{
			if (this.p == -1)
			{
				this.FillBuffer();
			}
			this.lastMarker = this.Index();
			return this.lastMarker;
		}
		public virtual void Release(int marker)
		{
		}
		public virtual void Rewind(int marker)
		{
			this.Seek(marker);
		}
		public void Rewind()
		{
			this.Seek(this.lastMarker);
		}
		public virtual void Seek(int index)
		{
			if (this.p == -1)
			{
				this.FillBuffer();
			}
			this.p = index;
		}
		public virtual int Index()
		{
			return this.p;
		}
		[Obsolete("Please use property Count instead.")]
		public virtual int Size()
		{
			return this.Count;
		}
		public override string ToString()
		{
			if (this.p == -1)
			{
				this.FillBuffer();
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.nodes.Count; i++)
			{
				object t = this.nodes[i];
				stringBuilder.Append(" ");
				stringBuilder.Append(this.adaptor.GetNodeType(t));
			}
			return stringBuilder.ToString();
		}
		public string ToTokenString(int start, int stop)
		{
			if (this.p == -1)
			{
				this.FillBuffer();
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = start;
			while (num < this.nodes.Count && num <= stop)
			{
				object treeNode = this.nodes[num];
				stringBuilder.Append(" ");
				stringBuilder.Append(this.adaptor.GetToken(treeNode));
				num++;
			}
			return stringBuilder.ToString();
		}
		public virtual string ToString(object start, object stop)
		{
			Console.Out.WriteLine("ToString");
			if (start == null || stop == null)
			{
				return null;
			}
			if (this.p == -1)
			{
				this.FillBuffer();
			}
			if (start is CommonTree)
			{
				Console.Out.Write("ToString: " + ((CommonTree)start).Token + ", ");
			}
			else
			{
				Console.Out.WriteLine(start);
			}
			if (stop is CommonTree)
			{
				Console.Out.WriteLine(((CommonTree)stop).Token);
			}
			else
			{
				Console.Out.WriteLine(stop);
			}
			if (this.tokens != null)
			{
				int tokenStartIndex = this.adaptor.GetTokenStartIndex(start);
				int stop2 = this.adaptor.GetTokenStopIndex(stop);
				if (this.adaptor.GetNodeType(stop) == 3)
				{
					stop2 = this.adaptor.GetTokenStopIndex(start);
				}
				else
				{
					if (this.adaptor.GetNodeType(stop) == Token.EOF)
					{
						stop2 = this.Count - 2;
					}
				}
				return this.tokens.ToString(tokenStartIndex, stop2);
			}
			int i;
			for (i = 0; i < this.nodes.Count; i++)
			{
				object obj = this.nodes[i];
				if (obj == start)
				{
					break;
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			string text;
			for (object obj = this.nodes[i]; obj != stop; obj = this.nodes[i])
			{
				text = this.adaptor.GetNodeText(obj);
				if (text == null)
				{
					text = " " + this.adaptor.GetNodeType(obj);
				}
				stringBuilder.Append(text);
				i++;
			}
			text = this.adaptor.GetNodeText(stop);
			if (text == null)
			{
				text = " " + this.adaptor.GetNodeType(stop);
			}
			stringBuilder.Append(text);
			return stringBuilder.ToString();
		}
	}
}
