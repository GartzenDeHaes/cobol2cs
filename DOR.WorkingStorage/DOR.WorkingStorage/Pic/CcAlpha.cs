using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DOR.Core;

namespace DOR.WorkingStorage.Pic
{
	class CcAlpha : CharacterClass
	{
		public CcAlpha(string lexum)
		: base(lexum)
		{
		}

		public CcAlpha(string lexum, string length)
		: base(lexum, length)
		{
		}

		public override void Format(StringBuilder buf, string raw, ref int pos)
		{
			string str = raw.Substring(pos, Length);
			pos -= str.Length;

			buf.Append(StringHelper.Reverse(str));
			
			if (str.Length < Length)
			{
				str = StringHelper.PadLeft(str, Length, ' ');
			}
		}

		public override string ToRawString(string raw, ref int pos)
		{
			return ToRawString(raw, ' ', false, ref pos);
		}
	}
}
