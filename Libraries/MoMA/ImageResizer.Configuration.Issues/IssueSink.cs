using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
namespace ImageResizer.Configuration.Issues
{
	[ComVisible(true)]
	public class IssueSink : IIssueProvider, IIssueReceiver
	{
		protected string defaultSource;
		private IDictionary<int, IIssue> _issueSet = new Dictionary<int, IIssue>();
		private IList<IIssue> _issues = new List<IIssue>();
		private object issueSync = new object();
		public IssueSink(string defaultSource)
		{
			this.defaultSource = defaultSource;
		}
		public virtual IEnumerable<IIssue> GetIssues()
		{
			object obj;
			Monitor.Enter(obj = this.issueSync);
			IEnumerable<IIssue> result;
			try
			{
				result = new List<IIssue>(this._issues);
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return result;
		}
		public virtual void AcceptIssue(IIssue i)
		{
			if (i.Source == null && i is Issue)
			{
				((Issue)i).Source = this.defaultSource;
			}
			int hashCode = i.GetHashCode();
			object obj;
			Monitor.Enter(obj = this.issueSync);
			try
			{
				if (!this._issueSet.ContainsKey(hashCode))
				{
					this._issueSet[hashCode] = i;
					this._issues.Add(i);
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
	}
}
