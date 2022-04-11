using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Divisions.Proc
{
	public class ReceiveControl
	{
		public string TableOccursTimes
		{
			get;
			protected set;
		}

		public string SyncDepthLimit
		{
			get;
			protected set;
		}

		public string ReplyMessage
		{
			get;
			protected set;
		}
		
		public ReceiveControl(Terminalize terms)
		{
			terms.Match("RECEIVE-CONTROL");
			terms.MatchOptional(".");

			if (terms.CurrentEquals("TABLE"))
			{
				terms.Next();
				terms.Match("OCCURS");
				TableOccursTimes = terms.Current.Str;
				terms.Next();
				terms.MatchOptional("TIMES");
			}

			if (terms.CurrentEquals("SYNCDEPTH"))
			{
				terms.Match("SYNCDEPTH");
				terms.MatchOptional("LIMIT");
				terms.MatchOptional("IS");
				SyncDepthLimit = terms.Current.Str;
				terms.Next();
			}

			terms.Match("REPLY");
			terms.Match("CONTAINS");
			ReplyMessage = terms.Current.Str;
			terms.Next();

			terms.Match("RECORD");
			terms.Match(StringNodeType.Period);
		}
	}
}
