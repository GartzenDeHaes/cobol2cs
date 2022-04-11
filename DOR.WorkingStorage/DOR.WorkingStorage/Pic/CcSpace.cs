using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core;

namespace DOR.WorkingStorage.Pic
{
	class CcSpace : ICharacterClass
	{
		public int Length
		{
			get { return 0; }
		}

		public string Mask
		{
			get;
			private set;
		}

		public CcSpace(string lexum)
		{
			Mask = lexum;
		}

		public void Format(StringBuilder buf, string raw, ref int pos)
		{
			buf.Append(StringHelper.RepeatChar(' ', Mask.Length));
		}

		public string ToRawString(string raw, ref int pos)
		{
			int count = 0;

			while (pos < raw.Length && raw[pos] == ' ' && count < Mask.Length)
			{
				pos++;
				count++;
			}
			return "";
		}
	}
}
