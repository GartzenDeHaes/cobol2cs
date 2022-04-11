using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Sections
{
	public class InputOutput : Section
	{
		public FileControl FileControlSection
		{
			get;
			private set;
		}

		public ScreenControl ScreenControlSection
		{
			get;
			private set;
		}

		public InputOutput(Terminalize terms)
		: base(terms.Current)
		{
			terms.Match("INPUT-OUTPUT");
			terms.Match("SECTION");
			terms.Match(StringNodeType.Period);

			if (terms.CurrentEquals("FILE-CONTROL"))
			{
				FileControlSection = new FileControl(terms);
			}

			if (terms.CurrentEquals("SCREEN-CONTROL"))
			{
				ScreenControlSection = new ScreenControl(terms);
			}
		}
	}
}
