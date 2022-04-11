using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Verbs
{
	public class Exit : IVerb
	{
		public const string Lexum = "EXIT";

		public bool IsExitParagraph
		{
			get;
			private set;
		}

		public bool IsWithError
		{
			get;
			private set;
		}

		public Exit(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.Exit;

			terms.Match(Lexum);

			if (terms.CurrentEquals("PARAGRAPH"))
			{
				IsExitParagraph = true;
				terms.Next();
			}

			terms.MatchOptional("PROGRAM");

			if (terms.CurrentEquals("WITH"))
			{
				IsWithError = true;
				terms.Next();
				terms.Match("ERROR");
			}
		}
	}
}
