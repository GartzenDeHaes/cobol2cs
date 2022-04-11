using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DOR.Core.Collections;
using CobolParser.Verbs;
using CobolParser.Parser;

namespace CobolParser.Sections
{
	public class LinkageSection : Section
	{
		public LinkageSection(Terminalize terms, WorkingStorageSection ws)
		: base(terms.Current)
		{
			terms.Match("LINKAGE");
			terms.Match("SECTION");
			terms.Match(StringNodeType.Period);

			while 
			(
				!terms.CurrentNextEquals(1, "SECTION") &&
				!terms.CurrentNextEquals(1, "DIVISION")
			)
			{
				if (terms.CurrentEquals("COPY"))
				{
					VerbLookup.Create(terms, DivisionType.Data);
					terms.Match(StringNodeType.Period);
				}
				else if (terms.CurrentEquals("EXEC"))
				{
					IVerb verb = VerbLookup.Create(terms, DivisionType.Data);
					Verbs.Add(verb);
				}
				else
				{
					ws.Data.AddOne(terms, "");
				}
			}
		}

		public void ListTables(List<string> lst)
		{
			foreach (IVerb v in Verbs)
			{
				ExecSql sql = v as ExecSql;
				if (null != sql)
				{
					sql.Sql.ListTables(lst);
				}
			}
		}
	}
}
