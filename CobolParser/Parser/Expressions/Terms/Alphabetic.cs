using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core;

namespace CobolParser.Expressions.Terms
{
	public class Alphabetic : ITerm
	{
		public Alphabetic(Terminalize terms)
		{
			terms.Match("ALPHABETIC");
		}

		public override string ToDocumentationString()
		{
			return "ALPHABETIC";
		}
	}
}
