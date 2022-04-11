using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DOR.Core.Collections;
using DOR.Core;
using CobolParser.Verbs;

namespace CobolParser.Divisions
{
	public class SubRoutine
	{
		public StringNode SubLexum
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			private set;
		}

		public Vector<IVerb> Verbs
		{
			get;
			private set;
		}

		public bool IsReferenced
		{
			get;
			set;
		}

		public SubRoutine(Terminalize terms)
		{
			SubLexum = terms.Current;

			Verbs = new Vector<IVerb>();

			while (terms.CurrentEquals("COPY"))
			{
				IVerb verb = VerbLookup.Create(terms, DivisionType.Procedure);
				Debug.Assert(null != verb);
				Verbs.Add(verb);
				terms.Match(StringNodeType.Period);
			}

			Name = terms.Current.Str;
			if (!IsSubName(terms))
			{
				throw new SyntaxError(terms.Current.FileName, terms.Current.LineNumber, "Invalid sub routine name " + terms.Current.Str);
			}
			terms.Next();
			terms.MatchOptional("SECTION");
			terms.Match(StringNodeType.Period);

			while (!terms.IsIterationComplete && !IsSubName(terms))
			{
				IVerb verb = VerbLookup.Create(terms, DivisionType.Procedure);
				if (null == verb)
				{
					throw new SyntaxError(terms.Current.FileName, terms.Current.LineNumber, "Verb not found " + terms.Current.Str);
				}
				Verbs.Add(verb);
			}
		}

		public static bool IsSubName(Terminalize terms)
		{
			return terms.Current.Type == StringNodeType.Word &&
				!terms.CurrentEquals("END-TRANSACTION") &&
				!terms.CurrentEquals("ABORT-TRANSACTION") &&
				!terms.CurrentEquals("BEGIN-TRANSACTION") &&
				(terms.CurrentNext(1).Type == StringNodeType.Period
				|| terms.CurrentNextEquals(1, "SECTION"));
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

		public string CommentProbe()
		{
			if (SubLexum == null)
			{
				return null;
			}

			if (SubLexum.Prev.Type == StringNodeType.Comment)
			{
				StringBuilder buf = new StringBuilder();
				StringNode node = SubLexum.Prev;

				while (node.Type == StringNodeType.Comment)
				{
					node = node.Prev;
				}
				node = node.Next;
				while (node.Type == StringNodeType.Comment)
				{
					buf.Append(node.Str);
					if (node.Next.Type == StringNodeType.Comment)
					{
						buf.Append("\r\n");
					}
					node = node.Next;
				}

				string cmt = buf.ToString();
				if (cmt == "/")
				{
					return "";
				}
				return cmt;
			}

			return null;
		}
	}
}
