using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DOR.Core.Collections;
using CobolParser.Verbs;
using CobolParser.Parser;

namespace CobolParser.Division
{
	public class IdentificationDiv : IDivision
	{
		public List<CompilerDirective> CompilerDirectives
		{
			get;
			private set;
		}

		public List<GuardianPath> SearchFiles
		{
			get;
			private set;
		}

		public string ProgramId
		{
			get;
			private set;
		}

		public string System
		{
			get;
			private set;
		}

		public string Author
		{
			get;
			private set;
		}

		public string DateWritten
		{
			get;
			private set;
		}

		public string HeaderComments
		{
			get;
			private set;
		}

		public IdentificationDiv()
		: base(DivisionType.Identification)
		{
			CompilerDirectives = new List<CompilerDirective>();
			SearchFiles = new List<GuardianPath>();
		}

		public override void Parse(Terminalize terms)
		{
			terms.SkipComments = false;

			StringBuilder headerDocs = new StringBuilder();
			Vector<StringNode> sentence = new Vector<StringNode>();

			while (!terms.IsIterationComplete && !terms.Current.Str.Equals("IDENTIFICATION"))
			{
				if (terms.Current.Type == StringNodeType.Comment)
				{
					headerDocs.Append(terms.Current.Str);
					headerDocs.Append('\n');
					terms.Next();
					continue;
				}

				if (terms.Current.Type == StringNodeType.QuestionMark)
				{
					CompilerDirective cdir = (CompilerDirective)VerbLookup.Create(terms, DivisionType);
					CompilerDirectives.Add(cdir);
					if (cdir.Directive.Str.Equals("SEARCH", StringComparison.InvariantCultureIgnoreCase))
					{
						if (GuardianDefine.IsDefine(cdir.Arg1))
						{
							SearchFiles.Add(ImportManager.ResolveDefine(new GuardianDefine(cdir.Arg1)));
						}
						else
						{
							SearchFiles.Add(new GuardianPath(cdir.Arg1));
						}

						for (int x = 0; x < cdir.ArgCount; x++)
						{
							if (cdir.ArgX(x).Type == StringNodeType.Eq)
							{
								x++;
								SearchFiles.Add(ImportManager.ResolveDefine(new GuardianDefine(cdir.ArgX(x))));
							}
							else
							{
								SearchFiles.Add(new GuardianPath(cdir.ArgX(x)));
							}
						}
					}
					continue;
				}

				throw new SyntaxError(terms.Current.FileName, terms.Current.LineNumber, "Unexpected terminal before identification division of " + terms.Current.Str);
			}

			terms.Match("IDENTIFICATION");
			terms.Match("DIVISION");
			terms.Match(StringNodeType.Period);

			while 
			(
				!terms.IsIterationComplete && 
				!terms.CurrentNextEquals(1, "DIVISION")
			)
			{
				while (terms.Current.Type == StringNodeType.Comment)
				{
					if (terms.Current.Str != "/")
					{
						headerDocs.Append(terms.Current.Str);
						headerDocs.Append('\n');
					}
					terms.Next();
				}

				if (terms.CurrentNextEquals(1, "DIVISION"))
				{
					break;
				}

				Debug.Assert(terms.Current.Type != StringNodeType.QuestionMark);
				Debug.Assert(!terms.CurrentNext(1).Str.Equals("SECTION"));

				terms.ReadSentence(sentence);
				if (sentence.Count == 0)
				{
					// stray period
					continue;
				}

				if (sentence[0].Str.Equals("PROGRAM-ID", StringComparison.InvariantCultureIgnoreCase))
				{
					terms.ReadSentence(sentence);
					Debug.Assert(sentence.Count > 0);
					ProgramId = sentence[0].Str;
					if (ProgramId.IndexOf('-') > -1)
					{
						System = ProgramId.Substring(ProgramId.IndexOf('-') + 1);
						if (System.IndexOf('-') > -1)
						{
							System = System.Substring(0, System.IndexOf('-'));
						}
					}
					else
					{
						System = "Unknown";
					}
					continue;
				}

				if (sentence[0].Str.Equals("AUTHOR", StringComparison.InvariantCultureIgnoreCase))
				{
					terms.ReadSentence(sentence);
					Debug.Assert(sentence.Count > 0);

					Author = "";
					for (int x = 0; x < sentence.Count; x++)
					{
						if (!sentence[x].Str.Equals("INSTALLATION", StringComparison.InvariantCultureIgnoreCase))
						{
							Author += sentence[x].Str + " ";
						}
					}
					Author = Author.Trim();
					continue;
				}

				if (sentence[0].Str.Equals("DATE-WRITTEN", StringComparison.InvariantCultureIgnoreCase))
				{
					terms.ReadSentence(sentence);
					Debug.Assert(sentence.Count > 0);

					for (int x = 0; x < sentence.Count; x++)
					{
						DateWritten += sentence[x].Str + " ";
					}
					DateWritten = DateWritten.Trim();
					continue;
				}

				Debug.WriteLine("Skipping " + sentence[0].Str + " on line " + sentence[0].LineNumber.ToString());
			}

			HeaderComments = headerDocs.ToString();
			terms.SkipComments = true;
		}
	}
}
