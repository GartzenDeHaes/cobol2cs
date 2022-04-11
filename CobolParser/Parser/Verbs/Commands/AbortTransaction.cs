using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Verbs
{
	public class AbortTransaction : IVerb
	{
		public static string Lexum = "ABORT-TRANSACTION";

		public AbortTransaction(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.AbortTrans;
			terms.Match(Lexum);
		}
	}
}
