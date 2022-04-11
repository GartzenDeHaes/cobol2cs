using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Verbs.Phrases
{
	public class SearchWhen
	{
		public IExpr Condition
		{
			get;
			private set;
		}

		public StatementBlock Stmts
		{
			get;
			private set;
		}

		public SearchWhen(Terminalize terms)
		{
			terms.Match("WHEN");
			Condition = IExpr.Parse(terms);
			Stmts = new StatementBlock(terms);
		}
	}
}
