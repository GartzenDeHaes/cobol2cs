using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CobolParser.SQL.Conds
{
	public class CastCond : ISqlCondToken
	{
		public SqlExpr Field
		{
			get;
			private set;
		}

		public SqlType NewType
		{
			get;
			private set;
		}

		public CastCond(SqlLex lex)
		{
			lex.Match(SqlToken.CAST);
			lex.Match(SqlToken.LPAR);

			Field = new SqlExpr(lex, true, false, false);

			if (lex.Token == SqlToken.AS)
			{
				lex.Match(SqlToken.AS);

				NewType = new SqlType(lex);
			}
			else
			{
				Debug.Assert(Field.Terms[0] is CondParam);
			}

			lex.MatchOptional("SIGNED");

			lex.Match(SqlToken.RPAR);
		}

		public override void GetParameters(List<CondParam> lst)
		{
			Field.GetParameters(lst);
		}

		public override void ListTables(List<string> lst)
		{
			Field.ListTables(lst);
		}
	}
}
