using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Verbs
{
	public class BeginTransaction : IVerb
	{
		public static string Lexum = "BEGIN-TRANSACTION";

		public BeginTransaction(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.BeginTrans;

			terms.Match(Lexum);
		}
	}
}
