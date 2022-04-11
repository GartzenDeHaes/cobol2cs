using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using DOR.Core;
using DOR.Core.Collections;
using DOR.WorkingStorage;

using CobolParser.Divisions.Proc;
using CobolParser.Expressions;
using CobolParser.Expressions.Terms;
using CobolParser.Generator;
using CobolParser.Records;
using CobolParser.Sections;
using CobolParser.Verbs;

namespace CobolParser.Text
{
	public class CsFormater
	{
		private Dictionary<string, int> _offsetMap = new Dictionary<string, int>();
		private List<string> _extraUsings = new List<string>();
		private List<string> _referencedProjects = new List<string>();

		const int _charWidth = 8;
		const int _charHeight = 16;

		public GuardianPath FileName
		{
			get;
			private set;
		}

		public CobolProgram Cobol
		{
			get;
			private set;
		}

		public List<string> OutputFileNames
		{
			get;
			private set;
		}

		public string CsProjectName
		{
			get;
			private set;
		}

		public bool Wpf
		{
			get;
			private set;
		}

		public UnitTestProject UnitTests
		{
			get;
			private set;
		}

		public string RootNameSpace(INamedField o)
		{
			if ((object)o != null && o.FileName.Volume == "DEFINE")
			{
				return "Tables";
			}
			if ((object)o == null || Cobol.FileName.Equals(o.FileName))
			{
				return CsFieldNameConverter.Convert(Cobol.ProgramId);
			}

			string ns = o.FileName.ToProbablyProgramName();
			_extraUsings.Add(ns);
			return ns;
		}

		public CsFormater(GuardianPath gpath, bool wpf)
		{
			Wpf = wpf;
			FileName = gpath;
			OutputFileNames = new List<string>();

			Cobol = new CobolProgram(gpath);
			Cobol.Procedure.PruneUnReferencedFunctions();
			Cobol.PruneUnusedScreens();

			string progFilename = CsFieldNameConverter.Convert(Cobol.ProgramId);
			if (!Directory.Exists(progFilename))
			{
				Directory.CreateDirectory(progFilename);
			}

			string propDir = Path.Combine(progFilename, "Properties");
			if (!Directory.Exists(propDir))
			{
				Directory.CreateDirectory(propDir);
			}

			// Write 01 records as classes
			Write(Cobol.Data.WorkingStorage.Data);

			string filename = Path.Combine(Cobol.SourceDirectory, CsFieldNameConverter.Convert(Cobol.ProgramId) + ".cs");

			OutputFileNames.Add(filename);
			if (File.Exists(filename))
			{
				File.Delete(filename);
			}

			bool writeUnitTest = Cobol.Environment.InputOutputSection != null &&
				Cobol.Data.Files != null;
			
			using (SourceWriter writer = new SourceWriter(File.CreateText(filename)))
			{
				using (CsVerbs vwriter = new CsVerbs(Cobol, writer, null))
				{
					if (writeUnitTest)
					{
						UnitTests = new UnitTestProject(Cobol, vwriter);
						vwriter.UnitTests = UnitTests;
					}

					WriteProgram(vwriter, writer);

					if (writeUnitTest)
					{
						UnitTests.WriteDataMocks();
						UnitTests.Dispose();
						UnitTests = null;
					}
				}
			}

			WriteScreenSection();

			//WriteTestMain();

			WriteCsproj(progFilename, propDir);
		}

		private void WriteHeader
		(
			SourceWriter writer, 
			string ns, 
			INamedField o,
			bool writeExtraUsings = true
		)
		{
			writer.WriteUsing("System");
			writer.WriteUsing("System.Collections.Generic");
			writer.WriteUsing("System.Data");
			writer.WriteUsing("System.Diagnostics");
			writer.WriteLine();

			writer.WriteUsing("DOR.Core");
			writer.WriteUsing("DOR.Core.Collections");
			writer.WriteUsing("DOR.WorkingStorage");
			writer.WriteUsing("DOR.Core.Config");
			writer.WriteUsing("DOR.Core.Data");
			writer.WriteUsing("DOR.Core.Data.Tandem");
			writer.WriteUsing("DOR.Core.Logging");
			writer.WriteUsing("DOR.WorkingStorage.Pic");
			writer.WriteUsing("Tables");
			writer.WriteLine();

			if (writeExtraUsings)
			{
				Dictionary<string, string> dupCheck = new Dictionary<string, string>();

				foreach (var u in _extraUsings)
				{
					if (dupCheck.ContainsKey(u))
					{
						continue;
					}
					dupCheck.Add(u, u);

					writer.WriteUsing(u);
				}

				if (_extraUsings.Count > 0)
				{
					writer.WriteLine();
				}
			}

			WriteNamespace(writer, ns, o);
		}

		private void WriteNamespace(SourceWriter writer, string ns, INamedField o)
		{
			writer.Write("namespace ");

			if (String.IsNullOrEmpty(ns))
			{
				writer.WriteLine(RootNameSpace(o));
			}
			else
			{
				writer.WriteLine(RootNameSpace(o) + ns);
			}
		}

		private void Write(Storage s)
		{
			foreach (var o in s.Fields)
			{
				Write01(o, 0);
			}
		}

		public static string GetNamespace(INamedField o)
		{
			string ns = "";

			if (!String.IsNullOrEmpty(o.FileSectionMessageName))
			{
				return CsFieldNameConverter.Convert(o.FileSectionMessageName);
			}

			INamedField parent = o.Parent;

			while (parent != null)
			{
				if (String.IsNullOrEmpty(ns))
				{
					ns = CsFieldNameConverter.Convert(parent.Name, parent.SpecialNameHandling);
				}
				else
				{
					ns = CsFieldNameConverter.Convert(parent.Name, parent.SpecialNameHandling) + "." + ns;
				}

				if (! String.IsNullOrEmpty(parent.FileSectionMessageName))
				{
					ns = CsFieldNameConverter.Convert(parent.FileSectionMessageName) + "." + ns;
				}

				parent = parent.Parent;
			}

			int dotCount = StringHelper.CountOccurancesOf(ns, '.');
			if (dotCount == 1)
			{
				ns = ns.Replace(".", "");
			}
			else if (dotCount > 1)
			{
				int spos = ns.IndexOf('.');
				int lpos = ns.LastIndexOf('.');
				ns = ns.Substring(0, spos) + ns.Substring(lpos + 1);
			}

			return ns;
		}

