using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.Expressions;

namespace CobolParser.Verbs.Phrases
{
	public class InspectConverting : IInspectOperator
	{
		public ITerm From
		{
			get;
			private set;
		}

		public ITerm To
		{
			get;
			private set;
		}

		public InspectConverting(Terminalize terms)
		{
			terms.Match("CONVERTING");
			From = ITerm.Parse(terms);
			terms.Match("TO");
			To = ITerm.Parse(terms);
		}
	}
}
