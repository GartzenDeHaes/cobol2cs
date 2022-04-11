using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Sections
{
	public class MessageSection : Section
	{
		public MessageSection(Terminalize terms)
		: base(terms.Current)
		{
			terms.Match("MESSAGE");
			terms.Match("SECTION");
			terms.Match(StringNodeType.Period);

			///TODO
			while 
			(
				!terms.CurrentNextEquals(1, "SECTION") &&
				!terms.CurrentNextEquals(1, "DIVISION")
			)
			{
				terms.Next();
			}
		}
	}
}
