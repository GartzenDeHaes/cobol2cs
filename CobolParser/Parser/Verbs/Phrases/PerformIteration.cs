using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CobolParser.Expressions;
using CobolParser.Expressions.Terms;

namespace CobolParser.Verbs.Phrases
{
	public class PerformIteration
	{
		public ITerm IterVar
		{
			get;
			private set;
		}

		public ITerm Step
		{
			get;
			private set;
		}

		public ITerm Start
		{
			get;
			private set;
		}

		public ITerm Stop
		{
			get;
			private set;
		}

		public PerformIteration(Terminalize terms)
		{
			if (VerbLookup.CanCreate(terms.Current, DivisionType.Procedure))
			{
				return;
			}
			if (terms.CurrentNextEquals(1, "TIMES") || terms.CurrentNextEquals(3, "TIMES"))
			{
				Stop = ITerm.Parse(terms);
				Start = new Number(1);
				terms.Match("TIMES");
				return;
			}
			terms.Match("VARYING");
			IterVar = ITerm.Parse(terms);
			terms.Match("FROM");
			Start = ITerm.Parse(terms);

			if (terms.CurrentEquals("TO"))
			{
				terms.Match("TO");
				Stop = ITerm.Parse(terms);
			}

			if (terms.CurrentEquals("BY"))
			{
				terms.Match("BY");
				Step = ITerm.Parse(terms);
			}
		}
	}
}
