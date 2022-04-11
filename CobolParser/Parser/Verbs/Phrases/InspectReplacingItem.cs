using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.Expressions;

namespace CobolParser.Verbs.Phrases
{
	public class InspectReplacingItem
	{
		public bool IsAll
		{
			get;
			set;
		}

		public bool IsLeading
		{
			get;
			set;
		}

		public bool IsFirst
		{
			get;
			set;
		}

		public ITerm FromText
		{
			get;
			set;
		}

		public ITerm ToText
		{
			get;
			set;
		}

		public InspectReplacingItem(Terminalize terms)
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
			else if (terms.CurrentEquals("FIRST"))
			{
				IsFirst = true;
				terms.Next();
			}

			FromText = ITerm.Parse(terms);
			terms.Match("BY");
			ToText = ITerm.Parse(terms);
		}
	}
}
