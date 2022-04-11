using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Verbs.Phrases
{
	public class PerformStatements : IPerformInner
	{
		public StatementBlock Stmts
		{
			get;
			private set;
		}

		public PerformIteration Iteration
		{
			get;
			private set;
		}

		public IExpr UntilExpr
		{
			get;
			private set;
		}

		public PerformStatements(Terminalize terms)
		{
			CheckVarying(terms);
			Stmts = new StatementBlock(terms);
			CheckVarying(terms);
		}

		private void CheckVarying(Terminalize terms)
		{
			if (terms.CurrentEquals("VARYING"))
			{
				Iteration = new PerformIteration(terms);
			}
			if (terms.CurrentEquals("UNITL"))
			{
				terms.Match("UNTIL");
				UntilExpr = IExpr.Parse(terms);
			}
		}
	}
}