		private void Write01(INamedField o, int startPos, SourceWriter writer = null)
		{
			if (Cobol.IsNative(o))
			{
				return;
			}

			bool closeWriter = writer == null;
			List<Association<INamedField, int>> innerTypes = new List<Association<INamedField, int>>();
			string name = CsFieldNameConverter.Convert(o.Name, o.SpecialNameHandling);
			string ns = GetNamespace(o);
			string filename = Cobol.SourceDirectory + (String.IsNullOrEmpty(ns) ? "" : ns + ".") + name + ".cs";

			var dups = from ofn in OutputFileNames where ofn.Equals(filename, StringComparison.InvariantCultureIgnoreCase) select ofn;
			if (dups.Count() > 0)
			{
			    o.SpecialNameHandling = true;
				if (o.Name.IndexOf("-") < 0)
				{
					name += "_";
				}
				else
				{
					name = o.Name.Replace("-", "_");
				}
				name = name.ToUpper();
				filename = Cobol.SourceDirectory + (String.IsNullOrEmpty(ns) ? "" : ns + ".") + name + ".cs";
			}

			foreach (var oo in o.SubGroups)
			{
				if 
				(
					oo.SubGroups.Count > 0 && 
					Cobol.Data.WorkingStorage.Data.GroupHasDuplicate(oo.Name)
				)
				{
					_extraUsings.Add(RootNameSpace(oo) + GetNamespace(oo));
				}				
			}

			_offsetMap.Clear();

			int pos = startPos;

			if (writer == null)
			{
				OutputFileNames.Add(filename);
				if (File.Exists(filename))
				{
					File.Delete(filename);
				}

				writer = new SourceWriter(File.CreateText(filename));

				WriteHeader(writer, ns, o, false);
			}
			else
			{
				WriteNamespace(writer, ns, o);
			}

			using (writer.Indent())
			{
				if (!String.IsNullOrEmpty(o.Comments))
				{
					writer.WriteLine("/*");
					writer.WriteLine(o.Comments.Replace("/*", "*").Replace("*/", "*"));
					writer.WriteLine("*/");
				}
				if (!String.IsNullOrEmpty(o.Attributes.Comments))
				{
					writer.WriteLine("/*");
					writer.WriteLine(o.Attributes.Comments.Replace("/*", "*").Replace("*/", "*"));
					writer.WriteLine("*/");
				}
				foreach (var s in o.RedefinesHints)
				{
					writer.WriteLine("// REDEFINE: " + s);
				}
				foreach (var s in o.RedefinedByHints)
				{
					writer.WriteLine("// REDEFINED BY: " + s);
				}
				if (o.RedefinesHints.Count > 0 || o.RedefinedByHints.Count > 0)
				{
					writer.WriteLine();
				}

				string access = "public";

				writer.WriteLine("[Serializable]");
				writer.WriteLine(access + " class " + name + " : WsRecord");
				using (writer.Indent())
				{
					// Constructorx
					if (o.Parent == null)
					{
						writer.WriteLine("public " + name + "()");
						writer.WriteLine(": this(new MemoryBuffer(" + o.SingleRecordLength(true).ToString() + "), 0)");
						writer.WriteLine("{");
						writer.WriteLine("}");
						writer.WriteLine();

						writer.WriteLine("public " + name + "(MemoryBuffer buf)");
						writer.WriteLine(": this(buf, 0)");
						writer.WriteLine("{");
						writer.WriteLine("}");
						writer.WriteLine();
					}

					writer.WriteLine("public " + name + "(MemoryBuffer buf, int basePos, bool isTemp = false)");
					writer.WriteLine(": base(\"" + o.Name + "\", buf, " + startPos + " + basePos, " + o.SingleRecordLength(true) + ")");

					using (writer.Indent())
					{
						if (o.Attributes.DefaultValue != null)
						{
							string val = o.Attributes.DefaultValue.ToCListStringList();

							if (val.Equals("\"SPACES\"", StringComparison.InvariantCultureIgnoreCase))
							{
								writer.WriteLine("ClearTo(' ');");
							}
							else if (val.Equals("\"ZEROES\"", StringComparison.InvariantCultureIgnoreCase))
							{
								writer.WriteLine("ClearTo('0');");
							}
							else
							{
								if (val != "\"\"")
								{
									writer.WriteLine("Set(" + val + ");");
								}
							}

							if (o.SubGroups.Count > 0)
							{
								writer.WriteLine();
							}
						}

						if (o.Attributes != null && o.Attributes.Pic != null)
						{
							// PIC on group record, or single field 01.
							writer.WriteLine("Format = PicFormat.Parse(\"" + o.Attributes.Pic.PicFormat.PictureClause + "\");");
							if (o.SubGroups.Count > 0)
							{
								writer.WriteLine();
							}
						}

						int sbasePosCnt = 0;

						foreach (var oo in o.SubGroups)
						{
							if (!_offsetMap.ContainsKey(oo.Name))
							{
								_offsetMap.Add(oo.Name, pos);
							}

							if 
							(
								oo.SubGroups.Count > 0 || 
								oo.Attributes.ValueChoices.Count > 0 ||
								(oo.Attributes.Occures != null && oo.Attributes.Occures.MaximumTimes > 1)
							)
							{
								if (oo.Attributes.Occures != null && oo.Attributes.Occures.MaximumTimes > 1)
								{
									if (oo.SubGroups.Count > 0 || oo.Attributes.ValueChoices.Count > 0)
									{
										writer.WriteLine(CsFieldNameConverter.Convert(oo.Name, oo.SpecialNameHandling) + " = new " + RootNameSpace(oo) + GetNamespace(oo) + "." + CsFieldNameConverter.Convert(oo.Name, oo.SpecialNameHandling) + "[" + oo.Attributes.Occures.MaximumTimes + "];");
									}
									else
									{
										writer.WriteLine(CsFieldNameConverter.Convert(oo.Name, oo.SpecialNameHandling) + " = new IBufferOffset[" + oo.Attributes.Occures.MaximumTimes + "];");
									}
									if (sbasePosCnt > 0)
									{
										writer.WriteLine("sbasePos = basePos;");
									}
									else
									{
										writer.WriteLine("int sbasePos = basePos;");
									}
									sbasePosCnt++;

									writer.WriteLine("for (int x = 0; x < " + oo.Attributes.Occures.MaximumTimes + "; x++)");
									using (writer.Indent())
									{
										string fqname = CsFieldNameConverter.Convert(oo.Name, oo.SpecialNameHandling);
										if (oo.SubGroups.Count > 0 || oo.Attributes.ValueChoices.Count > 0)
										{
											writer.WriteLine(fqname + "[x] = new " + RootNameSpace(oo) + GetNamespace(oo) + "." + CsFieldNameConverter.Convert(oo.Name, oo.SpecialNameHandling) + "(buf, sbasePos);");
										}
										else
										{
											writer.WriteIndent();
											writer.Write(fqname + "[x] = ");
											WriteOffsetNew
											(
												writer,
												oo,
												pos,
												oo.SingleRecordLength(),
												false,
												null,
												false,
												"sbasePos",
												oo.Attributes.Pic.PicFormat.PictureClause
											);
											writer.WriteLine();
										}
										writer.WriteLine("Add(\"" + fqname + "\" + x, " + fqname + "[x]);");
										writer.WriteLine("sbasePos += " + oo.SingleRecordLength() + ";");
									}
								}
								else
								{
									string oonamme = CsFieldNameConverter.Convert(oo.Name, oo.SpecialNameHandling);
									if (!Cobol.IsNative(oo))
									{
										writer.WriteLine
										(
											oonamme +
											" = new " +
											RootNameSpace(oo) + GetNamespace(oo) + "." + oonamme +
											"(buf, basePos);"
										);
										writer.WriteLine("Add(\"" + oonamme + "\", " + oonamme + ");");
									}
								}

								int subPos = pos;
								if (oo.Redefines != null)
								{
									subPos = _offsetMap[oo.Redefines.RedefinesOffsetNamed];
								}

								innerTypes.Add(new Association<INamedField,int>(oo, subPos));
							}
							else
							{
								WriteOffsetConstructorContent("", writer, oo, pos);
							}

							if (oo.Redefines == null)
							{
								pos += oo.Length();
							}
						}

					//    foreach (FieldOption88 c in o.Attributes.ValueChoices)
					//    {
					//        BufferType type = BufferType.Unknown;

					//        if (o.Attributes.Pic == null)
					//        {
					//            type = BufferType.String;
					//        }
					//        else if (o.Attributes.Pic.Decimals > 0)
					//        {
					//            type = BufferType.Decimal;
					//        }
					//        else if (o.Attributes.Pic.PicType == '9')
					//        {
					//            type = BufferType.Int;
					//        }
					//        else
					//        {
					//            type = BufferType.String;
					//        }
					//        writer.WriteIndent();

					//        if (!Cobol.IsNative(c))
					//        {
					//            writer.Write("Add(\"" + CsFieldNameConverter.Convert(c.Name) + "\", ");

					//            WriteOffsetNew
					//            (
					//                writer,
					//                o,
					//                pos,
					//                type,
					//                o.Attributes.Pic == null ? false : o.Attributes.Pic.Signed,
					//                o.Length(),
					//                true,
					//                o.Attributes.Pic == null ? 0 : o.Attributes.Pic.Decimals,
					//                c.Values.ToCListStringList(),
					//                true,
					//                "basePos"
					//            );
					//            writer.WriteLine();
					//        }
					//    }
					}

					if (o.SubGroups.Count > 0)
					{
						writer.WriteLine();
					}
					foreach (var oo in o.SubGroups)
					{
						name = CsFieldNameConverter.Convert(oo.Name, oo.SpecialNameHandling);

						if (oo.Attributes.Occures != null && oo.Attributes.Occures.MaximumTimes > 1)
						{
							string tname = oo.SubGroups.Count > 0 ? (RootNameSpace(oo) + GetNamespace(oo) + "." + name) : "IBufferOffset";
							writer.WriteLine(access + " " + tname + "[] " + name + " { get; set; }");
						}
						else if (oo.SubGroups.Count > 0 || oo.Attributes.ValueChoices.Count > 0)
						{
							string oons = GetNamespace(oo);
							writer.WriteLine(access + " " + RootNameSpace(oo) + oons + "." + name + " " + name + " { get; set; }");
						}
						else
						{
							if (Cobol.IsNative(oo))
							{
								string clrType = oo.Attributes.Pic.CsClrTypeName();
								string val = oo.Attributes.DefaultValue == null ? (clrType == "string" ? "\"\"" : "0") : oo.Attributes.DefaultValue.ToString();
								writer.WriteLine("private " + clrType + " _" + name + " = " + val + ";");
								writer.WriteLine(access + " " + clrType + " " + name);
								using (writer.Indent())
								{
									writer.WriteLine("get { return _" + name + "; }");
									writer.WriteLine("set { _" + name + " = value; RaisePropertyChanged(\"" + name + "Binding\"); }");
								}
								writer.WriteLine();
							}
							else
							{
								writer.WriteLine(access + " IBufferOffset " + name + " { get { return this[\"" + name + "\"]; } }");
							}
						}
					}

					foreach (var o88 in o.Attributes.ValueChoices)
					{
						name = CsFieldNameConverter.Convert(o88.Name);
						//writer.WriteLine(access + " IBufferOffset " + name + " { get { return this[\"" + name + "\"]; } }");

						if (o88.Values is Expr && ((Expr)o88.Values).Terms[0] is Range)
						{
							Range r = (Range)((Expr)o88.Values).Terms[0];

							writer.WriteIndent();
							writer.Write(access + " bool " + name + " { get { return ");

							for (int x = 0; x < r.Ranges.Count; x++)
							{
								if (x > 0)
								{
									writer.Write(" || ");
								}
								string f = r.Ranges[x].From.ToString();
								if (StringHelper.IsInt(StringHelper.StripQuotes(f)))
								{
									f = StringHelper.StripQuotes(f);
								}
								string t = r.Ranges[x].To.ToString();
								if (StringHelper.IsInt(StringHelper.StripQuotes(t)))
								{
									t = StringHelper.StripQuotes(t);
								}
								writer.Write("IsInRange(" + f + ", " + t + ")");
							}
							writer.Write("; } }");
							writer.WriteLine();
						}
						else
						{
							if (o88.Values.Count == 1)
							{
								writer.WriteLine(access + " readonly " + (o88.Values.IsNumber ? "int " : "string ") + name + "Value = " + o88.Values.ToString() + ";");
								writer.WriteLine(access + " bool " + name + " { get { return " + (o88.Values.IsNumber ? "ToInt()" : "ToString().Trim()") + " == " + name + "Value; } }");
							}
							else
							{
								writer.WriteLine(access + " bool " + name + " { get { return IsOneOf(" + o88.Values.ToStringTryConvertToInt() + "); } }");
							}
						}
					}
				}
			}

			if (innerTypes.Count > 0)
			{
				writer.WriteLine();

				foreach (var a in innerTypes)
				{
					_offsetMap = new Dictionary<string, int>();
					Write01(a.Left, a.Right, writer);
				}
			}

			if (closeWriter)
			{
				writer.Close();
			}
		}

		private void WriteOffsetConstructorContent
		(
			string wsRecordPrefix,
			SourceWriter writer, 
			INamedField o, 
			int pos
		)
		{
			if (o.Redefines != null)
			{
				pos = _offsetMap[o.Redefines.RedefinesOffsetNamed];
			}

			string name = CsFieldNameConverter.Convert(o.Name, o.SpecialNameHandling);
			int count = o.Attributes.Occures == null ? 1 : o.Attributes.Occures.MaximumTimes;
			int len = o.Length() / count;

			for (int x = 0; x < count; x++)
			{
				if (!Cobol.IsNative(o))
				{
					writer.WriteIndent();
				}

				if (count == 1)
				{
					if (!Cobol.IsNative(o))
					{
						writer.Write(wsRecordPrefix);
						writer.Write("Add(\"" + name + "\", ");
						WriteOffsetNew
						(
							writer,
							o,
							pos,
							len,
							count == 1,
							"",
							false,
							"basePos",
							o.Attributes.Pic.PicFormat.PictureClause
						);
						writer.WriteLine();
					}
				}

				if (o.Attributes.Occures != null && o.Attributes.Occures.MaximumTimes == 1)
				{
					if (o.SubGroups.Count > 0)
					{
						writer.WriteLine();
					}

					foreach (var oo in o.SubGroups)
					{
						name = CsFieldNameConverter.Convert(oo.Name, oo.SpecialNameHandling);
						writer.WriteLine(name + " = new " + RootNameSpace(oo) + name + "(buf, basePos);");
						writer.WriteLine("Add(\"" + name + "\", " + name + ");");
					}
				}

				pos += len;
			}
		}

		private void WriteOffsetNew
		(
			SourceWriter writer, 
			INamedField o,
			int pos, 
			int length,
			bool doubleClose,
			string f88,
			bool is88,
			string basePosName,
			string format
		)
		{
			string defVal = o.Attributes.DefaultValue == null ? "null" : o.Attributes.DefaultValue.ToCListStringList();

			writer.Write("new WsRecord(Buffer, ");
			writer.Write(pos.ToString());
			writer.Write(" + " + basePosName + ", ");
			writer.Write(length.ToString());
			writer.Write(", ");

			writer.Write(defVal);
			writer.Write(", ");
			writer.Write(o.Attributes.ValueExpandToSize.ToString().ToLower());
			writer.Write(", new string[] {");
			writer.Write(f88);
			writer.Write("}");

			writer.Write(", \"");
			writer.Write(o.Name);
			writer.Write("\", \"");
			writer.Write(o.FindAncesterRedefines());
			if (is88)
			{
				writer.Write("88");
			}
			writer.Write("\"");

			writer.Write(", \"");
			writer.Write(format);
			writer.Write("\", isTemp");

			if (doubleClose)
			{
				writer.Write("));");
			}
			else
			{
				writer.Write(");");
			}
		}

