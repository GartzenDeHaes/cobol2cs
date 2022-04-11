using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CobolParser.Verbs.Phrases;

namespace CobolParser.Verbs
{
	public class Print : OnErrorVerb
	{
		public static string Lexum = "PRINT";

		public Print(Terminalize terms)
		{
			Type = VerbType.Print;

			terms.Match(Lexum);
			terms.Match("SCREEN");

			base.Parse(terms);
		}
	}
}
