using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DOR.WorkingStorage.Pic
{
	class CcBinary : ICharacterClass
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

		public CcBinary(int length)
		{
			Length = length;
			Mask = "";
		}

		public void Format(StringBuilder buf, string raw, ref int pos)
		{
			for (int x = 0; x < Length; x++)
			{
				buf.Append(raw[pos++]);
			}
		}

		public string ToRawString(string raw, ref int pos)
		{
			StringBuilder buf = new StringBuilder();

			for (int x = 0; x < Length; x++)
			{
				if (pos >= raw.Length)
				{
					buf.Append(' ');
					continue;
				}
				buf.Append(raw[pos++]);
			}

			return buf.ToString();
		}
	}
}
