using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.Expressions;

namespace CobolParser.Verbs
{
	public class Close : IVerb
	{
		public const string Lexum = "CLOSE";

		public ValueList FileNames
		{
			get;
			private set;
		}

		public Close(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.Close;

			terms.Match(Lexum);
			FileNames = new ValueList(terms);
		}
	}
}
