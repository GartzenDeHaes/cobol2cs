using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using CobolParser.Division;
using CobolParser.Expressions;
using CobolParser.Expressions.Terms;
using CobolParser.Records;
using CobolParser.Parser;
using CobolParser.Verbs;
using CobolParser.SQL.Statements;
using DOR.Core.Collections;
using CobolParser.Text;

namespace CobolParser
{
	public class CobolProgram
	{
		public GuardianPath FileName
		{
			get;
			private set;
		}

		public IdentificationDiv Identification
		{
			get;
			private set;
		}

		public EnvironmentDiv Environment
		{
			get;
			private set;
		}

		public DataDiv Data
		{
			get;
			private set;
		}

		public ProcedureDiv Procedure
		{
			get;
			private set;
		}

		public int LineCount
		{
			get;
			private set;
		}

		public List<GuardianPath> SearchFiles
		{
			get { return Identification.SearchFiles; }
		}

		public string ProgramId
		{
			get 
			{ 
				return null == Identification ? "UNKNOWN" : Identification.ProgramId; 
			}
		}

		public string Namespace
		{
			get
			{
				return CsFieldNameConverter.Convert(ProgramId);
			}
		}

		public string Author
		{
			get
			{
				var a = null == Identification ? "UNKNOWN" : Identification.Author;
				return a == null ? "" : a;
			}
		}

		public string WrittenOn
		{
			get
			{
				var w = null == Identification ? "" : Identification.DateWritten;
				return w == null ? "" : w;
			}
		}

		public string FileHeaderComments
		{
			get
			{
				string c = null == Identification ? "" : Identification.HeaderComments;
				if (c.Length > 8096)
				{
					return c.Substring(0, 8096);
				}
				return c;
			}
		}

		public string MainSub
		{
			get;
			set;
		}

		public static SymbolTable CurrentSymbolTable
		{
			get;
			private set;
		}

		public SymbolTable SymbolTable
		{
			get;
			private set;
		}

		public string SourceDirectory
		{
			get { return CsFieldNameConverter.Convert(ProgramId) + Path.DirectorySeparatorChar; }
		}

		static CobolProgram()
		{
			CurrentSymbolTable = new SymbolTable();
		}

		public CobolProgram(GuardianPath gpath)
		{
			FileName = gpath;
			SymbolTable = CurrentSymbolTable;

			Terminalize terms;

			using (StreamReader reader = File.OpenText(gpath.WindowsFileName()))
			{
				terms = new Terminalize(gpath, reader);
			}

			LineCount = terms.LineCount;

			terms.BeginIteration();
			terms.Next();

			Identification = new IdentificationDiv();
			Identification.Parse(terms);

			Environment = new EnvironmentDiv();
			Environment.Parse(terms);

			Data = new DataDiv();
			Data.Parse(terms);

			Procedure = new ProcedureDiv();
			Procedure.Parse(terms);
		}

		public List<string> ListTables()
		{
			List<string> lst = new List<string>();

			Data.ListTables(lst);
			Procedure.ListTables(lst);

			return lst;
		}

		private Dictionary<string, string> _turnTargetList;

		public bool IsFieldTurnTarget(ScreenField sfield)
		{
			if (_turnTargetList == null)
			{
				_turnTargetList = new Dictionary<string, string>();
				
				foreach (var turn in Procedure.ListTurns())
				{
					foreach (var t in turn.Fields.Items)
					{
						ScreenField f = Data.ScreenRecord.FindScreenField(t);
						string key = f.Name;
						if (_turnTargetList.ContainsKey(key))
						{
							continue;
						}
						_turnTargetList.Add(key, key);
					}
				}
			}

			return _turnTargetList.ContainsKey(sfield.Name);
		}

		public bool IsFieldTurnTargetWithAttibute(ScreenField sfield, string attr)
		{
			foreach (var turn in Procedure.ListTurns())
			{
				if (attr == "*" || !turn.HasAttibute(attr))
				{
					continue;
				}

				foreach (var t in turn.Fields.Items)
				{
					ScreenField f = Data.ScreenRecord.FindScreenField(t);
					if (f == null)
					{
						// missing field
						// TURN TEMP PROTECTED   IN                   SC-BR-R700-MAIN-1.
						continue;
					}
					if (f.Name.Equals(sfield.Name, StringComparison.InvariantCultureIgnoreCase))
					{
						return true;
					}
				}
			}
			return false;
		}

		public INamedField FindFieldForTerm(object term)
		{
			if (term is INamedField)
			{
				return (INamedField)term;
			}

			if (term is ScreenField)
			{
				if (((ScreenField)term).FromExpr != null)
				{
					return FindFieldForTerm(((ScreenField)term).FromExpr);
				}
				else if (((ScreenField)term).UsingExpr != null)
				{
					return FindFieldForTerm(((ScreenField)term).UsingExpr);
				}
				else if (((ScreenField)term).ToExpr != null)
				{
					return FindFieldForTerm(((ScreenField)term).ToExpr);
				}
				else
				{
					return null;
				}
			}

			ITerm t = term as ITerm;
			if (t == null)
			{
				if (term is ExprTerm)
				{
					if (((ExprTerm)term).InnerExpression is Expr)
					{
						if (((Expr)((ExprTerm)term).InnerExpression).Terms.Count == 1)
						{
							t = ((Expr)((ExprTerm)term).InnerExpression).Terms[0];
						}
					}
				}
				else if (term is Expr)
				{
					if (((Expr)term).Terms.Count == 1)
					{
						t = ((Expr)term).Terms[0];
					}
				}
				if (t == null)
				{
					return null;
				}
			}

			if (t is Id)
			{
				Id id = (Id)t;
				return Data.LocateField(id.Value.Str, null);
			}
			else if (t is OffsetReference)
			{
				OffsetReference offref = (OffsetReference)t;
				return Data.LocateField
				(
					offref.OffsetChain[0].Value.Str,
					offref.OffsetChain[offref.OffsetChain.Count - 1].Value.Str
				);
			}
			return null;
		}

