using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CobolParser.Expressions;
using DOR.Core.Collections;

namespace CobolParser.Verbs
{
	public class Display : IVerb
	{
		public const string Lexum = "DISPLAY";

		public Vector<ITerm> Terms
		{
			get;
			private set;
		}

		public Display(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.Display;

			Terms = new Vector<ITerm>(3);

			terms.Match(Lexum);
			terms.MatchOptional("BASE");

			Terms.Add(ITerm.Parse(terms));

			while (! VerbLookup.CanCreate(terms.Current, DivisionType.Procedure))
			{
				if (terms.Current.Str.StartsWith("END", StringComparison.InvariantCultureIgnoreCase))
				{
					break;
				}
				terms.MatchOptional(",");
				Terms.Add(ITerm.Parse(terms));
			}
		}
	}
}
