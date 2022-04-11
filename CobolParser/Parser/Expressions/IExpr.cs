using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CobolParser.Expressions;
using DOR.Core;
using CobolParser.Expressions.Terms;

namespace CobolParser
{
	public abstract class IExpr
	{
		public abstract bool IsStringLit { get; }
		public abstract bool IsNumber { get; }
		public abstract int Count { get; }

		public abstract string ToDocumentationString();
		public abstract string ToCListStringList();
		public abstract string ToStringTryConvertToInt();

		public abstract IExpr Clone();

		public static IExpr Parse(Terminalize terms)
		{
			if 
			(
				terms.CurrentNextEquals(1, "OF") ||
				terms.CurrentNextEquals(1, "IN") ||
				terms.CurrentEquals("NOT") ||
				terms.CurrentEquals("FUNCTION") ||
				Operator.IsOperator(terms.CurrentNext(1)) ||
				terms.CurrentNext(1).Type == StringNodeType.Period ||
				VerbLookup.CanCreate(terms.CurrentNext(1), DivisionType.Procedure)
			)
			{
				return new Expr(terms);
			}

			if (terms.CurrentNext(1).Type == StringNodeType.Comma)
			{
				return new ValueList(terms);
			}

			if 
			(
				terms.CurrentNextEquals(1, "THRU") ||
				terms.CurrentNextEquals(1, "THROUGH")
			)
			{
				return new Expr(new Range(terms));
				//return new ValueList(terms);
			}

			if 
			(
				(terms.Current.Type == StringNodeType.Word ||
				terms.Current.Type == StringNodeType.Number ||
				terms.Current.Type == StringNodeType.Quoted) &&

				(terms.CurrentNext(1).Type == StringNodeType.Word ||
				terms.CurrentNext(1).Type == StringNodeType.Number ||
				terms.CurrentNext(1).Type == StringNodeType.Quoted)
			)
			{
				if (terms.CurrentNextEquals(1, "NUMERIC") || terms.CurrentNextEquals(1, "ALSO"))
				{
					return new Expr(terms);
				}
				if (ValueList.IsExitTerm(terms, 1))
				{
					return new Expr(ITerm.Parse(terms));
				}
				//if (terms.Current.Type == StringNodeType.Quoted && terms.CurrentNext(1).Type != StringNodeType.Comma)
				//{
				//    return new Expr(new StringLit(terms));
				//}
				return new ValueList(terms);
			}

			if (terms.Current.Str == "X" && terms.CurrentNext(1).Type == StringNodeType.Quoted)
			{
				return new Expr(terms);
			}

			return new Expr(terms);
		}
	}
}
