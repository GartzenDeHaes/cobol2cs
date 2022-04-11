using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core.Collections;

namespace CobolParser.Sections
{
	public class Configuration : Section
	{
		public SpecialNames Aliases
		{
			get;
			private set;
		}

		public Configuration(Terminalize terms)
		: base(terms.Current)
		{
			Vector<StringNode> sentence = new Vector<StringNode>();

			terms.Match("CONFIGURATION");
			terms.Match("SECTION");
			terms.Match(StringNodeType.Period);

			if (terms.CurrentEquals("SOURCE-COMPUTER"))
			{
				terms.Match("SOURCE-COMPUTER");
				terms.Match(StringNodeType.Period);
				terms.ReadSentence(sentence);
			}

			if (terms.CurrentEquals("OBJECT-COMPUTER"))
			{
				terms.Match("OBJECT-COMPUTER");
				terms.Match(StringNodeType.Period);
				terms.ReadSentence(sentence);
			}

			if (terms.CurrentEquals("SPECIAL-NAMES"))
			{
				Aliases = new SpecialNames(terms);
			}
		}
	}
}
