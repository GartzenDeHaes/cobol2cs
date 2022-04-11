using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core;

namespace CobolParser.SQL.Conds
{
	public class CondInterval : ISqlCondToken
	{
		public string Thing1
		{
			get;
			private set;
		}

		public string Thing2
		{
			get;
			private set;
		}

		public string Offset
		{
			get;
			private set;
		}

		public CondInterval(SqlLex lex)
		{
			lex.Match(SqlToken.INTERVAL);
			Thing1 = lex.Lexum.Str;
			lex.Next();
			Thing2 = lex.Lexum.Str;
			lex.Next();

			if (lex.Token == SqlToken.LPAR)
			{
				lex.Next();
				Offset = lex.Lexum.Str;
				lex.Next();
				lex.Match(SqlToken.RPAR);
			}
		}

		public override void GetParameters(List<CondParam> lst)
		{
		}

		public override string ToString()
		{
			return "INTERVAL " + StringHelper.EscapeString(Thing1).Replace("\\'", "'") + " " + StringHelper.EscapeString(Thing2).Replace("\\'", "'") + (String.IsNullOrEmpty(Offset) ? " " : "(" + Offset + ") ");
		}
	}
}
