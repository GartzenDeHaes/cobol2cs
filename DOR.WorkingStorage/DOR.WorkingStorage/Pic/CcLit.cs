using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core;

namespace DOR.WorkingStorage.Pic
{
	class CcLit : ICharacterClass
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

		public CcLit(string mask)
		{
			Length = 0;
			Mask = mask;
		}

		public void Format(StringBuilder buf, string raw, ref int pos)
		{
			buf.Append(Mask);
		}

		public string ToRawString(string raw, ref int pos)
		{
			int maskPos = 0;
			while (maskPos < Mask.Length && pos < raw.Length && raw[pos] == Mask[maskPos++])
			{
				pos++;
			}
			return "";
		}

		public string ToRawString(string raw, char pad, bool padLeft, ref int pos)
		{
			return "";
		}

		public override bool Equals(object obj)
		{
			return obj is CcLit &&
				((CcLit)obj).Mask == Mask;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
