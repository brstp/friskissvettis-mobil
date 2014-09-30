using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
namespace Antlr.Runtime.Tree
{
	[Serializable]
	public abstract class BaseTree : ITree
	{
		protected IList children;
		public virtual int ChildCount
		{
			get
			{
				if (this.children == null)
				{
					return 0;
				}
				return this.children.Count;
			}
		}
		public virtual bool IsNil
		{
			get
			{
				return false;
			}
		}
		public virtual int Line
		{
			get
			{
				return 0;
			}
		}
		public virtual int CharPositionInLine
		{
			get
			{
				return 0;
			}
		}
		public IList Children
		{
			get
			{
				return this.children;
			}
		}
		public virtual int ChildIndex
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}
		public virtual ITree Parent
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public abstract int Type
		{
			get;
		}
		public abstract int TokenStartIndex
		{
			get;
			set;
		}
		public abstract int TokenStopIndex
		{
			get;
			set;
		}
		public abstract string Text
		{
			get;
		}
		public BaseTree()
		{
		}
		public BaseTree(ITree node)
		{
		}
		public virtual ITree GetChild(int i)
		{
			if (this.children == null || i >= this.children.Count)
			{
				return null;
			}
			return (ITree)this.children[i];
		}
		public virtual void AddChild(ITree t)
		{
			if (t == null)
			{
				return;
			}
			BaseTree baseTree = (BaseTree)t;
			if (baseTree.IsNil)
			{
				if (this.children != null && this.children == baseTree.children)
				{
					throw new InvalidOperationException("attempt to add child list to itself");
				}
				if (baseTree.children != null)
				{
					if (this.children != null)
					{
						int count = baseTree.children.Count;
						for (int i = 0; i < count; i++)
						{
							ITree tree = (ITree)baseTree.Children[i];
							this.children.Add(tree);
							tree.Parent = this;
							tree.ChildIndex = this.children.Count - 1;
						}
						return;
					}
					this.children = baseTree.children;
					this.FreshenParentAndChildIndexes();
					return;
				}
			}
			else
			{
				if (this.children == null)
				{
					this.children = this.CreateChildrenList();
				}
				this.children.Add(t);
				baseTree.Parent = this;
				baseTree.ChildIndex = this.children.Count - 1;
			}
		}
		public void AddChildren(IList kids)
		{
			for (int i = 0; i < kids.Count; i++)
			{
				ITree t = (ITree)kids[i];
				this.AddChild(t);
			}
		}
		public virtual void SetChild(int i, ITree t)
		{
			if (t == null)
			{
				return;
			}
			if (t.IsNil)
			{
				throw new ArgumentException("Can't set single child to a list");
			}
			if (this.children == null)
			{
				this.children = this.CreateChildrenList();
			}
			this.children[i] = t;
			t.Parent = this;
			t.ChildIndex = i;
		}
		public virtual object DeleteChild(int i)
		{
			if (this.children == null)
			{
				return null;
			}
			ITree result = (ITree)this.children[i];
			this.children.RemoveAt(i);
			this.FreshenParentAndChildIndexes(i);
			return result;
		}
		public virtual void ReplaceChildren(int startChildIndex, int stopChildIndex, object t)
		{
			if (this.children == null)
			{
				throw new ArgumentException("indexes invalid; no children in list");
			}
			int num = stopChildIndex - startChildIndex + 1;
			BaseTree baseTree = (BaseTree)t;
			IList list;
			if (baseTree.IsNil)
			{
				list = baseTree.Children;
			}
			else
			{
				list = new List<object>(1);
				list.Add(baseTree);
			}
			int count = list.Count;
			int count2 = list.Count;
			int num2 = num - count;
			if (num2 == 0)
			{
				int num3 = 0;
				for (int i = startChildIndex; i <= stopChildIndex; i++)
				{
					BaseTree baseTree2 = (BaseTree)list[num3];
					this.children[i] = baseTree2;
					baseTree2.Parent = this;
					baseTree2.ChildIndex = i;
					num3++;
				}
				return;
			}
			if (num2 > 0)
			{
				for (int j = 0; j < count2; j++)
				{
					this.children[startChildIndex + j] = list[j];
				}
				int num4 = startChildIndex + count2;
				for (int k = num4; k <= stopChildIndex; k++)
				{
					this.children.RemoveAt(num4);
				}
				this.FreshenParentAndChildIndexes(startChildIndex);
				return;
			}
			int l;
			for (l = 0; l < num; l++)
			{
				this.children[startChildIndex + l] = list[l];
			}
			while (l < count)
			{
				this.children.Insert(startChildIndex + l, list[l]);
				l++;
			}
			this.FreshenParentAndChildIndexes(startChildIndex);
		}
		protected internal virtual IList CreateChildrenList()
		{
			return new List<object>();
		}
		public virtual void FreshenParentAndChildIndexes()
		{
			this.FreshenParentAndChildIndexes(0);
		}
		public virtual void FreshenParentAndChildIndexes(int offset)
		{
			int childCount = this.ChildCount;
			for (int i = offset; i < childCount; i++)
			{
				ITree child = this.GetChild(i);
				child.ChildIndex = i;
				child.Parent = this;
			}
		}
		public virtual void SanityCheckParentAndChildIndexes()
		{
			this.SanityCheckParentAndChildIndexes(null, -1);
		}
		public virtual void SanityCheckParentAndChildIndexes(ITree parent, int i)
		{
			if (parent != this.Parent)
			{
				throw new ArgumentException(string.Concat(new object[]
				{
					"parents don't match; expected ",
					parent,
					" found ",
					this.Parent
				}));
			}
			if (i != this.ChildIndex)
			{
				throw new NotSupportedException(string.Concat(new object[]
				{
					"child indexes don't match; expected ",
					i,
					" found ",
					this.ChildIndex
				}));
			}
			int childCount = this.ChildCount;
			for (int j = 0; j < childCount; j++)
			{
				CommonTree commonTree = (CommonTree)this.GetChild(j);
				commonTree.SanityCheckParentAndChildIndexes(this, j);
			}
		}
		public bool HasAncestor(int ttype)
		{
			return this.GetAncestor(ttype) != null;
		}
		public ITree GetAncestor(int ttype)
		{
			for (ITree parent = ((ITree)this).Parent; parent != null; parent = parent.Parent)
			{
				if (parent.Type == ttype)
				{
					return parent;
				}
			}
			return null;
		}
		public IList GetAncestors()
		{
			if (this.Parent == null)
			{
				return null;
			}
			IList list = new List<object>();
			for (ITree parent = ((ITree)this).Parent; parent != null; parent = parent.Parent)
			{
				list.Insert(0, parent);
			}
			return list;
		}
		public virtual string ToStringTree()
		{
			if (this.children == null || this.children.Count == 0)
			{
				return this.ToString();
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (!this.IsNil)
			{
				stringBuilder.Append("(");
				stringBuilder.Append(this.ToString());
				stringBuilder.Append(' ');
			}
			int num = 0;
			while (this.children != null && num < this.children.Count)
			{
				ITree tree = (ITree)this.children[num];
				if (num > 0)
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append(tree.ToStringTree());
				num++;
			}
			if (!this.IsNil)
			{
				stringBuilder.Append(")");
			}
			return stringBuilder.ToString();
		}
		public abstract override string ToString();
		public abstract ITree DupNode();
	}
}
