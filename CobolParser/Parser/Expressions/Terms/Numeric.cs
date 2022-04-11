using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Expressions.Terms
{
	public class Numeric : ITerm
	{
		public Numeric(Terminalize terms)
		{
			terms.Match("NUMERIC");
		}

		public override string ToDocumentationString()
		{
			return "NUMERIC";
		}

		public override string ToString()
		{
			return "Numeric.Instance()";
		}
	}
}