		Dictionary<string, string> cursoIdx = new Dictionary<string, string>();

		private void WriteCursors(SourceWriter writer, Vector<IVerb> verbs)
		{
			foreach (var vrb in verbs)
			{
				if (vrb is ExecSql)
				{
					if (((ExecSql)vrb).Sql is CobolParser.SQL.Statements.OpenSql)
					{
						CobolParser.SQL.Statements.OpenSql osql = (CobolParser.SQL.Statements.OpenSql)((ExecSql)vrb).Sql;

						if (cursoIdx.ContainsKey(osql.CursorName))
						{
							continue;
						}
						cursoIdx.Add(osql.CursorName, osql.CursorName);

						writer.WriteLine("private IDataReaderEx " + CsFieldNameConverter.Convert(osql.CursorName) + ";");
					}
				}
				else if (vrb is If)
				{
					WriteCursors(writer, ((If)vrb).Stmts.Stmts);
					if (((If)vrb).ElseStmts != null)
					{
						WriteCursors(writer, ((If)vrb).ElseStmts.Stmts);
					}
				}
				else if (vrb is Evaluate)
				{
					foreach (var when in ((Evaluate)vrb).Whens)
					{
						WriteCursors(writer, when.Stmts.Stmts);
					}
				}
			}
		}

		private string GuessAtGroupOffset(IExpr iexpr, int idx)
		{
			Debug.Assert(((Expr)iexpr).Terms.Count == 1);

			ITerm term = ((Expr)iexpr).Terms[0];
			INamedField f;
			StringBuilder buf = new StringBuilder();

			if (term is Id)
			{
				if (((Id)term).Offsets.Count != 0)
				{
					throw new NotImplementedException();
				}
				f = Cobol.Data.LocateField(((Id)term).Value.Str, null);
			}
			else if (term is OffsetReference)
			{
				OffsetReference or = (OffsetReference)term;
				f = Cobol.Data.LocateField(or.OffsetChain[0].Value.Str, or.OffsetChain[or.OffsetChain.Count - 1].Value.Str);
			}
			else
			{
				throw new Exception();
			}

			Stack<string> stk = new Stack<string>();

			while (f != null)
			{
				if (f.Occurances > 1)
				{
					stk.Push("[" + idx.ToString() + "]");
				}

				stk.Push(CsFieldNameConverter.Convert(f.Name));

				f = f.Parent;
			}

			int count = 0;
			while (stk.Count > 0)
			{
				if (count++ > 0 && stk.Peek()[0] != '[')
				{
					buf.Append('.');
				}
				buf.Append(stk.Pop());
			}

			return buf.ToString();
		}

