using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.SQL.Conds
{
	public class CondCase : ISqlCondToken
	{
		/// <summary>
		/// Not sure what this is
		/// </summary>
		public SqlExpr EntryExpr
		{
			get;
			private set;
		}

		public List<CondWhen> Whens
		{
			set;
			private get;
		}

		public SqlExpr ElseThis
		{
			set;
			private get;
		}

		public CondCase(SqlLex lex)
		{
			Whens = new List<CondWhen>();

			lex.Match(SqlToken.CASE);

			if (lex.Token != SqlToken.WHEN)
			{
				EntryExpr = new SqlExpr(lex, true, true, false);
			}

			while (lex.Token == SqlToken.WHEN)
			{
				Whens.Add(new CondWhen(lex));
			}

			if (lex.Token == SqlToken.ELSE)
			{
				lex.Match(SqlToken.ELSE);
				ElseThis = new SqlExpr(lex, true, false, false);
			}

			lex.Match(SqlToken.END);
		}

		public override void GetParameters(List<CondParam> lst)
		{
			if (EntryExpr != null)
			{
				EntryExpr.GetParameters(lst);
			}

			foreach (var w in Whens)
			{
				w.Cond.GetParameters(lst);
				w.Result.GetParameters(lst);
			}

			if (ElseThis != null)
			{
				ElseThis.GetParameters(lst);
			}
		}

		public override void ListTables(List<string> lst)
		{
			if (EntryExpr != null)
			{
				EntryExpr.ListTables(lst);
			}

			foreach (var w in Whens)
			{
				w.Cond.ListTables(lst);
				w.Result.ListTables(lst);
			}

			if (ElseThis != null)
			{
				ElseThis.ListTables(lst);
			}
		}
	}
}
