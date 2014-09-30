using Antlr.Runtime;
using System;
public class NCalcLexer : Lexer
{
	protected class DFA7 : DFA
	{
		public override string Description
		{
			get
			{
				return "252:1: FLOAT : ( ( DIGIT )* '.' ( DIGIT )+ ( E )? | ( DIGIT )+ E );";
			}
		}
		public DFA7(BaseRecognizer recognizer)
		{
			this.recognizer = recognizer;
			this.decisionNumber = 7;
			this.eot = NCalcLexer.DFA7_eot;
			this.eof = NCalcLexer.DFA7_eof;
			this.min = NCalcLexer.DFA7_min;
			this.max = NCalcLexer.DFA7_max;
			this.accept = NCalcLexer.DFA7_accept;
			this.special = NCalcLexer.DFA7_special;
			this.transition = NCalcLexer.DFA7_transition;
		}
	}
	protected class DFA14 : DFA
	{
		public override string Description
		{
			get
			{
				return "1:1: Tokens : ( T__19 | T__20 | T__21 | T__22 | T__23 | T__24 | T__25 | T__26 | T__27 | T__28 | T__29 | T__30 | T__31 | T__32 | T__33 | T__34 | T__35 | T__36 | T__37 | T__38 | T__39 | T__40 | T__41 | T__42 | T__43 | T__44 | T__45 | T__46 | T__47 | T__48 | TRUE | FALSE | ID | INTEGER | FLOAT | STRING | DATETIME | NAME | E | WS );";
			}
		}
		public DFA14(BaseRecognizer recognizer)
		{
			this.recognizer = recognizer;
			this.decisionNumber = 14;
			this.eot = NCalcLexer.DFA14_eot;
			this.eof = NCalcLexer.DFA14_eof;
			this.min = NCalcLexer.DFA14_min;
			this.max = NCalcLexer.DFA14_max;
			this.accept = NCalcLexer.DFA14_accept;
			this.special = NCalcLexer.DFA14_special;
			this.transition = NCalcLexer.DFA14_transition;
		}
	}
	public const int T__29 = 29;
	public const int T__28 = 28;
	public const int T__27 = 27;
	public const int T__26 = 26;
	public const int T__25 = 25;
	public const int T__24 = 24;
	public const int LETTER = 12;
	public const int T__23 = 23;
	public const int T__22 = 22;
	public const int T__21 = 21;
	public const int T__20 = 20;
	public const int FLOAT = 5;
	public const int ID = 10;
	public const int EOF = -1;
	public const int HexDigit = 17;
	public const int T__19 = 19;
	public const int NAME = 11;
	public const int DIGIT = 13;
	public const int T__42 = 42;
	public const int INTEGER = 4;
	public const int E = 14;
	public const int T__43 = 43;
	public const int T__40 = 40;
	public const int T__41 = 41;
	public const int T__46 = 46;
	public const int T__47 = 47;
	public const int T__44 = 44;
	public const int T__45 = 45;
	public const int T__48 = 48;
	public const int DATETIME = 7;
	public const int TRUE = 8;
	public const int T__30 = 30;
	public const int T__31 = 31;
	public const int T__32 = 32;
	public const int WS = 18;
	public const int T__33 = 33;
	public const int T__34 = 34;
	public const int T__35 = 35;
	public const int T__36 = 36;
	public const int T__37 = 37;
	public const int T__38 = 38;
	public const int T__39 = 39;
	public const int UnicodeEscape = 16;
	public const int FALSE = 9;
	public const int EscapeSequence = 15;
	public const int STRING = 6;
	private const string DFA7_eotS = "\u0004￿";
	private const string DFA7_eofS = "\u0004￿";
	private const string DFA7_minS = "\u0002.\u0002￿";
	private const string DFA7_maxS = "\u00019\u0001e\u0002￿";
	private const string DFA7_acceptS = "\u0002￿\u0001\u0001\u0001\u0002";
	private const string DFA7_specialS = "\u0004￿}>";
	private const string DFA14_eotS = "\u0003￿\u0001!\u0001\u001e\u0001$\u0001\u001e\u0001￿\u0001'\u0001)\u0001-\u00010\u0005￿\u0001\u001e\u0004￿\u0003\u001e\u00016\b￿\u00017\u0002￿\u0001\u001e\v￿\u0003\u001e\u0001￿\u0001\u001e\u0002￿\u0001<\u0001=\u0002\u001e\u0002￿\u0001@\u0001\u001e\u0001￿\u0001B\u0001￿";
	private const string DFA14_eofS = "C￿";
	private const string DFA14_minS = "\u0001\t\u0002￿\u0001|\u0001r\u0001&\u0001n\u0001￿\u0002=\u0001<\u0001=\u0005￿\u0001o\u0004￿\u0001r\u0001a\u0001+\u0001.\b￿\u00010\u0002￿\u0001d\v￿\u0001t\u0001u\u0001l\u0001￿\u00010\u0002￿\u00020\u0001e\u0001s\u0002￿\u00010\u0001e\u0001￿\u00010\u0001￿";
	private const string DFA14_maxS = "\u0001~\u0002￿\u0001|\u0001r\u0001&\u0001n\u0001￿\u0002=\u0002>\u0005￿\u0001o\u0004￿\u0001r\u0001a\u00019\u0001e\b￿\u0001z\u0002￿\u0001d\v￿\u0001t\u0001u\u0001l\u0001￿\u00019\u0002￿\u0002z\u0001e\u0001s\u0002￿\u0001z\u0001e\u0001￿\u0001z\u0001￿";
	private const string DFA14_acceptS = "\u0001￿\u0001\u0001\u0001\u0002\u0004￿\u0001\b\u0004￿\u0001\u0014\u0001\u0015\u0001\u0016\u0001\u0017\u0001\u0018\u0001￿\u0001\u001b\u0001\u001c\u0001\u001d\u0001\u001e\u0004￿\u0001#\u0001$\u0001%\u0001&\u0001!\u0001(\u0001\u0003\u0001\a\u0001￿\u0001\u0005\u0001\t\u0001￿\u0001\n\u0001\v\u0001\f\u0001\u0019\u0001\r\u0001\u000f\u0001\u0012\u0001\u000e\u0001\u0011\u0001\u0013\u0001\u0010\u0003￿\u0001'\u0001￿\u0001\"\u0001\u0004\u0004￿\u0001\u0006\u0001\u001a\u0002￿\u0001\u001f\u0001￿\u0001 ";
	private const string DFA14_specialS = "C￿}>";
	protected NCalcLexer.DFA7 dfa7;
	protected NCalcLexer.DFA14 dfa14;
	private static readonly string[] DFA7_transitionS = new string[]
	{
		"\u0001\u0002\u0001￿\n\u0001",
		"\u0001\u0002\u0001￿\n\u0001\v￿\u0001\u0003\u001f￿\u0001\u0003",
		"",
		""
	};
	private static readonly short[] DFA7_eot = DFA.UnpackEncodedString("\u0004￿");
	private static readonly short[] DFA7_eof = DFA.UnpackEncodedString("\u0004￿");
	private static readonly char[] DFA7_min = DFA.UnpackEncodedStringToUnsignedChars("\u0002.\u0002￿");
	private static readonly char[] DFA7_max = DFA.UnpackEncodedStringToUnsignedChars("\u00019\u0001e\u0002￿");
	private static readonly short[] DFA7_accept = DFA.UnpackEncodedString("\u0002￿\u0001\u0001\u0001\u0002");
	private static readonly short[] DFA7_special = DFA.UnpackEncodedString("\u0004￿}>");
	private static readonly short[][] DFA7_transition = DFA.UnpackEncodedStringArray(NCalcLexer.DFA7_transitionS);
	private static readonly string[] DFA14_transitionS = new string[]
	{
		"\u0002\u001f\u0001￿\u0002\u001f\u0012￿\u0001\u001f\u0001\t\u0001￿\u0001\u001c\u0001￿\u0001\u0010\u0001\u0005\u0001\u001b\u0001\u0013\u0001\u0014\u0001\u000e\u0001\f\u0001\u0015\u0001\r\u0001\u001a\u0001\u000f\n\u0019\u0001\u0002\u0001￿\u0001\n\u0001\b\u0001\v\u0001\u0001\u0001￿\u0004\u001e\u0001\u0018\u0015\u001e\u0001\u001d\u0002￿\u0001\a\u0001\u001e\u0001￿\u0001\u0006\u0003\u001e\u0001\u0018\u0001\u0017\a\u001e\u0001\u0011\u0001\u0004\u0004\u001e\u0001\u0016\u0006\u001e\u0001￿\u0001\u0003\u0001￿\u0001\u0012",
		"",
		"",
		"\u0001 ",
		"\u0001\"",
		"\u0001#",
		"\u0001%",
		"",
		"\u0001&",
		"\u0001(",
		"\u0001,\u0001+\u0001*",
		"\u0001.\u0001/",
		"",
		"",
		"",
		"",
		"",
		"\u00011",
		"",
		"",
		"",
		"",
		"\u00012",
		"\u00013",
		"\u00014\u0001￿\u00014\u0002￿\n5",
		"\u0001\u001a\u0001￿\n\u0019\v￿\u0001\u001a\u001f￿\u0001\u001a",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"\n\u001e\a￿\u001a\u001e\u0004￿\u0001\u001e\u0001￿\u001a\u001e",
		"",
		"",
		"\u00018",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"\u00019",
		"\u0001:",
		"\u0001;",
		"",
		"\n5",
		"",
		"",
		"\n\u001e\a￿\u001a\u001e\u0004￿\u0001\u001e\u0001￿\u001a\u001e",
		"\n\u001e\a￿\u001a\u001e\u0004￿\u0001\u001e\u0001￿\u001a\u001e",
		"\u0001>",
		"\u0001?",
		"",
		"",
		"\n\u001e\a￿\u001a\u001e\u0004￿\u0001\u001e\u0001￿\u001a\u001e",
		"\u0001A",
		"",
		"\n\u001e\a￿\u001a\u001e\u0004￿\u0001\u001e\u0001￿\u001a\u001e",
		""
	};
	private static readonly short[] DFA14_eot = DFA.UnpackEncodedString("\u0003￿\u0001!\u0001\u001e\u0001$\u0001\u001e\u0001￿\u0001'\u0001)\u0001-\u00010\u0005￿\u0001\u001e\u0004￿\u0003\u001e\u00016\b￿\u00017\u0002￿\u0001\u001e\v￿\u0003\u001e\u0001￿\u0001\u001e\u0002￿\u0001<\u0001=\u0002\u001e\u0002￿\u0001@\u0001\u001e\u0001￿\u0001B\u0001￿");
	private static readonly short[] DFA14_eof = DFA.UnpackEncodedString("C￿");
	private static readonly char[] DFA14_min = DFA.UnpackEncodedStringToUnsignedChars("\u0001\t\u0002￿\u0001|\u0001r\u0001&\u0001n\u0001￿\u0002=\u0001<\u0001=\u0005￿\u0001o\u0004￿\u0001r\u0001a\u0001+\u0001.\b￿\u00010\u0002￿\u0001d\v￿\u0001t\u0001u\u0001l\u0001￿\u00010\u0002￿\u00020\u0001e\u0001s\u0002￿\u00010\u0001e\u0001￿\u00010\u0001￿");
	private static readonly char[] DFA14_max = DFA.UnpackEncodedStringToUnsignedChars("\u0001~\u0002￿\u0001|\u0001r\u0001&\u0001n\u0001￿\u0002=\u0002>\u0005￿\u0001o\u0004￿\u0001r\u0001a\u00019\u0001e\b￿\u0001z\u0002￿\u0001d\v￿\u0001t\u0001u\u0001l\u0001￿\u00019\u0002￿\u0002z\u0001e\u0001s\u0002￿\u0001z\u0001e\u0001￿\u0001z\u0001￿");
	private static readonly short[] DFA14_accept = DFA.UnpackEncodedString("\u0001￿\u0001\u0001\u0001\u0002\u0004￿\u0001\b\u0004￿\u0001\u0014\u0001\u0015\u0001\u0016\u0001\u0017\u0001\u0018\u0001￿\u0001\u001b\u0001\u001c\u0001\u001d\u0001\u001e\u0004￿\u0001#\u0001$\u0001%\u0001&\u0001!\u0001(\u0001\u0003\u0001\a\u0001￿\u0001\u0005\u0001\t\u0001￿\u0001\n\u0001\v\u0001\f\u0001\u0019\u0001\r\u0001\u000f\u0001\u0012\u0001\u000e\u0001\u0011\u0001\u0013\u0001\u0010\u0003￿\u0001'\u0001￿\u0001\"\u0001\u0004\u0004￿\u0001\u0006\u0001\u001a\u0002￿\u0001\u001f\u0001￿\u0001 ");
	private static readonly short[] DFA14_special = DFA.UnpackEncodedString("C￿}>");
	private static readonly short[][] DFA14_transition = DFA.UnpackEncodedStringArray(NCalcLexer.DFA14_transitionS);
	public override string GrammarFileName
	{
		get
		{
			return "C:\\Users\\s.ros\\Documents\\Développement\\NCalc\\Grammar\\NCalc.g";
		}
	}
	public NCalcLexer()
	{
		this.InitializeCyclicDFAs();
	}
	public NCalcLexer(ICharStream input) : this(input, null)
	{
	}
	public NCalcLexer(ICharStream input, RecognizerSharedState state) : base(input, state)
	{
		this.InitializeCyclicDFAs();
	}
	public void mT__19()
	{
		try
		{
			int type = 19;
			int channel = 0;
			this.Match(63);
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mT__20()
	{
		try
		{
			int type = 20;
			int channel = 0;
			this.Match(58);
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mT__21()
	{
		try
		{
			int type = 21;
			int channel = 0;
			this.Match("||");
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mT__22()
	{
		try
		{
			int type = 22;
			int channel = 0;
			this.Match("or");
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mT__23()
	{
		try
		{
			int type = 23;
			int channel = 0;
			this.Match("&&");
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mT__24()
	{
		try
		{
			int type = 24;
			int channel = 0;
			this.Match("and");
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mT__25()
	{
		try
		{
			int type = 25;
			int channel = 0;
			this.Match(124);
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mT__26()
	{
		try
		{
			int type = 26;
			int channel = 0;
			this.Match(94);
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mT__27()
	{
		try
		{
			int type = 27;
			int channel = 0;
			this.Match(38);
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mT__28()
	{
		try
		{
			int type = 28;
			int channel = 0;
			this.Match("==");
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mT__29()
	{
		try
		{
			int type = 29;
			int channel = 0;
			this.Match(61);
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mT__30()
	{
		try
		{
			int type = 30;
			int channel = 0;
			this.Match("!=");
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mT__31()
	{
		try
		{
			int type = 31;
			int channel = 0;
			this.Match("<>");
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mT__32()
	{
		try
		{
			int type = 32;
			int channel = 0;
			this.Match(60);
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mT__33()
	{
		try
		{
			int type = 33;
			int channel = 0;
			this.Match("<=");
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mT__34()
	{
		try
		{
			int type = 34;
			int channel = 0;
			this.Match(62);
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mT__35()
	{
		try
		{
			int type = 35;
			int channel = 0;
			this.Match(">=");
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mT__36()
	{
		try
		{
			int type = 36;
			int channel = 0;
			this.Match("<<");
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mT__37()
	{
		try
		{
			int type = 37;
			int channel = 0;
			this.Match(">>");
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mT__38()
	{
		try
		{
			int type = 38;
			int channel = 0;
			this.Match(43);
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mT__39()
	{
		try
		{
			int type = 39;
			int channel = 0;
			this.Match(45);
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mT__40()
	{
		try
		{
			int type = 40;
			int channel = 0;
			this.Match(42);
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mT__41()
	{
		try
		{
			int type = 41;
			int channel = 0;
			this.Match(47);
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mT__42()
	{
		try
		{
			int type = 42;
			int channel = 0;
			this.Match(37);
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mT__43()
	{
		try
		{
			int type = 43;
			int channel = 0;
			this.Match(33);
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mT__44()
	{
		try
		{
			int type = 44;
			int channel = 0;
			this.Match("not");
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mT__45()
	{
		try
		{
			int type = 45;
			int channel = 0;
			this.Match(126);
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mT__46()
	{
		try
		{
			int type = 46;
			int channel = 0;
			this.Match(40);
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mT__47()
	{
		try
		{
			int type = 47;
			int channel = 0;
			this.Match(41);
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mT__48()
	{
		try
		{
			int type = 48;
			int channel = 0;
			this.Match(44);
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mTRUE()
	{
		try
		{
			int type = 8;
			int channel = 0;
			this.Match("true");
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mFALSE()
	{
		try
		{
			int type = 9;
			int channel = 0;
			this.Match("false");
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mID()
	{
		try
		{
			int type = 10;
			int channel = 0;
			this.mLETTER();
			while (true)
			{
				int num = 2;
				int num2 = this.input.LA(1);
				if ((num2 >= 48 && num2 <= 57) || (num2 >= 65 && num2 <= 90) || num2 == 95 || (num2 >= 97 && num2 <= 122))
				{
					num = 1;
				}
				int num3 = num;
				if (num3 != 1)
				{
					break;
				}
				if ((this.input.LA(1) < 48 || this.input.LA(1) > 57) && (this.input.LA(1) < 65 || this.input.LA(1) > 90) && this.input.LA(1) != 95 && (this.input.LA(1) < 97 || this.input.LA(1) > 122))
				{
					goto IL_128;
				}
				this.input.Consume();
			}
			this.state.type = type;
			this.state.channel = channel;
			return;
			IL_128:
			MismatchedSetException ex = new MismatchedSetException(null, this.input);
			this.Recover(ex);
			throw ex;
		}
		finally
		{
		}
	}
	public void mINTEGER()
	{
		try
		{
			int type = 4;
			int channel = 0;
			int num = 0;
			while (true)
			{
				int num2 = 2;
				int num3 = this.input.LA(1);
				if (num3 >= 48 && num3 <= 57)
				{
					num2 = 1;
				}
				int num4 = num2;
				if (num4 != 1)
				{
					break;
				}
				this.mDIGIT();
				num++;
			}
			if (num < 1)
			{
				EarlyExitException ex = new EarlyExitException(2, this.input);
				throw ex;
			}
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mFLOAT()
	{
		try
		{
			int type = 5;
			int channel = 0;
			switch (this.dfa7.Predict(this.input))
			{
			case 1:
			{
				int num3;
				while (true)
				{
					int num = 2;
					int num2 = this.input.LA(1);
					if (num2 >= 48 && num2 <= 57)
					{
						num = 1;
					}
					num3 = num;
					if (num3 != 1)
					{
						break;
					}
					this.mDIGIT();
				}
				this.Match(46);
				int num4 = 0;
				while (true)
				{
					int num5 = 2;
					int num6 = this.input.LA(1);
					if (num6 >= 48 && num6 <= 57)
					{
						num5 = 1;
					}
					num3 = num5;
					if (num3 != 1)
					{
						break;
					}
					this.mDIGIT();
					num4++;
				}
				if (num4 < 1)
				{
					EarlyExitException ex = new EarlyExitException(4, this.input);
					throw ex;
				}
				int num7 = 2;
				int num8 = this.input.LA(1);
				if (num8 == 69 || num8 == 101)
				{
					num7 = 1;
				}
				num3 = num7;
				if (num3 == 1)
				{
					this.mE();
				}
				break;
			}
			case 2:
			{
				int num9 = 0;
				while (true)
				{
					int num10 = 2;
					int num11 = this.input.LA(1);
					if (num11 >= 48 && num11 <= 57)
					{
						num10 = 1;
					}
					int num3 = num10;
					if (num3 != 1)
					{
						break;
					}
					this.mDIGIT();
					num9++;
				}
				if (num9 < 1)
				{
					EarlyExitException ex2 = new EarlyExitException(6, this.input);
					throw ex2;
				}
				this.mE();
				break;
			}
			}
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mSTRING()
	{
		try
		{
			int type = 6;
			int channel = 0;
			this.Match(39);
			while (true)
			{
				int num = 3;
				int num2 = this.input.LA(1);
				if (num2 == 92)
				{
					num = 1;
				}
				else
				{
					if ((num2 >= 32 && num2 <= 38) || (num2 >= 40 && num2 <= 91) || (num2 >= 93 && num2 <= 65535))
					{
						num = 2;
					}
				}
				switch (num)
				{
				case 1:
					this.mEscapeSequence();
					continue;
				case 2:
					if ((this.input.LA(1) >= 32 && this.input.LA(1) <= 38) || (this.input.LA(1) >= 40 && this.input.LA(1) <= 91) || (this.input.LA(1) >= 93 && this.input.LA(1) <= 65535))
					{
						this.input.Consume();
						continue;
					}
					goto IL_146;
				}
				break;
			}
			this.Match(39);
			this.state.type = type;
			this.state.channel = channel;
			return;
			IL_146:
			MismatchedSetException ex = new MismatchedSetException(null, this.input);
			this.Recover(ex);
			throw ex;
		}
		finally
		{
		}
	}
	public void mDATETIME()
	{
		try
		{
			int type = 7;
			int channel = 0;
			this.Match(35);
			while (true)
			{
				int num = 2;
				int num2 = this.input.LA(1);
				if ((num2 >= 0 && num2 <= 34) || (num2 >= 36 && num2 <= 65535))
				{
					num = 1;
				}
				int num3 = num;
				if (num3 != 1)
				{
					break;
				}
				if ((this.input.LA(1) < 0 || this.input.LA(1) > 34) && (this.input.LA(1) < 36 || this.input.LA(1) > 65535))
				{
					goto IL_DD;
				}
				this.input.Consume();
			}
			this.Match(35);
			this.state.type = type;
			this.state.channel = channel;
			return;
			IL_DD:
			MismatchedSetException ex = new MismatchedSetException(null, this.input);
			this.Recover(ex);
			throw ex;
		}
		finally
		{
		}
	}
	public void mNAME()
	{
		try
		{
			int type = 11;
			int channel = 0;
			this.Match(91);
			while (true)
			{
				int num = 2;
				int num2 = this.input.LA(1);
				if ((num2 >= 0 && num2 <= 92) || (num2 >= 94 && num2 <= 65535))
				{
					num = 1;
				}
				int num3 = num;
				if (num3 != 1)
				{
					break;
				}
				if ((this.input.LA(1) < 0 || this.input.LA(1) > 92) && (this.input.LA(1) < 94 || this.input.LA(1) > 65535))
				{
					goto IL_DE;
				}
				this.input.Consume();
			}
			this.Match(93);
			this.state.type = type;
			this.state.channel = channel;
			return;
			IL_DE:
			MismatchedSetException ex = new MismatchedSetException(null, this.input);
			this.Recover(ex);
			throw ex;
		}
		finally
		{
		}
	}
	public void mE()
	{
		try
		{
			int type = 14;
			int channel = 0;
			if (this.input.LA(1) != 69 && this.input.LA(1) != 101)
			{
				MismatchedSetException ex = new MismatchedSetException(null, this.input);
				this.Recover(ex);
				throw ex;
			}
			this.input.Consume();
			int num = 2;
			int num2 = this.input.LA(1);
			if (num2 == 43 || num2 == 45)
			{
				num = 1;
			}
			int num3 = num;
			if (num3 == 1)
			{
				if (this.input.LA(1) != 43 && this.input.LA(1) != 45)
				{
					MismatchedSetException ex = new MismatchedSetException(null, this.input);
					this.Recover(ex);
					throw ex;
				}
				this.input.Consume();
			}
			int num4 = 0;
			while (true)
			{
				int num5 = 2;
				int num6 = this.input.LA(1);
				if (num6 >= 48 && num6 <= 57)
				{
					num5 = 1;
				}
				num3 = num5;
				if (num3 != 1)
				{
					break;
				}
				this.mDIGIT();
				num4++;
			}
			if (num4 < 1)
			{
				EarlyExitException ex2 = new EarlyExitException(12, this.input);
				throw ex2;
			}
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public void mLETTER()
	{
		try
		{
			if ((this.input.LA(1) < 65 || this.input.LA(1) > 90) && this.input.LA(1) != 95 && (this.input.LA(1) < 97 || this.input.LA(1) > 122))
			{
				MismatchedSetException ex = new MismatchedSetException(null, this.input);
				this.Recover(ex);
				throw ex;
			}
			this.input.Consume();
		}
		finally
		{
		}
	}
	public void mDIGIT()
	{
		try
		{
			this.MatchRange(48, 57);
		}
		finally
		{
		}
	}
	public void mEscapeSequence()
	{
		try
		{
			this.Match(92);
			int num = this.input.LA(1);
			int num2;
			if (num <= 92)
			{
				if (num == 39)
				{
					num2 = 4;
					goto IL_AA;
				}
				if (num == 92)
				{
					num2 = 5;
					goto IL_AA;
				}
			}
			else
			{
				if (num == 110)
				{
					num2 = 1;
					goto IL_AA;
				}
				switch (num)
				{
				case 114:
					num2 = 2;
					goto IL_AA;
				case 116:
					num2 = 3;
					goto IL_AA;
				case 117:
					num2 = 6;
					goto IL_AA;
				}
			}
			NoViableAltException ex = new NoViableAltException("", 13, 0, this.input);
			throw ex;
			IL_AA:
			switch (num2)
			{
			case 1:
				this.Match(110);
				break;
			case 2:
				this.Match(114);
				break;
			case 3:
				this.Match(116);
				break;
			case 4:
				this.Match(39);
				break;
			case 5:
				this.Match(92);
				break;
			case 6:
				this.mUnicodeEscape();
				break;
			}
		}
		finally
		{
		}
	}
	public void mHexDigit()
	{
		try
		{
			if ((this.input.LA(1) < 48 || this.input.LA(1) > 57) && (this.input.LA(1) < 65 || this.input.LA(1) > 70) && (this.input.LA(1) < 97 || this.input.LA(1) > 102))
			{
				MismatchedSetException ex = new MismatchedSetException(null, this.input);
				this.Recover(ex);
				throw ex;
			}
			this.input.Consume();
		}
		finally
		{
		}
	}
	public void mUnicodeEscape()
	{
		try
		{
			this.Match(117);
			this.mHexDigit();
			this.mHexDigit();
			this.mHexDigit();
			this.mHexDigit();
		}
		finally
		{
		}
	}
	public void mWS()
	{
		try
		{
			int type = 18;
			if ((this.input.LA(1) < 9 || this.input.LA(1) > 10) && (this.input.LA(1) < 12 || this.input.LA(1) > 13) && this.input.LA(1) != 32)
			{
				MismatchedSetException ex = new MismatchedSetException(null, this.input);
				this.Recover(ex);
				throw ex;
			}
			this.input.Consume();
			int channel = 99;
			this.state.type = type;
			this.state.channel = channel;
		}
		finally
		{
		}
	}
	public override void mTokens()
	{
		switch (this.dfa14.Predict(this.input))
		{
		case 1:
			this.mT__19();
			break;
		case 2:
			this.mT__20();
			break;
		case 3:
			this.mT__21();
			break;
		case 4:
			this.mT__22();
			break;
		case 5:
			this.mT__23();
			break;
		case 6:
			this.mT__24();
			break;
		case 7:
			this.mT__25();
			break;
		case 8:
			this.mT__26();
			break;
		case 9:
			this.mT__27();
			break;
		case 10:
			this.mT__28();
			break;
		case 11:
			this.mT__29();
			break;
		case 12:
			this.mT__30();
			break;
		case 13:
			this.mT__31();
			break;
		case 14:
			this.mT__32();
			break;
		case 15:
			this.mT__33();
			break;
		case 16:
			this.mT__34();
			break;
		case 17:
			this.mT__35();
			break;
		case 18:
			this.mT__36();
			break;
		case 19:
			this.mT__37();
			break;
		case 20:
			this.mT__38();
			break;
		case 21:
			this.mT__39();
			break;
		case 22:
			this.mT__40();
			break;
		case 23:
			this.mT__41();
			break;
		case 24:
			this.mT__42();
			break;
		case 25:
			this.mT__43();
			break;
		case 26:
			this.mT__44();
			break;
		case 27:
			this.mT__45();
			break;
		case 28:
			this.mT__46();
			break;
		case 29:
			this.mT__47();
			break;
		case 30:
			this.mT__48();
			break;
		case 31:
			this.mTRUE();
			break;
		case 32:
			this.mFALSE();
			break;
		case 33:
			this.mID();
			break;
		case 34:
			this.mINTEGER();
			break;
		case 35:
			this.mFLOAT();
			break;
		case 36:
			this.mSTRING();
			break;
		case 37:
			this.mDATETIME();
			break;
		case 38:
			this.mNAME();
			break;
		case 39:
			this.mE();
			break;
		case 40:
			this.mWS();
			break;
		}
	}
	private void InitializeCyclicDFAs()
	{
		this.dfa7 = new NCalcLexer.DFA7(this);
		this.dfa14 = new NCalcLexer.DFA14(this);
	}
}
