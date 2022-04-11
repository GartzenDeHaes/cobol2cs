using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.SQL.Conds;

namespace CobolParser.SQL
{
	public class SqlType : ISqlCondToken
	{
		public string TypeName
		{
			get;
			private set;
		}

		public string Length
		{
			get;
			private set;
		}

		public string Extent
		{
			get;
			private set;
		}

		public SqlType(SqlLex lex)
		{
			// Allows intermixing of cobol with sql
			// FE: CAST(B004RTN_BATCH_NUM AS PIC 9(4))
			if (lex.Lexum.StrEquals("PIC"))
			{
				lex.Next();
				
				if (lex.Lexum.StrEquals("9"))
				{
					TypeName = "INT";
				}
				else
				{
					TypeName = "CHAR";
				}
				lex.Next();

				if (lex.Token == SqlToken.LPAR)
				{
					lex.Match(SqlToken.LPAR);
					Length = lex.Lexum.Str;
					lex.Next();
					lex.Match(SqlToken.RPAR);
				}
				else
				{
					Length = "1";
				}
				return;
			}

			TypeName = lex.Lexum.Str;
			lex.Next();

			if (lex.Token == SqlToken.LPAR)
			{
				lex.Match(SqlToken.LPAR);
				Length = lex.Lexum.Str;
				lex.Next();

				if (lex.Token == SqlToken.COMMA)
				{
					lex.Next();
					Extent = lex.Lexum.Str;
					lex.Next();
				}

				lex.Match(SqlToken.RPAR);
			}
		}

		public override void GetParameters(List<CondParam> lst)
		{
		}

		public override string ToString()
		{
			StringBuilder buf = new StringBuilder();
			buf.Append(TypeName);
			if (!String.IsNullOrEmpty(Length))
			{
				buf.Append("(");
				buf.Append(Length);
				if (!String.IsNullOrEmpty(Extent))
				{
					buf.Append(", ");
					buf.Append(Extent);
				}
				buf.Append(")");
			}

			return buf.ToString();
		}
	}
}
