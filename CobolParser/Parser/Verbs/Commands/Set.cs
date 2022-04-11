using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using CobolParser.Expressions;
using CobolParser.Expressions.Terms;

namespace CobolParser.Verbs
{
	public class Set : IVerb
	{
		public const string Lexum = "SET";

		public ValueList LValue
		{
			get;
			private set;
		}

		public IExpr RValue
		{
			get;
			private set;
		}

		/// <summary>
		/// SET WS-SUB UP BY 1.
		/// SET WS-SUB DOWN BY 1.
		/// </summary>
		public ITerm IncrementValue
		{
			get;
			private set;
		}

		public string UpOrDown
		{
			get;
			private set;
		}

		public Set(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.Set;

			terms.Match(Lexum);

			LValue = new ValueList(terms);

			if (terms.CurrentEquals("TO"))
			{
				terms.Next();
				RValue = IExpr.Parse(terms);
				Debug.Assert(!terms.CurrentEquals("+"));
				Debug.Assert(!terms.CurrentEquals("+1"));
			}
			else if (terms.CurrentEquals("AT"))
			{
				UpOrDown = "AT";
				terms.Next();
				RValue = new Expr(ITerm.Parse(terms));
				terms.MatchOptional(",");
				terms.MatchOptional("SHADOWED");
			}
			else
			{
				UpOrDown = terms.Current.Str;
				terms.MatchOptional("UP");
				terms.MatchOptional("DOWN");
				terms.Match("BY");
				IncrementValue = ITerm.Parse(terms);
			}
		}
	}
}
