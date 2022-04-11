using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CobolParser.Records;
using CobolParser.Expressions;
using CobolParser.Verbs;

namespace CobolParser.Parser
{
	public class Symbol
	{
		public StringNode Lexum
		{
			get;
			private set;
		}

		public INamedField Record
		{
			get;
			private set;
		}

		public IList<ITerm> References
		{
			get;
			private set;
		}

		public IList<Send> SendReferences
		{
			get;
			private set;
		}

		public IList<ScreenField> ScreenReferences
		{
			get;
			private set;
		}

		public Symbol(INamedField rec, StringNode lex)
		{
			Lexum = lex;
			Record = rec;
			References = new List<ITerm>();
			SendReferences = new List<Send>();
			ScreenReferences = new List<ScreenField>();
		}
	}
}
