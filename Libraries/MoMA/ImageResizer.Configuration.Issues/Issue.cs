using System;
using System.Runtime.InteropServices;
using System.Text;
namespace ImageResizer.Configuration.Issues
{
	[ComVisible(true)]
	public class Issue : IIssue
	{
		private string source;
		private string summary;
		private string details;
		private IssueSeverity severity = IssueSeverity.Warning;
		public string Source
		{
			get
			{
				return this.source;
			}
			set
			{
				this.source = value;
			}
		}
		public string Summary
		{
			get
			{
				return this.summary;
			}
			set
			{
				this.summary = value;
			}
		}
		public string Details
		{
			get
			{
				return this.details;
			}
			set
			{
				this.details = value;
			}
		}
		public IssueSeverity Severity
		{
			get
			{
				return this.severity;
			}
			set
			{
				this.severity = value;
			}
		}
		public Issue()
		{
		}
		public Issue(string message)
		{
			this.summary = message;
		}
		public Issue(string message, string details, IssueSeverity severity)
		{
			this.summary = message;
			this.details = details;
			this.severity = severity;
		}
		public Issue(string message, IssueSeverity severity)
		{
			this.summary = message;
			this.severity = severity;
		}
		public Issue(string source, string message, string details, IssueSeverity severity)
		{
			this.source = source;
			this.summary = message;
			this.details = details;
			this.severity = severity;
		}
		public override int GetHashCode()
		{
			StringBuilder stringBuilder = new StringBuilder(160);
			if (this.source != null)
			{
				stringBuilder.Append(this.source);
			}
			stringBuilder.Append('|');
			if (this.summary != null)
			{
				stringBuilder.Append(this.summary);
			}
			stringBuilder.Append('|');
			if (this.details != null)
			{
				stringBuilder.Append(this.details);
			}
			stringBuilder.Append('|');
			stringBuilder.Append((int)this.severity);
			return stringBuilder.ToString().GetHashCode();
		}
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				this.Source,
				"(",
				this.Severity.ToString(),
				"):\t",
				this.Summary,
				("\n" + this.Details).Replace("\n", "\n\t\t\t"),
				"\n"
			});
		}
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return base.Equals(obj);
			}
			return this.GetHashCode() == obj.GetHashCode();
		}
	}
}
