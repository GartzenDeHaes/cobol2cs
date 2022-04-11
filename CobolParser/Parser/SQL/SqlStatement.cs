using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.SQL.Conds;
using DOR.Core.Collections;

namespace CobolParser.SQL
{
	public abstract class SqlStatement
	{
		public List<SqlTable> Tables
		{
			get;
			private set;
		}

		public string Text
		{
			get;
			private set;
		}

		public SqlStatement(string sqlText)
		{
			Tables = new List<SqlTable>();
			Text = sqlText;
		}

		public virtual void ListTables(List<string> lst)
		{
			lst.AddRange(from t in Tables select t.TableName);
		}

		protected void ParseTableList(SqlLex lex)
		{
			while (lex.Token == SqlToken.ID || lex.Token == SqlToken.EQ)
			{
				Tables.Add(new SqlTable(lex));
				lex.MatchOptional(SqlToken.COMMA);
			}
		}

		public abstract bool HasParameterOf(string fqname);

		public abstract List<CondParam> GetParameters();
	}
}
