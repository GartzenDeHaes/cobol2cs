using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.SQL.Conds
{
	public class CondField : ISqlCondToken
	{
		public SqlField Field
		{
			get;
			private set;
		}

		public string DatePartToDatePart
		{
			get;
			private set;
		}

		public CondField(SqlLex lex, bool skipAs)
		{
			Field = new SqlField(lex, skipAs);

			if (!lex.IsEOF && lex.Lexum.Next != null && lex.Lexum.Next.StrEquals("TO"))
			{
				DatePartToDatePart = lex.Lexum.Str;
				lex.Next();
				lex.Match("TO");
				DatePartToDatePart += " TO " + lex.Lexum.Str;
				lex.Next();
				if (lex.Token == SqlToken.LPAR)
				{
					lex.Next();
					DatePartToDatePart += "(" + lex.Lexum.Str + ")";
					lex.Next();
					lex.Match(SqlToken.RPAR);
				}
			}
		}

		public override void GetParameters(List<CondParam> lst)
		{
		}

		public override string ToString()
		{
			if (String.IsNullOrEmpty(DatePartToDatePart))
			{
				return Field.ToString();
			}
			return Field.ToString() + " " + DatePartToDatePart;
		}
	}
}
