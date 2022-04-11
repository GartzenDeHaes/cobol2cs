using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.Expressions;

namespace CobolParser.Divisions.Proc
{
	public class AlternateRecordKey
	{
		public ITerm RecordName
		{
			get;
			private set;
		}

		public bool IsDupsOk
		{
			get;
			private set;
		}

		public AlternateRecordKey(Terminalize terms)
		{
			terms.Match("ALTERNATE");
			terms.Match("RECORD");
			terms.Match("KEY");
			terms.Match("IS");

			RecordName = ITerm.Parse(terms);

			if (terms.CurrentEquals("WITH"))
			{
				IsDupsOk = true;
				terms.Next();
				terms.Match("DUPLICATES");
			}
		}
	}
}
