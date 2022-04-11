using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DOR.Core.Config;
using DOR.Core.Collections;
using CobolParser.SQL;
using CobolParser.SQL.Statements;
using CobolParser.TandemData;

namespace CobolParser.Verbs
{
	public class ExecSql : IVerb
	{
		public const string Lexum = "EXEC SQL";

		public Vector<StringNode> Texts
		{
			get;
			private set;
		}

		public SqlStatement Sql
		{
			get;
			private set;
		}

		public string SqlText
		{
			get;
			private set;
		}

		public ExecSql(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.ExecSql;

			Texts = new Vector<StringNode>(30);

			terms.Match("EXEC");
			terms.Match("SQL");

			int startCharPos = terms.Current.CharPos;

			StringBuilder txt = new StringBuilder();

			while (! terms.Current.Str.Equals("END-EXEC"))
			{
				txt.Append(terms.Current.Str);
				txt.Append(" ");

				Texts.Add(terms.Current);
				if (!terms.Next())
				{
					throw new SyntaxError(terms.Current.FileName, terms.Current.LineNumber, "EOF processing EXEC SQL");
				}
			}

			SqlText = terms.Current.Capture(startCharPos);

			terms.Match("END-EXEC");

			Sql = SqlParser.Parse(Texts, txt.ToString());

			if (Sql is Include)
			{
				Include inc = (Include)Sql;

				if (inc.IncludeName.Equals("Structures", StringComparison.InvariantCultureIgnoreCase))
				{
					return;
				}

				Terminalize ins = new Terminalize(new GuardianPath("$D00.DEFINE.INVOKES"));
				GuardianPath filename = new GuardianPath("$d00.Clibsql.Clibsql");

				if (inc.IncludeName.Equals("SQLCA", StringComparison.InvariantCultureIgnoreCase))
				{
					ins.Add(new StringNode(filename, 0, 0, "01"));
					ins.Add(new StringNode(filename, 0, 0, inc.IncludeName));
					ins.Add(new StringNode(filename, 0, 0, "."));

					ins.Add(new StringNode(filename, 0, 0, "05"));
					ins.Add(new StringNode(filename, 0, 0, "ERRCODE"));
					ins.Add(new StringNode(filename, 0, 0, "PIC"));
					ins.Add(new StringNode(filename, 0, 0, "S9"));
					ins.Add(new StringNode(filename, 0, 0, "("));
					ins.Add(new StringNode(filename, 0, 0, "4"));
					ins.Add(new StringNode(filename, 0, 0, ")"));
					ins.Add(new StringNode(filename, 0, 0, "SIGN"));
					ins.Add(new StringNode(filename, 0, 0, "LEADING"));
					ins.Add(new StringNode(filename, 0, 0, "SEPARATE"));
					ins.Add(new StringNode(filename, 0, 0, "OCCURS"));
					ins.Add(new StringNode(filename, 0, 0, "2"));
					ins.Add(new StringNode(filename, 0, 0, "TIMES"));
					ins.Add(new StringNode(filename, 0, 0, "."));

					ins.Add(new StringNode(filename, 0, 0, "05"));
					ins.Add(new StringNode(filename, 0, 0, "PROCEDURE-ID"));
					ins.Add(new StringNode(filename, 0, 0, "PIC"));
					ins.Add(new StringNode(filename, 0, 0, "9999"));
					ins.Add(new StringNode(filename, 0, 0, "."));

					ins.Add(new StringNode(filename, 0, 0, "05"));
					ins.Add(new StringNode(filename, 0, 0, "ROWS"));
					ins.Add(new StringNode(filename, 0, 0, "PIC"));
					ins.Add(new StringNode(filename, 0, 0, "9999"));
				}
				else
				{
					ins.Add(new StringNode(filename, 0, 0, "01"));
					ins.Add(new StringNode(filename, 0, 0, inc.IncludeName));
					ins.Add(new StringNode(filename, 0, 0, "PIC"));
					ins.Add(new StringNode(filename, 0, 0, "S9"));
					ins.Add(new StringNode(filename, 0, 0, "("));
					ins.Add(new StringNode(filename, 0, 0, "4"));
					ins.Add(new StringNode(filename, 0, 0, ")"));
				}

				terms.Prev();
				terms.InjectAfterCurrent(ins);
				terms.Next();
			}
			else if (Sql is Invoke)
			{
				IList<ColumnDef> defs;
				Invoke invoke = (Invoke)Sql;

				try
				{
					using (var data = new TandemDataAccess(Configuration.AppConfig))
					{
						defs = data.Invoke(invoke.Database.ToString());
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("INVOKE error for {0} {1}", invoke.Database, ex.Message);
					defs = new List<ColumnDef>();
				}

				Terminalize ins = new Terminalize(new GuardianPath("$D00.DEFINE.INVOKES"));
				ins.Add(new StringNode(ins.FileName, 0, 0, "01"));
				ins.Add(new StringNode(ins.FileName, 0, 0, invoke.RecordName));

				foreach(var c in defs)
				{
					ins.Add(new StringNode(ins.FileName, 0, 0, "."));

					ins.Add(new StringNode(ins.FileName, 0, 0, "05"));
					ins.Add(new StringNode(ins.FileName, 0, 0, c.Name.Replace("_", "-")));
					ins.Add(new StringNode(ins.FileName, 0, 0, "PIC"));

					if (c.IsSigned)
					{
						ins.Add(new StringNode(ins.FileName, 0, 0, "S9"));
					}
					else if 
					(
						c.SqlType == "INTEGER" || 
						c.SqlType == "BIGINT" || 
						c.SqlType == "SMALLINT" ||
						c.SqlType == "DECIMAL" ||
						(c.SqlType != "TIMESTAMP" && c.Scale != 0)
					)
					{
						ins.Add(new StringNode(ins.FileName, 0, 0, "9"));
					}
					else
					{
						ins.Add(new StringNode(ins.FileName, 0, 0, "X"));
					}

					ins.Add(new StringNode(ins.FileName, 0, 0, "("));
					ins.Add(new StringNode(ins.FileName, 0, 0, c.Precision.ToString()));
					ins.Add(new StringNode(ins.FileName, 0, 0, ")"));

					if (c.SqlType != "TIMESTAMP" && c.Scale != 0)
					{
						ins.Add(new StringNode(ins.FileName, 0, 0, "V9"));
						ins.Add(new StringNode(ins.FileName, 0, 0, "("));
						ins.Add(new StringNode(ins.FileName, 0, 0, c.Scale.ToString()));
						ins.Add(new StringNode(ins.FileName, 0, 0, ")"));
					}
				}

				terms.Prev();
				terms.InjectAfterCurrent(ins);
				terms.Next();
			}
		}
	}
}
