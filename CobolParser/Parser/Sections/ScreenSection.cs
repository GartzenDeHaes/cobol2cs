using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core;
using DOR.Core.Collections;
using CobolParser.Records;
using CobolParser.Expressions;
using CobolParser.Expressions.Terms;

namespace CobolParser.Sections
{
	public class ScreenSection : Section
	{
		public Vector<Screen> Screens
		{
			get;
			private set;
		}

		public List<IVerb> CompilerDirectives
		{
			get;
			private set;
		}

		public ScreenSection(Terminalize terms)
		: base(terms.Current)
		{
			Screens = new Vector<Screen>();
			CompilerDirectives = new List<IVerb>();

			terms.Match("SCREEN");
			terms.Match("SECTION");
			terms.Match(StringNodeType.Period);

			while (StringHelper.IsInt(terms.Current.Str) || terms.Current.Type == StringNodeType.QuestionMark)
			{
				if (terms.Current.Type == StringNodeType.QuestionMark)
				{
					CompilerDirectives.Add(VerbLookup.Create(terms, DivisionType.Data));
				}
				else
				{
					Screens.Add(new Screen(terms));
				}
			}
		}

		public bool IsScreenField(string name, string parentName)
		{
			for (int x = 0; x < Screens.Count; x++)
			{
				if (Screens[x].Name.Equals(parentName, StringComparison.InvariantCultureIgnoreCase))
				{
					return Screens[x].HasField(name);
				}
			}
			return false;
		}

		public ScreenField FindScreenField(ITerm term)
		{
			if (term is Id)
			{
				return FindScreenField(((Id)term).Value.Str);
			}
			return FindScreenField(((OffsetReference)term).OffsetChain[0].ToString(), ((OffsetReference)term).OffsetChain[((OffsetReference)term).OffsetChain.Count-1].ToString());
		}

		public ScreenField FindScreenField(string name, string parentName = null)
		{
			for (int x = 0; x < Screens.Count; x++)
			{
				if (String.IsNullOrEmpty(parentName) || Screens[x].Name.Equals(parentName, StringComparison.InvariantCultureIgnoreCase))
				{
					ScreenField sf = Screens[x].FindField(name);
					if (sf != null)
					{
						return sf;
					}
					if (!String.IsNullOrEmpty(parentName))
					{
						return null;
					}
				}
			}
			return null;
		}

		public Screen FindScreen(string name)
		{
			for (int x = 0; x < Screens.Count; x++)
			{
				if (Screens[x].Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				{
					return Screens[x];
				}
			}

			return null;
		}

		public bool HasAnyArea()
		{
			foreach (var s in Screens)
			{
				if (s.HasArea())
				{
					return true;
				}
			}
			return false;
		}
	}
}
