using log4net.Appender;
using log4net.Core;
using log4net.ObjectRenderer;
using log4net.Util;
using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Security;
using System.Threading;
using System.Xml;
namespace log4net.Repository.Hierarchy
{
	public class XmlHierarchyConfigurator
	{
		private enum ConfigUpdateMode
		{
			Merge,
			Overwrite
		}
		private const string CONFIGURATION_TAG = "log4net";
		private const string RENDERER_TAG = "renderer";
		private const string APPENDER_TAG = "appender";
		private const string APPENDER_REF_TAG = "appender-ref";
		private const string PARAM_TAG = "param";
		private const string CATEGORY_TAG = "category";
		private const string PRIORITY_TAG = "priority";
		private const string LOGGER_TAG = "logger";
		private const string NAME_ATTR = "name";
		private const string TYPE_ATTR = "type";
		private const string VALUE_ATTR = "value";
		private const string ROOT_TAG = "root";
		private const string LEVEL_TAG = "level";
		private const string REF_ATTR = "ref";
		private const string ADDITIVITY_ATTR = "additivity";
		private const string THRESHOLD_ATTR = "threshold";
		private const string CONFIG_DEBUG_ATTR = "configDebug";
		private const string INTERNAL_DEBUG_ATTR = "debug";
		private const string CONFIG_UPDATE_MODE_ATTR = "update";
		private const string RENDERING_TYPE_ATTR = "renderingClass";
		private const string RENDERED_TYPE_ATTR = "renderedClass";
		private const string INHERITED = "inherited";
		private Hashtable m_appenderBag;
		private readonly Hierarchy m_hierarchy;
		public XmlHierarchyConfigurator(Hierarchy hierarchy)
		{
			this.m_hierarchy = hierarchy;
			this.m_appenderBag = new Hashtable();
		}
		public void Configure(XmlElement element)
		{
			if (element != null && this.m_hierarchy != null)
			{
				string localName = element.LocalName;
				if (localName != "log4net")
				{
					LogLog.Error("XmlHierarchyConfigurator: Xml element is - not a <log4net> element.");
				}
				else
				{
					if (!LogLog.InternalDebugging)
					{
						string attribute = element.GetAttribute("debug");
						LogLog.Debug("XmlHierarchyConfigurator: debug attribute [" + attribute + "].");
						if (attribute.Length > 0 && attribute != "null")
						{
							LogLog.InternalDebugging = OptionConverter.ToBoolean(attribute, true);
						}
						else
						{
							LogLog.Debug("XmlHierarchyConfigurator: Ignoring debug attribute.");
						}
						string attribute2 = element.GetAttribute("configDebug");
						if (attribute2.Length > 0 && attribute2 != "null")
						{
							LogLog.Warn("XmlHierarchyConfigurator: The \"configDebug\" attribute is deprecated.");
							LogLog.Warn("XmlHierarchyConfigurator: Use the \"debug\" attribute instead.");
							LogLog.InternalDebugging = OptionConverter.ToBoolean(attribute2, true);
						}
					}
					XmlHierarchyConfigurator.ConfigUpdateMode configUpdateMode = XmlHierarchyConfigurator.ConfigUpdateMode.Merge;
					string attribute3 = element.GetAttribute("update");
					if (attribute3 != null && attribute3.Length > 0)
					{
						try
						{
							configUpdateMode = (XmlHierarchyConfigurator.ConfigUpdateMode)OptionConverter.ConvertStringTo(typeof(XmlHierarchyConfigurator.ConfigUpdateMode), attribute3);
						}
						catch
						{
							LogLog.Error("XmlHierarchyConfigurator: Invalid update attribute value [" + attribute3 + "]");
						}
					}
					LogLog.Debug("XmlHierarchyConfigurator: Configuration update mode [" + configUpdateMode.ToString() + "].");
					if (configUpdateMode == XmlHierarchyConfigurator.ConfigUpdateMode.Overwrite)
					{
						this.m_hierarchy.ResetConfiguration();
						LogLog.Debug("XmlHierarchyConfigurator: Configuration reset before reading config.");
					}
					foreach (XmlNode xmlNode in element.ChildNodes)
					{
						if (xmlNode.NodeType == XmlNodeType.Element)
						{
							XmlElement xmlElement = (XmlElement)xmlNode;
							if (xmlElement.LocalName == "logger")
							{
								this.ParseLogger(xmlElement);
							}
							else
							{
								if (xmlElement.LocalName == "category")
								{
									this.ParseLogger(xmlElement);
								}
								else
								{
									if (xmlElement.LocalName == "root")
									{
										this.ParseRoot(xmlElement);
									}
									else
									{
										if (xmlElement.LocalName == "renderer")
										{
											this.ParseRenderer(xmlElement);
										}
										else
										{
											if (!(xmlElement.LocalName == "appender"))
											{
												this.SetParameter(xmlElement, this.m_hierarchy);
											}
										}
									}
								}
							}
						}
					}
					string attribute4 = element.GetAttribute("threshold");
					LogLog.Debug("XmlHierarchyConfigurator: Hierarchy Threshold [" + attribute4 + "]");
					if (attribute4.Length > 0 && attribute4 != "null")
					{
						Level level = (Level)this.ConvertStringTo(typeof(Level), attribute4);
						if (level != null)
						{
							this.m_hierarchy.Threshold = level;
						}
						else
						{
							LogLog.Warn("XmlHierarchyConfigurator: Unable to set hierarchy threshold using value [" + attribute4 + "] (with acceptable conversion types)");
						}
					}
				}
			}
		}
		protected IAppender FindAppenderByReference(XmlElement appenderRef)
		{
			string attribute = appenderRef.GetAttribute("ref");
			IAppender appender = (IAppender)this.m_appenderBag[attribute];
			IAppender result;
			if (appender != null)
			{
				result = appender;
			}
			else
			{
				XmlElement xmlElement = null;
				if (attribute != null && attribute.Length > 0)
				{
					foreach (XmlElement xmlElement2 in appenderRef.OwnerDocument.GetElementsByTagName("appender"))
					{
						if (xmlElement2.GetAttribute("name") == attribute)
						{
							xmlElement = xmlElement2;
							break;
						}
					}
				}
				if (xmlElement == null)
				{
					LogLog.Error("XmlHierarchyConfigurator: No appender named [" + attribute + "] could be found.");
					result = null;
				}
				else
				{
					appender = this.ParseAppender(xmlElement);
					if (appender != null)
					{
						this.m_appenderBag[attribute] = appender;
					}
					result = appender;
				}
			}
			return result;
		}
		protected IAppender ParseAppender(XmlElement appenderElement)
		{
			string attribute = appenderElement.GetAttribute("name");
			string attribute2 = appenderElement.GetAttribute("type");
			LogLog.Debug(string.Concat(new string[]
			{
				"XmlHierarchyConfigurator: Loading Appender [",
				attribute,
				"] type: [",
				attribute2,
				"]"
			}));
			IAppender result;
			try
			{
				IAppender appender = (IAppender)Activator.CreateInstance(SystemInfo.GetTypeFromString(attribute2, true, true));
				appender.Name = attribute;
				foreach (XmlNode xmlNode in appenderElement.ChildNodes)
				{
					if (xmlNode.NodeType == XmlNodeType.Element)
					{
						XmlElement xmlElement = (XmlElement)xmlNode;
						if (xmlElement.LocalName == "appender-ref")
						{
							string attribute3 = xmlElement.GetAttribute("ref");
							IAppenderAttachable appenderAttachable = appender as IAppenderAttachable;
							if (appenderAttachable != null)
							{
								LogLog.Debug(string.Concat(new string[]
								{
									"XmlHierarchyConfigurator: Attaching appender named [",
									attribute3,
									"] to appender named [",
									appender.Name,
									"]."
								}));
								IAppender appender2 = this.FindAppenderByReference(xmlElement);
								if (appender2 != null)
								{
									appenderAttachable.AddAppender(appender2);
								}
							}
							else
							{
								LogLog.Error(string.Concat(new string[]
								{
									"XmlHierarchyConfigurator: Requesting attachment of appender named [",
									attribute3,
									"] to appender named [",
									appender.Name,
									"] which does not implement log4net.Core.IAppenderAttachable."
								}));
							}
						}
						else
						{
							this.SetParameter(xmlElement, appender);
						}
					}
				}
				IOptionHandler optionHandler = appender as IOptionHandler;
				if (optionHandler != null)
				{
					optionHandler.ActivateOptions();
				}
				LogLog.Debug("XmlHierarchyConfigurator: Created Appender [" + attribute + "]");
				result = appender;
			}
			catch (Exception exception)
			{
				LogLog.Error(string.Concat(new string[]
				{
					"XmlHierarchyConfigurator: Could not create Appender [",
					attribute,
					"] of type [",
					attribute2,
					"]. Reported error follows."
				}), exception);
				result = null;
			}
			return result;
		}
		protected void ParseLogger(XmlElement loggerElement)
		{
			string attribute = loggerElement.GetAttribute("name");
			LogLog.Debug("XmlHierarchyConfigurator: Retrieving an instance of log4net.Repository.Logger for logger [" + attribute + "].");
			Logger logger = this.m_hierarchy.GetLogger(attribute) as Logger;
			Logger obj;
			Monitor.Enter(obj = logger);
			try
			{
				bool flag = OptionConverter.ToBoolean(loggerElement.GetAttribute("additivity"), true);
				LogLog.Debug(string.Concat(new object[]
				{
					"XmlHierarchyConfigurator: Setting [",
					logger.Name,
					"] additivity to [",
					flag,
					"]."
				}));
				logger.Additivity = flag;
				this.ParseChildrenOfLoggerElement(loggerElement, logger, false);
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		protected void ParseRoot(XmlElement rootElement)
		{
			Logger root = this.m_hierarchy.Root;
			Logger obj;
			Monitor.Enter(obj = root);
			try
			{
				this.ParseChildrenOfLoggerElement(rootElement, root, true);
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		protected void ParseChildrenOfLoggerElement(XmlElement catElement, Logger log, bool isRoot)
		{
			log.RemoveAllAppenders();
			foreach (XmlNode xmlNode in catElement.ChildNodes)
			{
				if (xmlNode.NodeType == XmlNodeType.Element)
				{
					XmlElement xmlElement = (XmlElement)xmlNode;
					if (xmlElement.LocalName == "appender-ref")
					{
						IAppender appender = this.FindAppenderByReference(xmlElement);
						string attribute = xmlElement.GetAttribute("ref");
						if (appender != null)
						{
							LogLog.Debug(string.Concat(new string[]
							{
								"XmlHierarchyConfigurator: Adding appender named [",
								attribute,
								"] to logger [",
								log.Name,
								"]."
							}));
							log.AddAppender(appender);
						}
						else
						{
							LogLog.Error("XmlHierarchyConfigurator: Appender named [" + attribute + "] not found.");
						}
					}
					else
					{
						if (xmlElement.LocalName == "level" || xmlElement.LocalName == "priority")
						{
							this.ParseLevel(xmlElement, log, isRoot);
						}
						else
						{
							this.SetParameter(xmlElement, log);
						}
					}
				}
			}
			IOptionHandler optionHandler = log as IOptionHandler;
			if (optionHandler != null)
			{
				optionHandler.ActivateOptions();
			}
		}
		protected void ParseRenderer(XmlElement element)
		{
			string attribute = element.GetAttribute("renderingClass");
			string attribute2 = element.GetAttribute("renderedClass");
			LogLog.Debug(string.Concat(new string[]
			{
				"XmlHierarchyConfigurator: Rendering class [",
				attribute,
				"], Rendered class [",
				attribute2,
				"]."
			}));
			IObjectRenderer objectRenderer = (IObjectRenderer)OptionConverter.InstantiateByClassName(attribute, typeof(IObjectRenderer), null);
			if (objectRenderer == null)
			{
				LogLog.Error("XmlHierarchyConfigurator: Could not instantiate renderer [" + attribute + "].");
			}
			else
			{
				try
				{
					this.m_hierarchy.RendererMap.Put(SystemInfo.GetTypeFromString(attribute2, true, true), objectRenderer);
				}
				catch (Exception exception)
				{
					LogLog.Error("XmlHierarchyConfigurator: Could not find class [" + attribute2 + "].", exception);
				}
			}
		}
		protected void ParseLevel(XmlElement element, Logger log, bool isRoot)
		{
			string text = log.Name;
			if (isRoot)
			{
				text = "root";
			}
			string attribute = element.GetAttribute("value");
			LogLog.Debug(string.Concat(new string[]
			{
				"XmlHierarchyConfigurator: Logger [",
				text,
				"] Level string is [",
				attribute,
				"]."
			}));
			if ("inherited" == attribute)
			{
				if (isRoot)
				{
					LogLog.Error("XmlHierarchyConfigurator: Root level cannot be inherited. Ignoring directive.");
				}
				else
				{
					LogLog.Debug("XmlHierarchyConfigurator: Logger [" + text + "] level set to inherit from parent.");
					log.Level = null;
				}
			}
			else
			{
				log.Level = log.Hierarchy.LevelMap[attribute];
				if (log.Level == null)
				{
					LogLog.Error(string.Concat(new string[]
					{
						"XmlHierarchyConfigurator: Undefined level [",
						attribute,
						"] on Logger [",
						text,
						"]."
					}));
				}
				else
				{
					LogLog.Debug(string.Concat(new object[]
					{
						"XmlHierarchyConfigurator: Logger [",
						text,
						"] level set to [name=\"",
						log.Level.Name,
						"\",value=",
						log.Level.Value,
						"]."
					}));
				}
			}
		}
		protected void SetParameter(XmlElement element, object target)
		{
			string text = element.GetAttribute("name");
			if (element.LocalName != "param" || text == null || text.Length == 0)
			{
				text = element.LocalName;
			}
			Type type = target.GetType();
			Type type2 = null;
			PropertyInfo propertyInfo = null;
			MethodInfo methodInfo = null;
			propertyInfo = type.GetProperty(text, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (propertyInfo != null && propertyInfo.CanWrite)
			{
				type2 = propertyInfo.PropertyType;
			}
			else
			{
				propertyInfo = null;
				methodInfo = this.FindMethodInfo(type, text);
				if (methodInfo != null)
				{
					type2 = methodInfo.GetParameters()[0].ParameterType;
				}
			}
			if (type2 == null)
			{
				LogLog.Error(string.Concat(new string[]
				{
					"XmlHierarchyConfigurator: Cannot find Property [",
					text,
					"] to set object on [",
					target.ToString(),
					"]"
				}));
			}
			else
			{
				string text2 = null;
				if (element.GetAttributeNode("value") != null)
				{
					text2 = element.GetAttribute("value");
				}
				else
				{
					if (element.HasChildNodes)
					{
						foreach (XmlNode xmlNode in element.ChildNodes)
						{
							if (xmlNode.NodeType == XmlNodeType.CDATA || xmlNode.NodeType == XmlNodeType.Text)
							{
								if (text2 == null)
								{
									text2 = xmlNode.InnerText;
								}
								else
								{
									text2 += xmlNode.InnerText;
								}
							}
						}
					}
				}
				if (text2 != null)
				{
					try
					{
						text2 = OptionConverter.SubstituteVariables(text2, Environment.GetEnvironmentVariables());
					}
					catch (SecurityException)
					{
						LogLog.Debug("XmlHierarchyConfigurator: Security exception while trying to expand environment variables. Error Ignored. No Expansion.");
					}
					Type type3 = null;
					string attribute = element.GetAttribute("type");
					if (attribute != null && attribute.Length > 0)
					{
						try
						{
							Type typeFromString = SystemInfo.GetTypeFromString(attribute, true, true);
							LogLog.Debug(string.Concat(new string[]
							{
								"XmlHierarchyConfigurator: Parameter [",
								text,
								"] specified subtype [",
								typeFromString.FullName,
								"]"
							}));
							if (!type2.IsAssignableFrom(typeFromString))
							{
								if (OptionConverter.CanConvertTypeTo(typeFromString, type2))
								{
									type3 = type2;
									type2 = typeFromString;
								}
								else
								{
									LogLog.Error(string.Concat(new string[]
									{
										"XmlHierarchyConfigurator: Subtype [",
										typeFromString.FullName,
										"] set on [",
										text,
										"] is not a subclass of property type [",
										type2.FullName,
										"] and there are no acceptable type conversions."
									}));
								}
							}
							else
							{
								type2 = typeFromString;
							}
						}
						catch (Exception exception)
						{
							LogLog.Error(string.Concat(new string[]
							{
								"XmlHierarchyConfigurator: Failed to find type [",
								attribute,
								"] set on [",
								text,
								"]"
							}), exception);
						}
					}
					object obj = this.ConvertStringTo(type2, text2);
					if (obj != null && type3 != null)
					{
						LogLog.Debug(string.Concat(new string[]
						{
							"XmlHierarchyConfigurator: Performing additional conversion of value from [",
							obj.GetType().Name,
							"] to [",
							type3.Name,
							"]"
						}));
						obj = OptionConverter.ConvertTypeTo(obj, type3);
					}
					if (obj != null)
					{
						if (propertyInfo != null)
						{
							LogLog.Debug(string.Concat(new string[]
							{
								"XmlHierarchyConfigurator: Setting Property [",
								propertyInfo.Name,
								"] to ",
								obj.GetType().Name,
								" value [",
								obj.ToString(),
								"]"
							}));
							try
							{
								propertyInfo.SetValue(target, obj, BindingFlags.SetProperty, null, null, CultureInfo.InvariantCulture);
							}
							catch (TargetInvocationException ex)
							{
								LogLog.Error(string.Concat(new object[]
								{
									"XmlHierarchyConfigurator: Failed to set parameter [",
									propertyInfo.Name,
									"] on object [",
									target,
									"] using value [",
									obj,
									"]"
								}), ex.InnerException);
							}
						}
						else
						{
							if (methodInfo != null)
							{
								LogLog.Debug(string.Concat(new string[]
								{
									"XmlHierarchyConfigurator: Setting Collection Property [",
									methodInfo.Name,
									"] to ",
									obj.GetType().Name,
									" value [",
									obj.ToString(),
									"]"
								}));
								try
								{
									methodInfo.Invoke(target, BindingFlags.InvokeMethod, null, new object[]
									{
										obj
									}, CultureInfo.InvariantCulture);
								}
								catch (TargetInvocationException ex)
								{
									LogLog.Error(string.Concat(new object[]
									{
										"XmlHierarchyConfigurator: Failed to set parameter [",
										text,
										"] on object [",
										target,
										"] using value [",
										obj,
										"]"
									}), ex.InnerException);
								}
							}
						}
					}
					else
					{
						LogLog.Warn(string.Concat(new object[]
						{
							"XmlHierarchyConfigurator: Unable to set property [",
							text,
							"] on object [",
							target,
							"] using value [",
							text2,
							"] (with acceptable conversion types)"
						}));
					}
				}
				else
				{
					object obj2 = null;
					if (type2 == typeof(string) && !this.HasAttributesOrElements(element))
					{
						obj2 = "";
					}
					else
					{
						Type defaultTargetType = null;
						if (XmlHierarchyConfigurator.IsTypeConstructible(type2))
						{
							defaultTargetType = type2;
						}
						obj2 = this.CreateObjectFromXml(element, defaultTargetType, type2);
					}
					if (obj2 == null)
					{
						LogLog.Error("XmlHierarchyConfigurator: Failed to create object to set param: " + text);
					}
					else
					{
						if (propertyInfo != null)
						{
							LogLog.Debug(string.Concat(new object[]
							{
								"XmlHierarchyConfigurator: Setting Property [",
								propertyInfo.Name,
								"] to object [",
								obj2,
								"]"
							}));
							try
							{
								propertyInfo.SetValue(target, obj2, BindingFlags.SetProperty, null, null, CultureInfo.InvariantCulture);
							}
							catch (TargetInvocationException ex)
							{
								LogLog.Error(string.Concat(new object[]
								{
									"XmlHierarchyConfigurator: Failed to set parameter [",
									propertyInfo.Name,
									"] on object [",
									target,
									"] using value [",
									obj2,
									"]"
								}), ex.InnerException);
							}
						}
						else
						{
							if (methodInfo != null)
							{
								LogLog.Debug(string.Concat(new object[]
								{
									"XmlHierarchyConfigurator: Setting Collection Property [",
									methodInfo.Name,
									"] to object [",
									obj2,
									"]"
								}));
								try
								{
									methodInfo.Invoke(target, BindingFlags.InvokeMethod, null, new object[]
									{
										obj2
									}, CultureInfo.InvariantCulture);
								}
								catch (TargetInvocationException ex)
								{
									LogLog.Error(string.Concat(new object[]
									{
										"XmlHierarchyConfigurator: Failed to set parameter [",
										methodInfo.Name,
										"] on object [",
										target,
										"] using value [",
										obj2,
										"]"
									}), ex.InnerException);
								}
							}
						}
					}
				}
			}
		}
		private bool HasAttributesOrElements(XmlElement element)
		{
			bool result;
			foreach (XmlNode xmlNode in element.ChildNodes)
			{
				if (xmlNode.NodeType == XmlNodeType.Attribute || xmlNode.NodeType == XmlNodeType.Element)
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
		private static bool IsTypeConstructible(Type type)
		{
			bool result;
			if (type.IsClass && !type.IsAbstract)
			{
				ConstructorInfo constructor = type.GetConstructor(new Type[0]);
				if (constructor != null && !constructor.IsAbstract && !constructor.IsPrivate)
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
		private MethodInfo FindMethodInfo(Type targetType, string name)
		{
			string strB = "Add" + name;
			MethodInfo[] methods = targetType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			MethodInfo[] array = methods;
			MethodInfo result;
			for (int i = 0; i < array.Length; i++)
			{
				MethodInfo methodInfo = array[i];
				if (!methodInfo.IsStatic)
				{
					if (string.Compare(methodInfo.Name, name, true, CultureInfo.InvariantCulture) == 0 || string.Compare(methodInfo.Name, strB, true, CultureInfo.InvariantCulture) == 0)
					{
						ParameterInfo[] parameters = methodInfo.GetParameters();
						if (parameters.Length == 1)
						{
							result = methodInfo;
							return result;
						}
					}
				}
			}
			result = null;
			return result;
		}
		protected object ConvertStringTo(Type type, string value)
		{
			object result;
			if (typeof(Level) == type)
			{
				Level level = this.m_hierarchy.LevelMap[value];
				if (level == null)
				{
					LogLog.Error("XmlHierarchyConfigurator: Unknown Level Specified [" + value + "]");
				}
				result = level;
			}
			else
			{
				result = OptionConverter.ConvertStringTo(type, value);
			}
			return result;
		}
		protected object CreateObjectFromXml(XmlElement element, Type defaultTargetType, Type typeConstraint)
		{
			Type type = null;
			string attribute = element.GetAttribute("type");
			object result;
			if (attribute == null || attribute.Length == 0)
			{
				if (defaultTargetType == null)
				{
					LogLog.Error("XmlHierarchyConfigurator: Object type not specified. Cannot create object of type [" + typeConstraint.FullName + "]. Missing Value or Type.");
					result = null;
					return result;
				}
				type = defaultTargetType;
			}
			else
			{
				try
				{
					type = SystemInfo.GetTypeFromString(attribute, true, true);
				}
				catch (Exception exception)
				{
					LogLog.Error("XmlHierarchyConfigurator: Failed to find type [" + attribute + "]", exception);
					result = null;
					return result;
				}
			}
			bool flag = false;
			if (typeConstraint != null)
			{
				if (!typeConstraint.IsAssignableFrom(type))
				{
					if (!OptionConverter.CanConvertTypeTo(type, typeConstraint))
					{
						LogLog.Error(string.Concat(new string[]
						{
							"XmlHierarchyConfigurator: Object type [",
							type.FullName,
							"] is not assignable to type [",
							typeConstraint.FullName,
							"]. There are no acceptable type conversions."
						}));
						result = null;
						return result;
					}
					flag = true;
				}
			}
			object obj = null;
			try
			{
				obj = Activator.CreateInstance(type);
			}
			catch (Exception ex)
			{
				LogLog.Error("XmlHierarchyConfigurator: Failed to construct object of type [" + type.FullName + "] Exception: " + ex.ToString());
			}
			foreach (XmlNode xmlNode in element.ChildNodes)
			{
				if (xmlNode.NodeType == XmlNodeType.Element)
				{
					this.SetParameter((XmlElement)xmlNode, obj);
				}
			}
			IOptionHandler optionHandler = obj as IOptionHandler;
			if (optionHandler != null)
			{
				optionHandler.ActivateOptions();
			}
			if (flag)
			{
				result = OptionConverter.ConvertTypeTo(obj, typeConstraint);
			}
			else
			{
				result = obj;
			}
			return result;
		}
	}
}
