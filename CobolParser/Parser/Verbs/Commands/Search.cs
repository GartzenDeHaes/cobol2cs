using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.Verbs.Phrases;
using CobolParser.Expressions;

namespace CobolParser.Verbs
{
	public class Search : IoVerb
	{
		public static string Lexum = "SEARCH";

		public List<SearchWhen> Whens
		{
			get;
			private set;
		}

		public string IndexName
		{
			get;
			private set;
		}

		public bool IsNextRecordMode
		{
			get;
			private set;
		}

		public ITerm IntoDest
		{
			get;
			private set;
		}

		/// <summary>
		/// SEARCH  TBL-TABLE-OF-MONTHS  VARYING  TBL-INDX
		/// </summary>
		public ITerm VaryingValue
		{
			get;
			private set;
		}

		/// <summary>
		/// SEARCH ALL WS-TAX-REPRESENT-TABLE
		/// </summary>
		public bool IsAll
		{
			get;
			private set;
		}
		
		public Search(Terminalize terms)
		{
			Type = VerbType.Search;

			Whens = new List<SearchWhen>();

			terms.Match(Lexum);

			if (terms.CurrentEquals("ALL"))
			{
				IsAll = true;
				terms.Next();
			}

			IndexName = terms.Current.Str;
			terms.Next();

			if (terms.CurrentEquals("NEXT"))
			{
				terms.Match("NEXT");
				terms.MatchOptional("RECORD");
				IsNextRecordMode = true;

				if (terms.CurrentEquals("INTO"))
				{
					terms.Match("INTO");
					IntoDest = ITerm.Parse(terms);
				}
			}

			if (terms.CurrentEquals("VARYING"))
			{
				terms.Next();
				VaryingValue = ITerm.Parse(terms);
			}

			base.ParseCases(terms);

			while (terms.CurrentEquals("WHEN"))
			{
				Whens.Add(new SearchWhen(terms));
			}

			terms.MatchOptional("END-SEARCH", "Expected END-SEARCH");
		}
	}
}
