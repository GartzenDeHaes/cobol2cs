using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using CobolParser.Verbs;
using CobolParser.Verbs.Phrases;
using CobolParser.Expressions;
using CobolParser.Expressions.Terms;
using CobolParser.Records;
using CobolParser.Divisions.Proc;
using DOR.Core;
using DOR.Core.Collections;
using System.IO;
using CobolParser.Generator;

namespace CobolParser.Text
{
	public class CsVerbs : IDisposable
	{
		private CobolProgram _prog;
		private ISourceWriter _writer;
		private CsSql _csSql;

		private bool GoToLableRequiredOnPeriod
		{
			get;
			set;
		}

		private int GoToLableNumber
		{
			get;
			set;
		}

		public List<string> ReferencedProjects
		{
			get;
			private set;
		}

		public ISourceWriter Writer
		{
			get { return _writer; }
			set { _writer = value; }
		}

		public string DataLayerName
		{
			get { return _csSql.DataLayerName; }
		}

		private UnitTestProject _unitTests;
		public UnitTestProject UnitTests
		{
			get { return _unitTests; }
			set { _unitTests = value; _csSql.UnitTests = value; }
		}

		public CsVerbs(CobolProgram prog, ISourceWriter writer, UnitTestProject utp)
		{
			_prog = prog;
			_writer = writer;
			_csSql = new CsSql(_prog, this, _writer, utp);
			UnitTests = utp;
			ReferencedProjects = new List<string>();
		}

		public void Write(StatementBlock stmts)
		{
			for (int x = 0; x < stmts.Stmts.Count; x++)
			{
				Write(stmts.Stmts[x]);
			}
		}

		public void Write(IVerb v)
		{
			string comment = v.CommentProbe();
			if (!String.IsNullOrEmpty(comment))
			{
				_writer.WriteLine("/*");
				_writer.WriteLine(comment.Replace("/*", "").Replace("*/", "").Replace("\n", "\n\t\t\t"));
				_writer.WriteLine("*/");
			}
			switch (v.Type)
			{
				case VerbType.AbortTrans:
					if (CsSql.BatchTransactions)
					{
						_writer.WriteLine("Connection.RollbackTrans();");
					}
					else
					{
						_writer.WriteLine("//");
						_writer.WriteLine("///TODO: ABORT-TRANSACTION");
						_writer.WriteLine("//");
					}
					break;
				case VerbType.Accept:
					Write((Accept)v);
					break;
				case VerbType.Add:
					Write((Add)v);
					break;
				case VerbType.BeginTrans:
					_writer.WriteLine();
					if (CsSql.BatchTransactions)
					{
						_writer.WriteLine("/// !!!!!!");
						_writer.WriteLine("///TODO: Transaction converted to batch update; manual corrections to client code");
						_writer.WriteLine("/// are probably needed. Also consider removing this.  In this environment, a ");
						_writer.WriteLine("/// transaction is not necessary if your program is doing a single insert or update,");
						_writer.WriteLine("/// or if a rollback isn't needed to ensure consistancy.");
						_writer.WriteLine("/// !!!!!!");
						_writer.WriteLine("Connection.BeginTrans();");
					}
					else
					{
						_writer.WriteLine("//");
						_writer.WriteLine("///TODO: BEGIN-TRANSACTION");
						_writer.WriteLine("//");
					}
					break;
				case VerbType.Call:
					Write((Call)v);
					break;
				case VerbType.Clear:
					throw new NotImplementedException("CLEAR");
				case VerbType.Close:
					Write((Close)v);
					break;
				case VerbType.CompilerDirective:
					break;
				case VerbType.CompilerWarn:
					Console.WriteLine("WARNING: " + ((CompilerWarning)v).Text);
					break;
				case VerbType.Compute:
					Write((Compute)v);
					break;
				case VerbType.Continue:
					break;
				case VerbType.Copy:
					break;
				case VerbType.Delay:
					_writer.WriteLine("System.Threading.Thread.Sleep(" + ((Delay)v).Length + ");");
					break;
				case VerbType.Delete:
					throw new NotImplementedException("DELETE");
				case VerbType.Display:
					Write((Display)v);
					break;
				case VerbType.Divide:
					Write((Divide)v);
					break;
				case VerbType.EndTrans:
					if (CsSql.BatchTransactions)
					{
						_writer.WriteLine("Connection.CommitTrans();");
					}
					else
					{
						_writer.WriteLine("//");
						_writer.WriteLine("///TODO: COMMIT-TRANSACTION");
						_writer.WriteLine("//");
					}
					break;
				case VerbType.Enter:
					Write((Enter)v);
					break;
				case VerbType.Eval:
					Write((Evaluate)v);
					break;
				case VerbType.ExecSql:
					Write((ExecSql)v);
					break;
				case VerbType.Exit:
					_writer.WriteLine("///TODO: Remove or factor this out.");
					_writer.WriteLine("throw new AbortRunException(\"Normal termaination\");");
					break;
				case VerbType.GoTo:
					Write((GoTo)v);
					break;
				case VerbType.If:
					Write((If)v);
					break;
				case VerbType.Init:
					Write((Initialize)v);
					break;
				case VerbType.Inspect:
					Write((Inspect)v);
					break;
				case VerbType.Move:
					Write((Move)v);
					break;
				case VerbType.Mult:
					Write((Multiply)v);
					break;
				case VerbType.Next:
					Write((Next)v);
					break;
				case VerbType.Open:
					Write((Open)v);
					break;
				case VerbType.PerformCall:
					Write((PerformCall)((Perform)v).PerformInner);
					break;
				case VerbType.PerformStmts:
					throw new NotImplementedException("PERMFORM (INLINE)");
				case VerbType.PerformOneOf:
					Write((PerformOneOf)((Perform)v).PerformInner);
					break;
				case VerbType.Print:
					Console.WriteLine("PRINT not supported.");
					_writer.WriteLine("///TODO: PRINT not supported");
					break;
				case VerbType.Period:
					if (GoToLableRequiredOnPeriod)
					{
						GoToLableRequiredOnPeriod = false;
						_writer.WriteLine("Label" + GoToLableNumber + ":");
						_writer.WriteLine(";");
						GoToLableNumber++;
					}
					break;
				case VerbType.Read:
					Write((Read)v);
					break;
				case VerbType.Reset:
					Write((Reset)v);
					break;
				case VerbType.Rewrite:
					throw new NotImplementedException("REWRITE");
				case VerbType.Search:
					Write((Search)v);
					break;
				case VerbType.Send:
					Write((Send)v);
					break;
				case VerbType.Set:
					Write((Set)v);
					break;
				case VerbType.Start:
					throw new NotImplementedException("START");
				case VerbType.Stop:
					Write((Stop)v);
					break;
				case VerbType.StringVerb:
					Write((StringVerb)v);
					break;
				case VerbType.Sub:
					Write((Subtract)v);
					break;
				case VerbType.Turn:
					Write((Turn)v);
					break;
				case VerbType.Unlock:
					throw new NotImplementedException("UNLOCK");
				case VerbType.Write:
					Write((CobolParser.Verbs.Write)v);
					break;
				case VerbType.Unstring:
					WriteUnString((StringVerb)v);
					break;
				default:
					throw new NotImplementedException("INTERNAL ERROR, UNKNOWN VERB");
			}
		}

