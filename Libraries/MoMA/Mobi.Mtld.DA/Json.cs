using Mobi.Mtld.DA.Exception;
using System;
using System.Collections;
using System.Globalization;
using System.Text;
namespace Mobi.Mtld.DA
{
	internal class Json
	{
		private class Tokeniser
		{
			private string s;
			private int p;
			public Tokeniser(string s)
			{
				this.s = s;
				this.p = 0;
			}
			public JsonException syntaxError(string message)
			{
				return new JsonException(message, JsonException.ErrorCode.BAD_DATA);
			}
			public bool More()
			{
				return this.p < this.s.Length;
			}
			public char Next()
			{
				if (this.More())
				{
					char result = this.s[this.p];
					this.p++;
					return result;
				}
				throw this.syntaxError("A Json object text must end with '}'");
			}
			public char Next(char c)
			{
				char c2 = this.Next();
				if (c2 != c)
				{
					throw this.syntaxError(string.Concat(new object[]
					{
						"Expected '",
						c,
						"' and instead saw '",
						c2,
						"'"
					}));
				}
				return c2;
			}
			public string Next(int n)
			{
				int num = this.p;
				int num2 = num + n;
				if (num2 >= this.s.Length)
				{
					throw this.syntaxError("Substring bounds error");
				}
				this.p += n;
				return this.s.Substring(num, num2);
			}
			public void Back()
			{
				if (this.p > 0)
				{
					this.p--;
				}
			}
			public string NextString(char quote)
			{
				StringBuilder stringBuilder = new StringBuilder();
				while (true)
				{
					char c = this.Next();
					char c2 = c;
					if (c2 == '\n' || c2 == '\r')
					{
						break;
					}
					if (c2 != '\\')
					{
						if (c == quote)
						{
							goto Block_9;
						}
						stringBuilder.Append(c);
					}
					else
					{
						c = this.Next();
						c2 = c;
						if (c2 <= 'f')
						{
							if (c2 != 'b')
							{
								if (c2 != 'f')
								{
									goto IL_118;
								}
								stringBuilder.Append('\f');
							}
							else
							{
								stringBuilder.Append('\b');
							}
						}
						else
						{
							if (c2 != 'n')
							{
								switch (c2)
								{
								case 'r':
									stringBuilder.Append('\r');
									break;
								case 's':
								case 'v':
								case 'w':
									goto IL_118;
								case 't':
									stringBuilder.Append('\t');
									break;
								case 'u':
									stringBuilder.Append((char)int.Parse(this.Next(4), NumberStyles.HexNumber));
									break;
								case 'x':
									stringBuilder.Append((char)int.Parse(this.Next(2), NumberStyles.HexNumber));
									break;
								default:
									goto IL_118;
								}
							}
							else
							{
								stringBuilder.Append('\n');
							}
						}
						continue;
						IL_118:
						stringBuilder.Append(c);
					}
				}
				throw this.syntaxError("Unterminated string");
				Block_9:
				return stringBuilder.ToString();
			}
			public string NextTo(char d)
			{
				StringBuilder stringBuilder = new StringBuilder();
				string result;
				while (true)
				{
					char c;
					try
					{
						c = this.Next();
					}
					catch (JsonException var_2_1B)
					{
						result = stringBuilder.ToString().Trim();
						return result;
					}
					if (c == d)
					{
						break;
					}
					stringBuilder.Append(c);
				}
				this.Back();
				result = stringBuilder.ToString().Trim();
				return result;
			}
			public string NextTo(string delimiters)
			{
				StringBuilder stringBuilder = new StringBuilder();
				string result;
				while (true)
				{
					char value;
					try
					{
						value = this.Next();
					}
					catch (JsonException var_2_1B)
					{
						result = stringBuilder.ToString().Trim();
						return result;
					}
					if (delimiters.IndexOf(value) >= 0)
					{
						break;
					}
					stringBuilder.Append(value);
				}
				this.Back();
				result = stringBuilder.ToString().Trim();
				return result;
			}
			public object NextValue()
			{
				char c = this.Next();
				char c2 = c;
				object result;
				if (c2 <= '(')
				{
					if (c2 != '"')
					{
						switch (c2)
						{
						case '\'':
							break;
						case '(':
							goto IL_6A;
						default:
							goto IL_82;
						}
					}
					result = this.NextString(c);
					return result;
				}
				if (c2 != '[')
				{
					if (c2 != '{')
					{
						goto IL_82;
					}
					this.Back();
					result = new Json(this).GetHashtable();
					return result;
				}
				IL_6A:
				this.Back();
				result = new Json(this).GetArray();
				return result;
				IL_82:
				StringBuilder stringBuilder = new StringBuilder();
				char c3 = c;
				while (c >= ' ' && ",:]}/\\\"[{;=#".IndexOf(c) < 0)
				{
					stringBuilder.Append(c);
					c = this.Next();
				}
				this.Back();
				string text = stringBuilder.ToString().Trim();
				if (text.Equals(""))
				{
					throw this.syntaxError("Missing value");
				}
				if (text.Equals("true", StringComparison.OrdinalIgnoreCase))
				{
					result = true;
				}
				else
				{
					if (text.Equals("false", StringComparison.OrdinalIgnoreCase))
					{
						result = false;
					}
					else
					{
						if (text.Equals("null", StringComparison.OrdinalIgnoreCase))
						{
							result = null;
						}
						else
						{
							if ((c3 >= '0' && c3 <= '9') || c3 == '.' || c3 == '-' || c3 == '+')
							{
								if (c3 == '0')
								{
									if (text.Length > 2 && (text[1] == 'x' || text[1] == 'X'))
									{
										try
										{
											result = int.Parse(text.Substring(2), NumberStyles.HexNumber);
											return result;
										}
										catch (Exception var_7_218)
										{
										}
									}
									else
									{
										try
										{
											result = int.Parse(text, NumberStyles.HexNumber);
											return result;
										}
										catch (Exception var_7_218)
										{
										}
									}
								}
								try
								{
									result = int.Parse(text);
									return result;
								}
								catch (Exception var_7_218)
								{
									try
									{
										result = long.Parse(text, NumberFormatInfo.InvariantInfo);
										return result;
									}
									catch (Exception var_8_27B)
									{
										try
										{
											result = double.Parse(text, NumberFormatInfo.InvariantInfo);
											return result;
										}
										catch (Exception var_9_296)
										{
											result = text;
											return result;
										}
									}
								}
							}
							result = text;
						}
					}
				}
				return result;
			}
		}
		private Json.Tokeniser json;
		public Json(string str)
		{
			this.json = new Json.Tokeniser(str);
		}
		private Json(Json.Tokeniser jsonTokeniser)
		{
			this.json = jsonTokeniser;
		}
		public Hashtable GetHashtable()
		{
			Hashtable hashtable = new Hashtable();
			if (this.json.Next() != '{')
			{
				throw this.json.syntaxError("A Json object text must begin with '{'");
			}
			char c2;
			while (true)
			{
				char c = this.json.Next();
				c2 = c;
				if (c2 == '}')
				{
					break;
				}
				this.json.Back();
				string key = this.json.NextValue().ToString();
				c = this.json.Next();
				if (c != ':')
				{
					goto Block_3;
				}
				hashtable.Add(key, this.json.NextValue());
				c2 = this.json.Next();
				if (c2 != ',')
				{
					goto Block_4;
				}
				if (this.json.Next() == '}')
				{
					goto Block_6;
				}
				this.json.Back();
			}
			Hashtable result = hashtable;
			return result;
			Block_3:
			throw this.json.syntaxError("Expected a ':' after a key");
			Block_4:
			if (c2 != '}')
			{
				throw this.json.syntaxError("Expected a ',' or '}'");
			}
			result = hashtable;
			return result;
			Block_6:
			result = hashtable;
			return result;
		}
		private Hashtable GetArray()
		{
			Hashtable hashtable = new Hashtable();
			int num = 0;
			char c = this.json.Next();
			char c2;
			if (c == '[')
			{
				c2 = ']';
			}
			else
			{
				if (c != '(')
				{
					throw this.json.syntaxError("A Json Array text must start with '['");
				}
				c2 = ')';
			}
			Hashtable result;
			if (this.json.Next() == ']')
			{
				result = hashtable;
			}
			else
			{
				this.json.Back();
				char c3;
				while (true)
				{
					if (this.json.Next() == ',')
					{
						this.json.Back();
						hashtable.Add(num++.ToString(), null);
					}
					else
					{
						this.json.Back();
						hashtable.Add(num++.ToString(), this.json.NextValue());
					}
					c = this.json.Next();
					c3 = c;
					if (c3 == ')')
					{
						goto IL_161;
					}
					if (c3 != ',')
					{
						break;
					}
					if (this.json.Next() == ']')
					{
						goto Block_8;
					}
					this.json.Back();
				}
				if (c3 != ']')
				{
					throw this.json.syntaxError("Expected a ',' or ']'");
				}
				goto IL_161;
				Block_8:
				result = hashtable;
				return result;
				IL_161:
				if (c2 != c)
				{
					throw this.json.syntaxError("Expected a '" + c2 + "'");
				}
				result = hashtable;
			}
			return result;
		}
	}
}
