using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using CobolParser.SQL.Statements;
using CobolParser.Records;
using CobolParser.SQL.Conds;
using DOR.Core.Collections;
using CobolParser.SQL;
using DOR.Core;
using CobolParser.Generator;

namespace CobolParser.Text
{
	public class CsSql : IDisposable
	{
		public static int Count;

		public static bool BatchTransactions
		{
			get;
			set;
		}

		public string DataLayerName
		{
			get;
			private set;
		}

		private CobolProgram _prog;
		private ISourceWriter _writer;
		private SourceWriter _writerI;
		private SourceWriter _writerDa;
		private SourceWriter _writerJava;
		private SourceWriter _writerJDa;
		private SourceWriter _writerCDa;
		private string _sqlListFileName;
		private CsVerbs _cvsx;
		private ISourceWriter _bufWriter = new SourceWriterBuffer();

		public UnitTestProject UnitTests
		{
			get;
			set;
		}

		public static INamedField Find(CobolProgram prog, CondParam v)
		{
			if (String.IsNullOrEmpty(v.RecordName))
			{
				return prog.Data.LocateField(v.FieldName, null);
			}
			else
			{
				return prog.Data.LocateField(v.FieldName, v.ParentRecordName());
			}
		}

		public CsSql
		(
			CobolProgram prog, 
			CsVerbs cvsx, 
			ISourceWriter writer, 
			UnitTestProject unitTests
		)
		{
			_prog = prog;
			_writer = writer;
			_cvsx = cvsx;
			UnitTests = unitTests;

			string convFileName = CsFieldNameConverter.Convert(prog.ProgramId);
			string daFilename = Path.Combine(prog.SourceDirectory, convFileName + "Data.cs");
			string idaFilename = Path.Combine(prog.SourceDirectory, "I" + convFileName + "Data.cs");
			string javaFilename = Path.Combine(prog.SourceDirectory, convFileName + "Data.java");
			string javaDaFilename = Path.Combine(prog.SourceDirectory, convFileName + "JavaData.cs");
			string cDaFilename = Path.Combine(prog.SourceDirectory, convFileName + "CData.cs");
			_sqlListFileName = Path.Combine(prog.SourceDirectory, convFileName + ".txt");

			if (File.Exists(_sqlListFileName))
			{
				File.Delete(_sqlListFileName);
			}

			if (File.Exists(daFilename))
			{
				File.Delete(daFilename);
			}
			_writerDa = new SourceWriter(daFilename);

			if (File.Exists(idaFilename))
			{
				File.Delete(idaFilename);
			}
			_writerI = new SourceWriter(idaFilename);

			if (File.Exists(javaDaFilename))
			{
				File.Delete(javaDaFilename);
			}
			_writerJDa = new SourceWriter(javaDaFilename);

			if (File.Exists(cDaFilename))
			{
				File.Delete(cDaFilename);
			}
			_writerCDa = new SourceWriter(cDaFilename);

			if (File.Exists(javaFilename))
			{
				File.Delete(javaFilename);
			}
			_writerJava = new SourceWriter(javaFilename);
			_writerJava.LineEnding = "\n";

			DataLayerName = convFileName + "Data";

			WriteJavaHeader();

			_writerI.WriteUsing("System");
			_writerI.WriteUsing("System.Collections.Generic");
			_writerI.WriteUsing("System.Data");
			_writerI.WriteLine();

			_writerI.WriteUsing("DOR.Core");
			_writerI.WriteUsing("DOR.Core.Collections");
			_writerI.WriteUsing("DOR.Core.Data");
			_writerI.WriteUsing("DOR.WorkingStorage");
			_writerI.WriteLine();

			_writerI.WriteLine("namespace " + convFileName);
			_writerI.IndentManual();
			_writerI.WriteLine("public interface I" + convFileName + "Data : IDisposable");
			_writerI.IndentManual();
			_writerI.WriteLine("void BeginTrans();");
			_writerI.WriteLine("void CommitTrans();");
			_writerI.WriteLine("void RollbackTrans();");
			_writerI.WriteLine();

			_writerCDa.WriteUsing(_writerJDa.WriteUsing(_writerDa.WriteUsing("System")));
			_writerCDa.WriteUsing(_writerJDa.WriteUsing(_writerDa.WriteUsing("System.Collections.Generic")));
			_writerCDa.WriteUsing(_writerJDa.WriteUsing(_writerDa.WriteUsing("System.Data")));
			_writerCDa.WriteUsing(_writerJDa.WriteUsing(_writerDa.WriteUsing("System.Diagnostics")));
			_writerCDa.WriteUsing(_writerJDa.WriteUsing("System.Text"));
			_writerCDa.WriteUsing(_writerJDa.WriteUsing("System.Xml"));
			_writerDa.WriteLine();
			_writerJDa.WriteLine();
			_writerCDa.WriteLine();

			_writerCDa.WriteUsing(_writerJDa.WriteUsing(_writerDa.WriteUsing("DOR.Core")));
			_writerCDa.WriteUsing(_writerJDa.WriteUsing(_writerDa.WriteUsing("DOR.Core.Collections")));
			_writerCDa.WriteUsing(_writerJDa.WriteUsing(_writerDa.WriteUsing("DOR.Core.Config")));
			_writerCDa.WriteUsing(_writerJDa.WriteUsing(_writerDa.WriteUsing("DOR.Core.Data")));
			_writerCDa.WriteUsing(_writerJDa.WriteUsing(_writerDa.WriteUsing("DOR.Core.Data.Tandem")));
			_writerCDa.WriteUsing(_writerJDa.WriteUsing(_writerDa.WriteUsing("DOR.WorkingStorage")));
			_writerDa.WriteLine();
			_writerJDa.WriteLine();
			_writerCDa.WriteLine();

			_writerCDa.WriteLine(_writerJDa.WriteLine(_writerDa.WriteLine("namespace " + convFileName)));
			_writerDa.IndentManual();
			_writerJDa.IndentManual();
			_writerCDa.IndentManual();

			_writerDa.WriteLine("public class " + convFileName + "Data : I" + convFileName + "Data");
			_writerJDa.WriteLine("public class " + convFileName + "JavaData : TandemDataAccessBase, I" + convFileName + "Data");
			_writerCDa.WriteLine("public class " + convFileName + "CData : CDataAccessBase, I" + convFileName + "Data");

			_writerDa.IndentManual();
			_writerJDa.IndentManual();
			_writerCDa.IndentManual();

			#region WriterDA Specific

			_writerDa.WriteLine("ISqlDataAccess Connection");
			using (_writerDa.Indent())
			{
				_writerDa.WriteLine("get; set;");
			}
			_writerDa.WriteLine();

			_writerDa.WriteLine("public " + convFileName + "Data(ISqlDataAccess connection)");
			using (_writerDa.Indent())
			{
				_writerDa.WriteLine("Connection = connection;");
			}
			_writerDa.WriteLine();

			_writerDa.WriteLine("public void BeginTrans()");
			_writerDa.WriteLine("{");
			_writerDa.WriteLine("\tConnection.BeginTrans();");
			_writerDa.WriteLine("}");
			_writerDa.WriteLine();

			_writerDa.WriteLine("public void CommitTrans()");
			_writerDa.WriteLine("{");
			_writerDa.WriteLine("\tConnection.CommitTrans();");
			_writerDa.WriteLine("}");
			_writerDa.WriteLine();

			_writerDa.WriteLine("public void RollbackTrans()");
			_writerDa.WriteLine("{");
			_writerDa.WriteLine("\tConnection.RollbackTrans();");
			_writerDa.WriteLine("}");
			_writerDa.WriteLine();

			_writerDa.WriteLine("public void Dispose()");
			_writerDa.WriteLine("{");
			_writerDa.WriteLine("\tConnection.Close();");
			_writerDa.WriteLine("\tConnection.Dispose();");
			_writerDa.WriteLine("}");
			_writerDa.WriteLine();

			#endregion

			#region Writer Java DA specific

			_writerJDa.WriteLine("public " + convFileName + "JavaData()");
			_writerJDa.WriteLine(": this(Configuration.AppConfig)");
			using (_writerJDa.Indent())
			{
			}
			_writerJDa.WriteLine();

			_writerJDa.WriteLine("public " + convFileName + "JavaData(IConfiguration config)");
			_writerJDa.WriteLine(": base(config)");
			using (_writerJDa.Indent())
			{
				_writerJDa.WriteLine("ServletDirectoryName = \"servlet-et\";");
			}
			_writerJDa.WriteLine();

			_writerJDa.WriteLine("public override void Dispose()");
			_writerJDa.WriteLine("{");
			_writerJDa.WriteLine("\tbase.Dispose();");
			_writerJDa.WriteLine("}");
			_writerJDa.WriteLine();

			#endregion

			#region Writer C DA specific

			_writerCDa.WriteLine("public " + convFileName + "CData(string connectionString)");
			_writerCDa.WriteLine(": base(connectionString)");
			using (_writerCDa.Indent())
			{
			}
			_writerCDa.WriteLine();

			_writerCDa.WriteLine("public " + convFileName + "CData()");
			_writerCDa.WriteLine(": this(Configuration.AppConfig)");
			using (_writerCDa.Indent())
			{
			}
			_writerCDa.WriteLine();

			_writerCDa.WriteLine("public " + convFileName + "CData(IConfiguration config)");
			_writerCDa.WriteLine(": base((string)config[\"C_CONNECTION_STRING\"])");
			using (_writerCDa.Indent())
			{
			}
			_writerCDa.WriteLine();

			_writerCDa.WriteLine("public override void Dispose()");
			_writerCDa.WriteLine("{");
			_writerCDa.WriteLine("\tbase.Dispose();");
			_writerCDa.WriteLine("}");
			_writerCDa.WriteLine();

			#endregion

			AppendSqlListFile("CREATE LOGIN " + _prog.Identification.System + "_Application WITH PASSWORD='1234';\n\n");
		}