		private void Write(Accept v)
		{
			_writer.WriteLine("// ** ACCEPT ** ");
			_writer.WriteLine();
			if (v.Action.Equals("UNTIL", StringComparison.InvariantCultureIgnoreCase))
			{
				// Screen ACCEPT

				_writer.WriteLine("if (LoadComplete != null)");
				_writer.WriteLine("{");
				_writer.WriteLine("\tLoadComplete();");
				_writer.WriteLine("}");
				_writer.WriteLine();
				_writer.WriteLine("string key = KeyQueue.Take();");
				_writer.WriteLine("switch(key)");
				using (_writer.Indent())
				{
					Dictionary<string, string> idx = new Dictionary<string, string>();

					for (int x = 0; x < v.UntilList.Count; x++)
					{
						foreach (var key in v.UntilList[x].Items)
						{
							if (idx.ContainsKey(key))
							{
								continue;
							}
							idx.Add(key, key);

							_writer.WriteLine("case \"" + key + "\":");
						}
						_writer.WriteLine("\tTerminationStatus = " + (x+1) + ";");
						_writer.WriteLine("\tbreak;");
					}
				} 
				return;
			}

			if (v.UntilExpr != null)
			{
				_writer.WriteIndent();
				_writer.Write("while (");
				if (v.Action.Equals("FROM", StringComparison.InvariantCultureIgnoreCase))
				{
					_writer.Write("!");
				}
				else
				{
					throw new NotImplementedException();
				}
				_writer.Write(" (/**TODO: Fixup key input condition */");
				//Write(v.UntilExpr);
				_writer.Write("))");
				_writer.IndentManual();
			}

			string fn = v.FromTarget.ToString();

			foreach (var t in v.Arg1.Items)
			{
				if (fn.Equals("TIME", StringComparison.InvariantCultureIgnoreCase))
				{
					_writer.WriteIndent();
					Write(t);
					if (_prog.IsNative(t))
					{
						_writer.Write(" = Int32.Parse(DateTime.Now.ToString(\"HHmmssff\"));");
					}
					else
					{
						_writer.Write(".Set(Int32.Parse(DateTime.Now.ToString(\"HHmmssff\")));");
					}
					_writer.WriteLine();
				}
				else if (fn.Equals("DATE", StringComparison.InvariantCultureIgnoreCase))
				{
					_writer.WriteIndent();
					Write(t);
					if (_prog.IsNative(t))
					{
						_writer.Write(" = Int32.Parse(DateTime.Now.ToString(\"yyMMdd\"));");
					}
					else
					{
						_writer.Write(".Set(Int32.Parse(DateTime.Now.ToString(\"yyMMdd\")));");
					}
					_writer.WriteLine();
				}
				else
				{
					throw new NotImplementedException("ACCEPT FROM " + fn);
				}
			}

			if (v.UntilExpr != null)
			{
				_writer.Unindent();
			}
		}

		private void Write(Add v)
		{
			Debug.Assert(v.Stmts == null);

			if (v.GivingTo != null)
			{
				_writer.WriteIndent();
				Write(v.GivingTo);
				_writer.Write(" = ");
				Debug.Assert(v.Term1.Items.Count == 1);				
				Write(v.Term1.Items[0]);
				_writer.Write(" + ");
				Debug.Assert(v.AddToThis.Items.Count == 1);
				Write(v.AddToThis.Items[0]);
				_writer.Write(";");
				_writer.WriteLine();
				return;
			}

			for (int x = 0; x < v.AddToThis.Items.Count; x++)
			{
				var t = v.AddToThis.Items[x];

				_writer.WriteIndent();
				Write(t);

				string name = "";
				string parent = "";

				if (t is OffsetReference)
				{
					name = ((OffsetReference)t).OffsetChain[0].Value.Str;
					parent = ((OffsetReference)t).OffsetChain[((OffsetReference)t).OffsetChain.Count - 1].Value.Str;
				}
				else
				{
					name = t.ToString();
				}

				INamedField o = _prog.Data.WorkingStorage.Data.LocateField(name, parent);

				_writer.Write(_prog.IsNative(o) ? " += " : ".Inc(");
				Debug.Assert(v.Term1.Items.Count == 1);
				Write(v.Term1.Items[0]);

				//if (!_prog.IsNative(v.Term1.Items[0]))
				//{
				//    if (!StringHelper.IsNumeric(v.Term1.Items[0].ToString()))
				//    {
				//        if (o.Decimals != 0)
				//        {
				//            _writer.Write(".ToDecimal()");
				//        }
				//        else
				//        {
				//            _writer.Write(".ToInt()");
				//        }
				//    }
				//}

				_writer.Write(_prog.IsNative(o) ? ";" : ");");
				_writer.WriteLine();
			}
		}

		private void Write(Verbs.Close v)
		{
			foreach (var t in v.FileNames.Items)
			{
				_writer.WriteLine(CsFieldNameConverter.Convert(((Id)t).Value.Str) + ".Close();");
			}
		}

		private void Write(Compute v)
		{
			Debug.Assert(v.Stmts == null);

			_writer.WriteIndent();
			Write(v.LValue);
			_writer.Write(_prog.IsNative(v.LValue) ? " = " : ".Set(");
			Write(v.RValue);
			_writer.Write(_prog.IsNative(v.LValue) ? ";" : ");");
			_writer.WriteLine();
		}

		private void Write(Divide v)
		{
			_writer.WriteIndent();
			Write(v.Dest);
			_writer.Write(_prog.IsNative(v.Dest) ? " = " : ".Set(");
			Write(v.Numerator);
			_writer.Write(" / ");
			Write(v.Denominator);
			_writer.Write(_prog.IsNative(v.Dest) ? ";" : (v.IsRounded ? ", true" : "") + ");");
			_writer.WriteLine();

			if (v.DestRemainder != null)
			{
				_writer.WriteIndent();
				Write(v.Dest);
				_writer.Write(_prog.IsNative(v.Dest) ? " = " : ".Set(");
				Write(v.Numerator);
				_writer.Write(" % ");
				Write(v.Denominator);
				_writer.Write(_prog.IsNative(v.Dest) ? ";" : ");");
				_writer.WriteLine();
			}
		}

		private void Write(Display v)
		{
			if (_prog.Data.ScreenRecord != null)
			{
				// display screen

				if (v.Terms.Count == 4)
				{
					_writer.WriteLine("///DISPLAY OVERLAY ");

					Debug.Assert(v.Terms[0].ToString() == "OVERLAY");
					Debug.Assert(v.Terms[2].ToString() == "AT");

					string overlayName = v.Terms[1].ToString();
					ScreenField f;
					if (v.Terms[3] is Id)
					{
						f = _prog.Data.ScreenRecord.FindScreenField(v.Terms[3].ToString());
					}
					else
					{
						f = _prog.Data.ScreenRecord.FindScreenField(((OffsetReference)v.Terms[3]).OffsetChain[0].ToString(), ((OffsetReference)v.Terms[3]).OffsetChain[1].ToString());
					}
					Debug.Assert(f != null);

					// move the content presenter
					_writer.WriteLine(CsFieldNameConverter.Convert(overlayName) + "Col = " + f.X + ";");
					_writer.WriteLine(CsFieldNameConverter.Convert(overlayName) + "Row = " + f.Y + ";");
					_writer.WriteLine(CsFieldNameConverter.Convert(overlayName) + "Visibility = Visibility.Visible;");
				}
				else
				{
					Debug.Assert(v.Terms.Count == 1);

					// close popups
					bool wasOverlays = false;
					foreach (var s in _prog.Data.ScreenRecord.Screens)
					{
						if (s.IsOverlay)
						{
							_writer.WriteLine(CsFieldNameConverter.Convert(s.Name) + "Visibility =  Visibility.Hidden;");
							wasOverlays = true;
						}
					}
					if (wasOverlays)
					{
						_writer.WriteLine();
					}

					_writer.WriteLine("HideScreens();");

					_writer.WriteIndent();
					Debug.Assert(v.Terms.Count == 1);
					Write(v.Terms[0]);
					_writer.Write("Visibility = Visibility.Visible;");
				}
				_writer.WriteLine();
				return;
			}

			// write to console

			_writer.WriteIndent();
			_writer.Write("Console.WriteLine(\"");

			for (int x = 0; x < v.Terms.Count; x++)
			{
				_writer.Write("{");
				_writer.Write(x.ToString());
				_writer.Write("}");
			}
			_writer.Write("\", ");

			for (int x = 0; x < v.Terms.Count; x++)
			{
				if (x > 0)
				{
					_writer.Write(", ");
				}
				Write(v.Terms[x]);

				if (! (v.Terms[x] is StringLit))
				{
					_writer.Write(".ToString().Trim()");
				}
			}

			_writer.Write(");");
			_writer.WriteLine();
		}

