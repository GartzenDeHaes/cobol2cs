using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using DOR.Core;
using DOR.Core.Collections;
using CobolParser.Records;
using CobolParser.Verbs;

namespace CobolParser
{
	public static class ImportManager
	{
		private static Dictionary<GuardianPath, Terminalize> _includeIdx = new Dictionary<GuardianPath, Terminalize>();
		private static Dictionary<GuardianDefine, GuardianPath> _defines = new Dictionary<GuardianDefine, GuardianPath>();
		private static Dictionary<string, Terminalize> _sections = new Dictionary<string, Terminalize>();
		private static Dictionary<GuardianPath, List<GuardianPath>> _programCopyBooks = new Dictionary<GuardianPath, List<GuardianPath>>();

		private static Dictionary<string, Offset> _ddlDefLookup = new Dictionary<string, Offset>();

		public static Dictionary<GuardianPath, Dictionary<string, Send>> _pathwaySends = new Dictionary<GuardianPath, Dictionary<string, Send>>();

		public static int LineCount
		{
			get;
			private set;
		}

		public static string BaseDirectory
		{
			get;
			set;
		}

		public static List<GuardianPath> ProgramCopyBooks(GuardianPath prog)
		{
			if (!_programCopyBooks.ContainsKey(prog))
			{
				return new List<GuardianPath>();
			}
			return _programCopyBooks[prog];
		}

		public static List<Terminalize> CopyBooks
		{
			get { return _includeIdx.Values.ToList(); }
		}

		public static void AddDefine(GuardianDefine def, GuardianPath path)
		{
			_defines.Add(def, path);
		}

		public static GuardianPath ResolveDefine(GuardianDefine def)
		{
			Debug.Assert(_defines.ContainsKey(def));
			return _defines[def];
		}

		public static void AddDdlDef(Offset rec)
		{
			if (_ddlDefLookup.ContainsKey(rec.Name))
			{
				return;
			}
			_ddlDefLookup.Add(rec.Name, rec);
		}

		public static Offset GetDllDef(string name)
		{
			if (_ddlDefLookup.ContainsKey(name))
			{
				return _ddlDefLookup[name];
			}
			return null;
		}

		public static IList<Offset> DdlOffsets
		{
			get { return _ddlDefLookup.Values.ToList(); }
		}

		public static void AddPathwaySend(GuardianPath file, Send server)
		{
			if (!_pathwaySends.ContainsKey(file))
			{
				_pathwaySends.Add(file, new Dictionary<string, Send>());
			}

			if (!_pathwaySends[file].ContainsKey(server.ServerClassName))
			{
				_pathwaySends[file].Add(server.ServerClassName, server);
			}
		}

		public static Dictionary<string, Send> PathwaySends(GuardianPath path)
		{
			if (_pathwaySends.ContainsKey(path))
			{
				return _pathwaySends[path];
			}
			return null;
		}

		public static Terminalize GetFile(GuardianPath file)
		{
			if (!_includeIdx.ContainsKey(file))
			{
				Load(file);
			}

			return Clone(_includeIdx[file]);
		}

		public static Terminalize GetSection(GuardianPath caller, GuardianPath file, string section)
		{
			if (!_programCopyBooks.ContainsKey(caller))
			{
				_programCopyBooks.Add(caller, new List<GuardianPath>());
			}
			_programCopyBooks[caller].Add(file);

			string sectionIdxKey = file.ToString() + "." + section;
			if (_sections.ContainsKey(sectionIdxKey))
			{
				return Clone(_sections[sectionIdxKey]);
			}

			if (!_includeIdx.ContainsKey(file))
			{
				Load(file);
			}

			Terminalize ddlTerms = _includeIdx[file];
			ddlTerms.BeginIteration();

			StringNode head = null;
			StringNode tail = ddlTerms.Last;
			bool isModeSection = false;

			while (ddlTerms.Next())
			{
				if (ddlTerms.Current.Str == "?" && (head == null || isModeSection))
				{
					if (isModeSection)
					{
						tail = ddlTerms.Current.Prev;
						break;
					}

					ddlTerms.Next();

					//Debug.WriteLine(ddlTerms.CurrentNext(1).Str);
					if (ddlTerms.Current.Str == "SECTION")
					{
						ddlTerms.Next();
						if (null == head && ddlTerms.CurrentEquals(section))
						{
							int lineNum = ddlTerms.Current.LineNumber;
							while (ddlTerms.Current.LineNumber == lineNum)
							{
								ddlTerms.Next();
							}

							if (ddlTerms.Current.Type == StringNodeType.QuestionMark)
							{
								if (ddlTerms.CurrentNextEquals(1, "SECTION"))
								{
									return new Terminalize(ddlTerms.FileName);
								}
							}
							head = ddlTerms.Current;
							isModeSection = true;
						}
					}
				}
				else if (!isModeSection)
				{
					if (ddlTerms.Current.Str == "01")
					{
						if (null == head)
						{
							if (ddlTerms.CurrentNext(1).Str == section)
							{
								head = ddlTerms.Current;
							}
						}
						else
						{
							tail = ddlTerms.Current.Prev;
							break;
						}
					}
				}
			}

			Debug.Assert(null != head);
			if (null == head)
			{
				return null;
			}

			Terminalize t = Clone(file, head, tail);
			_sections.Add(sectionIdxKey, Clone(t));

			return t;
		}

