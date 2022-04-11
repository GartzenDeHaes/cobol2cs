using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core.Collections;

namespace CobolParser.SQL.Conds
{
	public class Set
	{
		public SqlField Field
		{
			get;
			private set;
		}

		public SqlExpr Value
		{
			get;
			private set;
		}

		public Set(SqlLex lex)
		{
			Field = new SqlField(lex, true);
			lex.Match(SqlToken.EQ);
			Value = new SqlExpr(lex, true, false, false);
		}

		public void ListTables(List<string> lst)
		{
			Value.ListTables(lst);
		}

		public void GetParameters(List<CondParam> lst)
		{
			Value.GetParameters(lst);
		}
	}
}
