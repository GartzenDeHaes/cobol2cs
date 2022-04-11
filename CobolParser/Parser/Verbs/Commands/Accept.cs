using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.Expressions;
using CobolParser.Parser.Verbs.Phrases;

namespace CobolParser.Verbs
{
	public class Accept : IVerb
	{
		public const string Lexum = "ACCEPT";

		public ValueList Arg1
		{
			get;
			private set;
		}

		public string Action
		{
			get;
			private set;
		}

		public ITerm FromTarget
		{
			get;
			private set;
		}

		public List<AcceptUntilItem> UntilList
		{
			get;
			private set;
		}

		public IExpr UntilExpr
		{
			get;
			private set;
		}

		public Accept(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.Accept;
			UntilList= new List<AcceptUntilItem>();

			terms.Match(Lexum);

			Arg1 = new ValueList(terms);

			Action = terms.Current.Str;
			if (terms.CurrentEquals("FROM"))
			{
				terms.Match("FROM");
				FromTarget = ITerm.Parse(terms);
			}
			else if (terms.CurrentEquals("UNTIL"))
			{
				terms.Match("UNTIL");

				while 
				(
					terms.Current.Type != StringNodeType.Period &&
					! terms.CurrentEquals("ELSE")
				)
				{
					terms.MatchOptional("ESCAPE");
					terms.MatchOptional("ON");

					if (terms.CurrentEquals("TIMEOUT"))
					{
						terms.Match("TIMEOUT");
						terms.Match(StringNodeType.Number);
						continue;
					}

					UntilList.Add(new AcceptUntilItem(terms));
				}
			}
		}
	}
}