		private void WriteProgram(CsVerbs vwriter, SourceWriter writer)
		{
			writer.WriteLine("/*");
			writer.WriteLine(" * " + Cobol.FileName.ToString() + " dated " + File.GetLastWriteTime(Cobol.FileName.WindowsFileName()));
			writer.WriteLine(" * Conversion ran on " + DateTime.Now.ToString());
			writer.WriteLine("*/");

			if (Cobol.Data.ScreenRecord != null)
			{
				_extraUsings.Add("DOR.WPF");
				_extraUsings.Add("System.Collections.Concurrent");
				_extraUsings.Add("System.Windows");
				_extraUsings.Add("System.Windows.Threading");
				_extraUsings.Add("System.Threading");
				_extraUsings.Add("GalaSoft.MvvmLight.Threading");
			}

			WriteHeader(writer, null, null);

			using (writer.Indent())
			{
				string baseClass = Cobol.Data.ScreenRecord == null ? "CobolBase" : "CobolViewModelBase";

				writer.Write("/*" + Cobol.Identification.HeaderComments.Replace("\n", "\r\n") + "*/");
				writer.WriteLine();
				writer.WriteLine("[Serializable]");
				writer.WriteLine("public class " + CsFieldNameConverter.Convert(Cobol.ProgramId) + "Program : " + baseClass);
				using (writer.Indent())
				{
					if (Cobol.Data.ScreenRecord != null)
					{
						writer.WriteLine("///TODO:");
						writer.WriteLine("///\t1. If transactions are used, verify that the conversion to batch update");
						writer.WriteLine("///\t   Transactions are not required to do updates, so remove any that are");
						writer.WriteLine("///\t   unnecessary.  If you do need transactions for rollback, verify that the");
						writer.WriteLine("///\t   conversion to batch updates works correctly. Look for any instances of ROWS");
						writer.WriteLine("///\t   and SQL-DUP-REC.");
						writer.WriteLine("///\t2.  Change menu screen number in the constructor's :base() call.");
						writer.WriteLine();
					}

					writer.WriteLine("#region working storage");
					writer.WriteLine();

					foreach (var o in Cobol.Data.WorkingStorage.Data.Fields)
					{
						string prm = "";
						if (o.Redefines != null)
						{
							INamedField fo = Cobol.Data.WorkingStorage.Data.LocateField(o.Redefines.RedefinesOffsetNamed, null);
							string ns = GetNamespace((Offset)fo);
							prm = (String.IsNullOrEmpty(ns) ? "" : ns + ".") + CsFieldNameConverter.Convert(o.Redefines.RedefinesOffsetNamed) + ".Buffer";
						}

						if (o.Redefines == null)
						{
							string rootNs = RootNameSpace(o);

							if (Cobol.IsNative(o))
							{
								writer.WriteLine
								(
									"private " +
									o.Attributes.Pic.CsClrTypeName() +
									" " +
									(String.IsNullOrEmpty(o.FileSectionMessageName) ? "" : CsFieldNameConverter.Convert(o.FileSectionMessageName)) +
									CsFieldNameConverter.Convert(o.Name, o.SpecialNameHandling) +
									" = " + o.Attributes.DefaultValue.ToString() +
									";"
								);
							}
							else
							{
								writer.WriteIndent();
								if (o.Name == "CL-CONTROL-PARM")
								{
									writer.Write("internal ");
								}
								else
								{
									writer.Write("private ");
								}
								writer.Write(rootNs);
								writer.Write(".");
								writer.Write((String.IsNullOrEmpty(o.FileSectionMessageName) ? "" : CsFieldNameConverter.Convert(o.FileSectionMessageName) + "."));
								writer.Write(CsFieldNameConverter.Convert(o.Name, o.SpecialNameHandling));
								writer.Write(" ");
								writer.Write((String.IsNullOrEmpty(o.FileSectionMessageName) ? "" : CsFieldNameConverter.Convert(o.FileSectionMessageName)));
								writer.Write(CsFieldNameConverter.Convert(o.Name, o.SpecialNameHandling));

								if (o.Name != "CL-CONTROL-PARM")
								{
									writer.Write(" = new ");
									writer.Write(rootNs);
									writer.Write(".");
									writer.Write((String.IsNullOrEmpty(o.FileSectionMessageName) ? "" : CsFieldNameConverter.Convert(o.FileSectionMessageName) + "."));
									writer.Write(CsFieldNameConverter.Convert(o.Name, o.SpecialNameHandling));
									writer.Write("(" + prm + ")");
								}
								writer.Write(";");
								writer.WriteLine();
							}
						}
						else
						{
							writer.WriteLine
							(
								"private " +
								CsFieldNameConverter.Convert(o.Name, o.SpecialNameHandling) +
								" " +
								CsFieldNameConverter.Convert(o.Name, o.SpecialNameHandling) +
								";"
							);
						}
					}
					writer.WriteLine();
					writer.WriteLine("#endregion");
					writer.WriteLine();

					if (Wpf && Cobol.Data.ScreenRecord != null)
					{
						writer.WriteLine("#region data binding");
						writer.WriteLine();

						Dictionary<string, string> sidx = new Dictionary<string, string>();
						foreach (Screen s in Cobol.Data.ScreenRecord.Screens)
						{
							writer.WriteLine("#region " + s.Name);
							writer.WriteLine();

							writer.WriteLine("private Visibility _" + CsFieldNameConverter.Convert(s.Name) + "Visibility = Visibility.Hidden;");
							writer.WriteLine("public Visibility " + CsFieldNameConverter.Convert(s.Name) + "Visibility");
							using (writer.Indent())
							{
								writer.WriteLine("get { return _" + CsFieldNameConverter.Convert(s.Name) + "Visibility; }");
								writer.WriteLine("set { _" + CsFieldNameConverter.Convert(s.Name) + "Visibility = value; RaisePropertyChanged(\"" + CsFieldNameConverter.Convert(s.Name) + "Visibility\"); }");
							}
							writer.WriteLine();

							if (s.IsOverlay)
							{
								writer.WriteLine("private int _" + CsFieldNameConverter.Convert(s.Name) + "Col;");
								writer.WriteLine("public int " + CsFieldNameConverter.Convert(s.Name) + "Col");
								using (writer.Indent())
								{
									writer.WriteLine("get { return _" + CsFieldNameConverter.Convert(s.Name) + "Col; }");
									writer.WriteLine("set { _" + CsFieldNameConverter.Convert(s.Name) + "Col = value * " + _charWidth + "; RaisePropertyChanged(\"" + CsFieldNameConverter.Convert(s.Name) + "Col\"); }");
								}
								writer.WriteLine();

								writer.WriteLine("private int _" + CsFieldNameConverter.Convert(s.Name) + "Row;");
								writer.WriteLine("public int " + CsFieldNameConverter.Convert(s.Name) + "Row");
								using (writer.Indent())
								{
									writer.WriteLine("get { return _" + CsFieldNameConverter.Convert(s.Name) + "Row; }");
									writer.WriteLine("set { _" + CsFieldNameConverter.Convert(s.Name) + "Row = value * " + _charHeight + "; RaisePropertyChanged(\"" + CsFieldNameConverter.Convert(s.Name) + "Row\"); }");
								}
								writer.WriteLine();
							}

							foreach (var o in s.Fields)
							{
								bool ro = false;
								IExpr getexpr = null;
								IExpr setexpr = null;

								if (o.FromExpr != null)
								{
									getexpr = o.FromExpr;
									if (o.ToExpr == null)
									{
										setexpr = getexpr;
										ro = true;
									}
									else
									{
										setexpr = o.ToExpr;
									}
								}
								else if (o.ToExpr != null)
								{
									getexpr = o.ToExpr;
									setexpr = getexpr;
								}
								else if (o.UsingExpr != null)
								{
									getexpr = o.UsingExpr;
									setexpr = getexpr;
								}

								if (getexpr != null)
								{
									Offset f = IExprToOffset(getexpr);

									if (f == null)
									{
										writer.WriteLine("///");
										writer.WriteLine("///TODO: Couldn't locate field " + getexpr.ToString());
										writer.WriteLine("///");
										writer.WriteLine();
										continue;
									}

									string getfname = IExprToFq(getexpr);
									string setfname = IExprToFq(setexpr);

									if (sidx.ContainsKey(getfname))
									{
										continue;
									}
									sidx.Add(getfname, getfname);

									for (int x = 0; x < o.OccuresOnLines * (o.Occures == null ? 1 : o.Occures.Columns); x++)
									{
										writer.WriteIndent();
										writer.Write("public ");
										//string propType = f.Attributes.Pic == null ? "string" : f.Attributes.Pic.CsClrTypeName();
										string propType = "string";
										writer.Write(propType);
										writer.Write(" ");
										string pname = CsFieldNameConverter.Convert(f.Name) + (o.OccuresOnLines > 1 ? x.ToString() : "");
										writer.Write(pname);
										writer.Write("Binding");
										writer.WriteLine();
										using (writer.Indent())
										{
											string getfnameQ = o.OccuresOnLines > 1 ? GuessAtGroupOffset(getexpr, x) : getfname;
											string setfnameQ = o.OccuresOnLines > 1 ? GuessAtGroupOffset(setexpr, x) : setfname;

											string upshift = o.IsUpshift == "I" || o.IsUpshift == "I-O" || o.IsUpshift == "INPUT" || o.IsUpshift == "INPUT-OUTPUT" ? ".ToUpper()" : "";

											if (Cobol.IsNative(o))
											{
												writer.WriteLine("get { return " + getfnameQ + upshift + "; }");
												if (!ro)
												{
													writer.WriteLine("set { " + setfnameQ + " = value; }");
												}
											}
											else
											{
												if (f.TopLevelParent != null && f.TopLevelParent.Name == "CL-CONTROL-PARM")
												{
													writer.WriteLine("get { return ClControlParm == null ? \"\" : " + getfnameQ + ".ToString(); }");
												}
												else
												{
													if (!ro)
													{
														upshift = ".Trim()" + upshift;
													}
													if (o.Pic != null)
													{
														writer.WriteLine("get { return " + getfnameQ + ".ToString(\"" + o.Pic.PicFormat.PictureClause + "\")" + upshift + "; }");
													}
													else
													{
														writer.WriteLine("get { return " + getfnameQ + ".ToString()" + upshift + "; }");
													}
												}
												if (!ro)
												{
													writer.WriteLine("set { " + setfnameQ + ".Set(value); }");
												}
											}
										}
										writer.WriteLine();

										if (Cobol.IsFieldTurnTargetWithAttibute(o, "PROTECTED") || Cobol.IsFieldTurnTargetWithAttibute(o, "UNPROTECTED"))
										{
											writer.WriteIndent();
											writer.Write("private bool _");
											writer.Write(pname);
											writer.Write("IsEnabledBinding = ");
											writer.Write(o.IsProtected ? "false" : "true");
											writer.Write(";");
											writer.WriteLine();

											writer.WriteIndent();
											writer.Write("public bool ");
											writer.Write(pname);
											writer.Write("IsEnabledBinding");
											writer.WriteLine();
											using (writer.Indent())
											{
												writer.WriteLine("get { return _" + pname + "IsEnabledBinding; }");
												writer.WriteLine("set { _" + pname + "IsEnabledBinding = value; RaisePropertyChanged(\"" + pname + "IsEnabledBinding\"); }");
											}
											writer.WriteLine();
										}
									}
								}
								
								if (Cobol.IsFieldTurnTargetWithAttibute(o, "HIDDEN"))
								{
									string pname = CsFieldNameConverter.Convert(o.Name);
									writer.WriteIndent();
									writer.Write("private Visibility _");
									writer.Write(pname);
									writer.Write("VisibilityBinding = " + (o.IsHidden ? "Visibility.Hidden;" : "Visibility.Visible;"));
									writer.WriteLine();

									writer.WriteIndent();
									writer.Write("public Visibility ");
									writer.Write(pname);
									writer.Write("VisibilityBinding");
									writer.WriteLine();
									using (writer.Indent())
									{
										writer.WriteLine("get { return _" + pname + "VisibilityBinding; }");
										writer.WriteLine("set { _" + pname + "VisibilityBinding = value; RaisePropertyChanged(\"" + pname + "VisibilityBinding\"); }");
									}
									writer.WriteLine();
								}
							}
							writer.WriteLine("#endregion");
						}

						writer.WriteLine("#endregion");
						writer.WriteLine();
					}

					if (Cobol.Environment.InputOutputSection != null && Cobol.Environment.InputOutputSection.FileControlSection != null)
					{
						foreach (var v in Cobol.Environment.InputOutputSection.FileControlSection.Selects)
						{
							string name = CsFieldNameConverter.Convert(v.MessageName);
							writer.WriteLine("private IoRecord " + name + ";");
						}
						if (Cobol.Environment.InputOutputSection.FileControlSection.UsesDollarReceive())
						{
							writer.WriteLine("private IoRecord _receiveIn;");
						}

						writer.WriteLine();
					}

					/// Write cursors as class members
					foreach (var sub in Cobol.Procedure.SubRoutines)
					{
						if (sub.IsReferenced)
						{
							WriteCursors(writer, sub.Verbs);
						}
					}

					if (cursoIdx.Count > 0)
					{
						writer.WriteLine();
					}

					if (Cobol.Data.ScreenRecord == null)
					{
						writer.WriteLine("private I" + vwriter.DataLayerName + " DataAccess");
						using (writer.Indent())
						{
							writer.WriteLine("get; set;");
						}
						writer.WriteLine();
					}

					IList<string> tableIndexes = Cobol.Data.Data.GetTableIndexes();
					foreach (var s in tableIndexes)
					{
						writer.WriteLine("private long " + CsFieldNameConverter.Convert(s) + " = 0;");
					}
					if (tableIndexes.Count > 0)
					{
						writer.WriteLine();
					}

					if (Cobol.Procedure.ListVerbs<Send>().Count > 0)
					{
						writer.WriteLine("// Holds the reply code from SEND");
						writer.WriteLine("private long TerminationStatus = 0;");
						writer.WriteLine("///TODO: Unused");
						writer.WriteLine("private long TerminationSubstatus = 0;");
						writer.WriteLine();
					}

					if (Cobol.Data.ScreenRecord != null)
					{
						// constructor
						writer.WriteLine("///TODO: Set menu screen number in base() call");
						writer.WriteLine("public " + CsFieldNameConverter.Convert(Cobol.ProgramId) + "Program()");
						writer.WriteLine(": base(100)");
						using (writer.Indent())
						{
							if (Wpf && Cobol.Data.ScreenRecord != null)
							{
								writer.WriteLine("#region data binding redirects");
								writer.WriteLine();

								Dictionary<string, string> sidx = new Dictionary<string, string>();
								foreach (Screen s in Cobol.Data.ScreenRecord.Screens)
								{
									if (s.Fields.Count > 0)
									{
										writer.WriteLine("//");
										writer.WriteLine("// SCREEN: " + s.Name);
										writer.WriteLine("//");
									}

									foreach (var o in s.Fields)
									{
										IExpr expr = null;
										if (o.FromExpr != null)
										{
											expr = o.FromExpr;
										}
										else if (o.ToExpr != null)
										{
											expr = o.ToExpr;
										}
										else if (o.UsingExpr != null)
										{
											expr = o.UsingExpr;
										}

										if (expr != null)
										{
											Offset f = IExprToOffset(expr);
											if (f == null)
											{
												writer.WriteLine();
												writer.WriteLine("///");
												writer.WriteLine("///TODO: Can't locate field " + expr.ToString());
												writer.WriteLine("///");
												writer.WriteLine();
												continue;
											}

											string fname = IExprToFq(expr);

											// Comment out to allow dupliate fields, this
											// helps to create a seperate server for each
											// screen
											if (sidx.ContainsKey(fname))
											{
												continue;
											}

											sidx.Add(fname, fname);

											if (fname.StartsWith("ClControlParm."))
											{
												continue;
											}

											if (Cobol.IsNative(f))
											{
												// Just need to call back
												int dotPos = fname.LastIndexOf('.');
												string pfname = dotPos > -1 ? fname.Substring(0, dotPos) : fname;
												writer.WriteLine(pfname + ".RegisterNotifyPropertyChangedOnChild(this, \"" + CsFieldNameConverter.Convert(f.Name) + "Binding" + "\");");
											}
											else
											{
												if (o.OccuresOnLines > 1)
												{
													for (int x = 0; x < o.OccuresOnLines * (o.Occures == null ? 1 : o.Occures.Columns); x++)
													{
														writer.WriteLine(GuessAtGroupOffset(expr, x) + ".RegisterNotifyPropertyChanged(this, \"" + CsFieldNameConverter.Convert(f.Name) + x + "Binding" + "\");");
													}
												}
												else
												{
													writer.WriteLine(fname + ".RegisterNotifyPropertyChanged(this, \"" + CsFieldNameConverter.Convert(f.Name) + "Binding" + "\");");
												}
											}
										}
									}
									writer.WriteLine();
								}

								writer.WriteLine("#endregion");
								writer.WriteLine();
							}

							writer.WriteLine("if (IsInDesignMode)");
							{
								using (writer.Indent())
								{
									foreach (var o in Cobol.Data.WorkingStorage.Data.Fields)
									{
										if (o.Redefines == null)
										{
											if (!Cobol.IsNative(o) && o.Name != "CL-CONTROL-PARM")
											{
												writer.WriteLine(CsFieldNameConverter.Convert(o.Name, o.SpecialNameHandling) + ".ClearTo('0');");
											}
										}
									}
								}
							}
						}

						writer.WriteLine();
						writer.WriteLine("public override void Initialize(IContext ctx, IScreen screenDef, IScreenConfig cfg)");
						using (writer.Indent())
						{
							writer.WriteLine("ClControlParm = new Clibcl.ClControlParm(ctx.Session);");
							writer.WriteLine("ClControlParm.ClControlParmGrp.ClInformation.ClInformationGrp.ClAdvisoryGrp.ClAdvisoryMessage.RegisterNotifyPropertyChanged(this, \"ClAdvisoryMessageBinding\");");
							writer.WriteLine("ClControlParm.ClControlParmGrp.ClInformation.ClInformationGrp.ClScreenInformationLine.RegisterNotifyPropertyChanged(this, \"ClScreenInformationLineBinding\");");
							writer.WriteLine("ClControlParm.ClControlParmGrp.ClInformation.ClInformationGrp.ClText.RegisterNotifyPropertyChanged(this, \"ClTextBinding\");");
							writer.WriteLine();
							writer.WriteLine("Init(ClControlParm.ClControlParmGrp.ClInformation.ClInformationGrp.ClDestPgmfId, ClControlParm.ClControlParmGrp.ClInformation.ClInformationGrp.ClAdvisoryGrp.ClAdvisoryMessage, WsClSwitches.WsExitRequesterSwt, ClControlParm.ClControlParmGrp.ClInformation.ClInformationGrp.ClAdvisoryGrp.ClAdvisoryInd);");
							writer.WriteLine();
							writer.WriteLine("base.Initialize(ctx, screenDef, cfg);");
						}
						writer.WriteLine();

						writer.WriteLine("private void HideScreens()");
						using (writer.Indent())
						{
							foreach (var s in Cobol.Data.ScreenRecord.Screens)
							{
								writer.WriteLine(CsFieldNameConverter.Convert(s.Name) + "Visibility = Visibility.Hidden;");
							}
							writer.WriteLine();

							writer.WriteLine("foreach (var d in _turnTemps)");
							writer.WriteLine("{");
							writer.WriteLine("	d();");
							writer.WriteLine("}");
							writer.WriteLine("_turnTemps.Clear();");
						}
						writer.WriteLine();
					}

					bool foundSomething = false;
					int count = 0;
					foreach (var sub in Cobol.Procedure.SubRoutines)
					{
						if (! sub.IsReferenced)
						{
							continue;
						}
						if 
						(
							sub.Name.Equals("9700c-Screen-Print", StringComparison.InvariantCultureIgnoreCase) ||
							sub.Name.Equals("9710c-Timeout", StringComparison.InvariantCultureIgnoreCase) ||
							sub.Name.Equals("9720c-Timeout-Check", StringComparison.InvariantCultureIgnoreCase) ||
							sub.Name.Equals("9730c-Invalid-Key", StringComparison.InvariantCultureIgnoreCase) ||
							sub.Name.Equals("9740c-Application-Help", StringComparison.InvariantCultureIgnoreCase) ||
							sub.Name.Equals("9800c-Prev-Function", StringComparison.InvariantCultureIgnoreCase) ||
							sub.Name.Equals("9810c-Menu-Goto-Function", StringComparison.InvariantCultureIgnoreCase) ||
							sub.Name.Equals("9820c-Swap-Next-Function", StringComparison.InvariantCultureIgnoreCase) ||
							sub.Name.Equals("9830c-System-Menu", StringComparison.InvariantCultureIgnoreCase)
						)
						{
							continue;
						}

						if (count++ > 0)
						{
							writer.WriteLine();
						}

						string comment = sub.CommentProbe();
						if (!String.IsNullOrEmpty(comment))
						{
							writer.WriteLine("/*");
							writer.WriteLine(comment.Replace("/*", "").Replace("*/", "").Replace("\n", "\n\t\t\t"));
							writer.WriteLine("*/");
						}

						if (count == 1)
						{
							if
							(
								Cobol.Environment.InputOutputSection != null &&
								Cobol.Environment.InputOutputSection.FileControlSection != null &&
								Cobol.Environment.InputOutputSection.FileControlSection.UsesDollarReceive()
							)
							{
								writer.WriteLine
								(
									"public override IBufferOffset " +
									"Main" +
									"(IBufferOffset data, ILogger logger, ISqlDataAccess connection)"
								);
								writer.WriteLine("{");
								writer.WriteLine("\t///TODO: Change to Main(data, logger, new " + vwriter.DataLayerName + "Java() prior to production");
								writer.WriteLine("\treturn Main(data, logger, new " + vwriter.DataLayerName + "(connection));");
								writer.WriteLine("}");
								writer.WriteLine();

								writer.WriteLine
								(
									"public IBufferOffset " +
									"Main" +
									"(IBufferOffset data, ILogger logger, I" + vwriter.DataLayerName + " connection)"
								);
							}
							else
							{
								if (Cobol.Procedure.UsingArguments != null && Cobol.Data.ScreenRecord == null)
								{
									writer.WriteLine("protected override void Main(");

									for (int x = 0; x < Cobol.Procedure.UsingArguments.Items.Count; x++)
									{
										writer.WriteIndent();
										writer.Write("\t");
										if (x > 0)
										{
											writer.Write(", ");
										}
										if (Cobol.IsNative(Cobol.Procedure.UsingArguments.Items[x]))
										{
											writer.Write(Cobol.CsClrTypeNmae(Cobol.Procedure.UsingArguments.Items[x]));
										}
										else
										{
											vwriter.Write(Cobol.Procedure.UsingArguments.Items[x]);
										}
										writer.Write(" _");
										vwriter.Write(Cobol.Procedure.UsingArguments.Items[x]);
										writer.WriteLine();
									}
									writer.WriteLine(")");
								}
								else
								{
									writer.WriteLine("protected override void Main()");
								}
							}
						}
						else
						{
							writer.WriteLine("private void " + CsFieldNameConverter.Convert(sub.Name) + "()");
						}

						using (writer.Indent())
						{
							if (count == 1)
							{
								if
								(
									Cobol.Environment.InputOutputSection != null &&
									Cobol.Environment.InputOutputSection.FileControlSection != null &&
									Cobol.Environment.InputOutputSection.FileControlSection.UsesDollarReceive()
								)
								{
									writer.WriteLine("base.Main(data, logger, null);");
									writer.WriteLine("DataAccess = connection;");
									writer.WriteLine();
								}

								writer.WriteLine("try");
								writer.IndentManual();

								if (Cobol.Procedure.UsingArguments != null && Cobol.Data.ScreenRecord == null)
								{
									for (int x = 0; x < Cobol.Procedure.UsingArguments.Items.Count; x++)
									{
										writer.WriteIndent();
										vwriter.Write(Cobol.Procedure.UsingArguments.Items[x]);
										writer.Write(" = _");
										vwriter.Write(Cobol.Procedure.UsingArguments.Items[x]);
										writer.Write(";");
										writer.WriteLine();
									}
									if (Cobol.Procedure.UsingArguments.Items.Count > 0)
									{
										writer.WriteLine();
									}
								}

								foreach (var o in Cobol.Data.WorkingStorage.Data.Fields)
								{
									if (o.Redefines != null)
									{
										INamedField fo = Cobol.Data.WorkingStorage.Data.LocateField(o.Redefines.RedefinesOffsetNamed, null);
										string ons = GetNamespace((Offset)fo);

										writer.WriteLine
										(
											CsFieldNameConverter.Convert(o.Name, o.SpecialNameHandling) +
											" = new " +
											CsFieldNameConverter.Convert(o.Name, o.SpecialNameHandling) +
											"(" +
											(String.IsNullOrEmpty(ons) ? "" : ons + ".") +
											CsFieldNameConverter.Convert(o.Redefines.RedefinesOffsetNamed) + ".Buffer" +
											");"
										);
									}
								}

								if
								(
									Cobol.Environment.InputOutputSection != null &&
									Cobol.Environment.InputOutputSection.FileControlSection != null
								)
								{
									foreach (var v in Cobol.Environment.InputOutputSection.FileControlSection.Selects)
									{
										string name = CsFieldNameConverter.Convert(v.MessageName);
										Fd fd = Cobol.Data.Files.FindFdFor(v.MessageName);

										StringBuilder buf = new StringBuilder("new WsRecord[] {");
										for (int x = 0; x < fd.DataRecords.Count; x++)
										{
											if (x > 0)
											{
												buf.Append(", ");
											}
											if (!String.IsNullOrEmpty(fd.DataRecords[x].FileSectionMessageName))
											{
												buf.Append(CsFieldNameConverter.Convert(fd.DataRecords[x].FileSectionMessageName));
											}
											else
											{
												string nssub = GetNamespace(fd.DataRecords[x]);
												if (!String.IsNullOrEmpty(nssub))
												{
													buf.Append(nssub + ".");
												}
											}

											buf.Append(CsFieldNameConverter.Convert(fd.DataRecords[x].Name, fd.DataRecords[x].SpecialNameHandling));
										}
										buf.Append("}");

										writer.WriteLine(name + " = new IoRecord(" + buf.ToString() + ", \"" + v.FileName + "\", " + CsFieldNameConverter.Convert(v.StatusFunction) + ");");

										// These error handlers aren't necessary since Exception's will be thrown
										//if (Cobol.Procedure.DeclareSection != null)
										//{
										//    DeclareAfterError e = Cobol.Procedure.DeclareSection.Find(v.MessageName);
										//    if (e != null)
										//    {
										//        writer.WriteLine(name + ".IoAfterErrorHandler = delegate()");
										//        using (writer.Indent())
										//        {
										//            vwriter.Write(e.Stmts);
										//        }
										//        writer.WriteLine(";");
										//    }
										//}
									}
									writer.WriteLine();
								}

								foreach (var v in Cobol.Data.Verbs)
								{
									vwriter.Write(v);
									CsSql.Count++;
								}
							}

							foreach (IVerb v in sub.Verbs)
							{
								if (!foundSomething)
								{
									foundSomething = count > 1 || v is Perform;

									if (foundSomething)
									{
										Cobol.MainSub = CsFieldNameConverter.Convert(sub.Name);
									}
								}
								vwriter.Write(v);
							}

							// csproj files.
							_referencedProjects.AddRange(vwriter.ReferencedProjects);

							if (!foundSomething)
							{
								writer.WriteLine();
								Cobol.MainSub = CsFieldNameConverter.Convert(Cobol.Procedure.SubRoutines[1].Name);
								writer.WriteLine(Cobol.MainSub + "();");
								writer.WriteLine();
							}

							if (count == 1)
							{
								writer.Unindent();
								writer.WriteLine("catch (AbortRunException)");
								writer.WriteLine("{");
								writer.WriteLine("\t//Do nothing");
								writer.WriteLine("}");
							}

							if (count == 1 && Cobol.Data.ScreenRecord != null)
							{
								writer.WriteLine();
								writer.WriteLine("ExitRequester(ClControlParm.ClControlParmGrp.ClInformation.ClInformationGrp.ClDestPgmfId, ClControlParm.ClControlParmGrp.ClInformation.ClInformationGrp.ClText);");
							}

							if (Cobol.Environment.InputOutputSection != null)
							{
								if
								(
									count == 1 &&
									Cobol.Environment.InputOutputSection.FileControlSection != null &&
									Cobol.Environment.InputOutputSection.FileControlSection.UsesDollarReceive()
								)
								{
									writer.WriteLine("return " + CsFieldNameConverter.Convert(Cobol.Environment.InputOutputSection.FileControlSection.ReplyMessageName()) + ".Outputs[0];");
								}
							}
						}
					}
				}
			}

			writer.Close();
		}

