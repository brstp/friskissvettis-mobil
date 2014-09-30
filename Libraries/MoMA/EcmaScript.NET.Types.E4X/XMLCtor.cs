using System;
namespace EcmaScript.NET.Types.E4X
{
	internal class XMLCtor : IdFunctionObject
	{
		private const int Id_defaultSettings = 1;
		private const int Id_settings = 2;
		private const int Id_setSettings = 3;
		private const int MAX_FUNCTION_ID = 3;
		private const int Id_ignoreComments = 1;
		private const int Id_ignoreProcessingInstructions = 2;
		private const int Id_ignoreWhitespace = 3;
		private const int Id_prettyIndent = 4;
		private const int Id_prettyPrinting = 5;
		private const int MAX_INSTANCE_ID = 5;
		private static readonly object XMLCTOR_TAG = new object();
		private XMLLib lib = null;
		protected internal override int MaxInstanceId
		{
			get
			{
				return base.MaxInstanceId + 5;
			}
		}
		internal XMLCtor(XML xml, object tag, int id, int arity) : base(xml, tag, id, arity)
		{
			this.lib = xml.lib;
			base.ActivatePrototypeMap(3);
		}
		protected internal override int FindPrototypeId(string s)
		{
			int result = 0;
			string text = null;
			int length = s.Length;
			if (length == 8)
			{
				text = "settings";
				result = 2;
			}
			else
			{
				if (length == 11)
				{
					text = "setSettings";
					result = 3;
				}
				else
				{
					if (length == 15)
					{
						text = "defaultSettings";
						result = 1;
					}
				}
			}
			if (text != null && text != s && !text.Equals(s))
			{
				result = 0;
			}
			return result;
		}
		protected internal override void InitPrototypeId(int id)
		{
			int arity;
			string name;
			switch (id)
			{
			case 1:
				arity = 0;
				name = "defaultSettings";
				break;
			case 2:
				arity = 0;
				name = "settings";
				break;
			case 3:
				arity = 1;
				name = "setSettings";
				break;
			default:
				throw new ArgumentException(Convert.ToString(id));
			}
			base.InitPrototypeMethod(XMLCtor.XMLCTOR_TAG, id, name, arity);
		}
		public override object ExecIdCall(IdFunctionObject f, Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			object result;
			if (!f.HasTag(XMLCtor.XMLCTOR_TAG))
			{
				result = base.ExecIdCall(f, cx, scope, thisObj, args);
			}
			else
			{
				int methodId = f.MethodId;
				switch (methodId)
				{
				case 1:
				{
					this.lib.SetDefaultSettings();
					IScriptable scriptable = cx.NewObject(scope);
					this.WriteSetting(scriptable);
					result = scriptable;
					break;
				}
				case 2:
				{
					IScriptable scriptable = cx.NewObject(scope);
					this.WriteSetting(scriptable);
					result = scriptable;
					break;
				}
				case 3:
					if (args.Length == 0 || args[0] == null || args[0] == Undefined.Value)
					{
						this.lib.SetDefaultSettings();
					}
					else
					{
						if (args[0] is IScriptable)
						{
							this.ReadSettings((IScriptable)args[0]);
						}
					}
					result = Undefined.Value;
					break;
				default:
					throw new ArgumentException(Convert.ToString(methodId));
				}
			}
			return result;
		}
		protected internal override int FindInstanceIdInfo(string s)
		{
			int num = 0;
			string text = null;
			int length = s.Length;
			switch (length)
			{
			case 12:
				text = "prettyIndent";
				num = 4;
				break;
			case 13:
			case 15:
				break;
			case 14:
			{
				int num2 = (int)s[0];
				if (num2 == 105)
				{
					text = "ignoreComments";
					num = 1;
				}
				else
				{
					if (num2 == 112)
					{
						text = "prettyPrinting";
						num = 5;
					}
				}
				break;
			}
			case 16:
				text = "ignoreWhitespace";
				num = 3;
				break;
			default:
				if (length == 28)
				{
					text = "ignoreProcessingInstructions";
					num = 2;
				}
				break;
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
				case 4:
				case 5:
				{
					int attributes = 6;
					result = IdScriptableObject.InstanceIdInfo(attributes, base.MaxInstanceId + num);
					break;
				}
				default:
					throw new SystemException();
				}
			}
			return result;
		}
		protected internal override string GetInstanceIdName(int id)
		{
			string result;
			switch (id - base.MaxInstanceId)
			{
			case 1:
				result = "ignoreComments";
				break;
			case 2:
				result = "ignoreProcessingInstructions";
				break;
			case 3:
				result = "ignoreWhitespace";
				break;
			case 4:
				result = "prettyIndent";
				break;
			case 5:
				result = "prettyPrinting";
				break;
			default:
				result = base.GetInstanceIdName(id);
				break;
			}
			return result;
		}
		protected internal override object GetInstanceIdValue(int id)
		{
			object result;
			switch (id - base.MaxInstanceId)
			{
			case 1:
				result = this.lib.ignoreComments;
				break;
			case 2:
				result = this.lib.ignoreProcessingInstructions;
				break;
			case 3:
				result = this.lib.ignoreWhitespace;
				break;
			case 4:
				result = this.lib.prettyIndent;
				break;
			case 5:
				result = this.lib.prettyPrinting;
				break;
			default:
				result = base.GetInstanceIdValue(id);
				break;
			}
			return result;
		}
		protected internal override void SetInstanceIdValue(int id, object value_Renamed)
		{
			switch (id - base.MaxInstanceId)
			{
			case 1:
				this.lib.ignoreComments = ScriptConvert.ToBoolean(value_Renamed);
				break;
			case 2:
				this.lib.ignoreProcessingInstructions = ScriptConvert.ToBoolean(value_Renamed);
				break;
			case 3:
				this.lib.ignoreWhitespace = ScriptConvert.ToBoolean(value_Renamed);
				break;
			case 4:
				this.lib.prettyIndent = ScriptConvert.ToInt32(value_Renamed);
				break;
			case 5:
				this.lib.prettyPrinting = ScriptConvert.ToBoolean(value_Renamed);
				break;
			default:
				base.SetInstanceIdValue(id, value_Renamed);
				break;
			}
		}
		private void WriteSetting(IScriptable target)
		{
			for (int i = 1; i <= 5; i++)
			{
				int id = base.MaxInstanceId + i;
				string instanceIdName = this.GetInstanceIdName(id);
				object instanceIdValue = this.GetInstanceIdValue(id);
				ScriptableObject.PutProperty(target, instanceIdName, instanceIdValue);
			}
		}
		private void ReadSettings(IScriptable source)
		{
			for (int i = 1; i <= 5; i++)
			{
				int id = base.MaxInstanceId + i;
				string instanceIdName = this.GetInstanceIdName(id);
				object property = ScriptableObject.GetProperty(source, instanceIdName);
				if (property != UniqueTag.NotFound)
				{
					switch (i)
					{
					case 1:
					case 2:
					case 3:
					case 5:
						if (!(property is bool))
						{
							goto IL_A8;
						}
						break;
					case 4:
						if (!CliHelper.IsNumber(property))
						{
							goto IL_A8;
						}
						break;
					default:
						throw new SystemException();
					}
					this.SetInstanceIdValue(id, property);
				}
				IL_A8:;
			}
		}
	}
}
