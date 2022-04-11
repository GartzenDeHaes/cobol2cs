using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

using DOR.Core;
using DOR.Core.Collections;
using CobolParser.Parser;

namespace CobolParser.Records
{
	public class Offset : INamedField
	{
		public Symbol Symbol
		{
			get;
			private set;
		}

		public static bool AttemptNativeTypeConversion
		{
			get;
			set;
		}

		public GuardianPath FileName
		{
			get;
			private set;
		}

		public INamedField Parent
		{
			get;
			set;
		}

		public INamedField TopLevelParent
		{
			get
			{
				INamedField o = Parent;
				while (o != null && o.Parent != null)
				{
					o = o.Parent;
				}
				return o;
			}
		}

		public StringNode Level
		{
			get;
			private set;
		}

		public int LevelAsInt
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			private set;
		}

		private string _fqName = null;
		public string FullyQualifiedName
		{
			get
			{
				if (_fqName == null)
				{
					string name = Name;
					INamedField o = Parent;

					while (o != null)
					{
						name = o.Name + "." + name;
						o = o.Parent;
					}

					_fqName = name;
				}

				return _fqName;
			}
		}

		public OffsetAttributes Attributes
		{
			get;
			private set;
		}

		public Vector<INamedField> SubGroups
		{
			get;
			private set;
		}

		public Redefine Redefines
		{
			get;
			private set;
		}

		public string Comments
		{
			get { return Attributes.Comments; }
			set { Attributes.Comments = value; }
		}

		public int Occurances
		{
			get 
			{
				return Attributes.Occures == null ? 1 : Attributes.Occures.MaximumTimes;
			}
		}

		public bool SpecialNameHandling
		{
			get;
			set;
		}

		public bool IsInFileSection
		{
			get;
			set;
		}

		public string FileSectionMessageName
		{
			get;
			private set;
		}

		public List<string> RedefinesHints { get; private set; }
		public List<string> RedefinedByHints { get; private set; }

		public Offset()
		{
			RedefinesHints = new List<string>();
			RedefinedByHints = new List<string>();

			SubGroups = new Vector<INamedField>();

			if (null == Attributes)
			{
				Attributes = new OffsetAttributes();
			}
		}

		public Offset(Terminalize terms, Offset parent = null)
		: this()
		{
			Parent = parent;
			Parse(terms);
		}

		public int CountGroupsNamed(string name)
		{
			int count = 0;

			foreach (var o in SubGroups)
			{
				if (o.Name == name && o.SubGroups.Count > 0)
				{
					count++;
				}

				count += o.CountGroupsNamed(name);
			}

			return count;
		}

		public void FixupFillers(ref int count)
		{
			if (Name.Equals("FILLER", StringComparison.InvariantCultureIgnoreCase))
			{
				Name += count++;
			}

			foreach (var o in SubGroups)
			{
				o.FixupFillers(ref count);
			}
		}

