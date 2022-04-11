using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.SQL.Conds
{
	public class CondDateFormat : ISqlCondToken
	{
		public SqlExpr Expr
		{
			get;
			private set;
		}

		public string Method
		{
			get;
			private set;
		}

		public CondDateFormat(SqlLex lex)
		{
			lex.Match(SqlToken.DATEFORMAT);
			lex.Match(SqlToken.LPAR);

			Expr = new SqlExpr(lex, true, false, false);

			lex.Match(SqlToken.COMMA);
			Method = lex.Lexum.Str;
			lex.Next();

			lex.Match(SqlToken.RPAR);
		}

		public override void ListTables(List<string> lst)
		{
			Expr.ListTables(lst);
		}

		public override void GetParameters(List<CondParam> lst)
		{
			Expr.GetParameters(lst);
		}

		public override string ToString()
		{
			return "DATEFORMAT(" + Expr.ToString() + (String.IsNullOrEmpty(Method) ? "" : ", " + Method) + ")";
		}
	}
}
