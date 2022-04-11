using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DOR.Core;
using DOR.Core.Collections;
using CobolParser.Verbs;
using CobolParser.Records;
using CobolParser.Sections;
using CobolParser.SQL.Statements;
using CobolParser.SQL.Conds;
using CobolParser.Parser;

namespace CobolParser.Division
{
	public class DataDiv : IDivision
	{
		public Vector<IVerb> Verbs
		{
			get;
			private set;
		}

		public FileSection Files
		{
			get;
			private set;
		}

		public LinkageSection LinkageSect
		{
			get;
			private set;
		}

		public MessageSection MessageSect
		{
			get;
			private set;
		}

		public WorkingStorageSection WorkingStorage
		{
			get;
			private set;
		}

		public ScreenSection ScreenRecord
		{
			get;
			private set;
		}

		public Storage Data
		{
			get;
			private set;
		}

		public DataDiv()
		: base(DivisionType.Data)
		{
		}

		public INamedField LocateField(string name, string parentName)
		{
			INamedField o = WorkingStorage.Data.LocateField(name, parentName);
			if (o != null)
			{
				return o;
			}
			o = Data.LocateField(name, parentName);
			if (o != null)
			{
				return o;
			}

			Debug.WriteLine("DataDiv.LocateField: can't find " + name);
			return null;
		}

		public override void Parse(Terminalize terms)
		{
			Verbs = new Vector<IVerb>();
			Data = new Storage();

			terms.Match("DATA");
			terms.Match("DIVISION");
			terms.Match(StringNodeType.Period);

			if (terms.CurrentEquals("FILE"))
			{
				Files = new FileSection(terms, Data);
			}

			WorkingStorage = new WorkingStorageSection(terms, Data);

			while
			(
				StringHelper.IsInt(terms.Current.Str) ||
				terms.CurrentEquals("DEF") ||
				terms.CurrentEquals("EXEC") ||
				terms.CurrentEquals("COPY") ||
				(
					terms.Current.Type == StringNodeType.QuestionMark && 
					terms.CurrentNextEquals(1, "SOURCE")
				)
			)
			{
				if (StringHelper.IsInt(terms.Current.Str) || terms.CurrentEquals("DEF"))
				{
					WorkingStorage.Parse(terms);
				}
				else
				{
					IVerb v = VerbLookup.Create(terms, DivisionType);
					Verbs.Add(v);
					terms.MatchOptional(".", "Expected period after DIVISION sentence.");
				}

				if (terms.CurrentEquals("EXTENDED-STORAGE"))
				{
					terms.Next();
					terms.Match("SECTION");
					terms.Match(StringNodeType.Period);
				}
			}

			if (terms.CurrentEquals("LINKAGE"))
			{
				LinkageSect = new LinkageSection(terms, WorkingStorage);
			}

			if (terms.CurrentEquals("MESSAGE"))
			{
				MessageSect = new MessageSection(terms);
			}

			if (terms.CurrentEquals("SCREEN"))
			{
				ScreenRecord = new ScreenSection(terms);
			}

			if (!terms.CurrentEquals("PROCEDURE"))
			{
				throw new SyntaxError(terms.FileName, terms.Current.LineNumber, "Expected PROCEDURE, found " + terms.Current.Str);
			}

			WorkingStorage.Data.FixupFillers();
			WorkingStorage.Data.FixupRedefineHints();
		}

		public void ListTables(List<string> lst)
		{
			foreach(IVerb v in Verbs)
			{
				ExecSql sql = v as ExecSql;
				if (null != sql)
				{
					sql.Sql.ListTables(lst);
				}
			}

			if (null != LinkageSect)
			{
				LinkageSect.ListTables(lst);
			}
		}

		public bool FieldCouldBeNative(Offset o)
		{
			foreach (IVerb v in Verbs)
			{
				ExecSql sql = v as ExecSql;
				if (sql == null || sql.Sql == null)
				{
					continue;
				}
				DeclareCursor dc = sql.Sql as DeclareCursor;
				if (dc == null)
				{
					continue;
				}
				if (dc.Stmt is Select)
				{
					Select s = (Select)dc.Stmt;
					foreach (var t in s.Where.Terms)
					{
						if (t is CondParam)
						{
							CondParam prm = (CondParam)t;
							if (prm.FieldName.Equals(o.Name, StringComparison.InvariantCultureIgnoreCase))
							{
								return false;
							}
							if (prm.RecordName.Equals(o.Name, StringComparison.InvariantCultureIgnoreCase))
							{
								return false;
							}
						}
					}
				}
				else
				{
					throw new NotImplementedException();
				}
			}

			return true;
		}
	}
}