		private void Write(Enter v)
		{
			
			string functionName = StringHelper.StripQuotes(v.FunctionName);

			_writer.WriteIndent();

			if (v.Returns != null)
			{
				Write(v.Returns);
				_writer.Write(_prog.IsNative(v.Returns) ? " = " : ".Set(");
			}

			if 
			(
				functionName == "SQLCA_TOBUFFER2_" || 
				functionName == "SQLCADISPLAY" ||
				functionName == "GETSTARTUPTEXT" ||
				functionName == "FNAMEEXPAND" ||
				functionName == "OPEN" ||
				functionName == "FILEINFO" ||
				functionName == "GET^JULIANTIMESTAMP" ||
				functionName == "SQLCA_DISPLAY2_" ||
				functionName == "GETPARAMTEXT" ||
				functionName == "SERVERCLASS_SEND_" ||
				functionName == "SERVERCLASS_SEND_INFO_" ||
				functionName == "COMPUTEJULIANDAYNO" ||
				functionName == "INTERPRETJULIANDAYNO" ||
				functionName == "ABORTTRANSACTION"
			)
			{
				_writer.Write(functionName.Replace("^", "_"));
				_writer.Write("(");

				if (v.Arguements != null)
				{
					for (int x = 0; x < v.Arguements.Items.Count; x++)
					{
						if (x > 0)
						{
							_writer.Write(", ");
						}
						if (v.Arguements.Items[x].ToString() == "OMITTED")
						{
							_writer.Write("\"OMITTED\"");
						}
						else
						{
							Write(v.Arguements.Items[x]);
						}
					}
				}

				_writer.Write(")");

				if (functionName == "SERVERCLASS_SEND_")
				{
					_writer.WriteLine();
					_writer.WriteLine("/* TODO: Convert SERVERCLASS_SEND_ to method call */");
				}

			}
			else if (functionName == "TIME")
			{
				_writer.Write("TIME (");
				Write(v.Arguements.Items[0]);
				_writer.Write(".Buffer, ");
				Write(v.Arguements.Items[0]);
				_writer.Write(".Start)");
			}
			else if (functionName == "PROGRAMFILENAME")
			{
				Write(v.Arguements.Items[0]);
				_writer.Write(".Set(\"" + _prog.ProgramId + "\")");
			}
			else if (functionName == "ABEND")
			{
				_writer.Write("throw new AbendException()");
			}
			else
			{
				throw new NotFiniteNumberException("TAL function " + functionName);
			}

			if (v.Returns != null && !_prog.IsNative(v.Returns))
			{
				_writer.Write(")");
			}

			_writer.Write(";");
			_writer.WriteLine();
		}

		private void Write(ExecSql v)
		{
			if (v.Sql is CobolParser.SQL.Statements.BeginDeclareSection)
			{
				return;
			}
			if (v.Sql is CobolParser.SQL.Statements.EndDeclareSection)
			{
				return;
			}
			if (v.Sql is CobolParser.SQL.Statements.Invoke)
			{
				return;
			}
			if (v.Sql is CobolParser.SQL.Statements.Control)
			{
				return;
			}
			if (v.Sql is CobolParser.SQL.Statements.Select)
			{
				_csSql.SelectInto((CobolParser.SQL.Statements.Select)v.Sql, false);
				_csSql.AppendSqlListFile(v.SqlText);
				_csSql.AppendSqlListFile(";\n\n");
				return;
			}
			if (v.Sql is CobolParser.SQL.Statements.OpenSql)
			{
				List<ExecSql> lst = _prog.ListVerbs<ExecSql>();
				lst = lst.Where(e => e.Sql is CobolParser.SQL.Statements.DeclareCursor).ToList();
				List<CobolParser.SQL.Statements.DeclareCursor> dclst = 
					lst.Select(e => (CobolParser.SQL.Statements.DeclareCursor)e.Sql).ToList();

				CobolParser.SQL.Statements.OpenSql osql = (CobolParser.SQL.Statements.OpenSql)v.Sql;
				dclst = dclst.Where(e => (e.CursorName.ToString() == osql.CursorName)).ToList();

				_csSql.OpenCursor
				(
					(CobolParser.SQL.Statements.OpenSql)v.Sql,
					dclst.First()
				);
			}
			if (v.Sql is CobolParser.SQL.Statements.CommitTrans)
			{
				_csSql.Commit((CobolParser.SQL.Statements.CommitTrans)v.Sql);
			}
			if (v.Sql is CobolParser.SQL.Statements.DeclareCursor)
			{
				_csSql.DefCursor((CobolParser.SQL.Statements.DeclareCursor)v.Sql);
				string sql = v.SqlText;
				sql = sql.Substring(sql.IndexOf("CURSOR FOR") + 10);
				while (sql[0] == '\n' || sql[0] == '\r')
				{
					sql = sql.Substring(1);
				}
				_csSql.AppendSqlListFile(sql);
				_csSql.AppendSqlListFile(";\n\n");
			}
			if (v.Sql is CobolParser.SQL.Statements.Delete)
			{
				_csSql.Del((CobolParser.SQL.Statements.Delete)v.Sql);
				_csSql.AppendSqlListFile(v.SqlText);
				_csSql.AppendSqlListFile(";\n\n");
			}
			if (v.Sql is CobolParser.SQL.Statements.Fetch)
			{
				_csSql.ExecCursor((CobolParser.SQL.Statements.Fetch)v.Sql);
			}
			if (v.Sql is CobolParser.SQL.Statements.Insert)
			{
				_csSql.Ins((CobolParser.SQL.Statements.Insert)v.Sql, false);
				_csSql.AppendSqlListFile(v.SqlText);
				_csSql.AppendSqlListFile(";\n\n");
			}
			if (v.Sql is CobolParser.SQL.Statements.RollbackTrans)
			{
				_csSql.Abort((CobolParser.SQL.Statements.RollbackTrans)v.Sql);
			}
			if (v.Sql is CobolParser.SQL.Statements.Update)
			{
				_csSql.Set((CobolParser.SQL.Statements.Update)v.Sql, false);
				_csSql.AppendSqlListFile(v.SqlText);
				_csSql.AppendSqlListFile(";\n\n");
			}
			if (v.Sql is CobolParser.SQL.Statements.Close)
			{
				_csSql.CloseCursor((CobolParser.SQL.Statements.Close)v.Sql);
			}
		}

		private void Write(If v)
		{
			_writer.WriteIndent();
			_writer.Write("if (");
			Write(v.Condition);
			_writer.Write(")");
			_writer.WriteLine();

			using (_writer.Indent())
			{
				Write(v.Stmts);
			}

			if (v.ElseStmts != null)
			{
				_writer.WriteLine("else");

				using (_writer.Indent())
				{
					Write(v.ElseStmts);
				}
			}
		}

		private void Write(Initialize v)
		{
			foreach (var t in v.Terms.Items)
			{
				_writer.WriteIndent();
				Write(t);
				_writer.Write(".Initialize();");
				_writer.WriteLine();
			}

			_writer.WriteLine();
		}

		private void Write(Call call)
		{
			string name = StringHelper.StripQuotes(call.SubRoutine);
			string sys = name.Substring(name.IndexOf('-') + 1, 2);
			string num = name.Substring(name.LastIndexOf('-') + 1);

			name = CsFieldNameConverter.Convert(name);
			string ns = call.LibName == null ? name : call.LibName;

			_writer.WriteLine("///");
			_writer.WriteLine("///TODO: Refactor sub program arguments");
			_writer.WriteLine("///");

			_writer.WriteLine("var _" + name + " = new " + ns + "." + name + "Program();");

			_writer.WriteLine("_" + name + ".Main");
			_writer.WriteLine("(");

			bool writeComma = false;

			foreach (var arg in call.UsingThis.Items)
			{
				_writer.WriteIndent();
				_writer.Write("\t");

				if (writeComma)
				{
					_writer.Write(", ");
				}
				else
				{
					writeComma = true;
				}
				Write(arg);
				_writer.WriteLine();
			}

			_writer.WriteIndent();
			_writer.Write(");");
			_writer.WriteLine();

			GuardianPath gp = new GuardianPath(_prog.FileName.Drive, _prog.FileName.Volume, num + sys + "1u");
			if (!File.Exists(gp.WindowsFileName()))
			{
				gp = new GuardianPath(_prog.FileName.Drive, _prog.FileName.Volume, num + sys + "1w");
			}

			if (!File.Exists(gp.WindowsFileName()))
			{
				_writer.WriteLine("///TODO: Program not found " + call.SubRoutine);
				return;
			}

			CsFormater conv = new CsFormater(gp, false);

			if ((from p in ReferencedProjects where p == conv.CsProjectName select p).Count() == 0)
			{
				ReferencedProjects.Add(conv.CsProjectName);
			}
		}

