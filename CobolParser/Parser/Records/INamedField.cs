using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.Parser;
using DOR.WorkingStorage.Pic;
using DOR.Core.Collections;

namespace CobolParser.Records
{
	public interface INamedField
	{
		GuardianPath FileName
		{
			get;
		}

		Symbol Symbol
		{
			get;
		}

		INamedField Parent
		{
			get;
		}

		string Name
		{
			get;
		}

		string FullyQualifiedName
		{
			get;
		}

		int LevelAsInt
		{
			get;
		}

		OffsetAttributes Attributes
		{
			get;
		}

		INamedField TopLevelParent
		{
			get;
		}

		int Occurances
		{
			get;
		}

		bool SpecialNameHandling
		{
			get;
			set;
		}

		Vector<INamedField> SubGroups
		{
			get;
		}

		Redefine Redefines
		{
			get;
		}

		List<string> RedefinesHints { get; }
		List<string> RedefinedByHints { get; }

		string FileSectionMessageName
		{
			get;
		}

		string Comments
		{
			get;
			set;
		}

		bool HasParentNamed(string name);
		bool CanBeNative();
		INamedField FindAncesterNamed(string name);
		string ConversionMethod();
		int SingleRecordLength(bool countRedefines = false);
		INamedField LocateField(string name, string parentName);
		int Length();
		void GetTableIndexes(IList<string> lst);
		void FixupRedefineHints(IOffsetLocator storage);
		void FixupFillers(ref int count);
		string FindAncesterRedefines();
		int CountGroupsNamed(string name);
		void AddRedefines(string name);
		void AddRedefinedBy(string name);
		
		bool Has88();
	}
}
