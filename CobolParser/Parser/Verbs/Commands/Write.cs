using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CobolParser.Verbs.Phrases;
using CobolParser.Expressions;

namespace CobolParser.Verbs
{
	public class Write : IoVerb
	{
		public const string Lexum = "WRITE";

		public ITerm FileName
		{
			get;
			private set;
		}

		public ITerm FromVar
		{
			get;
			private set;
		}

		public ITerm AfterAdvancing
		{
			get;
			private set;
		}

		public string BeforeThis
		{
			get;
			private set;
		}

		public Write(Terminalize terms)
		{
			Type = VerbType.Write;

			terms.Match(Lexum);

			FileName = ITerm.Parse(terms);

			if (terms.CurrentEquals("FROM"))
			{
				terms.Next();
				FromVar = ITerm.Parse(terms);
			}

			if (terms.CurrentEquals("AFTER"))
			{
				terms.Next();
				terms.MatchOptional("ADVANCING");
				AfterAdvancing = ITerm.Parse(terms);
				terms.MatchOptional("LINES");
			}

			if (terms.CurrentEquals("BEFORE"))
			{
				terms.Next();
				BeforeThis = terms.Current.Str;
				terms.Next();
			}

			ParseCases(terms);

			terms.MatchOptional("END-WRITE", "Expected END-WRITE");
		}
	}
}
