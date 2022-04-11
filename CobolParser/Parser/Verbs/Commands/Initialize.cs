using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.Expressions;

namespace CobolParser.Verbs
{
	public class Initialize : IVerb
	{
		public const string Lexum = "INITIALIZE";

		public ValueList Terms
		{
			get;
			private set;
		}

		public Initialize(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.Init;

			terms.Match(Lexum);
			Terms = new ValueList(terms);
		}
	}
}
