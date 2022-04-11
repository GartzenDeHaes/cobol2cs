using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.SQL.Conds;

namespace CobolParser.SQL.Statements
{
	public class BeginDeclareSection : SqlStatement
	{
		public BeginDeclareSection(SqlLex lex, string sqlText)
		: base(sqlText)
		{
			lex.Match(SqlToken.BEGIN);

			if (lex.Lexum.StrEquals("WORK"))
			{
				lex.Next();
			}
			else
			{
				lex.Match(SqlToken.DECLARE);
				lex.Match(SqlToken.SECTION);
				lex.Match(SqlToken.SEMI);
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
