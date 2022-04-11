using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CobolParser.Records;
using DOR.Core;
using CobolParser.Verbs;
using CobolParser.Parser;

namespace CobolParser.Sections
{
	public class FileSection : Section
	{
		public FileSection(Terminalize terms, Storage data)
		: base(terms.Current)
		{
			terms.Match("FILE");
			terms.Match("SECTION");
			terms.Match(StringNodeType.Period);

			Debug.WriteLine("TODO: FILE SECTION");

			Fd lastFd = null;

			while (! terms.CurrentEquals("WORKING-STORAGE"))
			{
				if (VerbLookup.CanCreate(terms.Current, DivisionType.Data))
				{
					IVerb v = VerbLookup.Create(terms, DivisionType.Data);
					Verbs.Add(v);

					if (v is Fd)
					{
						lastFd = (Fd)v;

						if (terms.CurrentEquals("COPY"))
						{
							v = VerbLookup.Create(terms, DivisionType.Data);
							Verbs.Add(v);
							terms.Match(StringNodeType.Period);
						}
					}

					while (StringHelper.IsInt(terms.Current.Str) || terms.Current.StrEquals("DEF"))
					{
						Offset o = data.AddOne(terms, "");
						// Comment out to prevent MessageIn. and MessageOut. prefixes
						//o.IsInFileSection = true;
						//o.FileSectionMessageName = lastFd.MessageName;
						lastFd.DataRecords.Add(o);
					}
				}
				else
				{
					terms.Next();
				}
			}
		}

		public bool IsFdNamed(string name)
		{
			for (int x = 0; x < Verbs.Count; x++)
			{
				IVerb v = Verbs[x];

				if (v is Fd)
				{
					if (((Fd)v).MessageName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
					{
						return true;
					}
				}
			}

			return false;
		}

		public Fd FindFdFor(string name)
		{
			for (int x = 0; x < Verbs.Count; x++)
			{
				IVerb v = Verbs[x];

				if (v is Fd)
				{
					if (((Fd)v).MessageName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
					{
						return (Fd)v;
					}
				}
			}

			return null;
		}

		public Fd FindFdForRecord(string name)
		{
			for (int x = 0; x < Verbs.Count; x++)
			{
				IVerb v = Verbs[x];

				if (v is Fd)
				{
					if (((Fd)v).HadDataRecordNamed(name))
					{
						return (Fd)v;
					}
				}
			}

			return null;
		}

		public List<Fd> Fds()
		{
			List<Fd> l = new List<Fd>();

			for (int x = 0; x < Verbs.Count; x++)
			{
				if (Verbs[x] is Fd)
				{
					l.Add((Fd)Verbs[x]);
				}
			}

			return l;
		}
	}
}
