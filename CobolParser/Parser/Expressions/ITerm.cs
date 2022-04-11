using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DOR.Core;
using CobolParser.Expressions.Terms;
using CobolParser.Parser;

namespace CobolParser.Expressions
{
	public abstract class ITerm
	{
		public Symbol Symbol
		{
			get;
			protected set;
		}

		public abstract string ToDocumentationString();

		public virtual string ToCListStringList()
		{
			return StringHelper.EnsureQuotes(ToDocumentationString());
		}

		public static ITerm Parse(Terminalize terms)
		{
			// COBOL allows invalid commas
			terms.MatchOptional(",");

			if 
			(
				terms.CurrentEquals("END-EVALUATE") ||
				terms.CurrentEquals("END-IF") ||
				terms.CurrentEquals("END-PERFORM") ||
				terms.CurrentEquals("END-SEARCH") ||
				terms.CurrentEquals("END-INSPECT") ||
				terms.CurrentEquals("END-STRING") ||
				terms.CurrentEquals("END-UNSTRING") ||
				//terms.CurrentEquals("END-TRANSACTION") ||
				terms.CurrentEquals("OUTPUT") ||
				terms.CurrentEquals("DELIMITER")
			)
			{
				return null;
			}

			if 
			(
				terms.Current.Type == StringNodeType.Period || 
				terms.Current.Type == StringNodeType.QuestionMark ||
				VerbLookup.CanCreate(terms.Current, DivisionType.Procedure)
			)
			{
				return null;
			}
			if 
			(
				terms.CurrentNextEquals(1, "THRU") ||
				terms.CurrentNextEquals(1, "THROUGH") ||
				terms.CurrentNext(1).Type == StringNodeType.Colon
			)
			{
				return new Range(terms);
			}
			if (StringHelper.IsNumeric(terms.Current.Str))
			{
				return new Number(terms);
			}
			if (terms.Current.Type == StringNodeType.Quoted)
			{
				return new StringLit(terms);
			}
			if (terms.CurrentEquals("ZERO"))
			{
				terms.Next();
				return new Zero();
			}
			if (terms.CurrentEquals("ZEROS"))
			{
				terms.Next();
				return new Zeroes();
			}
			if (terms.CurrentEquals("ZEROES"))
			{
				terms.Next();
				return new Zeroes();
			}
			if (terms.CurrentEquals("SPACES"))
			{
				terms.Next();
				return new Spaces();
			}
			if (terms.CurrentEquals("SPACE"))
			{
				terms.Next();
				return new Space();
			}
			if (terms.CurrentEquals("QUOTE"))
			{
				terms.Next();
				return new Quote();
			}
			if (terms.CurrentEquals("FALSE"))
			{
				return new Bool(terms);
			}
			if (terms.CurrentEquals("TRUE"))
			{
				return new Bool(terms);
			}
			if (terms.CurrentEquals("NUMERIC"))
			{
				return new Numeric(terms);
			}
			if (terms.CurrentEquals("POSITIVE"))
			{
				return new Positive(terms);
			}
			if (terms.CurrentEquals("NEGATIVE"))
			{
				return new Negative(terms);
			}
			if (terms.CurrentEquals("ALPHABETIC"))
			{
				return new Alphabetic(terms);
			}
			if (terms.CurrentEquals("FUNCTION"))
			{
				return new Function(terms);
			}
			if 
			(
				terms.CurrentEquals("COMP") ||
				terms.CurrentEquals("COMP-2") ||
				terms.CurrentEquals("COMP-3") ||
				terms.CurrentEquals("COMP-4")
			)
			{
				terms.Next();
				terms.MatchOptional("SYNC");
				return null;
			}
			if (terms.Current.Type == StringNodeType.LPar)
			{
				return new ExprTerm(terms);
			}
			if (terms.Current.Str == "X" && terms.CurrentNext(1).Type == StringNodeType.Quoted)
			{
				return new StringLit(terms);
			}
			if 
			(
				terms.CurrentNextEquals(1, "OF") ||
				terms.CurrentNextEquals(1, "IN")
			)
			{
				return new OffsetReference(terms);
			}
			if (terms.Current.Type == StringNodeType.Word)
			{
				if
				(
					!terms.CurrentEquals("SIGN") &&
					!terms.CurrentEquals("PIC") &&
					!terms.CurrentEquals("PICTURE")
				)
				{
					return new Id(terms);
				}
			}

#if DEBUG
			if (terms.Current.Str != "SIGN" && terms.Current.Str != "PIC")
			{
				Debug.WriteLine("Term not found " + terms.Current.Str);
			}
#endif
			return null;
		}
	}
}
