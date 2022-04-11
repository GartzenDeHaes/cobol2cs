using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DOR.Core;
using CobolParser.Expressions;

namespace CobolParser.Records
{
	public class Occurances
	{
		public int MinimumTimes
		{
			get;
			private set;
		}

		public int MaximumTimes
		{
			get;
			private set;
		}

		public ITerm DependsOn
		{
			get;
			private set;
		}

		public int Columns
		{
			get;
			private set;
		}

		public int Lines
		{
			get;
			private set;
		}

		public int Skipping
		{
			get;
			private set;
		}

		public Occurances()
		{
			Columns = 1;
			Lines = 1;
			MinimumTimes = 1;
			MaximumTimes = 1;
		}

		public Occurances(Terminalize terms)
		{
			Columns = 1;

			terms.Match("OCCURS");

			terms.MatchOptional("ON");

			if (terms.CurrentEquals("IN"))
			{
				terms.Next();
				Debug.Assert(StringHelper.IsInt(terms.Current.Str));
				if (terms.CurrentNextEquals("COLUMNS"))
				{
					Columns = Int32.Parse(terms.Current.Str);
				}
				terms.Next();
				terms.MatchOptional("COLUMNS");
				if (terms.CurrentEquals("SKIPPING"))
				{
					terms.Match("SKIPPING");
					Skipping = Int32.Parse(terms.Current.Str);
				}
				terms.MatchOptional("OFFSET");
				IExpr.Parse(terms);	//< column list
				terms.MatchOptional("ON");
				if (terms.Current.Type == StringNodeType.Number)
				{
					terms.Match(StringNodeType.Number);
					terms.MatchOptional("LINES");

					if (terms.CurrentEquals("SKIPPING"))
					{
						terms.Next();
						Skipping = Int32.Parse(terms.Current.Str);
						terms.Next();
					}
				}
			}
			else if (StringHelper.IsNumeric(terms.Current.Str))
			{
				if (terms.CurrentNextEquals("LINES"))
				{
					Lines = Int32.Parse(terms.Current.Str);
					terms.Next();
					terms.Match("LINES");
				}
				else
				{
					MinimumTimes = Int32.Parse(terms.Current.Str);
					terms.Next();

					if (terms.Current.Str.Equals("TO", StringComparison.InvariantCultureIgnoreCase))
					{
						terms.Match("TO");
						MaximumTimes = Int32.Parse(terms.Current.Str);
						terms.Next();

						if (terms.CurrentEquals("DEPENDING"))
						{
							terms.Next();
							terms.MatchOptional("ON");
							DependsOn = ITerm.Parse(terms);
						}
					}
					else
					{
						MaximumTimes = MinimumTimes;
					}
				}

				terms.MatchOptional("TIMES");

				if (terms.CurrentEquals("SKIPPING"))
				{
					terms.Next();
					Skipping = Int32.Parse(terms.Current.Str);
					terms.Next();
				}

				if (terms.CurrentEquals("IN"))
				{
					terms.Match("IN");

					if (terms.CurrentNextEquals("COLUMNS"))
					{
						Columns = Int32.Parse(terms.Current.Str);
						terms.Next();
						terms.Match("COLUMNS");
					}
					else
					{
						terms.Next();
					}

					if (terms.CurrentEquals("SKIPPING"))
					{
						terms.Next();
						Skipping = Int32.Parse(terms.Current.Str);
						terms.Next();
					}

					if (terms.CurrentEquals("OFFSET"))
					{
						terms.Next();
						terms.Next();
					}
				}

				if (terms.CurrentEquals("DEPENDING"))
				{
					terms.Match("DEPENDING");
					terms.MatchOptional("ON");
					DependsOn = ITerm.Parse(terms);
				}
			}
			else
			{
				Debug.Fail("HNI");
			}
		}
	}
}
