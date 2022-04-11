using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CobolParser.SQL
{
	public class SqlTable
	{
		public string TableName
		{
			get;
			private set;
		}

		public string Alias
		{
			get;
			private set;
		}

		public SqlTable(SqlLex lex)
		{
			Debug.Assert(!lex.IsEOF);

			lex.MatchOptional(SqlToken.EQ);
			TableName = lex.Lexum.Str;
			lex.Next();

			if (!lex.IsEOF && lex.Token == SqlToken.ID)
			{
				Alias = lex.Lexum.Str;
				lex.Match(SqlToken.ID);
			}
		}

		public override string ToString()
		{
			string name = TableName.Equals("CASE", StringComparison.InvariantCultureIgnoreCase) ? "CMCASE" : TableName;
			return "[=defineOf]" + name + " " + Alias;
		}
	}
}
