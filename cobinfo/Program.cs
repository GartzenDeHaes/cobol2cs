using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using DOR.Core;
using DOR.Core.Config;
using DOR.Core.Collections;
using DOR.Core.Data.Tandem;

using CobolParser;
using CobolParser.Verbs;
using CobolParser.Records;
using CobolParser.TandemData;
using CobolParser.Text;
using CobolParser.Parser;

namespace cobinfo
{
	class Program
	{
		static void Main(string[] args)
		{
			ProgramArguments pa = new ProgramArguments(args);

			if (pa.HasAnyOfSwitches("?|help", '|'))
			{
				Console.WriteLine("cobinfo [--root=c:\\temp\\]");
				return;
			}

			if (pa.HasSwitch("root"))
			{
				ImportManager.BaseDirectory = pa["root"];
			}
			else
			{
				ImportManager.BaseDirectory = "c:\\temp\\COBOL\\";
			}

			string[] sourceFiles;

			if (pa.Parameters.Length == 1)
			{
				sourceFiles = new string[1];
				sourceFiles[0] = Path.Combine(Path.Combine(ImportManager.BaseDirectory, "d08.source"), pa.Parameters[0]);
			}
			else
			{
				sourceFiles = Directory.GetFiles
				(
					Path.Combine(ImportManager.BaseDirectory, "d08.source")
				);
			}

			string[] ddlFiles = Directory.GetFiles
			(
				Path.Combine(ImportManager.BaseDirectory, "d23.revprod")
			);

			foreach (string dfile in ddlFiles)
			{
				string f = Path.GetFileName(dfile).ToUpper();
				ImportManager.AddDefine(new GuardianDefine("=" + f), new GuardianPath("$d23", "revprod", f));
			}

			ImportManager.AddDorNonDatabaseDefines();

			//try
			//{
			if (pa.HasSwitch("ddl"))
			{
				LoadDdl2();
				return;
			}
			else if (pa.HasSwitch("tables"))
			{
				LoadDdl2(false);
				WriteTables();
				return;
			}
			else if (pa.HasSwitch("screenddlxref"))
			{
				ScreenFieldXref();
				return;
			}

			// load all source files

			int count = 0;
			int lineCount = 0;
			Vector<CobolProgram> progs = new Vector<CobolProgram>(2200);
			Dictionary<string, GuardianPath> progPaths = new Dictionary<string, GuardianPath>();

			foreach (string sourceFile in sourceFiles)
			{
				count++;

				GuardianPath gpath = ImportManager.GuardianPathFromWindows(sourceFile);

				Console.WriteLine("Parsing {0}", gpath.ToString());

				if (!File.Exists(sourceFile))
				{
					Console.WriteLine("{0} not found", sourceFile);
					return;
				}

				CobolProgram prog;

				try
				{
					prog = new CobolProgram(gpath);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
					continue;
				}

				progs.Add(prog);
				if (progPaths.ContainsKey(prog.Identification.ProgramId))
				{
					// assume progs with higher file numbers are better
					progPaths[prog.Identification.ProgramId] = prog.FileName;
				}
				else
				{
					progPaths.Add(prog.Identification.ProgramId, prog.FileName);
				}

				lineCount += prog.LineCount;

				Console.WriteLine("Parsed {0} {1} of {2} ({3:#,##0} lines)", prog.ProgramId, count, sourceFiles.Length, lineCount + ImportManager.LineCount);
			}

			if (pa.HasSwitch("files"))
			{
				// CobFile table
				using (var data = new CobEntities())
				{
					Console.WriteLine("Writing copy books");

					foreach (Terminalize t in ImportManager.CopyBooks)
					{
						CobFile f = new CobFile();

						f.File_Author_Name = "";
						f.File_Header_Txt = "";
						f.File_Loc_Num = t.LineCount;
						f.File_Name = t.FileName.ToString();
						f.File_Prog_Name = "";
						f.File_Written_Dt = "";
						f.File_Type_Code = "C";

						data.CobFiles.AddObject(f);
						data.SaveChanges();
					}

					Console.WriteLine("Writing program files");

					foreach (CobolProgram prog in progs)
					{
						CobFile f = new CobFile();

						f.File_Author_Name = prog.Author;
						f.File_Header_Txt = prog.FileHeaderComments;
						f.File_Loc_Num = prog.LineCount;
						f.File_Name = prog.FileName.ToString();
						f.File_Prog_Name = prog.ProgramId;
						f.File_Written_Dt = prog.WrittenOn;

						if (prog.FileName.File[0] == 'R')
						{
							f.File_Type_Code = "R";
						}
						else if (prog.FileName.File[0] == 'B')
						{
							f.File_Type_Code = "B";
						}
						else if (prog.FileName.File[0] == 'S')
						{
							f.File_Type_Code = "S";
						}
						else
						{
							f.File_Type_Code = "?";
						}

						data.CobFiles.AddObject(f);
						data.SaveChanges();
					}
				}
			}

			if (pa.HasSwitch("copybookxref"))
			{
				// CobFileXRefs

				using (var data = new CobEntities())
				{
					Console.WriteLine("Writing copy book xref");

					Dictionary<GuardianPath, GuardianPath> idx = new Dictionary<GuardianPath, GuardianPath>();

					foreach (CobolProgram prog in progs)
					{
						idx.Clear();

						foreach (GuardianPath file in ImportManager.ProgramCopyBooks(prog.FileName))
						{
							if (idx.ContainsKey(file))
							{
								continue;
							}
							idx.Add(file, file);

							CobFileXRef x = new CobFileXRef();
							x.Xref_File_Name = prog.FileName.ToString();
							x.Xref_DependsOnFile_Name = file.ToString();
							x.Xref_Type_Code = "C";

							data.CobFileXRefs.AddObject(x);
							data.SaveChanges();
						}
					}
				}
			}

			if (pa.HasSwitch("searchxref"))
			{
				using (var data = new CobEntities())
				{
					Console.WriteLine("Writing search xref");

					foreach (CobolProgram prog in progs)
					{
						foreach (GuardianPath file in prog.SearchFiles)
						{
							CobFileXRef x = new CobFileXRef();
							x.Xref_File_Name = prog.FileName.ToString();
							x.Xref_DependsOnFile_Name = file.ToString();
							x.Xref_Type_Code = "L";

							data.CobFileXRefs.AddObject(x);
							data.SaveChanges();
						}
					}
				}
			}

			if (pa.HasSwitch("listtables"))
			{
				ListTables(progs);
			}

			if (pa.HasSwitch("pathway"))
			{
				using (var data = new CobEntities())
				{
					Console.WriteLine("Writing pathway xref");
					Stack<CobolProgram> stk = new Stack<CobolProgram>();
					Dictionary<string, CobolProgram> pidx = new Dictionary<string, CobolProgram>();

					foreach (CobolProgram tprog in progs)
					{
						if (!pidx.ContainsKey(tprog.Identification.ProgramId))
						{
							pidx.Add(tprog.Identification.ProgramId, tprog);
						}
					}

					foreach (CobolProgram tprog in progs)
					{
						Dictionary<GuardianPath, string> wroteIdx = new Dictionary<GuardianPath, string>();
						stk.Clear();
						stk.Push(tprog);

						while (stk.Count > 0)
						{
							CobolProgram prog = stk.Pop();
							if (wroteIdx.ContainsKey(prog.FileName))
							{
								continue;
							}
							wroteIdx.Add(prog.FileName, "");

							Dictionary<string, Send> pdict = ImportManager.PathwaySends(prog.FileName);

							if (null == pdict)
							{
								continue;
							}

							foreach (Send send in pdict.Values)
							{
								if (!pidx.ContainsKey(send.ServerClassName))
								{
									continue;
								}
								CobolProgram toprog = pidx[send.ServerClassName];
								if (wroteIdx.ContainsKey(toprog.FileName))
								{
									continue;
								}

								stk.Push(toprog);

								CobFileXRef x = new CobFileXRef();
								x.Xref_File_Name = tprog.FileName.ToString();
								x.Xref_DependsOnFile_Name = progPaths[send.ServerClassName].ToString();
								x.Xref_Type_Code = "P";

								data.CobFileXRefs.AddObject(x);
								data.SaveChanges();
							}
						}
					}
				}
			}

			if (pa.HasSwitch("screens"))
			{
				using (var data = new CobEntities())
				{
					Console.WriteLine("Writing screen files");

					foreach (CobolProgram prog in progs)
					{
						if (null == prog.Data.ScreenRecord)
						{
							continue;
						}
						foreach (Screen screen in prog.Data.ScreenRecord.Screens)
						{
							CobScreen sc = new CobScreen();
							sc.File_Name = prog.FileName.ToString();
							sc.Screen_Name = screen.Name;
							sc.Screen_Num = screen.PgmfName;
							sc.Screen_IsOverlay_Ind = screen.IsOverlay;

							data.CobScreens.AddObject(sc);
							data.SaveChanges();
						}
					}
				}
			}

			if (pa.HasSwitch("screendefs"))
			{
				using (var data = new CobEntities())
				{
					Console.WriteLine("Writing screen fields");

					Random rand = new Random();

					foreach (CobolProgram prog in progs)
					{
						if (null == prog.Data.ScreenRecord)
						{
							continue;
						}

						foreach (Screen screen in prog.Data.ScreenRecord.Screens)
						{
							foreach (ScreenField field in screen.Fields)
							{
								if (null == field.ToExpr && null == field.UsingExpr && null == field.FromExpr && null == field.MDT)
								{
									continue;
								}

								CobScreenField f = new CobScreenField();
								f.File_Name = prog.FileName.ToString();
								f.Field_Line_Num = field.LineNumber;
								f.Screen_Name = screen.Name;
								f.Field_Attribs_Txt = field.TextAttributeString();
								f.Field_Fill_Txt = field.FillValue;
								f.Field_From_Txt = ExprToStr(field.FromExpr);
								f.Field_Lvl_Num = field.Level;
								f.Field_Name = field.Name;

								if (f.Field_Name == "FILLER")
								{
									f.Field_Name += " " + rand.Next(9999);
								}

								f.Field_OverlayHieght_Num = field.OverlayHeight;
								f.Field_OverlayWidth_Num = field.OverlayWidth;
								f.Field_RequiredLength_Txt = ExprToStr(field.RequiredLength);
								f.Field_RequiredOptions_Txt = ExprToStr(field.RequiredChoices);
								f.Field_To_Txt = ExprToStr(field.ToExpr);
								f.Field_Upshift_Txt = field.IsUpshift;
								f.Field_Using_Txt = ExprToStr(field.UsingExpr);
								f.Field_WhenAbsent_Txt = field.WhenAbsent;
								f.Field_WhenBlank_Txt = field.WhenBlank;
								f.Field_WhenFull_Txt = field.WhenFull;
								f.Field_X_Num = field.X;
								f.Field_Y_Num = field.Y;

								if (null != field.Pic)
								{
									f.Field_Pic_Txt = field.Pic.PicFormat.PictureClause;
								}

								data.CobScreenFields.AddObject(f);
							}
						}
						data.SaveChanges();
					}
				}
			}
			//}
			//catch (Exception ex)
			//{
			//	Console.WriteLine(ex.ToString());
			//}

			Console.WriteLine();
			Console.WriteLine("Press return to continue ...");
			Console.ReadKey();
		}

