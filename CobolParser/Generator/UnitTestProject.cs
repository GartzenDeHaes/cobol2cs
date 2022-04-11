using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using CobolParser.Text;
using CobolParser.Verbs;
using CobolParser.SQL.Statements;
using CobolParser.SQL;
using DOR.Core.Collections;
using CobolParser.SQL.Conds;

namespace CobolParser.Generator
{
	public class UnitTestProject : IDisposable
	{
		public ISourceWriter ModDataAccessWriter
		{
			get;
			private set;
		}

		public ISourceWriter TestWriter
		{
			get;
			private set;
		}

		private List<Association<string, SqlStatement>> _maps = new List<Association<string, SqlStatement>>();
		public List<Association<string, SqlStatement>> Maps
		{
			get { return _maps; }
		}

		private CsVerbs _cvsx;

		public UnitTestProject(CobolProgram prog, CsVerbs cvsx)
		{
			_cvsx = cvsx;
			string baseName = CsFieldNameConverter.Convert(prog.ProgramId);
			string name = baseName + "Test";

			if (!Directory.Exists(name))
			{
				Directory.CreateDirectory(name);
			}

			string csprojName = Path.Combine(name, name + ".csproj");
			string csnameModData = Path.Combine(name, "ModDataAccess.cs");
			string csnameTest = Path.Combine(name, name + ".cs");

			if (!File.Exists(csprojName))
			{
				CsProjFile csproj = new CsProjFile(prog.Namespace, true);

				csproj.CompileFiles.Add(Path.GetFileName(csnameModData));
				csproj.CompileFiles.Add(Path.GetFileName(csnameTest));
				csproj.TargetFrameworkProfile = "";
				csproj.SystemReferences.Items.Add("System.Web");
				csproj.ProjectReferences.Add("..\\..\\..\\..\\SharedSources\\DOR.Core\\DOR.Core\\DOR.Core.csproj");
				csproj.ProjectReferences.Add("..\\..\\..\\..\\SharedSources\\DOR.WorkingStorage\\DOR.WorkingStorage\\DOR.WorkingStorage.csproj");
				csproj.ProjectReferences.Add("C:\\sources\\Core Tax Systems\\Cobol\\CopyLibs\\CopyLibs.csproj");
				csproj.ProjectReferences.Add("..\\" + baseName + "\\" + baseName + ".csproj");

				File.WriteAllText(csprojName, csproj.ToXml());
			}

			if (File.Exists(csnameModData))
			{
				File.Delete(csnameModData);
			}

			if (File.Exists(csnameTest))
			{
				File.Delete(csnameTest);
			}

			ModDataAccessWriter = new SourceWriter(csnameModData);
			TestWriter = new SourceWriter(csnameTest);

			ModDataAccessWriter.WriteUsing("System");
			ModDataAccessWriter.WriteUsing("System.Data");
			ModDataAccessWriter.WriteUsing("System.Collections.Generic");
			ModDataAccessWriter.WriteUsing("System.Linq");
			ModDataAccessWriter.WriteUsing("System.Text");
			ModDataAccessWriter.WriteLine();
			ModDataAccessWriter.WriteUsing("DOR.Core.Data");
			ModDataAccessWriter.WriteUsing("DOR.WorkingStorage");
			ModDataAccessWriter.WriteUsing(baseName);
			ModDataAccessWriter.WriteLine();

			ModDataAccessWriter.WriteLine("namespace " + name);
			ModDataAccessWriter.IndentManual();
			ModDataAccessWriter.WriteLine("public class MockDataAccess : MockDataAccessBase, I" + baseName + "Data");
			ModDataAccessWriter.IndentManual();

			TestWriter.WriteUsing("System");
			TestWriter.WriteUsing("Microsoft.VisualStudio.TestTools.UnitTesting");
			TestWriter.WriteLine();
			TestWriter.WriteUsing("DOR.Core.Data");
			TestWriter.WriteUsing("DOR.Core.Logging");
			TestWriter.WriteUsing("DOR.WorkingStorage");
			TestWriter.WriteUsing(baseName);
			TestWriter.WriteUsing("Clibal");
			TestWriter.WriteLine();

			TestWriter.WriteLine("namespace " + name);
			TestWriter.IndentManual();
			TestWriter.WriteLine("[TestClass()]");
			TestWriter.WriteLine("public class " + baseName + "Test");
			TestWriter.IndentManual();

			TestWriter.WriteLine("private TestContext testContextInstance;");
			TestWriter.WriteLine("");
			TestWriter.WriteLine("public TestContext TestContext");
			TestWriter.WriteLine("{");
			TestWriter.WriteLine("	get");
			TestWriter.WriteLine("	{");
			TestWriter.WriteLine("		return testContextInstance;");
			TestWriter.WriteLine("	}");
			TestWriter.WriteLine("	set");
			TestWriter.WriteLine("	{");
			TestWriter.WriteLine("		testContextInstance = value;");
			TestWriter.WriteLine("	}");
			TestWriter.WriteLine("}");
			TestWriter.WriteLine("");
			TestWriter.WriteLine("#region Additional test attributes");
			TestWriter.WriteLine("// ");
			TestWriter.WriteLine("//You can use the following additional attributes as you write your tests:");
			TestWriter.WriteLine("//");
			TestWriter.WriteLine("//Use ClassInitialize to run code before running the first test in the class");
			TestWriter.WriteLine("//[ClassInitialize()]");
			TestWriter.WriteLine("//public static void MyClassInitialize(TestContext testContext)");
			TestWriter.WriteLine("//{");
			TestWriter.WriteLine("//}");
			TestWriter.WriteLine("//");
			TestWriter.WriteLine("//Use ClassCleanup to run code after all tests in a class have run");
			TestWriter.WriteLine("//[ClassCleanup()]");
			TestWriter.WriteLine("//public static void MyClassCleanup()");
			TestWriter.WriteLine("//{");
			TestWriter.WriteLine("//}");
			TestWriter.WriteLine("//");
			TestWriter.WriteLine("//Use TestInitialize to run code before running each test");
			TestWriter.WriteLine("//[TestInitialize()]");
			TestWriter.WriteLine("//public void MyTestInitialize()");
			TestWriter.WriteLine("//{");
			TestWriter.WriteLine("//}");
			TestWriter.WriteLine("//");
			TestWriter.WriteLine("//Use TestCleanup to run code after each test has run");
			TestWriter.WriteLine("//[TestCleanup()]");
			TestWriter.WriteLine("//public void MyTestCleanup()");
			TestWriter.WriteLine("//{");
			TestWriter.WriteLine("//}");
			TestWriter.WriteLine("//");
			TestWriter.WriteLine("#endregion");
			TestWriter.WriteLine();

			TestWriter.WriteLine("[TestMethod()]");
			TestWriter.WriteLine("public void MainTest()");
			using (TestWriter.Indent())
			{
				TestWriter.WriteLine("MockDataAccess da = new MockDataAccess();");
				TestWriter.WriteLine(baseName + "Program srv = new " + baseName + "Program();");

				Fd fdReceive = null;

				foreach (var fd in prog.Data.Files.Fds())
				{
					if (fd.MessageName.EndsWith("-IN"))
					{
						fdReceive = fd;
						break;
					}
				}
				TestWriter.WriteLine("var msg = new " + CsFieldNameConverter.Convert(fdReceive.DataRecords[0].Name, fdReceive.DataRecords[0].SpecialNameHandling) + "();");
				TestWriter.WriteLine();
				TestWriter.WriteLine("///TODO: setup message");
				TestWriter.WriteLine("msg.MsgCode.Set(100);");
				TestWriter.WriteLine();
				TestWriter.WriteLine("NullLogger log = new NullLogger();");
				TestWriter.WriteLine("IBufferOffset buf = srv.Main(msg, log, da);");
				TestWriter.WriteLine("Assert.AreEqual(\"\", log.Messages.ToString());");
				TestWriter.WriteLine();

				Fd replyFd = prog.Data.Files.FindFdFor(prog.Environment.InputOutputSection.FileControlSection.ReplyMessageName());
				string reply = CsFieldNameConverter.Convert(replyFd.DataRecords[0].Name, replyFd.DataRecords[0].SpecialNameHandling);

				TestWriter.WriteLine(reply + " reply = new " + reply + "();");
				TestWriter.WriteLine("reply.Set(buf);");
				TestWriter.WriteLine();
				TestWriter.WriteLine("///TODO: do something intelligent");
				TestWriter.WriteLine("Assert.AreEqual(\"0000\", reply.ReplyCode.ToString());");
			}
		}