		public void AppendSqlListFile(string txt)
		{
			File.AppendAllText(_sqlListFileName, txt);
		}

		public void Write
		(
			CondParam v,
			bool parameterize,
			bool writeQuotes,
			bool applyCasts,
			bool stripPeriods
		)
		{
			if (applyCasts && v.NullIndicatorField != null)
			{
				// :EMAIL-STATUS-DATE INDICATOR :EMAIL-STATUS-DATE-I TYPE AS DATE

				Debug.Assert(!parameterize);

				_writerJava.Write(_writerDa.Write("("));
				Write(v.NullIndicatorField, parameterize, writeQuotes, applyCasts, !parameterize);
				_writerJava.Write(_writerDa.Write(" < 0 ? "));

				if (! String.IsNullOrEmpty(v.NewType))
				{
					_writerJava.Write(_writerDa.Write("\""));
					_writerJava.Write(_writerDa.Write(v.NewType));
					_writerJava.Write(_writerDa.Write(" '\" + "));
				}

				ISourceWriter back = _cvsx.Writer;
				_cvsx.Writer = _bufWriter;
				_cvsx.GetFieldReference(v.FieldName, v.RecordName);
				_cvsx.Writer = back;

				if (stripPeriods)
				{
					_writerJava.Write(_writerDa.Write(_bufWriter.ToString().Replace(".", "")));
				}
				else
				{
					string ss = _writerDa.Write(_bufWriter.ToString());
					_writerJava.Write(ss.Replace(".", ""));
				}

				if (!String.IsNullOrEmpty(v.NewType))
				{
					_writerJava.Write(_writerDa.Write(" + \"'\""));
				}

				_writerJava.Write(_writerDa.Write(" : \"NULL\") "));
				return;
			}

			if (applyCasts && !String.IsNullOrEmpty(v.NewType))
			{
				if (writeQuotes)
				{
					_writerJava.Write(_writerDa.Write("\""));
				}
				_writerJava.Write(_writerDa.Write(" "));
				_writerJava.Write(_writerDa.Write(v.NewType));
				_writerJava.Write(_writerDa.Write(" "));
				if (writeQuotes)
				{
					_writerJava.Write(_writerDa.Write("\" + "));
				}
			}
			
			INamedField f = Find(_prog, v);
			if 
			(
				writeQuotes && 
				(
					v.NewType == "DATE" || 
					(
						!f.Attributes.Pic.PicFormat.IsNumeric && 
						(
							v.NewType == "TIMESTAMP" || f.Attributes.Pic.PicFormat.Decimals == 0
						)
					)
				)
			)
			{
				if (parameterize)
				{
					_writerJava.Write(_writerDa.Write("'"));
				}
				else
				{
					_writerJava.Write(_writerDa.Write("\""));
					_writerJava.Write(_writerDa.Write("'\" + "));
				}
			}

			if (parameterize)
			{
				_writerDa.Write("?[");
				_writerJava.Write("\" + ");
			}

			ISourceWriter back2 = _cvsx.Writer;
			_cvsx.Writer = new SourceWriterBuffer();
			_cvsx.GetFieldReference(v.FieldName, v.RecordName);
			string s = _cvsx.Writer.ToString();
			_cvsx.Writer = back2;

			if (stripPeriods)
			{
				_writerJava.Write(_writerDa.Write(s.Replace(".", "")));
			}
			else
			{
				_writerDa.Write(s);
				_writerJava.Write(s.Replace(".", ""));
			}

			if (parameterize)
			{
				_writerDa.Write("]");
				_writerJava.Write(" + \"");
			}

			if 
			(
				writeQuotes && 
				(
					v.NewType == "DATE" || 
					(
						!f.Attributes.Pic.PicFormat.IsNumeric && 
						(v.NewType == "TIMESTAMP" || f.Attributes.Pic.PicFormat.Decimals == 0)
					)
				)
			)
			{
				if (parameterize)
				{
					_writerJava.Write(_writerDa.Write("'"));
				}
				else
				{
					_writerJava.Write(_writerDa.Write(" + \"'\" "));
				}
			}

			if (v.AsTypeConversion != null)
			{
				Write(v.AsTypeConversion, false, true);
			}

			if (applyCasts && !String.IsNullOrEmpty(v.NewType))
			{
				if (writeQuotes)
				{
					_writerJava.Write(_writerDa.Write("+ \""));
				}
				if (!String.IsNullOrEmpty(v.NewTypeQualifier))
				{
					_writerJava.Write(_writerDa.Write(" "));
					_writerJava.Write(_writerDa.Write(v.NewTypeQualifier));
					_writerJava.Write(_writerDa.Write(" "));
				}
				if (writeQuotes)
				{
					_writerJava.Write(_writerDa.Write("\" "));
				}
			}
		}

