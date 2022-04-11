using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CobolParser.SQL.Conds;

namespace CobolParser.SQL.Statements
{
	public class DeclareCursor : SqlStatement
	{
		public ISqlCondToken CursorName
		{
			get;
			private set;
		}

		public SqlStatement Stmt
		{
			get;
			private set;
		}

		/// <summary>
		/// DECLARE :SQL-CURSOR-NAME CURSOR FOR :SQL-STATEMENT-NAME
		/// </summary>
		public CondParam StmtField
		{
			get;
			private set;
		}

		public DeclareCursor(SqlLex lex, string sqlText)
		: base(sqlText)
		{
			lex.Match(SqlToken.DECLARE);
			CursorName = ISqlCondToken.Parse(lex, true, false, false);
			lex.Match(SqlToken.CURSOR);
			lex.Match(SqlToken.FOR);

			lex.MatchOptional(SqlToken.LPAR);

			if (lex.Token == SqlToken.COLON)
			{
				StmtField = new CondParam(lex);
			}
			else
			{
				Stmt = SqlParser.Parse(lex, sqlText);
			}
			lex.MatchOptional(SqlToken.RPAR);
		}

		public override bool HasParameterOf(string fqname)
		{
			return false;
		}

		public override List<CondParam> GetParameters()
		{
			List<CondParam> prms = new List<CondParam>();
			return prms;
		}
	}
}