		public bool IsNative(object term)
		{
			INamedField f;
			if (term is INamedField)
			{
				f = (INamedField)term;
			}
			else if (term is ScreenField)
			{
				return false;
			}
			else
			{
				f = FindFieldForTerm(term);
			}
			if (f == null)
			{
				return false;
			}

			if (f.Attributes != null && f.Attributes.Pic != null)
			{
				if (f.Attributes.Pic.IsNativeBinary)
				{
					return true;
				}
			}

			bool ret = f.CanBeNative() && Data.FieldCouldBeNative((Offset)f);
			if (!ret)
			{
				return false;
			}

			if
			(
				Environment.InputOutputSection != null &&
				Data.Files != null
			)
			{
				foreach (var fd2 in Data.Files.Fds())
				{
					foreach (var o in fd2.DataRecords)
					{
						if (f.FullyQualifiedName == o.FullyQualifiedName)
						{
							return false;
						}
					}
				}
			}

			Vector<IVerb> vlist = new Vector<IVerb>();

			foreach (var sub in Procedure.SubRoutines)
			{
				vlist.AddRange(sub.Verbs);
				while (vlist.Count > 0)
				{
					var vrb = vlist.Pop();

					if (vrb is ExecSql)
					{
						if (((ExecSql)vrb).Sql.HasParameterOf(f.FullyQualifiedName))
						{
							return false;
						}
					}
					else if (vrb is If)
					{
						vlist.AddRange(((If)vrb).Stmts.Stmts);
						if (((If)vrb).ElseStmts != null)
						{
							vlist.AddRange(((If)vrb).ElseStmts.Stmts);
						}
					}
					else if (vrb is Evaluate)
					{
						foreach (var when in ((Evaluate)vrb).Whens)
						{
							vlist.AddRange(when.Stmts.Stmts);
						}
					}
				}
			}

			return ret;
		}

		public string CsClrTypeNmae(object term)
		{
			ITerm t = term as ITerm;
			if (t == null)
			{
				if (term is ExprTerm)
				{
					if (((ExprTerm)term).InnerExpression is Expr)
					{
						if (((Expr)((ExprTerm)term).InnerExpression).Terms.Count == 1)
						{
							t = ((Expr)((ExprTerm)term).InnerExpression).Terms[0];
						}
					}
				}
				if (t == null)
				{
					return "object";
				}
			}

			if (t is Id)
			{
				Id id = (Id)t;
				if (id.Offsets.Count > 0)
				{
					return "IBufferOffset";
				}
				INamedField f = Data.LocateField(id.Value.Str, null);
				return ((Offset)f).Attributes.Pic.CsClrTypeName();
			}
			else if (t is OffsetReference)
			{
				OffsetReference offref = (OffsetReference)t;
				INamedField f = Data.LocateField
				(
					offref.OffsetChain[0].Value.Str,
					offref.OffsetChain[offref.OffsetChain.Count - 1].Value.Str
				);
				return ((Offset)f).Attributes.Pic.CsClrTypeName();
			}
			return "object";
		}

		public void PruneUnusedScreens()
		{
			if (Data.ScreenRecord == null)
			{
				return;
			}

			List<Display> lst = Procedure.ListVerbs<Display>();

			foreach (var v in lst)
			{
				for (int x = 0; x < v.Terms.Count; x++)
				{
					Screen s = Data.ScreenRecord.FindScreen(v.Terms[x].ToString());
					if (s == null)
					{
						continue;
					}
					s.IsDisplayed = true;
				}
			}

			List<Screen> toDel = new List<Screen>();

			foreach (var s in Data.ScreenRecord.Screens)
			{
				if (!s.IsDisplayed)
				{
					toDel.Add(s);
				}
			}

			foreach (var s in toDel)
			{
				Console.WriteLine("Removing unused screen {0}", s.Name);
				Data.ScreenRecord.Screens.RemoveElement(s);
			}
		}

		public List<T> ListVerbs<T>() where T : IVerb
		{
			List<T> verbs = Procedure.ListVerbs<T>();

			if (Data.Verbs != null)
			{
				verbs.AddRange(Data.Verbs.ToArray().Where(v => v is T).Select(v => (T)v));
			}

			return verbs;
		}

		public List<DeclareCursor> ListCursors()
		{
			List<ExecSql> lst = ListVerbs<ExecSql>();
			lst = lst.Where(e => e.Sql is CobolParser.SQL.Statements.DeclareCursor).ToList();
			return lst.Select(e => (CobolParser.SQL.Statements.DeclareCursor)e.Sql).ToList();
		}
	}
}
