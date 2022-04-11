using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.SQL.Conds;

namespace CobolParser.SQL.Statements
{
	/// <summary>
	/// Used with PREPARE to get metadata (see s205ss1q)
	/// </summary>
	public class Describe : SqlStatement
	{
		public bool IsInput
		{
			get;
			private set;
		}

		public CondParam SqlStatementName
		{
			get;
			private set;
		}

		public CondParam SqlStatementNameDestField
		{
			get;
			private set;
		}

		public CondParam NamesDestField
		{
			get;
			private set;
		}

		public CondParam CollationsDestField
		{
			get;
			private set;
		}

		public Describe(SqlLex lex, string sqlText)
		: base(sqlText)
		{
			lex.Match(SqlToken.DESCRIBE);

			if (lex.Lexum.StrEquals("INPUT"))
			{
				IsInput = true;
				lex.Next();
			}

			while 
			(
				!lex.IsEOF && 
				(
					lex.Lexum.Next.StrEquals("INTO") ||
					lex.Lexum.Type == StringNodeType.Colon
				)
			)
			{
				if (lex.Token == SqlToken.COLON)
				{
					SqlStatementName = new CondParam(lex);
					lex.Match("INTO");
					SqlStatementNameDestField = new CondParam(lex);
				}
				else if (lex.Lexum.StrEquals("NAMES"))
				{
					lex.Next();
					lex.Match("INTO");
					NamesDestField = new CondParam(lex);
				}
				else if (lex.Lexum.StrEquals("COLLATIONS"))
				{
					lex.Next();
					lex.Match("INTO");
					CollationsDestField = new CondParam(lex);
				}
				else
				{
					throw new SyntaxError(lex.Lexum.FileName, lex.Lexum.LineNumber, "Unknown describe class of " + lex.Lexum.Str);
				}
			}
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
