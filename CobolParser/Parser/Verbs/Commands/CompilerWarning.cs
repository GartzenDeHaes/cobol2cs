using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Verbs
{
	public class CompilerWarning : IVerb
	{
		public const string Lexum = "!";

		public string Text
		{
			get;
			private set;
		}

		public CompilerWarning(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.CompilerWarn;

			Text = terms.Current.Str;
			int lineNum = terms.Current.LineNumber;

			terms.Match("!");

			if (lineNum == terms.Current.LineNumber)
			{
				Text += " " + terms.Current.Str;
			}
		}
	}
}
