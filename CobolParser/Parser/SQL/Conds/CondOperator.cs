using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CobolParser.SQL.Conds
{
	public class CondOperator : ISqlCondToken
	{
		public SqlToken Op
		{
			get;
			private set;
		}

		public string Lexum
		{
			get;
			private set;
		}

		public CondOperator(SqlLex lex)
		{
			Debug.Assert(IsOperator(lex.Token));
			Op = lex.Token;
			Lexum = lex.Lexum;
			lex.Match(Op);
		}

		public static bool IsOperator(SqlToken token)
		{
			return token == SqlToken.AND ||
				token == SqlToken.EQ ||
				token == SqlToken.GT ||
				token == SqlToken.GTEQ ||
				token == SqlToken.IN ||
				token == SqlToken.IS ||
				token == SqlToken.LIKE ||
				token == SqlToken.LT ||
				token == SqlToken.LTEQ ||
				token == SqlToken.NEQ ||
				token == SqlToken.NOT ||
				token == SqlToken.SPLAT ||
				token == SqlToken.OR;
		}

		public override string ToString()
		{
			return Lexum;
		}

		public override void GetParameters(List<CondParam> lst)
		{
		}
	}
}
