using System;
using System.Collections;
using System.Collections.Generic;
namespace Yahoo.Yui.Compressor
{
	public class ScriptOrFunctionScope
	{
		private readonly IDictionary<string, string> _hints = new SortedDictionary<string, string>();
		private readonly IDictionary<string, JavaScriptIdentifier> _identifiers = new SortedDictionary<string, JavaScriptIdentifier>();
		private bool _markedForMunging = true;
		public int BraceNesting
		{
			get;
			private set;
		}
		public ScriptOrFunctionScope ParentScope
		{
			get;
			private set;
		}
		private ArrayList SubScopes
		{
			get;
			set;
		}
		public int VarCount
		{
			get;
			set;
		}
		public ScriptOrFunctionScope(int braceNesting, ScriptOrFunctionScope parentScope)
		{
			this.BraceNesting = braceNesting;
			this.ParentScope = parentScope;
			this.SubScopes = new ArrayList();
			if (parentScope != null)
			{
				parentScope.SubScopes.Add(this);
			}
		}
		private ArrayList GetUsedSymbols()
		{
			ArrayList arrayList = new ArrayList();
			foreach (JavaScriptIdentifier current in this._identifiers.Values)
			{
				string value = current.MungedValue;
				if (string.IsNullOrEmpty(value))
				{
					value = current.Value;
				}
				arrayList.Add(value);
			}
			return arrayList;
		}
		private ArrayList GetAllUsedSymbols()
		{
			ArrayList arrayList = new ArrayList();
			for (ScriptOrFunctionScope scriptOrFunctionScope = this; scriptOrFunctionScope != null; scriptOrFunctionScope = scriptOrFunctionScope.ParentScope)
			{
				arrayList.AddRange(scriptOrFunctionScope.GetUsedSymbols());
			}
			return arrayList;
		}
		public JavaScriptIdentifier DeclareIdentifier(string symbol)
		{
			JavaScriptIdentifier javaScriptIdentifier = this._identifiers.ContainsKey(symbol) ? this._identifiers[symbol] : null;
			if (javaScriptIdentifier == null)
			{
				javaScriptIdentifier = new JavaScriptIdentifier(symbol, this);
				this._identifiers.Add(symbol, javaScriptIdentifier);
			}
			return javaScriptIdentifier;
		}
		public void Munge()
		{
			if (!this._markedForMunging)
			{
				return;
			}
			int num = 1;
			if (this.ParentScope != null)
			{
				ArrayList arrayList = new ArrayList();
				arrayList.AddRange(JavaScriptCompressor.Ones);
				foreach (string obj in this.GetAllUsedSymbols())
				{
					arrayList.Remove(obj);
				}
				if (arrayList.Count == 0)
				{
					num = 2;
					arrayList.AddRange(JavaScriptCompressor.Twos);
					foreach (string obj2 in this.GetAllUsedSymbols())
					{
						arrayList.Remove(obj2);
					}
				}
				if (arrayList.Count == 0)
				{
					num = 3;
					arrayList.AddRange(JavaScriptCompressor.Threes);
					foreach (string obj3 in this.GetAllUsedSymbols())
					{
						arrayList.Remove(obj3);
					}
				}
				if (arrayList.Count == 0)
				{
					throw new InvalidOperationException("The YUI Compressor ran out of symbols. Aborting...");
				}
				foreach (JavaScriptIdentifier current in this._identifiers.Values)
				{
					if (arrayList.Count == 0)
					{
						num++;
						if (num == 2)
						{
							arrayList.AddRange(JavaScriptCompressor.Twos);
						}
						else
						{
							if (num != 3)
							{
								throw new InvalidOperationException("The YUI Compressor ran out of symbols. Aborting...");
							}
							arrayList.AddRange(JavaScriptCompressor.Threes);
						}
						foreach (string obj4 in this.GetAllUsedSymbols())
						{
							arrayList.Remove(obj4);
						}
					}
					string mungedValue;
					if (current.MarkedForMunging)
					{
						mungedValue = (string)arrayList[0];
						arrayList.RemoveAt(0);
					}
					else
					{
						mungedValue = current.Value;
					}
					current.MungedValue = mungedValue;
				}
			}
			for (int i = 0; i < this.SubScopes.Count; i++)
			{
				ScriptOrFunctionScope scriptOrFunctionScope = (ScriptOrFunctionScope)this.SubScopes[i];
				scriptOrFunctionScope.Munge();
			}
		}
		public void PreventMunging()
		{
			if (this.ParentScope != null)
			{
				this._markedForMunging = false;
			}
		}
		public JavaScriptIdentifier GetIdentifier(string symbol)
		{
			if (!this._identifiers.ContainsKey(symbol))
			{
				return null;
			}
			return this._identifiers[symbol];
		}
		public void AddHint(string variableName, string variableType)
		{
			this._hints.Add(variableName, variableType);
		}
	}
}
