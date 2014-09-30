using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
namespace EcmaScript.NET.Types.Cli
{
	[ComVisible(true)]
	public class CliEventInfo : CliObject
	{
		public class ScriptableCallback
		{
			public Context Context;
			public IScriptable Scope;
			public IScriptable ThisObj;
			public BuiltinFunction Function;
			public object ScriptInvoke(object[] args)
			{
				return this.Function.Call(this.Context, this.Scope, this.ThisObj, args);
			}
		}
		private EventInfo m_EventInfo = null;
		private static Hashtable m_EventHandlerTypes = Hashtable.Synchronized(new Hashtable());
		public override string ClassName
		{
			get
			{
				return "NativeCliEventInfo";
			}
		}
		public CliEventInfo(EventInfo ei)
		{
			this.m_EventInfo = ei;
			base.Init(this.m_EventInfo, this.m_EventInfo.GetType());
		}
		internal object Add(object val2, Context cx)
		{
			object result;
			if (!(val2 is InterpretedFunction))
			{
				result = this;
			}
			else
			{
				InterpretedFunction interpretedFunction = (InterpretedFunction)val2;
				CliEventInfo.ScriptableCallback scriptableCallback = (CliEventInfo.ScriptableCallback)Activator.CreateInstance(CliEventInfo.GetOrCreateType(this.m_EventInfo.EventHandlerType));
				scriptableCallback.Context = cx;
				scriptableCallback.Function = interpretedFunction;
				scriptableCallback.ThisObj = base.ParentScope;
				scriptableCallback.Scope = interpretedFunction;
				this.m_EventInfo.AddEventHandler((base.ParentScope as CliObject).Object, Delegate.CreateDelegate(this.m_EventInfo.EventHandlerType, scriptableCallback, "DynMethod"));
				result = this;
			}
			return result;
		}
		internal object Del(object val2, Context cx)
		{
			object result;
			if (!(val2 is InterpretedFunction))
			{
				result = this;
			}
			else
			{
				InterpretedFunction interpretedFunction = (InterpretedFunction)val2;
				CliEventInfo.ScriptableCallback scriptableCallback = (CliEventInfo.ScriptableCallback)Activator.CreateInstance(CliEventInfo.GetOrCreateType(this.m_EventInfo.EventHandlerType));
				scriptableCallback.Context = cx;
				scriptableCallback.Function = interpretedFunction;
				scriptableCallback.ThisObj = base.ParentScope;
				scriptableCallback.Scope = interpretedFunction;
				this.m_EventInfo.RemoveEventHandler((base.ParentScope as CliObject).Object, Delegate.CreateDelegate(this.m_EventInfo.EventHandlerType, scriptableCallback, "DynMethod"));
				result = this;
			}
			return result;
		}
		private static Type GetOrCreateType(Type eventHandlerType)
		{
			Type result;
			if (CliEventInfo.m_EventHandlerTypes.Contains(eventHandlerType))
			{
				result = (Type)CliEventInfo.m_EventHandlerTypes[eventHandlerType];
			}
			else
			{
				MethodInfo method = eventHandlerType.GetMethod("Invoke");
				Type[] parameterTypes = CliHelper.GetParameterTypes(method.GetParameters());
				AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("Dyn_" + eventHandlerType.FullName.Replace(".", "_")), AssemblyBuilderAccess.Run);
				ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("DynModule");
				TypeBuilder typeBuilder = moduleBuilder.DefineType("DynType", TypeAttributes.Public, typeof(CliEventInfo.ScriptableCallback));
				MethodBuilder methodBuilder = typeBuilder.DefineMethod("DynMethod", MethodAttributes.Public, CallingConventions.Standard, method.ReturnType, parameterTypes);
				ILGenerator iLGenerator = methodBuilder.GetILGenerator();
				iLGenerator.DeclareLocal(typeof(object[]));
				int num = method.GetParameters().Length;
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Ldc_I4, num);
				iLGenerator.Emit(OpCodes.Newarr, typeof(object));
				iLGenerator.Emit(OpCodes.Stloc_0);
				for (int i = 0; i < num; i++)
				{
					iLGenerator.Emit(OpCodes.Ldloc_0);
					iLGenerator.Emit(OpCodes.Ldc_I4, i);
					iLGenerator.Emit(OpCodes.Ldarg, i + 1);
					iLGenerator.Emit(OpCodes.Stelem_Ref);
				}
				iLGenerator.Emit(OpCodes.Ldloc_0);
				iLGenerator.EmitCall(OpCodes.Call, typeof(CliEventInfo.ScriptableCallback).GetMethod("ScriptInvoke"), null);
				iLGenerator.Emit(OpCodes.Pop);
				iLGenerator.Emit(OpCodes.Ret);
				Type type = typeBuilder.CreateType();
				CliEventInfo.m_EventHandlerTypes[eventHandlerType] = type;
				result = type;
			}
			return result;
		}
	}
}
