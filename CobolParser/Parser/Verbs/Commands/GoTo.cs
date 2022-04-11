using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Verbs
{
	public class GoTo : IVerb
	{
		public const string Lexum = "GO";

		public string Target
		{
			get;
			private set;
		}

		public GoTo(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.GoTo;

			terms.Match("GO");
			terms.Match("TO");

			Target = terms.Current.Str;
			terms.Next();
		}
	}
}
