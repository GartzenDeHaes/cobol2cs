using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using CobolParser.Expressions;

namespace CobolParser.Verbs.Phrases
{
	public class InspectTallyingItem
	{
		public ITerm CounterVariable
		{
			get;
			private set;
		}

		public string BeforeInitial
		{
			get;
			private set;
		}

		public bool IsAll
		{
			get;
			private set;
		}

		public bool IsLeading
		{
			get;
			private set;
		}

		public string MatchString
		{
			get;
			private set;
		}

		public InspectTallyingItem(Terminalize terms)
		{
			CounterVariable = ITerm.Parse(terms);

			terms.MatchOptional("FOR");
			terms.MatchOptional("CHARACTERS");

			if (terms.CurrentEquals("BEFORE"))
			{
				terms.Next();
				terms.MatchOptional("INITIAL");
				BeforeInitial = terms.Current.Str;
				terms.Next();
			}
			else
			{
				if (terms.CurrentEquals("ALL"))
				{
					IsAll = true;
					terms.Next();
				}
				else if (terms.CurrentEquals("LEADING"))
				{
					IsLeading = true;
					terms.Next();
				}
				else
				{
					Debug.Fail("Interanl error");
				}

				MatchString = terms.Current.Str;
				terms.Next();
			}
		}
	}
}
