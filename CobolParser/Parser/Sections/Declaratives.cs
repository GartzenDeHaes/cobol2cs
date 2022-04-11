using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core.Collections;
using CobolParser.Divisions.Proc;

namespace CobolParser.Sections
{
	public class Declaratives : Section
	{
		public Vector<DeclareAfterError> DeclareAfterErrors
		{
			get;
			private set;
		}

		public Declaratives(Terminalize terms)
		: base(terms.Current)
		{
			terms.Match("DECLARATIVES");
			terms.Match(StringNodeType.Period);

			DeclareAfterErrors = new Vector<DeclareAfterError>(2);

			while (! (terms.CurrentEquals("END") && terms.CurrentNextEquals(1, "DECLARATIVES")))
			{
				DeclareAfterErrors.Add(new DeclareAfterError(terms));

				while (VerbLookup.CanCreate(terms.Current, DivisionType.Procedure))
				{
					DeclareAfterErrors.ElementAt(DeclareAfterErrors.Count - 1).Stmts.Stmts.AddRange((new StatementBlock(terms)).Stmts);
					terms.Match(StringNodeType.Period);
				}
			}

			terms.Match("END");
			terms.Match("DECLARATIVES");
			terms.Match(StringNodeType.Period);
		}

		public DeclareAfterError Find(string message)
		{
			for (int x = 0; x < DeclareAfterErrors.Count; x++)
			{
				if (DeclareAfterErrors[x].RecordName.Equals(message, StringComparison.InvariantCultureIgnoreCase))
				{
					return DeclareAfterErrors[x];
				}
			}

			return null;
		}
	}
}
