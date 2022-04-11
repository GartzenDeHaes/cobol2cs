using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core;

namespace CobolParser.Expressions.Terms
{
	public class Zero : ITerm
	{
		public override string ToDocumentationString()
		{
			return "ZERO";
		}


		public override string ToCListStringList()
		{
			return StringHelper.EnsureQuotes("0");
		}

		public override string ToString()
		{
			return "0";
		}
	}
}
