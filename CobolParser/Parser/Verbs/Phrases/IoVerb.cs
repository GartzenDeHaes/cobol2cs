using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Verbs.Phrases
{
	public class IoVerb : IVerb
	{
		public StatementBlock AtEndStmts
		{
			get;
			private set;
		}

		public StatementBlock NotAtEndStmts
		{
			get;
			private set;
		}

		public StatementBlock InvalidKeyStmts
		{
			get;
			private set;
		}

		public StatementBlock NotInvalidKeyStmts
		{
			get;
			private set;
		}

		protected IoVerb()
		: base(null)
		{
		}

		protected void ParseCases(Terminalize terms)
		{
			VerbLexum = terms.Current;

			while(true)
			{
				bool isNot = false;

				terms.MatchOptional("AT");

				if (terms.CurrentEquals("NOT"))
				{
					isNot = true;
					terms.Next();
				}

				terms.MatchOptional("AT");

				if (terms.CurrentEquals("END"))
				{
					terms.Match("END");
					if (isNot)
					{
						NotAtEndStmts = new StatementBlock(terms);
					}
					else
					{
						AtEndStmts = new StatementBlock(terms);
					}
				}
				else if (terms.CurrentEquals("INVALID"))
				{
					terms.Match("INVALID");
					terms.MatchOptional("KEY");
					if (isNot)
					{
						NotInvalidKeyStmts = new StatementBlock(terms);
					}
					else
					{
						InvalidKeyStmts = new StatementBlock(terms);
					}
				}
				else if (terms.CurrentEquals("GENERIC"))
				{
					terms.Next();
				}
				else
				{
					break;
				}
			}
		}
	}
}