		private void Write
		(
			ISqlCondToken c,
			bool parameterize,
			bool applyCasts
		)
		{
			if (c is CondOperator)
			{
				if (c.ToString() == "AND" || c.ToString() == "OR")
				{
					_writerJava.Write(_writerDa.Write("\" +"));
					_writerDa.WriteLine();
					_writerJava.WriteLine();
					_writerDa.WriteIndent();
					_writerJava.WriteIndent();
					_writerJava.Write(_writerDa.Write("\t\t\""));
				}
				_writerJava.Write(_writerDa.Write(c.ToString()));
			}
			else if (c is CondParam)
			{
				if (!parameterize)
				{
					_writerJava.Write(_writerDa.Write("\" + "));
				}
				Write((CondParam)c, parameterize, !parameterize, applyCasts, !parameterize);
				if (!parameterize)
				{
					_writerJava.Write(_writerDa.Write(" + \""));
				}
			}
			else if (c is CondField || c is CondValue)
			{
				_writerJava.Write(_writerDa.Write(c.ToString().Replace("\"", "\\\"")));
				_writerJava.Write(_writerDa.Write(" "));
			}
			else if (c is CondTrim)
			{
				_writerJava.Write(_writerDa.Write("TRIM("));
				if (((CondTrim)c).IsLeading)
				{
					_writerJava.Write(_writerDa.Write("LEADING "));
				}
				else if (((CondTrim)c).IsTrailing)
				{
					_writerJava.Write(_writerDa.Write("TRAILING "));
				}
				else if (((CondTrim)c).IsBoth)
				{
					_writerJava.Write(_writerDa.Write("BOTH "));
				}

				string removeThist = ((CondTrim)c).RemoveThis;
				if (!String.IsNullOrWhiteSpace(StringHelper.StripQuotes(removeThist)))
				{
					_writerJava.Write(_writerDa.Write(removeThist));
					_writerJava.Write(_writerDa.Write(" FROM "));
				}
				Write(((CondTrim)c).Field, parameterize);
				_writerJava.Write(_writerDa.Write(") "));
			}
			else if (c is CondExpr)
			{
				if (((CondExpr)c).Expr[0].UseParens)
				{
					_writerJava.Write(_writerDa.Write("("));
				}
				foreach (var t in ((CondExpr)c).Expr[0].Terms)
				{
					Write(t, parameterize, applyCasts);
					_writerJava.Write(_writerDa.Write(" "));
				}
				if (((CondExpr)c).Expr[0].UseParens)
				{
					_writerJava.Write(_writerDa.Write(")"));
				}
				if (((CondExpr)c).Expr.Count > 1)
				{
					Debug.Assert(((CondExpr)c).Expr.Count == 2);

					if (((CondExpr)c).Expr[1].UseParens)
					{
						_writerJava.Write(_writerDa.Write("("));
					}
					foreach (var t in ((CondExpr)c).Expr[1].Terms)
					{
						Write(t, parameterize, applyCasts);
						_writerJava.Write(_writerDa.Write(" "));
					}
					if (((CondExpr)c).Expr[1].UseParens)
					{
						_writerJava.Write(_writerDa.Write(")"));
					}
				}
			}
			else if (c is CondFunction)
			{
				CondFunction fn = (CondFunction)c;

				_writerJava.Write(_writerDa.Write(fn.FunctionName));
				_writerJava.Write(_writerDa.Write("("));

				if (fn.IsDistinct)
				{
					_writerJava.Write(_writerDa.Write("DISTINCT "));
				}

				bool writeComma = false;

				foreach (var e in fn.Arguments)
				{
					if (writeComma)
					{
						_writerJava.Write(_writerDa.Write(", "));
					}
					else
					{
						writeComma = true;
					}

					foreach (var t in e.Terms)
					{
						Write(t, parameterize, applyCasts);
						_writerJava.Write(_writerDa.Write(" "));
					}
				}

				_writerJava.Write(_writerDa.Write(")"));
			}
			else if (c is CondSubSelect)
			{
				CondSubSelect ss = (CondSubSelect)c;
				_writerJava.Write(_writerDa.Write("(\" +"));
				_writerDa.WriteLine();
				_writerJava.WriteLine();
				WriteSelectInner(ss.SubSelect, parameterize);
				_writerJava.Write(_writerDa.Write(" + "));
				_writerDa.WriteIndent();
				_writerJava.WriteIndent();
				_writerJava.Write(_writerDa.Write("\t\")"));
			}
			else if (c is CondIn)
			{
				CondIn i = (CondIn)c;
				_writerJava.Write(_writerDa.Write(" IN ("));
				if (i.SubSelect != null)
				{
					_writerJava.Write(_writerDa.Write("\" +"));
					WriteSelectInner(i.SubSelect, parameterize);
					_writerDa.WriteIndent();
					_writerJava.WriteIndent();
					_writerJava.Write(_writerDa.Write("\t+ \""));
				}
				else
				{
					for (int x = 0; x < i.Items.Count; x++)
					{
						if (x > 0)
						{
							_writerJava.Write(_writerDa.Write(", "));
						}

						if (i.Items[x][0] == '"')
						{
							_writerJava.Write(_writerDa.Write(i.Items[x].Replace("\"", "\\\"")));
						}
						else
						{
							_writerJava.Write(_writerDa.Write(i.Items[x]));
						}
					}
				}
				_writerJava.Write(_writerDa.Write(")"));
			}
			else if (c is Current)
			{
				_writerJava.Write(_writerDa.Write(c.ToString()));
				_writerJava.Write(_writerDa.Write(" "));
			}
			else if (c is CondNoOp)
			{
				// do nothing
			}
			else if (c is CondDateTime)
			{
				CondDateTime cdf = (CondDateTime)c;
				_writerJava.Write(_writerDa.Write("DATETIME "));
				_writerJava.Write(_writerDa.Write(cdf.Value.Replace("\"", "\\\"")));
				_writerJava.Write(_writerDa.Write(" "));
				_writerJava.Write(_writerDa.Write(cdf.FromSomething));
				_writerJava.Write(_writerDa.Write(" TO "));
				_writerJava.Write(_writerDa.Write(cdf.ToSomthing));
			}
			else if (c is CondBetween)
			{
				CondBetween b = (CondBetween)c;
				_writerJava.Write(_writerDa.Write("BETWEEN "));
				Write(b.Lows, true, true);
				_writerJava.Write(_writerDa.Write(" AND "));
				Write(b.Highs, true, true);
			}
			else if (c is CondFieldList)
			{
				CondFieldList cfl = (CondFieldList)c;
				for (int x = 0; x < cfl.Fields.Count; x++)
				{
					if (x > 0)
					{
						_writerJava.Write(_writerDa.Write(", "));
					}
					Write(cfl.Fields[0], true, true);
				}
			}
			else if (c is CondExists)
			{
				CondExists ce = (CondExists)c;
				_writerJava.Write(_writerDa.Write("EXISTS (\" +"));
				WriteSelectInner(ce.SubSelect, parameterize);
				_writerDa.WriteIndent();
				_writerJava.WriteIndent();
				_writerJava.Write(_writerDa.Write("\t+ \") "));
			}
			else if (c is CondInterval)
			{
				CondInterval ci = (CondInterval)c;
				_writerJava.Write(_writerDa.Write(ci.ToString()));
			}
			else if (c is CastCond)
			{
				CastCond cc = (CastCond)c;
				_writerJava.Write(_writerDa.Write("CAST('"));
				Write(cc.Field, parameterize);
				_writerJava.Write(_writerDa.Write("'"));
				// expr consumes the AS phrase
				if (cc.NewType != null)
				{
					_writerJava.Write(_writerDa.Write(" AS "));
					_writerJava.Write(_writerDa.Write(cc.NewType.ToString()));
				}
				_writerJava.Write(_writerDa.Write(")"));
			}
			else if (c is SqlType)
			{
				_writerJava.Write(_writerDa.Write(c.ToString()));
			}
			else if (c is CondDateFormat)
			{
				_writerJava.Write(_writerDa.Write(c.ToString()));
			}
			else
			{
				throw new Exception("Internal error");
			}
		}

