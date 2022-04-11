using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DOR.Core;
using DOR.Core.Collections;
using CobolParser.Text;
using CobolParser.Parser;

namespace CobolParser.Records
{
	public class Storage : IOffsetLocator
	{
		public Vector<Offset> Fields
		{
			get;
			private set;
		}

		public Storage()
		{
			Fields = new Vector<Offset>();
		}

		public bool GroupHasDuplicate(string name)
		{
			int count = 0;

			for (int x = 0; x < Fields.Count; x++)
			{
				if (name == Fields[x].Name && Fields[x].SubGroups.Count > 0)
				{
					count++;
				}

				count += Fields[x].CountGroupsNamed(name);

				if (count > 1)
				{
					return true;
				}
			}

			return false;
		}

		public INamedField LocateField(string name, string parentName)
		{
			Debug.Assert(name.IndexOf(',') < 0);

			if (name.IndexOf('.') > -1)
			{
				Debug.Assert(String.IsNullOrWhiteSpace(parentName));

				string[] parts = name.Split(new char[] { '.' });

				parentName = parts[0];
				name = parts[parts.Length - 1];
			}

			for (int x = 0; x < Fields.Count; x++)
			{
				INamedField n = Fields[x].LocateField(name, parentName);
				if (null != n)
				{
					return n;
				}
			}

			return null;
		}

		public Offset AddOne(Terminalize terms, string docs)
		{
			bool isDef = terms.CurrentEquals("DEF");
			bool patchUp = false;

			if (!isDef && terms.Current.Str != "01" && terms.Current.Str != "77")
			{
				patchUp = true;
			}

			Offset ret = null;

			do
			{
				Offset f = new Offset(terms);
				if (ret == null)
				{
					ret = f;
				}

				if (patchUp)
				{
					if (f.LevelAsInt == 10)
					{
						f.Parent = Fields.LastElement().SubGroups.LastElement();
						f.Parent.SubGroups.Add(f);
					}
					else
					{
						f.Parent = Fields.LastElement();
						f.Parent.SubGroups.Add(f);
					}

					if (terms.CurrentEquals("01"))
					{
						patchUp = false;
					}
				}
				else
				{
					f.Comments = docs;

					if (isDef)
					{
						f.RecordDef();
					}
					else
					{
						Fields.Add(f);
					}
				}
			}
			while (!terms.IsIterationComplete && StringHelper.IsInt(terms.Current.Str));

			return ret;
		}

		public void FixupRedefineHints()
		{
			for (int x = 0; x < Fields.Count; x++)
			{
				Fields[x].FixupRedefineHints(this);
			}
		}

		public void FixupFillers()
		{
			int count = 1;
			for (int x = 0; x < Fields.Count; x++)
			{
				Fields[x].FixupFillers(ref count);
			}
		}

		public IList<string> GetTableIndexes()
		{
			List<string> lst = new List<string>();

			for (int x = 0; x < Fields.Count; x++)
			{
				Fields[x].GetTableIndexes(lst);
			}

			return lst;
		}
	}
}
