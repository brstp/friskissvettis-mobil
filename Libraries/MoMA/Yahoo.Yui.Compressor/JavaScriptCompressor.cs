using EcmaScript.NET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
namespace Yahoo.Yui.Compressor
{
	public class JavaScriptCompressor
	{
		private const int BUILDING_SYMBOL_TREE = 1;
		private static readonly object _synLock = new object();
		private static readonly Regex SIMPLE_IDENTIFIER_NAME_PATTERN = new Regex("^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled);
		private static HashSet<string> _builtin;
		public static int CHECKING_SYMBOL_TREE = 2;
		private readonly ScriptOrFunctionScope _globalScope = new ScriptOrFunctionScope(-1, null);
		private readonly Hashtable _indexedScopes = new Hashtable();
		private readonly bool _isEvalIgnored;
		private readonly ErrorReporter _logger;
		private readonly Stack _scopes = new Stack();
		private readonly ArrayList _tokens;
		private readonly bool _verbose;
		private int _braceNesting;
		private int _mode;
		private bool _munge;
		private int _offset;
		internal static List<string> Ones;
		internal static List<string> Threes;
		internal static List<string> Twos;
		private static Hashtable Literals
		{
			get;
			set;
		}
		private static HashSet<string> Reserved
		{
			get;
			set;
		}
		public ErrorReporter ErrorReporter
		{
			get;
			private set;
		}
		public JavaScriptCompressor(string javaScript) : this(javaScript, true)
		{
		}
		public JavaScriptCompressor(string javaScript, bool isVerboseLogging) : this(javaScript, isVerboseLogging, Encoding.Default, null)
		{
		}
		public JavaScriptCompressor(string javaScript, bool isVerboseLogging, Encoding encoding, CultureInfo threadCulture) : this(javaScript, isVerboseLogging, encoding, threadCulture, false, null)
		{
		}
		public JavaScriptCompressor(string javaScript, bool isVerboseLogging, Encoding encoding, CultureInfo threadCulture, bool isEvalIgnored, ErrorReporter errorReporter)
		{
			if (string.IsNullOrEmpty(javaScript))
			{
				throw new ArgumentNullException("javaScript");
			}
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
			try
			{
				if (threadCulture != null)
				{
					Thread.CurrentThread.CurrentCulture = threadCulture;
					Thread.CurrentThread.CurrentUICulture = threadCulture;
				}
				JavaScriptCompressor.Initialise();
				this._verbose = isVerboseLogging;
				MemoryStream stream = new MemoryStream(encoding.GetBytes(javaScript));
				this.ErrorReporter = (errorReporter ?? new CustomErrorReporter(isVerboseLogging));
				this._logger = this.ErrorReporter;
				this._tokens = JavaScriptCompressor.Parse(new StreamReader(stream), this.ErrorReporter);
				this._isEvalIgnored = isEvalIgnored;
			}
			finally
			{
				if (threadCulture != null)
				{
					Thread.CurrentThread.CurrentCulture = currentCulture;
					Thread.CurrentThread.CurrentUICulture = currentUICulture;
				}
			}
		}
		private static void InitialiseBuiltIn()
		{
			if (JavaScriptCompressor._builtin == null)
			{
				object synLock;
				Monitor.Enter(synLock = JavaScriptCompressor._synLock);
				try
				{
					if (JavaScriptCompressor._builtin == null)
					{
						HashSet<string> builtin = new HashSet<string>
						{
							"NaN",
							"top"
						};
						JavaScriptCompressor._builtin = builtin;
					}
				}
				finally
				{
					Monitor.Exit(synLock);
				}
			}
		}
		private static void InitialiseOnesList()
		{
			if (JavaScriptCompressor.Ones == null)
			{
				object synLock;
				Monitor.Enter(synLock = JavaScriptCompressor._synLock);
				try
				{
					if (JavaScriptCompressor.Ones == null)
					{
						List<string> list = new List<string>();
						for (char c = 'a'; c <= 'z'; c += '\u0001')
						{
							list.Add(Convert.ToString(c, CultureInfo.InvariantCulture));
						}
						for (char c2 = 'A'; c2 <= 'Z'; c2 += '\u0001')
						{
							list.Add(Convert.ToString(c2, CultureInfo.InvariantCulture));
						}
						JavaScriptCompressor.Ones = list;
					}
				}
				finally
				{
					Monitor.Exit(synLock);
				}
			}
		}
		private static void InitialiseTwosList()
		{
			if (JavaScriptCompressor.Twos == null)
			{
				object synLock;
				Monitor.Enter(synLock = JavaScriptCompressor._synLock);
				try
				{
					if (JavaScriptCompressor.Twos == null)
					{
						List<string> list = new List<string>();
						for (int i = 0; i < JavaScriptCompressor.Ones.Count; i++)
						{
							string str = JavaScriptCompressor.Ones[i];
							for (char c = 'a'; c <= 'z'; c += '\u0001')
							{
								list.Add(str + Convert.ToString(c, CultureInfo.InvariantCulture));
							}
							for (char c2 = 'A'; c2 <= 'Z'; c2 += '\u0001')
							{
								list.Add(str + Convert.ToString(c2, CultureInfo.InvariantCulture));
							}
							for (char c3 = '0'; c3 <= '9'; c3 += '\u0001')
							{
								list.Add(str + Convert.ToString(c3, CultureInfo.InvariantCulture));
							}
						}
						list.Remove("as");
						list.Remove("is");
						list.Remove("do");
						list.Remove("if");
						list.Remove("in");
						foreach (string current in JavaScriptCompressor._builtin)
						{
							list.Remove(current);
						}
						JavaScriptCompressor.Twos = list;
					}
				}
				finally
				{
					Monitor.Exit(synLock);
				}
			}
		}
		private static void InitialiseThreesList()
		{
			if (JavaScriptCompressor.Threes == null)
			{
				object synLock;
				Monitor.Enter(synLock = JavaScriptCompressor._synLock);
				try
				{
					if (JavaScriptCompressor.Threes == null)
					{
						List<string> list = new List<string>();
						for (int i = 0; i < JavaScriptCompressor.Twos.Count; i++)
						{
							string str = JavaScriptCompressor.Twos[i];
							for (char c = 'a'; c <= 'z'; c += '\u0001')
							{
								list.Add(str + Convert.ToString(c, CultureInfo.InvariantCulture));
							}
							for (char c2 = 'A'; c2 <= 'Z'; c2 += '\u0001')
							{
								list.Add(str + Convert.ToString(c2, CultureInfo.InvariantCulture));
							}
							for (char c3 = '0'; c3 <= '9'; c3 += '\u0001')
							{
								list.Add(str + Convert.ToString(c3, CultureInfo.InvariantCulture));
							}
						}
						list.Remove("for");
						list.Remove("int");
						list.Remove("new");
						list.Remove("try");
						list.Remove("use");
						list.Remove("var");
						foreach (string current in JavaScriptCompressor._builtin)
						{
							list.Remove(current);
						}
						JavaScriptCompressor.Threes = list;
					}
				}
				finally
				{
					Monitor.Exit(synLock);
				}
			}
		}
		private static void InitialiseLiterals()
		{
			if (JavaScriptCompressor.Literals == null)
			{
				object synLock;
				Monitor.Enter(synLock = JavaScriptCompressor._synLock);
				try
				{
					if (JavaScriptCompressor.Literals == null)
					{
						Hashtable literals = new Hashtable
						{

							{
								149,
								"get "
							},

							{
								150,
								"set "
							},

							{
								44,
								"true"
							},

							{
								43,
								"false"
							},

							{
								41,
								"null"
							},

							{
								42,
								"this"
							},

							{
								107,
								"function"
							},

							{
								87,
								","
							},

							{
								83,
								"{"
							},

							{
								84,
								"}"
							},

							{
								85,
								"("
							},

							{
								86,
								")"
							},

							{
								81,
								"["
							},

							{
								82,
								"]"
							},

							{
								106,
								"."
							},

							{
								30,
								"new "
							},

							{
								31,
								"delete "
							},

							{
								110,
								"if"
							},

							{
								111,
								"else"
							},

							{
								117,
								"for"
							},

							{
								51,
								" in "
							},

							{
								121,
								"with"
							},

							{
								115,
								"while"
							},

							{
								116,
								"do"
							},

							{
								79,
								"try"
							},

							{
								122,
								"catch"
							},

							{
								123,
								"finally"
							},

							{
								49,
								"throw"
							},

							{
								112,
								"switch"
							},

							{
								118,
								"break"
							},

							{
								119,
								"continue"
							},

							{
								113,
								"case"
							},

							{
								114,
								"default"
							},

							{
								4,
								"return"
							},

							{
								120,
								"var "
							},

							{
								80,
								";"
							},

							{
								88,
								"="
							},

							{
								95,
								"+="
							},

							{
								96,
								"-="
							},

							{
								97,
								"*="
							},

							{
								98,
								"/="
							},

							{
								99,
								"%="
							},

							{
								89,
								"|="
							},

							{
								90,
								"^="
							},

							{
								91,
								"&="
							},

							{
								92,
								"<<="
							},

							{
								93,
								">>="
							},

							{
								94,
								">>>="
							},

							{
								100,
								"?"
							},

							{
								64,
								":"
							},

							{
								101,
								":"
							},

							{
								102,
								"||"
							},

							{
								103,
								"&&"
							},

							{
								9,
								"|"
							},

							{
								10,
								"^"
							},

							{
								11,
								"&"
							},

							{
								45,
								"==="
							},

							{
								46,
								"!=="
							},

							{
								12,
								"=="
							},

							{
								13,
								"!="
							},

							{
								15,
								"<="
							},

							{
								14,
								"<"
							},

							{
								17,
								">="
							},

							{
								16,
								">"
							},

							{
								52,
								" instanceof "
							},

							{
								18,
								"<<"
							},

							{
								19,
								">>"
							},

							{
								20,
								">>>"
							},

							{
								32,
								"typeof"
							},

							{
								124,
								"void "
							},

							{
								151,
								"const "
							},

							{
								26,
								"!"
							},

							{
								27,
								"~"
							},

							{
								28,
								"+"
							},

							{
								29,
								"-"
							},

							{
								104,
								"++"
							},

							{
								105,
								"--"
							},

							{
								21,
								"+"
							},

							{
								22,
								"-"
							},

							{
								23,
								"*"
							},

							{
								24,
								"/"
							},

							{
								25,
								"%"
							},

							{
								142,
								"::"
							},

							{
								141,
								".."
							},

							{
								144,
								".("
							},

							{
								145,
								"@"
							}
						};
						JavaScriptCompressor.Literals = literals;
					}
				}
				finally
				{
					Monitor.Exit(synLock);
				}
			}
		}
		private static void InitialiseReserved()
		{
			if (JavaScriptCompressor.Reserved == null)
			{
				object synLock;
				Monitor.Enter(synLock = JavaScriptCompressor._synLock);
				try
				{
					if (JavaScriptCompressor.Reserved == null)
					{
						HashSet<string> reserved = new HashSet<string>
						{
							"break",
							"case",
							"catch",
							"continue",
							"default",
							"delete",
							"do",
							"else",
							"finally",
							"for",
							"function",
							"if",
							"in",
							"instanceof",
							"new",
							"return",
							"switch",
							"this",
							"throw",
							"try",
							"typeof",
							"var",
							"void",
							"while",
							"with",
							"abstract",
							"boolean",
							"byte",
							"char",
							"class",
							"const",
							"debugger",
							"double",
							"enum",
							"export",
							"extends",
							"final",
							"float",
							"goto",
							"implements",
							"import",
							"int",
							"interface",
							"long",
							"native",
							"package",
							"private",
							"protected",
							"public",
							"short",
							"static",
							"super",
							"synchronized",
							"throws",
							"transient",
							"volatile",
							"arguments",
							"eval",
							"true",
							"false",
							"Infinity",
							"NaN",
							"null",
							"undefined"
						};
						JavaScriptCompressor.Reserved = reserved;
					}
				}
				finally
				{
					Monitor.Exit(synLock);
				}
			}
		}
		private static void Initialise()
		{
			JavaScriptCompressor.InitialiseBuiltIn();
			JavaScriptCompressor.InitialiseOnesList();
			JavaScriptCompressor.InitialiseTwosList();
			JavaScriptCompressor.InitialiseThreesList();
			JavaScriptCompressor.InitialiseLiterals();
			JavaScriptCompressor.InitialiseReserved();
		}
		private static int CountChar(string haystack, char needle)
		{
			int i = 0;
			int num = 0;
			int length = haystack.Length;
			while (i < length)
			{
				char c = haystack[i++];
				if (c == needle)
				{
					num++;
				}
			}
			return num;
		}
		private static int PrintSourceString(string source, int offset, StringBuilder stringBuilder)
		{
			int num = (int)source[offset];
			offset++;
			if ((32768 & num) != 0)
			{
				num = ((32767 & num) << 16 | (int)source[offset]);
				offset++;
			}
			if (stringBuilder != null)
			{
				string value = source.Substring(offset, num);
				stringBuilder.Append(value);
			}
			return offset + num;
		}
		private static int PrintSourceNumber(string source, int offset, StringBuilder stringBuilder)
		{
			double d = 0.0;
			char c = source[offset];
			offset++;
			if (c == 'S')
			{
				if (stringBuilder != null)
				{
					d = (double)source[offset];
				}
				offset++;
			}
			else
			{
				if (c != 'J' && c != 'D')
				{
					throw new InvalidOperationException();
				}
				if (stringBuilder != null)
				{
					long num = (long)((long)((ulong)source[offset]) << 48);
					num |= (long)((long)((ulong)source[offset + 1]) << 32);
					num |= (long)((long)((ulong)source[offset + 2]) << 16);
					num |= (long)((ulong)source[offset + 3]);
					d = ((c == 'J') ? ((double)num) : BitConverter.Int64BitsToDouble(num));
				}
				offset += 4;
			}
			if (stringBuilder != null)
			{
				stringBuilder.Append(ScriptConvert.ToString(d, 10));
			}
			return offset;
		}
		private static ArrayList Parse(StreamReader stream, ErrorReporter reporter)
		{
			CompilerEnvirons compilerEnv = new CompilerEnvirons();
			Parser parser = new Parser(compilerEnv, reporter);
			parser.Parse(stream, null, 1);
			string encodedSource = parser.EncodedSource;
			int i = 0;
			int length = encodedSource.Length;
			ArrayList arrayList = new ArrayList();
			StringBuilder stringBuilder = new StringBuilder();
			while (i < length)
			{
				int num = (int)encodedSource[i++];
				int num2 = num;
				switch (num2)
				{
				case 38:
				case 40:
					break;
				case 39:
					stringBuilder.Length = 0;
					i = JavaScriptCompressor.PrintSourceNumber(encodedSource, i, stringBuilder);
					arrayList.Add(new JavaScriptToken(num, stringBuilder.ToString()));
					continue;
				default:
					if (num2 != 47)
					{
						switch (num2)
						{
						case 154:
						case 155:
							break;
						default:
						{
							string text = (string)JavaScriptCompressor.Literals[num];
							if (text != null)
							{
								arrayList.Add(new JavaScriptToken(num, text));
								continue;
							}
							continue;
						}
						}
					}
					break;
				}
				stringBuilder.Length = 0;
				i = JavaScriptCompressor.PrintSourceString(encodedSource, i, stringBuilder);
				arrayList.Add(new JavaScriptToken(num, stringBuilder.ToString()));
			}
			return arrayList;
		}
		private static void ProcessStringLiterals(IList tokens, bool merge)
		{
			int num = tokens.Count;
			if (merge)
			{
				for (int i = 0; i < num; i++)
				{
					JavaScriptToken javaScriptToken = (JavaScriptToken)tokens[i];
					int tokenType = javaScriptToken.TokenType;
					if (tokenType == 21 && i > 0 && i < num)
					{
						JavaScriptToken javaScriptToken2 = (JavaScriptToken)tokens[i - 1];
						JavaScriptToken javaScriptToken3 = (JavaScriptToken)tokens[i + 1];
						if (javaScriptToken2.TokenType == 40 && javaScriptToken3.TokenType == 40 && (i == num - 1 || ((JavaScriptToken)tokens[i + 2]).TokenType != 106))
						{
							tokens[i - 1] = new JavaScriptToken(40, javaScriptToken2.Value + javaScriptToken3.Value);
							tokens.RemoveAt(i + 1);
							tokens.RemoveAt(i);
							i--;
							num -= 2;
						}
					}
				}
			}
			for (int i = 0; i < num; i++)
			{
				JavaScriptToken javaScriptToken = (JavaScriptToken)tokens[i];
				if (javaScriptToken.TokenType == 40)
				{
					string text = javaScriptToken.Value;
					int num2 = JavaScriptCompressor.CountChar(text, '\'');
					int num3 = JavaScriptCompressor.CountChar(text, '"');
					char c = (num3 <= num2) ? '"' : '\'';
					text = c + JavaScriptCompressor.EscapeString(text, c) + c;
					if (text.IndexOf("</script", StringComparison.OrdinalIgnoreCase) >= 0)
					{
						text = text.Replace("<\\/script", "<\\\\/script");
					}
					tokens[i] = new JavaScriptToken(40, text);
				}
			}
		}
		private static string EscapeString(string s, char quotechar)
		{
			if (quotechar != '"' && quotechar != '\'')
			{
				throw new ArgumentException("quotechar argument has to be a \" or a \\ character only.", "quotechar");
			}
			if (string.IsNullOrEmpty(s))
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int i = 0;
			int length = s.Length;
			while (i < length)
			{
				int num = (int)s[i];
				if (num == (int)quotechar)
				{
					stringBuilder.Append("\\");
				}
				stringBuilder.Append((char)num);
				i++;
			}
			return stringBuilder.ToString();
		}
		private static bool IsValidIdentifier(string s)
		{
			Match match = JavaScriptCompressor.SIMPLE_IDENTIFIER_NAME_PATTERN.Match(s);
			return match.Success && !JavaScriptCompressor.Reserved.Contains(s);
		}
		private static void OptimizeObjectMemberAccess(IList tokens)
		{
			int i = 0;
			int num = tokens.Count;
			while (i < num)
			{
				if (((JavaScriptToken)tokens[i]).TokenType == 81 && i > 0 && i < num - 2 && ((JavaScriptToken)tokens[i - 1]).TokenType == 38 && ((JavaScriptToken)tokens[i + 1]).TokenType == 40 && ((JavaScriptToken)tokens[i + 2]).TokenType == 82)
				{
					JavaScriptToken javaScriptToken = (JavaScriptToken)tokens[i + 1];
					string text = javaScriptToken.Value;
					text = text.Substring(1, text.Length - 2);
					if (JavaScriptCompressor.IsValidIdentifier(text))
					{
						tokens[i] = new JavaScriptToken(106, ".");
						tokens[i + 1] = new JavaScriptToken(38, text);
						tokens.RemoveAt(i + 2);
						i += 2;
						num--;
					}
				}
				i++;
			}
		}
		private static void OptimizeObjLitMemberDecl(IList tokens)
		{
			int i = 0;
			int count = tokens.Count;
			while (i < count)
			{
				if (((JavaScriptToken)tokens[i]).TokenType == 64 && i > 0 && ((JavaScriptToken)tokens[i - 1]).TokenType == 40)
				{
					JavaScriptToken javaScriptToken = (JavaScriptToken)tokens[i - 1];
					string text = javaScriptToken.Value;
					text = text.Substring(1, text.Length - 2);
					if (JavaScriptCompressor.IsValidIdentifier(text))
					{
						tokens[i - 1] = new JavaScriptToken(38, text);
					}
				}
				i++;
			}
		}
		private ScriptOrFunctionScope GetCurrentScope()
		{
			return (ScriptOrFunctionScope)this._scopes.Peek();
		}
		private void EnterScope(ScriptOrFunctionScope scope)
		{
			this._scopes.Push(scope);
		}
		private void LeaveCurrentScope()
		{
			this._scopes.Pop();
		}
		private JavaScriptToken ConsumeToken()
		{
			return (JavaScriptToken)this._tokens[this._offset++];
		}
		private JavaScriptToken GetToken(int delta)
		{
			return (JavaScriptToken)this._tokens[this._offset + delta];
		}
		private static JavaScriptIdentifier GetIdentifier(string symbol, ScriptOrFunctionScope scope)
		{
			while (scope != null)
			{
				JavaScriptIdentifier identifier = scope.GetIdentifier(symbol);
				if (identifier != null)
				{
					return identifier;
				}
				scope = scope.ParentScope;
			}
			return null;
		}
		private void ProtectScopeFromObfuscation(ScriptOrFunctionScope scope)
		{
			if (scope == null)
			{
				throw new ArgumentNullException("scope");
			}
			if (scope == this._globalScope)
			{
				return;
			}
			while (scope.ParentScope != this._globalScope)
			{
				scope = scope.ParentScope;
			}
			if (scope.ParentScope != this._globalScope)
			{
				throw new InvalidOperationException();
			}
			scope.PreventMunging();
		}
		private string GetDebugString(int max)
		{
			if (max <= 0)
			{
				throw new ArgumentOutOfRangeException("max");
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = Math.Max(this._offset - max, 0);
			int num2 = Math.Min(this._offset + max, this._tokens.Count);
			for (int i = num; i < num2; i++)
			{
				JavaScriptToken javaScriptToken = (JavaScriptToken)this._tokens[i];
				if (i == this._offset - 1)
				{
					stringBuilder.Append(" ---> ");
				}
				stringBuilder.Append(javaScriptToken.Value);
				if (i == this._offset - 1)
				{
					stringBuilder.Append(" <--- ");
				}
			}
			return stringBuilder.ToString();
		}
		private void Warn(string message, bool showDebugString)
		{
			if (this._verbose)
			{
				if (showDebugString)
				{
					message = message + Environment.NewLine + this.GetDebugString(10);
				}
				this._logger.Warning(message, null, -1, null, -1);
			}
		}
		private void ParseFunctionDeclaration()
		{
			ScriptOrFunctionScope currentScope = this.GetCurrentScope();
			JavaScriptToken javaScriptToken = this.ConsumeToken();
			if (javaScriptToken.TokenType == 38)
			{
				if (this._mode == 1)
				{
					string value = javaScriptToken.Value;
					if (currentScope.GetIdentifier(value) != null)
					{
						this.Warn("The function " + value + " has already been declared in the same scope...", true);
					}
					currentScope.DeclareIdentifier(value);
				}
				javaScriptToken = this.ConsumeToken();
			}
			if (javaScriptToken.TokenType != 85)
			{
				throw new InvalidOperationException();
			}
			ScriptOrFunctionScope scriptOrFunctionScope;
			if (this._mode == 1)
			{
				scriptOrFunctionScope = new ScriptOrFunctionScope(this._braceNesting, currentScope);
				this._indexedScopes.Add(this._offset, scriptOrFunctionScope);
			}
			else
			{
				scriptOrFunctionScope = (ScriptOrFunctionScope)this._indexedScopes[this._offset];
			}
			int num = 0;
			while ((javaScriptToken = this.ConsumeToken()).TokenType != 86)
			{
				if (javaScriptToken.TokenType != 38 && javaScriptToken.TokenType != 87)
				{
					throw new InvalidOperationException();
				}
				if (javaScriptToken.TokenType == 38 && this._mode == 1)
				{
					string value = javaScriptToken.Value;
					JavaScriptIdentifier javaScriptIdentifier = scriptOrFunctionScope.DeclareIdentifier(value);
					if (value.Equals("$super", StringComparison.OrdinalIgnoreCase) && num == 0)
					{
						javaScriptIdentifier.MarkedForMunging = false;
					}
					num++;
				}
			}
			javaScriptToken = this.ConsumeToken();
			if (javaScriptToken.TokenType != 83)
			{
				throw new InvalidOperationException();
			}
			this._braceNesting++;
			javaScriptToken = this.GetToken(0);
			if (javaScriptToken.TokenType == 40 && this.GetToken(1).TokenType == 80)
			{
				this.ConsumeToken();
				string text = javaScriptToken.Value;
				text = text.Substring(1, text.Length - 2).Trim();
				string[] array = text.Split(new char[]
				{
					','
				});
				int i = 0;
				while (i < array.Length)
				{
					string text2 = array[i];
					int num2 = text2.IndexOf(':');
					if (num2 <= 0 || num2 >= text2.Length - 1)
					{
						if (this._mode == 1)
						{
							this.Warn("Invalid hint syntax: " + text2, true);
							break;
						}
						break;
					}
					else
					{
						string text3 = text2.Substring(0, num2).Trim();
						string text4 = text2.Substring(num2 + 1).Trim();
						if (this._mode == 1)
						{
							scriptOrFunctionScope.AddHint(text3, text4);
						}
						else
						{
							if (this._mode == JavaScriptCompressor.CHECKING_SYMBOL_TREE)
							{
								JavaScriptIdentifier javaScriptIdentifier = scriptOrFunctionScope.GetIdentifier(text3);
								if (javaScriptIdentifier != null)
								{
									if (text4.Equals("nomunge", StringComparison.OrdinalIgnoreCase))
									{
										javaScriptIdentifier.MarkedForMunging = false;
									}
									else
									{
										this.Warn("Unsupported hint value: " + text2, true);
									}
								}
								else
								{
									this.Warn("Hint refers to an unknown identifier: " + text2, true);
								}
							}
						}
						i++;
					}
				}
			}
			this.ParseScope(scriptOrFunctionScope);
		}
		private void ParseCatch()
		{
			JavaScriptToken javaScriptToken = this.GetToken(-1);
			if (javaScriptToken.TokenType != 122)
			{
				throw new InvalidOperationException();
			}
			javaScriptToken = this.ConsumeToken();
			if (javaScriptToken.TokenType != 85)
			{
				throw new InvalidOperationException();
			}
			javaScriptToken = this.ConsumeToken();
			if (javaScriptToken.TokenType != 38)
			{
				throw new InvalidOperationException();
			}
			string value = javaScriptToken.Value;
			ScriptOrFunctionScope currentScope = this.GetCurrentScope();
			if (this._mode == 1)
			{
				currentScope.DeclareIdentifier(value);
			}
			else
			{
				JavaScriptIdentifier identifier = JavaScriptCompressor.GetIdentifier(value, currentScope);
				identifier.RefCount++;
			}
			javaScriptToken = this.ConsumeToken();
			if (javaScriptToken.TokenType != 86)
			{
				throw new InvalidOperationException();
			}
		}
		private void ParseExpression()
		{
			int braceNesting = this._braceNesting;
			int num = 0;
			int num2 = 0;
			int count = this._tokens.Count;
			while (this._offset < count)
			{
				JavaScriptToken javaScriptToken = this.ConsumeToken();
				ScriptOrFunctionScope currentScope = this.GetCurrentScope();
				int tokenType = javaScriptToken.TokenType;
				if (tokenType <= 87)
				{
					if (tokenType != 38)
					{
						switch (tokenType)
						{
						case 80:
						case 87:
							if (this._braceNesting == braceNesting && num == 0 && num2 == 0)
							{
								return;
							}
							break;
						case 81:
							num++;
							break;
						case 82:
							num--;
							break;
						case 83:
							this._braceNesting++;
							break;
						case 84:
							this._braceNesting--;
							if (this._braceNesting < braceNesting)
							{
								throw new InvalidOperationException();
							}
							break;
						case 85:
							num2++;
							break;
						case 86:
							num2--;
							break;
						}
					}
					else
					{
						string value = javaScriptToken.Value;
						if (this._mode == 1)
						{
							if (!this._isEvalIgnored && value.Equals("eval", StringComparison.OrdinalIgnoreCase))
							{
								this.ProtectScopeFromObfuscation(currentScope);
								this.Warn("Using 'eval' is not recommended." + (this._munge ? " Moreover, using 'eval' reduces the level of compression!" : ""), true);
							}
						}
						else
						{
							if (this._mode == JavaScriptCompressor.CHECKING_SYMBOL_TREE && (this._offset < 2 || (this.GetToken(-2).TokenType != 106 && this.GetToken(-2).TokenType != 149 && this.GetToken(-2).TokenType != 150)) && this.GetToken(0).TokenType != 64)
							{
								JavaScriptIdentifier identifier = JavaScriptCompressor.GetIdentifier(value, currentScope);
								if (identifier == null)
								{
									if (value.Length <= 3 && !JavaScriptCompressor._builtin.Contains(value))
									{
										this._globalScope.DeclareIdentifier(value);
									}
								}
								else
								{
									identifier.RefCount++;
								}
							}
						}
					}
				}
				else
				{
					if (tokenType != 107)
					{
						if (tokenType == 154)
						{
							if (this._mode == 1)
							{
								this.ProtectScopeFromObfuscation(currentScope);
								this.Warn("Using JScript conditional comments is not recommended." + (this._munge ? " Moreover, using JScript conditional comments reduces the level of compression!" : ""), true);
							}
						}
					}
					else
					{
						this.ParseFunctionDeclaration();
					}
				}
			}
		}
		private void ParseScope(ScriptOrFunctionScope scope)
		{
			int count = this._tokens.Count;
			this.EnterScope(scope);
			IL_378:
			while (this._offset < count)
			{
				JavaScriptToken javaScriptToken = this.ConsumeToken();
				int tokenType = javaScriptToken.TokenType;
				if (tokenType > 107)
				{
					switch (tokenType)
					{
					case 120:
						break;
					case 121:
						if (this._mode == 1)
						{
							this.ProtectScopeFromObfuscation(scope);
							this.Warn("Using 'with' is not recommended." + (this._munge ? " Moreover, using 'with' reduces the level of compression!" : ""), true);
							continue;
						}
						continue;
					case 122:
						this.ParseCatch();
						continue;
					default:
						if (tokenType != 151)
						{
							if (tokenType != 154)
							{
								continue;
							}
							if (this._mode == 1)
							{
								this.ProtectScopeFromObfuscation(scope);
								this.Warn("Using JScript conditional comments is not recommended." + (this._munge ? " Moreover, using JScript conditional comments reduces the level of compression." : ""), true);
								continue;
							}
							continue;
						}
						break;
					}
					if (javaScriptToken.TokenType == 120 && this._mode == 1 && scope.VarCount++ > 1)
					{
						this.Warn("Try to use a single 'var' statement per scope.", true);
					}
					while (true)
					{
						javaScriptToken = this.ConsumeToken();
						if (javaScriptToken.TokenType != 38)
						{
							break;
						}
						if (this._mode == 1)
						{
							string value = javaScriptToken.Value;
							if (scope.GetIdentifier(value) == null)
							{
								scope.DeclareIdentifier(value);
							}
							else
							{
								this.Warn("The variable " + value + " has already been declared in the same scope...", true);
							}
						}
						javaScriptToken = this.GetToken(0);
						if (javaScriptToken.TokenType != 80 && javaScriptToken.TokenType != 88 && javaScriptToken.TokenType != 87 && javaScriptToken.TokenType != 51)
						{
							goto Block_17;
						}
						if (javaScriptToken.TokenType == 51)
						{
							goto IL_378;
						}
						this.ParseExpression();
						javaScriptToken = this.GetToken(-1);
						if (javaScriptToken.TokenType == 80)
						{
							goto Block_19;
						}
					}
					throw new InvalidOperationException();
					Block_19:
					continue;
					Block_17:
					throw new InvalidOperationException();
				}
				if (tokenType != 38)
				{
					switch (tokenType)
					{
					case 83:
						this._braceNesting++;
						break;
					case 84:
						this._braceNesting--;
						if (this._braceNesting < scope.BraceNesting)
						{
							throw new InvalidOperationException();
						}
						if (this._braceNesting == scope.BraceNesting)
						{
							this.LeaveCurrentScope();
							return;
						}
						break;
					default:
						if (tokenType == 107)
						{
							this.ParseFunctionDeclaration();
						}
						break;
					}
				}
				else
				{
					string value = javaScriptToken.Value;
					if (this._mode == 1)
					{
						if (!this._isEvalIgnored && value.Equals("eval", StringComparison.OrdinalIgnoreCase))
						{
							this.ProtectScopeFromObfuscation(scope);
							this.Warn("Using 'eval' is not recommended." + (this._munge ? " Moreover, using 'eval' reduces the level of compression!" : ""), true);
						}
					}
					else
					{
						if (this._mode == JavaScriptCompressor.CHECKING_SYMBOL_TREE && (this._offset < 2 || this.GetToken(-2).TokenType != 106) && this.GetToken(0).TokenType != 64)
						{
							JavaScriptIdentifier identifier = JavaScriptCompressor.GetIdentifier(value, scope);
							if (identifier == null)
							{
								if (value.Length <= 3 && !JavaScriptCompressor._builtin.Contains(value))
								{
									this._globalScope.DeclareIdentifier(value);
								}
							}
							else
							{
								identifier.RefCount++;
							}
						}
					}
				}
			}
		}
		private void BuildSymbolTree()
		{
			this._offset = 0;
			this._braceNesting = 0;
			this._scopes.Clear();
			this._indexedScopes.Clear();
			this._indexedScopes.Add(0, this._globalScope);
			this._mode = 1;
			this.ParseScope(this._globalScope);
		}
		private void MungeSymboltree()
		{
			if (!this._munge)
			{
				return;
			}
			this._offset = 0;
			this._braceNesting = 0;
			this._scopes.Clear();
			this._mode = JavaScriptCompressor.CHECKING_SYMBOL_TREE;
			this.ParseScope(this._globalScope);
			this._globalScope.Munge();
		}
		private StringBuilder PrintSymbolTree(int linebreakpos, bool preserveAllSemiColons)
		{
			this._offset = 0;
			this._braceNesting = 0;
			this._scopes.Clear();
			int count = this._tokens.Count;
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			this.EnterScope(this._globalScope);
			while (this._offset < count)
			{
				JavaScriptToken javaScriptToken = this.ConsumeToken();
				string value = javaScriptToken.Value;
				ScriptOrFunctionScope scriptOrFunctionScope = this.GetCurrentScope();
				int tokenType = javaScriptToken.TokenType;
				if (tokenType <= 49)
				{
					if (tokenType <= 22)
					{
						if (tokenType != 4)
						{
							switch (tokenType)
							{
							case 21:
							case 22:
								stringBuilder.Append((string)JavaScriptCompressor.Literals[javaScriptToken.TokenType]);
								if (this._offset >= count)
								{
									continue;
								}
								javaScriptToken = this.GetToken(0);
								if (javaScriptToken.TokenType == 104 || javaScriptToken.TokenType == 105 || javaScriptToken.TokenType == 21 || javaScriptToken.TokenType == 105)
								{
									stringBuilder.Append(' ');
									continue;
								}
								if ((javaScriptToken.TokenType == 28 && this.GetToken(-1).TokenType == 21) || (javaScriptToken.TokenType == 29 && this.GetToken(-1).TokenType == 22))
								{
									stringBuilder.Append(' ');
									continue;
								}
								continue;
							default:
								goto IL_784;
							}
						}
					}
					else
					{
						if (tokenType != 32)
						{
							switch (tokenType)
							{
							case 38:
							{
								if ((this._offset >= 2 && this.GetToken(-2).TokenType == 106) || this.GetToken(0).TokenType == 64)
								{
									stringBuilder.Append(value);
									continue;
								}
								JavaScriptIdentifier identifier = JavaScriptCompressor.GetIdentifier(value, scriptOrFunctionScope);
								if (identifier == null)
								{
									stringBuilder.Append(value);
									continue;
								}
								if (identifier.MungedValue != null)
								{
									stringBuilder.Append(identifier.MungedValue);
								}
								else
								{
									stringBuilder.Append(value);
								}
								if (scriptOrFunctionScope != this._globalScope && identifier.RefCount == 0)
								{
									this.Warn(string.Concat(new string[]
									{
										"The symbol ",
										value,
										" is declared but is apparently never used.",
										Environment.NewLine,
										"This code can probably be written in a more compact way."
									}), true);
									continue;
								}
								continue;
							}
							case 39:
							case 40:
								break;
							default:
								switch (tokenType)
								{
								case 47:
									break;
								case 48:
									goto IL_784;
								case 49:
									goto IL_5D5;
								default:
									goto IL_784;
								}
								break;
							}
							stringBuilder.Append(value);
							continue;
						}
					}
					stringBuilder.Append((string)JavaScriptCompressor.Literals[javaScriptToken.TokenType]);
					if (this._offset >= count)
					{
						continue;
					}
					javaScriptToken = this.GetToken(0);
					if (javaScriptToken.TokenType != 85 && javaScriptToken.TokenType != 81 && javaScriptToken.TokenType != 83 && javaScriptToken.TokenType != 40 && javaScriptToken.TokenType != 47 && javaScriptToken.TokenType != 80)
					{
						stringBuilder.Append(' ');
						continue;
					}
					continue;
				}
				else
				{
					if (tokenType <= 107)
					{
						switch (tokenType)
						{
						case 80:
							if (preserveAllSemiColons || (this._offset < count && this.GetToken(0).TokenType != 84))
							{
								stringBuilder.Append(';');
							}
							if (linebreakpos >= 0 && stringBuilder.Length - num > linebreakpos)
							{
								stringBuilder.Append('\n');
								num = stringBuilder.Length;
								continue;
							}
							continue;
						case 81:
						case 82:
							goto IL_784;
						case 83:
							stringBuilder.Append('{');
							this._braceNesting++;
							continue;
						case 84:
							stringBuilder.Append('}');
							this._braceNesting--;
							if (this._braceNesting < scriptOrFunctionScope.BraceNesting)
							{
								throw new InvalidOperationException();
							}
							if (this._braceNesting == scriptOrFunctionScope.BraceNesting)
							{
								this.LeaveCurrentScope();
								continue;
							}
							continue;
						default:
							if (tokenType != 107)
							{
								goto IL_784;
							}
							stringBuilder.Append("function");
							javaScriptToken = this.ConsumeToken();
							if (javaScriptToken.TokenType == 38)
							{
								stringBuilder.Append(' ');
								value = javaScriptToken.Value;
								JavaScriptIdentifier identifier = JavaScriptCompressor.GetIdentifier(value, scriptOrFunctionScope);
								if (identifier == null)
								{
									throw new InvalidOperationException();
								}
								if (identifier.MungedValue != null)
								{
									stringBuilder.Append(identifier.MungedValue);
								}
								else
								{
									stringBuilder.Append(value);
								}
								if (scriptOrFunctionScope != this._globalScope && identifier.RefCount == 0)
								{
									this.Warn(string.Concat(new string[]
									{
										"The symbol ",
										value,
										" is declared but is apparently never used.",
										Environment.NewLine,
										"This code can probably be written in a more compact way."
									}), true);
								}
								javaScriptToken = this.ConsumeToken();
							}
							if (javaScriptToken.TokenType != 85)
							{
								throw new InvalidOperationException();
							}
							stringBuilder.Append('(');
							scriptOrFunctionScope = (ScriptOrFunctionScope)this._indexedScopes[this._offset];
							this.EnterScope(scriptOrFunctionScope);
							while ((javaScriptToken = this.ConsumeToken()).TokenType != 86)
							{
								if (javaScriptToken.TokenType != 38 && javaScriptToken.TokenType != 87)
								{
									throw new InvalidOperationException();
								}
								if (javaScriptToken.TokenType == 38)
								{
									value = javaScriptToken.Value;
									JavaScriptIdentifier identifier = JavaScriptCompressor.GetIdentifier(value, scriptOrFunctionScope);
									if (identifier == null)
									{
										throw new InvalidOperationException();
									}
									if (identifier.MungedValue != null)
									{
										stringBuilder.Append(identifier.MungedValue);
									}
									else
									{
										stringBuilder.Append(value);
									}
								}
								else
								{
									if (javaScriptToken.TokenType == 87)
									{
										stringBuilder.Append(',');
									}
								}
							}
							stringBuilder.Append(')');
							javaScriptToken = this.ConsumeToken();
							if (javaScriptToken.TokenType != 83)
							{
								throw new InvalidOperationException();
							}
							stringBuilder.Append('{');
							this._braceNesting++;
							javaScriptToken = this.GetToken(0);
							if (javaScriptToken.TokenType == 40 && this.GetToken(1).TokenType == 80)
							{
								this.ConsumeToken();
								this.ConsumeToken();
								continue;
							}
							continue;
						}
					}
					else
					{
						if (tokenType != 113)
						{
							switch (tokenType)
							{
							case 118:
							case 119:
								stringBuilder.Append((string)JavaScriptCompressor.Literals[javaScriptToken.TokenType]);
								if (this._offset < count && this.GetToken(0).TokenType != 80)
								{
									stringBuilder.Append(' ');
									continue;
								}
								continue;
							default:
								switch (tokenType)
								{
								case 154:
								case 155:
									if (stringBuilder.Length > 0 && stringBuilder[stringBuilder.Length - 1] != '\n')
									{
										stringBuilder.Append("\n");
									}
									stringBuilder.Append("/*");
									stringBuilder.Append(value);
									stringBuilder.Append("*/\n");
									continue;
								default:
									goto IL_784;
								}
								break;
							}
						}
					}
				}
				IL_5D5:
				stringBuilder.Append((string)JavaScriptCompressor.Literals[javaScriptToken.TokenType]);
				if (this._offset < count && this.GetToken(0).TokenType != 40)
				{
					stringBuilder.Append(' ');
					continue;
				}
				continue;
				IL_784:
				string text = (string)JavaScriptCompressor.Literals[javaScriptToken.TokenType];
				if (text != null)
				{
					stringBuilder.Append(text);
				}
				else
				{
					this.Warn("This symbol cannot be printed: " + value, true);
				}
			}
			if (!preserveAllSemiColons && stringBuilder.Length > 0 && this.GetToken(-1).TokenType != 154 && this.GetToken(-1).TokenType != 155)
			{
				if (stringBuilder[stringBuilder.Length - 1] == '\n')
				{
					stringBuilder[stringBuilder.Length - 1] = ';';
				}
				else
				{
					stringBuilder.Append(';');
				}
			}
			return stringBuilder;
		}
		public static string Compress(string javaScript)
		{
			return JavaScriptCompressor.Compress(javaScript, true);
		}
		public static string Compress(string javaScript, bool isVerboseLogging)
		{
			return JavaScriptCompressor.Compress(javaScript, isVerboseLogging, true, false, false, -1);
		}
		public static string Compress(string javaScript, bool isVerboseLogging, bool isObfuscateJavascript, bool preserveAllSemicolons, bool disableOptimizations, int lineBreakPosition)
		{
			return JavaScriptCompressor.Compress(javaScript, isVerboseLogging, isObfuscateJavascript, preserveAllSemicolons, disableOptimizations, lineBreakPosition, Encoding.Default, null);
		}
		public static string Compress(string javaScript, bool isVerboseLogging, bool isObfuscateJavascript, bool preserveAllSemicolons, bool disableOptimizations, int lineBreakPosition, Encoding encoding, CultureInfo threadCulture)
		{
			return JavaScriptCompressor.Compress(javaScript, isVerboseLogging, isObfuscateJavascript, preserveAllSemicolons, disableOptimizations, lineBreakPosition, encoding, threadCulture, false);
		}
		public static string Compress(string javaScript, bool isVerboseLogging, bool isObfuscateJavascript, bool preserveAllSemicolons, bool disableOptimizations, int lineBreakPosition, Encoding encoding, CultureInfo threadCulture, bool isEvalIgnored)
		{
			if (string.IsNullOrEmpty(javaScript))
			{
				throw new ArgumentNullException("javaScript");
			}
			JavaScriptCompressor javaScriptCompressor = new JavaScriptCompressor(javaScript, isVerboseLogging, encoding, threadCulture, isEvalIgnored, null);
			return javaScriptCompressor.Compress(isObfuscateJavascript, preserveAllSemicolons, disableOptimizations, lineBreakPosition);
		}
		public string Compress()
		{
			return this.Compress(true, false, false, -1);
		}
		public string Compress(bool isObfuscateJavascript, bool preserveAllSemicolons, bool disableOptimizations, int lineBreakPosition)
		{
			this._munge = isObfuscateJavascript;
			JavaScriptCompressor.ProcessStringLiterals(this._tokens, !disableOptimizations);
			if (!disableOptimizations)
			{
				JavaScriptCompressor.OptimizeObjectMemberAccess(this._tokens);
				JavaScriptCompressor.OptimizeObjLitMemberDecl(this._tokens);
			}
			this.BuildSymbolTree();
			this.MungeSymboltree();
			StringBuilder stringBuilder = this.PrintSymbolTree(lineBreakPosition, preserveAllSemicolons);
			return stringBuilder.ToString();
		}
	}
}