		private void WriteSelectInner
		(
			Select s,
			bool parameterize
		)
		{
			if (s.IsDistinct)
			{
				_writerJava.WriteLine(_writerDa.WriteLine("\t\"SELECT DISTINCT \" +"));
			}
			else
			{
				_writerJava.WriteLine(_writerDa.WriteLine("\t\"SELECT \" +"));
			}

			bool writeComma = false;

			foreach (var f in s.Fields)
			{
				_writerDa.WriteIndent();
				_writerJava.WriteIndent();
				_writerJava.Write(_writerDa.Write("\t\t"));

				if (writeComma)
				{
					_writerJava.Write(_writerDa.Write(" \", "));
				}
				else
				{
					_writerJava.Write(_writerDa.Write("\""));
					writeComma = true;
				}

				for (int x = 0; x < f.Terms.Count; x++)
				{
					Write(f.Terms[x], parameterize, true);
				}

				_writerJava.Write(_writerDa.Write("\" +"));
				_writerDa.WriteLine();
				_writerJava.WriteLine();
			}

			_writerJava.WriteLine(_writerDa.WriteLine("\t\"FROM \" +"));

			if (s.Joins.Count > 0)
			{
				foreach (var f in s.Joins)
				{
					_writerJava.WriteLine(_writerDa.WriteLine("\t\t\"" + f.ToString() + " \" +"));
				}
			}
			else
			{
				writeComma = false;
				foreach (var t in s.Tables)
				{
					string tv = t.ToString().Trim();
					if (tv.IndexOf(' ') < 0)
					{
						tv = tv + " " + t.TableName;
					}
					if (writeComma)
					{
						_writerJava.WriteLine(_writerDa.WriteLine("\t\t \", " + tv + " \" +"));
					}
					else
					{
						_writerJava.WriteLine(_writerDa.WriteLine("\t\t\" " + tv + " \" +"));
						writeComma = true;
					}
				}
			}

			if (s.Where != null)
			{
				_writerJava.WriteLine(_writerDa.WriteLine("\t\"WHERE \" +"));

				_writerDa.WriteIndent();
				_writerJava.WriteIndent();

				_writerJava.Write(_writerDa.Write("\t\t\""));

				for (int i = 0; i < s.Where.Terms.Count; i++)
				{
					var w = s.Where.Terms[i];

					if (!(w is CondFieldList))
					{
						Write(w, parameterize, true);
					}
					else
					{
						CondFieldList left = (CondFieldList)w;
						CondOperator op = (CondOperator)s.Where.Terms[i + 1];
						CondFieldList right = (CondFieldList)s.Where.Terms[i + 2];

						Debug.Assert(left.Fields.Count == right.Fields.Count);

						i += 2;

						_writerJava.Write(_writerDa.Write("(\" +"));
						_writerDa.WriteLine();
						_writerJava.WriteLine();
						_writerDa.WriteIndent();
						_writerJava.WriteIndent();
						_writerJava.Write(_writerDa.Write("\t\t\""));

						for (int x = 0; x < left.Fields.Count; x++)
						{
							if (x > 0)
							{
								_writerJava.Write(_writerDa.Write(" AND "));
							}

							Write(left.Fields[x], parameterize, true);
							_writerJava.Write(_writerDa.Write(" "));
							_writerJava.Write(_writerDa.Write(op.ToString()));
							_writerJava.Write(_writerDa.Write(" "));

							Write(right.Fields[x], parameterize, true);

							_writerJava.Write(_writerDa.Write("\" +"));
							_writerDa.WriteLine();
							_writerJava.WriteLine();
							_writerDa.WriteIndent();
							_writerJava.WriteIndent();
							_writerJava.Write(_writerDa.Write("\t\t\""));
						}

						_writerJava.Write(_writerDa.Write(")"));
					}
					_writerJava.Write(_writerDa.Write(" "));
				}
				_writerJava.Write(_writerDa.Write("\" +"));
				_writerDa.WriteLine();
				_writerJava.WriteLine();
			}

			if (s.Orders.Count > 0)
			{
				_writerJava.WriteLine(_writerDa.WriteLine("\t\"ORDER BY \" +"));
				writeComma = false;

				foreach (var o in s.Orders)
				{
					if (writeComma)
					{
						_writerJava.WriteLine(_writerDa.WriteLine("\t\t\", " + o.ToString() + "\" +"));
					}
					else
					{
						_writerJava.WriteLine(_writerDa.WriteLine("\t\t\"" + o.ToString() + "\" +"));
						writeComma = true;
					}
				}
			}

			if (s.Groups.Count > 0)
			{
				throw new NotImplementedException();
			}

			if (s.Having != null)
			{
				throw new NotImplementedException();
			}

			if (s.UnionWith != null)
			{
				_writerJava.WriteLine(_writerDa.WriteLine("\t\"UNION \" +"));
				WriteSelectInner(s.UnionWith, parameterize);
			}
			else
			{
				if (String.IsNullOrEmpty(s.ForThisAccess))
				{
					_writerJava.WriteLine(_writerDa.WriteLine("\t\" FOR BROWSE ACCESS\""));
				}
				else
				{
					_writerJava.WriteLine(_writerDa.WriteLine("\t\" FOR " + s.ForThisAccess + " ACCESS\""));
				}
			}
		}

		private void WriteJavaDaPrmBuild(List<CondParam> prms, string action, string methodName)
		{
			_writerJDa.WriteLine("List<TandemParameter> prm = new List<TandemParameter>();");
			_writerJDa.WriteLine();
			_writerJDa.WriteLine("prm.Add(BuildParameter(\"name\", \"" + methodName + "\"));");
			_writerJDa.WriteLine("prm.Add(BuildParameter(\"action\", \"" + action + "\"));");

			_writerCDa.WriteLine("List<ICParameter> prm = new List<ICParameter>();");
			_writerCDa.WriteLine();

			_prmIdx.Clear();

			ISourceWriter back = _cvsx.Writer;
			_cvsx.Writer = _bufWriter;

			foreach (var p in prms)
			{
				INamedField f = _cvsx.GetFieldReference(p.FieldName, p.RecordName);
				string name = _bufWriter.ToString().Replace(".", "");

				if (_prmIdx.ContainsKey(name))
				{
					continue;
				}
				_prmIdx.Add(name, p);

				_writerJDa.WriteLine("prm.Add(BuildParameter(\"" + name + "\", " + name + ".ToString()));");

				_writerCDa.WriteLine("prm.Add((ICParameter)(new CParameter<" + f.Attributes.Pic.CTypeName() + ">(\"" + name + "\", " + name + f.Attributes.Pic.CConversionMethod() + ")));");
			}
			_cvsx.Writer = back;

			if (prms.Count > 0)
			{
				_writerJDa.WriteLine();
				_writerCDa.WriteLine();
			}
		}

		private string WriteSelect
		(
			Select s,
			bool parameterize,
			string methodName
		)
		{
			string cmdName = "sql" + Count.ToString();
			_writerDa.WriteLine("string " + cmdName + " = ");
			_writerJava.WriteLine("String " + cmdName + " = ");

			#region Java DA CS

			WriteJavaDaPrmBuild(s.GetParameters(), "inquire", methodName);

			_writerJDa.WriteLine("return ExecuteReader(\"et." + DataLayerName + "\", null, prm.ToArray());");

			_writerCDa.WriteLine("return ExecuteReader(\"" + methodName + "\", prm.ToArray());");

			#endregion

			WriteSelectInner(s, parameterize);

			_writerJava.WriteLine(_writerDa.WriteLine(";"));

			_writerJava.WriteLine();
			_writerJava.WriteLine("boolean includeMeta = args.get(\"returnMetaData\") != null;");
			_writerJava.WriteLine(cmdName + " = ResolveDefines(" + cmdName + ");");
			_writerJava.WriteLine("executeQuery(" + cmdName + ", includeMeta, xml);");

			return cmdName;
		}

		private void WriteReaderRead
		(
			Vector<SqlExpr> fields,
			Vector<CondParam> IntoVars, 
			string readerName
		)
		{
			_writer.WriteLine("if (" + readerName + " != null && " + readerName + ".Read())");
			using (_writer.Indent())
			{
				_writer.WriteLine("Sqlcodex.Set(0);");
				_writer.WriteLine();

				int pos = 0;
				foreach (var v in IntoVars)
				{
					INamedField f = Find(_prog, v);

					if (!String.IsNullOrEmpty(v.NewType))
					{
						if (v.NewType.Equals("INTERVAL", StringComparison.InvariantCultureIgnoreCase))
						{
							_writer.WriteLine("///TODO: Java cannot convert INTERVAL to a usable value, so the SQL");
							_writer.WriteLine("///statement will get an error.  Try using Date or DateTime instead.");
						}
					}

					_writer.WriteIndent();

					_cvsx.GetFieldReference(v.FieldName, v.RecordName);

					_writer.Write(_prog.IsNative(f) ? " = " : ".Set(");

					bool forceString = false;

					if (!String.IsNullOrEmpty(v.NewType))
					{
						string fn = v.NewType + "-" + v.NewTypeQualifier;
						fn = fn.Replace(" ", "-").Replace("(", "").Replace(")", "");
						fn = CsFieldNameConverter.Convert(fn);
						_writer.Write(fn);
						_writer.Write("(");

						if (fn == "Timestamp")
						{
							forceString = true;
						}
					}

					_writer.Write(readerName);

					if (!forceString && f.Attributes.Pic.PicFormat.Decimals > 0)
					{
						_writer.Write(".GetDecimal(");
					}
					else if (f.Attributes.Pic.PicFormat.IsNumeric)
					{
						_writer.Write(".GetInt32(");
					}
					else
					{
						_writer.Write(".GetString(");
					}

					if 
					(
						fields[pos].Terms.Count == 1 && 
						fields[pos].Terms[0] is CondField &&
						String.IsNullOrWhiteSpace(((CondField)fields[pos].Terms[0]).Field.TableAlias)
					)
					{
						_writer.Write("\"" + ((CondField)fields[pos].Terms[0]).Field.FieldName + "\"");
					}
					else
					{
						_writer.Write(pos.ToString());
					}

					_writer.Write(")");

					if (!_prog.IsNative(f))
					{
						_writer.Write(")");
					}

					if (!String.IsNullOrEmpty(v.NewType))
					{
						_writer.Write(")");
					}

					_writer.Write(";");

					_writer.WriteLine();

					if (v.NullIndicatorField != null)
					{
						INamedField fnull = Find(_prog, v);
						_writer.WriteIndent();

						_cvsx.GetFieldReference(v.NullIndicatorField.FieldName, v.NullIndicatorField.RecordName);
						_writer.Write(_prog.IsNative(f) ? " = " : ".Set(");

						_writer.Write(readerName);
						_writer.Write(".IsDBNull(");
						_writer.Write(pos.ToString());
						_writer.Write(") ? -1 : 0");

						_writer.Write(_prog.IsNative(f) ? ";" : ");");
						_writer.WriteLine();
					}

					pos++;
				}
			}
			_writer.WriteLine("else");
			using (_writer.Indent())
			{
				_writer.WriteLine("if (Sqlcodex == 0)");
				_writer.WriteLine("{");
				_writer.WriteLine("\tSqlcodex.Set(100);");
				_writer.WriteLine("}");
			}
		}

