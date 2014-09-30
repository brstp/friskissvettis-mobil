using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public class Token
	{
		public const int ERROR = -1;
		public const int EOF = 0;
		public const int EOL = 1;
		public const int FIRST_BYTECODE_TOKEN = 2;
		public const int ENTERWITH = 2;
		public const int LEAVEWITH = 3;
		public const int RETURN = 4;
		public const int GOTO = 5;
		public const int IFEQ = 6;
		public const int IFNE = 7;
		public const int SETNAME = 8;
		public const int BITOR = 9;
		public const int BITXOR = 10;
		public const int BITAND = 11;
		public const int EQ = 12;
		public const int NE = 13;
		public const int LT = 14;
		public const int LE = 15;
		public const int GT = 16;
		public const int GE = 17;
		public const int LSH = 18;
		public const int RSH = 19;
		public const int URSH = 20;
		public const int ADD = 21;
		public const int SUB = 22;
		public const int MUL = 23;
		public const int DIV = 24;
		public const int MOD = 25;
		public const int NOT = 26;
		public const int BITNOT = 27;
		public const int POS = 28;
		public const int NEG = 29;
		public const int NEW = 30;
		public const int DELPROP = 31;
		public const int TYPEOF = 32;
		public const int GETPROP = 33;
		public const int SETPROP = 34;
		public const int GETELEM = 35;
		public const int SETELEM = 36;
		public const int CALL = 37;
		public const int NAME = 38;
		public const int NUMBER = 39;
		public const int STRING = 40;
		public const int NULL = 41;
		public const int THIS = 42;
		public const int FALSE = 43;
		public const int TRUE = 44;
		public const int SHEQ = 45;
		public const int SHNE = 46;
		public const int REGEXP = 47;
		public const int BINDNAME = 48;
		public const int THROW = 49;
		public const int RETHROW = 50;
		public const int IN = 51;
		public const int INSTANCEOF = 52;
		public const int LOCAL_LOAD = 53;
		public const int GETVAR = 54;
		public const int SETVAR = 55;
		public const int CATCH_SCOPE = 56;
		public const int ENUM_INIT_KEYS = 57;
		public const int ENUM_INIT_VALUES = 58;
		public const int ENUM_NEXT = 59;
		public const int ENUM_ID = 60;
		public const int THISFN = 61;
		public const int RETURN_RESULT = 62;
		public const int ARRAYLIT = 63;
		public const int OBJECTLIT = 64;
		public const int GET_REF = 65;
		public const int SET_REF = 66;
		public const int DEL_REF = 67;
		public const int REF_CALL = 68;
		public const int REF_SPECIAL = 69;
		public const int DEFAULTNAMESPACE = 70;
		public const int ESCXMLATTR = 71;
		public const int ESCXMLTEXT = 72;
		public const int REF_MEMBER = 73;
		public const int REF_NS_MEMBER = 74;
		public const int REF_NAME = 75;
		public const int REF_NS_NAME = 76;
		public const int SETPROP_GETTER = 77;
		public const int SETPROP_SETTER = 78;
		public const int LAST_BYTECODE_TOKEN = 78;
		public const int TRY = 79;
		public const int SEMI = 80;
		public const int LB = 81;
		public const int RB = 82;
		public const int LC = 83;
		public const int RC = 84;
		public const int LP = 85;
		public const int RP = 86;
		public const int COMMA = 87;
		public const int ASSIGN = 88;
		public const int ASSIGN_BITOR = 89;
		public const int ASSIGN_BITXOR = 90;
		public const int ASSIGN_BITAND = 91;
		public const int ASSIGN_LSH = 92;
		public const int ASSIGN_RSH = 93;
		public const int ASSIGN_URSH = 94;
		public const int ASSIGN_ADD = 95;
		public const int ASSIGN_SUB = 96;
		public const int ASSIGN_MUL = 97;
		public const int ASSIGN_DIV = 98;
		public const int ASSIGN_MOD = 99;
		public const int FIRST_ASSIGN = 88;
		public const int LAST_ASSIGN = 99;
		public const int HOOK = 100;
		public const int COLON = 101;
		public const int OR = 102;
		public const int AND = 103;
		public const int INC = 104;
		public const int DEC = 105;
		public const int DOT = 106;
		public const int FUNCTION = 107;
		public const int EXPORT = 108;
		public const int IMPORT = 109;
		public const int IF = 110;
		public const int ELSE = 111;
		public const int SWITCH = 112;
		public const int CASE = 113;
		public const int DEFAULT = 114;
		public const int WHILE = 115;
		public const int DO = 116;
		public const int FOR = 117;
		public const int BREAK = 118;
		public const int CONTINUE = 119;
		public const int VAR = 120;
		public const int WITH = 121;
		public const int CATCH = 122;
		public const int FINALLY = 123;
		public const int VOID = 124;
		public const int RESERVED = 125;
		public const int EMPTY = 126;
		public const int BLOCK = 127;
		public const int LABEL = 128;
		public const int TARGET = 129;
		public const int LOOP = 130;
		public const int EXPR_VOID = 131;
		public const int EXPR_RESULT = 132;
		public const int JSR = 133;
		public const int SCRIPT = 134;
		public const int TYPEOFNAME = 135;
		public const int USE_STACK = 136;
		public const int SETPROP_OP = 137;
		public const int SETELEM_OP = 138;
		public const int LOCAL_BLOCK = 139;
		public const int SET_REF_OP = 140;
		public const int DOTDOT = 141;
		public const int COLONCOLON = 142;
		public const int XML = 143;
		public const int DOTQUERY = 144;
		public const int XMLATTR = 145;
		public const int XMLEND = 146;
		public const int TO_OBJECT = 147;
		public const int TO_DOUBLE = 148;
		public const int GET = 149;
		public const int SET = 150;
		public const int CONST = 151;
		public const int SETCONST = 152;
		public const int SETCONSTVAR = 153;
		public const int CONDCOMMENT = 154;
		public const int KEEPCOMMENT = 155;
		public const int DEBUGGER = 156;
		public const int LAST_TOKEN = 157;
		internal static readonly bool printTrees = false;
		internal static readonly bool printICode = false;
		internal static readonly bool printNames = Token.printTrees || Token.printICode;
		public static string name(int token)
		{
			string result;
			switch (token)
			{
			case -1:
				result = "ERROR";
				return result;
			case 0:
				result = "EOF";
				return result;
			case 1:
				result = "EOL";
				return result;
			case 2:
				result = "ENTERWITH";
				return result;
			case 3:
				result = "LEAVEWITH";
				return result;
			case 4:
				result = "RETURN";
				return result;
			case 5:
				result = "GOTO";
				return result;
			case 6:
				result = "IFEQ";
				return result;
			case 7:
				result = "IFNE";
				return result;
			case 8:
				result = "SETNAME";
				return result;
			case 9:
				result = "BITOR";
				return result;
			case 10:
				result = "BITXOR";
				return result;
			case 11:
				result = "BITAND";
				return result;
			case 12:
				result = "EQ";
				return result;
			case 13:
				result = "NE";
				return result;
			case 14:
				result = "LT";
				return result;
			case 15:
				result = "LE";
				return result;
			case 16:
				result = "GT";
				return result;
			case 17:
				result = "GE";
				return result;
			case 18:
				result = "LSH";
				return result;
			case 19:
				result = "RSH";
				return result;
			case 20:
				result = "URSH";
				return result;
			case 21:
				result = "ADD";
				return result;
			case 22:
				result = "SUB";
				return result;
			case 23:
				result = "MUL";
				return result;
			case 24:
				result = "DIV";
				return result;
			case 25:
				result = "MOD";
				return result;
			case 26:
				result = "NOT";
				return result;
			case 27:
				result = "BITNOT";
				return result;
			case 28:
				result = "POS";
				return result;
			case 29:
				result = "NEG";
				return result;
			case 30:
				result = "NEW";
				return result;
			case 31:
				result = "DELPROP";
				return result;
			case 32:
				result = "TYPEOF";
				return result;
			case 33:
				result = "GETPROP";
				return result;
			case 34:
				result = "SETPROP";
				return result;
			case 35:
				result = "GETELEM";
				return result;
			case 36:
				result = "SETELEM";
				return result;
			case 37:
				result = "CALL";
				return result;
			case 38:
				result = "NAME";
				return result;
			case 39:
				result = "NUMBER";
				return result;
			case 40:
				result = "STRING";
				return result;
			case 41:
				result = "NULL";
				return result;
			case 42:
				result = "THIS";
				return result;
			case 43:
				result = "FALSE";
				return result;
			case 44:
				result = "TRUE";
				return result;
			case 45:
				result = "SHEQ";
				return result;
			case 46:
				result = "SHNE";
				return result;
			case 47:
				result = "OBJECT";
				return result;
			case 48:
				result = "BINDNAME";
				return result;
			case 49:
				result = "THROW";
				return result;
			case 50:
				result = "RETHROW";
				return result;
			case 51:
				result = "IN";
				return result;
			case 52:
				result = "INSTANCEOF";
				return result;
			case 53:
				result = "LOCAL_LOAD";
				return result;
			case 54:
				result = "GETVAR";
				return result;
			case 55:
				result = "SETVAR";
				return result;
			case 56:
				result = "CATCH_SCOPE";
				return result;
			case 57:
				result = "ENUM_INIT_KEYS";
				return result;
			case 58:
				result = "ENUM_INIT_VALUES";
				return result;
			case 59:
				result = "ENUM_NEXT";
				return result;
			case 60:
				result = "ENUM_ID";
				return result;
			case 61:
				result = "THISFN";
				return result;
			case 62:
				result = "RETURN_RESULT";
				return result;
			case 63:
				result = "ARRAYLIT";
				return result;
			case 64:
				result = "OBJECTLIT";
				return result;
			case 65:
				result = "GET_REF";
				return result;
			case 66:
				result = "SET_REF";
				return result;
			case 67:
				result = "DEL_REF";
				return result;
			case 68:
				result = "REF_CALL";
				return result;
			case 69:
				result = "REF_SPECIAL";
				return result;
			case 70:
				result = "DEFAULTNAMESPACE";
				return result;
			case 71:
				result = "ESCXMLATTR";
				return result;
			case 72:
				result = "ESCXMLTEXT";
				return result;
			case 73:
				result = "REF_MEMBER";
				return result;
			case 74:
				result = "REF_NS_MEMBER";
				return result;
			case 75:
				result = "REF_NAME";
				return result;
			case 76:
				result = "REF_NS_NAME";
				return result;
			case 79:
				result = "TRY";
				return result;
			case 80:
				result = "SEMI";
				return result;
			case 81:
				result = "LB";
				return result;
			case 82:
				result = "RB";
				return result;
			case 83:
				result = "LC";
				return result;
			case 84:
				result = "RC";
				return result;
			case 85:
				result = "LP";
				return result;
			case 86:
				result = "RP";
				return result;
			case 87:
				result = "COMMA";
				return result;
			case 88:
				result = "ASSIGN";
				return result;
			case 89:
				result = "ASSIGN_BITOR";
				return result;
			case 90:
				result = "ASSIGN_BITXOR";
				return result;
			case 91:
				result = "ASSIGN_BITAND";
				return result;
			case 92:
				result = "ASSIGN_LSH";
				return result;
			case 93:
				result = "ASSIGN_RSH";
				return result;
			case 94:
				result = "ASSIGN_URSH";
				return result;
			case 95:
				result = "ASSIGN_ADD";
				return result;
			case 96:
				result = "ASSIGN_SUB";
				return result;
			case 97:
				result = "ASSIGN_MUL";
				return result;
			case 98:
				result = "ASSIGN_DIV";
				return result;
			case 99:
				result = "ASSIGN_MOD";
				return result;
			case 100:
				result = "HOOK";
				return result;
			case 101:
				result = "COLON";
				return result;
			case 102:
				result = "OR";
				return result;
			case 103:
				result = "AND";
				return result;
			case 104:
				result = "INC";
				return result;
			case 105:
				result = "DEC";
				return result;
			case 106:
				result = "DOT";
				return result;
			case 107:
				result = "FUNCTION";
				return result;
			case 108:
				result = "EXPORT";
				return result;
			case 109:
				result = "IMPORT";
				return result;
			case 110:
				result = "IF";
				return result;
			case 111:
				result = "ELSE";
				return result;
			case 112:
				result = "SWITCH";
				return result;
			case 113:
				result = "CASE";
				return result;
			case 114:
				result = "DEFAULT";
				return result;
			case 115:
				result = "WHILE";
				return result;
			case 116:
				result = "DO";
				return result;
			case 117:
				result = "FOR";
				return result;
			case 118:
				result = "BREAK";
				return result;
			case 119:
				result = "CONTINUE";
				return result;
			case 120:
				result = "VAR";
				return result;
			case 121:
				result = "WITH";
				return result;
			case 122:
				result = "CATCH";
				return result;
			case 123:
				result = "FINALLY";
				return result;
			case 125:
				result = "RESERVED";
				return result;
			case 126:
				result = "EMPTY";
				return result;
			case 127:
				result = "BLOCK";
				return result;
			case 128:
				result = "LABEL";
				return result;
			case 129:
				result = "TARGET";
				return result;
			case 130:
				result = "LOOP";
				return result;
			case 131:
				result = "EXPR_VOID";
				return result;
			case 132:
				result = "EXPR_RESULT";
				return result;
			case 133:
				result = "JSR";
				return result;
			case 134:
				result = "SCRIPT";
				return result;
			case 135:
				result = "TYPEOFNAME";
				return result;
			case 136:
				result = "USE_STACK";
				return result;
			case 137:
				result = "SETPROP_OP";
				return result;
			case 138:
				result = "SETELEM_OP";
				return result;
			case 139:
				result = "LOCAL_BLOCK";
				return result;
			case 140:
				result = "SET_REF_OP";
				return result;
			case 141:
				result = "DOTDOT";
				return result;
			case 142:
				result = "COLONCOLON";
				return result;
			case 143:
				result = "XML";
				return result;
			case 144:
				result = "DOTQUERY";
				return result;
			case 145:
				result = "XMLATTR";
				return result;
			case 146:
				result = "XMLEND";
				return result;
			case 147:
				result = "TO_OBJECT";
				return result;
			case 148:
				result = "TO_DOUBLE";
				return result;
			case 149:
				result = "GET";
				return result;
			case 150:
				result = "SET";
				return result;
			case 151:
				result = "CONST";
				return result;
			case 152:
				result = "SETCONST";
				return result;
			case 153:
				result = "SETCONSTVAR";
				return result;
			case 154:
				result = "CONDCOMMENT";
				return result;
			case 155:
				result = "KEEPCOMMENT";
				return result;
			case 156:
				result = "DEBUGGER";
				return result;
			}
			result = "UNKNOWN Token Type";
			return result;
		}
	}
}
