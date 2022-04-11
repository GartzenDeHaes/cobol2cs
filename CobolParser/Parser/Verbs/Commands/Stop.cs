using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Verbs
{
	public class Stop : IVerb
	{
		public const string Lexum = "STOP";

		public int LineNumber
		{
			get;
			private set;
		}

		public Stop(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.Stop;
			LineNumber = terms.Current.LineNumber;
			terms.Match("STOP");
			terms.Match("RUN");
		}
	}
}
