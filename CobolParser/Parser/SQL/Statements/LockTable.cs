using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.SQL.Conds;

namespace CobolParser.SQL.Statements
{
	public class LockTable : SqlStatement
	{
		public LockTable(SqlLex lex, string sqlText)
		: base(sqlText)
		{
			lex.Match(SqlToken.LOCK);
			lex.Match("TABLE");
			lex.MatchOptional(SqlToken.EQ);
			lex.MatchOptional(SqlToken.ID);
			lex.MatchOptional("IN");
			lex.MatchOptional("EXCLUSIVE");
			lex.MatchOptional("MODE");
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
