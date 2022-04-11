using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CobolParser.Expressions;

namespace CobolParser.Verbs.Phrases
{
	public class PerformOneOf : IPerformInner
	{
		public ValueList Options
		{
			get;
			private set;
		}

		public ITerm DependingOn
		{
			get;
			private set;
		}

		public PerformOneOf(Terminalize terms)
		{
			terms.Match("ONE");
			terms.Match("OF");
			Options = new ValueList(terms);
			terms.Match("DEPENDING");
			terms.Match("ON");
			DependingOn = ITerm.Parse(terms);
		}
	}
}
