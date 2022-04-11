using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.Expressions;

namespace CobolParser.Verbs.Phrases
{
	public class StringItem
	{
		public ITerm Text
		{
			get;
			private set;
		}

		public string IsDelimitedBy
		{
			get;
			private set;
		}

		public StringItem(Terminalize terms)
		{
			Text = ITerm.Parse(terms);

			if (terms.CurrentEquals("DELIMITED"))
			{
				terms.Match("DELIMITED");
				terms.MatchOptional("BY");

				IsDelimitedBy = terms.Current.Str;
				terms.Next();

				terms.MatchOptional(",");
			}
		}
	}
}