		public void AddCallMap(string method, SqlStatement stmt)
		{
			_maps.Add(new Association<string, SqlStatement>(method, stmt));
		}

		private void WriteAargMaps
		(
			List<CondParam> args, 
			bool writeLeadingComma,
			bool writeIBufOff,
			bool derefValues
		)
		{
			if (args.Count == 0)
			{
				return;
			}

			SourceWriterBuffer bufWriter = new SourceWriterBuffer();

			Dictionary<string, string> _idx = new Dictionary<string, string>();
			int count = 0;

			foreach (var p in args)
			{
				ISourceWriter back = _cvsx.Writer;
				_cvsx.Writer = bufWriter;
				_cvsx.GetFieldReference(p.FieldName, p.RecordName);
				_cvsx.Writer = back;

				string pname = bufWriter.ToString().Replace(".", "");

				if (_idx.ContainsKey(pname))
				{
					continue;
				}
				_idx.Add(pname, pname);

				ModDataAccessWriter.WriteIndent();
				if (count > 0 || writeLeadingComma)
				{
					ModDataAccessWriter.Write("\t, ");
				}
				else
				{
					ModDataAccessWriter.Write("\t");
				}

				if (writeIBufOff)
				{
					ModDataAccessWriter.Write("IBufferOffset " + pname);
				}
				else if (derefValues)
				{
					ModDataAccessWriter.Write("new ArgumentMap(\"" + pname + "\", " + pname + ".ToString())");
				}
				else
				{
					ModDataAccessWriter.Write("new ArgumentMap(\"" + pname + "\", \"0\")");
				}
				ModDataAccessWriter.WriteLine();

				count++;
			}
		}

