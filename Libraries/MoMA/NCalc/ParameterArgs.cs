using System;
namespace NCalc
{
	public class ParameterArgs : EventArgs
	{
		private object _result;
		public object Result
		{
			get
			{
				return this._result;
			}
			set
			{
				this._result = value;
				this.HasResult = true;
			}
		}
		public bool HasResult
		{
			get;
			set;
		}
	}
}
