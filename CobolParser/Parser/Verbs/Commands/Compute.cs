using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CobolParser.Expressions;
using CobolParser.Verbs.Phrases;

namespace CobolParser.Verbs
{
	public class Compute : OnErrorVerb
	{
		public const string Lexum = "COMPUTE";

		public ITerm LValue
		{
			get;
			private set;
		}

		public IExpr RValue
		{
			get;
			private set;
		}

		public bool IsRounded
		{
			get;
			private set;
		}

		public Compute(Terminalize terms)
		{
			Type = VerbType.Compute;

			terms.Match(Lexum);
			LValue = ITerm.Parse(terms);

			if (terms.CurrentEquals("ROUNDED"))
			{
				IsRounded = true;
				terms.Next();
			}

			terms.Match(StringNodeType.Eq);

			RValue = IExpr.Parse(terms);

			base.Parse(terms);

			terms.MatchOptional("END-COMPUTE");
		}
	}
}
