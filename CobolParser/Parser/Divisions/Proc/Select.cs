using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.Expressions;
using System.Diagnostics;

namespace CobolParser.Divisions.Proc
{
	public class Select
	{
		public string MessageName
		{
			get;
			protected set;
		}

		public string FileName
		{
			get;
			protected set;
		}

		public string StatusFunction
		{
			get;
			protected set;
		}

		public string OrganizationIs
		{
			get;
			protected set;
		}

		public string AccessModeIs
		{
			get;
			protected set;
		}

		public string ReserveAreas
		{
			get;
			protected set;
		}

		public ITerm RecordKey
		{
			get;
			protected set;
		}

		public ITerm RelativeKey
		{
			get;
			protected set;
		}

		public List<AlternateRecordKey> AlternateKeyRecord
		{
			get;
			protected set;
		}

		public bool IsOptional
		{
			get;
			protected set;
		}

		public Select(Terminalize terms)
		{
			AlternateKeyRecord = new List<AlternateRecordKey>();

			terms.Match("SELECT");

			if (terms.CurrentEquals("OPTIONAL"))
			{
				terms.Next();
				IsOptional = true;
			}

			MessageName = terms.Current.Str;
			terms.Next();

			while (terms.Current.Type != StringNodeType.Period)
			{
				if (terms.CurrentEquals("FILE"))
				{
					terms.Next();
					terms.Match("STATUS");
					terms.MatchOptional("IS");

					StatusFunction = terms.Current.Str; ;
					terms.Next();
					continue;
				}
				if (terms.CurrentEquals("ORGANIZATION"))
				{
					terms.Next();
					terms.MatchOptional("IS");
					OrganizationIs = terms.Current.Str;
					terms.Next();
					continue;
				}
				if (terms.CurrentEquals("ASSIGN"))
				{
					terms.Next();
					terms.MatchOptional("TO");
					FileName = terms.Current.Str;
					terms.Next();
					continue;
				}
				if (terms.CurrentEquals("ACCESS"))
				{
					terms.Next();
					terms.MatchOptional("MODE");
					terms.MatchOptional("IS");

					AccessModeIs = terms.Current.Str;
					terms.Next();
					continue;
				}
				if (terms.CurrentEquals("RESERVE"))
				{
					terms.Next();
					ReserveAreas = terms.Current.Str;
					terms.Next();
					terms.Match("AREAS");
					continue;
				}
				if (terms.CurrentEquals("RECORD"))
				{
					terms.Next();
					terms.Match("KEY");
					terms.MatchOptional("IS");
					RecordKey = ITerm.Parse(terms);
					continue;
				}

				if (terms.CurrentEquals("RELATIVE"))
				{
					terms.Next();
					terms.Match("KEY");
					terms.Match("IS");
					RelativeKey = ITerm.Parse(terms);
					continue;
				}

				if (terms.CurrentEquals("ALTERNATE"))
				{
					AlternateKeyRecord.Add(new AlternateRecordKey(terms));
					continue;
				}

				Debug.Fail(terms.Current.Str);
			}

			terms.Match(StringNodeType.Period);
		}
	}
}
