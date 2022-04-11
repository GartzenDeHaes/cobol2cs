using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DOR.Core.Collections;
using CobolParser.SQL.Conds;

namespace CobolParser.SQL
{
	public class SqlExpr
	{
		public Vector<ISqlCondToken> Terms
		{
			get;
			private set;
		}

		public bool UseParens
		{
			get;
			set;
		}

		public SqlExpr()
		{
			Terms = new Vector<ISqlCondToken>();
		}

		public SqlExpr
		(
			SqlLex lex,
			bool skipAs,
			bool allowBetween,
			bool breakOnComma
		)
		: this()
		{
			Parse(lex, skipAs, allowBetween, breakOnComma);
		}

		public void Parse
		(
			SqlLex lex, 
			bool skipAs, 
			bool allowBetween, 
			bool breakOnComma
		)
		{
			Debug.Assert(!lex.IsEOF);

			while
			(
				!lex.IsEOF &&
				lex.Token != SqlToken.ORDER &&
				lex.Token != SqlToken.GROUP &&
				lex.Token != SqlToken.HAVING &&
				lex.Token != SqlToken.FOR &&
				lex.Token != SqlToken.WHEN &&
				lex.Token != SqlToken.SET &&
				lex.Token != SqlToken.FROM &&
				lex.Token != SqlToken.INTO &&
				lex.Token != SqlToken.COMMA &&
				lex.Token != SqlToken.RPAR &&
				lex.Token != SqlToken.ELSE &&
				lex.Token != SqlToken.END &&
				lex.Token != SqlToken.THEN &&
				lex.Token != SqlToken.LEFT &&
				lex.Token != SqlToken.RIGHT &&
				lex.Token != SqlToken.INNER &&
				lex.Token != SqlToken.WHERE &&
				lex.Token != SqlToken.BROWSE &&
				lex.Token != SqlToken.STABLE &&
				lex.Token != SqlToken.UNION &&
				lex.Token != SqlToken.JOIN
			)
			{
				if (!allowBetween && lex.Token == SqlToken.AS)
				{
					return;
				}

				if (Terms.Count == 0 && CondOperator.IsOperator(lex.Token))
				{
					Terms.Add(ISqlCondToken.Parse(lex, skipAs, allowBetween, breakOnComma));
					return;
				}

				if
				(
					breakOnComma && 
					(
						lex.Token == SqlToken.LT ||
						lex.Token == SqlToken.LTEQ ||
						lex.Token == SqlToken.GT ||
						lex.Token == SqlToken.GTEQ ||
						lex.Token == SqlToken.EQ ||
						lex.Token == SqlToken.NEQ ||
						lex.Token == SqlToken.AND ||
						lex.Token == SqlToken.OR ||
						lex.Token == SqlToken.NOT
					)
				)
				{
					return;
				}
				Terms.Add(ISqlCondToken.Parse(lex, skipAs, allowBetween, breakOnComma));
			}
		}

		public void ListTables(List<string> lst)
		{
			foreach (var t in Terms)
			{
				t.ListTables(lst);
			}
		}

		public void GetParameters(List<CondParam> prms)
		{
			foreach (var t in Terms)
			{
				t.GetParameters(prms);
			}
		}

		public override string ToString()
		{
			StringBuilder buf = new StringBuilder();

			for (int x = 0; x < Terms.Count; x++)
			{
				var t = Terms[x];

				buf.Append(t.ToString());

				if (t.ToString().EndsWith("FRACTION", StringComparison.InvariantCultureIgnoreCase))
				{
					x++;
					buf.Append("(");
					buf.Append(Terms[x].ToString());
					buf.Append(")");
				}

				buf.Append(" ");
			}

			return buf.ToString();
		}
	}
}