		private void WriteDaCallParameters(List<CondParam> prms)
		{
			_prmIdx.Clear();

			int count = 0;

			ISourceWriter back = _cvsx.Writer;
			_cvsx.Writer = _bufWriter;

			foreach (var p in prms)
			{
				_cvsx.GetFieldReference(p.FieldName, p.RecordName);
				string name = _bufWriter.ToString();

				if (_prmIdx.ContainsKey(name))
				{
					continue;
				}
				_prmIdx.Add(name, p);

				_writer.WriteIndent();
				_writer.Write("\t");
				
				if (count > 0)
				{
					_writer.Write(", ");
				}
				
				_writer.Write(name);
				_writer.WriteLine();
				
				count++;
			}

			_cvsx.Writer = back;
		}

		public void SelectInto
		(
			Select s,
			bool parameterize
		)
		{
			string sqlNameL = "sql" + Count.ToString();
			string sqlName = "Sql" + Count.ToString();

			StringBuilder buf = new StringBuilder();
			foreach (var t in s.Tables)
			{
				buf.Append(CsFieldNameConverter.Convert(t.TableName));
			}
			buf.Append("Sel");
			buf.Append(Count);
			sqlName = buf.ToString();

			if (UnitTests != null)
			{
				UnitTests.AddCallMap(sqlName, s);
			}

			List<CondParam> prms = s.GetParameters();
			
			WriteMethodSig(sqlName, prms, "IDataReaderEx", false, s);
			_writerI.Write(";");
			_writerI.WriteLine();
			_writerI.WriteLine();

			_writerDa.WriteIndent();
			_writerDa.Write("//");
			_writerJava.IndentManual();
			WriteJavaArgUnpack(prms);
			_writerDa.WriteLine();

			_writerJDa.IndentManual();
			_writerCDa.IndentManual();
			using (_writerDa.Indent())
			{
				WriteSelect(s, parameterize, sqlName);
				_writerDa.WriteLine("return (IDataReaderEx)Connection.ExecuteReader(" + sqlNameL + ");");
			}
			_writerDa.WriteLine();
			_writerJDa.Unindent();
			_writerCDa.Unindent();
			_writerJDa.WriteLine();
			_writerCDa.WriteLine();
		
			string readerName = "r" + (Count++).ToString();
			_writer.WriteLine("IDataReaderEx " + readerName + " = null;");
			_writer.WriteLine("try");
			using (_writer.Indent())
			{
				_writer.WriteLine(readerName + " = DataAccess." + sqlName);
				_writer.WriteLine("(");

				WriteDaCallParameters(prms);

				_writer.WriteLine(");");
			}
			_writer.WriteLine("catch (Exception ex)");
			using (_writer.Indent())
			{
				_writer.WriteLine("Logger.Write(SystemPID." + _prog.Identification.System + ", ex);");
				_writer.WriteLine("Sqlcodex.Set(-1);");
			}
			_writer.WriteLine();

			_writerJava.Unindent();
			_writerJava.WriteLine();

			WriteReaderRead(s.Fields, s.IntoVars, readerName);
		}

		private Dictionary<string, CondParam> _prmIdx = new Dictionary<string, CondParam>();

		private void WriteMethodSig
		(
			string pcname, 
			List<CondParam> prms, 
			string returnType,
			bool isCursor,
			SqlStatement stmt
		)
		{
			_writerJava.WriteLine("private void " + pcname + "(StringBuffer xml, HashMap<String, String> args)");
			_writerJava.WriteLine("throws SQLException");

			_writerI.WriteLine(returnType + " " + pcname);
			_writerI.WriteLine("(");
			_writerCDa.WriteLine(_writerJDa.WriteLine(_writerDa.WriteLine("public " + returnType + " " + pcname)));
			_writerCDa.WriteLine(_writerJDa.WriteLine(_writerDa.WriteLine("(")));

			AppendSqlListFile("GRANT EXECUTE ON " + pcname + " TO " + _prog.Identification.System + "_Application;\n\n");

			if (isCursor)
			{
				AppendSqlListFile("CREATE CURSOR " + pcname + "\n");
			}
			else
			{
				AppendSqlListFile("CREATE PROCEDURE " + pcname + "\n");
			}
			AppendSqlListFile("(\n");

			int count = 0;
			_prmIdx.Clear();

			ISourceWriter back = _cvsx.Writer;
			_cvsx.Writer = _bufWriter;

			foreach (var p in prms)
			{
				_cvsx.GetFieldReference(p.FieldName, p.RecordName);
				string pname = _bufWriter.ToString().Replace(".", "");

				if (_prmIdx.ContainsKey(pname))
				{
					continue;
				}
				_prmIdx.Add(pname, p);

				_writerI.WriteIndent();
				_writerI.Write("\t");

				AppendSqlListFile("\t");

				if (count > 0)
				{
					_writerI.Write(", ");
					AppendSqlListFile(", ");
				}

				INamedField f = _prog.Data.LocateField(p.FieldName, p.RecordName);
				AppendSqlListFile(pname + " " + f.Attributes.Pic.SqlTypeName() + "(" + f.Attributes.Pic.Length.ToString());
				if (f.Attributes.Pic.PicFormat.Decimals != 0)
				{
					AppendSqlListFile("," + f.Attributes.Pic.PicFormat.Decimals);
				}
				AppendSqlListFile(")\n");

				_writerI.Write("IBufferOffset " + pname);
				_writerI.WriteLine();

				_writerDa.WriteIndent();
				_writerJDa.WriteIndent();
				_writerCDa.WriteIndent();

				_writerCDa.Write(_writerJDa.Write(_writerDa.Write("\t")));
				if (count > 0)
				{
					_writerCDa.Write(_writerJDa.Write(_writerDa.Write(", ")));
				}
				_writerCDa.Write(_writerJDa.Write(_writerDa.Write("IBufferOffset " + pname)));
				_writerDa.WriteLine();
				_writerJDa.WriteLine();
				_writerCDa.WriteLine();

				count++;
			}
			_writerI.WriteIndent();
			_writerI.Write(")");
			_writerCDa.WriteLine(_writerJDa.WriteLine(_writerDa.WriteLine(")")));

			//if (stmt is Select)
			//{
			//    foreach (var p in ((Select)stmt).IntoVars)
			//    {
			//        _cvsx.GetFieldReference(p.FieldName, p.RecordName);
			//        string pname = _bufWriter.ToString().Replace(".", "");

			//        if (_prmIdx.ContainsKey(pname))
			//        {
			//            continue;
			//        }
			//        _prmIdx.Add(pname, p);

			//        AppendSqlListFile("\t");
			//        AppendSqlListFile(", ");

			//        INamedField f = _prog.Data.LocateField(p.FieldName, p.RecordName);
			//        AppendSqlListFile(pname + " " + f.Attributes.Pic.SqlTypeName() + "(" + f.Attributes.Pic.Length.ToString());
			//        if (f.Attributes.Pic.PicFormat.Decimals != 0)
			//        {
			//            AppendSqlListFile("," + f.Attributes.Pic.PicFormat.Decimals);
			//        }
			//        AppendSqlListFile(") OUT\n");
			//    }
			//}
			AppendSqlListFile(")\n");

			_cvsx.Writer = back;
		}

