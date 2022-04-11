using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Expressions.Terms
{
	public class Spaces : ITerm
	{
		public override string ToDocumentationString()
		{
			return "SPACES";
		}

		public override string ToString()
		{
			return "Spaces.Instance()";
		}
	}
}