		private static Terminalize Clone(Terminalize t)
		{
			return Clone(t.FileName, t.First, t.Last);
		}

		private static Terminalize Clone(GuardianPath file, StringNode head, StringNode tail)
		{
			Terminalize terms = new Terminalize(file);
			while (head != tail.Next)
			{
				terms.Add(head.Clone());
				head = head.Next;
			}
			return terms;
		}

		private static void Load(GuardianPath file)
		{
			if (!File.Exists(file.WindowsFileName()))
			{
				throw new SyntaxError(file, 0, "Not found");
			}

			Terminalize terms;
			using (StreamReader reader = File.OpenText(file.WindowsFileName()))
			{
				terms = new Terminalize(file, reader);
			}

			_includeIdx.Add(file, terms);

			LineCount += terms.LineCount;
		}

		public static GuardianPath GuardianPathFromWindows(string winPath)
		{
			int pos = winPath.IndexOf(BaseDirectory);
			Debug.Assert(pos > -1);

			string dirFn = winPath.Substring(pos + BaseDirectory.Length);
			pos = dirFn.IndexOf('\\');
			string fileName = dirFn.Substring(pos + 1);
			dirFn = dirFn.Substring(0, pos);

			pos = dirFn.IndexOf('.');

			return new GuardianPath("$" + dirFn.Substring(0, pos), dirFn.Substring(pos + 1), fileName);
		}

