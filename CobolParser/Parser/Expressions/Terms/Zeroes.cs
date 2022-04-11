using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Expressions.Terms
{
	public class Zeroes : ITerm
	{
		public override string ToDocumentationString()
		{
			return "ZEROES";
		}

		public override string ToString()
		{
			return "Zeroes.Instance()";
		}
	}
}
