using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.SQL.Conds;

namespace CobolParser.SQL
{
	public class Current : ISqlCondToken
	{
		public string Part
		{
			get;
			private set;
		}

		public Current(SqlLex lex)
		{
			lex.Match("CURRENT");

			if (lex.Lexum.StrEquals("YEAR"))
			{
				Part = lex.Lexum.Str;
				lex.Next();
			}
			else if (lex.Lexum.StrEquals("MONTH"))
			{
				Part = lex.Lexum.Str;
				lex.Next();
			}
			else if (lex.Lexum.StrEquals("DAY"))
			{
				Part = lex.Lexum.Str;
				lex.Next();
			}
			else if (lex.Lexum.StrEquals("HOUR"))
			{
				Part = lex.Lexum.Str;
				lex.Next();
			}

			if (lex.Lexum.StrEquals("TO"))
			{
				lex.Next();

				if (lex.Lexum.StrEquals("FRACTION"))
				{
					lex.Match("FRACTION");

					if (lex.Token == SqlToken.LPAR)
					{
						lex.Match(SqlToken.LPAR);
						lex.Match("6");
						lex.Match(SqlToken.RPAR);
						Part += " TO FRACTION(6)";
					}
				}
				else
				{
					Part += " TO " + lex.Lexum.Str;
					lex.Next();
				}
			}
		}

		public override void GetParameters(List<CondParam> lst)
		{
		}

		public override string ToString()
		{
			return "CURRENT" + (String.IsNullOrEmpty(Part) ? "" : " " + Part);
		}
	}
}
