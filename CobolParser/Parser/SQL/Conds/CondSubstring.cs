using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.SQL.Conds
{
	/// <summary>
	/// SUBSTRING(SCRTCERT_CERT_ID FROM 9 FOR 3)
	/// </summary>
	public class CondSubstring : ISqlCondToken
	{
		public SqlExpr Field
		{
			get;
			private set;
		}

		public SqlExpr From
		{
			get;
			private set;
		}

		public SqlExpr For
		{
			get;
			private set;
		}

		public CondSubstring(SqlLex lex)
		{
			lex.Match(SqlToken.SUBSTRING);
			lex.Match(SqlToken.LPAR);
			Field = new SqlExpr(lex, true, false, false);
			lex.Match(SqlToken.FROM);
			From = new SqlExpr(lex, true, false, false);
			if (lex.Token == SqlToken.FOR)
			{
				lex.Match(SqlToken.FOR);
				For = new SqlExpr(lex, true, false, false);
			}

			lex.Match(SqlToken.RPAR);
		}

		public override void GetParameters(List<CondParam> lst)
		{
			if (Field != null)
			{
				Field.GetParameters(lst);
			}
			if (For != null)
			{
				For.GetParameters(lst);
			}
			if (From != null)
			{
				From.GetParameters(lst);
			}
		}

		public override void ListTables(List<string> lst)
		{
			Field.ListTables(lst);
			From.ListTables(lst);

			if (null != For)
			{
				For.ListTables(lst);
			}
		}
	}
}
