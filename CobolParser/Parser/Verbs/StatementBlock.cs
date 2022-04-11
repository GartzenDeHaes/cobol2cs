using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DOR.Core.Collections;

namespace CobolParser
{
	public class StatementBlock
	{
		public Vector<IVerb> Stmts
		{
			get;
			private set;
		}

		public StatementBlock(Terminalize terms)
		{
			Stmts = new Vector<IVerb>();

			while 
			(
				//terms.CurrentEquals("END-TRANSACTION") ||
				(terms.Current.Type != StringNodeType.Period && 
				!terms.Current.StrEquals("END-EVALUATE") &&
				!terms.Current.StrEquals("END-IF") &&
				!terms.Current.StrEquals("END-PERFORM") &&
				!terms.Current.StrEquals("END-SEARCH") &&
				!terms.Current.StrEquals("END-STRING") &&
				!terms.Current.StrEquals("END-UNSTRING") &&
				!terms.Current.StrEquals("END-INSPECT") &&
				!terms.Current.StrEquals("END-READ") &&
				!terms.Current.StrEquals("END-WRITE") &&
				!terms.CurrentEquals("WHEN") &&
				!terms.CurrentEquals("ELSE") &&
				!terms.CurrentEquals("END") &&
				!terms.CurrentEquals("NOT"))
			)
			{
				IVerb verb = VerbLookup.Create(terms, DivisionType.Procedure);
				if (null == verb)
				{
					throw new SyntaxError(terms.Current.FileName, terms.Current.LineNumber, "Unknown verb of " + terms.Current.Str);
				}

				Stmts.Add(verb);
			}
		}
	}
}
