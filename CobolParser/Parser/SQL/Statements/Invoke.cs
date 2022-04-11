using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.SQL.Conds;

namespace CobolParser.SQL.Statements
{
	public class Invoke : SqlStatement
	{
		public GuardianDefine Database
		{
			get;
			private set;
		}

		public GuardianPath Path
		{
			get;
			private set;
		}

		public string RecordName
		{
			get;
			private set;
		}

		public Invoke(SqlLex lex, string sqlText)
		: base(sqlText)
		{
			lex.Match(SqlToken.INVOKE);

			if (lex.Token == SqlToken.EQ)
			{
				lex.Match(SqlToken.EQ);
				Database = new GuardianDefine(lex.Lexum.Str);
				lex.Next();

				///TODO: database tables are added yet
				//Path = ImportManager.ResolveDefine(Database);
			}
			else
			{
				Path = new GuardianPath(lex.Lexum.Str);
				lex.Next();
			}

			if (lex.Token == SqlToken.AS)
			{
				lex.Match(SqlToken.AS);
				RecordName = lex.Lexum.Str;
				lex.Match(SqlToken.ID);
			}
			else
			{
				RecordName = Database.DefineName;
			}
		}

		public override void ListTables(List<string> lst)
		{
			if (null != Database)
			{
				lst.Add(Database.ToString());
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
