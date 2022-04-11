using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DOR.Core.Collections;

namespace CobolParser.SQL
{
	public class SqlLex
	{
		private Vector<StringNode> _terms;
		private int _pos = -1;

		public StringNode Lexum
		{
			get { return _terms[_pos]; }
		}

		public SqlToken Token
		{
			get;
			private set;
		}

		public bool IsEOF
		{
			get { return _pos >= _terms.Count || (_pos > -1 && _terms[_pos].StrEquals("END-EXEC")); }
		}

		public SqlLex(Vector<StringNode> terms)
		{
			_terms = terms;
		}

		public bool Match(SqlToken tok)
		{
			if (IsEOF)
			{
				return false;
			}
			if (tok != Token)
			{
				throw new SyntaxError(Lexum.FileName, Lexum.LineNumber, "Expected " + tok.ToString() + ", found " + Lexum.Str);
			}

			return Next();
		}

		public bool Match(string s)
		{
			if (IsEOF)
			{
				return false;
			}
			if (! Lexum.StrEquals(s))
			{
				throw new SyntaxError(Lexum.FileName, Lexum.LineNumber, "Expected " + s + ", found " + Lexum.Str);
			}

			return Next();
		}

		public bool MatchOptional(SqlToken tok)
		{
			if (IsEOF)
			{
				return false;
			}
			if (tok != Token)
			{
				return true;
			}

			return Next();
		}

		public bool MatchOptional(string tok)
		{
			if (IsEOF)
			{
				return false;
			}
			if (! Lexum.StrEquals(tok))
			{
				return true;
			}

			return Next();
		}

		public string GetStringLit()
		{
			if (Lexum.Type == StringNodeType.Quoted)
			{
				return Lexum.Str;
			}

			Debug.Assert(Token == SqlToken.STRLIT);

			StringBuilder buf = new StringBuilder();

			if (Lexum.Str[Lexum.Str.Length - 1] == '\'')
			{
				if (Lexum.Str.Length > 1)
				{
					return Lexum.Str;
				}
				else
				{
					buf.Append(Lexum.Str);
					_pos++;
				}
			}

			while (!IsEOF && Lexum.Str[Lexum.Str.Length - 1] != '\'')
			{
				buf.Append(Lexum.Str);
				_pos++;
			}
			buf.Append(Lexum.Str);
			return buf.ToString();
		}

