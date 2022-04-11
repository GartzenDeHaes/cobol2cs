using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.Expressions;

namespace CobolParser.Verbs
{
	public class Enter : IVerb
	{
		public const string Lexum = "ENTER";

		public string LibName
		{
			get;
			private set;
		}

		public string FunctionName
		{
			get;
			private set;
		}

		public ValueList Arguements
		{
			get;
			private set;
		}

		public ITerm Returns
		{
			get;
			private set;
		}

		public Enter(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.Enter;

			terms.Match(Lexum);
			terms.MatchOptional("TAL");
			FunctionName = terms.Current.Str;
			terms.Next();

			if (terms.CurrentEquals("OF") || terms.CurrentEquals("IN"))
			{
				terms.Next();
				LibName = terms.Current.Str;
				terms.Next();
			}

			if (terms.CurrentEquals("USING"))
			{
				terms.Match("USING");
				Arguements = new ValueList(terms);
			}

			if (terms.CurrentEquals("GIVING"))
			{
				terms.Next();
				Returns = ITerm.Parse(terms);
			}
		}
	}
}