		private void WriteCsproj(string progDir, string propDir)
		{
			string csproj = Path.Combine(progDir, progDir + ".csproj");
			if (File.Exists(csproj))
			{
				File.Delete(csproj);
			}
			CsProjectName = csproj;
			
			StreamWriter writer = new StreamWriter(File.OpenWrite(csproj));

			string name = RootNameSpace(null);
			CsProjFile csprojObj = new CsProjFile(name, false);

			foreach (string fn in OutputFileNames)
			{
				csprojObj.CompileFiles.Add(Path.GetFileName(fn));
			}

			if 
			(
				Cobol.Environment.InputOutputSection != null &&
				Cobol.Data.Files != null
			)
			{
				csprojObj.CompileFiles.Add("I" + name + "Data.cs");
				csprojObj.CompileFiles.Add(name + "Data.cs");
				csprojObj.CompileFiles.Add(name + "JavaData.cs");
			}

			csprojObj.ProjectReferences.Add("..\\..\\..\\..\\SharedSources\\DOR.Core\\DOR.Core\\DOR.Core.csproj");
			csprojObj.ProjectReferences.Add("..\\..\\..\\..\\SharedSources\\DOR.WorkingStorage\\DOR.WorkingStorage\\DOR.WorkingStorage.csproj");

			foreach (string fn in _referencedProjects)
			{
				csprojObj.ProjectReferences.Add("..\\" + fn);
			}

			writer.WriteLine(csprojObj.ToXml());

			writer.Close();

			string sln = Path.Combine(progDir, progDir + ".sln");
			if (!File.Exists(sln))
			{
				writer = new StreamWriter(File.OpenWrite(sln));

				writer.WriteLine("Microsoft Visual Studio Solution File, Format Version 11.00");
				writer.WriteLine("# Visual Studio 2010");
				writer.WriteLine("Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"" + progDir + "\", \"" + progDir + ".csproj\", \"{" + csprojObj.ProjectConfig.ProjectGuid.ToString() + "}\"");
				writer.WriteLine("EndProject");
				writer.WriteLine("Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"DOR.Core\", \"c:\\sources\\SharedSources\\DOR.Core\\DOR.Core\\DOR.Core.csproj\", \"{9B060CA2-4185-49A5-84E8-417FBD18B342}\"");
				writer.WriteLine("EndProject");
				foreach (string fn in _referencedProjects)
				{
					writer.WriteLine("Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"" + fn.Substring(0, fn.IndexOf("\\")) + "\", \"..\\" + fn + "\", \"{9B060CA2-4185-49A5-84E8-417FBD18B342}\"");
					writer.WriteLine("EndProject");
				}
				writer.WriteLine("Global");

				writer.WriteLine("	GlobalSection(TeamFoundationVersionControl) = preSolution");
				writer.WriteLine("		SccNumberOfProjects = 1");
				writer.WriteLine("		SccEnterpriseProvider = {4CA58AB2-18FA-4F8D-95D4-32DDF27D184C}");
				writer.WriteLine("		SccTeamFoundationServer = http://dorprodtfs:8080/");
				writer.WriteLine("		SccProjectUniqueName0 = ..\\\\..\\\\..\\\\..\\\\..\\\\SharedSources\\\\DOR.Core\\\\DOR.Core\\\\DOR.Core.csproj");
				writer.WriteLine("		SccProjectName0 = ../../../../../DOR.Core/DOR.Core");
				writer.WriteLine("		SccAuxPath0 = http://dorprodtfs:8080");
				writer.WriteLine("		SccLocalPath0 = c:\\\\sources\\\\SharedSources\\\\DOR.Core\\\\DOR.Core");
				writer.WriteLine("		SccProvider0 = {4CA58AB2-18FA-4F8D-95D4-32DDF27D184C}");
				writer.WriteLine("	EndGlobalSection");
				writer.WriteLine("	GlobalSection(SolutionProperties) = preSolution");
				writer.WriteLine("		HideSolutionNode = FALSE");
				writer.WriteLine("	EndGlobalSection");
				writer.WriteLine("EndGlobal");
			}
			writer.Close();
		}

