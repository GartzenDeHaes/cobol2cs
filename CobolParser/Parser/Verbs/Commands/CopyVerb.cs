using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DOR.Core;

namespace CobolParser.Verbs
{
	public class CopyVerb : IVerb
	{
		public const string Lexum = "COPY";

		public GuardianPath FileName
		{
			get;
			protected set;
		}

		public StringNode RecordName
		{
			get;
			protected set;
		}

		public List<CopyReplacement> Replacements = new List<CopyReplacement>();

		public CopyVerb(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.Copy;

			terms.Match("COPY");

			RecordName = terms.Current;
			terms.Match(StringNodeType.Word);

			terms.Match("OF");

			if (terms.Current.Str[0] == '=' || terms.Current.Str[1] == '=')
			{
				FileName = ImportManager.ResolveDefine(new GuardianDefine(StringHelper.StripQuotes(terms.Current.Str)));
			}
			else
			{
				FileName = new GuardianPath(StringHelper.StripQuotes(terms.Current.Str));
			}

			terms.Next();

			if (terms.Current.Str.Equals("REPLACING", StringComparison.InvariantCultureIgnoreCase))
			{
				terms.Match(StringNodeType.Word);

				if (terms.Current.Type == StringNodeType.Eq)
				{
					ParseEqualDelim(terms);
				}
				else
				{
					ParseSpaceDelim(terms);
				}
			}

			Debug.Assert(terms.Current.Type == StringNodeType.Period);

			Terminalize copyTerms = ImportManager.GetSection(FileName, FileName, RecordName);
			if (null == copyTerms || copyTerms.LineCount == 0)
			{
				throw new Exception("COPY from " + terms.Current.FileName.ToString() + " Section " + RecordName.Str + " not found in " + FileName.ToString());
			}

			if (Replacements.Count > 0)
			{
				copyTerms.BeginIteration();

				while (copyTerms.Next())
				{
					foreach (CopyReplacement crep in Replacements)
					{
						// Not sure if this is true
						Debug.Assert(crep.FromTokens.Count == 1);

						if (crep.FromTokens[0].Str == copyTerms.Current.Str)
						{
							ReplaceTerminals(copyTerms, crep);
							break;
						}
					}
				}
			}

			terms.InjectAfterCurrent(copyTerms);
		}

		private void ReplaceTerminals(Terminalize terms, CopyReplacement crep)
		{
			StringNode node = terms.Current;

			for (int x = 0; x < crep.FromTokens.Count; x++)
			{
				if (! crep.FromTokens[x].StrEquals(node.Str))
				{
					return;
				}
				if (null == (node = node.Next))
				{
					return;
				}
			}

			for (int x = 0; x < crep.FromTokens.Count; x++)
			{
				terms.Remove();
			}

			for (int x = 0; x < crep.ToTokens.Count; x++)
			{
				terms.Insert(crep.ToTokens[x].Clone());
				terms.Next();
			}
			terms.Prev();
		}
		
		private void ParseEqualDelim(Terminalize terms)
		{
			while (terms.Current.Type == StringNodeType.Eq)
			{
				terms.Match("=");
				terms.Match("=");

				CopyReplacement sub = new CopyReplacement();

				while (!(terms.Current.Type == StringNodeType.Eq && terms.CurrentNext(1).Type == StringNodeType.Eq))
				{
					sub.FromTokens.Add(terms.Current);
					terms.Next();
				}

				terms.Match("=");
				terms.Match("=");

				terms.Match("BY");

				terms.Match("=");
				terms.Match("=");

				while (!(terms.Current.Type == StringNodeType.Eq && terms.CurrentNext(1).Type == StringNodeType.Eq))
				{
					sub.ToTokens.Add(terms.Current);
					terms.Next();
				}

				terms.Match("=");
				terms.Match("=");

				Replacements.Add(sub);

				terms.MatchOptional(",");
			}
		}

		private void ParseSpaceDelim(Terminalize terms)
		{
			while (terms.Current.Type != StringNodeType.Period)
			{
				CopyReplacement sub = new CopyReplacement();
				Replacements.Add(sub);

				sub.FromTokens.Add(terms.Current);
				terms.Next();

				terms.Match("BY");

				sub.ToTokens.Add(terms.Current);
				terms.Next();

				if (terms.Current.Type == StringNodeType.Comma)
				{
					terms.Next();
				}
			}
		}
	}
}
