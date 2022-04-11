using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CobolParser.Expressions.Terms;

namespace CobolParser.Verbs.Phrases
{
	public class PerformCall : IPerformInner
	{
		public Id SubRoutine
		{
			get;
			private set;
		}

		public Id ThroughSubRoutine
		{
			get;
			private set;
		}

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

		/// <summary>
		/// PERFORM 1650-BUILD-A-REASON 
		/// WITH TEST AFTER 
		/// UNTIL WS-SAVED-REAS-CODE NOT = WS-REAS-CODE
		/// </summary>
		public bool WithTestAfter
		{
			get;
			private set;
		}

		public PerformCall(Terminalize terms)
		{
			CheckVarying(terms);
			if (VerbLookup.CanCreate(terms.Current, DivisionType.Procedure))
			{
				Stmts = new StatementBlock(terms);
			}
			else
			{
				SubRoutine = new Id(terms);
			}

			if (terms.CurrentEquals("THRU") || terms.CurrentEquals("THROUGH"))
			{
				terms.Next();
				ThroughSubRoutine = new Id(terms);
			}

			if (terms.CurrentEquals("WITH"))
			{
				terms.Next();
				terms.Match("TEST");
				terms.Match("AFTER");
				WithTestAfter = true;
			}

			CheckVarying(terms);
		}

		private void CheckVarying(Terminalize terms)
		{
			if (terms.CurrentNextEquals(1, "TIMES") || terms.CurrentNextEquals(3, "TIMES") || terms.CurrentEquals("VARYING"))
			{
				Iteration = new PerformIteration(terms);
			}
			if (terms.CurrentEquals("UNTIL"))
			{
				terms.Match("UNTIL");
				UntilExpr = IExpr.Parse(terms);
			}
		}
	}
}
