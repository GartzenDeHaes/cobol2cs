using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CobolParser.SQL.Conds;

namespace CobolParser.SQL.Statements
{
	public class RollbackTrans : SqlStatement
	{
		public RollbackTrans(SqlLex lex, string sqlText)
		: base(sqlText)
		{
			Debug.Assert(lex.Token == SqlToken.ABORT || lex.Token == SqlToken.ROLLBACK);
			lex.Next();
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
