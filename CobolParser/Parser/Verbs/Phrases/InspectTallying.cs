using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Verbs.Phrases
{
	public class InspectTallying : IInspectOperator
	{
		public List<InspectTallyingItem> Items
		{
			get;
			private set;
		}

		public InspectTallying(Terminalize terms)
		{
			Items = new List<InspectTallyingItem>();

			terms.Match("TALLYING");

			while 
			(
				!VerbLookup.CanCreate(terms.Current, DivisionType.Procedure) &&
				!terms.CurrentEquals("REPLACING") &&
				!terms.CurrentEquals("CONVERTING")
			)
			{
				Items.Add(new InspectTallyingItem(terms));
				terms.MatchOptional(",");
			}
		}
	}
}