		private void Write(Evaluate v)
		{
			EvalWhen def = null;
			string ifpart = "if (";
			bool dropThrough = false;

			foreach (EvalWhen w in v.Whens)
			{
				if 
				(
					w.IsDefault || 
					(
						w.AlsoTerms.Count == 1 && 
						w.AlsoTerms[0] is Expr &&
						((Expr)w.AlsoTerms[0]).Terms.Count == 1 &&
						((Expr)w.AlsoTerms[0]).Terms[0] is Id &&
						((Id)((Expr)w.AlsoTerms[0]).Terms[0]).Value.StrEquals("OTHER")
					)
				)
				{
					def = w;
					continue;
				}

				_writer.WriteIndent();

				if (dropThrough)
				{
					_writer.Write("\t|| ");
				}
				else
				{
					_writer.Write(ifpart);
				}

				dropThrough = w.Stmts.Stmts.Count == 0;

				ifpart = "else if (";

				for (int x = 0; x < w.AlsoTerms.Count; x++)
				{
					if (x > 0)
					{
						_writer.Write(" && ");
					}
					Write(v.Conditions[x]);

					_writer.Write(".Equals(");

					Write(w.AlsoTerms[x]);

					_writer.Write(")");
				}
				if (!dropThrough)
				{
					_writer.Write(")");
				}
				_writer.WriteLine();

				if (w.Stmts.Stmts.Count > 0)
				{
					dropThrough = false;
					using (_writer.Indent())
					{
						Write(w.Stmts);
					}
				}
			}

			if (def != null)
			{
				_writer.WriteLine("else");
				using (_writer.Indent())
				{
					Write(def.Stmts);
				}
			}
		}

		private void Write(Inspect v)
		{
			_writer.WriteIndent();

			Write(v.Target);
			_writer.Write(_prog.IsNative(v.Target) ? " = " : ".Set(");
			Write(v.Target);
			_writer.Write(".ToString()");

			foreach (var op in v.Operations)
			{
				if (op is InspectReplacing)
				{
					foreach (var item in ((InspectReplacing)op).Items)
					{
						Debug.Assert(item.IsAll);
						_writer.Write(".Replace(");
						if (item.FromText is Spaces)
						{
							_writer.Write("\" \"");
						}
						else
						{
							Write(item.FromText);
						}
						_writer.Write(", ");
						Write(item.ToText);
						_writer.Write(")");
					}
				}
				else
				{
					throw new NotImplementedException("INSPECT OP");
				}
			}

			_writer.Write(_prog.IsNative(v.Target) ? ";" : ");");
			_writer.WriteLine();
		}

		private void Write(GoTo v)
		{
			_writer.WriteLine("goto Label" + GoToLableNumber + ";");
			GoToLableRequiredOnPeriod = true;
		}

		private void Write(Multiply v)
		{
			_writer.WriteIndent();

			bool isNative = false;
			if (v.Dest == null)
			{
				Write(v.Factor2);
				isNative = _prog.IsNative(v.Factor2);
			}
			{
				Write(v.Dest);
				isNative = _prog.IsNative(v.Dest);
			}

			_writer.Write(isNative ? " = " : ".Set(");

			Write(v.Factor1);
			_writer.Write(" * ");
			Write(v.Factor2);
			_writer.Write(isNative ? ";" : (v.IsRounded ? ", true" : "") + ");");
			_writer.WriteLine();

			Debug.Assert(! v.IsRounded);
		}

		private void Write(Next v)
		{
			_writer.WriteLine("goto Label" + GoToLableNumber + ";");
			GoToLableRequiredOnPeriod = true;
		}

		private void Write(Open v)
		{
			if (v.InputFiles != null)
			{
				foreach (var f in v.InputFiles.Items)
				{
					string name = ((Id)f).Value.Str;

					_writer.WriteIndent();
					_writer.Write(CsFieldNameConverter.Convert(name));
					_writer.Write(".Open(IoStreamMode.Input);");
					_writer.WriteLine();

					Select s = _prog.Environment.InputOutputSection.FileControlSection.Find(name);
					if (s != null)
					{
						if (s.FileName.Equals("$RECEIVE", StringComparison.InvariantCultureIgnoreCase))
						{
							_writer.WriteLine("_receiveIn = " + CsFieldNameConverter.Convert(name) + ";");
							_writer.WriteLine("_receiveIn.Add(InputRecord);");
							_writer.WriteLine();
						}
					}
				}
			}
			if (v.OutputFiles != null)
			{
				foreach (var f in v.OutputFiles.Items)
				{
					_writer.WriteIndent();
					_writer.Write(CsFieldNameConverter.Convert(((Id)f).Value.Str));
					_writer.Write(".Open(IoStreamMode.Output);");
					_writer.WriteLine();
				}
			}

			_writer.WriteLine();
		}

		private void Write(PerformCall v)
		{
			if (v.Iteration != null)
			{
				string start = v.Iteration.Start.ToString();
				_writer.WriteIndent();
				_writer.Write("for");
				_writer.WriteLine();
				_writer.WriteIndent();
				_writer.Write("(");
				_writer.WriteLine();
				_writer.WriteIndent();
				_writer.Write("\t");
				if (v.Iteration.IterVar == null)
				{
					_writer.Write("int x = ");
					Write(v.Iteration.Start);
					_writer.Write(";");
				}
				else
				{
					Write(v.Iteration.IterVar);
					_writer.Write(_prog.IsNative(v.Iteration.IterVar) ? " = " : ".Set(");
					Write(v.Iteration.Start);
					_writer.Write(_prog.IsNative(v.Iteration.IterVar) ? ";" : ");");
				}
				_writer.WriteLine();
				_writer.WriteIndent();
				_writer.Write("\t");
				if (v.UntilExpr == null)
				{
					if (v.Iteration.IterVar == null)
					{
						_writer.Write("x");
					}
					else
					{
						Write(v.Iteration.IterVar);
					}
					_writer.Write(" <= ");

					Write(v.Iteration.Stop);

					//if (! (v.Iteration.Stop is Number))
					//{
					//	_writer.Write(".ToInt()");
					//}
				}
				else
				{
					Debug.Assert(v.Iteration.Stop == null || v.Iteration.Stop.ToString() == "1");
					_writer.Write("! (");
					Write(v.UntilExpr);
					_writer.Write(")");
				}

				_writer.Write(";");
				_writer.WriteLine();
				_writer.WriteIndent();
				_writer.Write("\t");

				if (v.Iteration.IterVar == null)
				{
					_writer.Write("x += ");
				}
				else
				{
					Write(v.Iteration.IterVar);
					if (_prog.IsNative(v.Iteration.IterVar))
					{
						_writer.Write(" += ");
					}
					else
					{
						_writer.Write(".Inc(");
					}
				}
				if (v.Iteration.Step == null)
				{
					_writer.Write("1");
				}
				else
				{
					Write(v.Iteration.Step);
				}

				if (v.Iteration.IterVar != null && !_prog.IsNative(v.Iteration.IterVar))
				{
					_writer.Write(")");
				}
				_writer.WriteLine();
				_writer.WriteLine(")");
			}
			else if (v.UntilExpr != null)
			{
				_writer.WriteIndent();

				if (v.WithTestAfter)
				{
					_writer.WriteLine("do");
				}
				else
				{
					_writer.Write("while (! (");
					Write(v.UntilExpr);
					_writer.Write("))");
				}
				_writer.WriteLine();
			}

			if (v.Iteration != null || v.UntilExpr != null)
			{
				_writer.IndentManual();
			}

			if (v.SubRoutine != null)
			{
				_writer.WriteLine(CsFieldNameConverter.Convert(v.SubRoutine.Value.Str) + "();");
			}

			Debug.Assert(v.ThroughSubRoutine == null);

			if (v.Stmts != null)
			{
			    foreach (var stmt in v.Stmts.Stmts)
			    {
			        Write(stmt);
			    }
			}

			if (v.Iteration != null || v.UntilExpr != null)
			{
				_writer.Unindent();
			}

			if (v.WithTestAfter && v.Iteration == null)
			{
				_writer.Write("while (!(");
			    Write(v.UntilExpr);
				_writer.Write("));");
			}
		}