		public INamedField LocateField(string name, string parentName)
		{
			if (name.Equals(Name, StringComparison.InvariantCultureIgnoreCase) && HasParentNamed(parentName))
			{
				return this;
			}

			if (!String.IsNullOrEmpty(parentName) && parentName == FileSectionMessageName)
			{
				if (name.Equals(Name, StringComparison.InvariantCultureIgnoreCase))
				{
					return this;
				}
				foreach (var o in SubGroups)
				{
					INamedField f = o.LocateField(name, null);
					if (f != null)
					{
						return f;
					}
				}
			}

			foreach (var o in SubGroups)
			{
				INamedField f = o.LocateField(name, parentName);
				if (f != null)
				{
					return f;
				}
			}

			foreach (var e in Attributes.ValueChoices)
			{
				if (e.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				{
					return e;
				}
			}

			return null;
		}

		public void Parse(Terminalize terms)
		{
			FileName = terms.Current.FileName;

			Level = terms.Current;
			if (terms.CurrentEquals("DEF"))
			{
				LevelAsInt = 1;
			}
			else
			{
				LevelAsInt = Int32.Parse(Level.Str);
			}
			terms.Next();

			StringNode nameNode = null;

			if (terms.CurrentEquals("PIC"))
			{
				Name = "";
			}
			else
			{
				nameNode = terms.Current;
				Name = terms.Current.Str;
				terms.Match(StringNodeType.Word);
			}

			if (terms.Current.Type != StringNodeType.Period)
			{
				Attributes.Parse(Name, terms);
			}

			if (terms.CurrentEquals("REDEFINES"))
			{
				Redefines = new Redefine(terms);
				
				Debug.Assert(Attributes.Occures.MaximumTimes == 1);
				Attributes.Occures = Redefines.Occures;

				if (Redefines.RedefAsPic != null)
				{
					Debug.Assert(Attributes.Pic == null);
					Attributes.Pic = Redefines.RedefAsPic;
				}
			}

			terms.MatchOptional(StringNodeType.Period);

			if (nameNode != null)
			{
				Symbol = CobolProgram.CurrentSymbolTable.Add(this, nameNode);
			}

			while 
			(
				!terms.IsIterationComplete && 
				StringHelper.IsInt(terms.Current.Str) &&
				! Level.StrEquals(terms.Current.Str)
			)
			{
				int lvl = Int32.Parse(terms.Current.Str);
				if (lvl <= LevelAsInt)
				{
					break;
				}

				if (lvl == 88)
				{
					Attributes.ValueChoices.Add(new FieldOption88(terms, this));
					continue;
				}

				Offset f = new Offset(terms, this);
				SubGroups.Add(f);
			}
		}

		public void RecordDef()
		{
			ImportManager.AddDdlDef(this);

			foreach (Offset o in SubGroups)
			{
				o.RecordDef();
			}
		}

		public int Length()
		{
			int count = Attributes.Occures == null ? 1 : Attributes.Occures.MaximumTimes;
			return SingleRecordLength() * count;
		}

		public int SingleRecordLength(bool countRedefines = false)
		{
			if (Attributes.Pic != null)
			{
				return Attributes.Pic.Length;
			}

			if (!countRedefines && Redefines != null)
			{
				return 0;
			}

			int len = 0;

			foreach (var f in SubGroups)
			{
				len += f.Length();
			}

			return len;
		}

		public bool HasParentNamed(string name)
		{
			if (String.IsNullOrEmpty(name))
			{
				return true;
			}

			INamedField p = Parent;

			while (p != null)
			{
				if (p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
				p = p.Parent;
			}

			return false;
		}

		public INamedField FindAncesterNamed(string name)
		{
			if (Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
			{
				return this;
			}

			INamedField p = Parent;

			while (p != null)
			{
				if (p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				{
					return p;
				}
				p = p.Parent;
			}

			return null;
		}

		public string FindAncesterRedefines()
		{
			if (Redefines != null)
			{
				return Redefines.RedefinesOffsetNamed;
			}

			INamedField p = Parent;

			while (p != null)
			{
				if (p.Redefines != null)
				{
					return p.Redefines.RedefinesOffsetNamed;
				}
				p = p.Parent;
			}

			return "";
		}

		public void GetTableIndexes(IList<string> lst)
		{
			if (!String.IsNullOrEmpty(Attributes.IndexedBy))
			{
				lst.Add(Attributes.IndexedBy);
			}

			foreach (var o in SubGroups)
			{
				o.GetTableIndexes(lst);
			}
		}

		public INamedField FindChildNamed(string name)
		{
			foreach (var o in SubGroups)
			{
				if (o.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				{
					return o;
				}
			}

			return null;
		}

		public bool Has88()
		{
			foreach (var o in SubGroups)
			{
				if (o.LevelAsInt == 88)
				{
					return true;
				}
				if (o.Has88())
				{
					return true;
				}
			}

			return false;
		}

		public void AddRedefinedBy(string name)
		{
			RedefinedByHints.Add(name);

			foreach (var o in SubGroups)
			{
				o.AddRedefinedBy(name);
			}
		}

		public void AddRedefines(string name)
		{
			RedefinesHints.Add(name);

			foreach (var o in SubGroups)
			{
				o.AddRedefines(name);
			}
		}

		public void FixupRedefineHints(IOffsetLocator storage)
		{
			if (Redefines != null)
			{
				INamedField o = (Offset)storage.LocateField(Redefines.RedefinesOffsetNamed, null);
				if (o == null)
				{
					throw new Exception(Redefines.RedefinesOffsetNamed + " not found in Offset.FixupRedefineHints");
				}

				o.AddRedefinedBy(FullyQualifiedName);
				AddRedefines(o.FullyQualifiedName);
			}

			foreach (var o in SubGroups)
			{
				o.FixupRedefineHints(storage);
			}
		}

		public bool CanBeNative()
		{
			bool ret = AttemptNativeTypeConversion &&
				RedefinedByHints.Count == 0 &&
				RedefinesHints.Count == 0 &&
				SubGroups.Count == 0 &&
				Occurances < 2 &&
				Attributes.ValueChoices.Count == 0;

			if (ret)
			{
				INamedField p = Parent;
				while (p != null)
				{
					if 
					(
						p.Symbol != null && 
						(
							p.Symbol.References.Count > 0 ||
							p.Symbol.SendReferences.Count > 0
						)
					)
					{
						return false;
					}
					p = p.Parent;
				}
			}

			return ret;
		}

		public string ConversionMethod()
		{
			if (Attributes.Pic == null)
			{
				return ".ToString()";
			}
			string tp = Attributes.Pic.CsClrTypeName();
			if (tp == "string")
			{
				return ".ToString()";
			}
			if (tp == "int" || tp == "long")
			{
				return ".ToInt()";
			}
			if (tp == "decimal")
			{
				return ".ToDecimal()";
			}

			return "";
		}
	}
}
