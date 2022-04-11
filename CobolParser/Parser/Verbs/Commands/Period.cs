using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Verbs
{
	/// <summary>
	/// Target of NEXT SENTENCE
	/// </summary>
	public class PeriodVerb : IVerb
	{
		public const string Lexum = ".";

		public PeriodVerb(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.Period;

			terms.Match(StringNodeType.Period);
		}
	}
}
