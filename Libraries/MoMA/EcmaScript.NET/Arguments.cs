using EcmaScript.NET.Types;
using System;
using System.Threading;
namespace EcmaScript.NET
{
	internal sealed class Arguments : IdScriptableObject
	{
		private const int Id_callee = 1;
		private const int Id_length = 2;
		private const int Id_caller = 3;
		private const int MAX_INSTANCE_ID = 3;
		private object callerObj;
		private object calleeObj;
		private object lengthObj;
		private BuiltinCall activation;
		private object[] args;
		public override string ClassName
		{
			get
			{
				return "Arguments";
			}
		}
		protected internal override int MaxInstanceId
		{
			get
			{
				return 3;
			}
		}
		public Arguments(BuiltinCall activation)
		{
			this.activation = activation;
			IScriptable parentScope = activation.ParentScope;
			base.ParentScope = parentScope;
			this.SetPrototype(ScriptableObject.GetObjectPrototype(parentScope));
			this.args = activation.originalArgs;
			this.lengthObj = this.args.Length;
			BuiltinFunction function = activation.function;
			this.calleeObj = function;
			Context.Versions languageVersion = function.LanguageVersion;
			if (languageVersion <= Context.Versions.JS1_3 && languageVersion != Context.Versions.Default)
			{
				this.callerObj = null;
			}
			else
			{
				this.callerObj = UniqueTag.NotFound;
			}
		}
		public override bool Has(int index, IScriptable start)
		{
			bool result;
			if (0 <= index && index < this.args.Length)
			{
				if (this.args[index] != UniqueTag.NotFound)
				{
					result = true;
					return result;
				}
			}
			result = base.Has(index, start);
			return result;
		}
		public override object Get(int index, IScriptable start)
		{
			object result;
			if (0 <= index && index < this.args.Length)
			{
				object obj = this.args[index];
				if (obj != UniqueTag.NotFound)
				{
					if (this.sharedWithActivation(index))
					{
						BuiltinFunction function = this.activation.function;
						string paramOrVarName = function.getParamOrVarName(index);
						obj = this.activation.Get(paramOrVarName, this.activation);
						if (obj == UniqueTag.NotFound)
						{
							Context.CodeBug();
						}
					}
					result = obj;
					return result;
				}
			}
			result = base.Get(index, start);
			return result;
		}
		private bool sharedWithActivation(int index)
		{
			BuiltinFunction function = this.activation.function;
			int paramCount = function.ParamCount;
			bool result;
			if (index < paramCount)
			{
				if (index < paramCount - 1)
				{
					string paramOrVarName = function.getParamOrVarName(index);
					for (int i = index + 1; i < paramCount; i++)
					{
						if (paramOrVarName.Equals(function.getParamOrVarName(i)))
						{
							result = false;
							return result;
						}
					}
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
		public override object Put(int index, IScriptable start, object value)
		{
			object result;
			if (0 <= index && index < this.args.Length)
			{
				if (this.args[index] != UniqueTag.NotFound)
				{
					if (this.sharedWithActivation(index))
					{
						string paramOrVarName = this.activation.function.getParamOrVarName(index);
						result = this.activation.Put(paramOrVarName, this.activation, value);
						return result;
					}
					Monitor.Enter(this);
					try
					{
						if (this.args[index] != UniqueTag.NotFound)
						{
							if (this.args == this.activation.originalArgs)
							{
								this.args = new object[this.args.Length];
								this.args.CopyTo(this.args, 0);
							}
							this.args[index] = value;
							result = value;
							return result;
						}
					}
					finally
					{
						Monitor.Exit(this);
					}
				}
			}
			result = base.Put(index, start, value);
			return result;
		}
		public override void Delete(int index)
		{
			if (0 <= index && index < this.args.Length)
			{
				Monitor.Enter(this);
				try
				{
					if (this.args[index] != UniqueTag.NotFound)
					{
						if (this.args == this.activation.originalArgs)
						{
							this.args = new object[this.args.Length];
							this.args.CopyTo(this.args, 0);
						}
						this.args[index] = UniqueTag.NotFound;
						return;
					}
				}
				finally
				{
					Monitor.Exit(this);
				}
			}
			base.Delete(index);
		}
		protected internal override int FindInstanceIdInfo(string s)
		{
			int num = 0;
			string text = null;
			if (s.Length == 6)
			{
				int num2 = (int)s[5];
				if (num2 == 101)
				{
					text = "callee";
					num = 1;
				}
				else
				{
					if (num2 == 104)
					{
						text = "length";
						num = 2;
					}
					else
					{
						if (num2 == 114)
						{
							text = "caller";
							num = 3;
						}
					}
				}
			}
			if (text != null && text != s && !text.Equals(s))
			{
				num = 0;
			}
			int result;
			if (num == 0)
			{
				result = base.FindInstanceIdInfo(s);
			}
			else
			{
				switch (num)
				{
				case 1:
				case 2:
				case 3:
				{
					int attributes = 2;
					result = IdScriptableObject.InstanceIdInfo(attributes, num);
					break;
				}
				default:
					throw new ApplicationException();
				}
			}
			return result;
		}
		protected internal override string GetInstanceIdName(int id)
		{
			string result;
			switch (id)
			{
			case 1:
				result = "callee";
				break;
			case 2:
				result = "length";
				break;
			case 3:
				result = "caller";
				break;
			default:
				result = null;
				break;
			}
			return result;
		}
		protected internal override object GetInstanceIdValue(int id)
		{
			object result;
			switch (id)
			{
			case 1:
				result = this.calleeObj;
				break;
			case 2:
				result = this.lengthObj;
				break;
			case 3:
			{
				object obj = this.callerObj;
				if (obj == UniqueTag.NullValue)
				{
					obj = null;
				}
				else
				{
					if (obj == null)
					{
						BuiltinCall parentActivationCall = this.activation.parentActivationCall;
						if (parentActivationCall != null)
						{
							obj = parentActivationCall.Get("arguments", parentActivationCall);
						}
						else
						{
							obj = null;
						}
					}
				}
				result = obj;
				break;
			}
			default:
				result = base.GetInstanceIdValue(id);
				break;
			}
			return result;
		}
		protected internal override void SetInstanceIdValue(int id, object value)
		{
			switch (id)
			{
			case 1:
				this.calleeObj = value;
				break;
			case 2:
				this.lengthObj = value;
				break;
			case 3:
				this.callerObj = ((value != null) ? value : UniqueTag.NullValue);
				break;
			default:
				base.SetInstanceIdValue(id, value);
				break;
			}
		}
		internal override object[] GetIds(bool getAll)
		{
			object[] array = base.GetIds(getAll);
			if (getAll && this.args.Length != 0)
			{
				bool[] array2 = null;
				int num = this.args.Length;
				for (int num2 = 0; num2 != array.Length; num2++)
				{
					object obj = array[num2];
					if (obj is int)
					{
						int num3 = (int)obj;
						if (0 <= num3 && num3 < this.args.Length)
						{
							if (array2 == null)
							{
								array2 = new bool[this.args.Length];
							}
							if (!array2[num3])
							{
								array2[num3] = true;
								num--;
							}
						}
					}
				}
				if (num != 0)
				{
					object[] array3 = new object[num + array.Length];
					Array.Copy(array, 0, array3, num, array.Length);
					array = array3;
					int num4 = 0;
					for (int num2 = 0; num2 != this.args.Length; num2++)
					{
						if (array2 == null || !array2[num2])
						{
							array[num4] = num2;
							num4++;
						}
					}
					if (num4 != num)
					{
						Context.CodeBug();
					}
				}
			}
			return array;
		}
	}
}
