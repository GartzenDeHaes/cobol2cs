using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using CobolParser.Records;
using DOR.Core;
using CobolParser.Parser;

namespace CobolParser.Sections
{
	public class WorkingStorageSection : Section
	{
		public Storage Data
		{
			get;
			private set;
		}

		public WorkingStorageSection(Terminalize terms, Storage data)
		: base(terms.Current) 
		{
			Data = data;

			terms.Match("WORKING-STORAGE");
			terms.Match("SECTION");
			terms.Match(StringNodeType.Period);
		}

		public void Parse(Terminalize terms)
		{
			while (StringHelper.IsInt(terms.Current.Str) || terms.Current.StrEquals("DEF"))
			{
				Data.AddOne(terms, "");
			}
		}
	}
}
