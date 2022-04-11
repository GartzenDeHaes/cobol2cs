using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CobolParser.SQL.Conds;

namespace CobolParser.SQL.Statements
{
	public class OpenSql : SqlStatement
	{
		public string CursorName
		{
			get;
			private set;
		}

		public OpenSql(SqlLex lex, string sqlText)
		: base(sqlText)
		{
			lex.Match(SqlToken.OPEN);
			CursorName = lex.Lexum.Str;//ISqlCondToken.Parse(lex, true, false);
			lex.Next();
		}

		public override bool HasParameterOf(string fqname)
		{
			return false;
		}

		public override List<CondParam> GetParameters()
		{
			List<CondParam> prms = new List<CondParam>();
			return prms;
		}
	}
}
