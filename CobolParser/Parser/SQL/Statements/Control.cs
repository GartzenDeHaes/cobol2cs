using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CobolParser.SQL.Conds;

namespace CobolParser.SQL.Statements
{
	public class Control : SqlStatement
	{
		public Control(SqlLex lex, string sqlText)
		: base(sqlText)
		{
			lex.Match(SqlToken.CONTROL);

			if (lex.Lexum.StrEquals("QUERY"))
			{
				lex.Match("QUERY");
				lex.MatchOptional("INTERACTIVE");
				lex.MatchOptional("ACCESS");
				lex.MatchOptional("MDAM");
				lex.MatchOptional("OFF"); 
				lex.MatchOptional("ON");
			}
			else if (lex.Lexum.StrEquals("TABLE"))
			{
				lex.Match("TABLE");
				lex.Next();
				lex.MatchOptional("TABLELOCK");
				lex.MatchOptional("OFF");
				lex.MatchOptional("ON");
			}
			else if (lex.Lexum.StrEquals("EXECUTOR"))
			{
				lex.Match("EXECUTOR");
				lex.Match("PARALLEL");
				lex.Match("EXECUTION");
				lex.Match("ON");
			}
			else
			{
				Debug.Fail("asdfa");
			}
		}

		public override bool HasParameterOf(string fqname)
		{
			return false;
		}

		public override List<CondParam> GetParameters()
		{
			List<CondParam> prms = new List<CondParam>();
			return prms;
		}
	}
}
