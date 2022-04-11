using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core;

namespace CobolParser.Expressions.Terms
{
	public class Bool : ITerm
	{
		public bool Value
		{
			get;
			private set;
		}

		public Bool(Terminalize terms)
		{
			Value = terms.Current.Str.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
			terms.Match(StringNodeType.Word);
		}

		public override string ToDocumentationString()
		{
			return Value ? "TRUE" : "FALSE";
		}

		public override string ToCListStringList()
		{
			return StringHelper.EnsureQuotes(ToDocumentationString().ToLower());
		}

		public override string ToString()
		{
			return Value ? "true" : "false";
		}
	}
}
