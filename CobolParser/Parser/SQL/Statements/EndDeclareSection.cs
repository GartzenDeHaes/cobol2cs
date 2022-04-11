using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.SQL.Conds;

namespace CobolParser.SQL.Statements
{
	public class EndDeclareSection : SqlStatement
	{
		public EndDeclareSection(SqlLex lex, string sqlText)
		: base(sqlText)
		{
			lex.Match(SqlToken.END);
			lex.Match(SqlToken.DECLARE);
			lex.Match(SqlToken.SECTION);
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