		public bool Next()
		{
			_pos++;

			if(IsEOF)
			{
				Token = SqlToken.EOF;
				return false;
			}

			if (Lexum.Str[0] == '\'')
			{
				Token = SqlToken.STRLIT;
				return true;
			}

			switch (Lexum.Str.ToUpper())
			{
				case "(":
					Token = SqlToken.LPAR;
					break;
				case ")":
					Token = SqlToken.RPAR;
					break;
				case ">":
					if (Lexum.Next.Str == "=")
					{
						_pos++;
						Token = SqlToken.GTEQ;
						Lexum.Str = ">=";
						break;
					}
					Token = SqlToken.GT;
					break;
				case "<":
					if (Lexum.Next.Str == "=")
					{
						_pos++;
						Token = SqlToken.LTEQ;
						Lexum.Str = "<=";
						break;
					}
					if (Lexum.Next.Str == ">")
					{
						_pos++;
						Token = SqlToken.NEQ;
						Lexum.Str = "<>";
						break;
					}
					Token = SqlToken.LT;
					break;
				case "*":
					Token = SqlToken.SPLAT;
					break;
				case ":":
					Token = SqlToken.COLON;
					break;
				case ";":
					Token = SqlToken.SEMI;
					break;
				case "=":
					Token = SqlToken.EQ;
					break;
				case ".":
					Token = SqlToken.DOT;
					break;
				case ",":
					Token = SqlToken.COMMA;
					break;
				case "CREATE":
					Token = SqlToken.CREATE;
					break;
				case "PROCEDURE":
					Token = SqlToken.PROC;
					break;
				case "SELECT":
					Token = SqlToken.SELECT;
					break;
				case "FROM":
					Token = SqlToken.FROM;
					break;
				case "AS":
					Token = SqlToken.AS;
					break;
				case "INT":
					Token = SqlToken.INT;
					break;
				case "SMALLINT":
					Token = SqlToken.SMALLINT;
					break;
				case "BIGINT":
					Token = SqlToken.BIGINT;
					break;
				case "CHAR":
					Token = SqlToken.CHAR;
					break;
				case "WHERE":
					Token = SqlToken.WHERE;
					break;
				case "AND":
					Token = SqlToken.AND;
					break;
				case "OR":
					Token = SqlToken.OR;
					break;
				case "INNER":
					Token = SqlToken.INNER;
					break;
				case "JOIN":
					Token = SqlToken.JOIN;
					break;
				case "LEFT":
					Token = SqlToken.LEFT;
					break;
				case "RIGHT":
					Token = SqlToken.RIGHT;
					break;
				case "ON":
					Token = SqlToken.ON;
					break;
				case "INSERT":
					Token = SqlToken.INSERT;
					break;
				case "INTO":
					Token = SqlToken.INTO;
					break;
				case "VALUES":
					Token = SqlToken.VALUES;
					break;
				case "UPDATE":
					Token = SqlToken.UPDATE;
					break;
				case "SET":
					Token = SqlToken.SET;
					break;
				case "ORDER":
					Token = SqlToken.ORDER;
					break;
				case "BY":
					Token = SqlToken.BY;
					break;
				case "DESC": 
					Token = SqlToken.DESC;
					break;
				case "ASC":
					Token = SqlToken.ASC;
					break;
				case "IN":
					Token = SqlToken.IN;
					break;
				case "DELETE":
					Token = SqlToken.DELETE;
					break;
				case "GROUP":
					Token = SqlToken.GROUP;
					break;
				case "HAVING":
					Token = SqlToken.HAVING;
					break;
				case "LIKE":
					Token = SqlToken.LIKE;
					break;
				case "IS":
					Token = SqlToken.IS;
					break;
				case "NULL":
					Token = SqlToken.NULL;
					break;
				case "NOT":
					Token = SqlToken.NOT;
					break;
				case "UNION":
					Token = SqlToken.UNION;
					break;
				case "CASE":
					Token = SqlToken.CASE;
					break;
				case "WHEN":
					Token = SqlToken.WHEN;
					break;
				case "THEN":
					Token = SqlToken.THEN;
					break;
				case "ELSE":
					Token = SqlToken.ELSE;
					break;
				case "END":
					Token = SqlToken.END;
					break;
				case "DISTINCT":
					Token = SqlToken.DISTINCT;
					break;
				case "BEGIN":
					Token = SqlToken.BEGIN;
					break;
				case "TRANSACTION":
					Token = SqlToken.TRANS;
					break;
				case "COMMIT":
					Token = SqlToken.COMMIT;
					break;
				case "ABORT":
					Token = SqlToken.ABORT;
					break;
				case "INVOKE":
					Token = SqlToken.INVOKE;
					break;
				case "ALTER":
					Token = SqlToken.ALTER;
					break;
				case "DECLARE":
					Token = SqlToken.DECLARE;
					break;
				case "SECTION":
					Token = SqlToken.SECTION;
					break;
				case "INCLUDE":
					Token = SqlToken.INCLUDE;
					break;
				case "CURSOR":
					Token = SqlToken.CURSOR;
					break;
				case "FOR":
					Token = SqlToken.FOR;
					break;
				case "OPEN":
					Token = SqlToken.OPEN;
					break;
				case "FETCH":
					Token = SqlToken.FETCH;
					break;
				case "BETWEEN":
					Token = SqlToken.BETWEEN;
					break;
				case "CLOSE":
					Token = SqlToken.CLOSE;
					break;
				case "DATETIME":
					Token = SqlToken.DATETIME;
					break;
				case "FREE":
					Token = SqlToken.FREE;
					break;
				case "ROLLBACK":
					Token = SqlToken.ROLLBACK;
					break;
				case "CONTROL":
					Token = SqlToken.CONTROL;
					break;
				case "CAST":
					Token = SqlToken.CAST;
					break;
				case "EXISTS":
					Token = SqlToken.EXISTS;
					break;
				case "BROWSE":
					Token = SqlToken.BROWSE;
					break;
				case "STABLE":
					Token = SqlToken.STABLE;
					break;
				case "INTERVAL":
					Token = SqlToken.INTERVAL;
					break;
				case "LOCK":
					Token = SqlToken.LOCK;
					break;
				case "SUBSTRING":
					Token = SqlToken.SUBSTRING;
					break;
				case "DATEFORMAT":
					Token = SqlToken.DATEFORMAT;
					break;
				case "POSITION":
					Token = SqlToken.POSITION;
					break;
				case "TRIM":
					Token = SqlToken.TRIM;
					break;
				case "PREPARE":
					Token = SqlToken.PREPARE;
					break;
				case "DESCRIBE":
					Token = SqlToken.DESCRIBE;
					break;
				case "EXECUTE":
					Token = SqlToken.EXECUTE;
					break;

				default:
					Token = SqlToken.ID;
					break;
			}

			return true;
		}
	}
}
