using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CobolParser.Verbs.Phrases;
using CobolParser.Expressions;

namespace CobolParser.Verbs
{
	public class Start : IoVerb
	{
		public static string Lexum = "START";

		public ITerm StartingThis
		{
			get;
			private set;
		}

		public ITerm Position
		{
			get;
			private set;
		}

		public IExpr KeyIs
		{
			get;
			private set;
		}

		public bool IsApproximate
		{
			get;
			private set;
		}

		public Start(Terminalize terms)
		{
			Type = VerbType.Start;

			terms.Match(Lexum);

			StartingThis = ITerm.Parse(terms);

			KeyIs = IExpr.Parse(terms);

			terms.MatchOptional("AFTER");

			if (terms.CurrentEquals("POSITION"))
			{
				terms.Next();
				Position = ITerm.Parse(terms);
			}

			if (terms.CurrentEquals("APPROXIMATE"))
			{
				IsApproximate = true;
				terms.Next();
			}

			terms.MatchOptional("GENERIC");
			
			base.ParseCases(terms);

			terms.MatchOptional("END-START", "Expected END-START");
		}
	}
}
