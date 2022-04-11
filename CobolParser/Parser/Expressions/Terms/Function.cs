using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Expressions.Terms
{
	public class Function : ITerm
	{
		public string Name
		{
			get;
			private set;
		}

		public ExprTerm Arguments
		{
			get;
			private set;
		}

		public Function(Terminalize terms)
		{
			terms.Match("FUNCTION");
			Name = terms.Current.Str;
			terms.Next();

			if (terms.Current.Type == StringNodeType.LPar)
			{
				Arguments = new ExprTerm(terms);
			}
		}

		public override string ToDocumentationString()
		{
			return "FUNCTION " + Name + Arguments.ToDocumentationString();
		}

		public override string ToCListStringList()
		{
			return "\"" + Name + "(" + Arguments.ToCListStringList() + ")\"";
		}
	}
}
