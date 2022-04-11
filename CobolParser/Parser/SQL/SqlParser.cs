using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core.Collections;
using CobolParser.SQL.Statements;

namespace CobolParser.SQL
{
	public static class SqlParser
	{
		public static SqlStatement Parse(SqlLex lex, string sqlText)
		{
			switch (lex.Token)
			{
				case SqlToken.ALTER:
					return new Alter(lex, sqlText);
				case SqlToken.SELECT:
					return new Select(lex, sqlText);
				case SqlToken.ABORT:
					return new RollbackTrans(lex, sqlText);
				case SqlToken.INVOKE:
					return new Invoke(lex, sqlText);
				case SqlToken.INSERT:
					return new Insert(lex, sqlText);
				case SqlToken.DELETE:
					return new Delete(lex, sqlText);
				case SqlToken.CREATE:
					return new Create(lex, sqlText);
				case SqlToken.COMMIT:
					return new CommitTrans(lex, sqlText);
				case SqlToken.END:
					return new EndDeclareSection(lex, sqlText);
				case SqlToken.INCLUDE:
					return new Include(lex, sqlText);
				case SqlToken.DECLARE:
					return new DeclareCursor(lex, sqlText);
				case SqlToken.OPEN:
					return new OpenSql(lex, sqlText);
				case SqlToken.FETCH:
					return new Fetch(lex, sqlText);
				case SqlToken.CLOSE:
					return new Close(lex, sqlText);
				case SqlToken.UPDATE:
					return new Update(lex, sqlText);
				case SqlToken.FREE:
					return new Free(lex, sqlText);
				case SqlToken.ROLLBACK:
					return new RollbackTrans(lex, sqlText);
				case SqlToken.CONTROL:
					return new Control(lex, sqlText);
				case SqlToken.LOCK:
					return new LockTable(lex, sqlText);
				case SqlToken.PREPARE:
					return new Prepare(lex, sqlText);
				case SqlToken.DESCRIBE:
					return new Describe(lex, sqlText);
				case SqlToken.EXECUTE:
					return new Execute(lex, sqlText);
				case SqlToken.BEGIN:
					if (lex.Lexum.Next.StrEquals("TRANSACTION"))
					{
						return new BeginTrans(lex, sqlText);
					}
					else
					{
						return new BeginDeclareSection(lex, sqlText);
					}
				default:
					throw new SyntaxError(lex.Lexum.FileName, lex.Lexum.LineNumber, "Unknown SQL statement of " + lex.Lexum.Str);
			}
		}

		public static SqlStatement Parse(Vector<StringNode> terms, string sqlText)
		{
			SqlLex lex  = new SqlLex(terms);
			lex.Next();
			return Parse(lex, sqlText);
		}
	}
}
