using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.SQL.Conds
{
	public class CondDateTime : ISqlCondToken
	{
		public string Value
		{
			get;
			private set;
		}

		public string FromSomething
		{
			get;
			private set;
		}

		public string ToSomthing
		{
			get;
			private set;
		}

		public CondDateTime(SqlLex lex)
		{
			lex.Match(SqlToken.DATETIME);

			Value = lex.GetStringLit();
			lex.Next();

			FromSomething = lex.Lexum.Str;
			lex.Next();

			if (lex.Lexum.StrEquals("TO"))
			{
				lex.Match("TO");

				if (lex.Lexum.StrEquals("FRACTION"))
				{
					lex.Match("FRACTION");
					lex.Match(SqlToken.LPAR);
					ToSomthing = "FRACTION(" + lex.Lexum.Str + ")";
					lex.Next();
					lex.Match(SqlToken.RPAR);
				}
				else
				{
					ToSomthing = lex.Lexum.Str;
					lex.Next();
				}
			}
		}

		public override void GetParameters(List<CondParam> lst)
		{
		}
	}
}
