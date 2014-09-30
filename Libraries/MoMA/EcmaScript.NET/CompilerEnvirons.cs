using System;
using System.Collections;
using System.Runtime.InteropServices;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public class CompilerEnvirons
	{
		private ErrorReporter errorReporter;
		private Context.Versions languageVersion;
		private bool generateDebugInfo;
		private bool useDynamicScope;
		private bool reservedKeywordAsIdentifier;
		private bool allowMemberExprAsFunctionName;
		private bool xmlAvailable;
		private int optimizationLevel;
		private bool generatingSource;
		internal Hashtable activationNames;
		internal bool getterAndSetterSupport;
		public virtual bool UseDynamicScope
		{
			get
			{
				return this.useDynamicScope;
			}
		}
		public Context.Versions LanguageVersion
		{
			get
			{
				return this.languageVersion;
			}
			set
			{
				this.languageVersion = value;
			}
		}
		public CompilerEnvirons()
		{
			this.errorReporter = DefaultErrorReporter.instance;
			this.languageVersion = Context.Versions.Default;
			this.generateDebugInfo = true;
			this.useDynamicScope = false;
			this.reservedKeywordAsIdentifier = false;
			this.allowMemberExprAsFunctionName = false;
			this.xmlAvailable = true;
			this.optimizationLevel = 0;
			this.generatingSource = true;
		}
		public virtual void initFromContext(Context cx)
		{
			this.setErrorReporter(cx.ErrorReporter);
			this.languageVersion = cx.Version;
			this.useDynamicScope = cx.compileFunctionsWithDynamicScopeFlag;
			this.generateDebugInfo = (!cx.GeneratingDebugChanged || cx.GeneratingDebug);
			this.reservedKeywordAsIdentifier = cx.HasFeature(Context.Features.ReservedKeywordAsIdentifier);
			this.allowMemberExprAsFunctionName = cx.HasFeature(Context.Features.MemberExprAsFunctionName);
			this.xmlAvailable = cx.HasFeature(Context.Features.E4x);
			this.getterAndSetterSupport = cx.HasFeature(Context.Features.GetterAndSetter);
			this.optimizationLevel = cx.OptimizationLevel;
			this.generatingSource = cx.GeneratingSource;
			this.activationNames = cx.activationNames;
		}
		public ErrorReporter getErrorReporter()
		{
			return this.errorReporter;
		}
		public virtual void setErrorReporter(ErrorReporter errorReporter)
		{
			if (errorReporter == null)
			{
				throw new ArgumentException();
			}
			this.errorReporter = errorReporter;
		}
		public bool isGenerateDebugInfo()
		{
			return this.generateDebugInfo;
		}
		public virtual void setGenerateDebugInfo(bool flag)
		{
			this.generateDebugInfo = flag;
		}
		public bool isReservedKeywordAsIdentifier()
		{
			return this.reservedKeywordAsIdentifier;
		}
		public virtual void setReservedKeywordAsIdentifier(bool flag)
		{
			this.reservedKeywordAsIdentifier = flag;
		}
		public bool isAllowMemberExprAsFunctionName()
		{
			return this.allowMemberExprAsFunctionName;
		}
		public virtual void setAllowMemberExprAsFunctionName(bool flag)
		{
			this.allowMemberExprAsFunctionName = flag;
		}
		public bool isXmlAvailable()
		{
			return this.xmlAvailable;
		}
		public virtual void setXmlAvailable(bool flag)
		{
			this.xmlAvailable = flag;
		}
		public int getOptimizationLevel()
		{
			return this.optimizationLevel;
		}
		public virtual void setOptimizationLevel(int level)
		{
			Context.CheckOptimizationLevel(level);
			this.optimizationLevel = level;
		}
		public bool isGeneratingSource()
		{
			return this.generatingSource;
		}
		public virtual void setGeneratingSource(bool generatingSource)
		{
			this.generatingSource = generatingSource;
		}
	}
}
