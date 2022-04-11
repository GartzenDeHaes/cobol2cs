using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CobolParser.SQL.Conds;

namespace CobolParser.SQL.Statements
{
	public class Prepare : SqlStatement
	{
		public CondParam SqlSourceField
		{
			get;
			private set;
		}

		public CondParam StatementNameField
		{
			get;
			private set;
		}

		public Prepare(SqlLex lex, string sqlText)
		: base(sqlText)
		{
			lex.Match(SqlToken.PREPARE);
			StatementNameField = new CondParam(lex);
			lex.Match(SqlToken.FROM);
			SqlSourceField = new CondParam(lex);
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
