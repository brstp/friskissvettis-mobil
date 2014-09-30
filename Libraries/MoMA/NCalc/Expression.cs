using Antlr.Runtime;
using NCalc.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
namespace NCalc
{
	public class Expression
	{
		protected string OriginalExpression;
		private static bool _cacheEnabled = true;
		private static Dictionary<string, WeakReference> _compiledExpressions = new Dictionary<string, WeakReference>();
		private static readonly ReaderWriterLock Rwl = new ReaderWriterLock();
		protected Dictionary<string, IEnumerator> ParameterEnumerators;
		protected Dictionary<string, object> ParametersBackup;
		private Dictionary<string, object> _parameters;
		public event EvaluateFunctionHandler EvaluateFunction
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.EvaluateFunction = (EvaluateFunctionHandler)Delegate.Combine(this.EvaluateFunction, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.EvaluateFunction = (EvaluateFunctionHandler)Delegate.Remove(this.EvaluateFunction, value);
			}
		}
		public event EvaluateParameterHandler EvaluateParameter
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.EvaluateParameter = (EvaluateParameterHandler)Delegate.Combine(this.EvaluateParameter, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.EvaluateParameter = (EvaluateParameterHandler)Delegate.Remove(this.EvaluateParameter, value);
			}
		}
		public EvaluateOptions Options
		{
			get;
			set;
		}
		public static bool CacheEnabled
		{
			get
			{
				return Expression._cacheEnabled;
			}
			set
			{
				Expression._cacheEnabled = value;
				if (!Expression.CacheEnabled)
				{
					Expression._compiledExpressions = new Dictionary<string, WeakReference>();
				}
			}
		}
		public string Error
		{
			get;
			private set;
		}
		public LogicalExpression ParsedExpression
		{
			get;
			private set;
		}
		public Dictionary<string, object> Parameters
		{
			get
			{
				Dictionary<string, object> arg_1C_0;
				if ((arg_1C_0 = this._parameters) == null)
				{
					arg_1C_0 = (this._parameters = new Dictionary<string, object>());
				}
				return arg_1C_0;
			}
			set
			{
				this._parameters = value;
			}
		}
		public Expression(string expression) : this(expression, EvaluateOptions.None)
		{
		}
		public Expression(string expression, EvaluateOptions options)
		{
			if (string.IsNullOrEmpty(expression))
			{
				throw new ArgumentException("Expression can't be empty", "expression");
			}
			this.OriginalExpression = expression;
			this.Options = options;
		}
		public Expression(LogicalExpression expression) : this(expression, EvaluateOptions.None)
		{
		}
		public Expression(LogicalExpression expression, EvaluateOptions options)
		{
			if (expression == null)
			{
				throw new ArgumentException("Expression can't be null", "expression");
			}
			this.ParsedExpression = expression;
			this.Options = options;
		}
		private static void CleanCache()
		{
			List<string> list = new List<string>();
			try
			{
				Expression.Rwl.AcquireWriterLock(-1);
				foreach (KeyValuePair<string, WeakReference> current in Expression._compiledExpressions)
				{
					if (!current.Value.IsAlive)
					{
						list.Add(current.Key);
					}
				}
				foreach (string current2 in list)
				{
					Expression._compiledExpressions.Remove(current2);
					Trace.TraceInformation("Cache entry released: " + current2);
				}
			}
			finally
			{
				Expression.Rwl.ReleaseReaderLock();
			}
		}
		public static LogicalExpression Compile(string expression, bool nocache)
		{
			LogicalExpression logicalExpression = null;
			LogicalExpression result;
			if (Expression._cacheEnabled && !nocache)
			{
				try
				{
					Expression.Rwl.AcquireReaderLock(-1);
					if (Expression._compiledExpressions.ContainsKey(expression))
					{
						Trace.TraceInformation("Expression retrieved from cache: " + expression);
						WeakReference weakReference = Expression._compiledExpressions[expression];
						logicalExpression = (weakReference.Target as LogicalExpression);
						if (weakReference.IsAlive && logicalExpression != null)
						{
							result = logicalExpression;
							return result;
						}
					}
				}
				finally
				{
					Expression.Rwl.ReleaseReaderLock();
				}
			}
			if (logicalExpression == null)
			{
				NCalcLexer tokenSource = new NCalcLexer(new ANTLRStringStream(expression));
				NCalcParser nCalcParser = new NCalcParser(new CommonTokenStream(tokenSource));
				logicalExpression = nCalcParser.ncalcExpression().value;
				if (nCalcParser.Errors != null && nCalcParser.Errors.Count > 0)
				{
					throw new EvaluationException(string.Join(Environment.NewLine, nCalcParser.Errors.ToArray()));
				}
				if (Expression._cacheEnabled && !nocache)
				{
					try
					{
						Expression.Rwl.AcquireWriterLock(-1);
						Expression._compiledExpressions[expression] = new WeakReference(logicalExpression);
					}
					finally
					{
						Expression.Rwl.ReleaseWriterLock();
					}
					Expression.CleanCache();
					Trace.TraceInformation("Expression added to cache: " + expression);
				}
			}
			result = logicalExpression;
			return result;
		}
		public bool HasErrors()
		{
			bool result;
			try
			{
				if (this.ParsedExpression == null)
				{
					this.ParsedExpression = Expression.Compile(this.OriginalExpression, (this.Options & EvaluateOptions.NoCache) == EvaluateOptions.NoCache);
				}
				result = (this.ParsedExpression != null && this.Error != null);
			}
			catch (Exception ex)
			{
				this.Error = ex.Message;
				result = true;
			}
			return result;
		}
		public object Evaluate()
		{
			if (this.HasErrors())
			{
				throw new EvaluationException(this.Error);
			}
			if (this.ParsedExpression == null)
			{
				this.ParsedExpression = Expression.Compile(this.OriginalExpression, (this.Options & EvaluateOptions.NoCache) == EvaluateOptions.NoCache);
			}
			EvaluationVisitor evaluationVisitor = new EvaluationVisitor(this.Options);
			evaluationVisitor.EvaluateFunction += this.EvaluateFunction;
			evaluationVisitor.EvaluateParameter += this.EvaluateParameter;
			evaluationVisitor.Parameters = this.Parameters;
			object result;
			if ((this.Options & EvaluateOptions.IterateParameters) == EvaluateOptions.IterateParameters)
			{
				int num = -1;
				this.ParametersBackup = new Dictionary<string, object>();
				foreach (string current in this.Parameters.Keys)
				{
					this.ParametersBackup.Add(current, this.Parameters[current]);
				}
				this.ParameterEnumerators = new Dictionary<string, IEnumerator>();
				foreach (object current2 in this.Parameters.Values)
				{
					if (current2 is IEnumerable)
					{
						int num2 = 0;
						foreach (object current3 in (IEnumerable)current2)
						{
							num2++;
						}
						if (num == -1)
						{
							num = num2;
						}
						else
						{
							if (num2 != num)
							{
								throw new EvaluationException("When IterateParameters option is used, IEnumerable parameters must have the same number of items");
							}
						}
					}
				}
				foreach (string current in this.Parameters.Keys)
				{
					IEnumerable enumerable = this.Parameters[current] as IEnumerable;
					if (enumerable != null)
					{
						this.ParameterEnumerators.Add(current, enumerable.GetEnumerator());
					}
				}
				List<object> list = new List<object>();
				for (int i = 0; i < num; i++)
				{
					foreach (string current in this.ParameterEnumerators.Keys)
					{
						IEnumerator enumerator5 = this.ParameterEnumerators[current];
						enumerator5.MoveNext();
						this.Parameters[current] = enumerator5.Current;
					}
					this.ParsedExpression.Accept(evaluationVisitor);
					list.Add(evaluationVisitor.Result);
				}
				result = list;
			}
			else
			{
				this.ParsedExpression.Accept(evaluationVisitor);
				result = evaluationVisitor.Result;
			}
			return result;
		}
	}
}
