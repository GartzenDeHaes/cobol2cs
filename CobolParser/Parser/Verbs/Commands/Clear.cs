using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Verbs
{
	public class Clear : IVerb
	{
		public static string Lexum = "CLEAR";

		public Clear(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.Clear;

			terms.Match(Lexum);
			terms.Match("INPUT");
		}
	}
}
