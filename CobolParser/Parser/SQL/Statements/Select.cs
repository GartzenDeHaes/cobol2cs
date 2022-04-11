using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DOR.Core.Collections;
using CobolParser.SQL.Conds;

namespace CobolParser.SQL.Statements
{
	public class Select : SqlStatement
	{
		public int LineNumber
		{
			get;
			private set;
		}

		public bool IsDistinct
		{
			get;
			private set;
		}

		public Vector<SqlExpr> Fields
		{
			get;
			private set;
		}

		public Vector<SqlJoin> Joins
		{
			get;
			private set;
		}

		public Vector<CondParam> IntoVars
		{
			get;
			private set;
		}

		public SqlExpr Where
		{
			get;
			private set;
		}

		public Vector<OrderByItem> Orders
		{
			get;
			private set;
		}

		public Vector<SqlField> Groups
		{
			get;
			private set;
		}

		public SqlExpr Having
		{
			get;
			private set;
		}

		public string ForThisAccess
		{
			get;
			private set;
		}

		public Select UnionWith
		{
			get;
			private set;
		}

		public Select(SqlLex lex, string sqlText)
		: base(sqlText)
		{
			LineNumber = lex.Lexum.LineNumber;

			Fields = new Vector<SqlExpr>();
			IntoVars = new Vector<CondParam>();
			Joins = new Vector<SqlJoin>(4);
			Orders = new Vector<OrderByItem>();
			Groups = new Vector<SqlField>();

			lex.Match(SqlToken.SELECT);

			if (lex.Token == SqlToken.DISTINCT)
			{
				IsDistinct = true;
				lex.Next();
			}

			while (lex.Token != SqlToken.FROM && lex.Token != SqlToken.INTO)
			{
				Fields.Add(new SqlExpr(lex, false, false, false));
				lex.MatchOptional(SqlToken.COMMA);
			}

			if (lex.Token == SqlToken.INTO)
			{
				lex.Match(SqlToken.INTO);

				while (lex.Token != SqlToken.FROM)
				{
					IntoVars.Add(new CondParam(lex));
					lex.MatchOptional(SqlToken.COMMA);
				}
			}

			lex.Match(SqlToken.FROM);

			SqlJoin table = new SqlJoin(new SqlTable(lex));
			Tables.Add(table.Right);

			lex.MatchOptional(SqlToken.LPAR);

			while 
			(
				lex.Token == SqlToken.INNER ||
				lex.Token == SqlToken.LEFT ||
				lex.Token == SqlToken.RIGHT ||
				lex.Token == SqlToken.JOIN ||
				lex.Token == SqlToken.COMMA
			)
			{
				if (lex.Token == SqlToken.COMMA)
				{
					lex.Match(SqlToken.COMMA);
					SqlJoin join = new SqlJoin(SqlJoinType.INNER, table, lex);
					Tables.Add(join.Right);
					table = join;
				}
				else if (lex.Token == SqlToken.LEFT)
				{
					lex.Match(SqlToken.LEFT);
					lex.Match(SqlToken.JOIN);
					SqlJoin join = new SqlJoin(SqlJoinType.LEFT, table, lex);
					if (Joins.Count == 0)
					{
						Joins.Add(join);
					}
					else
					{
						Joins[0] = join;
					}
					Tables.Add(join.Right);
					table = join;
				}
				else if (lex.Token == SqlToken.RIGHT)
				{
					lex.Match(SqlToken.RIGHT);
					lex.Match(SqlToken.JOIN);
					SqlJoin join = new SqlJoin(SqlJoinType.RIGHT, table, lex);
					if (Joins.Count == 0)
					{
						Joins.Add(join);
					}
					Tables.Add(join.Right);
					table = join;
				}
				else if (lex.Token == SqlToken.INNER)
				{
					lex.Match(SqlToken.INNER);
					lex.Match(SqlToken.JOIN);
					SqlJoin join = new SqlJoin(SqlJoinType.INNER, table, lex);
					if (Joins.Count == 0)
					{
						Joins.Add(join);
					}
					Tables.Add(join.Right);
					table = join;
				}
				else if (lex.Token == SqlToken.JOIN)
				{
					lex.Match(SqlToken.JOIN);
					SqlJoin join = new SqlJoin(SqlJoinType.INNER, table, lex);
					if (Joins.Count == 0)
					{
						Joins.Add(join);
					}
					Tables.Add(join.Right);
					table = join;
				}
				lex.MatchOptional(SqlToken.RPAR);
			}

			while(!lex.IsEOF)
			{
				if (lex.Token == SqlToken.WHERE)
				{
					lex.Match(SqlToken.WHERE);
					Where = new SqlExpr(lex, true, true, false);
				}
				else if (lex.Token == SqlToken.ORDER)
				{
					lex.Next();
					lex.Match(SqlToken.BY);

					Orders.Add(new OrderByItem(lex));
					while (lex.Token == SqlToken.COMMA)
					{
						lex.Next();
						Orders.Add(new OrderByItem(lex));
					}
				}
				else if (lex.Token == SqlToken.GROUP)
				{
					lex.Next();
					lex.Match(SqlToken.BY);
				
					Groups.Add(new SqlField(lex, true));
					while (lex.Token == SqlToken.COMMA)
					{
						lex.Next();
						Groups.Add(new SqlField(lex, true));
					}
				}
				else if (lex.Token == SqlToken.HAVING)
				{
					lex.Next();
					Having = new SqlExpr(lex, true, true, false);
				}
				else
				{
					break;
				}
			}

			if (lex.Token == SqlToken.UNION)
			{
				lex.Next();
				lex.MatchOptional("ALL");
				UnionWith = new Select(lex, "");
			}

			lex.MatchOptional("FOR");

			if (!lex.IsEOF && (lex.Token == SqlToken.BROWSE || lex.Token == SqlToken.STABLE))
			{
				ForThisAccess = lex.Lexum.Str;
				lex.Next();
				lex.Match("ACCESS");
			}

			if (lex.Token == SqlToken.UNION)
			{
				lex.Next();
				lex.MatchOptional("ALL");

				bool expectRpar = lex.Token == SqlToken.LPAR;
				lex.MatchOptional(SqlToken.LPAR);
				UnionWith = new Select(lex, "");

				if (expectRpar)
				{
					lex.Match(SqlToken.RPAR);
				}
			}

			lex.MatchOptional("FOR");

			if (!lex.IsEOF && (lex.Token == SqlToken.BROWSE || lex.Token == SqlToken.STABLE))
			{
				ForThisAccess = lex.Lexum.Str;
				lex.Next();
				lex.Match("ACCESS");
			}
		}

