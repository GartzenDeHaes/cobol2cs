using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core;

namespace CobolParser.Expressions.Terms
{
	public class ExprTerm : ITerm
	{
		public IExpr InnerExpression
		{
			get;
			private set;
		}

		public ExprTerm(Terminalize terms)
		{
			terms.Match(StringNodeType.LPar);
			InnerExpression = IExpr.Parse(terms);

			while (terms.Current.Type != StringNodeType.RPar)
			{
				if (InnerExpression is Expr)
				{
					((Expr)InnerExpression).Terms.Add(ITerm.Parse(terms));
				}
				else
				{
					((ValueList)InnerExpression).Items.Add(ITerm.Parse(terms));
				}
			}
			terms.Match(StringNodeType.RPar);
		}

		public override string ToDocumentationString()
		{
			return "(" + InnerExpression.ToDocumentationString() + ")";
		}

		public override string ToCListStringList()
		{
			return "\"(" + InnerExpression.ToCListStringList() + ")\"";
		}

		public override string ToString()
		{
			return "(" + InnerExpression.ToString() + ")";
		}
	}
}
