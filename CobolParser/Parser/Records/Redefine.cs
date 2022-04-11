using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Records
{
	public class Redefine
	{
		public Picture RedefAsPic
		{
			get;
			private set;
		}

		public string RedefinesOffsetNamed
		{
			get;
			private set;
		}

		public Occurances Occures
		{
			get;
			private set;
		}

		public Redefine(Terminalize terms)
		{
			terms.Match("REDEFINES");
			RedefinesOffsetNamed = terms.Current.Str;
			terms.Next();

			while (true)
			{
				if (Picture.IsPictureKeyword(terms))
				{
					RedefAsPic = new Picture(terms);
				}
				else if (terms.Current.Str.Equals("OCCURS", StringComparison.InvariantCultureIgnoreCase))
				{
					Occures = new Occurances(terms);
				}
				else
				{
					return;
				}
			}
		}
	}
}
