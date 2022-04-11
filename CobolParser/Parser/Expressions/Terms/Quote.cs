using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Expressions.Terms
{
	public class Quote : ITerm
	{
		public Quote()
		{
		}

		public override string ToDocumentationString()
		{
			return "QUOTE";
		}

		public override string ToCListStringList()
		{
			return "\"\"\"";
		}
	}
}