		public void DefCursor(DeclareCursor s)
		{
			string name = CsFieldNameConverter.Convert(s.CursorName.ToString());
			string readerName = "Cmd" + name;
			
			List<CondParam> prms = s.Stmt.GetParameters();

			string pcname = CsFieldNameConverter.ConvertHarder(name);

			UnitTests.AddCallMap(pcname, s.Stmt);

			WriteMethodSig(pcname, prms, "IDataReaderEx", true, s);

			_writerDa.WriteIndent();
			_writerDa.Write("//");
			_writerJava.IndentManual();
			WriteJavaArgUnpack(prms);
			_writerDa.WriteLine();

			_writerI.Write(";");
			_writerI.WriteLine();
			_writerI.WriteLine();

			_prmIdx.Clear();

			_writerJDa.IndentManual();
			_writerCDa.IndentManual();

			using (_writerDa.Indent())
			{
				string sqlName = WriteSelect((Select)s.Stmt, true, pcname);

				_writerDa.WriteLine("TandemCommand " + readerName + " = new TandemCommand(sql" + Count.ToString() + ", Connection);");
				Count++;

				_writerJava.WriteIndent();
				_writerJava.Write("//");

				foreach (var p in prms)
				{
					if (_prmIdx.ContainsKey(p.Symbol.Lexum.Str))
					{
						continue;
					}
					_prmIdx.Add(p.Symbol.Lexum.Str, p);

					_writerDa.WriteIndent();
					_writerDa.Write(readerName + ".AddParameter(");
					Write(p, false, false, false, true);
					_writerDa.Write(", \"");
					Write(p, false, false, false, false);
					_writerDa.Write("\"");
					_writerDa.Write(");");
					_writerDa.WriteLine();
				}

				_writerDa.WriteLine();
				_writerDa.WriteLine("return (IDataReaderEx)" + readerName + ".ExecuteReader();");
				_writerJava.WriteLine();
			}
			_writerDa.WriteLine();
			_writerJDa.Unindent();
			_writerCDa.Unindent();
			_writerJDa.WriteLine();
			_writerCDa.WriteLine();

			_writerJava.Unindent();
			_writerJava.WriteLine();
		}

		public void OpenCursor(OpenSql s, DeclareCursor cursor)
		{
			string readerName = CsFieldNameConverter.Convert(s.CursorName.ToString());

			_writer.WriteLine("try");
			using (_writer.Indent())
			{
				_writer.WriteLine(readerName + " = DataAccess." + CsFieldNameConverter.ConvertHarder(readerName));
				_writer.WriteLine("(");

				List<CondParam> prms = cursor.Stmt.GetParameters();
				WriteDaCallParameters(prms);

				_writer.WriteLine(");");

				_writer.WriteLine("Sqlcodex.Set(0);");
			}
			_writer.WriteLine("catch (Exception ex)");
			using (_writer.Indent())
			{
				_writer.WriteLine("Logger.Write(SystemPID." + _prog.Identification.System + ", ex);");
				_writer.WriteLine("Sqlcodex.Set(-1);");
			}
			_writer.WriteLine();
		}

		public void Commit(CommitTrans s)
		{
			if (BatchTransactions)
			{
				Console.WriteLine("WARNING: Transaction converted to batch update; manual corrections to client code are probably needed.");
				_writer.WriteLine("//");
				_writer.WriteLine("///TODO: Transaction converted to batch update; manual corrections to client code");
				_writer.WriteLine("///are probably needed.");
				_writer.WriteLine("//");
				_writer.WriteLine("DataAccess.CommitTrans();");
			}
			else
			{
				_writer.WriteLine("//");
				_writer.WriteLine("///TODO: COMMIT-TRANSACTION");
				_writer.WriteLine("//");
			}
		}

		public void Del(Delete s)
		{
			string sqlName = "Sql" + Count.ToString();
			StringBuilder buf = new StringBuilder();
			foreach (var t in s.Tables)
			{
				buf.Append(CsFieldNameConverter.Convert(t.TableName));
			}
			buf.Append("Del");
			buf.Append(Count);
			sqlName = buf.ToString();

			UnitTests.AddCallMap(sqlName, s);

			List<CondParam> prms = s.GetParameters();
			WriteMethodSig(sqlName, prms, "int", false, s);
			_writerI.Write(";");
			_writerI.WriteLine();
			_writerI.WriteLine();

			_writerDa.WriteIndent();
			_writerDa.Write("//");
			_writerJava.IndentManual();
			WriteJavaArgUnpack(prms);
			_writerDa.WriteLine();

			_writerJDa.IndentManual();
			_writerCDa.IndentManual();
			WriteJavaDaPrmBuild(prms, "delete", sqlName);
			_writerJDa.WriteLine("return Execute(\"et." + DataLayerName + "\", prm.ToArray(), null);");
			_writerCDa.WriteLine("return ExecuteNonQuery(\"" + sqlName + "\", prm.ToList());");
			_writerJDa.Unindent();
			_writerCDa.Unindent();
			_writerJDa.WriteLine();
			_writerCDa.WriteLine();

			_writer.WriteLine("try");
			using (_writer.Indent())
			{
				using (_writerDa.Indent())
				{
					_writer.WriteLine("Sqlcodex.Set(0);");
					_writer.WriteLine("Sqlca.Rows = DataAccess." + sqlName);
					_writer.WriteLine("(");
					WriteDaCallParameters(prms);
					_writer.WriteLine(");");

					_writerDa.WriteLine("return Connection.ExecuteNonQuery");
					_writerDa.WriteLine("(");

					_writerJava.WriteLine("String sql = ");

					_writerJava.WriteLine(_writerDa.WriteLine("\t\"DELETE FROM " + s.Tables[0].ToString() + "\" +"));
					_writerJava.WriteLine(_writerDa.WriteLine("\t\"WHERE \" +"));

					_writerDa.WriteIndent();
					_writerJava.WriteIndent();
					_writerJava.Write(_writerDa.Write("\t\t\" +"));
					Write(s.Where, false);
					_writerJava.Write(_writerDa.Write("\" +"));
					_writerDa.WriteLine();
					_writerJava.WriteLine();
					_writerJava.WriteLine(_writerDa.WriteLine("\t\"FOR " + s.ForThisAccess + " ACCESS\""));
					_writerDa.WriteLine(");");
					_writerJava.WriteLine(";");
				}
				_writerDa.WriteLine();
			}
			_writer.WriteLine("catch (Exception ex)");
			using (_writer.Indent())
			{
				_writer.WriteLine("Logger.Write(SystemPID." + _prog.Identification.System + ", ex);");
				_writer.WriteLine("Sqlcodex.Set(-1);");
			}
			_writer.WriteLine();

			_writerJava.WriteLine();
			_writerJava.WriteLine("executeNonQuery(sql.toString().replace(\"[=defineOf]\", \"=\"), xml);");

			_writerJava.Unindent();
			_writerJava.WriteLine();

			Count++;
		}

		public void ExecCursor(Fetch s)
		{
			List<DeclareCursor> lst = _prog.ListCursors();
			DeclareCursor cursor = lst.Where(c => c.CursorName.ToString() == s.CursorName.ToString()).First();
			WriteReaderRead(((Select)cursor.Stmt).Fields, s.Prms, CsFieldNameConverter.Convert(s.CursorName.ToString()));
		}

		public void Write
		(
			SqlExpr s,
			bool parameterize
		)
		{
			foreach (var e in s.Terms)
			{
				if (s.ToString() == "AND")
				{
					_writerJava.Write(_writerDa.Write("\" +"));
					_writerDa.WriteLine();
					_writerJava.WriteLine();
					_writerDa.WriteIndent();
					_writerJava.WriteIndent();
					_writerJava.Write(_writerDa.Write("\t\t\""));
				}
				Write(e, parameterize, true);
				_writerJava.Write(_writerDa.Write(" "));
			}
		}

