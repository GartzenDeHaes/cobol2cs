using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.SQL.Conds;

namespace CobolParser.SQL.Statements
{
	public class CommitTrans : SqlStatement
	{
		public CommitTrans(SqlLex lex, string sqlText)
		: base(sqlText)
		{
			lex.Match(SqlToken.COMMIT);
			lex.Next();
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
