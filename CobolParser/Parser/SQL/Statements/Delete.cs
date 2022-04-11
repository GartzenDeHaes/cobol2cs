using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core.Collections;
using CobolParser.SQL.Conds;

namespace CobolParser.SQL.Statements
{
	public class Delete : SqlStatement
	{
		public SqlExpr Where
		{
			get;
			private set;
		}

		public string ForThisAccess
		{
			get;
			private set;
		}

		public Delete(SqlLex lex, string sqlText)
		: base(sqlText)
		{
			lex.Match(SqlToken.DELETE);
			lex.Match(SqlToken.FROM);
			ParseTableList(lex);

			if (lex.Token == SqlToken.WHERE)
			{
				lex.Match(SqlToken.WHERE);
				Where = new SqlExpr(lex, true, true, false);
			}

			if (lex.Token == SqlToken.FOR)
			{
				lex.Match(SqlToken.FOR);
				ForThisAccess = lex.Lexum.Str;
				lex.Next();
				lex.Match("ACCESS");
			}
		}

		public override bool HasParameterOf(string fqname)
		{
			if (Where != null)
			{
				foreach (var c in Where.Terms)
				{
					if (c is CondParam && ((CondParam)c).Symbol.Record.FullyQualifiedName == fqname)
					{
						return true;
					}
				}
			}

			return false;
		}

		public override List<CondParam> GetParameters()
		{
			List<CondParam> prms = new List<CondParam>();
			if (Where != null)
			{
				Where.GetParameters(prms);
			}
			return prms;
		}
	}
}