		private void WriteMockDataAdd(Select s)
		{
			ModDataAccessWriter.WriteLine("\t\"<ROWS>\\r\\n\" +");
			ModDataAccessWriter.WriteLine("\t\t\"<row \" +");

			for (int x = 0; x < s.Fields.Count; x++)
			{
				var f = s.Fields[x];
				string name;

				if (f.Terms.Count == 1)
				{
					name = f.Terms[0].ToString();
				}
				else
				{
					name = "Field" + x;
				}

				ModDataAccessWriter.WriteLine("\t\t\"" + name + "=\\\"0\\\" \\r\\n\" +");
			}

			ModDataAccessWriter.WriteLine("\t\"/></ROWS>\\r\\n\"");
			WriteAargMaps(s.GetParameters(), true, false, false);
		}

		private void WriteMockDataAdd(Insert s)
		{
			ModDataAccessWriter.WriteLine("\t\"<ROWS count=\\\"1\\\">\\r\\n\" +");
			ModDataAccessWriter.WriteLine("\t\"/></ROWS>\\r\\n\"");
			WriteAargMaps(s.GetParameters(), true, false, false);
		}

		private void WriteMockDataAdd(Update s)
		{
			ModDataAccessWriter.WriteLine("\t\"<ROWS count=\\\"1\\\">\\r\\n\" +");
			ModDataAccessWriter.WriteLine("\t\"/></ROWS>\\r\\n\"");
			WriteAargMaps(s.GetParameters(), true, false, false);
		}

		private void WriteMockDataAdd(CobolParser.SQL.Statements.Delete s)
		{
			ModDataAccessWriter.WriteLine("\t\"<ROWS count=\\\"1\\\">\\r\\n\" +");
			ModDataAccessWriter.WriteLine("\t\"/></ROWS>\\r\\n\"");
			WriteAargMaps(s.GetParameters(), true, false, false);
		}

		public void WriteDataMocks()
		{
			Dictionary<string, string> _idx = new Dictionary<string, string>();

			ModDataAccessWriter.WriteLine("public MockDataAccess()");

			using (ModDataAccessWriter.Indent())
			{
				ModDataAccessWriter.WriteLine("///TODO: replace these generic stubs with real test data");
				ModDataAccessWriter.WriteLine();

				foreach (var a in _maps)
				{
					if (_idx.ContainsKey(a.Left))
					{
						continue;
					}
					_idx.Add(a.Left, a.Left);

					ModDataAccessWriter.WriteLine("Add");
					ModDataAccessWriter.WriteLine("(");
					ModDataAccessWriter.WriteLine("\t\"" + a.Left + "\",");
					ModDataAccessWriter.WriteLine("\t\"<?xml version=\\\"1.0\\\" encoding=\\\"utf-8\\\"?>\\r\\n\" +");
					if (a.Right is Select)
					{
						WriteMockDataAdd((Select)a.Right);
					}
					else if (a.Right is Insert)
					{
						WriteMockDataAdd((Insert)a.Right);
					}
					else if (a.Right is Update)
					{
						WriteMockDataAdd((Update)a.Right);
					}
					else if (a.Right is CobolParser.SQL.Statements.Delete)
					{
						WriteMockDataAdd((CobolParser.SQL.Statements.Delete)a.Right);
					}
					else
					{
						throw new Exception("Internal error");
					}
					ModDataAccessWriter.WriteLine(");");
					ModDataAccessWriter.WriteLine();
				}
			}

			ModDataAccessWriter.WriteLine();

			foreach (var a in _maps)
			{
				if (a.Right is Select)
				{
					ModDataAccessWriter.WriteLine("public IDataReaderEx " + a.Left);
				}
				else
				{
					ModDataAccessWriter.WriteLine("public int " + a.Left);
				}
				ModDataAccessWriter.WriteLine("(");

				List<CondParam> lst = a.Right.GetParameters();
				WriteAargMaps(lst, false, true, false);

				ModDataAccessWriter.WriteLine(")");
				
				using (ModDataAccessWriter.Indent())
				{
					if (a.Right is Select)
					{
						ModDataAccessWriter.WriteLine("return Dispatch");
					}
					else
					{
						ModDataAccessWriter.WriteLine("return DispatchNonQuery");
					}
					ModDataAccessWriter.WriteLine("(");

					ModDataAccessWriter.WriteLine("\t\"" + a.Left + "\"");

					WriteAargMaps(lst, true, false, true);

					ModDataAccessWriter.WriteLine(");");
				}

				ModDataAccessWriter.WriteLine();
			}
		}

		public void Dispose()
		{
			ModDataAccessWriter.WriteLine("public void Dispose()");
			ModDataAccessWriter.WriteLine("{");
			ModDataAccessWriter.WriteLine("}");

			ModDataAccessWriter.Unindent();
			ModDataAccessWriter.Unindent();
			ModDataAccessWriter.Close();
			ModDataAccessWriter.Dispose();

			TestWriter.Unindent();
			TestWriter.Unindent();
			TestWriter.Close();
			TestWriter.Dispose();
		}
	}
}
