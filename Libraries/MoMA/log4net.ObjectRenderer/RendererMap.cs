using log4net.Util;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
namespace log4net.ObjectRenderer
{
	public class RendererMap
	{
		private Hashtable m_map;
		private Hashtable m_cache = new Hashtable();
		private static IObjectRenderer s_defaultRenderer = new DefaultRenderer();
		public IObjectRenderer DefaultRenderer
		{
			get
			{
				return RendererMap.s_defaultRenderer;
			}
		}
		public RendererMap()
		{
			this.m_map = Hashtable.Synchronized(new Hashtable());
		}
		public string FindAndRender(object obj)
		{
			string text = obj as string;
			string result;
			if (text != null)
			{
				result = text;
			}
			else
			{
				StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
				this.FindAndRender(obj, stringWriter);
				result = stringWriter.ToString();
			}
			return result;
		}
		public void FindAndRender(object obj, TextWriter writer)
		{
			if (obj == null)
			{
				writer.Write(SystemInfo.NullText);
			}
			else
			{
				string text = obj as string;
				if (text != null)
				{
					writer.Write(text);
				}
				else
				{
					try
					{
						this.Get(obj.GetType()).RenderObject(this, obj, writer);
					}
					catch (Exception ex)
					{
						LogLog.Error("RendererMap: Exception while rendering object of type [" + obj.GetType().FullName + "]", ex);
						string str = "";
						if (obj != null && obj.GetType() != null)
						{
							str = obj.GetType().FullName;
						}
						writer.Write("<log4net.Error>Exception rendering object type [" + str + "]");
						if (ex != null)
						{
							string str2 = null;
							try
							{
								str2 = ex.ToString();
							}
							catch
							{
							}
							writer.Write("<stackTrace>" + str2 + "</stackTrace>");
						}
						writer.Write("</log4net.Error>");
					}
				}
			}
		}
		public IObjectRenderer Get(object obj)
		{
			IObjectRenderer result;
			if (obj == null)
			{
				result = null;
			}
			else
			{
				result = this.Get(obj.GetType());
			}
			return result;
		}
		public IObjectRenderer Get(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			IObjectRenderer objectRenderer = (IObjectRenderer)this.m_cache[type];
			if (objectRenderer == null)
			{
				for (Type type2 = type; type2 != null; type2 = type2.BaseType)
				{
					objectRenderer = this.SearchTypeAndInterfaces(type2);
					if (objectRenderer != null)
					{
						break;
					}
				}
				if (objectRenderer == null)
				{
					objectRenderer = RendererMap.s_defaultRenderer;
				}
				this.m_cache[type] = objectRenderer;
			}
			return objectRenderer;
		}
		private IObjectRenderer SearchTypeAndInterfaces(Type type)
		{
			IObjectRenderer objectRenderer = (IObjectRenderer)this.m_map[type];
			IObjectRenderer result;
			if (objectRenderer != null)
			{
				result = objectRenderer;
			}
			else
			{
				Type[] interfaces = type.GetInterfaces();
				for (int i = 0; i < interfaces.Length; i++)
				{
					Type type2 = interfaces[i];
					objectRenderer = this.SearchTypeAndInterfaces(type2);
					if (objectRenderer != null)
					{
						result = objectRenderer;
						return result;
					}
				}
				result = null;
			}
			return result;
		}
		public void Clear()
		{
			this.m_map.Clear();
			this.m_cache.Clear();
		}
		public void Put(Type typeToRender, IObjectRenderer renderer)
		{
			this.m_cache.Clear();
			if (typeToRender == null)
			{
				throw new ArgumentNullException("typeToRender");
			}
			if (renderer == null)
			{
				throw new ArgumentNullException("renderer");
			}
			this.m_map[typeToRender] = renderer;
		}
	}
}
