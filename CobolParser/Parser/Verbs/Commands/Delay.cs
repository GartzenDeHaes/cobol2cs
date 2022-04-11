using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Verbs
{
	public class Delay : IVerb
	{
		public static string Lexum = "DELAY";

		public string Length
		{
			get;
			private set;
		}

		public Delay(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.Delay;

			terms.Match(Lexum);
			Length = terms.Current.Str;
			terms.Next();
		}
	}
}
