using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.SQL.Conds
{
	public class CondValue : ISqlCondToken
	{
		public string Value
		{
			get;
			private set;
		}

		public CondValue(SqlLex lex)
		{
			if (lex.Token == SqlToken.STRLIT)
			{
				Value = lex.GetStringLit();
			}
			else
			{
				Value = lex.Lexum.Str;
			}
			lex.Next();
		}

		public override void GetParameters(List<CondParam> lst)
		{
		}

		public override string ToString()
		{
			return Value;
		}
	}
}
