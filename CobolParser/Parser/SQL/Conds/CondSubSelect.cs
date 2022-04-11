using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.SQL.Statements;

namespace CobolParser.SQL.Conds
{
	public class CondSubSelect : ISqlCondToken
	{
		public Select SubSelect
		{
			get;
			private set;
		}

		public CondSubSelect(SqlLex lex)
		{
			lex.Match(SqlToken.LPAR);

			SubSelect = new Select(lex, "");

			lex.Match(SqlToken.RPAR);
		}

		public override void GetParameters(List<CondParam> lst)
		{
			lst.AddRange(SubSelect.GetParameters());
		}

		public override void ListTables(List<string> lst)
		{
			SubSelect.ListTables(lst);
		}
	}
}
