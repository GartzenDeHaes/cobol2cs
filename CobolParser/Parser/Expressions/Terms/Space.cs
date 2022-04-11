using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DOR.Core;

namespace CobolParser.Expressions.Terms
{
	public class Space : ITerm
	{
		public override string ToDocumentationString()
		{
			return "SPACE";
		}

		public override string ToCListStringList()
		{
			return StringHelper.EnsureQuotes(" ");
		}

		public override string ToString()
		{
			return StringHelper.EnsureQuotes(" ");
		}
	}
}
