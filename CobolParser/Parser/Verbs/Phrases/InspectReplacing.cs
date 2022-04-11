using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Verbs.Phrases
{
	public class InspectReplacing : IInspectOperator
	{
		public List<InspectReplacingItem> Items
		{
			get;
			private set;
		}

		public InspectReplacing(Terminalize terms)
		{
			Items = new List<InspectReplacingItem>();

			terms.Match("REPLACING");

			while 
			(
				terms.Current.Type == StringNodeType.Quoted ||
				terms.CurrentEquals("ALL") ||
				terms.CurrentEquals("LEADING") ||
				terms.CurrentEquals("FIRST")
			)
			{
				InspectReplacingItem item = new InspectReplacingItem(terms);
				Items.Add(item);
				terms.MatchOptional(",");
			}
		}
	}
}
