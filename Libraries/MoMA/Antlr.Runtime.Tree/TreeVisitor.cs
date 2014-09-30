using System;
namespace Antlr.Runtime.Tree
{
	public class TreeVisitor
	{
		protected ITreeAdaptor adaptor;
		public TreeVisitor(ITreeAdaptor adaptor)
		{
			this.adaptor = adaptor;
		}
		public TreeVisitor() : this(new CommonTreeAdaptor())
		{
		}
		public object Visit(object t, ITreeVisitorAction action)
		{
			bool flag = this.adaptor.IsNil(t);
			if (action != null && !flag)
			{
				t = action.Pre(t);
			}
			int childCount = this.adaptor.GetChildCount(t);
			for (int i = 0; i < childCount; i++)
			{
				object child = this.adaptor.GetChild(t, i);
				object obj = this.Visit(child, action);
				object child2 = this.adaptor.GetChild(t, i);
				if (obj != child2)
				{
					this.adaptor.SetChild(t, i, obj);
				}
			}
			if (action != null && !flag)
			{
				t = action.Post(t);
			}
			return t;
		}
	}
}
