using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using CobolParser.SQL.Statements;

namespace CobolParser.SQL.Conds
{
	public class CondIn : ISqlCondToken
	{
		public List<string> Items
		{
			get;
			private set;
		}

		public Select SubSelect
		{
			get;
			private set;
		}

		public CondIn(SqlLex lex)
		{
			Items = new List<string>();

			lex.Match(SqlToken.IN);
			lex.Match(SqlToken.LPAR);

			if (lex.Token == SqlToken.SELECT)
			{
				SubSelect = new Select(lex, "");
			}
			else
			{
				while (lex.Token != SqlToken.RPAR)
				{
					Items.Add(lex.Lexum.Str);
					lex.Next();
					lex.MatchOptional(SqlToken.COMMA);
				}
			}

			lex.Match(SqlToken.RPAR);
		}

		public override void ListTables(List<string> lst)
		{
			if (null != SubSelect)
			{
				SubSelect.ListTables(lst);
			}
		}

		public override void GetParameters(List<CondParam> lst)
		{
			if (SubSelect != null)
			{
				lst.AddRange(SubSelect.GetParameters());
			}
		}

		public override string ToString()
		{
			Debug.Assert(SubSelect == null);

			StringBuilder buf = new StringBuilder();

			buf.Append("IN (");

			for (int x = 0; x < Items.Count; x++)
			{
				if (x > 0)
				{
					buf.Append(", ");
				}
				buf.Append(Items[x]);
			}
			buf.Append(")");

			return buf.ToString();
		}
	}
}
