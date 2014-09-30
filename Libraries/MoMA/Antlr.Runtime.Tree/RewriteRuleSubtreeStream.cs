using System;
using System.Collections;
using System.Collections.Generic;
namespace Antlr.Runtime.Tree
{
	public class RewriteRuleSubtreeStream : RewriteRuleElementStream<object>
	{
		private delegate object ProcessHandler(object o);
		public RewriteRuleSubtreeStream(ITreeAdaptor adaptor, string elementDescription) : base(adaptor, elementDescription)
		{
		}
		public RewriteRuleSubtreeStream(ITreeAdaptor adaptor, string elementDescription, object oneElement) : base(adaptor, elementDescription, oneElement)
		{
		}
		public RewriteRuleSubtreeStream(ITreeAdaptor adaptor, string elementDescription, IList<object> elements) : base(adaptor, elementDescription, elements)
		{
		}
		[Obsolete("This constructor is for internal use only and might be phased out soon. Use instead the one with IList<T>.")]
		public RewriteRuleSubtreeStream(ITreeAdaptor adaptor, string elementDescription, IList elements) : base(adaptor, elementDescription, elements)
		{
		}
		public object NextNode()
		{
			return this.FetchObject((object o) => this.adaptor.DupNode(o));
		}
		private object FetchObject(RewriteRuleSubtreeStream.ProcessHandler ph)
		{
			if (this.RequiresDuplication())
			{
				return ph(base._Next());
			}
			return base._Next();
		}
		private bool RequiresDuplication()
		{
			int count = base.Count;
			return this.dirty || (this.cursor >= count && count == 1);
		}
		public override object NextTree()
		{
			return this.FetchObject((object o) => this.Dup(o));
		}
		private object Dup(object el)
		{
			return this.adaptor.DupTree(el);
		}
	}
}
