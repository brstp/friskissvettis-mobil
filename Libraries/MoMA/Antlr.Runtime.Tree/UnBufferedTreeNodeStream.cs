using Antlr.Runtime.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
namespace Antlr.Runtime.Tree
{
	public class UnBufferedTreeNodeStream : ITreeNodeStream, IIntStream
	{
		protected class TreeWalkState
		{
			protected internal int currentChildIndex;
			protected internal int absoluteNodeIndex;
			protected internal object currentNode;
			protected internal object previousNode;
			protected internal int nodeStackSize;
			protected internal int indexStackSize;
			protected internal object[] lookahead;
		}
		public const int INITIAL_LOOKAHEAD_BUFFER_SIZE = 5;
		private ITree currentEnumerationNode;
		protected bool uniqueNavigationNodes;
		protected internal object root;
		protected ITokenStream tokens;
		private ITreeAdaptor adaptor;
		protected internal StackList nodeStack = new StackList();
		protected internal StackList indexStack = new StackList();
		protected internal object currentNode;
		protected internal object previousNode;
		protected internal int currentChildIndex;
		protected int absoluteNodeIndex;
		protected internal object[] lookahead = new object[5];
		protected internal int head;
		protected internal int tail;
		protected IList markers;
		protected int markDepth;
		protected int lastMarker;
		protected object down;
		protected object up;
		protected object eof;
		public virtual object TreeSource
		{
			get
			{
				return this.root;
			}
		}
		public virtual object Current
		{
			get
			{
				return this.currentEnumerationNode;
			}
		}
		public virtual int Count
		{
			get
			{
				CommonTreeNodeStream commonTreeNodeStream = new CommonTreeNodeStream(this.root);
				return commonTreeNodeStream.Count;
			}
		}
		public ITreeAdaptor TreeAdaptor
		{
			get
			{
				return this.adaptor;
			}
		}
		public string SourceName
		{
			get
			{
				return this.TokenStream.SourceName;
			}
		}
		public ITokenStream TokenStream
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
		protected int LookaheadSize
		{
			get
			{
				if (this.tail >= this.head)
				{
					return this.tail - this.head;
				}
				return this.lookahead.Length - this.head + this.tail;
			}
		}
		public virtual void Reset()
		{
			this.currentNode = this.root;
			this.previousNode = null;
			this.currentChildIndex = -1;
			this.absoluteNodeIndex = -1;
			this.head = (this.tail = 0);
		}
		public virtual bool MoveNext()
		{
			if (this.currentNode == null)
			{
				this.AddLookahead(this.eof);
				this.currentEnumerationNode = null;
				return false;
			}
			if (this.currentChildIndex == -1)
			{
				this.currentEnumerationNode = (ITree)this.handleRootNode();
				return true;
			}
			if (this.currentChildIndex < this.adaptor.GetChildCount(this.currentNode))
			{
				this.currentEnumerationNode = (ITree)this.VisitChild(this.currentChildIndex);
				return true;
			}
			this.WalkBackToMostRecentNodeWithUnvisitedChildren();
			if (this.currentNode != null)
			{
				this.currentEnumerationNode = (ITree)this.VisitChild(this.currentChildIndex);
				return true;
			}
			return false;
		}
		public UnBufferedTreeNodeStream(object tree) : this(new CommonTreeAdaptor(), tree)
		{
		}
		public UnBufferedTreeNodeStream(ITreeAdaptor adaptor, object tree)
		{
			this.root = tree;
			this.adaptor = adaptor;
			this.Reset();
			this.down = adaptor.Create(2, "DOWN");
			this.up = adaptor.Create(3, "UP");
			this.eof = adaptor.Create(Token.EOF, "EOF");
		}
		public virtual object Get(int i)
		{
			throw new NotSupportedException("stream is unbuffered");
		}
		public virtual object LT(int k)
		{
			if (k == -1)
			{
				return this.previousNode;
			}
			if (k < 0)
			{
				throw new ArgumentNullException("tree node streams cannot look backwards more than 1 node", "k");
			}
			if (k == 0)
			{
				return Tree.INVALID_NODE;
			}
			this.fill(k);
			return this.lookahead[(this.head + k - 1) % this.lookahead.Length];
		}
		protected internal virtual void fill(int k)
		{
			int lookaheadSize = this.LookaheadSize;
			for (int i = 1; i <= k - lookaheadSize; i++)
			{
				this.MoveNext();
			}
		}
		protected internal virtual void AddLookahead(object node)
		{
			this.lookahead[this.tail] = node;
			this.tail = (this.tail + 1) % this.lookahead.Length;
			if (this.tail == this.head)
			{
				object[] destinationArray = new object[2 * this.lookahead.Length];
				int num = this.lookahead.Length - this.head;
				Array.Copy(this.lookahead, this.head, destinationArray, 0, num);
				Array.Copy(this.lookahead, 0, destinationArray, num, this.tail);
				this.lookahead = destinationArray;
				this.head = 0;
				this.tail += num;
			}
		}
		public virtual void Consume()
		{
			this.fill(1);
			this.absoluteNodeIndex++;
			this.previousNode = this.lookahead[this.head];
			this.head = (this.head + 1) % this.lookahead.Length;
		}
		public virtual int LA(int i)
		{
			object obj = (ITree)this.LT(i);
			if (obj == null)
			{
				return 0;
			}
			return this.adaptor.GetNodeType(obj);
		}
		public virtual int Mark()
		{
			if (this.markers == null)
			{
				this.markers = new List<object>();
				this.markers.Add(null);
			}
			this.markDepth++;
			UnBufferedTreeNodeStream.TreeWalkState treeWalkState;
			if (this.markDepth >= this.markers.Count)
			{
				treeWalkState = new UnBufferedTreeNodeStream.TreeWalkState();
				this.markers.Add(treeWalkState);
			}
			else
			{
				treeWalkState = (UnBufferedTreeNodeStream.TreeWalkState)this.markers[this.markDepth];
			}
			treeWalkState.absoluteNodeIndex = this.absoluteNodeIndex;
			treeWalkState.currentChildIndex = this.currentChildIndex;
			treeWalkState.currentNode = this.currentNode;
			treeWalkState.previousNode = this.previousNode;
			treeWalkState.nodeStackSize = this.nodeStack.Count;
			treeWalkState.indexStackSize = this.indexStack.Count;
			int lookaheadSize = this.LookaheadSize;
			int num = 0;
			treeWalkState.lookahead = new object[lookaheadSize];
			int i = 1;
			while (i <= lookaheadSize)
			{
				treeWalkState.lookahead[num] = this.LT(i);
				i++;
				num++;
			}
			this.lastMarker = this.markDepth;
			return this.markDepth;
		}
		public virtual void Release(int marker)
		{
			this.markDepth = marker;
			this.markDepth--;
		}
		public virtual void Rewind(int marker)
		{
			if (this.markers == null)
			{
				return;
			}
			UnBufferedTreeNodeStream.TreeWalkState treeWalkState = (UnBufferedTreeNodeStream.TreeWalkState)this.markers[marker];
			this.absoluteNodeIndex = treeWalkState.absoluteNodeIndex;
			this.currentChildIndex = treeWalkState.currentChildIndex;
			this.currentNode = treeWalkState.currentNode;
			this.previousNode = treeWalkState.previousNode;
			this.nodeStack.Capacity = treeWalkState.nodeStackSize;
			this.indexStack.Capacity = treeWalkState.indexStackSize;
			this.head = (this.tail = 0);
			while (this.tail < treeWalkState.lookahead.Length)
			{
				this.lookahead[this.tail] = treeWalkState.lookahead[this.tail];
				this.tail++;
			}
			this.Release(marker);
		}
		public void Rewind()
		{
			this.Rewind(this.lastMarker);
		}
		public virtual void Seek(int index)
		{
			if (index < this.Index())
			{
				throw new ArgumentOutOfRangeException("can't seek backwards in node stream", "index");
			}
			while (this.Index() < index)
			{
				this.Consume();
			}
		}
		public virtual int Index()
		{
			return this.absoluteNodeIndex + 1;
		}
		[Obsolete("Please use property Count instead.")]
		public virtual int Size()
		{
			return this.Count;
		}
		protected internal virtual object handleRootNode()
		{
			object obj = this.currentNode;
			this.currentChildIndex = 0;
			if (this.adaptor.IsNil(obj))
			{
				obj = this.VisitChild(this.currentChildIndex);
			}
			else
			{
				this.AddLookahead(obj);
				if (this.adaptor.GetChildCount(this.currentNode) == 0)
				{
					this.currentNode = null;
				}
			}
			return obj;
		}
		protected internal virtual object VisitChild(int child)
		{
			this.nodeStack.Push(this.currentNode);
			this.indexStack.Push(child);
			if (child == 0 && !this.adaptor.IsNil(this.currentNode))
			{
				this.AddNavigationNode(2);
			}
			this.currentNode = this.adaptor.GetChild(this.currentNode, child);
			this.currentChildIndex = 0;
			object obj = this.currentNode;
			this.AddLookahead(obj);
			this.WalkBackToMostRecentNodeWithUnvisitedChildren();
			return obj;
		}
		protected internal virtual void AddNavigationNode(int ttype)
		{
			object node;
			if (ttype == 2)
			{
				if (this.HasUniqueNavigationNodes)
				{
					node = this.adaptor.Create(2, "DOWN");
				}
				else
				{
					node = this.down;
				}
			}
			else
			{
				if (this.HasUniqueNavigationNodes)
				{
					node = this.adaptor.Create(3, "UP");
				}
				else
				{
					node = this.up;
				}
			}
			this.AddLookahead(node);
		}
		protected internal virtual void WalkBackToMostRecentNodeWithUnvisitedChildren()
		{
			while (this.currentNode != null && this.currentChildIndex >= this.adaptor.GetChildCount(this.currentNode))
			{
				this.currentNode = this.nodeStack.Pop();
				if (this.currentNode == null)
				{
					return;
				}
				this.currentChildIndex = (int)this.indexStack.Pop();
				this.currentChildIndex++;
				if (this.currentChildIndex >= this.adaptor.GetChildCount(this.currentNode))
				{
					if (!this.adaptor.IsNil(this.currentNode))
					{
						this.AddNavigationNode(3);
					}
					if (this.currentNode == this.root)
					{
						this.currentNode = null;
					}
				}
			}
		}
		public void ReplaceChildren(object parent, int startChildIndex, int stopChildIndex, object t)
		{
			throw new NotSupportedException("can't do stream rewrites yet");
		}
		public override string ToString()
		{
			return this.ToString(this.root, null);
		}
		public virtual string ToString(object start, object stop)
		{
			if (start == null)
			{
				return null;
			}
			if (this.tokens != null)
			{
				int tokenStartIndex = this.adaptor.GetTokenStartIndex(start);
				int stop2 = this.adaptor.GetTokenStopIndex(stop);
				if (stop != null && this.adaptor.GetNodeType(stop) == 3)
				{
					stop2 = this.adaptor.GetTokenStopIndex(start);
				}
				else
				{
					stop2 = this.Count - 1;
				}
				return this.tokens.ToString(tokenStartIndex, stop2);
			}
			StringBuilder stringBuilder = new StringBuilder();
			this.ToStringWork(start, stop, stringBuilder);
			return stringBuilder.ToString();
		}
		protected internal virtual void ToStringWork(object p, object stop, StringBuilder buf)
		{
			if (!this.adaptor.IsNil(p))
			{
				string text = this.adaptor.GetNodeText(p);
				if (text == null)
				{
					text = " " + this.adaptor.GetNodeType(p);
				}
				buf.Append(text);
			}
			if (p == stop)
			{
				return;
			}
			int childCount = this.adaptor.GetChildCount(p);
			if (childCount > 0 && !this.adaptor.IsNil(p))
			{
				buf.Append(" ");
				buf.Append(2);
			}
			for (int i = 0; i < childCount; i++)
			{
				object child = this.adaptor.GetChild(p, i);
				this.ToStringWork(child, stop, buf);
			}
			if (childCount > 0 && !this.adaptor.IsNil(p))
			{
				buf.Append(" ");
				buf.Append(3);
			}
		}
	}
}
