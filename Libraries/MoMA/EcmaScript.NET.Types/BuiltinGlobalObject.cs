using System;
using System.Runtime.InteropServices;
using System.Text;
namespace EcmaScript.NET.Types
{
	[ComVisible(true)]
	public class BuiltinGlobalObject : IdScriptableObject
	{
		private const int Id_print = 1;
		private const int Id_version = 2;
		private const int Id_options = 3;
		private const int Id_gc = 4;
		private const int MAX_PROTOTYPE_ID = 4;
		private static readonly object GLOBALOBJECT_TAG = new object();
		public override string ClassName
		{
			get
			{
				return "global";
			}
		}
		public override object ExecIdCall(IdFunctionObject f, Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			object result;
			switch (f.MethodId)
			{
			case 1:
				for (int i = 0; i < args.Length; i++)
				{
					if (i > 0)
					{
						Console.Out.Write(" ");
					}
					Console.Out.Write(ScriptConvert.ToString(args[i]));
				}
				Console.Out.WriteLine();
				result = Undefined.Value;
				break;
			case 2:
				if (args.Length > 0)
				{
					if (CliHelper.IsNumber(args[0]))
					{
						int version = (int)ScriptConvert.ToNumber(args[0]);
						if (Context.IsValidLanguageVersion(version))
						{
							cx.Version = Context.ToValidLanguageVersion(version);
						}
					}
				}
				result = (int)cx.Version;
				break;
			case 3:
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (cx.HasFeature(Context.Features.Strict))
				{
					stringBuilder.Append("strict");
				}
				result = stringBuilder.ToString();
				break;
			}
			case 4:
				GC.Collect();
				result = Undefined.Value;
				break;
			default:
				throw f.Unknown();
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
				name = "print";
				break;
			case 2:
				arity = 0;
				name = "version";
				break;
			case 3:
				arity = 0;
				name = "options";
				break;
			case 4:
				arity = 0;
				name = "gc";
				break;
			default:
				throw new ArgumentException(Convert.ToString(id));
			}
			base.InitPrototypeMethod(BuiltinGlobalObject.GLOBALOBJECT_TAG, id, name, arity);
		}
		internal override object[] GetIds(bool getAll)
		{
			return base.GetIds(getAll);
		}
		public void Init(IScriptable scope, bool zealed)
		{
			base.ActivatePrototypeMap(4);
		}
		protected internal override int FindPrototypeId(string s)
		{
			int result = 0;
			string text = null;
			int length = s.Length;
			if (length == 2)
			{
				if (s[0] == 'g' && s[1] == 'c')
				{
					result = 4;
					return result;
				}
			}
			else
			{
				if (length == 5)
				{
					text = "print";
					result = 1;
				}
				else
				{
					if (length == 7)
					{
						int num = (int)s[0];
						if (num == 111)
						{
							text = "options";
							result = 3;
						}
						else
						{
							if (num == 118)
							{
								text = "version";
								result = 2;
							}
						}
					}
				}
			}
			if (text != null && text != s && !text.Equals(s))
			{
				result = 0;
			}
			return result;
		}
		private int HideIfNotSet(int id)
		{
			id = this.HideIfNotSet(Context.Features.NonEcmaPrintFunction, id, 1);
			id = this.HideIfNotSet(Context.Features.NonEcmaOptionsFunction, id, 3);
			id = this.HideIfNotSet(Context.Features.NonEcmaVersionFunction, id, 2);
			id = this.HideIfNotSet(Context.Features.Strict, id, 4);
			return id;
		}
		private int HideIfNotSet(Context.Features feature, int id, int requiredId)
		{
			int result;
			if (id != requiredId)
			{
				result = id;
			}
			else
			{
				if (Context.CurrentContext.HasFeature(feature))
				{
					result = id;
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}
	}
}
