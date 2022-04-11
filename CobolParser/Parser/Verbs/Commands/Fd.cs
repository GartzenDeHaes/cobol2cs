using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CobolParser.Records;

namespace CobolParser.Verbs
{
	public class Fd : IVerb
	{
		public static string Lexum = "FD";

		public string MessageName
		{
			get;
			protected set;
		}

		public string LabelRecordSetting
		{
			get;
			protected set;
		}

		public string RecordMinChars
		{
			get;
			protected set;
		}

		public string RecordMaxChars
		{
			get;
			protected set;
		}

		public string BlockSize
		{
			get;
			protected set;
		}

		public string DataRecordIs
		{
			get;
			protected set;
		}

		public List<Offset> DataRecords
		{
			get;
			set;
		}

		public Fd(Terminalize terms)
		: base(terms.Current)
		{
			DataRecords = new List<Offset>();

			terms.Match("FD");

			MessageName = terms.Current.Str;
			terms.Match(StringNodeType.Word);

			while 
			(
				terms.Current.Type != StringNodeType.Period && 
				!terms.CurrentEquals("COPY") &&
				!terms.CurrentEquals("01")
			)
			{
				if (terms.CurrentEquals("LABEL"))
				{
					terms.Match("LABEL");
					terms.Match("RECORDS");
					terms.MatchOptional("ARE");
					LabelRecordSetting = terms.Current.Str;
					terms.Next();
				}
				else if (terms.CurrentEquals("RECORD"))
				{
					terms.Next();
					terms.Match("CONTAINS");
					RecordMinChars = terms.Current.Str;
					terms.Next();

					if (terms.CurrentEquals("TO"))
					{
						terms.Next();
						RecordMaxChars = terms.Current.Str;
						terms.Next();
					}

					if (terms.CurrentEquals("Characters"))
					{
						terms.Next();
					}
				}
				else if (terms.CurrentEquals("BLOCK"))
				{
					terms.Next();
					terms.Match("CONTAINS");

					BlockSize = terms.Current.Str;
					terms.Next();

					terms.Match("RECORDS");
					terms.Next();
				}
				else if (terms.CurrentEquals("LABEL"))
				{
					terms.Next();
					terms.MatchOptional("RECORD");
					terms.MatchOptional("RECORDS");
					terms.MatchOptional("ARE");
					terms.MatchOptional("OMITTED");
				}
				else if (terms.CurrentEquals("DATA"))
				{
					terms.Next();
					terms.Match("RECORD");
					terms.MatchOptional("IS");
					DataRecordIs = terms.Current.Str;
					terms.Next();
				}
				else if (terms.CurrentEquals("RECORD"))
				{
					terms.Next();
					terms.MatchOptional("IS");
					terms.MatchOptional("VARYING");
					terms.MatchOptional("FROM");

					terms.Next();
					terms.Match("TO");
					terms.Next();
					terms.MatchOptional("CHARACTERS");
				}
				else
				{
					Debug.Fail(terms.Current.Str);
				}
			}

			if (!terms.CurrentEquals("COPY") && !terms.CurrentEquals("01"))
			{
				terms.Match(StringNodeType.Period);
			}
		}

		public bool HadDataRecordNamed(string name)
		{
			foreach (Offset o in DataRecords)
			{
				if (o.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}

			return false;
		}
	}
}
