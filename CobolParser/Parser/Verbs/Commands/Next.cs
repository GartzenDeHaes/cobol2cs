using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Verbs
{
	public class Next : IVerb
	{
		public const string Lexum = "NEXT";

		public Next(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.Next;

			terms.Match(Lexum);
			terms.Match("SENTENCE");
		}
	}
}
