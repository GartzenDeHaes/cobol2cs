﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.WorkingStorage.Pic
{
	class CcDot : ICharacterClass
	{
		public int Length
		{
			get { return 1; }
		}

		public string Mask
		{
			get { return "."; }
		}

		public void Format(StringBuilder buf, string raw, ref int pos)
		{
			if (raw[pos] == '.')
			{
				pos--;
			}
			buf.Append('.');
		}

		public string ToRawString(string raw, ref int pos)
		{
			if (pos < raw.Length && raw[pos] == '.')
			{
				pos++;
			}
			return ".";
		}

		public override bool Equals(object obj)
		{
			return obj is CcDot;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
