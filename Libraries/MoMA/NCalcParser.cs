using Antlr.Runtime;
using Antlr.Runtime.Tree;
using NCalc.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
public class NCalcParser : Parser
{
	public class ncalcExpression_return : ParserRuleReturnScope
	{
		public LogicalExpression value;
		private CommonTree tree;
		public override object Tree
		{
			get
			{
				return this.tree;
			}
			set
			{
				this.tree = (CommonTree)value;
			}
		}
	}
	public class logicalExpression_return : ParserRuleReturnScope
	{
		public LogicalExpression value;
		private CommonTree tree;
		public override object Tree
		{
			get
			{
				return this.tree;
			}
			set
			{
				this.tree = (CommonTree)value;
			}
		}
	}
	public class conditionalExpression_return : ParserRuleReturnScope
	{
		public LogicalExpression value;
		private CommonTree tree;
		public override object Tree
		{
			get
			{
				return this.tree;
			}
			set
			{
				this.tree = (CommonTree)value;
			}
		}
	}
	public class booleanAndExpression_return : ParserRuleReturnScope
	{
		public LogicalExpression value;
		private CommonTree tree;
		public override object Tree
		{
			get
			{
				return this.tree;
			}
			set
			{
				this.tree = (CommonTree)value;
			}
		}
	}
	public class bitwiseOrExpression_return : ParserRuleReturnScope
	{
		public LogicalExpression value;
		private CommonTree tree;
		public override object Tree
		{
			get
			{
				return this.tree;
			}
			set
			{
				this.tree = (CommonTree)value;
			}
		}
	}
	public class bitwiseXOrExpression_return : ParserRuleReturnScope
	{
		public LogicalExpression value;
		private CommonTree tree;
		public override object Tree
		{
			get
			{
				return this.tree;
			}
			set
			{
				this.tree = (CommonTree)value;
			}
		}
	}
	public class bitwiseAndExpression_return : ParserRuleReturnScope
	{
		public LogicalExpression value;
		private CommonTree tree;
		public override object Tree
		{
			get
			{
				return this.tree;
			}
			set
			{
				this.tree = (CommonTree)value;
			}
		}
	}
	public class equalityExpression_return : ParserRuleReturnScope
	{
		public LogicalExpression value;
		private CommonTree tree;
		public override object Tree
		{
			get
			{
				return this.tree;
			}
			set
			{
				this.tree = (CommonTree)value;
			}
		}
	}
	public class relationalExpression_return : ParserRuleReturnScope
	{
		public LogicalExpression value;
		private CommonTree tree;
		public override object Tree
		{
			get
			{
				return this.tree;
			}
			set
			{
				this.tree = (CommonTree)value;
			}
		}
	}
	public class shiftExpression_return : ParserRuleReturnScope
	{
		public LogicalExpression value;
		private CommonTree tree;
		public override object Tree
		{
			get
			{
				return this.tree;
			}
			set
			{
				this.tree = (CommonTree)value;
			}
		}
	}
	public class additiveExpression_return : ParserRuleReturnScope
	{
		public LogicalExpression value;
		private CommonTree tree;
		public override object Tree
		{
			get
			{
				return this.tree;
			}
			set
			{
				this.tree = (CommonTree)value;
			}
		}
	}
	public class multiplicativeExpression_return : ParserRuleReturnScope
	{
		public LogicalExpression value;
		private CommonTree tree;
		public override object Tree
		{
			get
			{
				return this.tree;
			}
			set
			{
				this.tree = (CommonTree)value;
			}
		}
	}
	public class unaryExpression_return : ParserRuleReturnScope
	{
		public LogicalExpression value;
		private CommonTree tree;
		public override object Tree
		{
			get
			{
				return this.tree;
			}
			set
			{
				this.tree = (CommonTree)value;
			}
		}
	}
	public class primaryExpression_return : ParserRuleReturnScope
	{
		public LogicalExpression value;
		private CommonTree tree;
		public override object Tree
		{
			get
			{
				return this.tree;
			}
			set
			{
				this.tree = (CommonTree)value;
			}
		}
	}
	public class value_return : ParserRuleReturnScope
	{
		public ValueExpression value;
		private CommonTree tree;
		public override object Tree
		{
			get
			{
				return this.tree;
			}
			set
			{
				this.tree = (CommonTree)value;
			}
		}
	}
	public class identifier_return : ParserRuleReturnScope
	{
		public Identifier value;
		private CommonTree tree;
		public override object Tree
		{
			get
			{
				return this.tree;
			}
			set
			{
				this.tree = (CommonTree)value;
			}
		}
	}
	public class expressionList_return : ParserRuleReturnScope
	{
		public List<LogicalExpression> value;
		private CommonTree tree;
		public override object Tree
		{
			get
			{
				return this.tree;
			}
			set
			{
				this.tree = (CommonTree)value;
			}
		}
	}
	public class arguments_return : ParserRuleReturnScope
	{
		public List<LogicalExpression> value;
		private CommonTree tree;
		public override object Tree
		{
			get
			{
				return this.tree;
			}
			set
			{
				this.tree = (CommonTree)value;
			}
		}
	}
	public const int T__29 = 29;
	public const int T__28 = 28;
	public const int T__27 = 27;
	public const int T__26 = 26;
	public const int T__25 = 25;
	public const int T__24 = 24;
	public const int T__23 = 23;
	public const int LETTER = 12;
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
	private const char BS = '\\';
	public static readonly string[] tokenNames = new string[]
	{
		"<invalid>",
		"<EOR>",
		"<DOWN>",
		"<UP>",
		"INTEGER",
		"FLOAT",
		"STRING",
		"DATETIME",
		"TRUE",
		"FALSE",
		"ID",
		"NAME",
		"LETTER",
		"DIGIT",
		"E",
		"EscapeSequence",
		"UnicodeEscape",
		"HexDigit",
		"WS",
		"'?'",
		"':'",
		"'||'",
		"'or'",
		"'&&'",
		"'and'",
		"'|'",
		"'^'",
		"'&'",
		"'=='",
		"'='",
		"'!='",
		"'<>'",
		"'<'",
		"'<='",
		"'>'",
		"'>='",
		"'<<'",
		"'>>'",
		"'+'",
		"'-'",
		"'*'",
		"'/'",
		"'%'",
		"'!'",
		"'not'",
		"'~'",
		"'('",
		"')'",
		"','"
	};
	protected ITreeAdaptor adaptor = new CommonTreeAdaptor();
	private static NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
	public static readonly BitSet FOLLOW_logicalExpression_in_ncalcExpression56;
	public static readonly BitSet FOLLOW_EOF_in_ncalcExpression58;
	public static readonly BitSet FOLLOW_conditionalExpression_in_logicalExpression78;
	public static readonly BitSet FOLLOW_19_in_logicalExpression84;
	public static readonly BitSet FOLLOW_conditionalExpression_in_logicalExpression88;
	public static readonly BitSet FOLLOW_20_in_logicalExpression90;
	public static readonly BitSet FOLLOW_conditionalExpression_in_logicalExpression94;
	public static readonly BitSet FOLLOW_booleanAndExpression_in_conditionalExpression121;
	public static readonly BitSet FOLLOW_set_in_conditionalExpression130;
	public static readonly BitSet FOLLOW_conditionalExpression_in_conditionalExpression146;
	public static readonly BitSet FOLLOW_bitwiseOrExpression_in_booleanAndExpression180;
	public static readonly BitSet FOLLOW_set_in_booleanAndExpression189;
	public static readonly BitSet FOLLOW_bitwiseOrExpression_in_booleanAndExpression205;
	public static readonly BitSet FOLLOW_bitwiseXOrExpression_in_bitwiseOrExpression237;
	public static readonly BitSet FOLLOW_25_in_bitwiseOrExpression246;
	public static readonly BitSet FOLLOW_bitwiseOrExpression_in_bitwiseOrExpression256;
	public static readonly BitSet FOLLOW_bitwiseAndExpression_in_bitwiseXOrExpression290;
	public static readonly BitSet FOLLOW_26_in_bitwiseXOrExpression299;
	public static readonly BitSet FOLLOW_bitwiseAndExpression_in_bitwiseXOrExpression309;
	public static readonly BitSet FOLLOW_equalityExpression_in_bitwiseAndExpression341;
	public static readonly BitSet FOLLOW_27_in_bitwiseAndExpression350;
	public static readonly BitSet FOLLOW_equalityExpression_in_bitwiseAndExpression360;
	public static readonly BitSet FOLLOW_relationalExpression_in_equalityExpression394;
	public static readonly BitSet FOLLOW_set_in_equalityExpression405;
	public static readonly BitSet FOLLOW_set_in_equalityExpression422;
	public static readonly BitSet FOLLOW_relationalExpression_in_equalityExpression441;
	public static readonly BitSet FOLLOW_shiftExpression_in_relationalExpression474;
	public static readonly BitSet FOLLOW_32_in_relationalExpression485;
	public static readonly BitSet FOLLOW_33_in_relationalExpression495;
	public static readonly BitSet FOLLOW_34_in_relationalExpression506;
	public static readonly BitSet FOLLOW_35_in_relationalExpression516;
	public static readonly BitSet FOLLOW_shiftExpression_in_relationalExpression528;
	public static readonly BitSet FOLLOW_additiveExpression_in_shiftExpression560;
	public static readonly BitSet FOLLOW_36_in_shiftExpression571;
	public static readonly BitSet FOLLOW_37_in_shiftExpression581;
	public static readonly BitSet FOLLOW_additiveExpression_in_shiftExpression593;
	public static readonly BitSet FOLLOW_multiplicativeExpression_in_additiveExpression625;
	public static readonly BitSet FOLLOW_38_in_additiveExpression636;
	public static readonly BitSet FOLLOW_39_in_additiveExpression646;
	public static readonly BitSet FOLLOW_multiplicativeExpression_in_additiveExpression658;
	public static readonly BitSet FOLLOW_unaryExpression_in_multiplicativeExpression690;
	public static readonly BitSet FOLLOW_40_in_multiplicativeExpression701;
	public static readonly BitSet FOLLOW_41_in_multiplicativeExpression711;
	public static readonly BitSet FOLLOW_42_in_multiplicativeExpression721;
	public static readonly BitSet FOLLOW_unaryExpression_in_multiplicativeExpression733;
	public static readonly BitSet FOLLOW_primaryExpression_in_unaryExpression760;
	public static readonly BitSet FOLLOW_set_in_unaryExpression771;
	public static readonly BitSet FOLLOW_primaryExpression_in_unaryExpression779;
	public static readonly BitSet FOLLOW_45_in_unaryExpression791;
	public static readonly BitSet FOLLOW_primaryExpression_in_unaryExpression794;
	public static readonly BitSet FOLLOW_39_in_unaryExpression805;
	public static readonly BitSet FOLLOW_primaryExpression_in_unaryExpression807;
	public static readonly BitSet FOLLOW_46_in_primaryExpression829;
	public static readonly BitSet FOLLOW_logicalExpression_in_primaryExpression831;
	public static readonly BitSet FOLLOW_47_in_primaryExpression833;
	public static readonly BitSet FOLLOW_value_in_primaryExpression843;
	public static readonly BitSet FOLLOW_identifier_in_primaryExpression851;
	public static readonly BitSet FOLLOW_arguments_in_primaryExpression856;
	public static readonly BitSet FOLLOW_INTEGER_in_value876;
	public static readonly BitSet FOLLOW_FLOAT_in_value884;
	public static readonly BitSet FOLLOW_STRING_in_value892;
	public static readonly BitSet FOLLOW_DATETIME_in_value901;
	public static readonly BitSet FOLLOW_TRUE_in_value908;
	public static readonly BitSet FOLLOW_FALSE_in_value916;
	public static readonly BitSet FOLLOW_ID_in_identifier934;
	public static readonly BitSet FOLLOW_NAME_in_identifier942;
	public static readonly BitSet FOLLOW_logicalExpression_in_expressionList966;
	public static readonly BitSet FOLLOW_48_in_expressionList973;
	public static readonly BitSet FOLLOW_logicalExpression_in_expressionList977;
	public static readonly BitSet FOLLOW_46_in_arguments1006;
	public static readonly BitSet FOLLOW_expressionList_in_arguments1010;
	public static readonly BitSet FOLLOW_47_in_arguments1017;
	public ITreeAdaptor TreeAdaptor
	{
		get
		{
			return this.adaptor;
		}
		set
		{
			this.adaptor = value;
		}
	}
	public override string[] TokenNames
	{
		get
		{
			return NCalcParser.tokenNames;
		}
	}
	public override string GrammarFileName
	{
		get
		{
			return "C:\\Users\\s.ros\\Documents\\Développement\\NCalc\\Grammar\\NCalc.g";
		}
	}
	public List<string> Errors
	{
		get;
		private set;
	}
	public NCalcParser(ITokenStream input) : this(input, new RecognizerSharedState())
	{
	}
	public NCalcParser(ITokenStream input, RecognizerSharedState state) : base(input, state)
	{
		this.InitializeCyclicDFAs();
	}
	private string extractString(string text)
	{
		StringBuilder stringBuilder = new StringBuilder(text);
		int startIndex = 1;
		int num;
		while ((num = stringBuilder.ToString().IndexOf('\\', startIndex)) != -1)
		{
			char c = stringBuilder[num + 1];
			char c2 = c;
			if (c2 <= '\\')
			{
				if (c2 != '\'')
				{
					if (c2 != '\\')
					{
						goto IL_163;
					}
					stringBuilder.Remove(num, 2).Insert(num, '\\');
				}
				else
				{
					stringBuilder.Remove(num, 2).Insert(num, '\'');
				}
			}
			else
			{
				if (c2 != 'n')
				{
					switch (c2)
					{
					case 'r':
						stringBuilder.Remove(num, 2).Insert(num, '\r');
						break;
					case 's':
						goto IL_163;
					case 't':
						stringBuilder.Remove(num, 2).Insert(num, '\t');
						break;
					case 'u':
					{
						string value = stringBuilder[num + 4] + stringBuilder[num + 5];
						string value2 = stringBuilder[num + 2] + stringBuilder[num + 3];
						char value3 = Encoding.Unicode.GetChars(new byte[]
						{
							Convert.ToByte(value, 16),
							Convert.ToByte(value2, 16)
						})[0];
						stringBuilder.Remove(num, 6).Insert(num, value3);
						break;
					}
					default:
						goto IL_163;
					}
				}
				else
				{
					stringBuilder.Remove(num, 2).Insert(num, '\n');
				}
			}
			startIndex = num + 1;
			continue;
			IL_163:
			throw new RecognitionException("Unvalid escape sequence: \\" + c);
		}
		stringBuilder.Remove(0, 1);
		stringBuilder.Remove(stringBuilder.Length - 1, 1);
		return stringBuilder.ToString();
	}
	public override void DisplayRecognitionError(string[] tokenNames, RecognitionException e)
	{
		base.DisplayRecognitionError(tokenNames, e);
		if (this.Errors == null)
		{
			this.Errors = new List<string>();
		}
		string errorHeader = this.GetErrorHeader(e);
		string errorMessage = this.GetErrorMessage(e, tokenNames);
		this.Errors.Add(errorMessage + " at " + errorHeader);
	}
	public NCalcParser.ncalcExpression_return ncalcExpression()
	{
		NCalcParser.ncalcExpression_return ncalcExpression_return = new NCalcParser.ncalcExpression_return();
		ncalcExpression_return.Start = this.input.LT(1);
		try
		{
			CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
			base.PushFollow(NCalcParser.FOLLOW_logicalExpression_in_ncalcExpression56);
			NCalcParser.logicalExpression_return logicalExpression_return = this.logicalExpression();
			this.state.followingStackPointer--;
			this.adaptor.AddChild(commonTree, logicalExpression_return.Tree);
			IToken token = (IToken)this.Match(this.input, -1, NCalcParser.FOLLOW_EOF_in_ncalcExpression58);
			ncalcExpression_return.value = ((logicalExpression_return != null) ? logicalExpression_return.value : null);
			ncalcExpression_return.Stop = this.input.LT(-1);
			ncalcExpression_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			this.adaptor.SetTokenBoundaries(ncalcExpression_return.Tree, (IToken)ncalcExpression_return.Start, (IToken)ncalcExpression_return.Stop);
		}
		catch (RecognitionException ex)
		{
			this.ReportError(ex);
			this.Recover(this.input, ex);
			ncalcExpression_return.Tree = (CommonTree)this.adaptor.ErrorNode(this.input, (IToken)ncalcExpression_return.Start, this.input.LT(-1), ex);
		}
		finally
		{
		}
		return ncalcExpression_return;
	}
	public NCalcParser.logicalExpression_return logicalExpression()
	{
		NCalcParser.logicalExpression_return logicalExpression_return = new NCalcParser.logicalExpression_return();
		logicalExpression_return.Start = this.input.LT(1);
		try
		{
			CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
			base.PushFollow(NCalcParser.FOLLOW_conditionalExpression_in_logicalExpression78);
			NCalcParser.conditionalExpression_return conditionalExpression_return = this.conditionalExpression();
			this.state.followingStackPointer--;
			this.adaptor.AddChild(commonTree, conditionalExpression_return.Tree);
			logicalExpression_return.value = ((conditionalExpression_return != null) ? conditionalExpression_return.value : null);
			int num = 2;
			int num2 = this.input.LA(1);
			if (num2 == 19)
			{
				num = 1;
			}
			int num3 = num;
			if (num3 == 1)
			{
				IToken payload = (IToken)this.Match(this.input, 19, NCalcParser.FOLLOW_19_in_logicalExpression84);
				CommonTree child = (CommonTree)this.adaptor.Create(payload);
				this.adaptor.AddChild(commonTree, child);
				base.PushFollow(NCalcParser.FOLLOW_conditionalExpression_in_logicalExpression88);
				NCalcParser.conditionalExpression_return conditionalExpression_return2 = this.conditionalExpression();
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree, conditionalExpression_return2.Tree);
				IToken payload2 = (IToken)this.Match(this.input, 20, NCalcParser.FOLLOW_20_in_logicalExpression90);
				CommonTree child2 = (CommonTree)this.adaptor.Create(payload2);
				this.adaptor.AddChild(commonTree, child2);
				base.PushFollow(NCalcParser.FOLLOW_conditionalExpression_in_logicalExpression94);
				NCalcParser.conditionalExpression_return conditionalExpression_return3 = this.conditionalExpression();
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree, conditionalExpression_return3.Tree);
				logicalExpression_return.value = new TernaryExpression((conditionalExpression_return != null) ? conditionalExpression_return.value : null, (conditionalExpression_return2 != null) ? conditionalExpression_return2.value : null, (conditionalExpression_return3 != null) ? conditionalExpression_return3.value : null);
			}
			logicalExpression_return.Stop = this.input.LT(-1);
			logicalExpression_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			this.adaptor.SetTokenBoundaries(logicalExpression_return.Tree, (IToken)logicalExpression_return.Start, (IToken)logicalExpression_return.Stop);
		}
		catch (RecognitionException ex)
		{
			this.ReportError(ex);
			this.Recover(this.input, ex);
			logicalExpression_return.Tree = (CommonTree)this.adaptor.ErrorNode(this.input, (IToken)logicalExpression_return.Start, this.input.LT(-1), ex);
		}
		finally
		{
		}
		return logicalExpression_return;
	}
	public NCalcParser.conditionalExpression_return conditionalExpression()
	{
		NCalcParser.conditionalExpression_return conditionalExpression_return = new NCalcParser.conditionalExpression_return();
		conditionalExpression_return.Start = this.input.LT(1);
		try
		{
			try
			{
				CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
				base.PushFollow(NCalcParser.FOLLOW_booleanAndExpression_in_conditionalExpression121);
				NCalcParser.booleanAndExpression_return booleanAndExpression_return = this.booleanAndExpression();
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree, booleanAndExpression_return.Tree);
				conditionalExpression_return.value = ((booleanAndExpression_return != null) ? booleanAndExpression_return.value : null);
				while (true)
				{
					int num = 2;
					int num2 = this.input.LA(1);
					if (num2 >= 21 && num2 <= 22)
					{
						num = 1;
					}
					int num3 = num;
					if (num3 != 1)
					{
						break;
					}
					IToken payload = this.input.LT(1);
					if (this.input.LA(1) < 21 || this.input.LA(1) > 22)
					{
						goto IL_151;
					}
					this.input.Consume();
					this.adaptor.AddChild(commonTree, (CommonTree)this.adaptor.Create(payload));
					this.state.errorRecovery = false;
					BinaryExpressionType type = BinaryExpressionType.Or;
					base.PushFollow(NCalcParser.FOLLOW_conditionalExpression_in_conditionalExpression146);
					NCalcParser.conditionalExpression_return conditionalExpression_return2 = this.conditionalExpression();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, conditionalExpression_return2.Tree);
					conditionalExpression_return.value = new BinaryExpression(type, conditionalExpression_return.value, (conditionalExpression_return2 != null) ? conditionalExpression_return2.value : null);
				}
				conditionalExpression_return.Stop = this.input.LT(-1);
				conditionalExpression_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
				this.adaptor.SetTokenBoundaries(conditionalExpression_return.Tree, (IToken)conditionalExpression_return.Start, (IToken)conditionalExpression_return.Stop);
				goto IL_28D;
				IL_151:
				MismatchedSetException ex = new MismatchedSetException(null, this.input);
				throw ex;
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
				conditionalExpression_return.Tree = (CommonTree)this.adaptor.ErrorNode(this.input, (IToken)conditionalExpression_return.Start, this.input.LT(-1), ex2);
			}
			IL_28D:;
		}
		finally
		{
		}
		return conditionalExpression_return;
	}
	public NCalcParser.booleanAndExpression_return booleanAndExpression()
	{
		NCalcParser.booleanAndExpression_return booleanAndExpression_return = new NCalcParser.booleanAndExpression_return();
		booleanAndExpression_return.Start = this.input.LT(1);
		try
		{
			try
			{
				CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
				base.PushFollow(NCalcParser.FOLLOW_bitwiseOrExpression_in_booleanAndExpression180);
				NCalcParser.bitwiseOrExpression_return bitwiseOrExpression_return = this.bitwiseOrExpression();
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree, bitwiseOrExpression_return.Tree);
				booleanAndExpression_return.value = ((bitwiseOrExpression_return != null) ? bitwiseOrExpression_return.value : null);
				while (true)
				{
					int num = 2;
					int num2 = this.input.LA(1);
					if (num2 >= 23 && num2 <= 24)
					{
						num = 1;
					}
					int num3 = num;
					if (num3 != 1)
					{
						break;
					}
					IToken payload = this.input.LT(1);
					if (this.input.LA(1) < 23 || this.input.LA(1) > 24)
					{
						goto IL_151;
					}
					this.input.Consume();
					this.adaptor.AddChild(commonTree, (CommonTree)this.adaptor.Create(payload));
					this.state.errorRecovery = false;
					BinaryExpressionType type = BinaryExpressionType.And;
					base.PushFollow(NCalcParser.FOLLOW_bitwiseOrExpression_in_booleanAndExpression205);
					NCalcParser.bitwiseOrExpression_return bitwiseOrExpression_return2 = this.bitwiseOrExpression();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, bitwiseOrExpression_return2.Tree);
					booleanAndExpression_return.value = new BinaryExpression(type, booleanAndExpression_return.value, (bitwiseOrExpression_return2 != null) ? bitwiseOrExpression_return2.value : null);
				}
				booleanAndExpression_return.Stop = this.input.LT(-1);
				booleanAndExpression_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
				this.adaptor.SetTokenBoundaries(booleanAndExpression_return.Tree, (IToken)booleanAndExpression_return.Start, (IToken)booleanAndExpression_return.Stop);
				goto IL_28D;
				IL_151:
				MismatchedSetException ex = new MismatchedSetException(null, this.input);
				throw ex;
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
				booleanAndExpression_return.Tree = (CommonTree)this.adaptor.ErrorNode(this.input, (IToken)booleanAndExpression_return.Start, this.input.LT(-1), ex2);
			}
			IL_28D:;
		}
		finally
		{
		}
		return booleanAndExpression_return;
	}
	public NCalcParser.bitwiseOrExpression_return bitwiseOrExpression()
	{
		NCalcParser.bitwiseOrExpression_return bitwiseOrExpression_return = new NCalcParser.bitwiseOrExpression_return();
		bitwiseOrExpression_return.Start = this.input.LT(1);
		try
		{
			CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
			base.PushFollow(NCalcParser.FOLLOW_bitwiseXOrExpression_in_bitwiseOrExpression237);
			NCalcParser.bitwiseXOrExpression_return bitwiseXOrExpression_return = this.bitwiseXOrExpression();
			this.state.followingStackPointer--;
			this.adaptor.AddChild(commonTree, bitwiseXOrExpression_return.Tree);
			bitwiseOrExpression_return.value = ((bitwiseXOrExpression_return != null) ? bitwiseXOrExpression_return.value : null);
			while (true)
			{
				int num = 2;
				int num2 = this.input.LA(1);
				if (num2 == 25)
				{
					num = 1;
				}
				int num3 = num;
				if (num3 != 1)
				{
					break;
				}
				IToken payload = (IToken)this.Match(this.input, 25, NCalcParser.FOLLOW_25_in_bitwiseOrExpression246);
				CommonTree child = (CommonTree)this.adaptor.Create(payload);
				this.adaptor.AddChild(commonTree, child);
				BinaryExpressionType type = BinaryExpressionType.BitwiseOr;
				base.PushFollow(NCalcParser.FOLLOW_bitwiseOrExpression_in_bitwiseOrExpression256);
				NCalcParser.bitwiseOrExpression_return bitwiseOrExpression_return2 = this.bitwiseOrExpression();
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree, bitwiseOrExpression_return2.Tree);
				bitwiseOrExpression_return.value = new BinaryExpression(type, bitwiseOrExpression_return.value, (bitwiseOrExpression_return2 != null) ? bitwiseOrExpression_return2.value : null);
			}
			bitwiseOrExpression_return.Stop = this.input.LT(-1);
			bitwiseOrExpression_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			this.adaptor.SetTokenBoundaries(bitwiseOrExpression_return.Tree, (IToken)bitwiseOrExpression_return.Start, (IToken)bitwiseOrExpression_return.Stop);
		}
		catch (RecognitionException ex)
		{
			this.ReportError(ex);
			this.Recover(this.input, ex);
			bitwiseOrExpression_return.Tree = (CommonTree)this.adaptor.ErrorNode(this.input, (IToken)bitwiseOrExpression_return.Start, this.input.LT(-1), ex);
		}
		finally
		{
		}
		return bitwiseOrExpression_return;
	}
	public NCalcParser.bitwiseXOrExpression_return bitwiseXOrExpression()
	{
		NCalcParser.bitwiseXOrExpression_return bitwiseXOrExpression_return = new NCalcParser.bitwiseXOrExpression_return();
		bitwiseXOrExpression_return.Start = this.input.LT(1);
		try
		{
			CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
			base.PushFollow(NCalcParser.FOLLOW_bitwiseAndExpression_in_bitwiseXOrExpression290);
			NCalcParser.bitwiseAndExpression_return bitwiseAndExpression_return = this.bitwiseAndExpression();
			this.state.followingStackPointer--;
			this.adaptor.AddChild(commonTree, bitwiseAndExpression_return.Tree);
			bitwiseXOrExpression_return.value = ((bitwiseAndExpression_return != null) ? bitwiseAndExpression_return.value : null);
			while (true)
			{
				int num = 2;
				int num2 = this.input.LA(1);
				if (num2 == 26)
				{
					num = 1;
				}
				int num3 = num;
				if (num3 != 1)
				{
					break;
				}
				IToken payload = (IToken)this.Match(this.input, 26, NCalcParser.FOLLOW_26_in_bitwiseXOrExpression299);
				CommonTree child = (CommonTree)this.adaptor.Create(payload);
				this.adaptor.AddChild(commonTree, child);
				BinaryExpressionType type = BinaryExpressionType.BitwiseXOr;
				base.PushFollow(NCalcParser.FOLLOW_bitwiseAndExpression_in_bitwiseXOrExpression309);
				NCalcParser.bitwiseAndExpression_return bitwiseAndExpression_return2 = this.bitwiseAndExpression();
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree, bitwiseAndExpression_return2.Tree);
				bitwiseXOrExpression_return.value = new BinaryExpression(type, bitwiseXOrExpression_return.value, (bitwiseAndExpression_return2 != null) ? bitwiseAndExpression_return2.value : null);
			}
			bitwiseXOrExpression_return.Stop = this.input.LT(-1);
			bitwiseXOrExpression_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			this.adaptor.SetTokenBoundaries(bitwiseXOrExpression_return.Tree, (IToken)bitwiseXOrExpression_return.Start, (IToken)bitwiseXOrExpression_return.Stop);
		}
		catch (RecognitionException ex)
		{
			this.ReportError(ex);
			this.Recover(this.input, ex);
			bitwiseXOrExpression_return.Tree = (CommonTree)this.adaptor.ErrorNode(this.input, (IToken)bitwiseXOrExpression_return.Start, this.input.LT(-1), ex);
		}
		finally
		{
		}
		return bitwiseXOrExpression_return;
	}
	public NCalcParser.bitwiseAndExpression_return bitwiseAndExpression()
	{
		NCalcParser.bitwiseAndExpression_return bitwiseAndExpression_return = new NCalcParser.bitwiseAndExpression_return();
		bitwiseAndExpression_return.Start = this.input.LT(1);
		try
		{
			CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
			base.PushFollow(NCalcParser.FOLLOW_equalityExpression_in_bitwiseAndExpression341);
			NCalcParser.equalityExpression_return equalityExpression_return = this.equalityExpression();
			this.state.followingStackPointer--;
			this.adaptor.AddChild(commonTree, equalityExpression_return.Tree);
			bitwiseAndExpression_return.value = ((equalityExpression_return != null) ? equalityExpression_return.value : null);
			while (true)
			{
				int num = 2;
				int num2 = this.input.LA(1);
				if (num2 == 27)
				{
					num = 1;
				}
				int num3 = num;
				if (num3 != 1)
				{
					break;
				}
				IToken payload = (IToken)this.Match(this.input, 27, NCalcParser.FOLLOW_27_in_bitwiseAndExpression350);
				CommonTree child = (CommonTree)this.adaptor.Create(payload);
				this.adaptor.AddChild(commonTree, child);
				BinaryExpressionType type = BinaryExpressionType.BitwiseAnd;
				base.PushFollow(NCalcParser.FOLLOW_equalityExpression_in_bitwiseAndExpression360);
				NCalcParser.equalityExpression_return equalityExpression_return2 = this.equalityExpression();
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree, equalityExpression_return2.Tree);
				bitwiseAndExpression_return.value = new BinaryExpression(type, bitwiseAndExpression_return.value, (equalityExpression_return2 != null) ? equalityExpression_return2.value : null);
			}
			bitwiseAndExpression_return.Stop = this.input.LT(-1);
			bitwiseAndExpression_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			this.adaptor.SetTokenBoundaries(bitwiseAndExpression_return.Tree, (IToken)bitwiseAndExpression_return.Start, (IToken)bitwiseAndExpression_return.Stop);
		}
		catch (RecognitionException ex)
		{
			this.ReportError(ex);
			this.Recover(this.input, ex);
			bitwiseAndExpression_return.Tree = (CommonTree)this.adaptor.ErrorNode(this.input, (IToken)bitwiseAndExpression_return.Start, this.input.LT(-1), ex);
		}
		finally
		{
		}
		return bitwiseAndExpression_return;
	}
	public NCalcParser.equalityExpression_return equalityExpression()
	{
		NCalcParser.equalityExpression_return equalityExpression_return = new NCalcParser.equalityExpression_return();
		equalityExpression_return.Start = this.input.LT(1);
		BinaryExpressionType type = BinaryExpressionType.Unknown;
		try
		{
			try
			{
				CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
				base.PushFollow(NCalcParser.FOLLOW_relationalExpression_in_equalityExpression394);
				NCalcParser.relationalExpression_return relationalExpression_return = this.relationalExpression();
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree, relationalExpression_return.Tree);
				equalityExpression_return.value = ((relationalExpression_return != null) ? relationalExpression_return.value : null);
				while (true)
				{
					int num = 2;
					int num2 = this.input.LA(1);
					if (num2 >= 28 && num2 <= 31)
					{
						num = 1;
					}
					int num3 = num;
					if (num3 != 1)
					{
						break;
					}
					int num4 = this.input.LA(1);
					int num5;
					if (num4 >= 28 && num4 <= 29)
					{
						num5 = 1;
					}
					else
					{
						if (num4 < 30 || num4 > 31)
						{
							goto IL_140;
						}
						num5 = 2;
					}
					switch (num5)
					{
					case 1:
					{
						IToken payload = this.input.LT(1);
						if (this.input.LA(1) < 28 || this.input.LA(1) > 29)
						{
							goto IL_1EF;
						}
						this.input.Consume();
						this.adaptor.AddChild(commonTree, (CommonTree)this.adaptor.Create(payload));
						this.state.errorRecovery = false;
						type = BinaryExpressionType.Equal;
						break;
					}
					case 2:
					{
						IToken payload2 = this.input.LT(1);
						if (this.input.LA(1) < 30 || this.input.LA(1) > 31)
						{
							goto IL_287;
						}
						this.input.Consume();
						this.adaptor.AddChild(commonTree, (CommonTree)this.adaptor.Create(payload2));
						this.state.errorRecovery = false;
						type = BinaryExpressionType.NotEqual;
						break;
					}
					}
					base.PushFollow(NCalcParser.FOLLOW_relationalExpression_in_equalityExpression441);
					NCalcParser.relationalExpression_return relationalExpression_return2 = this.relationalExpression();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, relationalExpression_return2.Tree);
					equalityExpression_return.value = new BinaryExpression(type, equalityExpression_return.value, (relationalExpression_return2 != null) ? relationalExpression_return2.value : null);
				}
				equalityExpression_return.Stop = this.input.LT(-1);
				equalityExpression_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
				this.adaptor.SetTokenBoundaries(equalityExpression_return.Tree, (IToken)equalityExpression_return.Start, (IToken)equalityExpression_return.Stop);
				goto IL_3C9;
				IL_140:
				NoViableAltException ex = new NoViableAltException("", 7, 0, this.input);
				throw ex;
				IL_1EF:
				MismatchedSetException ex2 = new MismatchedSetException(null, this.input);
				throw ex2;
				IL_287:
				ex2 = new MismatchedSetException(null, this.input);
				throw ex2;
			}
			catch (RecognitionException ex3)
			{
				this.ReportError(ex3);
				this.Recover(this.input, ex3);
				equalityExpression_return.Tree = (CommonTree)this.adaptor.ErrorNode(this.input, (IToken)equalityExpression_return.Start, this.input.LT(-1), ex3);
			}
			IL_3C9:;
		}
		finally
		{
		}
		return equalityExpression_return;
	}
	public NCalcParser.relationalExpression_return relationalExpression()
	{
		NCalcParser.relationalExpression_return relationalExpression_return = new NCalcParser.relationalExpression_return();
		relationalExpression_return.Start = this.input.LT(1);
		BinaryExpressionType type = BinaryExpressionType.Unknown;
		try
		{
			try
			{
				CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
				base.PushFollow(NCalcParser.FOLLOW_shiftExpression_in_relationalExpression474);
				NCalcParser.shiftExpression_return shiftExpression_return = this.shiftExpression();
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree, shiftExpression_return.Tree);
				relationalExpression_return.value = ((shiftExpression_return != null) ? shiftExpression_return.value : null);
				while (true)
				{
					int num = 2;
					int num2 = this.input.LA(1);
					if (num2 >= 32 && num2 <= 35)
					{
						num = 1;
					}
					int num3 = num;
					if (num3 != 1)
					{
						break;
					}
					int num4;
					switch (this.input.LA(1))
					{
					case 32:
						num4 = 1;
						goto IL_15B;
					case 33:
						num4 = 2;
						goto IL_15B;
					case 34:
						num4 = 3;
						goto IL_15B;
					case 35:
						num4 = 4;
						goto IL_15B;
					}
					goto Block_8;
					IL_15B:
					switch (num4)
					{
					case 1:
					{
						IToken payload = (IToken)this.Match(this.input, 32, NCalcParser.FOLLOW_32_in_relationalExpression485);
						CommonTree child = (CommonTree)this.adaptor.Create(payload);
						this.adaptor.AddChild(commonTree, child);
						type = BinaryExpressionType.Lesser;
						break;
					}
					case 2:
					{
						IToken payload2 = (IToken)this.Match(this.input, 33, NCalcParser.FOLLOW_33_in_relationalExpression495);
						CommonTree child2 = (CommonTree)this.adaptor.Create(payload2);
						this.adaptor.AddChild(commonTree, child2);
						type = BinaryExpressionType.LesserOrEqual;
						break;
					}
					case 3:
					{
						IToken payload3 = (IToken)this.Match(this.input, 34, NCalcParser.FOLLOW_34_in_relationalExpression506);
						CommonTree child3 = (CommonTree)this.adaptor.Create(payload3);
						this.adaptor.AddChild(commonTree, child3);
						type = BinaryExpressionType.Greater;
						break;
					}
					case 4:
					{
						IToken payload4 = (IToken)this.Match(this.input, 35, NCalcParser.FOLLOW_35_in_relationalExpression516);
						CommonTree child4 = (CommonTree)this.adaptor.Create(payload4);
						this.adaptor.AddChild(commonTree, child4);
						type = BinaryExpressionType.GreaterOrEqual;
						break;
					}
					}
					base.PushFollow(NCalcParser.FOLLOW_shiftExpression_in_relationalExpression528);
					NCalcParser.shiftExpression_return shiftExpression_return2 = this.shiftExpression();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, shiftExpression_return2.Tree);
					relationalExpression_return.value = new BinaryExpression(type, relationalExpression_return.value, (shiftExpression_return2 != null) ? shiftExpression_return2.value : null);
				}
				relationalExpression_return.Stop = this.input.LT(-1);
				relationalExpression_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
				this.adaptor.SetTokenBoundaries(relationalExpression_return.Tree, (IToken)relationalExpression_return.Start, (IToken)relationalExpression_return.Stop);
				goto IL_3BC;
				Block_8:
				NoViableAltException ex = new NoViableAltException("", 9, 0, this.input);
				throw ex;
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
				relationalExpression_return.Tree = (CommonTree)this.adaptor.ErrorNode(this.input, (IToken)relationalExpression_return.Start, this.input.LT(-1), ex2);
			}
			IL_3BC:;
		}
		finally
		{
		}
		return relationalExpression_return;
	}
	public NCalcParser.shiftExpression_return shiftExpression()
	{
		NCalcParser.shiftExpression_return shiftExpression_return = new NCalcParser.shiftExpression_return();
		shiftExpression_return.Start = this.input.LT(1);
		BinaryExpressionType type = BinaryExpressionType.Unknown;
		try
		{
			try
			{
				CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
				base.PushFollow(NCalcParser.FOLLOW_additiveExpression_in_shiftExpression560);
				NCalcParser.additiveExpression_return additiveExpression_return = this.additiveExpression();
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree, additiveExpression_return.Tree);
				shiftExpression_return.value = ((additiveExpression_return != null) ? additiveExpression_return.value : null);
				while (true)
				{
					int num = 2;
					int num2 = this.input.LA(1);
					if (num2 >= 36 && num2 <= 37)
					{
						num = 1;
					}
					int num3 = num;
					if (num3 != 1)
					{
						break;
					}
					int num4 = this.input.LA(1);
					int num5;
					if (num4 == 36)
					{
						num5 = 1;
					}
					else
					{
						if (num4 != 37)
						{
							goto IL_128;
						}
						num5 = 2;
					}
					switch (num5)
					{
					case 1:
					{
						IToken payload = (IToken)this.Match(this.input, 36, NCalcParser.FOLLOW_36_in_shiftExpression571);
						CommonTree child = (CommonTree)this.adaptor.Create(payload);
						this.adaptor.AddChild(commonTree, child);
						type = BinaryExpressionType.LeftShift;
						break;
					}
					case 2:
					{
						IToken payload2 = (IToken)this.Match(this.input, 37, NCalcParser.FOLLOW_37_in_shiftExpression581);
						CommonTree child2 = (CommonTree)this.adaptor.Create(payload2);
						this.adaptor.AddChild(commonTree, child2);
						type = BinaryExpressionType.RightShift;
						break;
					}
					}
					base.PushFollow(NCalcParser.FOLLOW_additiveExpression_in_shiftExpression593);
					NCalcParser.additiveExpression_return additiveExpression_return2 = this.additiveExpression();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, additiveExpression_return2.Tree);
					shiftExpression_return.value = new BinaryExpression(type, shiftExpression_return.value, (additiveExpression_return2 != null) ? additiveExpression_return2.value : null);
				}
				shiftExpression_return.Stop = this.input.LT(-1);
				shiftExpression_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
				this.adaptor.SetTokenBoundaries(shiftExpression_return.Tree, (IToken)shiftExpression_return.Start, (IToken)shiftExpression_return.Stop);
				goto IL_30E;
				IL_128:
				NoViableAltException ex = new NoViableAltException("", 11, 0, this.input);
				throw ex;
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
				shiftExpression_return.Tree = (CommonTree)this.adaptor.ErrorNode(this.input, (IToken)shiftExpression_return.Start, this.input.LT(-1), ex2);
			}
			IL_30E:;
		}
		finally
		{
		}
		return shiftExpression_return;
	}
	public NCalcParser.additiveExpression_return additiveExpression()
	{
		NCalcParser.additiveExpression_return additiveExpression_return = new NCalcParser.additiveExpression_return();
		additiveExpression_return.Start = this.input.LT(1);
		BinaryExpressionType type = BinaryExpressionType.Unknown;
		try
		{
			try
			{
				CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
				base.PushFollow(NCalcParser.FOLLOW_multiplicativeExpression_in_additiveExpression625);
				NCalcParser.multiplicativeExpression_return multiplicativeExpression_return = this.multiplicativeExpression();
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree, multiplicativeExpression_return.Tree);
				additiveExpression_return.value = ((multiplicativeExpression_return != null) ? multiplicativeExpression_return.value : null);
				while (true)
				{
					int num = 2;
					int num2 = this.input.LA(1);
					if (num2 >= 38 && num2 <= 39)
					{
						num = 1;
					}
					int num3 = num;
					if (num3 != 1)
					{
						break;
					}
					int num4 = this.input.LA(1);
					int num5;
					if (num4 == 38)
					{
						num5 = 1;
					}
					else
					{
						if (num4 != 39)
						{
							goto IL_128;
						}
						num5 = 2;
					}
					switch (num5)
					{
					case 1:
					{
						IToken payload = (IToken)this.Match(this.input, 38, NCalcParser.FOLLOW_38_in_additiveExpression636);
						CommonTree child = (CommonTree)this.adaptor.Create(payload);
						this.adaptor.AddChild(commonTree, child);
						type = BinaryExpressionType.Plus;
						break;
					}
					case 2:
					{
						IToken payload2 = (IToken)this.Match(this.input, 39, NCalcParser.FOLLOW_39_in_additiveExpression646);
						CommonTree child2 = (CommonTree)this.adaptor.Create(payload2);
						this.adaptor.AddChild(commonTree, child2);
						type = BinaryExpressionType.Minus;
						break;
					}
					}
					base.PushFollow(NCalcParser.FOLLOW_multiplicativeExpression_in_additiveExpression658);
					NCalcParser.multiplicativeExpression_return multiplicativeExpression_return2 = this.multiplicativeExpression();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, multiplicativeExpression_return2.Tree);
					additiveExpression_return.value = new BinaryExpression(type, additiveExpression_return.value, (multiplicativeExpression_return2 != null) ? multiplicativeExpression_return2.value : null);
				}
				additiveExpression_return.Stop = this.input.LT(-1);
				additiveExpression_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
				this.adaptor.SetTokenBoundaries(additiveExpression_return.Tree, (IToken)additiveExpression_return.Start, (IToken)additiveExpression_return.Stop);
				goto IL_30D;
				IL_128:
				NoViableAltException ex = new NoViableAltException("", 13, 0, this.input);
				throw ex;
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
				additiveExpression_return.Tree = (CommonTree)this.adaptor.ErrorNode(this.input, (IToken)additiveExpression_return.Start, this.input.LT(-1), ex2);
			}
			IL_30D:;
		}
		finally
		{
		}
		return additiveExpression_return;
	}
	public NCalcParser.multiplicativeExpression_return multiplicativeExpression()
	{
		NCalcParser.multiplicativeExpression_return multiplicativeExpression_return = new NCalcParser.multiplicativeExpression_return();
		multiplicativeExpression_return.Start = this.input.LT(1);
		BinaryExpressionType type = BinaryExpressionType.Unknown;
		try
		{
			try
			{
				CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
				base.PushFollow(NCalcParser.FOLLOW_unaryExpression_in_multiplicativeExpression690);
				NCalcParser.unaryExpression_return unaryExpression_return = this.unaryExpression();
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree, unaryExpression_return.Tree);
				multiplicativeExpression_return.value = ((unaryExpression_return != null) ? unaryExpression_return.value : null);
				while (true)
				{
					int num = 2;
					int num2 = this.input.LA(1);
					if (num2 >= 40 && num2 <= 42)
					{
						num = 1;
					}
					int num3 = num;
					if (num3 != 1)
					{
						break;
					}
					int num4;
					switch (this.input.LA(1))
					{
					case 40:
						num4 = 1;
						goto IL_147;
					case 41:
						num4 = 2;
						goto IL_147;
					case 42:
						num4 = 3;
						goto IL_147;
					}
					goto Block_8;
					IL_147:
					switch (num4)
					{
					case 1:
					{
						IToken payload = (IToken)this.Match(this.input, 40, NCalcParser.FOLLOW_40_in_multiplicativeExpression701);
						CommonTree child = (CommonTree)this.adaptor.Create(payload);
						this.adaptor.AddChild(commonTree, child);
						type = BinaryExpressionType.Times;
						break;
					}
					case 2:
					{
						IToken payload2 = (IToken)this.Match(this.input, 41, NCalcParser.FOLLOW_41_in_multiplicativeExpression711);
						CommonTree child2 = (CommonTree)this.adaptor.Create(payload2);
						this.adaptor.AddChild(commonTree, child2);
						type = BinaryExpressionType.Div;
						break;
					}
					case 3:
					{
						IToken payload3 = (IToken)this.Match(this.input, 42, NCalcParser.FOLLOW_42_in_multiplicativeExpression721);
						CommonTree child3 = (CommonTree)this.adaptor.Create(payload3);
						this.adaptor.AddChild(commonTree, child3);
						type = BinaryExpressionType.Modulo;
						break;
					}
					}
					base.PushFollow(NCalcParser.FOLLOW_unaryExpression_in_multiplicativeExpression733);
					NCalcParser.unaryExpression_return unaryExpression_return2 = this.unaryExpression();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, unaryExpression_return2.Tree);
					multiplicativeExpression_return.value = new BinaryExpression(type, multiplicativeExpression_return.value, (unaryExpression_return2 != null) ? unaryExpression_return2.value : null);
				}
				multiplicativeExpression_return.Stop = this.input.LT(-1);
				multiplicativeExpression_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
				this.adaptor.SetTokenBoundaries(multiplicativeExpression_return.Tree, (IToken)multiplicativeExpression_return.Start, (IToken)multiplicativeExpression_return.Stop);
				goto IL_360;
				Block_8:
				NoViableAltException ex = new NoViableAltException("", 15, 0, this.input);
				throw ex;
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
				multiplicativeExpression_return.Tree = (CommonTree)this.adaptor.ErrorNode(this.input, (IToken)multiplicativeExpression_return.Start, this.input.LT(-1), ex2);
			}
			IL_360:;
		}
		finally
		{
		}
		return multiplicativeExpression_return;
	}
	public NCalcParser.unaryExpression_return unaryExpression()
	{
		NCalcParser.unaryExpression_return unaryExpression_return = new NCalcParser.unaryExpression_return();
		unaryExpression_return.Start = this.input.LT(1);
		CommonTree commonTree = null;
		try
		{
			int num = this.input.LA(1);
			int num2;
			switch (num)
			{
			case 4:
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
				break;
			default:
			{
				switch (num)
				{
				case 39:
					num2 = 4;
					goto IL_E2;
				case 43:
				case 44:
					num2 = 2;
					goto IL_E2;
				case 45:
					num2 = 3;
					goto IL_E2;
				case 46:
					goto IL_A2;
				}
				NoViableAltException ex = new NoViableAltException("", 17, 0, this.input);
				throw ex;
			}
			}
			IL_A2:
			num2 = 1;
			IL_E2:
			switch (num2)
			{
			case 1:
			{
				commonTree = (CommonTree)this.adaptor.GetNilNode();
				base.PushFollow(NCalcParser.FOLLOW_primaryExpression_in_unaryExpression760);
				NCalcParser.primaryExpression_return primaryExpression_return = this.primaryExpression();
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree, primaryExpression_return.Tree);
				unaryExpression_return.value = ((primaryExpression_return != null) ? primaryExpression_return.value : null);
				break;
			}
			case 2:
			{
				commonTree = (CommonTree)this.adaptor.GetNilNode();
				IToken payload = this.input.LT(1);
				if (this.input.LA(1) < 43 || this.input.LA(1) > 44)
				{
					MismatchedSetException ex2 = new MismatchedSetException(null, this.input);
					throw ex2;
				}
				this.input.Consume();
				this.adaptor.AddChild(commonTree, (CommonTree)this.adaptor.Create(payload));
				this.state.errorRecovery = false;
				base.PushFollow(NCalcParser.FOLLOW_primaryExpression_in_unaryExpression779);
				NCalcParser.primaryExpression_return primaryExpression_return2 = this.primaryExpression();
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree, primaryExpression_return2.Tree);
				unaryExpression_return.value = new UnaryExpression(UnaryExpressionType.Not, (primaryExpression_return2 != null) ? primaryExpression_return2.value : null);
				break;
			}
			case 3:
			{
				commonTree = (CommonTree)this.adaptor.GetNilNode();
				IToken payload2 = (IToken)this.Match(this.input, 45, NCalcParser.FOLLOW_45_in_unaryExpression791);
				CommonTree child = (CommonTree)this.adaptor.Create(payload2);
				this.adaptor.AddChild(commonTree, child);
				base.PushFollow(NCalcParser.FOLLOW_primaryExpression_in_unaryExpression794);
				NCalcParser.primaryExpression_return primaryExpression_return3 = this.primaryExpression();
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree, primaryExpression_return3.Tree);
				unaryExpression_return.value = new UnaryExpression(UnaryExpressionType.BitwiseNot, (primaryExpression_return3 != null) ? primaryExpression_return3.value : null);
				break;
			}
			case 4:
			{
				commonTree = (CommonTree)this.adaptor.GetNilNode();
				IToken payload3 = (IToken)this.Match(this.input, 39, NCalcParser.FOLLOW_39_in_unaryExpression805);
				CommonTree child2 = (CommonTree)this.adaptor.Create(payload3);
				this.adaptor.AddChild(commonTree, child2);
				base.PushFollow(NCalcParser.FOLLOW_primaryExpression_in_unaryExpression807);
				NCalcParser.primaryExpression_return primaryExpression_return4 = this.primaryExpression();
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree, primaryExpression_return4.Tree);
				unaryExpression_return.value = new UnaryExpression(UnaryExpressionType.Negate, (primaryExpression_return4 != null) ? primaryExpression_return4.value : null);
				break;
			}
			}
			unaryExpression_return.Stop = this.input.LT(-1);
			unaryExpression_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			this.adaptor.SetTokenBoundaries(unaryExpression_return.Tree, (IToken)unaryExpression_return.Start, (IToken)unaryExpression_return.Stop);
		}
		catch (RecognitionException ex3)
		{
			this.ReportError(ex3);
			this.Recover(this.input, ex3);
			unaryExpression_return.Tree = (CommonTree)this.adaptor.ErrorNode(this.input, (IToken)unaryExpression_return.Start, this.input.LT(-1), ex3);
		}
		finally
		{
		}
		return unaryExpression_return;
	}
	public NCalcParser.primaryExpression_return primaryExpression()
	{
		NCalcParser.primaryExpression_return primaryExpression_return = new NCalcParser.primaryExpression_return();
		primaryExpression_return.Start = this.input.LT(1);
		CommonTree commonTree = null;
		try
		{
			int num = this.input.LA(1);
			int num2;
			switch (num)
			{
			case 4:
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
				num2 = 2;
				break;
			case 10:
			case 11:
				num2 = 3;
				break;
			default:
				if (num != 46)
				{
					NoViableAltException ex = new NoViableAltException("", 19, 0, this.input);
					throw ex;
				}
				num2 = 1;
				break;
			}
			switch (num2)
			{
			case 1:
			{
				commonTree = (CommonTree)this.adaptor.GetNilNode();
				IToken payload = (IToken)this.Match(this.input, 46, NCalcParser.FOLLOW_46_in_primaryExpression829);
				CommonTree child = (CommonTree)this.adaptor.Create(payload);
				this.adaptor.AddChild(commonTree, child);
				base.PushFollow(NCalcParser.FOLLOW_logicalExpression_in_primaryExpression831);
				NCalcParser.logicalExpression_return logicalExpression_return = this.logicalExpression();
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree, logicalExpression_return.Tree);
				IToken payload2 = (IToken)this.Match(this.input, 47, NCalcParser.FOLLOW_47_in_primaryExpression833);
				CommonTree child2 = (CommonTree)this.adaptor.Create(payload2);
				this.adaptor.AddChild(commonTree, child2);
				primaryExpression_return.value = ((logicalExpression_return != null) ? logicalExpression_return.value : null);
				break;
			}
			case 2:
			{
				commonTree = (CommonTree)this.adaptor.GetNilNode();
				base.PushFollow(NCalcParser.FOLLOW_value_in_primaryExpression843);
				NCalcParser.value_return value_return = this.value();
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree, value_return.Tree);
				primaryExpression_return.value = ((value_return != null) ? value_return.value : null);
				break;
			}
			case 3:
			{
				commonTree = (CommonTree)this.adaptor.GetNilNode();
				base.PushFollow(NCalcParser.FOLLOW_identifier_in_primaryExpression851);
				NCalcParser.identifier_return identifier_return = this.identifier();
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree, identifier_return.Tree);
				primaryExpression_return.value = ((identifier_return != null) ? identifier_return.value : null);
				int num3 = 2;
				int num4 = this.input.LA(1);
				if (num4 == 46)
				{
					num3 = 1;
				}
				num = num3;
				if (num == 1)
				{
					base.PushFollow(NCalcParser.FOLLOW_arguments_in_primaryExpression856);
					NCalcParser.arguments_return arguments_return = this.arguments();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, arguments_return.Tree);
					primaryExpression_return.value = new Function((identifier_return != null) ? identifier_return.value : null, ((arguments_return != null) ? arguments_return.value : null).ToArray());
				}
				break;
			}
			}
			primaryExpression_return.Stop = this.input.LT(-1);
			primaryExpression_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			this.adaptor.SetTokenBoundaries(primaryExpression_return.Tree, (IToken)primaryExpression_return.Start, (IToken)primaryExpression_return.Stop);
		}
		catch (RecognitionException ex2)
		{
			this.ReportError(ex2);
			this.Recover(this.input, ex2);
			primaryExpression_return.Tree = (CommonTree)this.adaptor.ErrorNode(this.input, (IToken)primaryExpression_return.Start, this.input.LT(-1), ex2);
		}
		finally
		{
		}
		return primaryExpression_return;
	}
	public NCalcParser.value_return value()
	{
		NCalcParser.value_return value_return = new NCalcParser.value_return();
		value_return.Start = this.input.LT(1);
		CommonTree commonTree = null;
		IToken token = null;
		try
		{
			int num;
			switch (this.input.LA(1))
			{
			case 4:
				num = 1;
				break;
			case 5:
				num = 2;
				break;
			case 6:
				num = 3;
				break;
			case 7:
				num = 4;
				break;
			case 8:
				num = 5;
				break;
			case 9:
				num = 6;
				break;
			default:
			{
				NoViableAltException ex = new NoViableAltException("", 20, 0, this.input);
				throw ex;
			}
			}
			switch (num)
			{
			case 1:
			{
				commonTree = (CommonTree)this.adaptor.GetNilNode();
				token = (IToken)this.Match(this.input, 4, NCalcParser.FOLLOW_INTEGER_in_value876);
				CommonTree child = (CommonTree)this.adaptor.Create(token);
				this.adaptor.AddChild(commonTree, child);
				try
				{
					value_return.value = new ValueExpression(int.Parse((token != null) ? token.Text : null));
				}
				catch (OverflowException)
				{
					value_return.value = new ValueExpression((float)long.Parse((token != null) ? token.Text : null));
				}
				break;
			}
			case 2:
			{
				commonTree = (CommonTree)this.adaptor.GetNilNode();
				IToken token2 = (IToken)this.Match(this.input, 5, NCalcParser.FOLLOW_FLOAT_in_value884);
				CommonTree child2 = (CommonTree)this.adaptor.Create(token2);
				this.adaptor.AddChild(commonTree, child2);
				value_return.value = new ValueExpression(double.Parse((token2 != null) ? token2.Text : null, NumberStyles.Float, NCalcParser.numberFormatInfo));
				break;
			}
			case 3:
			{
				commonTree = (CommonTree)this.adaptor.GetNilNode();
				IToken token3 = (IToken)this.Match(this.input, 6, NCalcParser.FOLLOW_STRING_in_value892);
				CommonTree child3 = (CommonTree)this.adaptor.Create(token3);
				this.adaptor.AddChild(commonTree, child3);
				value_return.value = new ValueExpression(this.extractString((token3 != null) ? token3.Text : null));
				break;
			}
			case 4:
			{
				commonTree = (CommonTree)this.adaptor.GetNilNode();
				IToken token4 = (IToken)this.Match(this.input, 7, NCalcParser.FOLLOW_DATETIME_in_value901);
				CommonTree child4 = (CommonTree)this.adaptor.Create(token4);
				this.adaptor.AddChild(commonTree, child4);
				value_return.value = new ValueExpression(DateTime.Parse(((token4 != null) ? token4.Text : null).Substring(1, ((token4 != null) ? token4.Text : null).Length - 2)));
				break;
			}
			case 5:
			{
				commonTree = (CommonTree)this.adaptor.GetNilNode();
				IToken payload = (IToken)this.Match(this.input, 8, NCalcParser.FOLLOW_TRUE_in_value908);
				CommonTree child5 = (CommonTree)this.adaptor.Create(payload);
				this.adaptor.AddChild(commonTree, child5);
				value_return.value = new ValueExpression(true);
				break;
			}
			case 6:
			{
				commonTree = (CommonTree)this.adaptor.GetNilNode();
				IToken payload2 = (IToken)this.Match(this.input, 9, NCalcParser.FOLLOW_FALSE_in_value916);
				CommonTree child6 = (CommonTree)this.adaptor.Create(payload2);
				this.adaptor.AddChild(commonTree, child6);
				value_return.value = new ValueExpression(false);
				break;
			}
			}
			value_return.Stop = this.input.LT(-1);
			value_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			this.adaptor.SetTokenBoundaries(value_return.Tree, (IToken)value_return.Start, (IToken)value_return.Stop);
		}
		catch (RecognitionException ex2)
		{
			this.ReportError(ex2);
			this.Recover(this.input, ex2);
			value_return.Tree = (CommonTree)this.adaptor.ErrorNode(this.input, (IToken)value_return.Start, this.input.LT(-1), ex2);
		}
		finally
		{
		}
		return value_return;
	}
	public NCalcParser.identifier_return identifier()
	{
		NCalcParser.identifier_return identifier_return = new NCalcParser.identifier_return();
		identifier_return.Start = this.input.LT(1);
		CommonTree commonTree = null;
		try
		{
			int num = this.input.LA(1);
			int num2;
			if (num == 10)
			{
				num2 = 1;
			}
			else
			{
				if (num != 11)
				{
					NoViableAltException ex = new NoViableAltException("", 21, 0, this.input);
					throw ex;
				}
				num2 = 2;
			}
			switch (num2)
			{
			case 1:
			{
				commonTree = (CommonTree)this.adaptor.GetNilNode();
				IToken token = (IToken)this.Match(this.input, 10, NCalcParser.FOLLOW_ID_in_identifier934);
				CommonTree child = (CommonTree)this.adaptor.Create(token);
				this.adaptor.AddChild(commonTree, child);
				identifier_return.value = new Identifier((token != null) ? token.Text : null);
				break;
			}
			case 2:
			{
				commonTree = (CommonTree)this.adaptor.GetNilNode();
				IToken token2 = (IToken)this.Match(this.input, 11, NCalcParser.FOLLOW_NAME_in_identifier942);
				CommonTree child2 = (CommonTree)this.adaptor.Create(token2);
				this.adaptor.AddChild(commonTree, child2);
				identifier_return.value = new Identifier(((token2 != null) ? token2.Text : null).Substring(1, ((token2 != null) ? token2.Text : null).Length - 2));
				break;
			}
			}
			identifier_return.Stop = this.input.LT(-1);
			identifier_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			this.adaptor.SetTokenBoundaries(identifier_return.Tree, (IToken)identifier_return.Start, (IToken)identifier_return.Stop);
		}
		catch (RecognitionException ex2)
		{
			this.ReportError(ex2);
			this.Recover(this.input, ex2);
			identifier_return.Tree = (CommonTree)this.adaptor.ErrorNode(this.input, (IToken)identifier_return.Start, this.input.LT(-1), ex2);
		}
		finally
		{
		}
		return identifier_return;
	}
	public NCalcParser.expressionList_return expressionList()
	{
		NCalcParser.expressionList_return expressionList_return = new NCalcParser.expressionList_return();
		expressionList_return.Start = this.input.LT(1);
		List<LogicalExpression> list = new List<LogicalExpression>();
		try
		{
			CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
			base.PushFollow(NCalcParser.FOLLOW_logicalExpression_in_expressionList966);
			NCalcParser.logicalExpression_return logicalExpression_return = this.logicalExpression();
			this.state.followingStackPointer--;
			this.adaptor.AddChild(commonTree, logicalExpression_return.Tree);
			list.Add((logicalExpression_return != null) ? logicalExpression_return.value : null);
			while (true)
			{
				int num = 2;
				int num2 = this.input.LA(1);
				if (num2 == 48)
				{
					num = 1;
				}
				int num3 = num;
				if (num3 != 1)
				{
					break;
				}
				IToken payload = (IToken)this.Match(this.input, 48, NCalcParser.FOLLOW_48_in_expressionList973);
				CommonTree child = (CommonTree)this.adaptor.Create(payload);
				this.adaptor.AddChild(commonTree, child);
				base.PushFollow(NCalcParser.FOLLOW_logicalExpression_in_expressionList977);
				NCalcParser.logicalExpression_return logicalExpression_return2 = this.logicalExpression();
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree, logicalExpression_return2.Tree);
				list.Add((logicalExpression_return2 != null) ? logicalExpression_return2.value : null);
			}
			expressionList_return.value = list;
			expressionList_return.Stop = this.input.LT(-1);
			expressionList_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			this.adaptor.SetTokenBoundaries(expressionList_return.Tree, (IToken)expressionList_return.Start, (IToken)expressionList_return.Stop);
		}
		catch (RecognitionException ex)
		{
			this.ReportError(ex);
			this.Recover(this.input, ex);
			expressionList_return.Tree = (CommonTree)this.adaptor.ErrorNode(this.input, (IToken)expressionList_return.Start, this.input.LT(-1), ex);
		}
		finally
		{
		}
		return expressionList_return;
	}
	public NCalcParser.arguments_return arguments()
	{
		NCalcParser.arguments_return arguments_return = new NCalcParser.arguments_return();
		arguments_return.Start = this.input.LT(1);
		arguments_return.value = new List<LogicalExpression>();
		try
		{
			CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
			IToken payload = (IToken)this.Match(this.input, 46, NCalcParser.FOLLOW_46_in_arguments1006);
			CommonTree child = (CommonTree)this.adaptor.Create(payload);
			this.adaptor.AddChild(commonTree, child);
			int num = 2;
			int num2 = this.input.LA(1);
			if ((num2 >= 4 && num2 <= 11) || num2 == 39 || (num2 >= 43 && num2 <= 46))
			{
				num = 1;
			}
			int num3 = num;
			if (num3 == 1)
			{
				base.PushFollow(NCalcParser.FOLLOW_expressionList_in_arguments1010);
				NCalcParser.expressionList_return expressionList_return = this.expressionList();
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree, expressionList_return.Tree);
				arguments_return.value = ((expressionList_return != null) ? expressionList_return.value : null);
			}
			IToken payload2 = (IToken)this.Match(this.input, 47, NCalcParser.FOLLOW_47_in_arguments1017);
			CommonTree child2 = (CommonTree)this.adaptor.Create(payload2);
			this.adaptor.AddChild(commonTree, child2);
			arguments_return.Stop = this.input.LT(-1);
			arguments_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			this.adaptor.SetTokenBoundaries(arguments_return.Tree, (IToken)arguments_return.Start, (IToken)arguments_return.Stop);
		}
		catch (RecognitionException ex)
		{
			this.ReportError(ex);
			this.Recover(this.input, ex);
			arguments_return.Tree = (CommonTree)this.adaptor.ErrorNode(this.input, (IToken)arguments_return.Start, this.input.LT(-1), ex);
		}
		finally
		{
		}
		return arguments_return;
	}
	private void InitializeCyclicDFAs()
	{
	}
	static NCalcParser()
	{
		// Note: this type is marked as 'beforefieldinit'.
		ulong[] bits_ = new ulong[1];
		NCalcParser.FOLLOW_logicalExpression_in_ncalcExpression56 = new BitSet(bits_);
		NCalcParser.FOLLOW_EOF_in_ncalcExpression58 = new BitSet(new ulong[]
		{
			2uL
		});
		NCalcParser.FOLLOW_conditionalExpression_in_logicalExpression78 = new BitSet(new ulong[]
		{
			524290uL
		});
		NCalcParser.FOLLOW_19_in_logicalExpression84 = new BitSet(new ulong[]
		{
			132491151151088uL
		});
		NCalcParser.FOLLOW_conditionalExpression_in_logicalExpression88 = new BitSet(new ulong[]
		{
			1048576uL
		});
		NCalcParser.FOLLOW_20_in_logicalExpression90 = new BitSet(new ulong[]
		{
			132491151151088uL
		});
		NCalcParser.FOLLOW_conditionalExpression_in_logicalExpression94 = new BitSet(new ulong[]
		{
			2uL
		});
		NCalcParser.FOLLOW_booleanAndExpression_in_conditionalExpression121 = new BitSet(new ulong[]
		{
			6291458uL
		});
		NCalcParser.FOLLOW_set_in_conditionalExpression130 = new BitSet(new ulong[]
		{
			132491151151088uL
		});
		NCalcParser.FOLLOW_conditionalExpression_in_conditionalExpression146 = new BitSet(new ulong[]
		{
			6291458uL
		});
		NCalcParser.FOLLOW_bitwiseOrExpression_in_booleanAndExpression180 = new BitSet(new ulong[]
		{
			25165826uL
		});
		NCalcParser.FOLLOW_set_in_booleanAndExpression189 = new BitSet(new ulong[]
		{
			132491151151088uL
		});
		NCalcParser.FOLLOW_bitwiseOrExpression_in_booleanAndExpression205 = new BitSet(new ulong[]
		{
			25165826uL
		});
		NCalcParser.FOLLOW_bitwiseXOrExpression_in_bitwiseOrExpression237 = new BitSet(new ulong[]
		{
			33554434uL
		});
		NCalcParser.FOLLOW_25_in_bitwiseOrExpression246 = new BitSet(new ulong[]
		{
			132491151151088uL
		});
		NCalcParser.FOLLOW_bitwiseOrExpression_in_bitwiseOrExpression256 = new BitSet(new ulong[]
		{
			33554434uL
		});
		NCalcParser.FOLLOW_bitwiseAndExpression_in_bitwiseXOrExpression290 = new BitSet(new ulong[]
		{
			67108866uL
		});
		NCalcParser.FOLLOW_26_in_bitwiseXOrExpression299 = new BitSet(new ulong[]
		{
			132491151151088uL
		});
		NCalcParser.FOLLOW_bitwiseAndExpression_in_bitwiseXOrExpression309 = new BitSet(new ulong[]
		{
			67108866uL
		});
		NCalcParser.FOLLOW_equalityExpression_in_bitwiseAndExpression341 = new BitSet(new ulong[]
		{
			134217730uL
		});
		NCalcParser.FOLLOW_27_in_bitwiseAndExpression350 = new BitSet(new ulong[]
		{
			132491151151088uL
		});
		NCalcParser.FOLLOW_equalityExpression_in_bitwiseAndExpression360 = new BitSet(new ulong[]
		{
			134217730uL
		});
		NCalcParser.FOLLOW_relationalExpression_in_equalityExpression394 = new BitSet(new ulong[]
		{
			(ulong)-268435454
		});
		NCalcParser.FOLLOW_set_in_equalityExpression405 = new BitSet(new ulong[]
		{
			132491151151088uL
		});
		NCalcParser.FOLLOW_set_in_equalityExpression422 = new BitSet(new ulong[]
		{
			132491151151088uL
		});
		NCalcParser.FOLLOW_relationalExpression_in_equalityExpression441 = new BitSet(new ulong[]
		{
			(ulong)-268435454
		});
		NCalcParser.FOLLOW_shiftExpression_in_relationalExpression474 = new BitSet(new ulong[]
		{
			64424509442uL
		});
		NCalcParser.FOLLOW_32_in_relationalExpression485 = new BitSet(new ulong[]
		{
			132491151151088uL
		});
		NCalcParser.FOLLOW_33_in_relationalExpression495 = new BitSet(new ulong[]
		{
			132491151151088uL
		});
		NCalcParser.FOLLOW_34_in_relationalExpression506 = new BitSet(new ulong[]
		{
			132491151151088uL
		});
		NCalcParser.FOLLOW_35_in_relationalExpression516 = new BitSet(new ulong[]
		{
			132491151151088uL
		});
		NCalcParser.FOLLOW_shiftExpression_in_relationalExpression528 = new BitSet(new ulong[]
		{
			64424509442uL
		});
		NCalcParser.FOLLOW_additiveExpression_in_shiftExpression560 = new BitSet(new ulong[]
		{
			206158430210uL
		});
		NCalcParser.FOLLOW_36_in_shiftExpression571 = new BitSet(new ulong[]
		{
			132491151151088uL
		});
		NCalcParser.FOLLOW_37_in_shiftExpression581 = new BitSet(new ulong[]
		{
			132491151151088uL
		});
		NCalcParser.FOLLOW_additiveExpression_in_shiftExpression593 = new BitSet(new ulong[]
		{
			206158430210uL
		});
		NCalcParser.FOLLOW_multiplicativeExpression_in_additiveExpression625 = new BitSet(new ulong[]
		{
			824633720834uL
		});
		NCalcParser.FOLLOW_38_in_additiveExpression636 = new BitSet(new ulong[]
		{
			132491151151088uL
		});
		NCalcParser.FOLLOW_39_in_additiveExpression646 = new BitSet(new ulong[]
		{
			132491151151088uL
		});
		NCalcParser.FOLLOW_multiplicativeExpression_in_additiveExpression658 = new BitSet(new ulong[]
		{
			824633720834uL
		});
		NCalcParser.FOLLOW_unaryExpression_in_multiplicativeExpression690 = new BitSet(new ulong[]
		{
			7696581394434uL
		});
		NCalcParser.FOLLOW_40_in_multiplicativeExpression701 = new BitSet(new ulong[]
		{
			132491151151088uL
		});
		NCalcParser.FOLLOW_41_in_multiplicativeExpression711 = new BitSet(new ulong[]
		{
			132491151151088uL
		});
		NCalcParser.FOLLOW_42_in_multiplicativeExpression721 = new BitSet(new ulong[]
		{
			132491151151088uL
		});
		NCalcParser.FOLLOW_unaryExpression_in_multiplicativeExpression733 = new BitSet(new ulong[]
		{
			7696581394434uL
		});
		NCalcParser.FOLLOW_primaryExpression_in_unaryExpression760 = new BitSet(new ulong[]
		{
			2uL
		});
		NCalcParser.FOLLOW_set_in_unaryExpression771 = new BitSet(new ulong[]
		{
			70368744181744uL
		});
		NCalcParser.FOLLOW_primaryExpression_in_unaryExpression779 = new BitSet(new ulong[]
		{
			2uL
		});
		NCalcParser.FOLLOW_45_in_unaryExpression791 = new BitSet(new ulong[]
		{
			70368744181744uL
		});
		NCalcParser.FOLLOW_primaryExpression_in_unaryExpression794 = new BitSet(new ulong[]
		{
			2uL
		});
		NCalcParser.FOLLOW_39_in_unaryExpression805 = new BitSet(new ulong[]
		{
			70368744181744uL
		});
		NCalcParser.FOLLOW_primaryExpression_in_unaryExpression807 = new BitSet(new ulong[]
		{
			2uL
		});
		NCalcParser.FOLLOW_46_in_primaryExpression829 = new BitSet(new ulong[]
		{
			132491151151088uL
		});
		NCalcParser.FOLLOW_logicalExpression_in_primaryExpression831 = new BitSet(new ulong[]
		{
			140737488355328uL
		});
		NCalcParser.FOLLOW_47_in_primaryExpression833 = new BitSet(new ulong[]
		{
			2uL
		});
		NCalcParser.FOLLOW_value_in_primaryExpression843 = new BitSet(new ulong[]
		{
			2uL
		});
		NCalcParser.FOLLOW_identifier_in_primaryExpression851 = new BitSet(new ulong[]
		{
			70368744177666uL
		});
		NCalcParser.FOLLOW_arguments_in_primaryExpression856 = new BitSet(new ulong[]
		{
			2uL
		});
		NCalcParser.FOLLOW_INTEGER_in_value876 = new BitSet(new ulong[]
		{
			2uL
		});
		NCalcParser.FOLLOW_FLOAT_in_value884 = new BitSet(new ulong[]
		{
			2uL
		});
		NCalcParser.FOLLOW_STRING_in_value892 = new BitSet(new ulong[]
		{
			2uL
		});
		NCalcParser.FOLLOW_DATETIME_in_value901 = new BitSet(new ulong[]
		{
			2uL
		});
		NCalcParser.FOLLOW_TRUE_in_value908 = new BitSet(new ulong[]
		{
			2uL
		});
		NCalcParser.FOLLOW_FALSE_in_value916 = new BitSet(new ulong[]
		{
			2uL
		});
		NCalcParser.FOLLOW_ID_in_identifier934 = new BitSet(new ulong[]
		{
			2uL
		});
		NCalcParser.FOLLOW_NAME_in_identifier942 = new BitSet(new ulong[]
		{
			2uL
		});
		NCalcParser.FOLLOW_logicalExpression_in_expressionList966 = new BitSet(new ulong[]
		{
			281474976710658uL
		});
		NCalcParser.FOLLOW_48_in_expressionList973 = new BitSet(new ulong[]
		{
			132491151151088uL
		});
		NCalcParser.FOLLOW_logicalExpression_in_expressionList977 = new BitSet(new ulong[]
		{
			281474976710658uL
		});
		NCalcParser.FOLLOW_46_in_arguments1006 = new BitSet(new ulong[]
		{
			273228639506416uL
		});
		NCalcParser.FOLLOW_expressionList_in_arguments1010 = new BitSet(new ulong[]
		{
			140737488355328uL
		});
		NCalcParser.FOLLOW_47_in_arguments1017 = new BitSet(new ulong[]
		{
			2uL
		});
	}
}