		private void Write(PerformOneOf v)
		{
			_writer.WriteLine("switch(" + CsFieldNameConverter.Convert(v.DependingOn.ToString()) + ")");
			using (_writer.Indent())
			{
				for (int x = 0; x < v.Options.Items.Count; x++)
				{
					_writer.WriteLine("case " + (x + 1) + ":");
					_writer.WriteLine("\t" + CsFieldNameConverter.Convert(v.Options.Items[x].ToString()) + "();");
					_writer.WriteLine("\tbreak;");
				}
			}
		}

		private void Write(Read v)
		{
			string name = CsFieldNameConverter.Convert(v.FileName);

			if (v.ReadInto == null)
			{
				_writer.WriteLine(name + ".Read();");
			}
			else
			{
				string fn = CsFieldNameConverter.Convert(v.ReadInto.ToString());
				_writer.WriteLine(name + ".Read(" + fn + ");");
			}

			if (v.AtEndStmts != null)
			{
				_writer.WriteLine("if (" + name + ".IsEof)");
				using (_writer.Indent())
				{
					Write(v.AtEndStmts);
				}
			}

			if (v.NotAtEndStmts != null)
			{
				_writer.WriteLine("if (! " + name + ".IsEof)");
				using (_writer.Indent())
				{
					Write(v.NotAtEndStmts);
				}
			}

			if (v.InvalidKeyStmts != null)
			{
				_writer.WriteLine("if (" + name + ".IsInvalidKey)");
				using (_writer.Indent())
				{
					Write(v.InvalidKeyStmts);
				}
			}

			if (v.NotInvalidKeyStmts != null)
			{
				_writer.WriteLine("if (! " + name + ".IsInvalidKey)");
				using (_writer.Indent())
				{
					Write(v.NotInvalidKeyStmts);
				}
			}
		}

		public void Write(IExpr expr)
		{
			if (expr is ValueList)
			{
				int count = 0;
				foreach (ITerm t in ((ValueList)expr).Items)
				{
					if (count++ > 0)
					{
						_writer.Write(",");
					}
					Write(t);
				}
			}
			else
			{
				int count = 0;
				Expr e = (Expr)expr;

				if (e.Terms.Count == 3 && e.Terms[2] is Numeric)
				{
					Write(e.Terms[0]);

					Debug.Assert(e.Terms[1] is Operator);

					if (e.Terms[1].ToString() == "!")
					{
						_writer.Write(" != ");
					}
					else
					{
						Write(e.Terms[1]);
					}
					Write(e.Terms[2]);
					return;
				}

				if (e.Terms.Count == 2 && e.Terms[1] is Numeric)
				{
					Write(e.Terms[0]);
					_writer.Write(" == ");
					Write(e.Terms[1]);
					return;
				}

				for (int x = 0; x < e.Terms.Count; x++ )
				{
					ITerm t = e.Terms[x];
					if
					(
						(t is Id || t is OffsetReference) &&
						x < e.Terms.Count - 4 &&
						e.Terms[x + 1].ToString() == "==" &&
						(e.Terms[x + 2] is StringLit || e.Terms[x + 2] is Number) &&
						e.Terms[x + 3].ToString() == "||" &&
						(e.Terms[x + 4] is StringLit || e.Terms[x + 4] is Number)
					)
					{
						_writer.WriteLine();
						_writer.WriteIndent();
						_writer.Write("\t");

						ITerm op = e.Terms[x + 1];
						Write(t);
						_writer.Write(" ");
						Write(op);
						x += 2;
						_writer.Write(" ");
						Debug.Assert(e.Terms[x] is StringLit || e.Terms[x] is Number);
						Write(e.Terms[x++]);

						_writer.WriteLine();

						while
						(
							x < e.Terms.Count - 1 &&
							e.Terms[x].ToString() == "||" &&
							(e.Terms[x + 1] is StringLit || e.Terms[x + 1] is Number)
						)
						{
							_writer.WriteIndent();
							_writer.Write("\t");

							_writer.Write(" || ");
							Write(t);
							_writer.Write(" ");
							Write(op);
							_writer.Write(" ");
							Write(e.Terms[x + 1]);
							x += 2;

							_writer.WriteLine();
						}
						_writer.WriteIndent();
					}
					else
					{
						if (count++ > 0)
						{
							_writer.Write(" ");
						}

						if (! (t is Operator)  && x < e.Terms.Count - 1 && e.Terms[x + 1] is Numeric)
						{
							Write(t);
							_writer.Write(" == ");
							Write(e.Terms[x + 1]);
							x++;
						}
						else
						{
							Write(t);

							// seems to cause more harm that good
							//if (!_prog.IsNative(t))
							//{
							//INamedField f = _prog.FindFieldForTerm(t);
							//if (f != null)
							//{
							//_writer.Write(f.ConversionMethod());
							//}
							//}
						}
					}
				}
			}
		}

		private void Write(Stop v)
		{
			string replyName = _prog.Environment.InputOutputSection.FileControlSection.ReplyMessageName();

			_writer.WriteLine("///TODO: Remove or factor this out.");
			if (replyName == null || replyName == "void")
			{
				_writer.WriteLine("throw new AbortRunException();");
			}
			else
			{
				_writer.WriteLine("throw new AbortRunException(" + CsFieldNameConverter.Convert(replyName) + ".Outputs[0]);");
			}
		}

		private void Write(Move v)
		{
			foreach (ITerm t in v.Dest.Items)
			{
				_writer.WriteIndent();
				Write(t);
				_writer.Write(_prog.IsNative(t) ? " = " : ".Set(");
				Write(v.Source);
				_writer.Write(_prog.IsNative(t) ? ";" : ");");
				_writer.WriteLine();
			}
		}

		private void Write(Reset v)
		{
			foreach (var t in v.Field.Items)
			{
				_writer.WriteIndent();

				INamedField f = _prog.FindFieldForTerm(t);
				if (f == null)
				{
					ScreenField sf;
					if (t is OffsetReference)
					{
						OffsetReference offref = (OffsetReference)t;
						sf = _prog.Data.ScreenRecord.FindScreenField
						(
							offref.OffsetChain[0].Value.Str,
							offref.OffsetChain[offref.OffsetChain.Count - 1].Value.Str
						);
					}
					else
					{
						sf = _prog.Data.ScreenRecord.FindScreenField(t.ToString());
					}
					_writer.Write(CsFieldNameConverter.Convert(sf.Name));
				}
				else
				{
					_writer.Write(f.FullyQualifiedName);
				}
				_writer.Write(".Reset();");
				_writer.WriteLine();
			}
		}

