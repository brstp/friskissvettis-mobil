using log4net.Util;
using System;
using System.Collections;
using System.IO;
namespace log4net.ObjectRenderer
{
	public sealed class DefaultRenderer : IObjectRenderer
	{
		public void RenderObject(RendererMap rendererMap, object obj, TextWriter writer)
		{
			if (rendererMap == null)
			{
				throw new ArgumentNullException("rendererMap");
			}
			if (obj == null)
			{
				writer.Write(SystemInfo.NullText);
			}
			else
			{
				Array array = obj as Array;
				if (array != null)
				{
					this.RenderArray(rendererMap, array, writer);
				}
				else
				{
					IEnumerable enumerable = obj as IEnumerable;
					if (enumerable != null)
					{
						ICollection collection = obj as ICollection;
						if (collection != null && collection.Count == 0)
						{
							writer.Write("{}");
						}
						else
						{
							IDictionary dictionary = obj as IDictionary;
							if (dictionary != null)
							{
								this.RenderEnumerator(rendererMap, dictionary.GetEnumerator(), writer);
							}
							else
							{
								this.RenderEnumerator(rendererMap, enumerable.GetEnumerator(), writer);
							}
						}
					}
					else
					{
						IEnumerator enumerator = obj as IEnumerator;
						if (enumerator != null)
						{
							this.RenderEnumerator(rendererMap, enumerator, writer);
						}
						else
						{
							if (obj is DictionaryEntry)
							{
								this.RenderDictionaryEntry(rendererMap, (DictionaryEntry)obj, writer);
							}
							else
							{
								string text = obj.ToString();
								writer.Write((text == null) ? SystemInfo.NullText : text);
							}
						}
					}
				}
			}
		}
		private void RenderArray(RendererMap rendererMap, Array array, TextWriter writer)
		{
			if (array.Rank != 1)
			{
				writer.Write(array.ToString());
			}
			else
			{
				writer.Write(array.GetType().Name + " {");
				int length = array.Length;
				if (length > 0)
				{
					rendererMap.FindAndRender(array.GetValue(0), writer);
					for (int i = 1; i < length; i++)
					{
						writer.Write(", ");
						rendererMap.FindAndRender(array.GetValue(i), writer);
					}
				}
				writer.Write("}");
			}
		}
		private void RenderEnumerator(RendererMap rendererMap, IEnumerator enumerator, TextWriter writer)
		{
			writer.Write("{");
			if (enumerator != null && enumerator.MoveNext())
			{
				rendererMap.FindAndRender(enumerator.Current, writer);
				while (enumerator.MoveNext())
				{
					writer.Write(", ");
					rendererMap.FindAndRender(enumerator.Current, writer);
				}
			}
			writer.Write("}");
		}
		private void RenderDictionaryEntry(RendererMap rendererMap, DictionaryEntry entry, TextWriter writer)
		{
			rendererMap.FindAndRender(entry.Key, writer);
			writer.Write("=");
			rendererMap.FindAndRender(entry.Value, writer);
		}
	}
}
