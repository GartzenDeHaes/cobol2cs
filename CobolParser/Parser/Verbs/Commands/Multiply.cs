using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.Expressions;

namespace CobolParser.Verbs
{
	public class Multiply : IVerb
	{
		public const string Lexum = "MULTIPLY";

		public ITerm Factor1
		{
			get;
			private set;
		}

		public ITerm Factor2
		{
			get;
			private set;
		}

		public ITerm Dest
		{
			get;
			private set;
		}

		public bool IsRounded
		{
			get;
			private set;
		}

		public Multiply(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.Mult;

			terms.Match(Lexum);

			Factor1 = ITerm.Parse(terms);
			terms.Match("BY");
			Factor2 = ITerm.Parse(terms);

			if (terms.CurrentEquals("GIVING"))
			{
				terms.Next();
				Dest = ITerm.Parse(terms);
			}

			if (terms.CurrentEquals("ROUNDED"))
			{
				IsRounded = true;
				terms.Next();
			}

			terms.MatchOptional("END-MULTIPLY", "Expected END-MULTIPLY");
		}
	}
}
