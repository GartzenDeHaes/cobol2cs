using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DOR.Core.Collections;
using CobolParser.SQL.Conds;

namespace CobolParser.SQL.Statements
{
	public class Insert : SqlStatement
	{
		public Vector<SqlField> FieldList
		{
			get;
			private set;
		}

		public Vector<SqlExpr> Values
		{
			get;
			private set;
		}

		public Select SubSelect
		{
			get;
			private set;
		}

		public Insert(SqlLex lex, string sqlText)
		: base(sqlText)
		{
			Values = new Vector<SqlExpr>();

			lex.Match(SqlToken.INSERT);
			lex.Match(SqlToken.INTO);

			ParseTableList(lex);

			if (lex.Token == SqlToken.LPAR)
			{
				lex.Match(SqlToken.LPAR);

				if (lex.Token == SqlToken.SELECT)
				{
					SubSelect = new Select(lex, "");
					lex.Match(SqlToken.RPAR);
					return;
				}
				else
				{
					FieldList = new Vector<SqlField>();

					while (lex.Token != SqlToken.RPAR)
					{
						FieldList.Add(new SqlField(lex, true));
						lex.MatchOptional(SqlToken.COMMA);
					}
				}
				lex.Match(SqlToken.RPAR);
			}

			if (lex.Token == SqlToken.SELECT)
			{
				SubSelect = new Select(lex, "");
				return;
			}

			lex.MatchOptional(SqlToken.VALUES);
			lex.Match(SqlToken.LPAR);

			if (lex.Token == SqlToken.SELECT)
			{
				SubSelect = new Select(lex, "");
			}
			else
			{
				while (lex.Token != SqlToken.RPAR)
				{
					Values.Add(new SqlExpr(lex, true, false, false));
					lex.MatchOptional(SqlToken.COMMA);
				}
			}
			lex.Match(SqlToken.RPAR);
		}

		public override void ListTables(List<string> lst)
		{
			base.ListTables(lst);

			if (null != SubSelect)
			{
				SubSelect.ListTables(lst);
			}

			foreach (var v in Values)
			{
				v.ListTables(lst);
			}
		}

		public override bool HasParameterOf(string fqname)
		{
			return false;
		}

		public override List<CondParam> GetParameters()
		{
			List<CondParam> prms = new List<CondParam>();

			foreach (var v in Values)
			{
				v.GetParameters(prms);
			}

			if (SubSelect != null)
			{
				prms.AddRange(SubSelect.GetParameters());
			}

			return prms;
		}
	}
}
