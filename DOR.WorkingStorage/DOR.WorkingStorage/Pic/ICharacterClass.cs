using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.WorkingStorage.Pic
{
	interface ICharacterClass
	{
		int Length { get; }
		string Mask { get; }

		void Format(StringBuilder buf, string raw, ref int pos);
		string ToRawString(string raw, ref int pos);
	}
}
