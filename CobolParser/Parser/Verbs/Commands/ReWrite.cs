using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CobolParser.Verbs.Phrases;
using CobolParser.Expressions;

namespace CobolParser.Verbs
{
	public class ReWrite : IoVerb
	{
		public static string Lexum = "REWRITE";

		public ITerm FileName
		{
			get;
			private set;
		}

		public ITerm FromThis
		{
			get;
			private set;
		}

		public ReWrite(Terminalize terms)
		{
			Type = VerbType.Rewrite;

			terms.Match(Lexum);

			FileName = ITerm.Parse(terms);

			if (terms.CurrentEquals("FROM"))
			{
				terms.Next();
				FromThis = ITerm.Parse(terms);
			}

			terms.MatchOptional("WITH");
			terms.MatchOptional("UNLOCK");

			base.ParseCases(terms);

			terms.MatchOptional("END-REWRITE", "Expected END-REWRITE");
		}
	}
}
