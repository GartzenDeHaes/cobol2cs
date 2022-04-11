using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DOR.Core.Collections;

namespace CobolParser.SQL.Conds
{
	public class CondFunction : ISqlCondToken
	{
		public string FunctionName
		{
			get;
			private set;
		}

		public Vector<SqlExpr> Arguments
		{
			get;
			private set;
		}

		public bool IsDistinct
		{
			get;
			private set;
		}

		public CondFunction(SqlLex lex)
		{
			Arguments = new Vector<SqlExpr>();

			FunctionName = lex.Lexum.Str;
			lex.Match(SqlToken.ID);

			lex.Match(SqlToken.LPAR);

			if (lex.Token == SqlToken.DISTINCT)
			{
				IsDistinct = true;
				lex.Next();
			}

			Arguments.Add(new SqlExpr(lex, true, false, true));

			while (lex.Token == SqlToken.COMMA)
			{
				lex.Next();
				Arguments.Add(new SqlExpr(lex, true, false, true));
			}

			lex.Match(SqlToken.RPAR);
		}

		public override void ListTables(List<string> lst)
		{
			foreach (var a in Arguments)
			{
				a.ListTables(lst);
			}
		}

		public override void GetParameters(List<CondParam> lst)
		{
			foreach (var a in Arguments)
			{
				a.GetParameters(lst);
			}
		}

		public override string ToString()
		{
			StringBuilder buf = new StringBuilder();
			buf.Append(FunctionName);
			buf.Append("(");

			if (IsDistinct)
			{
				buf.Append("DISTINCT ");
			}

			foreach (var a in Arguments)
			{
				buf.Append(a.ToString());
				buf.Append(" ");
			}

			buf.Append(")");

			return buf.ToString();
		}
	}
}
