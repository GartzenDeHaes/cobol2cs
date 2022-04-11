using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.Expressions;

namespace CobolParser.Parser.Verbs.Phrases
{
	public class UnstringItem
	{
		public ITerm Dest
		{
			get;
			private set;
		}

		public ITerm DelimiterDest
		{
			get;
			private set;
		}

		public UnstringItem(Terminalize terms)
		{
			Dest = ITerm.Parse(terms);

			if (terms.CurrentEquals("DELIMITER"))
			{
				terms.Match("DELIMITER");
				terms.MatchOptional("IN");

				DelimiterDest = ITerm.Parse(terms);
			}
		}
	}
}
