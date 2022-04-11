using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CobolParser.SQL
{
	public class SqlField
	{
		public string FieldName
		{
			get;
			private set;
		}

		public string Alias
		{
			get;
			private set;
		}

		public string TableAlias
		{
			get;
			private set;
		}

		public SqlField(SqlLex lex, bool skipAs)
		{
			Debug.Assert(!lex.IsEOF);

			if 
			(
				!lex.IsEOF && 
				lex.Lexum.Next != null && 
				lex.Lexum.Next.Type == StringNodeType.Period
			)
			{
			}
			else
			{
				FieldName = lex.Lexum.Str;
				Debug.Assert(lex.Token == SqlToken.ID || lex.Token == SqlToken.SPLAT);
				lex.Next();
			}

			if 
			(
				lex.Token == SqlToken.AS &&
				! skipAs
			)
			{
				lex.Match(SqlToken.AS);
				Alias = lex.Lexum.Str;
				lex.Match(SqlToken.AS);
			}
		}

		public override string ToString()
		{
			if (String.IsNullOrEmpty(TableAlias))
			{
				return FieldName;
			}
			return TableAlias + "." + FieldName;
		}
	}
}