		//private void WriteTestMain()
		//{
		//    string filename = SourceDirectory + "Program.cs";
		//    OutputFileNames.Add(filename);
		//    if (File.Exists(filename))
		//    {
		//        //File.Delete(filename);
		//        return;
		//    }

		//    SourceWriter writer = new SourceWriter(File.CreateText(filename));
		//    WriteHeader(writer, RootNameSpace(null), null);

		//    Fd replyFd = null;
		//    string reply = null;

		//    if 
		//    (
		//        Cobol.Environment.InputOutputSection != null &&
		//        Cobol.Data.Files != null
		//    )
		//    {
		//        replyFd = Cobol.Data.Files.FindFdFor(Cobol.Environment.InputOutputSection.FileControlSection.ReplyMessageName());
		//        if (replyFd != null)
		//        {
		//            reply = CsFieldNameConverter.Convert(replyFd.DataRecords[0].Name, replyFd.DataRecords[0].SpecialNameHandling);
		//        }
		//    }

		//    using (writer.Indent())
		//    {
		//        writer.WriteLine("class Program");
		//        using (writer.Indent())
		//        {
		//            writer.WriteLine("static void Main(string[] args)");
		//            using (writer.Indent())
		//            {
		//                writer.WriteLine("using (var s = new " + CsFieldNameConverter.Convert(Cobol.ProgramId) + "Program())");
		//                using (writer.Indent())
		//                {
		//                    Fd fdReceive = null;

		//                    if (Cobol.Data.Files != null)
		//                    {
		//                        foreach (var fd in Cobol.Data.Files.Fds())
		//                        {
		//                            if (fd.MessageName.EndsWith("-IN"))
		//                            {
		//                                fdReceive = fd;
		//                                break;
		//                            }
		//                        }
		//                    }

		//                    CsVerbs vwriter = new CsVerbs(Cobol, writer);
		//                    if (fdReceive == null)
		//                    {
		//                        if (Cobol.Procedure.UsingArguments != null)
		//                        {
		//                            for (int x = 0; x < Cobol.Procedure.UsingArguments.Items.Count; x++)
		//                            {
		//                                writer.WriteIndent();
		//                                writer.Write("var _");
		//                                vwriter.Write(Cobol.Procedure.UsingArguments.Items[x]);
		//                                if (Cobol.IsNative(Cobol.Procedure.UsingArguments.Items[x]))
		//                                {
		//                                    writer.Write(" = \"\";");
		//                                }
		//                                else
		//                                {
		//                                    writer.Write(" = new ");
		//                                    vwriter.Write(Cobol.Procedure.UsingArguments.Items[x]);
		//                                    writer.Write("();");
		//                                }
		//                                writer.WriteLine();
		//                            }
		//                        }
		//                    }
		//                    else
		//                    {
		//                        writer.WriteLine("var data = new " + CsFieldNameConverter.Convert(fdReceive.DataRecords[0].Name, fdReceive.DataRecords[0].SpecialNameHandling) + "();");
		//                        writer.WriteLine();

		//                        foreach (var o in fdReceive.DataRecords[0].SubGroups)
		//                        {
		//                            writer.WriteLine("data." + CsFieldNameConverter.Convert(o.Name, o.SpecialNameHandling) + ".Set(0);");
		//                        }

		//                        writer.WriteLine();
		//                    }

		//                    writer.WriteLine("try");
		//                    using (writer.Indent())
		//                    {
		//                        if (fdReceive == null)
		//                        {
		//                            writer.WriteIndent();

		//                            writer.Write("Write(s." + CsFieldNameConverter.Convert(Cobol.Procedure.MainSubName()) + "(");
		//                            if (Cobol.Procedure.UsingArguments != null)
		//                            {
		//                                for (int x = 0; x < Cobol.Procedure.UsingArguments.Items.Count; x++)
		//                                {
		//                                    if (x > 0)
		//                                    {
		//                                        writer.Write(", ");
		//                                    }
		//                                    writer.Write("_");
		//                                    writer.Write(CsFieldNameConverter.Convert(Cobol.Procedure.UsingArguments.Items[x].ToString()));
		//                                }
		//                            }
		//                            writer.Write("));");

		//                            writer.WriteLine();
		//                        }
		//                        else
		//                        {
		//                            writer.WriteLine("Write(s." + CsFieldNameConverter.Convert(Cobol.Procedure.SubRoutines[0].Name) + "(data));");
		//                        }
		//                    }

		//                    writer.WriteLine("catch (AbortRunException arex)");
		//                    using (writer.Indent())
		//                    {
		//                        writer.WriteLine("if ((object)arex.OutRecord != null)");
		//                        using (writer.Indent())
		//                        {
		//                            writer.WriteLine("Write(arex.OutRecord);");
		//                        }
		//                        writer.WriteLine("else");
		//                        using (writer.Indent())
		//                        {
		//                            writer.WriteLine("Console.WriteLine(\"STOP\");");
		//                        }
		//                    }

		//                    writer.WriteLine("Console.WriteLine(\"PRESS RETURN TO EXIT\");");
		//                    writer.WriteLine("Console.ReadKey();");
		//                }
		//            }

		//            writer.WriteLine();
		//            writer.WriteLine("private static void Write(IBufferOffset msg)");
		//            using (writer.Indent())
		//            {
		//                writer.WriteLine("Console.WriteLine(msg.ToDebugPrintString());");
		//                writer.WriteLine("Console.WriteLine();");
		//            }
		//        }
		//    }

		//    writer.Close();
		//}

		private string OffsetFq(string vname, string parent)
		{
			INamedField f = Cobol.Data.LocateField(vname, parent);
			if (f == null)
			{
				return CsFieldNameConverter.Convert(vname);
			}

			string name = CsFieldNameConverter.Convert(f.Name);
			f = f.Parent;

			while (f != null)
			{
				name = CsFieldNameConverter.Convert(f.Name) + "." + name;
				f = f.Parent;
			}

			return name;
		}

		private string ITermToFq(ITerm iterm)
		{
			if (iterm is Id)
			{
				if (((Id)iterm).Offsets.Count != 0)
				{
					throw new NotImplementedException();
				}
				return OffsetFq(((Id)iterm).Value.Str, null);
			}
			else if (iterm is OffsetReference)
			{
				return OffsetFq(((OffsetReference)iterm).OffsetChain[0].Value.Str, ((OffsetReference)iterm).OffsetChain[((OffsetReference)iterm).OffsetChain.Count - 1].Value.Str);
			}
			else
			{
				throw new Exception();
			}
		}

		private string IExprToFq(IExpr iexpr)
		{
			if (iexpr is Expr)
			{
				return ITermToFq(((Expr)iexpr).Terms[0]);
			}
			else
			{
				throw new Exception();
			}
		}

		private Offset IExprToOffset(IExpr iexpr)
		{
			ITerm iterm = ((Expr)iexpr).Terms[0];
			if (iterm is Id)
			{
				if (((Id)iterm).Offsets.Count != 0)
				{
					throw new NotImplementedException();
				}
				return (Offset)Cobol.Data.LocateField(((Id)iterm).Value.Str, null);
			}
			else if (iterm is OffsetReference)
			{
				return (Offset)Cobol.Data.LocateField(((OffsetReference)iterm).OffsetChain[0].Value.Str, ((OffsetReference)iterm).OffsetChain[((OffsetReference)iterm).OffsetChain.Count - 1].Value.Str);
			}
			else
			{
				throw new Exception();
			}
		}

