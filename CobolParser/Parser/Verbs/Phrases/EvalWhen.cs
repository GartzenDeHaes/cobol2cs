using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core.Collections;
using CobolParser.Expressions;

namespace CobolParser.Verbs.Phrases
{
	public class EvalWhen
	{
		public Vector<IExpr> AlsoTerms
		{
			get;
			private set;
		}

		public StatementBlock Stmts
		{
			get;
			private set;
		}

		public bool IsDefault
		{
			get;
			private set;
		}

		public EvalWhen(Terminalize terms)
		{
			AlsoTerms = new Vector<IExpr>(3);

			terms.Match("WHEN");

			while (true)
			{
				AlsoTerms.Add(IExpr.Parse(terms));

				if (terms.CurrentEquals("ALSO"))
				{
					terms.Next();
				}
				else
				{
					break;
				}
			}

			if (terms.CurrentEquals("OTHER"))
			{
				IsDefault = true;
				terms.Next();
			}

			Stmts = new StatementBlock(terms);
		}
	}
}
