using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CobolParser.SQL.Conds
{
	public abstract class ISqlCondToken
	{
		public virtual void ListTables(List<string> lst)
		{
		}

		public abstract void GetParameters(List<CondParam> lst);

		public static ISqlCondToken Parse
		(
			SqlLex lex, 
			bool skipAs, 
			bool allowBetween,
			bool breakOnComma
		)
		{
			Debug.Assert(! lex.IsEOF);

			ISqlCondToken ret = null;

			if (lex.Token == SqlToken.EXISTS)
			{
				ret = new CondExists(lex);
			}
			else if (lex.Token == SqlToken.CAST)
			{
				ret = new CastCond(lex);
			}
			else if (lex.Token == SqlToken.SUBSTRING)
			{
				return new CondSubstring(lex);
			}
			else if (lex.Token == SqlToken.DATEFORMAT)
			{
				ret = new CondDateFormat(lex);
			}
			else if (lex.Token == SqlToken.POSITION)
			{
				ret = new CondPostion(lex);
			}
			else if (lex.Token == SqlToken.TRIM)
			{
				ret = new CondTrim(lex);
			}
			else if (lex.Lexum.StrEquals("CURRENT"))
			{
				ret = new Current(lex);
			}
			else if (lex.Lexum.StrEquals("NULL"))
			{
				return new CondNull(lex);
			}
			else if (lex.Token == SqlToken.IN)
			{
				return new CondIn(lex);
			}
			else if (CondOperator.IsOperator(lex.Token))
			{
				return new CondOperator(lex);
			}
			else if (lex.Token == SqlToken.INTERVAL)
			{
				return new CondInterval(lex);
			}
			else if (lex.Lexum.Type == StringNodeType.Colon)
			{
				ret = new CondParam(lex);
				if (!breakOnComma && lex.Token == SqlToken.COMMA && allowBetween)
				{
					return new CondFieldList(lex, ret);
				}
			}
			else if (lex.Token == SqlToken.LPAR)
			{
				if (lex.Lexum.Next.StrEquals("SELECT"))
				{
					ret = new CondSubSelect(lex);
				}
				else
				{
					ret = new CondExpr(lex, false);
				}
			}
			else if (lex.Token == SqlToken.BETWEEN)
			{
				return new CondBetween(lex);
			}
			else if 
			(
				!CondOperator.IsOperator(lex.Token) && 
				lex.Lexum.Next.Type == StringNodeType.LPar)
			{
				ret = new CondFunction(lex);
				if (CondOperator.IsOperator(lex.Token))
				{
					CondExpr expr = new CondExpr();
					expr.Expr[0].Terms.Add(ret);
					expr.Parse(lex, breakOnComma);
					ret = expr;
				}
			}
			else if (lex.Token == SqlToken.CASE)
			{
				return new CondCase(lex);
			}
			else if (lex.Token == SqlToken.STRLIT || lex.Lexum.Type == StringNodeType.Number)
			{
				ret = new CondValue(lex);
				if (!breakOnComma && lex.Token == SqlToken.COMMA && allowBetween)
				{
					return new CondFieldList(lex, ret);
				}
			}
			else if (lex.Token == SqlToken.DATETIME)
			{
				return new CondDateTime(lex);
			}

			if (null == ret)
			{
				ret = new CondField(lex, skipAs);

				if (!breakOnComma && lex.Token == SqlToken.COMMA && allowBetween)
				{
					ret = new CondFieldList(lex, ret);
				}
			}

			return ret;
		}
	}
}
