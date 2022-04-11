using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Divisions.Proc
{
	public class DeclareAfterError
	{
		public string RecordName
		{
			get;
			private set;
		}

		public StatementBlock Stmts
		{
			get;
			private set;
		}

		public DeclareAfterError(Terminalize terms)
		{
			terms.Next();
			terms.Match("SECTION");
			terms.Match(StringNodeType.Period);

			if (terms.CurrentEquals("USE"))
			{
				terms.Match("USE");
				if (terms.CurrentEquals("FOR"))
				{
					terms.Match("FOR");
					terms.Match("SCREEN");
					terms.Match("RECOVERY");

					if (terms.CurrentEquals("ON"))
					{
						terms.Match("ON");
						RecordName = terms.Current.Str;
						terms.Next();
					}
					terms.Match(StringNodeType.Period);
				}
				else
				{
					terms.Match("AFTER");
					terms.Match("ERROR");
					terms.Match("PROCEDURE");
					terms.Match("ON");

					RecordName = terms.Current.Str;

					// skip unused para name
					terms.Next();
					terms.Match(StringNodeType.Period);

					terms.Next();
					terms.Match(StringNodeType.Period);
				}
			}
			else
			{
				// skip unused para name
				terms.Next();
				terms.Match(StringNodeType.Period);
			}

			Stmts = new StatementBlock(terms);
			terms.Match(StringNodeType.Period);
		}
	}
}
