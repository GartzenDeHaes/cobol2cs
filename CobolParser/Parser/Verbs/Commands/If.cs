using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CobolParser.Verbs
{
	public class If : IVerb
	{
		public const string Lexum = "IF";

		public IExpr Condition
		{
			get;
			private set;
		}

		public StatementBlock Stmts
		{
			get;
			private set;
		}

		public StatementBlock ElseStmts
		{
			get;
			private set;
		}

		public If(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.If;

			terms.Match(Lexum);

			Condition = IExpr.Parse(terms);

			terms.MatchOptional("THEN");

			Stmts = new StatementBlock(terms);

			if (terms.CurrentEquals("ELSE"))
			{
				terms.Match("ELSE");
				ElseStmts = new StatementBlock(terms);
			}

			terms.MatchOptional("END-IF", "Expected END-IF");
		}
	}
}
