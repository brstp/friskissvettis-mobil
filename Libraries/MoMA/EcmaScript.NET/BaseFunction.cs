using EcmaScript.NET.Types;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public class BaseFunction : IdScriptableObject, IFunction, IScriptable, ICallable
	{
		private const int Id_length = 1;
		private const int Id_arity = 2;
		private const int Id_name = 3;
		private const int Id_prototype = 4;
		private const int Id_arguments = 5;
		private const int MAX_INSTANCE_ID = 5;
		private const int Id_constructor = 1;
		private const int Id_toString = 2;
		private const int Id_toSource = 3;
		private const int Id_apply = 4;
		private const int Id_call = 5;
		private const int MAX_PROTOTYPE_ID = 5;
		private static readonly object FUNCTION_TAG = new object();
		private object prototypeProperty;
		private bool isPrototypePropertyImmune;
		public override string ClassName
		{
			get
			{
				return "Function";
			}
		}
		protected internal override int MaxInstanceId
		{
			get
			{
				return 5;
			}
		}
		public virtual object ImmunePrototypeProperty
		{
			set
			{
				if (this.isPrototypePropertyImmune)
				{
					throw new ApplicationException();
				}
				this.prototypeProperty = ((value != null) ? value : UniqueTag.NullValue);
				this.isPrototypePropertyImmune = true;
			}
		}
		public virtual int Arity
		{
			get
			{
				return 0;
			}
		}
		public virtual int Length
		{
			get
			{
				return 0;
			}
		}
		public virtual string FunctionName
		{
			get
			{
				return "";
			}
		}
		internal virtual object PrototypeProperty
		{
			get
			{
				object obj = this.prototypeProperty;
				if (obj == null)
				{
					Monitor.Enter(this);
					try
					{
						obj = this.prototypeProperty;
						if (obj == null)
						{
							this.SetupDefaultPrototype();
							obj = this.prototypeProperty;
						}
					}
					finally
					{
						Monitor.Exit(this);
					}
				}
				else
				{
					if (obj == UniqueTag.NullValue)
					{
						obj = null;
					}
				}
				return obj;
			}
		}
		private object Arguments
		{
			get
			{
				object obj = base.DefaultGet("arguments");
				object result;
				if (obj != UniqueTag.NotFound)
				{
					result = obj;
				}
				else
				{
					Context currentContext = Context.CurrentContext;
					BuiltinCall builtinCall = ScriptRuntime.findFunctionActivation(currentContext, this);
					result = ((builtinCall == null) ? null : builtinCall.Get("arguments", builtinCall));
				}
				return result;
			}
		}
		internal static void Init(IScriptable scope, bool zealed)
		{
			new BaseFunction
			{
				isPrototypePropertyImmune = true
			}.ExportAsJSClass(5, scope, zealed);
		}
		public BaseFunction()
		{
		}
		public BaseFunction(IScriptable scope, IScriptable prototype) : base(scope, prototype)
		{
		}
		public override bool HasInstance(IScriptable instance)
		{
			object property = ScriptableObject.GetProperty(this, "prototype");
			if (property is IScriptable)
			{
				return ScriptRuntime.jsDelegatesTo(instance, (IScriptable)property);
			}
			throw ScriptRuntime.TypeErrorById("msg.instanceof.bad.prototype", new string[]
			{
				this.FunctionName
			});
		}
		protected internal override int FindInstanceIdInfo(string s)
		{
			int num = 0;
			string text = null;
			switch (s.Length)
			{
			case 4:
				text = "name";
				num = 3;
				break;
			case 5:
				text = "arity";
				num = 2;
				break;
			case 6:
				text = "length";
				num = 1;
				break;
			case 9:
			{
				int num2 = (int)s[0];
				if (num2 == 97)
				{
					text = "arguments";
					num = 5;
				}
				else
				{
					if (num2 == 112)
					{
						text = "prototype";
						num = 4;
					}
				}
				break;
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
				int attributes;
				switch (num)
				{
				case 1:
				case 2:
				case 3:
					attributes = 7;
					break;
				case 4:
					attributes = (this.isPrototypePropertyImmune ? 7 : 4);
					break;
				case 5:
					attributes = 6;
					break;
				default:
					throw new ApplicationException();
				}
				result = IdScriptableObject.InstanceIdInfo(attributes, num);
			}
			return result;
		}
		protected internal override string GetInstanceIdName(int id)
		{
			string result;
			switch (id)
			{
			case 1:
				result = "length";
				break;
			case 2:
				result = "arity";
				break;
			case 3:
				result = "name";
				break;
			case 4:
				result = "prototype";
				break;
			case 5:
				result = "arguments";
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
			switch (id)
			{
			case 1:
				result = this.Length;
				break;
			case 2:
				result = this.Arity;
				break;
			case 3:
				result = this.FunctionName;
				break;
			case 4:
				result = this.PrototypeProperty;
				break;
			case 5:
				result = this.Arguments;
				break;
			default:
				result = base.GetInstanceIdValue(id);
				break;
			}
			return result;
		}
		protected internal override void SetInstanceIdValue(int id, object value)
		{
			if (id == 4)
			{
				if (!this.isPrototypePropertyImmune)
				{
					this.prototypeProperty = ((value != null) ? value : UniqueTag.NullValue);
				}
			}
			else
			{
				if (id == 5)
				{
					if (value == UniqueTag.NotFound)
					{
						Context.CodeBug();
					}
					base.DefaultPut("arguments", value);
				}
				base.SetInstanceIdValue(id, value);
			}
		}
		protected internal override void FillConstructorProperties(IdFunctionObject ctor)
		{
			ctor.SetPrototype(this);
			base.FillConstructorProperties(ctor);
		}
		protected internal override void InitPrototypeId(int id)
		{
			int arity;
			string name;
			switch (id)
			{
			case 1:
				arity = 1;
				name = "constructor";
				break;
			case 2:
				arity = 1;
				name = "toString";
				break;
			case 3:
				arity = 1;
				name = "toSource";
				break;
			case 4:
				arity = 2;
				name = "apply";
				break;
			case 5:
				arity = 1;
				name = "call";
				break;
			default:
				throw new ArgumentException(Convert.ToString(id));
			}
			base.InitPrototypeMethod(BaseFunction.FUNCTION_TAG, id, name, arity);
		}
		public override object ExecIdCall(IdFunctionObject f, Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			object result;
			if (!f.HasTag(BaseFunction.FUNCTION_TAG))
			{
				result = base.ExecIdCall(f, cx, scope, thisObj, args);
			}
			else
			{
				int methodId = f.MethodId;
				switch (methodId)
				{
				case 1:
					result = BaseFunction.JsConstructor(cx, scope, args);
					break;
				case 2:
				{
					BaseFunction baseFunction = this.RealFunction(thisObj, f);
					int num = ScriptConvert.ToInt32(args, 0);
					result = baseFunction.Decompile(num, 4);
					break;
				}
				case 3:
				{
					BaseFunction baseFunction = this.RealFunction(thisObj, f);
					int num = 0;
					int flags = 2;
					if (args.Length != 0)
					{
						num = ScriptConvert.ToInt32(args[0]);
						if (num >= 0)
						{
							flags = 0;
						}
						else
						{
							num = 0;
						}
					}
					result = baseFunction.Decompile(num, flags);
					break;
				}
				case 4:
				case 5:
					result = ScriptRuntime.applyOrCall(methodId == 4, cx, scope, thisObj, args);
					break;
				default:
					throw new ArgumentException(Convert.ToString(methodId));
				}
			}
			return result;
		}
		private BaseFunction RealFunction(IScriptable thisObj, IdFunctionObject f)
		{
			object defaultValue = thisObj.GetDefaultValue(typeof(IFunction));
			if (defaultValue is BaseFunction)
			{
				return (BaseFunction)defaultValue;
			}
			throw ScriptRuntime.TypeErrorById("msg.incompat.call", new string[]
			{
				f.FunctionName
			});
		}
		protected internal virtual IScriptable GetClassPrototype()
		{
			object obj = this.PrototypeProperty;
			IScriptable result;
			if (obj is IScriptable)
			{
				result = (IScriptable)obj;
			}
			else
			{
				result = ScriptableObject.getClassPrototype(this, "Object");
			}
			return result;
		}
		public virtual object Call(Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			return Undefined.Value;
		}
		public virtual IScriptable Construct(Context cx, IScriptable scope, object[] args)
		{
			IScriptable scriptable = this.CreateObject(cx, scope);
			if (scriptable != null)
			{
				object obj = this.Call(cx, scope, scriptable, args);
				if (obj is IScriptable)
				{
					scriptable = (IScriptable)obj;
				}
			}
			else
			{
				object obj = this.Call(cx, scope, null, args);
				if (!(obj is IScriptable))
				{
					throw new ApplicationException("Bad implementaion of call as constructor, name=" + this.FunctionName + " in " + base.GetType().FullName);
				}
				scriptable = (IScriptable)obj;
				if (scriptable.GetPrototype() == null)
				{
					scriptable.SetPrototype(this.GetClassPrototype());
				}
				if (scriptable.ParentScope == null)
				{
					IScriptable parentScope = base.ParentScope;
					if (scriptable != parentScope)
					{
						scriptable.ParentScope = parentScope;
					}
				}
			}
			return scriptable;
		}
		public virtual IScriptable CreateObject(Context cx, IScriptable scope)
		{
			IScriptable scriptable = new BuiltinObject();
			scriptable.SetPrototype(this.GetClassPrototype());
			scriptable.ParentScope = base.ParentScope;
			return scriptable;
		}
		internal virtual string Decompile(int indent, int flags)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = 0 != (flags & 1);
			if (!flag)
			{
				stringBuilder.Append("function ");
				stringBuilder.Append(this.FunctionName);
				stringBuilder.Append("() {\n\t");
			}
			stringBuilder.Append("[native code, arity=");
			stringBuilder.Append(this.Arity);
			stringBuilder.Append("]\n");
			if (!flag)
			{
				stringBuilder.Append("}\n");
			}
			return stringBuilder.ToString();
		}
		private void SetupDefaultPrototype()
		{
			BuiltinObject builtinObject = new BuiltinObject();
			builtinObject.DefineProperty("constructor", this, 2);
			this.prototypeProperty = builtinObject;
			IScriptable objectPrototype = ScriptableObject.GetObjectPrototype(this);
			if (objectPrototype != builtinObject)
			{
				builtinObject.SetPrototype(objectPrototype);
			}
		}
		private static object JsConstructor(Context cx, IScriptable scope, object[] args)
		{
			int num = args.Length;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("function ");
			if (cx.Version != Context.Versions.JS1_2)
			{
				stringBuilder.Append("anonymous");
			}
			stringBuilder.Append('(');
			for (int i = 0; i < num - 1; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append(ScriptConvert.ToString(args[i]));
			}
			stringBuilder.Append(") {");
			if (num != 0)
			{
				string value = ScriptConvert.ToString(args[num - 1]);
				stringBuilder.Append(value);
			}
			stringBuilder.Append('}');
			string source = stringBuilder.ToString();
			int[] array = new int[1];
			string text = Context.GetSourcePositionFromStack(array);
			if (text == null)
			{
				text = "<eval'ed string>";
				array[0] = 1;
			}
			string sourceName = ScriptRuntime.makeUrlForGeneratedScript(false, text, array[0]);
			IScriptable topLevelScope = ScriptableObject.GetTopLevelScope(scope);
			ErrorReporter compilationErrorReporter = DefaultErrorReporter.ForEval(cx.ErrorReporter);
			return cx.CompileFunction(topLevelScope, source, new Interpreter(), compilationErrorReporter, sourceName, 1, null);
		}
		protected internal override int FindPrototypeId(string s)
		{
			int result = 0;
			string text = null;
			int length = s.Length;
			switch (length)
			{
			case 4:
				text = "call";
				result = 5;
				break;
			case 5:
				text = "apply";
				result = 4;
				break;
			case 6:
			case 7:
				break;
			case 8:
			{
				int num = (int)s[3];
				if (num == 111)
				{
					text = "toSource";
					result = 3;
				}
				else
				{
					if (num == 116)
					{
						text = "toString";
						result = 2;
					}
				}
				break;
			}
			default:
				if (length == 11)
				{
					text = "constructor";
					result = 1;
				}
				break;
			}
			if (text != null && text != s && !text.Equals(s))
			{
				result = 0;
			}
			return result;
		}
	}
}
