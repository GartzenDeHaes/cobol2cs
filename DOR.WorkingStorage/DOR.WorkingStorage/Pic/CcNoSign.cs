using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DOR.WorkingStorage.Pic
{
	class CcNoSign : ICharacterClass
	{
		public int Length
		{
			get { return 0; }
		}

		public string Mask
		{
			get { return ""; }
		}

		public void Format(StringBuilder buf, string raw, ref int pos)
		{
			if (pos < 0)
			{
				return;
			}
			if (raw[pos] == '+' || raw[pos] == '-')
			{
				pos--;
			}
		}

		public string ToRawString(string raw, ref int pos)
		{
			if (pos < raw.Length && (raw[pos] == '+' || raw[pos] == '-'))
			{
				pos++;
			}
			return "";
		}

		public override bool Equals(object obj)
		{
			return obj is CcNoSign;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