		private void Write(Search v)
		{
			// Loop through the indicated occures using index by (no all) or 
			// key is (search all). Exectue statements when condition matches
			// current record.

			Debug.Assert(v.IntoDest == null);

			if (v.IntoDest != null)
			{
				throw new NotImplementedException("SEARCH ... INTO not implemeted");
			}
			if (v.InvalidKeyStmts != null)
			{
				throw new NotImplementedException("SEARCH ... INVALID KEy not implemeted");
			}
			if (v.NotInvalidKeyStmts != null)
			{
				throw new NotImplementedException("SEARCH ... NOT INVALID KEy not implemeted");
			}

			INamedField f = _prog.Data.LocateField(v.IndexName, null);

			_writer.WriteIndent();
			_writer.Write("for(; ");

			if (v.VaryingValue != null)
			{
				Write(v.VaryingValue);
			}
			else
			{
				_writer.Write(CsFieldNameConverter.Convert(f.Attributes.IndexedBy));
			}

			_writer.Write(" < ");
			GetFieldReference(v.IndexName);
			_writer.Write(".Length; ");

			if (v.VaryingValue != null)
			{
				Write(v.VaryingValue);
				if (_prog.IsNative(v.VaryingValue))
				{
					_writer.Write("++)");
				}
				else
				{
					_writer.Write(".Inc(1))");
				}
			}
			else
			{
				_writer.Write(CsFieldNameConverter.Convert(f.Attributes.IndexedBy));
				_writer.Write("++)");
			}

			_writer.WriteLine();

			using (_writer.Indent())
			{
				if (v.NotAtEndStmts != null)
				{
					Write(v.NotAtEndStmts);
					_writer.WriteLine();
				}
				foreach (var w in v.Whens)
				{
					_writer.WriteIndent();
					_writer.Write("if (");
					Write(w.Condition);
					_writer.Write(")");
					using (_writer.Indent())
					{
						Write(w.Stmts);
					}
				}
			}

			if (v.AtEndStmts != null)
			{
				Write(v.AtEndStmts);
			}
		}

		private void Write(Send v)
		{
			INamedField f = _prog.FindFieldForTerm(v.MessageDataElement);
			string server = CsFieldNameConverter.Convert(StringHelper.StripQuotes(v.ServerClassName));

			if (v.OnError != null)
			{
				_writer.WriteLine("try");
				_writer.IndentManual();
			}

			_writer.WriteLine("var server = new " + server + "." + server + "Program();");
			_writer.WriteIndent();
			_writer.Write("IBufferOffset reply = server.Main(");
			Write(v.MessageDataElement);
			_writer.Write(", Logger, Connection);");
			_writer.WriteLine();
			_writer.WriteLine("reply.Buffer.Get(out TerminationStatus, 0, 5, true);");
			_writer.WriteLine();

			string theIf = "if";
			int count = 1;

			foreach (var r in v.OutputCases.Cases)
			{
				_writer.WriteIndent();
				_writer.Write(theIf);
				_writer.Write("(TerminationStatus == ");
				Write(r.Left);
				_writer.Write(")");
				_writer.WriteLine();
				using (_writer.Indent())
				{
					_writer.WriteLine(CsFieldNameConverter.Convert(r.Right) + ".Set(reply);");
					_writer.WriteLine("TerminationStatus = " + count + ";");
					count++;
				}
				theIf = "else if";
			}
			_writer.WriteLine("else");
			using (_writer.Indent())
			{
				_writer.WriteLine("Context.Log.Write(Facility.Local7, Severity.Error, SystemPID." + _prog.Identification.System + ", \"Unhandled server reply code of \" + TerminationStatus);");
				_writer.WriteLine("ClControlParm.ClInformation.ClInformationGrp.ClAdvisoryGrp.ClAdvisoryMessage.Set(\"Internal error\");");
			}

			if (v.OnError != null)
			{
				_writer.Unindent();
				_writer.WriteLine("catch (Exception ex)");
				using (_writer.Indent())
				{
					_writer.WriteLine("Context.Log.Write(SystemPID." + _prog.Identification.System + ", ex);");
					//_writer.WriteLine("ClControlParm.ClInformation.ClInformationGrp.ClAdvisoryGrp.ClAdvisoryMessage.Set(ex.Message);");
					Write(v.OnError);
				}
			}
		}

		private void Write(Set v)
		{
			foreach (var t in v.LValue.Items)
			{
				_writer.WriteIndent();
				Write(t);
				if (String.IsNullOrEmpty(v.UpOrDown))
				{
					_writer.Write(_prog.IsNative(t) ? " = " : ".Set(");					
					Write(v.RValue);
				}
				else
				{
					Debug.Assert(v.RValue == null);

					if (_prog.IsNative(t))
					{
						_writer.Write(" += ");
					}
					else
					{
						_writer.Write(".Inc(");
					}

					if (v.UpOrDown.Equals("UP", StringComparison.InvariantCultureIgnoreCase))
					{
						Write(v.IncrementValue);
					}
					else
					{
						_writer.Write("-");
						Write(v.IncrementValue);
					}
				}
				if (_prog.IsNative(t))
				{
					_writer.Write(";");
				}
				else
				{
					_writer.Write(");");
				}
				_writer.WriteLine();
			}
		}

		private void Write(StringVerb v)
		{
			Debug.Assert(v.OnOverflowStmts == null);
			Debug.Assert(v.WithPointerIs == null);

			if (v.IsUnString)
			{
				// split string by delimiter
				throw new NotImplementedException("UNSTRING");
			}
			else
			{
				foreach (var t in v.Dest.Items)
				{
					_writer.WriteIndent();
					Write(t);

					_writer.Write(_prog.IsNative(t) ? " = " : ".Set(");					

					bool writePlus = false;

					foreach (var si in v.Items)
					{
						if (writePlus)
						{
							_writer.Write(" + ");
						}
						else
						{
							writePlus = true;
						}

						Write(si.Text);
						if (! (si.Text is StringLit))
						{
							_writer.Write(".ToString().Trim()");
						}
					}

					if (_prog.IsNative(t))
					{
						_writer.Write(";");
					}
					else
					{
						_writer.Write(");");
					}
					_writer.WriteLine();
				}
			}
		}

		private void Write(Subtract v)
		{
			// on size error
			Debug.Assert(v.Stmts == null);

			if (v.GivingTo == null)
			{
				for (int x = 0; x < v.FromThis.Items.Count; x++)
				{
					_writer.WriteIndent();
					Write(v.FromThis.Items[x]);
					if (_prog.IsNative(v.FromThis.Items[x]))
					{
						_writer.Write(" -= ");
					}
					else
					{
						_writer.Write(".Inc(-");
					}
					Write(v.SubtractThis.Items[x]);
					if (_prog.IsNative(v.FromThis.Items[x]))
					{
						_writer.Write(";");
					}
					else
					{
						if (!StringHelper.IsNumeric(v.SubtractThis.Items[x].ToString()))
						{
							_writer.Write(".ToDecimal()");
						}
						_writer.Write(");");
					}
					_writer.WriteLine();
				}
			}
			else
			{
				for (int x = 0; x < v.GivingTo.Items.Count; x++)
				{
					_writer.WriteIndent();
					Write(v.GivingTo.Items[x]);
					_writer.Write(".Set(");
					Write(v.FromThis.Items[x]);
					_writer.Write(" - ");
					Write(v.SubtractThis.Items[x]);
					_writer.Write(");");
					_writer.WriteLine();
				}
			}
		}

		private string ToCsName(INamedField o)
		{
			StringBuilder buf = new StringBuilder();

			while (o != null)
			{
				if (buf.Length > 0)
				{
					buf.Append('.');
				}
				buf.Append(StringHelper.Reverse(CsFieldNameConverter.Convert(o.Name)));
				o = o.Parent;
			}

			return StringHelper.Reverse(buf.ToString());
		}

