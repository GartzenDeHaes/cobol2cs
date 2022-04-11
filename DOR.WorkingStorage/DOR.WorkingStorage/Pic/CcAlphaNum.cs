using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DOR.Core;

namespace DOR.WorkingStorage.Pic
{
	class CcAlphaNum : CharacterClass
	{
		public CcAlphaNum(string lexum)
		: base(lexum)
		{
		}

		public CcAlphaNum(string lexum, string length)
		: base(lexum, length)
		{
		}

		public override void Format(StringBuilder buf, string raw, ref int pos)
		{
			if (pos == raw.Length - 1 && Length <= raw.Length)
			{
				pos -= Length;
				if (raw.Length > Length)
				{
					buf.Append(StringHelper.Reverse(raw.Substring(0, Length)));
				}
				else if (raw.Length < Length)
				{
					buf.Append(StringHelper.Reverse(StringHelper.PadLeft(raw, Length - raw.Length, ' ')));
				}
				else
				{
					buf.Append(StringHelper.Reverse(raw));
				}
				return;
			}

			int padLen = pos < (Length-1) ? Length - 1 - pos : 0;
			for (int x = 0; x < Length && pos >= 0; x++)
			{
				buf.Append(raw[pos--]);
			}
			if (padLen > 0)
			{
				buf.Append(StringHelper.RepeatChar(' ', padLen));
			}
		}

		public override string ToRawString(string raw, ref int pos)
		{
			int maskPos = 0;
			StringBuilder buf = new StringBuilder();

			if (pos < raw.Length && (raw[pos] == '-' || raw[pos] == '+'))
			{
				if (pos + 1 < raw.Length && Char.IsDigit(raw[pos + 1]))
				{
					pos++;
				}
			}

			while (pos < raw.Length && maskPos < Length)
			{
				buf.Append(raw[pos++]);
				maskPos++;
			}

			if (maskPos < Length)
			{
				buf.Append(StringHelper.RepeatChar(' ', Length - maskPos));
			}

			return buf.ToString();
		}
	}
}
