using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.Verbs.Phrases;
using CobolParser.Expressions;

namespace CobolParser.Verbs
{
	public class Call : OnErrorVerb
	{
		public static string Lexum = "CALL";

		public string LibName
		{
			get;
			private set;
		}

		public string SubRoutine
		{
			get;
			private set;
		}

		public ValueList UsingThis
		{
			get;
			private set;
		}

		public Call(Terminalize terms)
		{
			Type = VerbType.Call;

			terms.Match(Lexum);

			SubRoutine = terms.Current.Str;
			terms.Next();

			if (terms.CurrentEquals("OF"))
			{
				terms.Next();
				LibName = terms.Current.Str;
				terms.Next();
			}

			if (terms.CurrentEquals("USING"))
			{
				terms.Next();
				UsingThis = new ValueList(terms);
			}

			base.Parse(terms);
		}
	}
}
