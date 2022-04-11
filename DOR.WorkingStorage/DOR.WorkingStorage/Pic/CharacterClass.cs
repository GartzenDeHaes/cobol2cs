using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DOR.Core;

namespace DOR.WorkingStorage.Pic
{
	abstract class CharacterClass : ICharacterClass
	{
		public int Length
		{
			get;
			protected set;
		}

		public string Mask
		{
			get;
			private set;
		}

		public CharacterClass(string lex)
		{
			Length = lex.Length;
			Mask = lex;
		}

		public CharacterClass(string lex, int length)
		{
			Length = length;
			Mask = lex;
		}

		public CharacterClass(string lex, string length)
		{
			Length = Int32.Parse(length);

			if (lex.Length == 1)
			{
				Mask = StringHelper.RepeatChar(lex[0], Length);
			}
			else
			{
				Mask = lex;
			}
		}

		public abstract void Format(StringBuilder buf, string raw, ref int pos);
		public abstract string ToRawString(string raw, ref int pos);

		public string ToRawString(string raw, char pad, bool padLeft, ref int pos)
		{
			string data;

			if (padLeft)
			{
				data = StringHelper.SubstringPadLeft(raw, pos, Length, pad);
			}
			else
			{
				data = StringHelper.SubstringPadRight(raw, pos, Length, pad);
			}
			if (Length > raw.Length - pos)
			{
				// result padded
				pos += raw.Length - pos;
			}
			else
			{
				pos += data.Length;
			}
			if (data.Length < Length)
			{
				data = StringHelper.PadLeft(data, Length, pad);
			}
			return data;
		}

		protected void FormatMask
		(
			char maskChar, 
			char padChar,
			bool isPadLeft, 
			bool acceptMinSigns,
			bool acceptPlusSigns,
			StringBuilder buf, 
			string raw, 
			ref int pos
		)
		{
			if (isPadLeft)
			{
				int maskPos = 0;
				int posOrig = pos;
				int dotPos = raw.IndexOf('.');

				if (dotPos < 0)
				{
					pos -= Length;
				}
				else
				{
					pos = dotPos;
				}

				StringBuilder bufRev = new StringBuilder();
				int writeLen = 0;

				for (int x = pos + 1; x <= posOrig && maskPos < Mask.Length; x++)
				{
					char ch = raw[x];
					if (ch == '-' || ch == '+')
					{
						break;
					}
					char chMask = Mask[maskPos++];
					if (chMask != maskChar)
					{
						bufRev.Append(chMask);
						maskPos++;
					}
					bufRev.Append(ch);
					writeLen++;
				}

				if (writeLen < Mask.Length)
				{
					buf.Append(StringHelper.RepeatChar(padChar, Mask.Length - writeLen));
				}
				buf.Append(StringHelper.Reverse(bufRev.ToString()));
			}
			else
			{
				int maskPos = Mask.Length - 1;

				while (maskPos >= 0)
				{
					if (pos < 0 || (!acceptMinSigns && raw[pos] == '-'))
					{
						buf.Append(padChar);
						maskPos--;
						continue;
					}
					if (pos < 0 || (!acceptPlusSigns && raw[pos] == '+'))
					{
						buf.Append(padChar);
						maskPos--;
						continue;
					}

					char ch = raw[pos--];
					Debug.Assert(ch != '.');

					char chMask = Mask[maskPos--];
					if (chMask != maskChar)
					{
						buf.Append(chMask);
						maskPos--;
					}
					if (maskChar == '9' && ch == ' ')
					{
						buf.Append('0');
					}
					else
					{
						buf.Append(ch);
					}
				}
			}
		}
		
		public override bool Equals(object obj)
		{
			if (obj is CharacterClass)
			{
				return Mask == ((CharacterClass)obj).Mask && Length == ((CharacterClass)obj).Length;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
