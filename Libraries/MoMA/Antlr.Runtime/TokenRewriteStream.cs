using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
namespace Antlr.Runtime
{
	public class TokenRewriteStream : CommonTokenStream
	{
		private class RewriteOpComparer : IComparer
		{
			public virtual int Compare(object o1, object o2)
			{
				TokenRewriteStream.RewriteOperation rewriteOperation = (TokenRewriteStream.RewriteOperation)o1;
				TokenRewriteStream.RewriteOperation rewriteOperation2 = (TokenRewriteStream.RewriteOperation)o2;
				if (rewriteOperation.index < rewriteOperation2.index)
				{
					return -1;
				}
				if (rewriteOperation.index > rewriteOperation2.index)
				{
					return 1;
				}
				return 0;
			}
		}
		protected internal class RewriteOperation
		{
			protected internal int instructionIndex;
			protected internal int index;
			protected internal object text;
			protected internal TokenRewriteStream parent;
			protected internal RewriteOperation(int index, object text, TokenRewriteStream parent)
			{
				this.index = index;
				this.text = text;
				this.parent = parent;
			}
			public virtual int Execute(StringBuilder buf)
			{
				return this.index;
			}
			public override string ToString()
			{
				string text = base.GetType().FullName;
				int num = text.IndexOf('$');
				text = text.Substring(num + 1, text.Length - (num + 1));
				return string.Concat(new object[]
				{
					"<",
					text,
					"@",
					this.index,
					":\"",
					this.text,
					"\">"
				});
			}
		}
		protected internal class InsertBeforeOp : TokenRewriteStream.RewriteOperation
		{
			public InsertBeforeOp(int index, object text, TokenRewriteStream parent) : base(index, text, parent)
			{
			}
			public override int Execute(StringBuilder buf)
			{
				buf.Append(this.text);
				buf.Append(this.parent.Get(this.index).Text);
				return this.index + 1;
			}
		}
		protected internal class ReplaceOp : TokenRewriteStream.RewriteOperation
		{
			protected internal int lastIndex;
			public ReplaceOp(int from, int to, object text, TokenRewriteStream parent) : base(from, text, parent)
			{
				this.lastIndex = to;
			}
			public override int Execute(StringBuilder buf)
			{
				if (this.text != null)
				{
					buf.Append(this.text);
				}
				return this.lastIndex + 1;
			}
			public override string ToString()
			{
				return string.Concat(new object[]
				{
					"<ReplaceOp@",
					this.index,
					"..",
					this.lastIndex,
					":\"",
					this.text,
					"\">"
				});
			}
		}
		protected internal class DeleteOp : TokenRewriteStream.ReplaceOp
		{
			public DeleteOp(int from, int to, TokenRewriteStream parent) : base(from, to, null, parent)
			{
			}
			public override string ToString()
			{
				return string.Concat(new object[]
				{
					"<DeleteOp@",
					this.index,
					"..",
					this.lastIndex,
					">"
				});
			}
		}
		public const string DEFAULT_PROGRAM_NAME = "default";
		public const int PROGRAM_INIT_SIZE = 100;
		public const int MIN_TOKEN_INDEX = 0;
		protected IDictionary programs;
		protected IDictionary lastRewriteTokenIndexes;
		public TokenRewriteStream()
		{
			this.Init();
		}
		public TokenRewriteStream(ITokenSource tokenSource) : base(tokenSource)
		{
			this.Init();
		}
		public TokenRewriteStream(ITokenSource tokenSource, int channel) : base(tokenSource, channel)
		{
			this.Init();
		}
		protected internal virtual void Init()
		{
			this.programs = new Hashtable();
			this.programs["default"] = new List<object>(100);
			this.lastRewriteTokenIndexes = new Hashtable();
		}
		public virtual void Rollback(int instructionIndex)
		{
			this.Rollback("default", instructionIndex);
		}
		public virtual void Rollback(string programName, int instructionIndex)
		{
			IList list = (IList)this.programs[programName];
			if (list != null)
			{
				this.programs[programName] = ((List<object>)list).GetRange(0, instructionIndex);
			}
		}
		public virtual void DeleteProgram()
		{
			this.DeleteProgram("default");
		}
		public virtual void DeleteProgram(string programName)
		{
			this.Rollback(programName, 0);
		}
		public virtual void InsertAfter(IToken t, object text)
		{
			this.InsertAfter("default", t, text);
		}
		public virtual void InsertAfter(int index, object text)
		{
			this.InsertAfter("default", index, text);
		}
		public virtual void InsertAfter(string programName, IToken t, object text)
		{
			this.InsertAfter(programName, t.TokenIndex, text);
		}
		public virtual void InsertAfter(string programName, int index, object text)
		{
			this.InsertBefore(programName, index + 1, text);
		}
		public virtual void InsertBefore(IToken t, object text)
		{
			this.InsertBefore("default", t, text);
		}
		public virtual void InsertBefore(int index, object text)
		{
			this.InsertBefore("default", index, text);
		}
		public virtual void InsertBefore(string programName, IToken t, object text)
		{
			this.InsertBefore(programName, t.TokenIndex, text);
		}
		public virtual void InsertBefore(string programName, int index, object text)
		{
			TokenRewriteStream.RewriteOperation value = new TokenRewriteStream.InsertBeforeOp(index, text, this);
			IList program = this.GetProgram(programName);
			program.Add(value);
		}
		public virtual void Replace(int index, object text)
		{
			this.Replace("default", index, index, text);
		}
		public virtual void Replace(int from, int to, object text)
		{
			this.Replace("default", from, to, text);
		}
		public virtual void Replace(IToken indexT, object text)
		{
			this.Replace("default", indexT, indexT, text);
		}
		public virtual void Replace(IToken from, IToken to, object text)
		{
			this.Replace("default", from, to, text);
		}
		public virtual void Replace(string programName, int from, int to, object text)
		{
			if (from > to || from < 0 || to < 0 || to >= this.tokens.Count)
			{
				throw new ArgumentOutOfRangeException(string.Concat(new object[]
				{
					"replace: range invalid: ",
					from,
					"..",
					to,
					"(size=",
					this.tokens.Count,
					")"
				}));
			}
			TokenRewriteStream.RewriteOperation rewriteOperation = new TokenRewriteStream.ReplaceOp(from, to, text, this);
			IList program = this.GetProgram(programName);
			rewriteOperation.instructionIndex = program.Count;
			program.Add(rewriteOperation);
		}
		public virtual void Replace(string programName, IToken from, IToken to, object text)
		{
			this.Replace(programName, from.TokenIndex, to.TokenIndex, text);
		}
		public virtual void Delete(int index)
		{
			this.Delete("default", index, index);
		}
		public virtual void Delete(int from, int to)
		{
			this.Delete("default", from, to);
		}
		public virtual void Delete(IToken indexT)
		{
			this.Delete("default", indexT, indexT);
		}
		public virtual void Delete(IToken from, IToken to)
		{
			this.Delete("default", from, to);
		}
		public virtual void Delete(string programName, int from, int to)
		{
			this.Replace(programName, from, to, null);
		}
		public virtual void Delete(string programName, IToken from, IToken to)
		{
			this.Replace(programName, from, to, null);
		}
		public virtual int GetLastRewriteTokenIndex()
		{
			return this.GetLastRewriteTokenIndex("default");
		}
		protected virtual int GetLastRewriteTokenIndex(string programName)
		{
			object obj = this.lastRewriteTokenIndexes[programName];
			if (obj == null)
			{
				return -1;
			}
			return (int)obj;
		}
		protected virtual void SetLastRewriteTokenIndex(string programName, int i)
		{
			this.lastRewriteTokenIndexes[programName] = i;
		}
		protected virtual IList GetProgram(string name)
		{
			IList list = (IList)this.programs[name];
			if (list == null)
			{
				list = this.InitializeProgram(name);
			}
			return list;
		}
		private IList InitializeProgram(string name)
		{
			IList list = new List<object>(100);
			this.programs[name] = list;
			return list;
		}
		public virtual string ToOriginalString()
		{
			return this.ToOriginalString(0, this.Count - 1);
		}
		public virtual string ToOriginalString(int start, int end)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = start;
			while (num >= 0 && num <= end && num < this.tokens.Count)
			{
				stringBuilder.Append(this.Get(num).Text);
				num++;
			}
			return stringBuilder.ToString();
		}
		public override string ToString()
		{
			return this.ToString(0, this.Count - 1);
		}
		public virtual string ToString(string programName)
		{
			return this.ToString(programName, 0, this.Count - 1);
		}
		public override string ToString(int start, int end)
		{
			return this.ToString("default", start, end);
		}
		public virtual string ToString(string programName, int start, int end)
		{
			IList list = (IList)this.programs[programName];
			if (end > this.tokens.Count - 1)
			{
				end = this.tokens.Count - 1;
			}
			if (start < 0)
			{
				start = 0;
			}
			if (list == null || list.Count == 0)
			{
				return this.ToOriginalString(start, end);
			}
			StringBuilder stringBuilder = new StringBuilder();
			IDictionary dictionary = this.ReduceToSingleOperationPerIndex(list);
			int num = start;
			while (num <= end && num < this.tokens.Count)
			{
				TokenRewriteStream.RewriteOperation rewriteOperation = (TokenRewriteStream.RewriteOperation)dictionary[num];
				dictionary.Remove(num);
				IToken token = (IToken)this.tokens[num];
				if (rewriteOperation == null)
				{
					stringBuilder.Append(token.Text);
					num++;
				}
				else
				{
					num = rewriteOperation.Execute(stringBuilder);
				}
			}
			if (end == this.tokens.Count - 1)
			{
				IEnumerator enumerator = dictionary.Values.GetEnumerator();
				while (enumerator.MoveNext())
				{
					TokenRewriteStream.InsertBeforeOp insertBeforeOp = (TokenRewriteStream.InsertBeforeOp)enumerator.Current;
					if (insertBeforeOp.index >= this.tokens.Count - 1)
					{
						stringBuilder.Append(insertBeforeOp.text);
					}
				}
			}
			return stringBuilder.ToString();
		}
		protected IDictionary ReduceToSingleOperationPerIndex(IList rewrites)
		{
			for (int i = 0; i < rewrites.Count; i++)
			{
				TokenRewriteStream.RewriteOperation rewriteOperation = (TokenRewriteStream.RewriteOperation)rewrites[i];
				if (rewriteOperation != null && rewriteOperation is TokenRewriteStream.ReplaceOp)
				{
					TokenRewriteStream.ReplaceOp replaceOp = (TokenRewriteStream.ReplaceOp)rewrites[i];
					IList kindOfOps = this.GetKindOfOps(rewrites, typeof(TokenRewriteStream.InsertBeforeOp), i);
					for (int j = 0; j < kindOfOps.Count; j++)
					{
						TokenRewriteStream.InsertBeforeOp insertBeforeOp = (TokenRewriteStream.InsertBeforeOp)kindOfOps[j];
						if (insertBeforeOp.index >= replaceOp.index && insertBeforeOp.index <= replaceOp.lastIndex)
						{
							rewrites[insertBeforeOp.instructionIndex] = null;
						}
					}
					IList kindOfOps2 = this.GetKindOfOps(rewrites, typeof(TokenRewriteStream.ReplaceOp), i);
					for (int k = 0; k < kindOfOps2.Count; k++)
					{
						TokenRewriteStream.ReplaceOp replaceOp2 = (TokenRewriteStream.ReplaceOp)kindOfOps2[k];
						if (replaceOp2.index >= replaceOp.index && replaceOp2.lastIndex <= replaceOp.lastIndex)
						{
							rewrites[replaceOp2.instructionIndex] = null;
						}
						else
						{
							bool flag = replaceOp2.lastIndex < replaceOp.index || replaceOp2.index > replaceOp.lastIndex;
							bool flag2 = replaceOp2.index == replaceOp.index && replaceOp2.lastIndex == replaceOp.lastIndex;
							if (!flag && !flag2)
							{
								throw new ArgumentOutOfRangeException(string.Concat(new object[]
								{
									"replace op boundaries of ",
									replaceOp,
									" overlap with previous ",
									replaceOp2
								}));
							}
						}
					}
				}
			}
			for (int l = 0; l < rewrites.Count; l++)
			{
				TokenRewriteStream.RewriteOperation rewriteOperation2 = (TokenRewriteStream.RewriteOperation)rewrites[l];
				if (rewriteOperation2 != null && rewriteOperation2 is TokenRewriteStream.InsertBeforeOp)
				{
					TokenRewriteStream.InsertBeforeOp insertBeforeOp2 = (TokenRewriteStream.InsertBeforeOp)rewrites[l];
					IList kindOfOps3 = this.GetKindOfOps(rewrites, typeof(TokenRewriteStream.InsertBeforeOp), l);
					for (int m = 0; m < kindOfOps3.Count; m++)
					{
						TokenRewriteStream.InsertBeforeOp insertBeforeOp3 = (TokenRewriteStream.InsertBeforeOp)kindOfOps3[m];
						if (insertBeforeOp3.index == insertBeforeOp2.index)
						{
							insertBeforeOp2.text = this.CatOpText(insertBeforeOp2.text, insertBeforeOp3.text);
							rewrites[insertBeforeOp3.instructionIndex] = null;
						}
					}
					IList kindOfOps4 = this.GetKindOfOps(rewrites, typeof(TokenRewriteStream.ReplaceOp), l);
					for (int n = 0; n < kindOfOps4.Count; n++)
					{
						TokenRewriteStream.ReplaceOp replaceOp3 = (TokenRewriteStream.ReplaceOp)kindOfOps4[n];
						if (insertBeforeOp2.index == replaceOp3.index)
						{
							replaceOp3.text = this.CatOpText(insertBeforeOp2.text, replaceOp3.text);
							rewrites[l] = null;
						}
						else
						{
							if (insertBeforeOp2.index >= replaceOp3.index && insertBeforeOp2.index <= replaceOp3.lastIndex)
							{
								throw new ArgumentOutOfRangeException(string.Concat(new object[]
								{
									"insert op ",
									insertBeforeOp2,
									" within boundaries of previous ",
									replaceOp3
								}));
							}
						}
					}
				}
			}
			IDictionary dictionary = new Hashtable();
			for (int num = 0; num < rewrites.Count; num++)
			{
				TokenRewriteStream.RewriteOperation rewriteOperation3 = (TokenRewriteStream.RewriteOperation)rewrites[num];
				if (rewriteOperation3 != null)
				{
					if (dictionary[rewriteOperation3.index] != null)
					{
						throw new Exception("should only be one op per index");
					}
					dictionary[rewriteOperation3.index] = rewriteOperation3;
				}
			}
			return dictionary;
		}
		protected string CatOpText(object a, object b)
		{
			string str = "";
			string str2 = "";
			if (a != null)
			{
				str = a.ToString();
			}
			if (b != null)
			{
				str2 = b.ToString();
			}
			return str + str2;
		}
		protected IList GetKindOfOps(IList rewrites, Type kind)
		{
			return this.GetKindOfOps(rewrites, kind, rewrites.Count);
		}
		protected IList GetKindOfOps(IList rewrites, Type kind, int before)
		{
			IList list = new List<object>();
			int num = 0;
			while (num < before && num < rewrites.Count)
			{
				TokenRewriteStream.RewriteOperation rewriteOperation = (TokenRewriteStream.RewriteOperation)rewrites[num];
				if (rewriteOperation != null && rewriteOperation.GetType() == kind)
				{
					list.Add(rewriteOperation);
				}
				num++;
			}
			return list;
		}
		public virtual string ToDebugString()
		{
			return this.ToDebugString(0, this.Count - 1);
		}
		public virtual string ToDebugString(int start, int end)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = start;
			while (num >= 0 && num <= end && num < this.tokens.Count)
			{
				stringBuilder.Append(this.Get(num));
				num++;
			}
			return stringBuilder.ToString();
		}
	}
}
