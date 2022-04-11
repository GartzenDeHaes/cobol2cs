using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Verbs.Phrases
{
	public class OnErrorVerb : IVerb
	{
		public StatementBlock Stmts
		{
			get;
			private set;
		}

		public OnErrorVerb()
		: base(null)
		{
		}

		public void Parse(Terminalize terms)
		{
			VerbLexum = terms.Current;

			if (! terms.CurrentEquals("ON"))
			{
				return;
			}

			terms.Match("ON");
			terms.MatchOptional("SIZE");
			terms.Match("ERROR");

			Stmts = new StatementBlock(terms);
		}
	}
}
