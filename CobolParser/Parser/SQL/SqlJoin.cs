using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.SQL.Conds;

namespace CobolParser.SQL
{
	public enum SqlJoinType
	{
		ROOT,
		LEFT,
		RIGHT,
		INNER
	}

	public class SqlJoin
	{
		public SqlJoinType Type
		{
			get;
			private set;
		}

		public SqlJoin Left
		{
			get;
			private set;
		}

		public SqlTable Right
		{
			get;
			private set;
		}

		public SqlExpr OnClause
		{
			get;
			private set;
		}

		public SqlJoin(SqlTable tbl)
		{
			Type = SqlJoinType.ROOT;
			Right = tbl;
		}

		public SqlJoin(SqlTable left, SqlTable right)
		{
			Type = SqlJoinType.INNER;
			Left = new SqlJoin(left);
			Right = right;
		}

		public SqlJoin(SqlJoinType type, SqlJoin left, SqlLex lex)
		{
			Type = type;
			Left = left;
			Right = new SqlTable(lex);

			if (lex.Token == SqlToken.ON)
			{
				lex.Match(SqlToken.ON);
				OnClause = new SqlExpr(lex, true, false, false);
			}
		}

		public void ListTables(List<string> lst)
		{
			if (null != Left)
			{
				Left.ListTables(lst);
			}

			if (null != Right)
			{
				lst.Add(Right.TableName);
			}
		}

		public void GetParameters(List<CondParam> lst)
		{
			if (null != Left)
			{
				Left.GetParameters(lst);
			}

			if (null != OnClause)
			{
				OnClause.GetParameters(lst);
			}
		}

		public override string ToString()
		{
			if (Left == null)
			{
				return Right.ToString();
			}

			if (OnClause == null)
			{
				return Left.ToString() + ", " + Right.ToString();
			}

			StringBuilder buf = new StringBuilder();

			buf.Append(Left.ToString());

			if (Type == SqlJoinType.RIGHT)
			{
				buf.Append(" RIGHT JOIN ");
			}
			else if (Type == SqlJoinType.LEFT)
			{
				buf.Append(" LEFT JOIN ");
			}
			else
			{
				buf.Append(" INNER JOIN ");
			}

			buf.Append(Right.ToString());

			buf.Append("\" +");
			buf.Append(Environment.NewLine);
			buf.Append("\t\t\t\t\t\"");
			buf.Append(" ON (");
			buf.Append(OnClause.ToString());
			buf.Append(")");
			buf.Append("\" +");
			buf.Append(Environment.NewLine);
			buf.Append("\t\t\t\t\t\"");

			return buf.ToString();
		}
	}
}
