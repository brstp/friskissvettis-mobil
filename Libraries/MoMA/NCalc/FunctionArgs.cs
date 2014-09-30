using System;
namespace NCalc
{
	public class FunctionArgs : EventArgs
	{
		private object _result;
		private Expression[] _parameters = new Expression[0];
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
		public Expression[] Parameters
		{
			get
			{
				return this._parameters;
			}
			set
			{
				this._parameters = value;
			}
		}
		public object[] EvaluateParameters()
		{
			object[] array = new object[this._parameters.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = this._parameters[i].Evaluate();
			}
			return array;
		}
	}
}