		public void Ins
		(
			Insert s,
			bool parameterize
		)
		{
			Debug.Assert(s.Tables.Count == 1);
			Debug.Assert(s.SubSelect == null);

			string sqlName = "Sql" + Count.ToString();
			StringBuilder buf = new StringBuilder();
			foreach (var t in s.Tables)
			{
				buf.Append(CsFieldNameConverter.Convert(t.TableName));
			}
			buf.Append("Ins");
			buf.Append(Count);
			sqlName = buf.ToString();

			if (UnitTests != null)
			{
				UnitTests.AddCallMap(sqlName, s);
			}

			List<CondParam> prms = s.GetParameters();
			WriteMethodSig(sqlName, prms, "int", false, s);
			_writerI.Write(";");
			_writerI.WriteLine();
			_writerI.WriteLine();

			_writerDa.WriteIndent();
			_writerDa.Write("//");
			_writerJava.IndentManual();
			WriteJavaArgUnpack(prms);
			_writerDa.WriteLine();

			_writerJDa.IndentManual();
			_writerCDa.IndentManual();
			WriteJavaDaPrmBuild(prms, "insert", sqlName);
			_writerJDa.WriteLine("return Execute(\"et." + DataLayerName + "\", prm.ToArray(), null);");
			_writerCDa.WriteLine("return ExecuteNonQuery(\"" + sqlName + "\", prm.ToList());");
			_writerJDa.Unindent();
			_writerCDa.Unindent();
			_writerJDa.WriteLine();
			_writerCDa.WriteLine();

			_writer.WriteLine("try");
			using (_writer.Indent())
			{
				using (_writerDa.Indent())
				{
					_writer.WriteLine("Sqlca.Rows = DataAccess." + sqlName);
					_writer.WriteLine("(");
					WriteDaCallParameters(prms);
					_writer.WriteLine(");");

					_writerJava.WriteLine("String sql =");

					_writerDa.WriteLine("return Connection.ExecuteNonQuery");
					_writerDa.WriteLine("(");

					_writerJava.WriteLine(_writerDa.WriteLine("\t\"INSERT INTO " + s.Tables[0].ToString() + "\" +"));
					_writerJava.WriteLine(_writerDa.WriteLine("\t\"(\" +"));

					if (s.FieldList != null)
					{
						for (int x = 0; x < s.FieldList.Count; x++)
						{
							_writerDa.WriteIndent();
							_writerJava.WriteIndent();
							_writerJava.Write(_writerDa.Write("\t\t"));

							_writerJava.Write(_writerDa.Write("\""));
							if (x > 0)
							{
								_writerJava.Write(_writerDa.Write(", "));
							}
							_writerJava.Write(_writerDa.Write(s.FieldList[x].ToString()));
							_writerJava.Write(_writerDa.Write("\" +"));
							_writerDa.WriteLine();
							_writerJava.WriteLine();
						}
					}

					_writerJava.WriteLine(_writerDa.WriteLine("\t\")\" +"));
					_writerJava.WriteLine(_writerDa.WriteLine("\t\"VALUES\" +"));
					_writerJava.WriteLine(_writerDa.WriteLine("\t\"(\" +"));

					for (int x = 0; x < s.Values.Count; x++)
					{
						_writerDa.WriteIndent();
						_writerJava.WriteIndent();
						_writerJava.Write(_writerDa.Write("\t\t\""));
						if (x > 0)
						{
							_writerJava.Write(_writerDa.Write(", "));
						}
						Write(s.Values[x], parameterize);
						_writerJava.Write(_writerDa.Write("\" +"));
						_writerDa.WriteLine();
						_writerJava.WriteLine();
					}

					_writerJava.WriteLine(_writerDa.WriteLine("\t\")\""));
					_writerDa.WriteLine(");");
				}
				_writerDa.WriteLine();
				_writerJava.WriteLine(";");
			}
			_writer.WriteLine("catch (Exception ex)");
			using (_writer.Indent())
			{
				_writer.WriteLine("Logger.Write(SystemPID." + _prog.Identification.System + ", ex);");
				_writer.WriteLine();
				_writer.WriteLine("if (ex.Message.IndexOf(\"contains a key value that must be unique but is already present in a row\") > -1)");
				_writer.WriteLine("{");
				_writer.WriteLine("\tSqlcodex.Set(-8227);");
				_writer.WriteLine("}");
				_writer.WriteLine("else");
				_writer.WriteLine("{");
				_writer.WriteLine("\tSqlcodex.Set(-1);");
				_writer.WriteLine("}");
			}
			_writer.WriteLine();

			_writerJava.WriteLine();
			_writerJava.WriteLine("executeNonQuery(sql.toString().replace(\"[=defineOf]\", \"=\"), xml);");

			_writerJava.Unindent();
			_writerJava.WriteLine();

			Count++;
		}

		public void Abort(RollbackTrans s)
		{
			if (BatchTransactions)
			{
				_writer.WriteLine("DataAccess.RollbackTrans();");
			}
			else
			{
				_writer.WriteLine("//");
				_writer.WriteLine("///TODO: ABORT-TRANSACTION");
				_writer.WriteLine("//");
			}
		}

		public void Set
		(
			Update s,
			bool parameterize
		)
		{
			string sqlName = "Sql" + Count.ToString();
			StringBuilder buf = new StringBuilder();
			foreach (var t in s.Tables)
			{
				buf.Append(CsFieldNameConverter.Convert(t.TableName));
			}
			buf.Append("Upd");
			buf.Append(Count);
			sqlName = buf.ToString();

			UnitTests.AddCallMap(sqlName, s);

			List<CondParam> prms = s.GetParameters();
			WriteMethodSig(sqlName, prms, "int", false, s);
			_writerI.Write(";");
			_writerI.WriteLine();
			_writerI.WriteLine();

			_writerDa.WriteIndent();
			_writerDa.Write("//");
			_writerJava.IndentManual();
			WriteJavaArgUnpack(prms);
			_writerDa.WriteLine();

			_writerJDa.IndentManual();
			_writerCDa.IndentManual();
			WriteJavaDaPrmBuild(prms, "update", sqlName);
			_writerJDa.WriteLine("return Execute(\"et." + DataLayerName + "\", prm.ToArray(), null);");
			_writerCDa.WriteLine("return ExecuteNonQuery(\"" + sqlName + "\", prm.ToList());");
			_writerJDa.Unindent();
			_writerCDa.Unindent();
			_writerJDa.WriteLine();
			_writerCDa.WriteLine();

			_writer.WriteLine("try");
			using (_writer.Indent())
			{
				using (_writerDa.Indent())
				{
					_writer.WriteLine("Sqlca.Rows = DataAccess." + sqlName);
					_writer.WriteLine("(");
					WriteDaCallParameters(prms);
					_writer.WriteLine(");");

					_writerJava.WriteLine("String sql = ");

					_writerDa.WriteLine("return Connection.ExecuteNonQuery");
					_writerDa.WriteLine("(");

					_writerJava.WriteLine(_writerDa.WriteLine("\t\"UPDATE " + s.Tables[0].ToString() + "\" +"));

					_writerJava.WriteLine(_writerDa.WriteLine("\t\"SET \" +"));

					for (int x = 0; x < s.Sets.Count; x++)
					{
						_writerDa.WriteIndent();
						_writerJava.WriteIndent();

						_writerJava.Write(_writerDa.Write("\t\t\""));
						if (x > 0)
						{
							_writerJava.Write(_writerDa.Write(", "));
						}
						_writerJava.Write(_writerDa.Write(s.Sets[x].Field.ToString()));
						_writerJava.Write(_writerDa.Write(" = "));
						Write(s.Sets[x].Value, parameterize);
						_writerJava.Write(_writerDa.Write("\" +"));
						_writerDa.WriteLine();
						_writerJava.WriteLine();
					}

					_writerJava.WriteLine(_writerDa.WriteLine("\t\"WHERE \" +"));

					_writerDa.WriteIndent();
					_writerJava.WriteIndent();

					_writerJava.Write(_writerDa.Write("\t\t\""));
					Write(s.Where, parameterize);
					_writerJava.Write(_writerDa.Write("\""));
					if (!String.IsNullOrWhiteSpace(s.ForThisAccess))
					{
						_writerJava.Write(_writerDa.Write(" +"));
					}
					_writerDa.WriteLine();
					_writerJava.WriteLine();

					if (!String.IsNullOrWhiteSpace(s.ForThisAccess))
					{
						_writerDa.WriteIndent();
						_writerJava.WriteIndent();
						_writerJava.Write(_writerDa.Write("\t\"FOR " + s.ForThisAccess + " ACCESS\""));
						_writerDa.WriteLine();
						_writerJava.WriteLine();
					}
					_writerDa.WriteLine(");");
					_writerJava.WriteLine(";");
				}
				_writerDa.WriteLine();
				_writerJava.WriteLine();
			}

			_writer.WriteLine("catch (Exception ex)");
			using (_writer.Indent())
			{
				_writer.WriteLine("Logger.Write(SystemPID." + _prog.Identification.System + ", ex);");
				_writer.WriteLine("Sqlcodex.Set(-1);");
			}
			_writer.WriteLine();

			_writerJava.WriteLine("executeNonQuery(sql.toString().replace(\"[=defineOf]\", \"=\"), xml);");

			_writerJava.Unindent();
			_writerJava.WriteLine();

			Count++;
		}

