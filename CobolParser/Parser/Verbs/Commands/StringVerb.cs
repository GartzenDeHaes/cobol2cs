using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DOR.Core.Collections;
using CobolParser.Expressions;
using CobolParser.Verbs.Phrases;
using CobolParser.Parser.Verbs.Phrases;

namespace CobolParser.Verbs
{
	public class StringVerb : IVerb
	{
		public const string Lexum = "STRING";

		public bool IsUnString
		{
			get;
			private set;
		}

		public Vector<StringItem> Items = new Vector<StringItem>();

		public Vector<UnstringItem> UnstringItems = new Vector<UnstringItem>();

		public ValueList Dest
		{
			get;
			private set;
		}

		public ITerm WithPointerIs
		{
			get;
			private set;
		}

		public StatementBlock OnOverflowStmts
		{
			get;
			private set;
		}

		public StringVerb(Terminalize terms)
		: base(terms.Current)
		{
			if (terms.CurrentEquals("UNSTRING"))
			{
				Type = VerbType.Unstring;

				IsUnString = true;
				terms.Match("UNSTRING");
			}
			else
			{
				Type = VerbType.StringVerb;

				terms.Match(Lexum);
			}

			while (! terms.CurrentEquals("INTO"))
			{
				terms.MatchOptional(",");
				Items.Add(new StringItem(terms));
			}

			terms.Match("INTO");

			if (IsUnString)
			{
				while 
				(
					!terms.CurrentEquals(".") &&
					!terms.CurrentEquals("END-STRING") &&
					!terms.CurrentEquals("END-UNSTRING") &&
					!terms.CurrentEquals("WITH") &&
					!terms.CurrentEquals("ON")
				)
				{
					UnstringItems.Add(new UnstringItem(terms));
				}
			}
			else
			{
				Dest = new ValueList(terms);
			}

			if (terms.CurrentEquals("WITH"))
			{
				terms.Match("WITH");
				terms.Match("POINTER");
				WithPointerIs = ITerm.Parse(terms);
			}

			if (terms.CurrentEquals("ON"))
			{
				terms.Match("ON");
				terms.Match("OVERFLOW");
				OnOverflowStmts = new StatementBlock(terms);
			}

			terms.MatchOptional("END-STRING");
			terms.MatchOptional("END-UNSTRING");
		}
	}
}
