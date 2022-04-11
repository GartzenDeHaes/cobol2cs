using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DOR.Core;

namespace DOR.WorkingStorage.Pic
{
	class CcPlusSigned : CcNum
	{
		public CcPlusSigned(string mask)
		: base(mask, StringHelper.CountOccurancesOf(mask, '+'))
		{
		}

		public override void Format(StringBuilder buf, string raw, ref int pos)
		{
			StringBuilder buf2 = new StringBuilder();

			FormatMask('+', '0', false, false, false, buf2, raw, ref pos);

			string sign;
			if (pos < 0)
			{
				sign = raw[0].ToString();
			}
			else
			{
				sign = raw[pos].ToString();
			}
			if (sign != "+" && sign != "-" && sign != "0")
			{
				sign = " ";
			}

			int signOff = 1;

			if (sign == " ")
			{
				sign = "+";
			}
			else if (sign == "0")
			{
				sign = "+";
				signOff = 0;
			}

			while (buf2.Length > 0 && buf2[buf2.Length - 1] == '0')
			{
				buf2.Length--;
			}

			if (buf2.Length == 0)
			{
				buf.Append(StringHelper.RepeatChar(' ', Length));
				return;
			}

			string b = buf2.ToString().TrimEnd();
			buf.Append(b + sign + StringHelper.RepeatChar(' ', Length - signOff - b.Length));
		}

		public override string ToRawString(string raw, ref int pos)
		{
			string s = raw.Substring(pos, pos + Length > raw.Length ? raw.Length - pos : Length);
			int dotPos = s.IndexOf('.');
			
			if (dotPos > -1)
			{
				s = s.Substring(0, dotPos);
			}

			pos = dotPos;

			if (s[0] == ' ' || s[s.Length - 1] == ' ')
			{
				s = s.Trim();
			}

			char sign = '+';
			if (s[0] == '+')
			{
				s = s.Substring(1);
			}
			else if(s[0] == '-')
			{
				s = s.Substring(1);
				sign = '-';
			}

			int trimPos = 0;
			while (s[trimPos] == '0')
			{
				trimPos++;
			}
			if (trimPos > 0)
			{
				s = s.Substring(trimPos);
			}

			return StringHelper.PadLeft(sign + s, Length, ' ');
		}

		public override bool Equals(object obj)
		{
			return obj is CcPlusSigned && ((CcPlusSigned)obj).Mask == Mask;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
