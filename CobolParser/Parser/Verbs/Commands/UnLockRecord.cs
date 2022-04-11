using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Verbs
{
	public class UnLockRecord : IVerb
	{
		public static string Lexum = "UNLOCKRECORD";

		public string FileName
		{
			get;
			private set;
		}

		public UnLockRecord(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.Unlock;

			terms.Match(Lexum);

			FileName = terms.Current.Str;
			terms.Next();
		}
	}
}
