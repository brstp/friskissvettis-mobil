using System;
using System.Collections;
using System.Collections.Generic;
namespace Antlr.Runtime.Tree
{
	public class TreeWizard
	{
		public interface ContextVisitor
		{
			void Visit(object t, object parent, int childIndex, IDictionary labels);
		}
		public abstract class Visitor : TreeWizard.ContextVisitor
		{
			public void Visit(object t, object parent, int childIndex, IDictionary labels)
			{
				this.Visit(t);
			}
			public abstract void Visit(object t);
		}
		private sealed class RecordAllElementsVisitor : TreeWizard.Visitor
		{
			private IList list;
			public RecordAllElementsVisitor(IList list)
			{
				this.list = list;
			}
			public override void Visit(object t)
			{
				this.list.Add(t);
			}
		}
		private sealed class PatternMatchingContextVisitor : TreeWizard.ContextVisitor
		{
			private TreeWizard owner;
			private TreeWizard.TreePattern pattern;
			private IList list;
			public PatternMatchingContextVisitor(TreeWizard owner, TreeWizard.TreePattern pattern, IList list)
			{
				this.owner = owner;
				this.pattern = pattern;
				this.list = list;
			}
			public void Visit(object t, object parent, int childIndex, IDictionary labels)
			{
				if (this.owner._Parse(t, this.pattern, null))
				{
					this.list.Add(t);
				}
			}
		}
		private sealed class InvokeVisitorOnPatternMatchContextVisitor : TreeWizard.ContextVisitor
		{
			private TreeWizard owner;
			private TreeWizard.TreePattern pattern;
			private TreeWizard.ContextVisitor visitor;
			private Hashtable labels = new Hashtable();
			public InvokeVisitorOnPatternMatchContextVisitor(TreeWizard owner, TreeWizard.TreePattern pattern, TreeWizard.ContextVisitor visitor)
			{
				this.owner = owner;
				this.pattern = pattern;
				this.visitor = visitor;
			}
			public void Visit(object t, object parent, int childIndex, IDictionary unusedlabels)
			{
				this.labels.Clear();
				if (this.owner._Parse(t, this.pattern, this.labels))
				{
					this.visitor.Visit(t, parent, childIndex, this.labels);
				}
			}
		}
		public class TreePattern : CommonTree
		{
			public string label;
			public bool hasTextArg;
			public TreePattern(IToken payload) : base(payload)
			{
			}
			public override string ToString()
			{
				if (this.label != null)
				{
					return "%" + this.label + ":" + base.ToString();
				}
				return base.ToString();
			}
		}
		public class WildcardTreePattern : TreeWizard.TreePattern
		{
			public WildcardTreePattern(IToken payload) : base(payload)
			{
			}
		}
		public class TreePatternTreeAdaptor : CommonTreeAdaptor
		{
			public override object Create(IToken payload)
			{
				return new TreeWizard.TreePattern(payload);
			}
		}
		protected ITreeAdaptor adaptor;
		protected IDictionary tokenNameToTypeMap;
		public TreeWizard(ITreeAdaptor adaptor)
		{
			this.adaptor = adaptor;
		}
		public TreeWizard(ITreeAdaptor adaptor, IDictionary tokenNameToTypeMap)
		{
			this.adaptor = adaptor;
			this.tokenNameToTypeMap = tokenNameToTypeMap;
		}
		public TreeWizard(ITreeAdaptor adaptor, string[] tokenNames)
		{
			this.adaptor = adaptor;
			this.tokenNameToTypeMap = this.ComputeTokenTypes(tokenNames);
		}
		public TreeWizard(string[] tokenNames) : this(null, tokenNames)
		{
		}
		public IDictionary ComputeTokenTypes(string[] tokenNames)
		{
			IDictionary dictionary = new Hashtable();
			if (tokenNames == null)
			{
				return dictionary;
			}
			for (int i = Token.MIN_TOKEN_TYPE; i < tokenNames.Length; i++)
			{
				string key = tokenNames[i];
				dictionary.Add(key, i);
			}
			return dictionary;
		}
		public int GetTokenType(string tokenName)
		{
			if (this.tokenNameToTypeMap == null)
			{
				return 0;
			}
			object obj = this.tokenNameToTypeMap[tokenName];
			if (obj != null)
			{
				return (int)obj;
			}
			return 0;
		}
		public IDictionary Index(object t)
		{
			IDictionary dictionary = new Hashtable();
			this._Index(t, dictionary);
			return dictionary;
		}
		protected void _Index(object t, IDictionary m)
		{
			if (t == null)
			{
				return;
			}
			int nodeType = this.adaptor.GetNodeType(t);
			IList list = m[nodeType] as IList;
			if (list == null)
			{
				list = new List<object>();
				m[nodeType] = list;
			}
			list.Add(t);
			int childCount = this.adaptor.GetChildCount(t);
			for (int i = 0; i < childCount; i++)
			{
				object child = this.adaptor.GetChild(t, i);
				this._Index(child, m);
			}
		}
		public IList Find(object t, int ttype)
		{
			IList list = new List<object>();
			this.Visit(t, ttype, new TreeWizard.RecordAllElementsVisitor(list));
			return list;
		}
		public IList Find(object t, string pattern)
		{
			IList list = new List<object>();
			TreePatternLexer tokenizer = new TreePatternLexer(pattern);
			TreePatternParser treePatternParser = new TreePatternParser(tokenizer, this, new TreeWizard.TreePatternTreeAdaptor());
			TreeWizard.TreePattern treePattern = (TreeWizard.TreePattern)treePatternParser.Pattern();
			if (treePattern == null || treePattern.IsNil || treePattern.GetType() == typeof(TreeWizard.WildcardTreePattern))
			{
				return null;
			}
			int type = treePattern.Type;
			this.Visit(t, type, new TreeWizard.PatternMatchingContextVisitor(this, treePattern, list));
			return list;
		}
		public object FindFirst(object t, int ttype)
		{
			return null;
		}
		public object FindFirst(object t, string pattern)
		{
			return null;
		}
		public void Visit(object t, int ttype, TreeWizard.ContextVisitor visitor)
		{
			this._Visit(t, null, 0, ttype, visitor);
		}
		protected void _Visit(object t, object parent, int childIndex, int ttype, TreeWizard.ContextVisitor visitor)
		{
			if (t == null)
			{
				return;
			}
			if (this.adaptor.GetNodeType(t) == ttype)
			{
				visitor.Visit(t, parent, childIndex, null);
			}
			int childCount = this.adaptor.GetChildCount(t);
			for (int i = 0; i < childCount; i++)
			{
				object child = this.adaptor.GetChild(t, i);
				this._Visit(child, t, i, ttype, visitor);
			}
		}
		public void Visit(object t, string pattern, TreeWizard.ContextVisitor visitor)
		{
			TreePatternLexer tokenizer = new TreePatternLexer(pattern);
			TreePatternParser treePatternParser = new TreePatternParser(tokenizer, this, new TreeWizard.TreePatternTreeAdaptor());
			TreeWizard.TreePattern treePattern = (TreeWizard.TreePattern)treePatternParser.Pattern();
			if (treePattern == null || treePattern.IsNil || treePattern.GetType() == typeof(TreeWizard.WildcardTreePattern))
			{
				return;
			}
			int type = treePattern.Type;
			this.Visit(t, type, new TreeWizard.InvokeVisitorOnPatternMatchContextVisitor(this, treePattern, visitor));
		}
		public bool Parse(object t, string pattern, IDictionary labels)
		{
			TreePatternLexer tokenizer = new TreePatternLexer(pattern);
			TreePatternParser treePatternParser = new TreePatternParser(tokenizer, this, new TreeWizard.TreePatternTreeAdaptor());
			TreeWizard.TreePattern t2 = (TreeWizard.TreePattern)treePatternParser.Pattern();
			return this._Parse(t, t2, labels);
		}
		public bool Parse(object t, string pattern)
		{
			return this.Parse(t, pattern, null);
		}
		protected bool _Parse(object t1, TreeWizard.TreePattern t2, IDictionary labels)
		{
			if (t1 == null || t2 == null)
			{
				return false;
			}
			if (t2.GetType() != typeof(TreeWizard.WildcardTreePattern))
			{
				if (this.adaptor.GetNodeType(t1) != t2.Type)
				{
					return false;
				}
				if (t2.hasTextArg && !this.adaptor.GetNodeText(t1).Equals(t2.Text))
				{
					return false;
				}
			}
			if (t2.label != null && labels != null)
			{
				labels[t2.label] = t1;
			}
			int childCount = this.adaptor.GetChildCount(t1);
			int childCount2 = t2.ChildCount;
			if (childCount != childCount2)
			{
				return false;
			}
			for (int i = 0; i < childCount; i++)
			{
				object child = this.adaptor.GetChild(t1, i);
				TreeWizard.TreePattern t3 = (TreeWizard.TreePattern)t2.GetChild(i);
				if (!this._Parse(child, t3, labels))
				{
					return false;
				}
			}
			return true;
		}
		public object Create(string pattern)
		{
			TreePatternLexer tokenizer = new TreePatternLexer(pattern);
			TreePatternParser treePatternParser = new TreePatternParser(tokenizer, this, this.adaptor);
			return treePatternParser.Pattern();
		}
		public static bool Equals(object t1, object t2, ITreeAdaptor adaptor)
		{
			return TreeWizard._Equals(t1, t2, adaptor);
		}
		public new bool Equals(object t1, object t2)
		{
			return TreeWizard._Equals(t1, t2, this.adaptor);
		}
		protected static bool _Equals(object t1, object t2, ITreeAdaptor adaptor)
		{
			if (t1 == null || t2 == null)
			{
				return false;
			}
			if (adaptor.GetNodeType(t1) != adaptor.GetNodeType(t2))
			{
				return false;
			}
			if (!adaptor.GetNodeText(t1).Equals(adaptor.GetNodeText(t2)))
			{
				return false;
			}
			int childCount = adaptor.GetChildCount(t1);
			int childCount2 = adaptor.GetChildCount(t2);
			if (childCount != childCount2)
			{
				return false;
			}
			for (int i = 0; i < childCount; i++)
			{
				object child = adaptor.GetChild(t1, i);
				object child2 = adaptor.GetChild(t2, i);
				if (!TreeWizard._Equals(child, child2, adaptor))
				{
					return false;
				}
			}
			return true;
		}
	}
}
