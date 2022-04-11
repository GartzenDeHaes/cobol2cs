using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DOR.Core;

namespace DOR.WorkingStorage.Pic
{
	class CcMask : CharacterClass
	{
		private char MaskChar
		{
			get;
			set;
		}

		/// <summary>
		/// If true, write output from left to right (after dot)
		/// </summary>
		public bool IsPadLeft
		{
			get;
			set;
		}

		public CcMask(string mask)
		: base(mask, StringHelper.CountOccurancesOf(mask, mask[0]))
		{
			MaskChar = mask[0];
		}

		public CcMask(string mask, int length)
		: base(mask, length)
		{
			MaskChar = mask[0];
		}

		public override void Format(StringBuilder buf, string raw, ref int pos)
		{
			StringBuilder buf2 = new StringBuilder();
			FormatMask(Mask[0], ' ', IsPadLeft, true, false, buf2, raw, ref pos);

			if 
			(
				buf2[0] == '0' && 
				!IsPadLeft &&
				StringHelper.CountOccurancesOf(buf2, '0') + 
				StringHelper.CountOccurancesOf(buf2, ' ') == buf2.Length
			)
			{
				buf.Append(StringHelper.RepeatChar(' ', Mask.Length));
			}
			else
			{
				if (buf2.Length > 1)
				{
					char last = buf2[buf2.Length - 1];
					char last2 = buf2[buf2.Length - 2];
					if 
					(
						last == '0' || 
						(last == '-' && last2 == '0') || 
						(last == '+' && last2 == '0')
					)
					{
						// strip

						if (last == '-' || last == '+')
						{
							buf2[buf2.Length-1] = ' ';
						}

						int x = buf2.Length - 1;
						for (; x >= 0; x--)
						{
							if (buf2[x] == '0' || buf2[x] == ',' || buf2[x] == '/')
							{
								buf2[x] = ' ';
								continue;
							}
							break;
						}
						if (last == '-' || last == '+')
						{
							buf2[x] = last;
						}
					}
				}

				buf.Append(buf2);
			}
		}

		public override string ToRawString(string raw, ref int pos)
		{
			if (MaskChar == '-' || MaskChar == '+')
			{
				return ToRawString(raw.Replace(",", "").Replace("/", ""), ' ', true, ref pos);
			}
			else
			{
				if (pos == 0 && raw[0] == '-')
				{
					// sign trailing
					pos++;
				}
				raw = raw.Replace(",", "").Replace("/", "");
				int dotPos = raw.IndexOf('.', pos);
				if (dotPos >= 0)
				{
					raw = raw.Substring(0, dotPos);
				}
				return ToRawString(raw, '0', true, ref pos);
			}
		}
	}
}
