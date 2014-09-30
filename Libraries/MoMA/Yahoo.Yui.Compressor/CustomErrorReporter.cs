using EcmaScript.NET;
using System;
using System.Collections.Specialized;
namespace Yahoo.Yui.Compressor
{
	public class CustomErrorReporter : ErrorReporter
	{
		private readonly bool _isVerboseLogging;
		public StringCollection ErrorMessages
		{
			get;
			private set;
		}
		public CustomErrorReporter(bool isVerboseLogging)
		{
			this._isVerboseLogging = isVerboseLogging;
			this.ErrorMessages = new StringCollection();
		}
		public virtual void Warning(string message, string sourceName, int line, string lineSource, int lineOffset)
		{
			if (this._isVerboseLogging)
			{
				string value = "[WARNING] " + message;
				Console.WriteLine(value);
				this.ErrorMessages.Add(value);
			}
		}
		public virtual void Error(string message, string sourceName, int line, string lineSource, int lineOffset)
		{
			throw new InvalidOperationException("[ERROR] " + message);
		}
		public virtual EcmaScriptRuntimeException RuntimeError(string message, string sourceName, int line, string lineSource, int lineOffset)
		{
			throw new InvalidOperationException("[ERROR] EcmaScriptRuntimeException :: " + message);
		}
	}
}
