using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CobolParser.Verbs.Phrases;
using CobolParser.Expressions;

namespace CobolParser.Verbs
{
	public class Read : IoVerb
	{
		public const string Lexum = "READ";

		public string FileName
		{
			get;
			private set;
		}

		public bool IsNextRecordMode
		{
			get;
			private set;
		}

		public ITerm ReadInto
		{
			get;
			private set;
		}

		public ITerm KeyIs
		{
			get;
			private set;
		}

		public ITerm PromptWith
		{
			get;
			private set;
		}

		public bool IsReversed
		{
			get;
			private set;
		}

		public Read(Terminalize terms)
		{
			Type = VerbType.Read;

			terms.Match(Lexum);
			FileName = terms.Current.Str;
			terms.Next();

			if (terms.CurrentEquals("REVERSED"))
			{
				IsReversed = true;
				terms.Next();
			}

			while (true)
			{
				if (terms.CurrentEquals("NEXT") || terms.CurrentEquals("RECORD"))
				{
					terms.MatchOptional("NEXT");
					terms.MatchOptional("RECORD");
					IsNextRecordMode = true;
				}
				else if (terms.CurrentEquals("INTO"))
				{
					terms.Next();
					ReadInto = ITerm.Parse(terms);
				}
				else if (terms.CurrentEquals("KEY"))
				{
					terms.Next();
					terms.MatchOptional("IS");
					KeyIs = ITerm.Parse(terms);
				}
				else if (terms.CurrentEquals("LOCK"))
				{
					terms.Next();
				}
				else if (terms.CurrentEquals("WITH"))
				{
					terms.Match("WITH");
					if (terms.CurrentEquals("LOCK"))
					{
						terms.Match("LOCK");
					}
					else
					{
						terms.MatchOptional("PROMPT");
						PromptWith = ITerm.Parse(terms);
					}
				}
				else if (terms.CurrentEquals("END"))
				{
					base.ParseCases(terms);
				}
				else if (terms.CurrentEquals("NOT"))
				{
					base.ParseCases(terms);
				}
				else
				{
					break;
				}
			}

			ParseCases(terms);

			terms.MatchOptional("END-READ", "Expected END-READ");
		}
	}
}
