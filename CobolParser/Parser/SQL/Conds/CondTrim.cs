using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.SQL.Conds
{
	public class CondTrim : ISqlCondToken
	{
		public bool IsLeading
		{
			get;
			private set;
		}

		public bool IsTrailing
		{
			get;
			private set;
		}

		public bool IsBoth
		{
			get;
			private set;
		}

		public string RemoveThis
		{
			get;
			private set;
		}

		public SqlExpr Field
		{
			get;
			private set;
		}

		public CondTrim(SqlLex lex)
		{
			lex.Match(SqlToken.TRIM);
			lex.Match(SqlToken.LPAR);

			if (lex.Lexum.StrEquals("TRAILING"))
			{
				IsTrailing = true;
				lex.Next();
			}
			else if (lex.Lexum.StrEquals("LEADING"))
			{
				IsLeading = true;
				lex.Next();
			}
			else if (lex.Lexum.StrEquals("BOTH"))
			{
				IsBoth = true;
				lex.Next();
			}

			if (lex.Token == SqlToken.STRLIT || lex.Lexum.Type == StringNodeType.Quoted)
			{
				RemoveThis = lex.GetStringLit();
				lex.Next();
			}
			else
			{
				RemoveThis = "\" \"";
			}

			lex.MatchOptional(SqlToken.FROM);

			Field = new SqlExpr(lex, true, false, false);

			lex.Match(SqlToken.RPAR);
		}

		public override void ListTables(List<string> lst)
		{
			if (null != Field)
			{
				Field.ListTables(lst);
			}
		}

		public override void GetParameters(List<CondParam> lst)
		{
			if (Field != null)
			{
				Field.GetParameters(lst);
			}
		}
	}
}
