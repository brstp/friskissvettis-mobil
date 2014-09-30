using System;
using System.Text;
namespace log4net.Util
{
	public sealed class SystemStringFormat
	{
		private readonly IFormatProvider m_provider;
		private readonly string m_format;
		private readonly object[] m_args;
		public SystemStringFormat(IFormatProvider provider, string format, params object[] args)
		{
			this.m_provider = provider;
			this.m_format = format;
			this.m_args = args;
		}
		public override string ToString()
		{
			return SystemStringFormat.StringFormat(this.m_provider, this.m_format, this.m_args);
		}
		private static string StringFormat(IFormatProvider provider, string format, params object[] args)
		{
			string result;
			try
			{
				if (format == null)
				{
					result = null;
				}
				else
				{
					if (args == null)
					{
						result = format;
					}
					else
					{
						result = string.Format(provider, format, args);
					}
				}
			}
			catch (Exception ex)
			{
				LogLog.Warn("StringFormat: Exception while rendering format [" + format + "]", ex);
				result = SystemStringFormat.StringFormatError(ex, format, args);
			}
			catch
			{
				LogLog.Warn("StringFormat: Exception while rendering format [" + format + "]");
				result = SystemStringFormat.StringFormatError(null, format, args);
			}
			return result;
		}
		private static string StringFormatError(Exception formatException, string format, object[] args)
		{
			string result;
			try
			{
				StringBuilder stringBuilder = new StringBuilder("<log4net.Error>");
				if (formatException != null)
				{
					stringBuilder.Append("Exception during StringFormat: ").Append(formatException.Message);
				}
				else
				{
					stringBuilder.Append("Exception during StringFormat");
				}
				stringBuilder.Append(" <format>").Append(format).Append("</format>");
				stringBuilder.Append("<args>");
				SystemStringFormat.RenderArray(args, stringBuilder);
				stringBuilder.Append("</args>");
				stringBuilder.Append("</log4net.Error>");
				result = stringBuilder.ToString();
			}
			catch (Exception exception)
			{
				LogLog.Error("StringFormat: INTERNAL ERROR during StringFormat error handling", exception);
				result = "<log4net.Error>Exception during StringFormat. See Internal Log.</log4net.Error>";
			}
			catch
			{
				LogLog.Error("StringFormat: INTERNAL ERROR during StringFormat error handling");
				result = "<log4net.Error>Exception during StringFormat. See Internal Log.</log4net.Error>";
			}
			return result;
		}
		private static void RenderArray(Array array, StringBuilder buffer)
		{
			if (array == null)
			{
				buffer.Append(SystemInfo.NullText);
			}
			else
			{
				if (array.Rank != 1)
				{
					buffer.Append(array.ToString());
				}
				else
				{
					buffer.Append("{");
					int length = array.Length;
					if (length > 0)
					{
						SystemStringFormat.RenderObject(array.GetValue(0), buffer);
						for (int i = 1; i < length; i++)
						{
							buffer.Append(", ");
							SystemStringFormat.RenderObject(array.GetValue(i), buffer);
						}
					}
					buffer.Append("}");
				}
			}
		}
		private static void RenderObject(object obj, StringBuilder buffer)
		{
			if (obj == null)
			{
				buffer.Append(SystemInfo.NullText);
			}
			else
			{
				try
				{
					buffer.Append(obj);
				}
				catch (Exception ex)
				{
					buffer.Append("<Exception: ").Append(ex.Message).Append(">");
				}
				catch
				{
					buffer.Append("<Exception>");
				}
			}
		}
	}
}