		private static string ExprToStr(IExpr expr)
		{
			if (null == expr)
			{
				return null;
			}

			return expr.ToDocumentationString();
		}

		private static void ListTables(Vector<CobolProgram> progs)
		{
			ScreenTables(progs);

			//Console.WriteLine("listing tables");

			//using (var data = new CobEntities())
			//{
			//    Dictionary<string, string> idx = new Dictionary<string, string>();

			//    foreach (CobolProgram prog in progs)
			//    {
			//        List<string> lst = prog.ListTables();

			//        foreach (string s in lst)
			//        {
			//            string ss = s.ToUpper();
			//            if (ss[0] == '=')
			//            {
			//                ss = ss.Substring(1);
			//            }
			//            if (idx.ContainsKey(prog.FileName.ToString() + "." + ss))
			//            {
			//                continue;
			//            }
			//            idx.Add(prog.FileName.ToString() + "." + ss, s);

			//            CobFileXRef x = new CobFileXRef();
			//            x.Xref_File_Name = prog.FileName.ToString();
			//            x.Xref_DependsOnFile_Name = ss;
			//            x.Xref_Type_Code = "T";

			//            data.CobFileXRefs.AddObject(x);
			//            data.SaveChanges();
			//        }
			//    }
			//}
		}

		private static void ScreenFieldXref()
		{
			Console.WriteLine("Writing Screen/DDL xref");

			using (var data = new CobEntities())
			{
				var sfields = (from s in data.CobScreenFields select s);
				foreach (var sf in sfields.ToList())
				{
					if (!String.IsNullOrEmpty(sf.DdlField_Name))
					{
						continue;
					}
					if (null != sf.Field_Using_Txt)
					{
						DdlParse(data, sf.Field_Using_Txt, sf);
					}
					else if (null != sf.Field_To_Txt)
					{
						DdlParse(data, sf.Field_To_Txt, sf);
					}
					else if (null != sf.Field_From_Txt)
					{
						DdlParse(data, sf.Field_From_Txt, sf);
					}
					else
					{
						Debug.WriteLine("Shouldn't happen");
					}
				}
			}
		}

