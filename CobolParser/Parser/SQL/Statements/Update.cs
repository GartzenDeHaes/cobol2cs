using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DOR.Core.Collections;
using CobolParser.SQL.Conds;

namespace CobolParser.SQL.Statements
{
	public class Update : SqlStatement
	{
		public SqlTable Table
		{
			get;
			private set;
		}

		public Vector<Set> Sets
		{
			get;
			private set;
		}

		public SqlExpr Where
		{
			get;
			private set;
		}

		public string ForThisAccess
		{
			get;
			private set;
		}

		public Update(SqlLex lex, string sqlText)
		: base(sqlText)
		{
			Sets = new Vector<Set>();

			lex.Match(SqlToken.UPDATE);

			if (lex.Lexum.StrEquals("ALL") || lex.Lexum.StrEquals("STATISTICS"))
			{
				return;
			}

			Table = new SqlTable(lex);
			Tables.Add(Table);

			lex.Match(SqlToken.SET);
			while (!lex.IsEOF && lex.Token == SqlToken.ID)
			{
				Sets.Add(new Set(lex));
				lex.MatchOptional(SqlToken.COMMA);
			}

			if (lex.Token == SqlToken.WHERE)
			{
				lex.Match(SqlToken.WHERE);
				Where = new SqlExpr(lex, true, true, false);
			}

			if (lex.Token == SqlToken.FOR)
			{
				lex.Match(SqlToken.FOR);
				ForThisAccess = lex.Lexum.Str;
				lex.Next();
				lex.Match("ACCESS");
			}
		}

		public override void ListTables(List<string> lst)
		{
			base.ListTables(lst);

			if (null != Where)
			{
				Where.ListTables(lst);
			}

			foreach (var s in Sets)
			{
				s.ListTables(lst);
			}
		}

		public override bool HasParameterOf(string fqname)
		{
			return false;
		}

		public override List<CondParam> GetParameters()
		{
			List<CondParam> prms = new List<CondParam>();
			Where.GetParameters(prms);
			for (int x = 0; x < Sets.Count; x++)
			{
				Sets[x].GetParameters(prms);
			}
			return prms;
		}
	}
}
