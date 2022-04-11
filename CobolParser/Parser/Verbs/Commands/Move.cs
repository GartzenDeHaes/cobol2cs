using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.Verbs.Phrases;
using CobolParser.Expressions;

namespace CobolParser.Verbs
{
	public class Move : IVerb
	{
		public const string Lexum = "MOVE";

		public IExpr Source
		{
			get;
			private set;
		}

		public ValueList Dest
		{
			get;
			private set;
		}

		public Move(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.Move;

			terms.Match("MOVE");

			terms.MatchOptional("CORR");
			terms.MatchOptional("CORRESPONDING");

			Source = IExpr.Parse(terms);

			terms.Match("TO");
			Dest = new ValueList(terms);
		}
	}
}