		private void WriteScreenSection()
		{
			if (Cobol.Data.ScreenRecord == null || Cobol.Data.ScreenRecord.Screens == null)
			{
				return;
			}

			if (!Wpf)
			{
				Console.WriteLine("Skipping screens, use --wpf flag to generate screens");
				return;
			}

			string filename = Path.Combine(Cobol.SourceDirectory, "Screens.xaml");
			OutputFileNames.Add(filename);
			if (File.Exists(filename))
			{
				File.Delete(filename);
			}

			string ns = RootNameSpace(null) + "Req";
			string cls = ns + ".Screens";

			using (StreamWriter writer = new StreamWriter(File.OpenWrite(filename)))
			{
				WriteWpfHeader(writer, ns, cls);

				foreach (Screen s in Cobol.Data.ScreenRecord.Screens)
				{
					if (s.IsOverlay)
					{
						continue;
					}

					WriteWpfScreen(writer, s, ns, cls);
				}

				WriteWpfFooter(writer, ns, cls);
				writer.Close();
			}

			filename = Path.Combine(Cobol.SourceDirectory, "Screens.xaml.cs");
			OutputFileNames.Add(filename);
			if (File.Exists(filename))
			{
				File.Delete(filename);
			}

			using (StreamWriter cbh = new StreamWriter(File.OpenWrite(filename)))
			{
				WriteWpfCodeBehind(cbh, Cobol.Data.ScreenRecord.Screens[0].PgmfId, ns, cls);
			}
		}

