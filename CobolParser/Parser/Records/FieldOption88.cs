using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CobolParser.Parser;
using DOR.Core.Collections;

namespace CobolParser.Records
{
	public class FieldOption88 : INamedField
	{
		public GuardianPath FileName
		{
			get { return Parent.FileName; }
		}

		public Symbol Symbol
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

		public IExpr Values
		{
			get;
			private set;
		}

		public bool IsAll
		{
			get;
			private set;
		}

		public INamedField Parent
		{
			get;
			private set;
		}

		public OffsetAttributes Attributes
		{
			get;
			private set;
		}

		public INamedField TopLevelParent
		{
			get
			{
				INamedField o = Parent;
				while (o.Parent != null)
				{
					o = o.Parent;
				}
				return o;
			}
		}

		public int LevelAsInt
		{
			get { return 88; }
		}

		public int Occurances
		{
			get { return 1; }
		}

		public bool SpecialNameHandling
		{
			get;
			set;
		}

		public Vector<INamedField> SubGroups
		{
			get { return new Vector<INamedField>(); }
		}

		public List<string> RedefinesHints 
		{ 
			get; private set; 
		}
		
		public List<string> RedefinedByHints 
		{ 
			get; private set; 
		}

		public Redefine Redefines
		{
			get { return null; }
		}

		public string FileSectionMessageName
		{
			get { return ""; }
		}

		public string Comments
		{
			get;
			set;
		}

		public FieldOption88(Terminalize terms, Offset parent)
		{
			List<string> RedefinesHints = new List<string>();
			List<string> RedefinedByHints = new List<string>();

			Parent = parent;

			terms.Match("88");
			Name = terms.Current.Str;
			Symbol = CobolProgram.CurrentSymbolTable.Add(this, terms.Current);
			terms.Next();

			terms.MatchOptional("VALUE");
			terms.MatchOptional("VALUES");
			terms.MatchOptional("IS");
			terms.MatchOptional("ARE");

			if (terms.Current.Str.Equals("ALL", StringComparison.InvariantCultureIgnoreCase))
			{
				IsAll = true;
				terms.Next();
			}

			Values = IExpr.Parse(terms);

			terms.Match(StringNodeType.Period);
		}

		public bool HasParentNamed(string name)
		{
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
			Debug.Fail("Shouldn't happen");
			return null;
		}

		public bool Has88()
		{
			return false;
		}

		public bool CanBeNative()
		{
			return false;
		}

		public string ConversionMethod()
		{
			return "";
		}

		public int SingleRecordLength(bool countRedefines = false)
		{
			return 0;
		}

		public INamedField LocateField(string name, string parentName)
		{
			return null;
		}

		public int Length()
		{
			return 0;
		}

		public void GetTableIndexes(IList<string> lst)
		{
		}

		public void FixupRedefineHints(IOffsetLocator storage)
		{
		}

		public void FixupFillers(ref int count)
		{
		}

		public string FindAncesterRedefines()
		{
			return "";
		}

		public int CountGroupsNamed(string name)
		{
			return 0;
		}

		public void AddRedefines(string name)
		{
		}

		public void AddRedefinedBy(string name)
		{
		}
	}
}
