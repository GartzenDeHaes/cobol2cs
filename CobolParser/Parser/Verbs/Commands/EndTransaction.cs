using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Verbs
{
	public class EndTransaction : IVerb
	{
		public static string Lexum = "END-TRANSACTION";

		public EndTransaction(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.EndTrans;

			terms.Match(Lexum);
		}
	}
}
