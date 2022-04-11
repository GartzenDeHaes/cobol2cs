using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core;

namespace CobolParser.Expressions.Terms
{
	public class Negative : ITerm
	{
		public Negative(Terminalize terms)
		{
			terms.Match("NEGATIVE");
		}

		public override string ToDocumentationString()
		{
			return "NEGATIVE";
		}
	}
}
