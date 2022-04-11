using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.SQL.Conds
{
	public class CondNull : ISqlCondToken
	{
		public CondNull(SqlLex lex)
		{
			lex.Match(SqlToken.NULL);
		}

		public override void GetParameters(List<CondParam> lst)
		{
		}
	}
}
