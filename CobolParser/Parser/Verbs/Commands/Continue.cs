using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Verbs
{
	public class Continue : IVerb
	{
		public const string Lexum = "CONTINUE";

		public Continue(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.Continue;

			terms.Match("CONTINUE");
		}
	}
}