		public static INamedField FindDdlDef(string name)
		{
			foreach (var o in _ddlDefLookup.Values)
			{
				if (o.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				{
					return o;
				}

				var oo = o.FindChildNamed(name);
				if (oo != null)
				{
					return oo;
				}
			}

			return null;
		}

		public static void AddDorNonDatabaseDefines()
		{
			ImportManager.AddDefine(new GuardianDefine("=ENV-CODE"), new GuardianPath("$d23", "srccsubr", "env"));
			ImportManager.AddDefine(new GuardianDefine("=EMAIL-CODE"), new GuardianPath("$d10", "srccsubr", "email"));
			ImportManager.AddDefine(new GuardianDefine("=COBLIB"), new GuardianPath("$d23", "srccsubr", "coblib"));
			ImportManager.AddDefine(new GuardianDefine("=CLW-CODE"), new GuardianPath("$d23", "srccsubr", "clw"));
			ImportManager.AddDefine(new GuardianDefine("=XML-CODE"), new GuardianPath("$d23", "srccsubr", "xml"));
			ImportManager.AddDefine(new GuardianDefine("=JOBCODE"), new GuardianPath("$d23", "srccsubr", "job"));
			ImportManager.AddDefine(new GuardianDefine("=HTML-CODE"), new GuardianPath("$d23", "srccsubr", "html"));
			ImportManager.AddDefine(new GuardianDefine("=CLWEB-CODE"), new GuardianPath("$d23", "srccsubr", "clweb"));
			ImportManager.AddDefine(new GuardianDefine("=EMAIL-SQL-ERROR"), new GuardianPath("$d10", "sssrc", "emailsql"));
			ImportManager.AddDefine(new GuardianDefine("=T220SW"), new GuardianPath("$d23", "OBJTSUBR", "T220SWO"));
			ImportManager.AddDefine(new GuardianDefine("=COBOL85_UTIL"), new GuardianPath("$system", "system", "cbl85utl"));
			ImportManager.AddDefine(new GuardianDefine("=COBOLLIB"), new GuardianPath("$system", "system", "cobollib"));
			ImportManager.AddDefine(new GuardianDefine("=U100AC"), new GuardianPath("$d08", "source", "u100a1w"));
			ImportManager.AddDefine(new GuardianDefine("=U100AR"), new GuardianPath("$d08", "source", "u100ar1w"));
			ImportManager.AddDefine(new GuardianDefine("=U100BR"), new GuardianPath("$d08", "source", "u100br1w"));
			ImportManager.AddDefine(new GuardianDefine("=U100FE"), new GuardianPath("$d08", "source", "u100fe1w"));
			ImportManager.AddDefine(new GuardianDefine("=U100UP"), new GuardianPath("$d08", "source", "u100up1u"));
			ImportManager.AddDefine(new GuardianDefine("=U101UP"), new GuardianPath("$d08", "source", "u101up1u"));
			ImportManager.AddDefine(new GuardianDefine("=U110AC"), new GuardianPath("$d08", "source", "u110ac1w"));
			ImportManager.AddDefine(new GuardianDefine("=U110BR"), new GuardianPath("$d08", "source", "u110br1u"));
			ImportManager.AddDefine(new GuardianDefine("=U111BR"), new GuardianPath("$d08", "source", "u111br1u"));
			ImportManager.AddDefine(new GuardianDefine("=U112BR"), new GuardianPath("$d08", "source", "u112br1u"));
			ImportManager.AddDefine(new GuardianDefine("=U113BR"), new GuardianPath("$d08", "source", "u113br1u"));
			ImportManager.AddDefine(new GuardianDefine("=U120AC"), new GuardianPath("$d08", "source", "u120ac1w"));
			ImportManager.AddDefine(new GuardianDefine("=U120BR"), new GuardianPath("$d08", "source", "u120br1w"));
			ImportManager.AddDefine(new GuardianDefine("=U120AR"), new GuardianPath("$d08", "source", "u100ar1w"));
			ImportManager.AddDefine(new GuardianDefine("=U130AC"), new GuardianPath("$d08", "source", "u130ac1w"));
			ImportManager.AddDefine(new GuardianDefine("=U140AC"), new GuardianPath("$d08", "source", "u140ac1w"));
			ImportManager.AddDefine(new GuardianDefine("=U200AR"), new GuardianPath("$d08", "source", "u200ar1u"));
			ImportManager.AddDefine(new GuardianDefine("=U901CR"), new GuardianPath("$d08", "source", "u901cr1w"));
			ImportManager.AddDefine(new GuardianDefine("=U920CR"), new GuardianPath("$d08", "source", "u920cr1w"));
			ImportManager.AddDefine(new GuardianDefine("=U930CR"), new GuardianPath("$d08", "source", "u930cr1w"));
			ImportManager.AddDefine(new GuardianDefine("=U950CR"), new GuardianPath("$d08", "source", "u950cr1w"));
			ImportManager.AddDefine(new GuardianDefine("=U950ET"), new GuardianPath("$d08", "source", "u950et1w"));
			ImportManager.AddDefine(new GuardianDefine("=U951ET"), new GuardianPath("$d08", "source", "u951et1w"));
			ImportManager.AddDefine(new GuardianDefine("=U951CR"), new GuardianPath("$d08", "source", "u951cr1w"));
			ImportManager.AddDefine(new GuardianDefine("=U952CR"), new GuardianPath("$d08", "source", "u952cr1w"));
			ImportManager.AddDefine(new GuardianDefine("=U953CR"), new GuardianPath("$d08", "source", "u953cr1w"));
			ImportManager.AddDefine(new GuardianDefine("=U955CR"), new GuardianPath("$d08", "source", "u955cr1w"));
			ImportManager.AddDefine(new GuardianDefine("=U955ET"), new GuardianPath("$d08", "source", "u955et1w"));
			ImportManager.AddDefine(new GuardianDefine("=U960ET"), new GuardianPath("$d08", "source", "u960et1w"));
			ImportManager.AddDefine(new GuardianDefine("=U965ET"), new GuardianPath("$d08", "source", "u965et1w"));
			ImportManager.AddDefine(new GuardianDefine("=U970ET"), new GuardianPath("$d08", "source", "u970et1w"));
			ImportManager.AddDefine(new GuardianDefine("=U975ET"), new GuardianPath("$d08", "source", "u975et1w"));
			ImportManager.AddDefine(new GuardianDefine("=U975TA"), new GuardianPath("$d08", "sources", "u975ta1w"));
			ImportManager.AddDefine(new GuardianDefine("=U999SS"), new GuardianPath("$d08", "source", "u999ss1w"));
			ImportManager.AddDefine(new GuardianDefine("=ULSUET"), new GuardianPath("$d08", "OBJQSUBR", "ULSUETO"));
			ImportManager.AddDefine(new GuardianDefine("=T100CL"), new GuardianPath("$d08", "source", "t100cl1t"));
			ImportManager.AddDefine(new GuardianDefine("=T230SW"), new GuardianPath("$d08", "source", "t230sw1t"));
		}
	}
}
