using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CobolParser.Parser.Verbs.Phrases
{
	public class AcceptUntilItem
	{
		public List<string> Items
		{
			get;
			private set;
		}

		public AcceptUntilItem(Terminalize terms)
		{
			Items = new List<string>();

			if (terms.Current.Type == StringNodeType.LPar)
			{
				terms.Match(StringNodeType.LPar);

				while (terms.Current.Type != StringNodeType.RPar)
				{
					ParseUnit(terms);
				}

				terms.Match(StringNodeType.RPar);
			}
			else
			{
				ParseUnit(terms);
			}
		}

		private void ParseUnit(Terminalize terms)
		{
			Dictionary<string, string> idx = new Dictionary<string, string>();

			if (terms.CurrentNextEquals("THRU") || terms.CurrentNextEquals("THROUGH"))
			{
				string prefix1;
				string suffix1;
				string prefix2;
				string suffix2;
				int startnum;
				int endnum;

				SplitNumeric(terms.Current.Str, out prefix1, out startnum, out suffix1);
				terms.Next();
				terms.Next();
				SplitNumeric(terms.Current.Str, out prefix2, out endnum, out suffix2);
				terms.Next();

				if (prefix1 == prefix2)
				{
					Debug.Assert(startnum < endnum);
					while (startnum <= endnum)
					{
						string item = prefix1 + startnum + suffix1;
						if (idx.ContainsKey(item))
						{
							continue;
						}
						idx.Add(item, item);

						Items.Add(item);
						startnum++;
					}
				}
				else
				{
					while (startnum <= 12)
					{
						string item = prefix1 + startnum + suffix1;
						if (idx.ContainsKey(item))
						{
							continue;
						}
						idx.Add(item, item);

						Items.Add(item);
						startnum++;
					}
					startnum = 1;
					while (startnum <= endnum)
					{
						string item = prefix2 + startnum + suffix2;
						if (idx.ContainsKey(item))
						{
							continue;
						}
						idx.Add(item, item);

						Items.Add(item);
						startnum++;
					}
				}
			}
			else
			{
				Items.Add(terms.Current.Str);
				terms.Next();
			}
		}

		private void SplitNumeric(string s, out string prefix, out int num, out string suffix)
		{
			StringBuilder buf = new StringBuilder();
			int state = 0;
			prefix = null;
			num = 0;

			for (int x = 0; x < s.Length; x++)
			{
				char ch = s[x];

				switch (state)
				{
					case 0:
						// prefix
						if (Char.IsNumber(ch))
						{
							prefix = buf.ToString();
							buf.Length = 0;
							state = 1;
						}
						buf.Append(ch);
						break;
					case 1:
						// number
						if (! Char.IsNumber(ch))
						{
							num = Int32.Parse(buf.ToString());
							buf.Length = 0;
							state = 2;
						}
						buf.Append(ch);
						break;
					case 2:
						buf.Append(ch);
						break;
				}
			}

			suffix = buf.ToString();
		}
	}
}
