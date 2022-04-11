using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core.Collections;
using CobolParser.Sections;
using System.Diagnostics;
using CobolParser.Parser;

namespace CobolParser.Division
{
	public class EnvironmentDiv : IDivision
	{
		public Configuration ConfigSection
		{
			get;
			private set;
		}

		public InputOutput InputOutputSection
		{
			get;
			private set;
		}

		public EnvironmentDiv()
		: base(DivisionType.Environment)
		{
		}

		public override void Parse(Terminalize terms)
		{
			terms.Match("ENVIRONMENT");
			terms.Match("DIVISION");
			terms.Match(StringNodeType.Period);

			if (terms.CurrentEquals("CONFIGURATION"))
			{
				ConfigSection = new Configuration(terms);
			}

			if (terms.CurrentEquals("INPUT-OUTPUT"))
			{
				InputOutputSection = new InputOutput(terms);
			}

			Debug.Assert(terms.CurrentEquals("DATA"));
		}
	}
}
