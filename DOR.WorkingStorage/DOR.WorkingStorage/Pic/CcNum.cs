using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DOR.Core;

namespace DOR.WorkingStorage.Pic
{
	class CcNum : CharacterClass
	{
		public bool IsPadLeft
		{
			get;
			set;
		}

		public CcNum(string lexum)
		: base(lexum)
		{
		}

		public CcNum(string lexum, string length)
		: base(lexum, length)
		{
		}

		public CcNum(string lexum, int length)
			: base(lexum, length)
		{
		}

		public override void Format(StringBuilder buf, string raw, ref int pos)
		{
			FormatMask('9', '0', IsPadLeft, false, false, buf, raw, ref pos);
		}

		public override string ToRawString(string raw, ref int pos)
		{
			StringBuilder buf = new StringBuilder();

			for (int x = 0; pos < raw.Length && x < Length; x++)
			{
				char ch = raw[pos];
				if (ch == '.' || ch == '/')
				{
					break;
				}
				if (ch == ',')
				{
					continue;
				}
				if (ch == ' ')
				{
					buf.Append('0');
					pos++;
					continue;
				}
				if (! Char.IsDigit(ch))
				{
					break;
				}
				buf.Append(raw[pos++]);
			}

			if (buf.Length < Length)
			{
				if (IsPadLeft)
				{
					return StringHelper.PadRight(buf.ToString(), Length, '0');
				}
				else
				{
					return StringHelper.PadLeft(buf.ToString(), Length, '0');
				}
			}
			if (buf.Length > Length)
			{
				return buf.ToString().Substring(0, Length);
			}
			return buf.ToString();
		}
	}
}