		private void Write(Turn v)
		{
			foreach (var f in v.Fields.Items)
			{
				if (f is Id && ((Id)f).Offsets.Count != 0)
				{
					_writer.WriteLine();
					_writer.WriteLine("///TODO: array offset not supported for TURN");
					_writer.WriteLine("/// " + f.ToString());
					_writer.WriteLine();
				}
				foreach (var a in v.Attributes)
				{
					if 
					(
						a.Equals("DIM", StringComparison.InvariantCultureIgnoreCase) ||
						a.Equals("BLINK", StringComparison.InvariantCultureIgnoreCase)
					)
					{
						continue;
					}

					if (a.Equals("PROTECTED", StringComparison.InvariantCultureIgnoreCase))
					{
						ScreenField sfield = _prog.Data.ScreenRecord.FindScreenField(f);
						if (sfield == null)
						{
							// missing field
							// TURN TEMP PROTECTED   IN                   SC-BR-R700-MAIN-1.
							continue;
						}
						
						INamedField o = _prog.FindFieldForTerm(sfield);

						int oOccuraces = 1;

						if (o == null)
						{
							continue;
						}

						oOccuraces = o.Occurances;
						if (oOccuraces < 2 && o.Parent != null)
						{
							oOccuraces = o.Parent.Occurances;
						}

						for (int x = 0; x < oOccuraces; x++)
						{
							string extraName = oOccuraces > 1 ? x.ToString() : "";

							_writer.WriteIndent();
							_writer.Write(CsFieldNameConverter.Convert(o.Name));
							_writer.Write(extraName);
							_writer.Write("IsEnabledBinding = false;");
							_writer.WriteLine();

							if (v.IsTemp)
							{
								_writer.WriteIndent();
								_writer.Write("_turnTemps.Add(new DelegateNoArgs(() => { ");
								_writer.Write(CsFieldNameConverter.Convert(o.Name));
								_writer.Write(extraName);
								_writer.Write("IsEnabledBinding = true; }));");
								_writer.WriteLine();
							}
						}
					}
					else if (a.Equals("UNPROTECTED", StringComparison.InvariantCultureIgnoreCase))
					{
						ScreenField sfield = _prog.Data.ScreenRecord.FindScreenField(f);
						if (sfield == null)
						{
							// missing field
							// TURN TEMP PROTECTED   IN                   SC-BR-R700-MAIN-1.
							continue;
						}
						INamedField o = _prog.FindFieldForTerm(sfield);

						for (int x = 0; x < o.Occurances; x++)
						{
							string extraName = o.Occurances > 1 ? x.ToString() : "";

							_writer.WriteIndent();
							_writer.Write(CsFieldNameConverter.Convert(o.Name));
							_writer.Write(extraName);
							_writer.Write("IsEnabledBinding = true;");
							_writer.WriteLine();

							if (v.IsTemp)
							{
								_writer.WriteIndent();
								_writer.Write("_turnTemps.Add(new DelegateNoArgs(() => { ");
								_writer.Write(CsFieldNameConverter.Convert(o.Name));
								_writer.Write(extraName);
								_writer.Write("IsEnabledBinding = false; }));");
								_writer.WriteLine();
							}
						}
					}
					else if (a.Equals("REVERSE", StringComparison.InvariantCultureIgnoreCase))
					{
						_writer.WriteLine("/// TODO: ");
						_writer.WriteIndent();
						_writer.Write("//");

						StringNode lex = v.VerbLexum;
						while (lex.Type != StringNodeType.Period)
						{
							_writer.Write(" " + lex.Str);
							lex = lex.Next;
						}
						_writer.WriteLine();
					}
					else if (a.Equals("HIDDEN", StringComparison.InvariantCultureIgnoreCase))
					{
						_writer.WriteIndent();
						ScreenField sfield = _prog.Data.ScreenRecord.FindScreenField(f);
						_writer.Write(CsFieldNameConverter.Convert(sfield.Name));
						_writer.Write("VisibilityBinding = Visibility.Hidden;");
						_writer.WriteLine();

						if (v.IsTemp)
						{
							_writer.WriteIndent();
							_writer.Write("_turnTemps.Add(new DelegateNoArgs(() => { ");
							_writer.Write(CsFieldNameConverter.Convert(sfield.Name));
							_writer.Write("VisibilityBinding = Visibility.Visible; }));");
							_writer.WriteLine();
						}
					}
					else if 
					(
						a.Equals("NOUNDERLINE", StringComparison.InvariantCultureIgnoreCase) ||
						a.Equals("UNDERLINE", StringComparison.InvariantCultureIgnoreCase) ||
						a.Equals("NORMAL", StringComparison.InvariantCultureIgnoreCase)
					)
					{
						_writer.WriteLine("/// TODO: ");
						_writer.WriteIndent();
						_writer.Write("//");

						StringNode lex = v.VerbLexum;
						while (lex.Type != StringNodeType.Period)
						{
							_writer.Write(" " + lex.Str);
							lex = lex.Next;
						}
						_writer.WriteLine();
					}
					else
					{
						throw new NotImplementedException();
					}
				}
			}

			_writer.WriteLine();
		}

		private void WriteUnString(StringVerb v)
		{
			Debug.Assert(v.IsUnString);

			_writer.WriteLine("int pos = 0;");

			Debug.Assert(v.Items.Count == 1);

			for (int x = 0; x < v.UnstringItems.Count; x++)
			{
				_writer.WriteIndent();
				_writer.Write("Unstring(");
				Write(v.Items[0].Text);
				_writer.Write(", ");
				if (!String.IsNullOrWhiteSpace(v.Items[0].IsDelimitedBy))
				{
					_writer.Write(v.Items[0].IsDelimitedBy);
					_writer.Write(", ");
				}
				Write(v.UnstringItems[x].Dest);

				if (!String.IsNullOrWhiteSpace(v.Items[0].IsDelimitedBy))
				{
					_writer.Write(", ");

					if (v.UnstringItems[x].DelimiterDest == null)
					{
						_writer.Write("null");
					}
					else
					{
						Write(v.UnstringItems[x].DelimiterDest);
					}
				}

				_writer.Write(", ref pos);");
				_writer.WriteLine();
			}

			if (v.WithPointerIs != null)
			{
				Write(v.WithPointerIs);
				_writer.Write(".Set(pos + 1);");
			}

			if (v.OnOverflowStmts != null)
			{
				_writer.WriteIndent();
				_writer.Write("if (");
				if (v.WithPointerIs == null)
				{
					_writer.Write("pos ");
				}
				else
				{
					Write(v.WithPointerIs);
					_writer.Write(" ");
				}
				_writer.Write("> -1)");
				_writer.WriteLine();

				using (_writer.Indent())
				{
					Write(v.OnOverflowStmts);
				}
			}

			_writer.WriteLine();
		}

		private void Write(CobolParser.Verbs.Write v)
		{
			string name;

			if (v.FileName is OffsetReference)
			{
				OffsetReference oref = (OffsetReference)v.FileName;

				name = CsFieldNameConverter.Convert(oref.OffsetChain[oref.OffsetChain.Count - 1].Value.Str);
				_writer.WriteIndent();
				_writer.Write(name);
				_writer.Write(".Write(");
				Write(oref);
				_writer.Write(");");
				_writer.WriteLine();
			}
			else
			{
				string varName = "";

				if (v.FileName != null)
				{
					varName = v.FileName.ToString();
				}
				else if (v.FromVar != null)
				{
					varName = v.FromVar.ToString();
				}

				Fd fd = _prog.Data.Files.FindFdForRecord(varName);
				name = CsFieldNameConverter.Convert(fd == null ? varName : fd.MessageName);

				_writer.WriteIndent();
				_writer.Write(name);
				_writer.Write(".Write(");

				if (v.FileName != null)
				{
					Write(v.FileName);
				}
				else if (v.FromVar != null)
				{
					Write(v.FromVar);
				}
	
				_writer.Write(");");
				_writer.WriteLine();
			}

			if (v.AtEndStmts != null)
			{
				_writer.WriteLine("if (" + name + ".IsEof)");
				using (_writer.Indent())
				{
					Write(v.AtEndStmts);
				}
			}

			if (v.NotAtEndStmts != null)
			{
				_writer.WriteLine("if (! " + name + ".IsEof)");
				using (_writer.Indent())
				{
					Write(v.NotAtEndStmts);
				}
			}

			if (v.InvalidKeyStmts != null)
			{
				_writer.WriteLine("if (" + name + ".IsInvalidKey)");
				using (_writer.Indent())
				{
					Write(v.InvalidKeyStmts);
				}
			}

			if (v.NotInvalidKeyStmts != null)
			{
				_writer.WriteLine("if (! " + name + ".IsInvalidKey)");
				using (_writer.Indent())
				{
					Write(v.NotInvalidKeyStmts);
				}
			}
		}

