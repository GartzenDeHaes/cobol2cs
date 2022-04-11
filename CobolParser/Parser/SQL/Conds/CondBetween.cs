using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core.Collections;
using System.Diagnostics;

namespace CobolParser.SQL.Conds
{
	public class CondBetween : ISqlCondToken
	{
		public CondFieldList Lows
		{
			get;
			private set;
		}

		public CondFieldList Highs
		{
			get;
			private set;
		}

		public CondBetween(SqlLex lex)
		{
			lex.Match(SqlToken.BETWEEN);

			int exprectRpar = 0;
			if (lex.Lexum.Type == StringNodeType.LPar)
			{
				lex.Match(SqlToken.LPAR);
				exprectRpar = 1;
			}

			Lows = new CondFieldList(lex);

			if (exprectRpar > 0/* && lex.Lexum.Type == StringNodeType.RPar*/)
			{
				lex.Match(SqlToken.RPAR);
				exprectRpar--;
			}

			lex.MatchOptional(SqlToken.AND);

			if (lex.Lexum.Type == StringNodeType.LPar)
			{
				lex.Match(SqlToken.LPAR);
				exprectRpar++;
			}
			Highs = new CondFieldList(lex);

			while (exprectRpar > 0)
			{
				lex.Match(SqlToken.RPAR);
				exprectRpar--;
			}
		}

		public override void GetParameters(List<CondParam> lst)
		{
			Highs.GetParameters(lst);
			Lows.GetParameters(lst);
		}
	}
}
