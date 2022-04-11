using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CobolParser.SQL.Conds;

namespace CobolParser.SQL.Statements
{
	public class Close : SqlStatement
	{
		public ISqlCondToken CursorName
		{
			get;
			private set;
		}

		public Close(SqlLex lex, string sqlText)
		: base(sqlText)
		{
			lex.Match(SqlToken.CLOSE);
			CursorName = ISqlCondToken.Parse(lex, true, false, false);
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