		private static void DdlParse(CobEntities data, string fd, CobScreenField sf)
		{
			string[] parts = fd.Split(new char[] { '.' });
			for (int x = 0; x < parts.Length; x++)
			{
				string s = parts[x];
				if (s.StartsWith("WS-"))
				{
					s = s.Substring(3);
				}
				if (StringHelper.CountOccurancesOf(s, '-') < 2)
				{
					continue;
				}
				var ddl = (from d in data.CobDDLs where d.Field_Name.Contains(s) select d.Field_Name);
				if (ddl.Any())
				{
					if (ddl.Count() > 2)
					{
						continue;
					}
					Console.WriteLine("{0} == {1}", parts[x], ddl.First());
					foreach (string cd in ddl)
					{
						Console.WriteLine("\t{0}", cd);
					}
					sf.DdlField_Name = ddl.First();
					data.SaveChanges();
					break;
				}
			}
		}

		private static string FindProbablySystemName(IList<ColumnDef> cols)
		{
			Dictionary<string, int> idx = new Dictionary<string, int>();
			foreach (var c in cols)
			{
				if (c.Name.Equals("Rec-Update-Num", StringComparison.InvariantCultureIgnoreCase))
				{
					continue;
				}
				INamedField o = ImportManager.FindDdlDef(c.Name.Replace("_", "-"));
				if (o == null)
				{
					continue;
				}
				string sys = StringHelper.RightStr(o.FileName.File, 2).ToUpper();
				if (!idx.ContainsKey(sys))
				{
					idx.Add(sys, 0);
				}
				idx[sys] += 1;
			}

			if (idx.Count == 0)
			{
				return "";
			}
			if (idx.Count == 1 && idx.First().Key == "DA")
			{
				return "";
			}
			var sortedDict = (from i in idx orderby i.Value descending select i);
			return sortedDict.First().Key;
		}

