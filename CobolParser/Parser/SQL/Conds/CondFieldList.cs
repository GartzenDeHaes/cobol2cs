using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core.Collections;

namespace CobolParser.SQL.Conds
{
	public class CondFieldList : ISqlCondToken
	{
		public Vector<ISqlCondToken> Fields
		{
			get;
			private set;
		}

		public CondFieldList(SqlLex lex)
		{
			Fields = new Vector<ISqlCondToken>();

			if (lex.Token != SqlToken.COMMA)
			{
				// token is comma when called from the 2nd constructor
				Fields.Add(ISqlCondToken.Parse(lex, false, false, false));
			}

			while 
			(
				lex.Token == SqlToken.COMMA && 
				(
					lex.Lexum.Next.Type == StringNodeType.Colon ||
					lex.Lexum.Next.Type == StringNodeType.Word ||
					lex.Lexum.Next.Type == StringNodeType.Number ||
					lex.Lexum.Next.Type == StringNodeType.Quoted 
				)
			)
			{
				lex.Match(SqlToken.COMMA);
				Fields.Add(new CondExpr(lex, true));
			}
		}

		public CondFieldList(SqlLex lex, ISqlCondToken field)
		: this(lex)
		{
			Fields.InsertElementAt(field, 0);
		}

		public override void GetParameters(List<CondParam> lst)
		{
			for (int x = 0; x < Fields.Count; x++)
			{
				Fields[x].GetParameters(lst);
			}
		}
	}
}