		public override void ListTables(List<string> lst)
		{
			base.ListTables(lst);

			if (null != UnionWith)
			{
				UnionWith.ListTables(lst);
			}

			foreach (var f in Fields)
			{
				f.ListTables(lst);
			}

			foreach (var j in Joins)
			{
				j.ListTables(lst);
			}

			if (null != Where)
			{
				Where.ListTables(lst);
			}

			if (null != Having)
			{
				Having.ListTables(lst);
			}
		}

		public override List<CondParam> GetParameters()
		{
			List<CondParam> prms = new List<CondParam>();

			if (null != UnionWith)
			{
				prms.AddRange(UnionWith.GetParameters());
			}

			foreach (var f in Fields)
			{
				f.GetParameters(prms);
			}

			foreach (var j in Joins)
			{
				j.GetParameters(prms);
			}

			if (null != Where)
			{
				Where.GetParameters(prms);
			}

			if (null != Having)
			{
				Having.GetParameters(prms);
			}

			return prms;
		}

		public override bool HasParameterOf(string fqname)
		{
			foreach (var j in Joins)
			{
				foreach (var t in j.OnClause.Terms)
				{
					if (t is CondParam && ((CondParam)t).Symbol.Record.FullyQualifiedName == fqname)
					{
						return true;
					}
				}
			}

			if (Where != null)
			{
				foreach (var c in Where.Terms)
				{
					if (c is CondParam && ((CondParam)c).Symbol.Record.FullyQualifiedName == fqname)
					{
						return true;
					}
				}
			}

			if (Having != null)
			{
				foreach (var h in Having.Terms)
				{
					if (h is CondParam && ((CondParam)h).Symbol.Record.FullyQualifiedName == fqname)
					{
						return true;
					}
				}
			}

			if (UnionWith != null)
			{
				return UnionWith.HasParameterOf(fqname);
			}
			return false;
		}
	}
}
