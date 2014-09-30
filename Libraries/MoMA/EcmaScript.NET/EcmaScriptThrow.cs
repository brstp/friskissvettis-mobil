using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public class EcmaScriptThrow : EcmaScriptException
	{
		private object value;
		public virtual object Value
		{
			get
			{
				return this.value;
			}
		}
		public override string Message
		{
			get
			{
				IScriptable scriptable = this.value as IScriptable;
				string result;
				if (scriptable != null)
				{
					result = ScriptRuntime.DefaultObjectToString(scriptable);
				}
				else
				{
					result = ScriptConvert.ToString(this.value);
				}
				return result;
			}
		}
		public EcmaScriptThrow(object value, string sourceName, int lineNumber)
		{
			base.RecordErrorOrigin(sourceName, lineNumber, null, 0);
			this.value = value;
		}
	}
}
