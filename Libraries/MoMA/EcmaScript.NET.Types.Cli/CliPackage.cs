using EcmaScript.NET.Attributes;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
namespace EcmaScript.NET.Types.Cli
{
	[EcmaScriptClass("Cli"), ComVisible(true)]
	public class CliPackage : CliObject
	{
		public static void Init(IScriptable scope)
		{
			CliPackage cliPackage = new CliPackage();
			ScriptableObject.DefineProperty(scope, "cli", cliPackage, 2);
			cliPackage.ParentScope = scope;
		}
		[EcmaScriptFunction("load")]
		public void Load(string assembly)
		{
			try
			{
				CliPackage.ImportAssembly(base.ParentScope, Assembly.LoadWithPartialName(assembly));
			}
			catch (FileNotFoundException ex)
			{
				throw ScriptRuntime.ConstructError("EvalError", "Failed to load assembly: " + ex.Message);
			}
		}
		[EcmaScriptFunction("using")]
		public void Using(string ns)
		{
			CliPackage.ImportTypes(base.ParentScope, ns);
		}
		public static void ImportAssembly(IScriptable scope, Assembly ass)
		{
			CliPackage.ImportAssembly(scope, ass, null);
		}
		public static void ImportAssembly(IScriptable scope, Assembly ass, string startsWith)
		{
			Type[] types = ass.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				Type type = types[i];
				if (startsWith == null || type.FullName.StartsWith(startsWith))
				{
					CliPackage.ImportType(scope, type);
				}
			}
		}
		public static void ImportTypes(IScriptable scope, string startsWith)
		{
			Assembly[] assemblies = Context.CurrentContext.AppDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				Assembly ass = assemblies[i];
				CliPackage.ImportAssembly(scope, ass, startsWith);
			}
		}
		public static void ImportType(IScriptable scope, Type type)
		{
			if (type.IsPublic)
			{
				if (!ScriptRuntime.IsNativeRuntimeType(type))
				{
					if (!(type.Name == "Object"))
					{
						string[] array = type.FullName.Split(new char[]
						{
							'.'
						});
						IScriptable scriptable = scope;
						for (int i = 0; i < array.Length - 1; i++)
						{
							IScriptable scriptable2 = scriptable.Get(array[i], scriptable) as IScriptable;
							if (scriptable2 == null)
							{
								scriptable2 = new BuiltinObject();
								scriptable.Put(array[i], scriptable, scriptable2);
							}
							scriptable = scriptable2;
						}
						object value;
						if (type.IsEnum)
						{
							value = new CliEnum((Enum)Activator.CreateInstance(type));
						}
						else
						{
							value = CliType.GetNativeCliType(type);
						}
						scope.Put(array[array.Length - 1], scope, value);
						scriptable.Put(array[array.Length - 1], scriptable, value);
					}
				}
			}
		}
	}
}
