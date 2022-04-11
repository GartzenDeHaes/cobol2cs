using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.Expressions;

namespace CobolParser.Verbs
{
	public class Divide : IVerb
	{
		public const string Lexum = "DIVIDE";

		public ITerm Denominator
		{
			get;
			private set;
		}

		public ITerm Numerator
		{
			get;
			private set;
		}

		public ITerm Dest
		{
			get;
			private set;
		}

		public ITerm DestRemainder
		{
			get;
			private set;
		}

		public bool IsRounded
		{
			get;
			private set;
		}

		public Divide(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.Divide;

			terms.Match(Lexum);

			bool intoFormat = false;
			ITerm term = ITerm.Parse(terms);
			if (terms.CurrentEquals("BY"))
			{
				Numerator = term;
				terms.Match("BY");
				Denominator = ITerm.Parse(terms);
			}
			else
			{
				Denominator = term;
				terms.Match("INTO");
				Numerator = ITerm.Parse(terms);
				intoFormat = true;
			}

			if (terms.CurrentEquals("GIVING"))
			{
				terms.Next();
				Dest = ITerm.Parse(terms);

				if (terms.CurrentEquals("REMAINDER"))
				{
					terms.Next();
					DestRemainder = ITerm.Parse(terms);
				}
			}
			else
			{
				if (intoFormat)
				{
					Dest = Numerator;
				}
				else
				{
					Dest = Denominator;
				}
			}

			if (terms.CurrentEquals("ROUNDED"))
			{
				IsRounded = true;
				terms.Next();
			}

			terms.MatchOptional("END-DIVIDE");
		}	
	}
}
