using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CobolParser.SQL.Conds;
using DOR.Core.Collections;

namespace CobolParser.SQL.Statements
{
	public class Fetch : SqlStatement
	{
		public ISqlCondToken CursorName
		{
			get;
			private set;
		}

		public Vector<CondParam> Prms
		{
			get;
			private set;
		}

		/// <summary>
		/// FETCH :SQL-CURSOR-NAME USING DESCRIPTOR :SQLDA-OUT
		/// </summary>
		public CondParam Descriptor
		{
			get;
			private set;
		}

		public Fetch(SqlLex lex, string sqlText)
		: base(sqlText)
		{
			Prms = new Vector<CondParam>();

			lex.Match(SqlToken.FETCH);

			CursorName = ISqlCondToken.Parse(lex, true, false, false);

			if (lex.Token == SqlToken.INTO)
			{
				lex.Match(SqlToken.INTO);

				while (!lex.IsEOF)
				{
					Prms.Add(new CondParam(lex));
					lex.MatchOptional(SqlToken.COMMA);
				}
			}

			if (!lex.IsEOF && lex.Lexum.StrEquals("USING"))
			{
				lex.Next();
				lex.Match("DESCRIPTOR");
				Descriptor = new CondParam(lex);
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