		public void CloseCursor(Close s)
		{
			string cnamem = CsFieldNameConverter.Convert(s.CursorName.ToString());
			_writer.WriteLine("if (" + cnamem + " != null)");
			using (_writer.Indent())
			{
				_writer.WriteLine(cnamem + ".Close();");
			}
			_writer.WriteLine("Sqlcodex.Set(0);");
		}

		public void Dispose()
		{
			WriteJavaLoad();

			_writerI.Unindent();
			_writerI.Unindent();
			_writerI.Close();
			_writerI.Dispose();

			_writerDa.Unindent();
			_writerDa.Unindent();
			_writerDa.Close();
			_writerDa.Dispose();

			_writerJava.Unindent();
			_writerJava.Close();
			_writerJava.Dispose();

			_writerJDa.Unindent();
			_writerJDa.Unindent();
			_writerJDa.Close();
			_writerJDa.Dispose();

			_writerCDa.Unindent();
			_writerCDa.Unindent();
			_writerCDa.Close();
			_writerCDa.Dispose();
		}

		private void WriteJavaHeader()
		{
			_writerJava.WriteLine("//!!! Do NOT edit in Visual Studio -- Tandem doesn't like DOS line endings");
			_writerJava.WriteLine();
			_writerJava.WriteLine("// nice javac et/" + DataLayerName + ".java -d ../classes -classpath lib/daDefs.jar:lib/tdmext.jar:lib/tmf.jar:lib/smtp.jar:lib/mailapi.jar:lib/imap.jar:lib/activation.jar:lib/jdom.jar:lib/jdbcMx.jar:lib/sqlmp.jar:lib/sqlj.jar:lib/servlet-api.jar:.");
			_writerJava.WriteLine("// Servlet restart macro (GUARDIAN): web t");
			_writerJava.WriteLine("// Windows JAR's at Q:\\TandemDorJavaClasses\\TestClasses");
			_writerJava.WriteLine();
			
			_writerJava.WriteLine("package et;");
			_writerJava.WriteLine();

			_writerJava.WriteLine("import javax.servlet.ServletConfig;");
			_writerJava.WriteLine("import javax.servlet.ServletException;");
			_writerJava.WriteLine("import javax.servlet.http.HttpServletRequest;");
			_writerJava.WriteLine("import javax.servlet.http.HttpServletResponse;");
			_writerJava.WriteLine("import javax.servlet.http.HttpServlet;");
			_writerJava.WriteLine("import javax.servlet.ServletOutputStream;");
			_writerJava.WriteLine();
			_writerJava.WriteLine("import java.util.*;");
			_writerJava.WriteLine("import java.text.SimpleDateFormat;");
			_writerJava.WriteLine("import java.io.IOException;");
			_writerJava.WriteLine("import java.sql.*;");
			_writerJava.WriteLine("import java.lang.reflect.*;");
			_writerJava.WriteLine();
			_writerJava.WriteLine("import com.tandem.ext.util.DataConversion;");
			_writerJava.WriteLine("import com.tandem.tsmp.*;");
			_writerJava.WriteLine("import com.tandem.tmf.Current;");
			_writerJava.WriteLine();
			_writerJava.WriteLine("import gov.revenue.*;");
			_writerJava.WriteLine("import da.taxlnote;");
			_writerJava.WriteLine();

			_writerJava.WriteLine("public class " + DataLayerName + " extends SqlServletBaseClass");
			_writerJava.IndentManual();

			_writerJava.WriteLine("public static void main(String[] cmdArgs)");
			using (_writerJava.Indent())
			{
				_writerJava.WriteLine("HashMap args = new HashMap();");
				_writerJava.WriteLine("if (cmdArgs.length > 1)");
				using (_writerJava.Indent())
				{
					_writerJava.WriteLine("args.put(\"name\", cmdArgs[0]);");
					_writerJava.WriteLine();
					_writerJava.WriteLine("for (int x = 1; x < cmdArgs.length; x++)");
					_writerJava.WriteLine("{");
					_writerJava.WriteLine("	parseKeyValuePair(cmdArgs[x], args);");
					_writerJava.WriteLine("}");
				}
				_writerJava.WriteLine("else");
				using (_writerJava.Indent())
				{
					_writerJava.WriteLine("args.put(\"name\", \"\");");
				}
				_writerJava.WriteLine();
				_writerJava.WriteLine(DataLayerName + " instance = new " + DataLayerName + "();");
				_writerJava.WriteLine("mainInner(instance, args);");
			}
			_writerJava.WriteLine();

			_writerJava.WriteLine("protected String outerTagName()");
			_writerJava.WriteLine("{");
			_writerJava.WriteLine("	return \"ROWS\";");
			_writerJava.WriteLine("}");
			_writerJava.WriteLine();
			_writerJava.WriteLine("protected String description()");
			_writerJava.WriteLine("{");
			_writerJava.WriteLine("	return \"" + _prog.ProgramId + "\";");
			_writerJava.WriteLine("}");
			_writerJava.WriteLine();
			_writerJava.WriteLine("protected HashMap createDefaultArguments()");
			_writerJava.WriteLine("throws Exception");
			_writerJava.WriteLine("{");
			_writerJava.WriteLine("	HashMap args = new HashMap();");
			_writerJava.WriteLine("	args.put(\"name\", null);");
			_writerJava.WriteLine("	args.put(\"returnMetaData\", null);");
			_writerJava.WriteLine("	return args;");
			_writerJava.WriteLine("}");
			_writerJava.WriteLine();
			_writerJava.WriteLine("protected boolean validateArguments(HashMap args)");
			_writerJava.WriteLine("throws Exception");
			_writerJava.WriteLine("{");
			_writerJava.WriteLine("	return args.get(\"name\") != null;");
			_writerJava.WriteLine("}");
			_writerJava.WriteLine();
		}

		private void WriteJavaLoad()
		{
			_writerJava.WriteLine("protected void load");
			_writerJava.WriteLine("(");
			_writerJava.WriteLine("	String action, ");
			_writerJava.WriteLine("	HashMap args, ");
			_writerJava.WriteLine("	StringBuffer xml");
			_writerJava.WriteLine(")");
			_writerJava.WriteLine("throws ");
			_writerJava.WriteLine("	SQLException,");
			_writerJava.WriteLine("	ServletException, ");
			_writerJava.WriteLine("	IOException, ");
			_writerJava.WriteLine("	com.tandem.ext.util.DataConversionException, ");
			_writerJava.WriteLine("	com.tandem.tsmp.TsmpSendException, ");
			_writerJava.WriteLine("	com.tandem.tsmp.TsmpServerUnavailableException");
			using (_writerJava.Indent())
			{
				_writerJava.WriteLine("String name = (String)args.get(\"name\");");

				if (UnitTests != null)
				{
					_writerJava.WriteLine();

					string iftxt = "if";
					foreach (var s in UnitTests.Maps)
					{
						_writerJava.WriteLine(iftxt + " (name.equals(\"" + s.Left + "\"))");
						iftxt = "else if";
						using (_writerJava.Indent())
						{
							_writerJava.WriteLine(s.Left + "(xml, args);");
						}
					}
				}

				_writerJava.WriteLine("else");
				using (_writerJava.Indent())
				{
					_writerJava.WriteLine("xml.append(\"<error>Unknown name argument</error>\");");
				}
			}
		}

		private void WriteJavaArgUnpack(List<CondParam> prms)
		{
			Dictionary<string, string> idx = new Dictionary<string, string>();

			foreach (var p in prms)
			{
				string idxName = p.RecordName + "." + p.FieldName;
				if (idx.ContainsKey(idxName))
				{
					continue;
				}
				idx.Add(idxName, idxName);

				INamedField f = Find(_prog, p);
				bool unquote = false;

				if (f.Attributes.Pic != null && f.Attributes.Pic.PicFormat.IsNumeric)
				{
					unquote = true;
				}

				_writerJava.WriteIndent();
				_writerJava.Write("String ");
				Write(p, false, false, false, true);
				_writerJava.Write(" = ");

				if (unquote)
				{
					_writerJava.Write("Unquote(");
				}
				else
				{
					_writerJava.Write("EnsureQuotes(");
				}

				_writerJava.Write("args.get(\"");
				Write(p, false, false, false, true);
				_writerJava.Write("\"");
				_writerJava.Write(")");
				_writerJava.Write(");");
				_writerJava.WriteLine();
			}

			if (prms.Count > 0)
			{
				_writerJava.WriteLine();
			}
		}
	}
}
