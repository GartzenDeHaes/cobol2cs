using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CobolParser.Expressions;

namespace CobolParser.Verbs
{
	public class Reset : IVerb
	{
		public static string Lexum = "RESET";

		public ValueList Field
		{
			get;
			private set;
		}

		public Reset(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.Reset;

			terms.Match(Lexum);
			Field = new ValueList(terms);
		}
	}
}