		private void WriteWpfHeader(TextWriter writer, string ns, string cls)
		{
			writer.WriteLine("<cobol:CobolScreenBase x:Class=\"" + cls + "\"");
			writer.WriteLine("\txmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"");
			writer.WriteLine("\txmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"");
			writer.WriteLine("\txmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\"");
			writer.WriteLine("\txmlns:d=\"http://schemas.microsoft.com/expression/blend/2008\"");
			writer.WriteLine("\tmc:Ignorable=\"d\"");
			writer.WriteLine("\td:DesignHeight=\"400\"");
			writer.WriteLine("\td:DesignWidth=\"700\"");
			writer.WriteLine("\tWidth=\"auto\"");
			writer.WriteLine("\tHeight=\"auto\"");
			writer.WriteLine("\txmlns:wpf=\"clr-namespace:DOR.WPF;assembly=DOR.WPF\"");
			writer.WriteLine("\txmlns:cobol=\"clr-namespace:DOR.WorkingStorage.Wpf;assembly=DOR.WorkingStorage\"");
			writer.WriteLine("\txmlns:core=\"clr-namespace:DOR.Core;assembly=DOR.Core\"");
			writer.WriteLine("\txmlns:viewmodel=\"clr-namespace:" + RootNameSpace(null) + "\"");
			writer.WriteLine("\tFontSize=\"14\" FontFamily=\"Courier New\"");
			writer.WriteLine(">");

			writer.WriteLine("<UserControl.Resources>");
			writer.WriteLine("	<ObjectDataProvider ObjectType=\"{x:Type viewmodel:" + CsFieldNameConverter.Convert(Cobol.ProgramId) + "Program}\" x:Key=\"ViewModelData\"></ObjectDataProvider>");
			writer.WriteLine("</UserControl.Resources>");

			writer.WriteLine("<Grid DataContext=\"{Binding Source={StaticResource ViewModelData}}\" Name=\"GrdScreen\" KeyUp=\"GrdScreen_KeyUp\">");
			writer.WriteLine("	<StackPanel Margin=\"30,10,30,5\">");
			writer.WriteLine("		<TextBlock Name=\"TxtError\" Visibility=\"{Binding Path=ErrorVisibility}\" Foreground=\"Red\" Grid.Row=\"0\" Grid.ColumnSpan=\"4\" Margin=\"20,0,20,3\" Text=\"{Binding Path=ErrorMessage}\"></TextBlock>");
			writer.WriteLine("		<Canvas>");
		}

		private void WriteWpfFooter(TextWriter writer, string ns, string cls)
		{
			// Write overlays as popups on all screens
			foreach (Screen ovr in Cobol.Data.ScreenRecord.Screens)
			{
				if (!ovr.IsOverlay)
				{
					continue;
				}
				writer.WriteLine("<Canvas Visibility=\"{Binding Path=" + CsFieldNameConverter.Convert(ovr.Name) + "Visibility}\" Name=\"" + CsFieldNameConverter.Convert(ovr.Name) + "\" Canvas.Left=\"{Binding Path=" + CsFieldNameConverter.Convert(ovr.Name) + "Col}\" Canvas.Top=\"{Binding Path=" + CsFieldNameConverter.Convert(ovr.Name) + "Row}\" >");
				WriteWpfScreenFields(writer, ovr, ns, cls);
				writer.WriteLine("</Canvas>");
			}
			writer.WriteLine("		</Canvas>");
			writer.WriteLine("	</StackPanel>");
			writer.WriteLine("</Grid>");
			writer.WriteLine("</cobol:CobolScreenBase>");
		}

		private void WriteWpfScreenFields(TextWriter writer, Screen s, string ns, string cls)
		{
			string screenCsName = CsFieldNameConverter.Convert(s.Name);

			foreach (var f in s.Fields)
			{
				if (f.X <= 0 || f.Y <= 0)
				{
					// group record
					continue;
				}

				if (!String.IsNullOrEmpty(f.Comments))
				{
					writer.WriteLine("<!-- " + f.Comments + " -->");
				}

				IExpr getexp = null;
				if (f.FromExpr != null)
				{
					getexp = f.FromExpr;
					Debug.Assert(f.UsingExpr == null);
				}
				else if (f.UsingExpr != null)
				{
					getexp = f.UsingExpr;
					Debug.Assert(f.FromExpr == null);
					Debug.Assert(f.ToExpr == null);
				}
				else if (f.ToExpr != null)
				{
					getexp = f.ToExpr;
					Debug.Assert(f.FromExpr == null);
					Debug.Assert(f.FromExpr == null);
				}

				INamedField getwsf = null;
				if (getexp != null)
				{
					Debug.Assert(getexp is Expr);
					Expr expr = (Expr)getexp;
					Debug.Assert(expr.Terms.Count == 1);
					ITerm term = expr.Terms[0];

					if (term is OffsetReference)
					{
						getwsf = Cobol.Data.LocateField(((OffsetReference)term).OffsetChain[0].ToString(), ((OffsetReference)term).OffsetChain[((OffsetReference)term).OffsetChain.Count - 1].ToString());
					}
					else
					{
						getwsf = Cobol.Data.LocateField(term.ToString(), null);
					}
				}

				string defValue = f.DefaultValue == null ? null : f.DefaultValue.ToString();
				if (f.DefaultValue is ValueList)
				{
					defValue = StringHelper.StripQuotes(((ValueList)f.DefaultValue).Items[0].ToString());
				}
				if (defValue == null)
				{
					if (getwsf != null && getwsf.Attributes != null && getwsf.Attributes.DefaultValue != null)
					{
						defValue = getwsf.Attributes.DefaultValue.ToString();
					}
					else
					{
						defValue = "";
					}
				}
				if (defValue.Length == 0 || defValue[0] != '"')
				{
					defValue = "\"" + defValue + "\"";
				}
				if (defValue == "\"Spaces.Instance()\"" || defValue == "\"Space.Instance()\"")
				{
					defValue = "\"\"";
				}
				else if (defValue == "\"Zeroes.Instance()\"" || defValue == "\"Zero.Instance()\"")
				{
					defValue = "\"\"";
				}

				int len = 1;
				if (f.RequiredLength != null)
				{
					len = Int32.Parse(f.RequiredLength.ToString());
				}
				else if (f.Pic != null && f.Pic.PicFormat != null)
				{
					len = f.Pic.PicFormat.DisplayLength;
				}
				else if (getwsf != null && getwsf.Attributes != null)
				{
					if (getwsf.Attributes.Pic != null)
					{
						len = getwsf.Attributes.Pic.PicFormat.DisplayLength;
					}
				}
				else if (defValue.Length > 2)
				{
					len = defValue.Length - 2;
				}

				string textBlockAttribs = "";
				if (f.IsUnderline)
				{
					textBlockAttribs += " TextDecorations=\"Underline\"";
				}
				if (f.IsReverse)
				{
					textBlockAttribs += " Foreground=\"White\" Background=\"Black\"";
				}
				if (f.IsDim)
				{
					textBlockAttribs += " Foreground=\"Navy\"";
				}
				if (f.IsDimReverse)
				{
					textBlockAttribs += " Foreground=\"White\" Background=\"Navy\"";
				}

				for (int row = 0; row < f.OccuresOnLines; row++)
				{
					for (int col = 0; col < (f.Occures == null ? 1 : f.Occures.Columns); col++)
					{
						int colOffset = f.Occures == null ? 0 : ((col) % f.Occures.Columns) * f.Occures.Skipping;
						int rowOffset = f.Occures == null ? 0 : ((row) % f.Occures.Lines) * f.Occures.Skipping;

						if (f.IsArea)
						{
							continue;
						}
						else if (f.FromExpr != null)
						{
							if (getwsf == null)
							{
								writer.WriteLine("<!-- TODO: field not found " + f.Name + "-->");
								continue;
							}
							Debug.Assert(f.UsingExpr == null);
							getexp = f.FromExpr;
							writer.WriteLine("<TextBlock Canvas.Top=\"" + ((f.Y - 1 + row + rowOffset) * _charHeight) + "\" Canvas.Left=\"" + ((f.X - 1 + colOffset) * _charWidth) + "\" Width=\"" + (int)((len * 1.06) * _charWidth) + "\" Height=\"" + (_charHeight) + "\"" + textBlockAttribs + "");
							writer.WriteLine("\tText=\"{Binding Path=" + CsFieldNameConverter.Convert(getwsf == null ? f.Name : getwsf.Name) + (f.OccuresOnLines > 1 ? (row * f.Occures.Columns + col).ToString() : "") + "Binding}\" ");
							if (f.OccuresOnLines == 1)
							{
								writer.WriteLine("\tName=\"" + screenCsName + CsFieldNameConverter.Convert(f.Name) + "\"");
							}
							else
							{
								writer.WriteLine("\tName=\"" + screenCsName + CsFieldNameConverter.Convert(f.Name) + (row * f.Occures.Columns + col).ToString() + "\"");
							}
							writer.WriteLine(" />");
							continue;
						}
						else if (f.UsingExpr != null)
						{
							getexp = f.UsingExpr;
							Debug.Assert(f.FromExpr == null);
							Debug.Assert(f.ToExpr == null);
						}
						else if (f.ToExpr != null)
						{
							getexp = f.ToExpr;
							Debug.Assert(f.FromExpr == null);
							Debug.Assert(f.UsingExpr == null);
						}
						else
						{
							writer.WriteLine("<TextBlock Canvas.Top=\"" + ((f.Y - 1 + row + rowOffset) * _charHeight) + "\" Canvas.Left=\"" + ((f.X - 1 + colOffset) * _charWidth) + "\" Width=\"" + (int)((len * 1.06) * _charWidth) + "\" Height=\"" + (_charHeight) + "\"" + textBlockAttribs);
							writer.WriteLine("\tText=\"" + StringHelper.XmlEncode(StringHelper.StripQuotes(defValue).Replace("\\\"", "\"")) + "\" />");
							continue;
						}

						if (getexp != null)
						{
							if 
							(
								f.RequiredChoices == null &&
								! getwsf.Has88() &&
								(
									getwsf.Attributes == null ||
									getwsf.Attributes.ValueConstraints.Count == 0
								)
							)
							{
								// text box
								writer.WriteLine("<TextBox FontSize=\"12\" Padding=\"0, 0, 0, 0\"");
								writer.WriteLine("\tCanvas.Top=\"" + ((f.Y - 1 + row + rowOffset) * _charHeight) + "\" Canvas.Left=\"" + ((f.X - 1 + colOffset) * _charWidth) + "\"");
								writer.WriteLine("\tWidth=\"" + (6 + len * _charWidth) + "\" MaxLength=\"" + len + "\" MaxLines=\"1\"");
								writer.WriteLine("\tText=\"{Binding Path=" + CsFieldNameConverter.Convert(getwsf == null ? f.Name : getwsf.Name) + (f.OccuresOnLines > 1 ? (row * f.Occures.Columns + col).ToString() : "") + "Binding}\" ");
								writer.WriteLine("\tPreviewKeyUp=\"TextBox_PreviewKeyUp\"");
								if (Cobol.IsFieldTurnTargetWithAttibute(f, "PROTECTED") || Cobol.IsFieldTurnTargetWithAttibute(f, "UNPROTECTED"))
								{
									writer.WriteLine("\tIsEnabled=\"{Binding Path=" + CsFieldNameConverter.Convert(getwsf == null ? f.Name : getwsf.Name) + (f.OccuresOnLines > 1 ? (row * f.Occures.Columns + col).ToString() : "") + "IsEnabledBinding}\" ");
								}
								if (Cobol.IsFieldTurnTargetWithAttibute(f, "HIDDEN"))
								{
									writer.WriteLine("\tVisibility=\"{Binding Path=" + CsFieldNameConverter.Convert(getwsf == null ? f.Name : getwsf.Name) + (f.OccuresOnLines > 1 ? (row * f.Occures.Columns + col).ToString() : "") + "VisibilityBinding}\" ");
								}
								if (!String.IsNullOrWhiteSpace(f.IsUpshift))
								{
									writer.WriteLine("\tCharacterCasing=\"Upper\" ");
								}
								if (f.WhenFull == "TAB")
								{
									writer.WriteLine("\tKeyUp=\"AutoTab_KeyUp\" ");
								}
								if (f.OccuresOnLines == 1)
								{
									writer.WriteLine("\tName=\"" + screenCsName + CsFieldNameConverter.Convert(f.Name) + "\" GotFocus=\"TextBox_GotFocus\" />");
								}
								else
								{
									writer.WriteLine("\tName=\"" + screenCsName + CsFieldNameConverter.Convert(f.Name) + row + "\" GotFocus=\"TextBox_GotFocus\" />");
								}
							}
							else
							{
								// drop down

								if (f.RequiredLength == null)
								{
									len = f.Pic.Length;
								}
								else
								{
									len = Int32.Parse(f.RequiredLength.ToString());
								}
								writer.WriteLine("<ComboBox Grid.Row=\"" + (f.Y - 1 + row + rowOffset) + "\" Grid.Column=\"" + (f.X - 1 + colOffset) + "\"");
								writer.WriteLine("\tGrid.ColumnSpan=\"" + (len + 3) + "\"");
								writer.WriteLine("\tText=\"{Binding Path=" + CsFieldNameConverter.Convert(getwsf.Name) + "Binding" + (f.OccuresOnLines > 1 ? (row * f.Occures.Columns + col).ToString() : "") + "}\" ");
								writer.WriteLine("\tName=\"" + screenCsName + CsFieldNameConverter.Convert(f.Name) + "\"");
								writer.WriteLine("\tPreviewKeyUp=\"TextBox_PreviewKeyUp\"");
								writer.WriteLine(">");

								if (f.RequiredChoices != null)
								{
									if (f.RequiredChoices is ValueList)
									{
										foreach (var r in ((ValueList)f.RequiredChoices).Items)
										{
											if (r is Range)
											{
												foreach (var sr in ((Range)r).Ranges)
												{
													if (!StringHelper.IsInt(sr.From.ToString()))
													{
														writer.WriteLine("<ComboBoxItem Content=" + StringHelper.EnsureQuotes(sr.From.ToString()) + " />");
														writer.WriteLine("<ComboBoxItem Content=" + StringHelper.EnsureQuotes(sr.To.ToString()) + " />");
														continue;
													}
													int from = Int32.Parse(StringHelper.StripQuotes(sr.From.ToString()));
													int to = Int32.Parse(StringHelper.StripQuotes(sr.To.ToString()));

													if (from > to)
													{
														throw new Exception("COBOL is evil");
													}

													while (from <= to)
													{
														writer.WriteLine("<ComboBoxItem Content=" + StringHelper.EnsureQuotes(from.ToString()) + " />");
														from++;
													}
												}
											}
											else
											{
												writer.WriteLine("<ComboBoxItem Content=" + StringHelper.EnsureQuotes(r.ToString()) + " />");
											}
										}
									}
									else
									{
										foreach (var sr in ((Range)((Expr)f.RequiredChoices).Terms[0]).Ranges)
										{
											int from = Int32.Parse(StringHelper.StripQuotes(sr.From.ToString()));
											int to = Int32.Parse(StringHelper.StripQuotes(sr.To.ToString()));

											if (from > to)
											{
												throw new Exception("COBOL is evil");
											}

											while (from <= to)
											{
												writer.WriteLine("<ComboBoxItem Content=" + StringHelper.EnsureQuotes(from.ToString()) + " />");
												from++;
											}
										}
									}
								}
								else if (getwsf.Attributes != null && getwsf.Attributes.ValueConstraints.Count > 0)
								{
									foreach (var r in getwsf.Attributes.ValueConstraints)
									{
										writer.WriteLine("<ComboBoxItem Content=" + StringHelper.EnsureQuotes(r.Str) + " />");
									}
								}
								else
								{
									Debug.Assert(getwsf.Has88());

								}

								writer.WriteLine("</ComboBox>");
							}
						}
					}
				}
			}
		}

		private void WriteWpfScreen(TextWriter writer, Screen s, string ns, string cls)
		{
			string screenCsName = CsFieldNameConverter.Convert(s.Name);

			writer.WriteLine("<Canvas Name=\"" + screenCsName + "\" Width=\"" + (int)(80.06 * _charWidth) + "\" Height=\"" + (24 * _charHeight) + "\" Visibility=\"{Binding Path=" + screenCsName + "Visibility}\">");

			WriteWpfScreenFields(writer, s, ns, cls);

			writer.WriteLine("</Canvas>");
		}

		private void WriteWpfCodeBehind(TextWriter writer, int PgmfId, string ns, string cls)
		{
			SourceWriter cswriter = new SourceWriter(writer);

			cswriter.WriteUsing("System");
			cswriter.WriteUsing("System.Collections.Generic");
			cswriter.WriteUsing("System.Diagnostics");
			cswriter.WriteUsing("System.Linq");
			cswriter.WriteUsing("System.Text");
			cswriter.WriteUsing("System.ComponentModel");
			cswriter.WriteUsing("System.Threading");
			cswriter.WriteUsing("System.Windows");
			cswriter.WriteUsing("System.Windows.Controls");
			cswriter.WriteUsing("System.Windows.Data");
			cswriter.WriteUsing("System.Windows.Documents");
			cswriter.WriteUsing("System.Windows.Input");
			cswriter.WriteUsing("System.Windows.Media");
			cswriter.WriteUsing("System.Windows.Media.Imaging");
			cswriter.WriteUsing("System.Windows.Shapes");
			cswriter.WriteLine();
			cswriter.WriteUsing("DOR.Core");
			cswriter.WriteUsing("DOR.ShellContext.Model");
			cswriter.WriteUsing("DOR.WorkingStorage.Wpf");
			cswriter.WriteUsing("DOR.WPF");
			cswriter.WriteUsing("GalaSoft.MvvmLight.Threading");
			cswriter.WriteLine();

			string programName = CsFieldNameConverter.Convert(Cobol.ProgramId);

			cswriter.WriteLine("namespace " + ns);
			using (cswriter.Indent())
			{
				cswriter.WriteLine("public partial class Screens : CobolScreenBase");
				using (cswriter.Indent())
				{
					cswriter.WriteLine("private " + RootNameSpace(null) + "." + programName + "Program " + programName);
					using (cswriter.Indent())
					{
						cswriter.WriteLine("get { return (" + RootNameSpace(null) + "." + programName + "Program)ViewModel; }");
					}
					cswriter.WriteLine();

					cswriter.WriteLine("public Screens()");
					using (cswriter.Indent())
					{
						cswriter.WriteLine("InitializeComponent();");
					}
					cswriter.WriteLine();

					cswriter.WriteLine("public override void Initialize(IContext ctx, IScreenConfig cfg)");
					using (cswriter.Indent())
					{
						cswriter.WriteLine("base.Initialize(ctx, cfg);");
						cswriter.WriteLine();
						cswriter.WriteLine(programName + ".LoadComplete += () =>");
						cswriter.WriteLine("{");
						cswriter.WriteLine("\tViewModel.Context.WaitIndicatorVisibility = Visibility.Hidden;");
						cswriter.WriteLine("};");
						cswriter.WriteLine();
						cswriter.WriteLine("///TODO: pull any command line values");
						cswriter.WriteLine("//if (cfg.HasKey(\"tra\") && cfg.StringAt(\"tra\") != ScWsTraIdDisplayBinding.Text)");
						cswriter.WriteLine("//{");
						cswriter.WriteLine("//	ScInputField.Text = cfg.StringAt(\"tra\").Trim();");
						cswriter.WriteLine("//	if (StringHelper.IsInt(ScInputField.Text))");
						cswriter.WriteLine("//	{");
						cswriter.WriteLine("//		GrdScreen_KeyUp(Key.F1);");
						cswriter.WriteLine("//	}");
						cswriter.WriteLine("//}");
						cswriter.WriteLine("//else if (ViewModel.Context.Session.TraId.HasValue)");
						cswriter.WriteLine("//{");
						cswriter.WriteLine("//	if (Int32.Parse(ScWsTraIdDisplayBinding.Text) != ViewModel.Context.Session.TraId.Value)");
						cswriter.WriteLine("//	{");
						cswriter.WriteLine("//		ScInputField.Text = ViewModel.Context.Session.TraId.ToString();");
						cswriter.WriteLine("//		GrdScreen_KeyUp(Key.F1);");
						cswriter.WriteLine("//	}");
						cswriter.WriteLine("//}");
					}
					cswriter.WriteLine();

					cswriter.WriteLine("private void GrdScreen_KeyUp(object sender, KeyEventArgs e)");
					cswriter.WriteLine("{");
					cswriter.WriteLine("	GrdScreen_KeyUp(e.Key);");
					cswriter.WriteLine("}");
				}
			}
			cswriter.Close();
		}
	}
}
