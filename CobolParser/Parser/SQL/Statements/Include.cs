using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.SQL.Conds;

namespace CobolParser.SQL.Statements
{
	public class Include : SqlStatement
	{
		public string IncludeName
		{
			get;
			private set;
		}

		public Include(SqlLex lex, string sqlText)
		: base(sqlText)
		{
			lex.Match(SqlToken.INCLUDE);
			IncludeName = lex.Lexum.Str;
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
