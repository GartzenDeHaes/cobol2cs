using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.WorkingStorage.Pic
{
	class CcSigned : ICharacterClass
	{
		public int Length
		{
			get { return 1; }
		}

		public string Mask
		{
			get { return "-"; }
		}

		public void Format(StringBuilder buf, string raw, ref int pos)
		{
			if (pos >= 0 && raw[pos] == '-')
			{
				buf.Append('-');
				pos--;
			}
			else if (pos >= 0 && (raw[pos] == '+' || raw[pos] == ' '))
			{
				buf.Append('+');
				pos--;
			}
			else
			{
				buf.Append('+');
			}
		}

		public string ToRawString(string raw, ref int pos)
		{
			if (pos < raw.Length && raw[pos] == '-')
			{
				pos++;
				return "-";
			}
			if (pos < raw.Length && raw[pos] == '+')
			{
				pos++;
				return "+";
			}
			if (pos < raw.Length && raw[pos] == ' ')
			{
				pos++;
				return "+";
			}
			if (raw[0] == '-')
			{
				return "-";
			}
			if (raw[0] == '+')
			{
				return "+";
			}
			if (raw[raw.Length-1] == '-')
			{
				return "-";
			}
			if (raw[raw.Length - 1] == '+')
			{
				return "+";
			}

			return "+";
		}

		public override bool Equals(object obj)
		{
			return obj is CcSigned;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
