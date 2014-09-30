using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public interface IScriptable
	{
		string ClassName
		{
			get;
		}
		IScriptable ParentScope
		{
			get;
			set;
		}
		object Get(string name, IScriptable start);
		object Get(int index, IScriptable start);
		bool Has(string name, IScriptable start);
		bool Has(int index, IScriptable start);
		object Put(string name, IScriptable start, object value);
		object Put(int index, IScriptable start, object value);
		void Delete(string name);
		void Delete(int index);
		IScriptable GetPrototype();
		void SetPrototype(IScriptable prototype);
		object[] GetIds();
		object GetDefaultValue(Type hint);
		bool HasInstance(IScriptable instance);
	}
}
