using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.Expressions;
using CobolParser.Expressions.Terms;

namespace CobolParser.Sections
{
	public class SpecialNames : Section
	{
		public Dictionary<string, IExpr> Map
		{
			get;
			private set;
		}

		public List<IVerb> Copies
		{
			get;
			private set;
		}

		public SpecialNames(Terminalize terms)
		: base(terms.Current)
		{
			Map = new Dictionary<string, IExpr>();
			Copies = new List<IVerb>();

			terms.Match("SPECIAL-NAMES");
			terms.Match(StringNodeType.Period);

			while 
			(
				! terms.CurrentNextEquals(1, "SECTION") &&
				! terms.CurrentNextEquals(1, "DIVISION")
			)
			{
				if (terms.CurrentEquals("COPY"))
				{
					Copies.Add(VerbLookup.Create(terms, DivisionType.Environment));
					terms.MatchOptional(".");
					continue;
				}

				bool isFile = terms.CurrentEquals("FILE");
				terms.MatchOptional("FILE");

				string key = terms.Current.Str;
				terms.Next();
				terms.Match("IS");

				if (terms.Current.Type == StringNodeType.LPar)
				{
					Map.Add(key, IExpr.Parse(terms));
				}
				else
				{
					if (isFile)
					{
						Map.Add(terms.Current.Str, new Expr(new StringLit(key)));
						terms.Next();
					}
					else
					{
						Map.Add(key, new Expr(ITerm.Parse(terms)));
					}
				}

				terms.MatchOptional(",");
				terms.MatchOptional(".");
			}
		}
	}
}
