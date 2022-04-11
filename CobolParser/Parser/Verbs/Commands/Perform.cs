using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CobolParser.Verbs.Phrases;

namespace CobolParser.Verbs
{
	public class Perform : IVerb
	{
		public const string Lexum = "PERFORM";

		private IPerformInner _inner;
		public IPerformInner PerformInner
		{
			get { return _inner; }
		}

		public int LineNumber
		{
			get;
			private set;
		}

		public Perform(Terminalize terms)
		: base(terms.Current)
		{
			LineNumber = terms.Current.LineNumber;
			terms.Match(Lexum);

			if (VerbLookup.CanCreate(terms.Current, DivisionType.Procedure))
			{
				Type = VerbType.PerformStmts;

				_inner = new PerformStatements(terms);
			}
			else if (terms.CurrentEquals("ONE"))
			{
				Type = VerbType.PerformOneOf;

				_inner = new PerformOneOf(terms);
			}
			else
			{
				Type = VerbType.PerformCall;

				_inner = new PerformCall(terms);
			}

			if (terms.CurrentEquals("END-PERFORM"))
			{
				terms.Next();
			}
		}
	}
}
