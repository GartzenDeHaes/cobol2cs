using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DOR.Core.Collections;

namespace CobolParser.SQL.Conds
{
	public class CondExpr : ISqlCondToken
	{
		public Vector<SqlExpr> Expr
		{
			get;
			private set;
		}

		public CondExpr()
		{
			Expr = new Vector<SqlExpr>();
			Expr.Add(new SqlExpr());
		}

		public CondExpr(SqlLex lex, bool breakOnComma)
		{
			Expr = new Vector<SqlExpr>();
			Parse(lex, breakOnComma);
		}

		public void Parse(SqlLex lex, bool breakOnComma)
		{
			bool expectRpar = false;

			if (lex.Token == SqlToken.LPAR)
			{
				expectRpar = true;
				lex.Match(SqlToken.LPAR);
			}

			if (Expr.Count != 0)
			{
				Expr[0].Parse(lex, true, true, breakOnComma);
			}
			else
			{
				Expr.Add(new SqlExpr(lex, true, true, breakOnComma));
			}
			if (expectRpar)
			{
				Expr[0].UseParens = true;

				lex.Match(SqlToken.RPAR);

				if (CondOperator.IsOperator(lex.Token))
				{
					SqlExpr expr = new SqlExpr();
					expr.Terms.Add(new CondNoOp());
					expr.Parse(lex, true, true, breakOnComma);
					Expr.Add(expr);
				}
			}
		}

		public override void ListTables(List<string> lst)
		{
			foreach (var e in Expr)
			{
				e.ListTables(lst);
			}
		}

		public override void GetParameters(List<CondParam> lst)
		{
			foreach (var e in Expr)
			{
				e.GetParameters(lst);
			}
		}

		public override string ToString()
		{
			string sql = "";

			foreach (var e in Expr)
			{
				if (sql.Length > 0)
				{
					sql += " ";
				}
				sql += e.ToString();
			}

			return sql;
		}
	}
}