		private static void WriteJavaColsEq(IList<ColumnDef> cols, SourceWriter writer)
		{
			for (int x = 0; x < cols.Count; x++)
			{
				ColumnDef c = cols[x];
				writer.WriteLine("if (args.get(\"" + c.Name + "\") != null)");
				using (writer.Indent())
				{
					writer.WriteLine("if (writeAnd)");
					writer.WriteLine("\tsql.append(\" AND \");");
					writer.WriteLine("else");
					writer.WriteLine("\twriteAnd = true;");
					writer.WriteLine("sql.append(\" " + c.Name + " = \");");
					writer.WriteLine("sql.append(args.get(\"" + c.Name + "\"));");
				}
			}
		}

		private static void WriteJavaWhere(IList<ColumnDef> cols, SourceWriter writer)
		{
			writer.WriteLine("sql.append(\"WHERE \");");
			WriteJavaColsEq(cols, writer);
		}

		private static void WriteFactoryStub()
		{
			string filename = "TandemDataFactory.cs";

			if (File.Exists(filename))
			{
				File.Delete(filename);
			}

			SourceWriter writer = new SourceWriter(new StreamWriter(File.OpenWrite(filename)));
			writer.WriteUsing("System");
			writer.WriteUsing("System.Collections.Generic");
			writer.WriteUsing("System.Linq");
			writer.WriteUsing("System.Text");
			writer.WriteLine();
			writer.WriteUsing("DOR.Core");
			writer.WriteUsing("DOR.Core.Config");
			writer.WriteUsing("DOR.Core.Data.Tandem");
			writer.WriteLine();
			writer.WriteUsing("TandemData.Entities");
			writer.WriteUsing("BusinessRegistration.Model");
			writer.WriteLine();

			writer.WriteLine("namespace TandemData");
			writer.IndentManual();
			writer.WriteLine("public partial class TandemDataFactory : TandemDataAccessBase");
			writer.IndentManual();
			writer.WriteLine("public TandemDataFactory(Configuration config)");
			writer.WriteLine(": base(config)");
			writer.WriteLine("{");
			writer.WriteLine("\tServletDirectoryName = \"servlet-et\";");
			writer.WriteLine("}");
			writer.WriteLine();
			writer.WriteLine("public TandemDataFactory()");
			writer.WriteLine(": this(Configuration.AppConfig)");
			writer.WriteLine("{");
			writer.WriteLine("}");
			writer.WriteLine();
			writer.Close();
		}

		private static List<string> _daFiles = new List<string>();

		private static void WriteFactoryStubClose()
		{
			string filename = "TandemDataFactory.cs";
			SourceWriter writer = new SourceWriter(File.AppendText(filename));
			writer.IndentLevel = 2;
			writer.Unindent();
			writer.Unindent();
			writer.Close();

			foreach (var fn in _daFiles)
			{
				writer = new SourceWriter(File.AppendText(fn));
				writer.IndentLevel = 2;
				writer.Unindent();
				writer.Unindent();
				writer.Close();
			}
		}

