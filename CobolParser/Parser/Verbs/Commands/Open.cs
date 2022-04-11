using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.Expressions;

namespace CobolParser.Verbs
{
	public class Open : IVerb
	{
		public const string Lexum = "OPEN";

		public ValueList OutputFiles
		{
			get;
			private set;
		}

		public ValueList InputFiles
		{
			get;
			private set;
		}

		public bool IsExtendMode
		{
			get;
			private set;
		}

		public Open(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.Open;

			terms.Match(Lexum);

			while(true)
			{
				if (terms.CurrentEquals("INPUT"))
				{
					terms.Match("INPUT");
					InputFiles = new ValueList(terms);
				}
				else if (terms.CurrentEquals("OUTPUT") || terms.CurrentEquals("EXTEND"))
				{
					terms.MatchOptional("OUTPUT");
					if (terms.CurrentEquals("EXTEND"))
					{
						terms.Match("EXTEND");
						IsExtendMode = true;
					}
					OutputFiles = new ValueList(terms);
				}
				else if (terms.CurrentEquals("I-O"))
				{
					terms.Match("I-O");
					InputFiles = new ValueList(terms);
					OutputFiles = (ValueList)InputFiles.Clone();
				}
				else
				{
					break;
				}
				if (terms.CurrentEquals("SYNC"))
				{
					terms.Match("SYNC");
					terms.Match("DEPTH");
				}
			}
		}
	}
}