		private void WriteOffsets(Vector<IExpr> offsets)
		{
			foreach (var off in offsets)
			{
				if (off is Expr)
				{
					Expr e = (Expr)off;
					if (e.IsNumber || StringHelper.IsInt(e.ToString()))
					{
						_writer.Write("[");
						_writer.Write(e.ToString());
						_writer.Write("-1]");
					}
					else
					{
						if (e.Terms.Count > 1)
						{
							throw new Exception("Field offset expr assumption error");
						}
						if (e.Terms[0] is Range)
						{
							Debug.Assert(((Range)e.Terms[0]).Ranges.Count == 1);
							_writer.Write(".CreateSubRange(" + ((Range)e.Terms[0]).Ranges[0].From.ToString() + ", " + ((Range)e.Terms[0]).Ranges[0].To.ToString() + ")");
						}
						else if (e.Terms[0] is OffsetReference || e.Terms[0] is Id)
						{
							// base var is a group with occurances
							_writer.Write("[");
							Write(e.Terms[0]);
							_writer.Write(".ToInt32()-1]");
						}
						else
						{
							throw new Exception("Field offset expr not range assumption error");
						}
					}
				}
				else
				{
					throw new Exception("Field offset assumption error");
				}
			}
		}

		public INamedField GetFieldReference
		(
			string name, 
			string parent = null, 
			Vector<IExpr> offsets = null
		)
		{
			if (parent == "")
			{
				parent = null;
			}

			if (parent != null)
			{
				int dotPos = parent.LastIndexOf('.');
				if (dotPos > -1)
				{
					parent = parent.Substring(dotPos + 1);
				}
			}

			INamedField f = _prog.Data.LocateField(name, parent);
			if (f == null)
			{
				if (name.Equals("HIGH-VALUES", StringComparison.InvariantCultureIgnoreCase))
				{
					_writer.Write("HighValues.Instance()");
				}
				else if (name.Equals("LOW-VALUES", StringComparison.InvariantCultureIgnoreCase))
				{
					_writer.Write("LowValues.Instance()");
				}
				else
				{
					_writer.Write(CsFieldNameConverter.Convert(name));
				}
				return f;
			}
			if (f is Offset && !String.IsNullOrEmpty(((Offset)f).FileSectionMessageName))
			{
				_writer.Write(CsFieldNameConverter.Convert(((Offset)f).FileSectionMessageName));
			}
			if (f.Parent == null)
			{
				// setting the 01 record, pretty common.
				_writer.Write(CsFieldNameConverter.Convert(f.Name, f.SpecialNameHandling));
				return f;
			}

			Vector<INamedField> ov = new Vector<INamedField>();
			INamedField o = f;

			while (o != null)
			{
				ov.Add(o);
				o = o.Parent;
			}

			for (int x = ov.Count - 1; x >= 0; x--)
			{
				if (ov[x] is Offset && !String.IsNullOrEmpty(((Offset)ov[x]).FileSectionMessageName))
				{
					_writer.Write(CsFieldNameConverter.Convert(((Offset)ov[x]).FileSectionMessageName));
				}

				_writer.Write(CsFieldNameConverter.Convert(ov[x].Name, ov[x].SpecialNameHandling));

				if (ov[x].Occurances > 1 && offsets != null)
				{
					WriteOffsets(offsets);
					offsets = null;
				}

				if (x > 0)
				{
					_writer.Write(".");
				}
			}

			if (offsets != null && offsets.Count > 0)
			{
				WriteOffsets(offsets);
			}

			return f;
		}

		public void Write(ITerm t)
		{
			if (t is Id)
			{
				Id id = (Id)t;
				GetFieldReference(id.Value.Str, null, id.Offsets);
			}
			else if (t is OffsetReference)
			{
				OffsetReference offref = (OffsetReference)t;
				INamedField f = _prog.Data.LocateField
				(
					offref.OffsetChain[0].Value.Str,
					offref.OffsetChain[offref.OffsetChain.Count - 1].Value.Str
				);

				if 
				(
					f == null && 
					_prog.Data.ScreenRecord != null && 
					_prog.Data.ScreenRecord.IsScreenField(offref.OffsetChain[0].Value.Str, offref.OffsetChain[offref.OffsetChain.Count - 1].Value.Str)
				)
				{
					_writer.Write(CsFieldNameConverter.Convert(offref.OffsetChain[0].Value.Str));
					return;
				}

				if (f == null && _prog.Data.Files.IsFdNamed(offref.OffsetChain[offref.OffsetChain.Count - 1].Value.Str))
				{
					// The FD name is valid as a record name.
					_writer.Write(" /**TODO: FD record reference*/ ");
					_writer.Write(CsFieldNameConverter.Convert(offref.OffsetChain[offref.OffsetChain.Count - 1].Value.Str) + "." + CsFieldNameConverter.Convert(offref.OffsetChain[0].Value.Str));
					return;
				}

				Stack<INamedField> stk = new Stack<INamedField>();
				INamedField fo = f.Parent;
				while (fo != null)
				{
					stk.Push(fo);
					fo = fo.Parent;
				}

				bool writeDot = false;

				if (stk.Count == 0)
				{
					Debug.Assert(f is INamedField);
					Debug.Assert(((INamedField)f).FileSectionMessageName.Length > 0);
					_writer.Write(CsFieldNameConverter.Convert(((INamedField)f).FileSectionMessageName));
				}

				while (stk.Count > 0)
				{
					fo = stk.Pop();

					if (writeDot)
					{
						_writer.Write(".");
					}
					else
					{
						writeDot = true;
						string fons = CsFormater.GetNamespace(fo);
						if (!String.IsNullOrEmpty(fons))
						{
							_writer.Write(fons);
							if (String.IsNullOrEmpty(fo.FileSectionMessageName))
							{
								_writer.Write(".");
							}
						}
					}

					_writer.Write(CsFieldNameConverter.Convert(fo.Name, fo.SpecialNameHandling));

					if (fo.Attributes.Occures != null && fo.Attributes.Occures.MaximumTimes > 1)
					{
						Id occurs = offref.FindOffset();

						if (occurs != null)
						{
							for (int x = 0; x < occurs.Offsets.Count; x++)
							{
								_writer.Write("[");
								if (! occurs.Offsets[x].IsNumber)
								{
									_writer.Write("(int)");
								}
								Write(occurs.Offsets[x]);
								if (! occurs.Offsets[x].IsNumber && !_prog.IsNative(occurs.Offsets[x]))
								{
									_writer.Write(".ToInt()");
								}
								_writer.Write("-1]");
							}
						}
						else
						{
							// Binary copy
							//throw new NotImplementedException();
						}
					}
				}

				if (writeDot)
				{
					_writer.Write(".");
				}
				_writer.Write(CsFieldNameConverter.Convert(f.Name, f.SpecialNameHandling));

				if (f.Occurances > 1)
				{
					Id occurs = offref.FindOffset();

					for (int x = 0; x < occurs.Offsets.Count; x++)
					{
						_writer.Write("[");
						if (!occurs.Offsets[x].IsNumber)
						{
							_writer.Write("(int)");
						}
						Write(occurs.Offsets[x]);
						if (! occurs.Offsets[x].IsNumber)
						{
							_writer.Write(".ToInt()");
						}
						_writer.Write("-1]");
					}
				}
			}
			else if (t is ExprTerm)
			{
				_writer.Write("(");
				Write(((ExprTerm)t).InnerExpression);
				_writer.Write(")");
			}
			else if (t is Range)
			{
				Debug.Assert(((Range)t).Ranges.Count == 1);

				_writer.Write("CreateRange(");
				Write(((Range)t).Ranges[0].From);
				_writer.Write(", ");
				Write(((Range)t).Ranges[0].To);
				_writer.Write(")");
			}
			else if (t is Function)
			{
				Function f = (Function)t;
				_writer.Write(CsFieldNameConverter.Convert(f.Name));

				if (f.Arguments != null)
				{
					Write(f.Arguments);
				}
			}
			else
			{
				_writer.Write(t.ToString());
			}
		}

		public void Dispose()
		{
			if (_csSql != null)
			{
				_csSql.Dispose();
			}
		}
	}
}
