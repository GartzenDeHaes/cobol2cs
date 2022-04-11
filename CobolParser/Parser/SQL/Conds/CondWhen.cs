using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.SQL.Conds
{
	public class CondWhen
	{
		public SqlExpr Cond
		{
			get;
			private set;
		}

		public SqlExpr Result
		{
			get;
			private set;
		}

		public CondWhen(SqlLex lex)
		{
			lex.Match(SqlToken.WHEN);
			Cond = new SqlExpr(lex, true, false, false);
			lex.Match(SqlToken.THEN);
			Result = new SqlExpr(lex, true, false, false);
		}
	}
}
