using System;
namespace Antlr.Runtime
{
	public abstract class DFA
	{
		public delegate int SpecialStateTransitionHandler(DFA dfa, int s, IIntStream input);
		public const bool debug = false;
		protected short[] eot;
		protected short[] eof;
		protected char[] min;
		protected char[] max;
		protected short[] accept;
		protected short[] special;
		protected short[][] transition;
		protected int decisionNumber;
		public DFA.SpecialStateTransitionHandler specialStateTransitionHandler;
		protected BaseRecognizer recognizer;
		public virtual string Description
		{
			get
			{
				return "n/a";
			}
		}
		public int Predict(IIntStream input)
		{
			int marker = input.Mark();
			int num = 0;
			int result;
			try
			{
				char c;
				while (true)
				{
					int num2 = (int)this.special[num];
					if (num2 >= 0)
					{
						num = this.specialStateTransitionHandler(this, num2, input);
						if (num == -1)
						{
							break;
						}
						input.Consume();
					}
					else
					{
						if (this.accept[num] >= 1)
						{
							goto Block_4;
						}
						c = (char)input.LA(1);
						if (c >= this.min[num] && c <= this.max[num])
						{
							int num3 = (int)this.transition[num][(int)(c - this.min[num])];
							if (num3 < 0)
							{
								if (this.eot[num] < 0)
								{
									goto IL_CE;
								}
								num = (int)this.eot[num];
								input.Consume();
							}
							else
							{
								num = num3;
								input.Consume();
							}
						}
						else
						{
							if (this.eot[num] < 0)
							{
								goto IL_10D;
							}
							num = (int)this.eot[num];
							input.Consume();
						}
					}
				}
				this.NoViableAlt(num, input);
				result = 0;
				return result;
				Block_4:
				result = (int)this.accept[num];
				return result;
				IL_CE:
				this.NoViableAlt(num, input);
				result = 0;
				return result;
				IL_10D:
				if (c == (char)Token.EOF && this.eof[num] >= 0)
				{
					result = (int)this.accept[(int)this.eof[num]];
				}
				else
				{
					this.NoViableAlt(num, input);
					result = 0;
				}
			}
			finally
			{
				input.Rewind(marker);
			}
			return result;
		}
		protected void NoViableAlt(int s, IIntStream input)
		{
			if (this.recognizer.state.backtracking > 0)
			{
				this.recognizer.state.failed = true;
				return;
			}
			NoViableAltException ex = new NoViableAltException(this.Description, this.decisionNumber, s, input);
			this.Error(ex);
			throw ex;
		}
		public virtual void Error(NoViableAltException nvae)
		{
		}
		public virtual int SpecialStateTransition(int s, IIntStream input)
		{
			return -1;
		}
		public static short[] UnpackEncodedString(string encodedString)
		{
			int num = 0;
			for (int i = 0; i < encodedString.Length; i += 2)
			{
				num += (int)encodedString[i];
			}
			short[] array = new short[num];
			int num2 = 0;
			for (int j = 0; j < encodedString.Length; j += 2)
			{
				char c = encodedString[j];
				char c2 = encodedString[j + 1];
				for (int k = 1; k <= (int)c; k++)
				{
					array[num2++] = (short)c2;
				}
			}
			return array;
		}
		public static short[][] UnpackEncodedStringArray(string[] encodedStrings)
		{
			short[][] array = new short[encodedStrings.Length][];
			for (int i = 0; i < encodedStrings.Length; i++)
			{
				array[i] = DFA.UnpackEncodedString(encodedStrings[i]);
			}
			return array;
		}
		public static char[] UnpackEncodedStringToUnsignedChars(string encodedString)
		{
			int num = 0;
			for (int i = 0; i < encodedString.Length; i += 2)
			{
				num += (int)encodedString[i];
			}
			char[] array = new char[num];
			int num2 = 0;
			for (int j = 0; j < encodedString.Length; j += 2)
			{
				char c = encodedString[j];
				char c2 = encodedString[j + 1];
				for (int k = 1; k <= (int)c; k++)
				{
					array[num2++] = c2;
				}
			}
			return array;
		}
		public int SpecialTransition(int state, int symbol)
		{
			return 0;
		}
	}
}