		private static bool UsesBrms(IList<ColumnDef> cols)
		{
			foreach (var c in cols)
			{
				if (c.Name.Equals("TRA_ID", StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private static void WritePartialMethods
		(
			string dtoName,
			string servletName,
			string ns,
			IList<ColumnDef> cols
		)
		{
			string filename = "TandemDataFactory" + ns + ".cs";
			bool writeHeader = !File.Exists(filename);

			if (writeHeader)
			{
				FileStream fs = File.Create(filename);
				fs.Close();
			}

			SourceWriter writer = new SourceWriter(File.AppendText(filename));

			if (writeHeader)
			{
				_daFiles.Add(filename);

				writer.WriteUsing("System");
				writer.WriteUsing("System.Collections.Generic");
				writer.WriteUsing("System.Linq");
				writer.WriteUsing("System.Text");
				writer.WriteLine();
				writer.WriteUsing("DOR.Core");
				writer.WriteUsing("DOR.Core.Config");
				writer.WriteUsing("DOR.Core.Data.Tandem");
				writer.WriteLine();
				writer.WriteUsing("BusinessRegistration.Model");
				writer.WriteUsing("TandemData.Entities");
				if (!String.IsNullOrEmpty(ns))
				{
					writer.WriteUsing("TandemData.Entities." + ns);
				}
				writer.WriteLine();

				writer.WriteLine("namespace TandemData");
				writer.IndentManual();
				writer.WriteLine("public partial class TandemDataFactory");
				writer.IndentManual();
			}
			else
			{
				writer.IndentLevel = 2;
			}

			writer.WriteLine("///WARNING!!! Requires manual correction prior to use");
			writer.WriteLine("public IList<" + dtoName + "> " + dtoName + "Sel");
			writer.WriteLine("(");
			writer.WriteLine("\t" + cols[0].CsharpTypeName() + " " + CsFieldNameConverter.Convert(cols[0].Name.Replace("_", "-")));
			writer.WriteLine(")");

			using (writer.Indent())
			{
				writer.WriteLine("TandemDataReader reader = ExecuteReader");
				writer.WriteLine("(");
				writer.WriteLine("\t\"et." + (String.IsNullOrEmpty(ns) ? "" : ns + ".") + servletName + "\",");
				writer.WriteLine("\tnew TandemParameter(\"action\", \"inquire\"), ");
				writer.WriteLine("\tnew TandemParameter(\"" + cols[0].Name + "\", " + CsFieldNameConverter.Convert(cols[0].Name.Replace("_", "-")) + ".ToString())");
				writer.WriteLine(");");
				writer.WriteLine();

				writer.WriteLine("List<" + dtoName + "> lst = new List<" + dtoName + ">();");
				writer.WriteLine("while (reader.Read())");
				using (writer.Indent())
				{
					writer.WriteLine(dtoName + " rec = new " + dtoName + "();");
					foreach (var c in cols)
					{
						string parseClass = c.ParseClass();
						writer.WriteIndent();
						writer.Write("rec.");
						writer.Write(CsFieldNameConverter.Convert(c.Name.Replace("_", "-")));
						writer.Write(" = ");
						if (String.IsNullOrEmpty(parseClass))
						{
							writer.Write("(string)");
						}
						else
						{
							writer.Write(parseClass);
							writer.Write(".Parse((string)");
						}
						writer.Write("reader[\"");
						writer.Write(c.Name);
						writer.Write("\"]");
						if (!String.IsNullOrEmpty(parseClass))
						{
							writer.Write(")");
						}
						writer.Write(";");
						writer.WriteLine();
					}

					writer.WriteLine();
					writer.WriteLine("lst.Add(rec);");
				}

				writer.WriteLine("return lst;");
			}

			writer.WriteLine();
			writer.Close();
		}

		private static string WriteJava(string define, string ns, IList<ColumnDef> cols)
		{
			if (!Directory.Exists("Java"))
			{
				Directory.CreateDirectory("Java");
			}

			string servletName = define.Substring(1) + "Sel";
			string path = String.IsNullOrEmpty(ns) ? "Java" : Path.Combine("Java", ns);

			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			string filename = Path.Combine(path, servletName + ".java");

			if (File.Exists(filename))
			{
				File.Delete(filename);
			}

			SourceWriter writer = new SourceWriter(new StreamWriter(File.OpenWrite(filename)));

			// Tandem don't like DOS line ending
			writer.LineEnding = "\n";

			writer.WriteLine("// nice javac et/" + (String.IsNullOrEmpty(ns) ? "" : ns + "/") + servletName + ".java -d ../classes -classpath lib/daDefs.jar:lib/tdmext.jar:lib/tmf.jar:lib/smtp.jar:lib/mailapi.jar:lib/imap.jar:lib/activation.jar:lib/jdom.jar:lib/jdbcMx.jar:lib/sqlmp.jar:lib/sqlj.jar:lib/servlet-api.jar:.");
			writer.WriteLine();

			if (String.IsNullOrEmpty(ns))
			{
				writer.WriteLine("package et;");
			}
			else
			{
				writer.WriteLine("package et." + ns + ";");
			}

			writer.WriteLine();

			writer.WriteLine("import javax.servlet.*;");
			writer.WriteLine("import javax.servlet.http.*;");
			writer.WriteLine("");
			writer.WriteLine("import java.util.*;");
			writer.WriteLine("import java.text.*;");
			writer.WriteLine("import java.io.*;");
			writer.WriteLine("");
			writer.WriteLine("import java.sql.*;");
			writer.WriteLine("");
			writer.WriteLine("import com.tandem.ext.util.*;");
			writer.WriteLine("");
			writer.WriteLine("import com.tandem.tsmp.*;");
			writer.WriteLine("import com.tandem.tmf.*;");
			writer.WriteLine("");
			writer.WriteLine("import gov.revenue.*;");
			writer.WriteLine("import da.*;");
			writer.WriteLine("import elf.Util;");
			writer.WriteLine();

			writer.WriteLine("public class " + servletName + " extends SqlServletBaseClass");
			using (writer.Indent())
			{
				writer.WriteLine("public static void main(String[] cmdArgs)");
				using (writer.Indent())
				{
					writer.WriteLine("HashMap args = new HashMap();");
					writer.WriteLine("for (int x = 0; x < cmdArgs.length; x++)");
					using (writer.Indent())
					{
						writer.WriteLine("int pos = cmdArgs[x].indexOf('=');");
						writer.WriteLine("args.put(cmdArgs[x].substring(0, pos), cmdArgs[x].substring(pos + 1));");
					}
					writer.WriteLine();
					writer.WriteLine(servletName + " instance = new " + servletName + "();");
					writer.WriteLine("mainInner(instance, args);");
				}
				writer.WriteLine();

				writer.WriteLine("protected String description()");
				using (writer.Indent())
				{
					writer.WriteLine("return \"Selects " + define + ".\";");
				}
				writer.WriteLine();

				writer.WriteLine("protected HashMap createDefaultArguments()");
				writer.WriteLine("throws Exception");
				using (writer.Indent())
				{
					writer.WriteLine("HashMap args = new HashMap();");
					writer.WriteLine("args.put(\"returnMetaData\", null);");

					foreach (var c in cols)
					{
						writer.WriteIndent();
						writer.Write("args.put(\"");
						writer.Write(c.Name);
						writer.Write("\", null);");
						writer.WriteLine();
					}

					writer.WriteLine("return args;");
				}

				writer.WriteLine("protected boolean validateArguments(HashMap args)");
				writer.WriteLine("throws Exception");
				using (writer.Indent())
				{
					writer.WriteLine("// Use args.get(\"FIELD_NAME\") != null");
					writer.WriteLine("// to validate the presence of required fields");
					writer.WriteLine("return args.size() > 0;");
				}
				writer.WriteLine();

				writer.WriteLine("protected void load");
				writer.WriteLine("(");
				writer.WriteLine("	String action, ");
				writer.WriteLine("	HashMap args, ");
				writer.WriteLine("	StringBuffer buf");
				writer.WriteLine(")");
				writer.WriteLine("throws ");
				writer.WriteLine("	SQLException,");
				writer.WriteLine("	ServletException, ");
				writer.WriteLine("	IOException, ");
				writer.WriteLine("	com.tandem.ext.util.DataConversionException, ");
				writer.WriteLine("	com.tandem.tsmp.TsmpSendException, ");
				writer.WriteLine("	com.tandem.tsmp.TsmpServerUnavailableException");

				using (writer.Indent())
				{
					writer.WriteLine("StringBuffer sql = new StringBuffer();");
					writer.WriteLine("boolean writeAnd = false;");
					writer.WriteLine();
					writer.WriteLine("if (action.equals(\"inquire\"))");
					using (writer.Indent())
					{
						writer.WriteLine();
						writer.WriteLine("sql.append(\"SELECT * \");");
						writer.WriteLine("sql.append(\"FROM \");");
						writer.WriteLine("sql.append(" + define.Substring(1).ToLower() + ".define " + ");");
						writer.WriteLine("sql.append(\" \");");
						WriteJavaWhere(cols, writer);
					}
					writer.WriteLine();

					writer.WriteLine("else if (action.equals(\"update\"))");
					using (writer.Indent())
					{
						writer.WriteLine("sql.append(\"UPDATE \");");
						writer.WriteLine("sql.append(" + define.Substring(1).ToLower() + ".define " + ");");
						writer.WriteLine("sql.append(\" SET \");");
						writer.WriteLine("///");
						writer.WriteLine("/// TODO: Fix this");
						writer.WriteLine("buf.append(\"<error>UPDATE not implemented</error>\");");
						writer.WriteLine("///");
						WriteJavaWhere(cols, writer);
					}
					writer.WriteLine("else if (action.equals(\"delete\"))");
					using (writer.Indent())
					{
						writer.WriteLine("sql.append(\"DELETE FROM \");");
						writer.WriteLine("sql.append(" + define.Substring(1).ToLower() + ".define " + ");");
						writer.WriteLine("sql.append(\" \");");
						WriteJavaWhere(cols, writer);
					}
					writer.WriteLine("else if (action.equals(\"insert\"))");
					using (writer.Indent())
					{
						writer.WriteLine("sql.append(\"INSERT INTO \");");
						writer.WriteLine("sql.append(" + define.Substring(1).ToLower() + ".define " + ");");
						writer.WriteLine("sql.append(\" (\");");

						for (int x = 0; x < cols.Count; x++)
						{
							if (x > 0)
							{
								writer.WriteLine("sql.append(\", \");");
							}
							ColumnDef c = cols[x];
							writer.WriteLine("sql.append(\"" + c.Name + "\");");
						}

						writer.WriteLine("sql.append(\") VALUES (\");");

						for (int x = 0; x < cols.Count; x++)
						{
							if (x > 0)
							{
								writer.WriteLine("sql.append(\", \");");
							}
							ColumnDef c = cols[x];
							if (c.SqlType == "DECIMAL" || c.SqlType == "INTEGER" || c.SqlType == "BIGINT" || c.SqlType == "SMALLINT")
							{
								writer.WriteLine("sql.append((String)args.get(\"" + c.Name + " \"));");
							}
							else
							{
								writer.WriteLine("sql.append(\"\\\"\");");
								writer.WriteLine("sql.append((String)args.get(\"" + c.Name + " \"));");
								writer.WriteLine("sql.append(\"\\\"\");");
							}
						}

						writer.WriteLine("sql.append(\")\");");
					}
					writer.WriteLine();

					writer.WriteLine("execSql(sql.toString(), buf);");
				}
			}

			writer.Close();

			return servletName;
		}

		private static void WriteTables()
		{
			string filename = "defines.txt";
			if (!File.Exists(filename))
			{
				filename = "C:\\sources\\SharedSources\\cob2cs\\CobolParser\\Docs\\defines.txt";
			}
			if (!File.Exists(filename))
			{
				Console.WriteLine("Cannot locate defines.txt");
				return;
			}

			if (!Directory.Exists("Tables"))
			{
				Directory.CreateDirectory("Tables");
			}

			WriteFactoryStub();

			TandemDataAccess con = new TandemDataAccess();
			int found = 0;
			int notfound = 0;
			int docfound = 0;
			string[] lines = File.ReadAllLines(filename);

			for (int x = 0; x < lines.Length; x++)
			{
				IList<ColumnDef> cols;
				try
				{
					cols = con.Invoke(lines[x]);
					found++;
				}
				catch (Exception)
				{
					notfound++;
					Console.WriteLine("Java define missing for " + lines[x]);
					continue;
				}

				string ns = "";
				string systemName = FindProbablySystemName(cols);
				if (!String.IsNullOrEmpty(systemName))
				{
					int pos = filename.IndexOf('\\');
					filename = filename.Substring(0, pos + 1) + systemName + "." + filename.Substring(pos + 1);
					ns = "." + systemName;
				}

				if (String.IsNullOrEmpty(systemName))
				{
					filename = Path.Combine("Tables", lines[x].Substring(1) + ".cs");
				}
				else
				{
					string path = Path.Combine("Tables", systemName);
					if (!Directory.Exists(path))
					{
						Directory.CreateDirectory(path);
					}
					filename = Path.Combine(path, lines[x].Substring(1) + ".cs");
				}
				if (File.Exists(filename))
				{
					File.Delete(filename);
				}

				Console.WriteLine("Writing " + lines[x]);

				string servletName = WriteJava(lines[x], systemName, cols);

				WritePartialMethods(lines[x].Substring(1), servletName, systemName, cols);

				SourceWriter writer = new SourceWriter(new StreamWriter(File.OpenWrite(filename)));
				writer.WriteUsing("System");
				writer.WriteUsing("System.Collections.Generic");
				writer.WriteUsing("System.Diagnostics");
				writer.WriteUsing("System.IO");
				writer.WriteUsing("System.Runtime.Serialization");
				writer.WriteLine();
				writer.WriteUsing("DOR.Core");
				if (UsesBrms(cols))
				{
					writer.WriteUsing("BusinessRegistration.Model");
				}
				writer.WriteLine();

				writer.WriteLine("namespace TandemData.Entities" + ns);
				using (writer.Indent())
				{
					writer.WriteLine("[Serializable]");
					writer.WriteLine("[DataContract]");
					writer.WriteLine("public class " + lines[x].Substring(1));

					int isDocFound = 0;
					using (writer.Indent())
					{
						foreach (var c in cols)
						{
							INamedField o = ImportManager.FindDdlDef(c.Name.Replace("_", "-"));

							writer.WriteLine("/// <summary>");
							if (o != null)
							{
								if (!String.IsNullOrEmpty(o.Attributes.Heading))
								{
									writer.WriteLine("/// " + o.Attributes.Heading);
								}
								if (o.Attributes.Pic != null)
								{
									writer.WriteLine("/// PIC " + o.Attributes.Pic.PicFormat.PictureClause);
								}
								if (o.Comments != null)
								{
									isDocFound++;
									writer.WriteLine("/*");
									writer.WriteLine(o.Comments.Replace("/*", "").Replace("*/", "").Replace("\n*", "\r\n\t\t///"));
									writer.WriteLine("*/");
								}
							}
							writer.WriteLine("/// </summary>");
							writer.WriteLine("/// <remarks>");
							writer.WriteLine("/// " + c.Name + " " + c.SqlType + "(" + c.Size + ", " + c.Precision + ":" + c.Scale + ")" + (c.IsIdentity ? " IDENTITY" : "") + (c.IsNullable ? " NULLABLE" : "") + (c.IsSigned ? " SIGNED" : ""));
							writer.WriteLine("/// </remarks>");

							writer.WriteLine("[DataMember]");
							writer.WriteIndent();
							writer.Write("public ");
							writer.Write(c.CsharpTypeName());
							writer.Write(" ");
							writer.Write(CsFieldNameConverter.Convert(c.Name.Replace("_", "-")));
							writer.WriteLine();

							using (writer.Indent())
							{
								writer.WriteLine("get; set;");
							}
							writer.WriteLine();
						}
					}
					if (isDocFound >= cols.Count / 2)
					{
						docfound++;
					}
				}

				writer.Close();
			}

			WriteFactoryStubClose();

			Console.WriteLine("{0} found; {2} documented; {1} not found", found, notfound, docfound);
		}

		private static void LoadDdl2(bool writeToDb = true)
		{
			Console.WriteLine("Writing CobDDL");

			string[] ddlFiles = Directory.GetFiles
			(
				Path.Combine(ImportManager.BaseDirectory, "d23.revprod"), "ddlsrc*"
			);

			// fix up file load order dependancies
			for (int x = 0; x < ddlFiles.Length; x++)
			{
				if (ddlFiles[x].EndsWith("ddlsrcda", StringComparison.InvariantCultureIgnoreCase))
				{
					string s = ddlFiles[0];
					ddlFiles[0] = ddlFiles[x];
					ddlFiles[x] = s;
				}
				if (ddlFiles[x].EndsWith("ddlsrcah", StringComparison.InvariantCultureIgnoreCase))
				{
					string s = ddlFiles[x];
					ddlFiles[x] = ddlFiles[ddlFiles.Length - 1];
					ddlFiles[ddlFiles.Length - 1] = s;
				}
			}

			StringBuilder buf = new StringBuilder();
			Storage storage = new Storage();
			GuardianPath caller = new GuardianPath("$d10.cobol.parser");
			SymbolTable symtab = new SymbolTable();

			foreach (string df in ddlFiles)
			{
				Console.WriteLine(df);

				GuardianPath fileName = ImportManager.GuardianPathFromWindows(df);
				string system = StringHelper.RightStr(df, 2).ToUpper();

				Terminalize terms = ImportManager.GetFile(fileName);

				buf.Clear();

				terms.SkipComments = false;
				terms.BeginIteration();
				terms.Next();

				while
				(
					!terms.IsIterationComplete &&
					(
						terms.CurrentEquals("DEF") ||
						terms.Current.Type == StringNodeType.Comment ||
						terms.Current.Type == StringNodeType.QuestionMark
					)
				)
				{
					if (terms.Current.Type == StringNodeType.QuestionMark)
					{
						VerbLookup.Create(terms, DivisionType.Data);
						continue;
					}
					if (terms.Current.Type == StringNodeType.Comment)
					{
						if
						(
							terms.Current.Str != "*" &&
							!terms.Current.Str.StartsWith("****") &&
							!terms.Current.Str.StartsWith("*-----")
						)
						{
							buf.Append(terms.Current.Str);
							buf.Append('\n');
						}
						terms.Match(StringNodeType.Comment);
						continue;
					}

					storage.AddOne(terms, buf.ToString());
					buf.Clear();
				}
			}

			if (!writeToDb)
			{
				return;
			}

			using (var data = new CobEntities())
			{
				foreach (Offset o in ImportManager.DdlOffsets)
				{
					CobDDL d = new CobDDL();
					d.Field_Docs_Txt = o.Comments == null ? "" : o.Comments;
					d.Field_Name = o.Name;
					d.Field_Pic_Txt = o.Attributes.Pic == null ? "RECORD" : o.Attributes.Pic.PicFormat.PictureClause;
					d.Field_System_Code = StringHelper.RightStr(o.FileName.File, 2).ToUpper();
					d.File_Name = o.FileName.ToString();
					d.Field_Heading_Txt = o.Attributes.Heading == null ? "" : o.Attributes.Heading;

					var q = (from z in data.CobDDLs where z.Field_Name == o.Name select z);
					if (q.Any())
					{
						continue;
					}

					data.CobDDLs.AddObject(d);
					data.SaveChanges();
				}
			}
		}

		public static void ScreenTables(Vector<CobolProgram> progs)
		{
			Console.WriteLine("listing screen tables");

			using (var data = new CobEntities())
			{
				Dictionary<string, string> idx = new Dictionary<string, string>();
				Stack<CobolProgram> stk = new Stack<CobolProgram>();
				Dictionary<string, CobolProgram> pidx = new Dictionary<string, CobolProgram>();

				foreach (CobolProgram tprog in progs)
				{
					if (!pidx.ContainsKey(tprog.Identification.ProgramId))
					{
						pidx.Add(tprog.Identification.ProgramId, tprog);
					}
				}

				foreach (CobolProgram tprog in progs)
				{
					Dictionary<GuardianPath, string> wroteIdx = new Dictionary<GuardianPath, string>();
					stk.Clear();
					stk.Push(tprog);

					while (stk.Count > 0)
					{
						CobolProgram prog = stk.Pop();

						if (prog.Data.ScreenRecord == null)
						{
							continue;
						}

						if (wroteIdx.ContainsKey(prog.FileName))
						{
							continue;
						}
						wroteIdx.Add(prog.FileName, "");

						Dictionary<string, Send> pdict = ImportManager.PathwaySends(prog.FileName);

						if (null == pdict)
						{
							continue;
						}

						foreach (Send send in pdict.Values)
						{
							if (!pidx.ContainsKey(send.ServerClassName))
							{
								continue;
							}
							CobolProgram toprog = pidx[send.ServerClassName];
							if (wroteIdx.ContainsKey(toprog.FileName))
							{
								continue;
							}

							stk.Push(toprog);

							List<string> lst = toprog.ListTables();

							foreach (string s in lst)
							{
								string ss = s.ToUpper();
								if (ss[0] == '=')
								{
									ss = ss.Substring(1);
								}
								if (idx.ContainsKey(prog.FileName.ToString() + "." + ss))
								{
									continue;
								}
								idx.Add(prog.FileName.ToString() + "." + ss, s);

								CobFileXRef x = new CobFileXRef();
								x.Xref_File_Name = prog.FileName.ToString();
								x.Xref_DependsOnFile_Name = ss;
								x.Xref_Type_Code = "T";

								data.CobFileXRefs.AddObject(x);
								data.SaveChanges();
							}
						}
					}
				}
			}
		}
	}
}
