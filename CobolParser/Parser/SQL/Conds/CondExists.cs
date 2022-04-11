using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.SQL.Statements;

namespace CobolParser.SQL.Conds
{
	public class CondExists : ISqlCondToken
	{
		public Select SubSelect
		{
			get;
			private set;
		}

		public CondExists(SqlLex lex)
		{
			lex.Match(SqlToken.EXISTS);
			lex.Match(SqlToken.LPAR);
			SubSelect = new Select(lex, "");
			lex.Match(SqlToken.RPAR);
		}

		public override void ListTables(List<string> lst)
		{
			SubSelect.ListTables(lst);
		}

		public override void GetParameters(List<CondParam> lst)
		{
			lst.AddRange(SubSelect.GetParameters());
		}
	}
}
