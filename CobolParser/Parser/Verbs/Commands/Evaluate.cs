using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core.Collections;
using CobolParser.Expressions;
using CobolParser.Verbs.Phrases;
using System.Diagnostics;

namespace CobolParser.Verbs
{
	public class Evaluate : IVerb
	{
		public const string Lexum = "EVALUATE";

		public Vector<IExpr> Conditions
		{
			get;
			private set;
		}

		public Vector<EvalWhen> Whens
		{
			get;
			private set;
		}

		public Evaluate(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.Eval;

			Conditions = new Vector<IExpr>(3);
			Whens = new Vector<EvalWhen>(3);

			terms.Match(Lexum);

			while (true)
			{
				Conditions.Add(IExpr.Parse(terms));

				if (terms.CurrentEquals("ALSO"))
				{
					terms.Next();
				}
				else
				{
					break;
				}
			}

			Debug.Assert(Conditions.Count > 0);

			while (terms.CurrentEquals("WHEN"))
			{
				Whens.Add(new EvalWhen(terms));
			}

			terms.MatchOptional("END-EVALUATE");
		}
	}
}
