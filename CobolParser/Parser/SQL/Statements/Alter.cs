using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.SQL.Conds;

namespace CobolParser.SQL.Statements
{
	public class Alter : SqlStatement
	{
		public Alter(SqlLex lex, string sqlText)
		: base(sqlText)
		{
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
