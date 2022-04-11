using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.Verbs.Phrases;

namespace CobolParser.Verbs
{
	public class Delete : IoVerb
	{
		public const string Lexum = "DELETE";

		public string FileName
		{
			get;
			private set;
		}

		public bool IsRecord
		{
			get;
			private set;
		}

		public Delete(Terminalize terms)
		{
			Type = VerbType.Delete;

			terms.Match(Lexum);

			FileName = terms.Current.Str;
			terms.Next();

			if (terms.CurrentEquals("RECORD"))
			{
				IsRecord = true;
				terms.Next();
			}

			ParseCases(terms);
		}
	}
}
