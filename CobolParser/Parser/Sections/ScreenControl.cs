using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.Expressions;

namespace CobolParser.Sections
{
	public class ScreenControl
	{
		public ITerm ErrorEnhancement
		{
			get;
			private set;
		}

		public ScreenControl(Terminalize terms)
		{
			terms.Match("SCREEN-CONTROL");
			terms.Match(StringNodeType.Period);

			if (terms.CurrentEquals("ERROR-ENHANCEMENT"))
			{
				terms.Next();
				terms.MatchOptional("IS");
				ErrorEnhancement = ITerm.Parse(terms);
			}
		}
	}
}
