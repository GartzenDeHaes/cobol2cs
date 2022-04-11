using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CobolParser.Expressions;
using CobolParser.Verbs.Phrases;

namespace CobolParser.Verbs
{
	public class Add : OnErrorVerb
	{
		public const string Lexum = "ADD";

		public ValueList Term1
		{
			get;
			private set;
		}

		public ValueList AddToThis
		{
			get;
			private set;
		}

		public ITerm GivingTo
		{
			get;
			private set;
		}

		public Add(Terminalize terms)
		{
			Type = VerbType.Add;

			terms.Match(Lexum);
			Term1 = new ValueList(terms);

			terms.MatchOptional("TO");

			AddToThis = new ValueList(terms);

			if (terms.CurrentEquals("GIVING"))
			{
				terms.Match("GIVING");
				GivingTo = ITerm.Parse(terms);
			}

			base.Parse(terms);

			terms.MatchOptional("END-ADD");
		}
	}
}
