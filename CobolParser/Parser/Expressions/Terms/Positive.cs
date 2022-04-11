using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Expressions.Terms
{
	public class Positive : ITerm
	{
		public Positive(Terminalize terms)
		{
			terms.Match("POSITIVE");
		}

		public override string ToDocumentationString()
		{
			return "POSITIVE";
		}
	}
}
