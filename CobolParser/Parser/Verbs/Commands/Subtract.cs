using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CobolParser.Expressions;
using CobolParser.Verbs.Phrases;

namespace CobolParser.Verbs
{
	public class Subtract : OnErrorVerb
	{
		public static string Lexum = "SUBTRACT";

		public ValueList SubtractThis
		{
			get;
			private set;
		}

		public ValueList FromThis
		{
			get;
			private set;
		}

		public ValueList GivingTo
		{
			get;
			private set;
		}

		public Subtract(Terminalize terms)
		{
			Type = VerbType.Sub;

			terms.Match(Lexum);

			SubtractThis = new ValueList(terms);
			terms.Match("FROM");
			FromThis = new ValueList(terms);

			if (terms.CurrentEquals("GIVING"))
			{
				terms.Next();
				GivingTo = new ValueList(terms);
			}

			base.Parse(terms);
		}
	}
}
