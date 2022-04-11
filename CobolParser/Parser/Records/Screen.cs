using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DOR.Core.Collections;
using DOR.Core;

namespace CobolParser.Records
{
	public class Screen
	{
		private Vector<ScreenField> _fields = new Vector<ScreenField>();

		public bool IsDisplayed
		{
			get;
			set;
		}

		public string Name
		{
			get;
			private set;
		}

		public string PgmfName
		{
			get 
			{ 
				string n = Name.Substring(Name.IndexOf('-') + 1);
				if (StringHelper.CountOccurancesOf(n, '-') > 1)
				{
					int pos = n.IndexOf('-', n.IndexOf('-') + 1);
					return n.Substring(0, pos);
				}
				return n;
			}
		}

		public int PgmfId
		{
			get
			{
				return Int32.Parse(StringHelper.RightStr(PgmfName, 3));
			}
		}

		public Vector<ScreenField> Fields
		{
			get { return _fields; }
		}

		public bool IsOverlay
		{
			get;
			private set;
		}

		public Screen(Terminalize terms)
		{
			terms.Match("01");

			Name = terms.Current.Str;
			terms.Match(StringNodeType.Word);

			if (terms.CurrentEquals("OVERLAY"))
			{
				terms.Next();
				IsOverlay = true;
			}

			if (terms.CurrentEquals("SIZE"))
			{
				terms.Match("SIZE");
				terms.Match(StringNodeType.Number);
				terms.MatchOptional(",");
				terms.Match(StringNodeType.Number);
			}
			terms.Match(StringNodeType.Period);

			while (terms.Current.Str != "01" && ! terms.Current.Str.Equals("PROCEDURE"))
			{
				Fields.Add(new ScreenField(terms, this));

				while (terms.Current.Type == StringNodeType.Comma)
				{
					// COBOL ignores commas, should have filter them in Terminalize
					terms.Next();
				}
			}
		}

		public bool HasField(string name)
		{
			for (int x = 0; x < Fields.Count; x++)
			{
				if (Fields[x].Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}

			return false;
		}

		public ScreenField FindField(string name)
		{
			for (int x = 0; x < Fields.Count; x++)
			{
				if (Fields[x].Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				{
					return Fields[x];
				}
			}

			return null;
		}

		public bool HasArea()
		{
			for (int x = 0; x < Fields.Count; x++)
			{
				if (Fields[x].IsArea)
				{
					return true;
				}
			}

			return false;
		}
	}
}
