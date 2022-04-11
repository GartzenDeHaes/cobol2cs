using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.SQL.Conds
{
	public class CondPostion : ISqlCondToken
	{
		public string Value
		{
			get;
			private set;
		}

		public SqlExpr Field
		{
			get;
			private set;
		}

		public string Start
		{
			get;
			private set;
		}

		public CondPostion(SqlLex lex)
		{
			lex.Match(SqlToken.POSITION);
			lex.Match(SqlToken.LPAR);
			Value = lex.Lexum.Str;
			lex.Next();

			lex.Match("IN");

			Field = new SqlExpr(lex, true, false, false);

			if (lex.Token == SqlToken.COMMA)
			{
				lex.Next();
				Start = lex.Lexum.Str;
				lex.Next();
			}

			lex.Match(SqlToken.RPAR);
		}

		public override void GetParameters(List<CondParam> lst)
		{
			Field.GetParameters(lst);
		}
	}
}
