using EcmaScript.NET.Collections;
using EcmaScript.NET.Debugging;
using EcmaScript.NET.Types;
using EcmaScript.NET.Types.Cli;
using EcmaScript.NET.Types.E4X;
using EcmaScript.NET.Types.RegExp;
using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public class Context : IDisposable
	{
		public enum Features
		{
			None,
			E4x = 2,
			GetterAndSetter = 4,
			NonEcmaGetYear = 8,
			DynamicScope = 16,
			MemberExprAsFunctionName = 32,
			ReservedKeywordAsIdentifier = 64,
			ToStringAsSource = 128,
			ParentProtoProperties = 256,
			StrictVars = 512,
			StrictEval = 1024,
			NonEcmaPrintFunction = 2048,
			NonEcmaVersionFunction = 4096,
			NonEcmaOptionsFunction = 8192,
			Strict = 16384,
			NonEcmaGcFunction = 16384,
			NonEcmaItObject = 32768
		}
		public enum Versions
		{
			Unknown = -1,
			Default,
			JS1_0 = 100,
			JS1_1 = 110,
			JS1_2 = 120,
			JS1_3 = 130,
			JS1_4 = 140,
			JS1_5 = 150,
			JS1_6 = 160
		}
		public const string languageVersionProperty = "language version";
		public const string errorReporterProperty = "error reporter";
		private int m_MaximumInterpreterStackDepth = 8092;
		private AppDomain m_AppDomain = null;
		public static readonly object[] EmptyArgs;
		private Context.Features m_Features = Context.Features.None;
		private static string implementationVersion;
		private ContextFactory factory;
		private bool m_Sealed;
		private object m_SealKey;
		internal IScriptable topCallScope;
		internal BuiltinCall currentActivationCall;
		internal XMLLib cachedXMLLib;
		internal ObjToIntMap iterating;
		internal object interpreterSecurityDomain;
		internal Context.Versions m_Version = Context.Versions.Unknown;
		private SecurityController securityController;
		private ErrorReporter m_ErrorReporter;
		internal RegExpProxy regExpProxy;
		private CultureInfo culture;
		private bool generatingDebug;
		private bool generatingDebugChanged;
		private bool generatingSource = true;
		internal bool compileFunctionsWithDynamicScopeFlag = false;
		internal bool useDynamicScope;
		private int m_OptimizationLevel;
		internal EcmaScript.NET.Debugging.Debugger m_Debugger;
		private object debuggerData;
		private int enterCount;
		private Hashtable hashtable;
		private bool creationEventWasSent;
		internal Hashtable activationNames;
		internal object lastInterpreterFrame;
		internal ObjArray previousInterpreterInvocations;
		internal int instructionCount;
		internal int instructionThreshold;
		internal int scratchIndex;
		internal long scratchUint32;
		internal IScriptable scratchScriptable;
		public event ContextWrapHandler OnWrap;
		public int MaximumInterpreterStackDepth
		{
			get
			{
				return this.m_MaximumInterpreterStackDepth;
			}
			set
			{
				if (this.Sealed)
				{
					Context.OnSealedMutation();
				}
				this.m_MaximumInterpreterStackDepth = value;
			}
		}
		public AppDomain AppDomain
		{
			get
			{
				return this.m_AppDomain;
			}
		}
		private static LocalDataStoreSlot LocalSlot
		{
			get
			{
				LocalDataStoreSlot localDataStoreSlot = Thread.GetNamedDataSlot("Context");
				if (localDataStoreSlot == null)
				{
					localDataStoreSlot = Thread.AllocateNamedDataSlot("Context");
				}
				return localDataStoreSlot;
			}
		}
		public static Context CurrentContext
		{
			get
			{
				return (Context)Thread.GetData(Context.LocalSlot);
			}
		}
		public ContextFactory Factory
		{
			get
			{
				ContextFactory global = this.factory;
				if (global == null)
				{
					global = ContextFactory.Global;
				}
				return global;
			}
		}
		public bool Sealed
		{
			get
			{
				return this.m_Sealed;
			}
		}
		public string ImplementationVersion
		{
			get
			{
				if (Context.implementationVersion == null)
				{
					Context.implementationVersion = ScriptRuntime.GetMessage("implementation.version", new object[0]);
				}
				return Context.implementationVersion;
			}
		}
		public static object UndefinedValue
		{
			get
			{
				return Undefined.Value;
			}
		}
		public bool GeneratingDebug
		{
			get
			{
				return this.generatingDebug;
			}
			set
			{
				if (this.m_Sealed)
				{
					Context.OnSealedMutation();
				}
				this.generatingDebugChanged = true;
				if (value && this.OptimizationLevel > 0)
				{
					this.OptimizationLevel = 0;
				}
				this.generatingDebug = value;
			}
		}
		public bool GeneratingSource
		{
			get
			{
				return this.generatingSource;
			}
			set
			{
				if (this.m_Sealed)
				{
					Context.OnSealedMutation();
				}
				this.generatingSource = value;
			}
		}
		public int OptimizationLevel
		{
			get
			{
				return this.m_OptimizationLevel;
			}
			set
			{
				if (this.m_Sealed)
				{
					Context.OnSealedMutation();
				}
				if (value == -2)
				{
					value = -1;
				}
				Context.CheckOptimizationLevel(value);
				this.m_OptimizationLevel = value;
			}
		}
		public object DebuggerContextData
		{
			get
			{
				return this.debuggerData;
			}
		}
		public int InstructionObserverThreshold
		{
			get
			{
				return this.instructionThreshold;
			}
			set
			{
				if (this.m_Sealed)
				{
					Context.OnSealedMutation();
				}
				if (value < 0)
				{
					throw new ArgumentException();
				}
				this.instructionThreshold = value;
			}
		}
		internal RegExpProxy RegExpProxy
		{
			get
			{
				if (this.regExpProxy == null)
				{
					this.regExpProxy = new RegExpImpl();
				}
				return this.regExpProxy;
			}
			set
			{
				this.regExpProxy = value;
			}
		}
		internal bool VersionECMA1
		{
			get
			{
				return this.m_Version == Context.Versions.Default || this.m_Version >= Context.Versions.JS1_3;
			}
		}
		public bool GeneratingDebugChanged
		{
			get
			{
				return this.generatingDebugChanged;
			}
		}
		public Context.Versions Version
		{
			get
			{
				return this.m_Version;
			}
			set
			{
				if (this.m_Sealed)
				{
					Context.OnSealedMutation();
				}
				this.m_Version = value;
				this.InitDefaultFeatures();
			}
		}
		public ErrorReporter ErrorReporter
		{
			get
			{
				ErrorReporter result;
				if (this.m_ErrorReporter == null)
				{
					result = DefaultErrorReporter.instance;
				}
				else
				{
					result = this.m_ErrorReporter;
				}
				return result;
			}
			set
			{
				if (this.m_Sealed)
				{
					Context.OnSealedMutation();
				}
				if (value == null)
				{
					throw new ArgumentException();
				}
				this.m_ErrorReporter = value;
			}
		}
		public CultureInfo CurrentCulture
		{
			get
			{
				if (this.culture == null)
				{
					this.culture = Thread.CurrentThread.CurrentCulture;
				}
				return this.culture;
			}
			set
			{
				if (this.m_Sealed)
				{
					Context.OnSealedMutation();
				}
				this.culture = value;
				if (Thread.CurrentThread.CurrentCulture != this.culture)
				{
					Thread.CurrentThread.CurrentCulture = this.culture;
				}
			}
		}
		public SecurityController SecurityController
		{
			get
			{
				SecurityController global = SecurityController.Global;
				SecurityController result;
				if (global != null)
				{
					result = global;
				}
				else
				{
					result = this.securityController;
				}
				return result;
			}
			set
			{
				if (this.m_Sealed)
				{
					Context.OnSealedMutation();
				}
				if (value == null)
				{
					throw new ArgumentException();
				}
				if (this.securityController != null)
				{
					throw new SecurityException("Can not overwrite existing SecurityController object");
				}
				if (SecurityController.HasGlobal())
				{
					throw new SecurityException("Can not overwrite existing global SecurityController object");
				}
				this.securityController = value;
			}
		}
		public EcmaScript.NET.Debugging.Debugger Debugger
		{
			get
			{
				return this.m_Debugger;
			}
		}
		public object Wrap(IScriptable scope, object obj, Type staticType)
		{
			object result;
			if (obj == null || obj is IScriptable)
			{
				result = obj;
			}
			else
			{
				if (staticType == null)
				{
					staticType = obj.GetType();
				}
				if (staticType.IsArray)
				{
					result = new CliArray(scope, obj as Array);
				}
				else
				{
					if (staticType.IsPrimitive)
					{
						result = obj;
					}
					else
					{
						if (this.OnWrap != null)
						{
							ContextWrapEventArgs contextWrapEventArgs = new ContextWrapEventArgs(this, scope, obj, staticType);
							this.OnWrap(this, contextWrapEventArgs);
							obj = contextWrapEventArgs.Target;
						}
						result = new CliObject(obj);
					}
				}
			}
			return result;
		}
		public Context(AppDomain appDomain)
		{
			if (appDomain == null)
			{
				throw new ArgumentNullException("appDomain");
			}
			this.Version = Context.Versions.Default;
			this.m_AppDomain = appDomain;
		}
		public static Context Enter()
		{
			return Context.Enter(null, AppDomain.CurrentDomain);
		}
		public static Context Enter(AppDomain appDomain)
		{
			return Context.Enter(null, appDomain);
		}
		public static Context Enter(Context cx)
		{
			return Context.Enter(cx, AppDomain.CurrentDomain);
		}
		public static Context Enter(Context cx, AppDomain appDomain)
		{
			Context currentContext = Context.CurrentContext;
			Context result;
			if (currentContext != null)
			{
				if (cx != null && cx != currentContext && cx.enterCount != 0)
				{
					throw new ArgumentException("Cannot enter Context active on another thread");
				}
				if (currentContext.factory != null)
				{
					result = currentContext;
					return result;
				}
				if (currentContext.m_Sealed)
				{
					Context.OnSealedMutation();
				}
				cx = currentContext;
			}
			else
			{
				if (cx == null)
				{
					cx = new Context(appDomain);
				}
				else
				{
					if (cx.m_Sealed)
					{
						Context.OnSealedMutation();
					}
				}
				if (cx.enterCount != 0 || cx.factory != null)
				{
					throw new ApplicationException();
				}
				if (!cx.creationEventWasSent)
				{
					cx.creationEventWasSent = true;
					ContextFactory.Global.FireOnContextCreated(cx);
				}
			}
			if (currentContext == null)
			{
				Thread.SetData(Context.LocalSlot, cx);
			}
			cx.enterCount++;
			result = cx;
			return result;
		}
		public static void Exit()
		{
			Context currentContext = Context.CurrentContext;
			if (currentContext == null)
			{
				throw new ApplicationException("Calling Context.exit without previous Context.enter");
			}
			if (currentContext.factory == null)
			{
				if (currentContext.enterCount < 1)
				{
					Context.CodeBug();
				}
				if (currentContext.m_Sealed)
				{
					Context.OnSealedMutation();
				}
				currentContext.enterCount--;
				if (currentContext.enterCount == 0)
				{
					Thread.SetData(Context.LocalSlot, null);
					ContextFactory.Global.FireOnContextReleased(currentContext);
				}
			}
		}
		public static object Call(ContextFactory factory, ICallable callable, IScriptable scope, IScriptable thisObj, object[] args)
		{
			if (factory == null)
			{
				factory = ContextFactory.Global;
			}
			Context context = Context.CurrentContext;
			object result;
			if (context != null)
			{
				object obj;
				if (context.factory != null)
				{
					obj = callable.Call(context, scope, thisObj, args);
				}
				else
				{
					context.factory = factory;
					try
					{
						obj = callable.Call(context, scope, thisObj, args);
					}
					finally
					{
						context.factory = null;
					}
				}
				result = obj;
			}
			else
			{
				context = Context.PrepareNewContext(AppDomain.CurrentDomain, factory);
				try
				{
					result = callable.Call(context, scope, thisObj, args);
				}
				finally
				{
					Context.ReleaseContext(context);
				}
			}
			return result;
		}
		internal void InitDefaultFeatures()
		{
			this.m_Features = Context.Features.None;
			this.SetFeature(Context.Features.E4x, this.Version == Context.Versions.Default || this.Version >= Context.Versions.JS1_6);
			this.SetFeature(Context.Features.GetterAndSetter, this.Version == Context.Versions.Default || this.Version >= Context.Versions.JS1_5);
			this.SetFeature(Context.Features.NonEcmaGetYear, this.Version == Context.Versions.JS1_0 || this.Version == Context.Versions.JS1_1 || this.Version == Context.Versions.JS1_2);
			this.SetFeature(Context.Features.ToStringAsSource, this.Version == Context.Versions.JS1_2);
			this.SetFeature(Context.Features.ParentProtoProperties, true);
		}
		private static Context PrepareNewContext(AppDomain appDomain, ContextFactory factory)
		{
			Context context = new Context(appDomain);
			if (context.factory != null || context.enterCount != 0)
			{
				throw new ApplicationException("factory.makeContext() returned Context instance already associated with some thread");
			}
			context.factory = factory;
			factory.FireOnContextCreated(context);
			if (factory.Sealed && !context.Sealed)
			{
				context.Seal(null);
			}
			Thread.SetData(Context.LocalSlot, context);
			return context;
		}
		private static void ReleaseContext(Context cx)
		{
			Thread.SetData(Context.LocalSlot, null);
			try
			{
				cx.factory.FireOnContextReleased(cx);
			}
			finally
			{
				cx.factory = null;
			}
		}
		public void Seal(object sealKey)
		{
			if (this.m_Sealed)
			{
				Context.OnSealedMutation();
			}
			this.m_Sealed = true;
			this.m_SealKey = sealKey;
		}
		public void Unseal(object sealKey)
		{
			if (sealKey == null)
			{
				throw new ArgumentException();
			}
			if (this.m_SealKey != sealKey)
			{
				throw new ArgumentException();
			}
			if (!this.m_Sealed)
			{
				throw new ApplicationException();
			}
			this.m_Sealed = false;
			this.m_SealKey = null;
		}
		internal static void OnSealedMutation()
		{
			throw new ApplicationException();
		}
		public static bool IsValidLanguageVersion(int version)
		{
			return Context.ToValidLanguageVersion(version) != Context.Versions.Unknown;
		}
		public static Context.Versions ToValidLanguageVersion(int version)
		{
			Context.Versions result = Context.Versions.Unknown;
			if (version > 0 || version < 160)
			{
				result = (Context.Versions)version;
			}
			return result;
		}
		public static void CheckLanguageVersion(int version)
		{
			if (Context.IsValidLanguageVersion(version))
			{
				return;
			}
			throw new ArgumentException("Bad language version: " + version);
		}
		public static void ReportWarning(string message, string sourceName, int lineno, string lineSource, int lineOffset)
		{
			Context currentContext = Context.CurrentContext;
			currentContext.ErrorReporter.Warning(message, sourceName, lineno, lineSource, lineOffset);
		}
		public static void ReportWarningById(string messageId, params string[] arguments)
		{
			int[] array = new int[1];
			int[] array2 = array;
			string sourcePositionFromStack = Context.GetSourcePositionFromStack(array2);
			Context.ReportWarning(ScriptRuntime.GetMessage(messageId, arguments), sourcePositionFromStack, array2[0], null, 0);
		}
		public static void ReportWarning(string message)
		{
			int[] array = new int[1];
			int[] array2 = array;
			string sourcePositionFromStack = Context.GetSourcePositionFromStack(array2);
			Context.ReportWarning(message, sourcePositionFromStack, array2[0], null, 0);
		}
		public static void ReportError(string message, string sourceName, int lineno, string lineSource, int lineOffset)
		{
			Context currentContext = Context.CurrentContext;
			if (currentContext != null)
			{
				currentContext.ErrorReporter.Error(message, sourceName, lineno, lineSource, lineOffset);
				return;
			}
			throw new EcmaScriptRuntimeException(message, sourceName, lineno, lineSource, lineOffset);
		}
		public static void ReportError(string message)
		{
			int[] array = new int[1];
			int[] array2 = array;
			string sourcePositionFromStack = Context.GetSourcePositionFromStack(array2);
			Context.ReportError(message, sourcePositionFromStack, array2[0], null, 0);
		}
		public static EcmaScriptRuntimeException ReportRuntimeError(string message, string sourceName, int lineno, string lineSource, int lineOffset)
		{
			Context currentContext = Context.CurrentContext;
			if (currentContext != null)
			{
				return currentContext.ErrorReporter.RuntimeError(message, sourceName, lineno, lineSource, lineOffset);
			}
			throw new EcmaScriptRuntimeException(message, sourceName, lineno, lineSource, lineOffset);
		}
		internal static EcmaScriptRuntimeException ReportRuntimeErrorById(string messageId, params object[] args)
		{
			return Context.ReportRuntimeError(ScriptRuntime.GetMessage(messageId, args));
		}
		public static EcmaScriptRuntimeException ReportRuntimeError(string message)
		{
			int[] array = new int[1];
			int[] array2 = array;
			string sourcePositionFromStack = Context.GetSourcePositionFromStack(array2);
			return Context.ReportRuntimeError(message, sourcePositionFromStack, array2[0], null, 0);
		}
		public ScriptableObject InitStandardObjects()
		{
			return this.InitStandardObjects(null, false);
		}
		public IScriptable InitStandardObjects(ScriptableObject scope)
		{
			return this.InitStandardObjects(scope, false);
		}
		public ScriptableObject InitStandardObjects(ScriptableObject scope, bool zealed)
		{
			return ScriptRuntime.InitStandardObjects(this, scope, zealed);
		}
		public object EvaluateString(IScriptable scope, string source, string sourceName, int lineno, object securityDomain)
		{
			IScript script = this.CompileString(source, sourceName, lineno, securityDomain);
			object result;
			if (script != null)
			{
				result = script.Exec(this, scope);
			}
			else
			{
				result = null;
			}
			return result;
		}
		public object EvaluateReader(IScriptable scope, StreamReader sr, string sourceName, int lineno, object securityDomain)
		{
			IScript script = this.CompileReader(sr, sourceName, lineno, securityDomain);
			object result;
			if (script != null)
			{
				result = script.Exec(this, scope);
			}
			else
			{
				result = null;
			}
			return result;
		}
		public ScriptOrFnNode IsCompilableUnit(string source)
		{
			ScriptOrFnNode scriptOrFnNode = null;
			bool flag = false;
			CompilerEnvirons compilerEnvirons = new CompilerEnvirons();
			compilerEnvirons.initFromContext(this);
			compilerEnvirons.setGeneratingSource(false);
			Parser parser = new Parser(compilerEnvirons, DefaultErrorReporter.instance);
			try
			{
				scriptOrFnNode = parser.Parse(source, null, 1);
			}
			catch (EcmaScriptRuntimeException)
			{
				flag = true;
			}
			ScriptOrFnNode result;
			if (!flag || !parser.Eof)
			{
				result = scriptOrFnNode;
			}
			else
			{
				result = null;
			}
			return result;
		}
		public IScript CompileReader(StreamReader sr, string sourceName, int lineno, object securityDomain)
		{
			if (lineno < 0)
			{
				throw new ArgumentException("lineno may not be negative", "lineno");
			}
			return (IScript)this.CompileImpl(null, sr, null, sourceName, lineno, securityDomain, false, null, null);
		}
		public IScript CompileString(string source, string sourceName, int lineno, object securityDomain)
		{
			if (lineno < 0)
			{
				lineno = 0;
			}
			return this.CompileString(source, null, null, sourceName, lineno, securityDomain);
		}
		internal IScript CompileString(string source, Interpreter compiler, ErrorReporter compilationErrorReporter, string sourceName, int lineno, object securityDomain)
		{
			return (IScript)this.CompileImpl(null, null, source, sourceName, lineno, securityDomain, false, compiler, compilationErrorReporter);
		}
		public IFunction CompileFunction(IScriptable scope, string source, string sourceName, int lineno, object securityDomain)
		{
			return this.CompileFunction(scope, source, null, null, sourceName, lineno, securityDomain);
		}
		internal IFunction CompileFunction(IScriptable scope, string source, Interpreter compiler, ErrorReporter compilationErrorReporter, string sourceName, int lineno, object securityDomain)
		{
			return (IFunction)this.CompileImpl(scope, null, source, sourceName, lineno, securityDomain, true, compiler, compilationErrorReporter);
		}
		public string DecompileScript(IScript script, int indent)
		{
			BuiltinFunction builtinFunction = (BuiltinFunction)script;
			return builtinFunction.Decompile(indent, 0);
		}
		public string DecompileFunction(IFunction fun, int indent)
		{
			string result;
			if (fun is BaseFunction)
			{
				result = ((BaseFunction)fun).Decompile(indent, 0);
			}
			else
			{
				result = "function " + fun.ClassName + "() {\n\t[native code]\n}\n";
			}
			return result;
		}
		public string DecompileFunctionBody(IFunction fun, int indent)
		{
			string result;
			if (fun is BaseFunction)
			{
				BaseFunction baseFunction = (BaseFunction)fun;
				result = baseFunction.Decompile(indent, 1);
			}
			else
			{
				result = "[native code]\n";
			}
			return result;
		}
		public IScriptable NewObject(IScriptable scope)
		{
			return this.NewObject(scope, "Object", ScriptRuntime.EmptyArgs);
		}
		public IScriptable NewObject(IScriptable scope, string constructorName)
		{
			return this.NewObject(scope, constructorName, ScriptRuntime.EmptyArgs);
		}
		public IScriptable NewObject(IScriptable scope, string constructorName, object[] args)
		{
			scope = ScriptableObject.GetTopLevelScope(scope);
			IFunction existingCtor = ScriptRuntime.getExistingCtor(this, scope, constructorName);
			if (args == null)
			{
				args = ScriptRuntime.EmptyArgs;
			}
			return existingCtor.Construct(this, scope, args);
		}
		public IScriptable NewArray(IScriptable scope, int length)
		{
			BuiltinArray builtinArray = new BuiltinArray((long)length);
			ScriptRuntime.setObjectProtoAndParent(builtinArray, scope);
			return builtinArray;
		}
		public IScriptable NewArray(IScriptable scope, object[] elements)
		{
			Type elementType = elements.GetType().GetElementType();
			if (elementType != typeof(object))
			{
				throw new ArgumentException();
			}
			BuiltinArray builtinArray = new BuiltinArray(elements);
			ScriptRuntime.setObjectProtoAndParent(builtinArray, scope);
			return builtinArray;
		}
		public object[] GetElements(IScriptable obj)
		{
			return ScriptRuntime.getArrayElements(obj);
		}
		public static object CliToJS(Context cx, object value, IScriptable scope)
		{
			object result;
			if (value is string || CliHelper.IsNumber(value) || value is bool || value is IScriptable)
			{
				result = value;
			}
			else
			{
				if (value is char)
				{
					result = Convert.ToString((char)value);
				}
				else
				{
					Type type = value as Type;
					if (type != null)
					{
						result = cx.Wrap(scope, value, (Type)value);
					}
					else
					{
						result = cx.Wrap(scope, value, null);
					}
				}
			}
			return result;
		}
		public static object JsToCli(object value, Type desiredType)
		{
			return CliObject.CoerceType(desiredType, value);
		}
		public static ApplicationException ThrowAsScriptRuntimeEx(Exception e)
		{
			while (e is TargetInvocationException)
			{
				e = ((TargetInvocationException)e).InnerException;
			}
			if (e is EcmaScriptException)
			{
				throw e;
			}
			throw new EcmaScriptRuntimeException(e);
		}
		public static bool IsValidOptimizationLevel(int optimizationLevel)
		{
			return -1 <= optimizationLevel && optimizationLevel <= 9;
		}
		public static void CheckOptimizationLevel(int optimizationLevel)
		{
			if (Context.IsValidOptimizationLevel(optimizationLevel))
			{
				return;
			}
			throw new ArgumentException("Optimization level outside [-1..9]: " + optimizationLevel);
		}
		public object GetThreadLocal(object key)
		{
			object result;
			if (this.hashtable == null)
			{
				result = null;
			}
			else
			{
				result = this.hashtable[key];
			}
			return result;
		}
		public void PutThreadLocal(object key, object value)
		{
			if (this.m_Sealed)
			{
				Context.OnSealedMutation();
			}
			if (this.hashtable == null)
			{
				this.hashtable = Hashtable.Synchronized(new Hashtable());
			}
			this.hashtable[key] = value;
		}
		public void RemoveThreadLocal(object key)
		{
			if (this.m_Sealed)
			{
				Context.OnSealedMutation();
			}
			if (this.hashtable != null)
			{
				this.hashtable.Remove(key);
			}
		}
		public void SetDebugger(EcmaScript.NET.Debugging.Debugger debugger, object contextData)
		{
			if (this.m_Sealed)
			{
				Context.OnSealedMutation();
			}
			this.m_Debugger = debugger;
			this.debuggerData = contextData;
		}
		public static DebuggableScript getDebuggableView(IScript script)
		{
			DebuggableScript result;
			if (script is BuiltinFunction)
			{
				result = ((BuiltinFunction)script).DebuggableView;
			}
			else
			{
				result = null;
			}
			return result;
		}
		public bool HasFeature(Context.Features feature)
		{
			return (this.m_Features & feature) == feature;
		}
		public void SetFeature(Context.Features feature, bool isEnabled)
		{
			if (isEnabled)
			{
				this.m_Features |= feature;
			}
			else
			{
				if (this.HasFeature(feature))
				{
					this.m_Features ^= feature;
				}
			}
		}
		protected internal void ObserveInstructionCount(int instructionCount)
		{
			this.Factory.ObserveInstructionCount(this, instructionCount);
		}
		private object CompileImpl(IScriptable scope, StreamReader sourceReader, string sourceString, string sourceName, int lineno, object securityDomain, bool returnFunction, Interpreter compiler, ErrorReporter compilationErrorReporter)
		{
			if (securityDomain != null && this.securityController == null)
			{
				throw new ArgumentException("securityDomain should be null if setSecurityController() was never called");
			}
			if (!(sourceReader == null ^ sourceString == null))
			{
				Context.CodeBug();
			}
			if (!(scope == null ^ returnFunction))
			{
				Context.CodeBug();
			}
			CompilerEnvirons compilerEnvirons = new CompilerEnvirons();
			compilerEnvirons.initFromContext(this);
			if (compilationErrorReporter == null)
			{
				compilationErrorReporter = compilerEnvirons.getErrorReporter();
			}
			if (this.m_Debugger != null)
			{
				if (sourceReader != null)
				{
					sourceString = sourceReader.ReadToEnd();
					sourceReader = null;
				}
			}
			Parser parser = new Parser(compilerEnvirons, compilationErrorReporter);
			if (returnFunction)
			{
				parser.calledByCompileFunction = true;
			}
			ScriptOrFnNode scriptOrFnNode;
			if (sourceString != null)
			{
				scriptOrFnNode = parser.Parse(sourceString, sourceName, lineno);
			}
			else
			{
				scriptOrFnNode = parser.Parse(sourceReader, sourceName, lineno);
			}
			if (returnFunction)
			{
				if (scriptOrFnNode.FunctionCount != 1 || scriptOrFnNode.FirstChild == null || scriptOrFnNode.FirstChild.Type != 107)
				{
					throw new ArgumentException("compileFunction only accepts source with single JS function: " + sourceString);
				}
			}
			if (compiler == null)
			{
				compiler = new Interpreter();
			}
			string encodedSource = parser.EncodedSource;
			object obj = compiler.Compile(compilerEnvirons, scriptOrFnNode, encodedSource, returnFunction);
			if (this.m_Debugger != null)
			{
				if (sourceString == null)
				{
					Context.CodeBug();
				}
				if (!(obj is DebuggableScript))
				{
					throw new ApplicationException("NOT SUPPORTED");
				}
				DebuggableScript dscript = (DebuggableScript)obj;
				Context.NotifyDebugger(this, dscript, sourceString);
			}
			object result;
			if (returnFunction)
			{
				result = compiler.CreateFunctionObject(this, scope, obj, securityDomain);
			}
			else
			{
				result = compiler.CreateScriptObject(obj, securityDomain);
			}
			return result;
		}
		private static void NotifyDebugger(Context cx, DebuggableScript dscript, string debugSource)
		{
			cx.m_Debugger.HandleCompilationDone(cx, dscript, debugSource);
			for (int num = 0; num != dscript.FunctionCount; num++)
			{
				Context.NotifyDebugger(cx, dscript.GetFunction(num), debugSource);
			}
		}
		internal static string GetSourcePositionFromStack(int[] linep)
		{
			Context currentContext = Context.CurrentContext;
			string result;
			if (currentContext == null)
			{
				result = null;
			}
			else
			{
				if (currentContext.lastInterpreterFrame != null)
				{
					result = Interpreter.GetSourcePositionFromStack(currentContext, linep);
				}
				else
				{
					StackTrace stackTrace = new StackTrace();
					StackFrame frame = stackTrace.GetFrame(0);
					linep[0] = frame.GetFileLineNumber();
					result = Path.GetFileName(frame.GetFileName());
				}
			}
			return result;
		}
		public void AddActivationName(string name)
		{
			if (this.m_Sealed)
			{
				Context.OnSealedMutation();
			}
			if (this.activationNames == null)
			{
				this.activationNames = Hashtable.Synchronized(new Hashtable(5));
			}
			this.activationNames[name] = name;
		}
		public bool IsActivationNeeded(string name)
		{
			return this.activationNames != null && this.activationNames.ContainsKey(name);
		}
		public void RemoveActivationName(string name)
		{
			if (this.m_Sealed)
			{
				Context.OnSealedMutation();
			}
			if (this.activationNames != null)
			{
				this.activationNames.Remove(name);
			}
		}
		static Context()
		{
			Context.EmptyArgs = ScriptRuntime.EmptyArgs;
		}
		public void Dispose()
		{
			Context.Exit();
		}
		public static ApplicationException CodeBug()
		{
			ApplicationException ex = new ApplicationException("FAILED ASSERTION");
			Console.Error.WriteLine(ex.ToString());
			throw ex;
		}
	}
}
