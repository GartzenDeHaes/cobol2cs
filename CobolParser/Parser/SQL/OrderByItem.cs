using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.SQL
{
	public class OrderByItem
	{
		public SqlField Field
		{
			get;
			private set;
		}

		public string Order
		{
			get;
			private set;
		}

		public OrderByItem(SqlLex lex)
		{
			Field = new SqlField(lex, true);

			if (!lex.IsEOF && (lex.Lexum.StrEquals("ASC") || lex.Lexum.StrEquals("DESC")))
			{
				Order = lex.Lexum.Str;
				lex.Next();
			}
		}

		public override string ToString()
		{
			return Field.ToString() + (String.IsNullOrEmpty(Order) ? "" : " " + Order);
		}
	}
}
