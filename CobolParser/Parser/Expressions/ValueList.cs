using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core;
using CobolParser.Expressions.Terms;

namespace CobolParser.Expressions
{
	public class ValueList: IExpr
	{
		public List<ITerm> Items
		{
			get;
			private set;
		}

		public override int Count
		{
			get { return Items.Count; }
		}

		public override bool IsStringLit
		{
			get
			{
				return Items.Count == 1 && Items[0] is StringLit;
			}
		}

		public override bool IsNumber
		{
			get
			{
				return Items.Count == 1 && Items[0] is Number;
			}
		}

		protected ValueList()
		{
			Items = new List<ITerm>();
		}

		public ValueList(Terminalize terms)
		: this()
		{
			ITerm t = null;
			while 
			(
				terms.Current.Type != StringNodeType.Period && 
				(t = ITerm.Parse(terms)) != null
			)
			{
				Items.Add(t);

				while (terms.Current.Type == StringNodeType.Comma)
				{
					terms.Next();
				}

				if (IsExitTerm(terms, 0))
				{
					break;
				}
			}
		}

		public static bool IsExitTerm(Terminalize terms, int pos)
		{
			return
				terms.Current.Type == StringNodeType.RPar ||
				Operator.IsOperator(terms.Current) ||
				VerbLookup.CanCreate(terms.Current, DivisionType.Procedure) ||
				terms.CurrentNextEquals(pos, "INSPECT") || //< remove later
				terms.CurrentNextEquals(pos, "IF") || //< remove later
				terms.CurrentNextEquals(pos, "OCCURS") ||
				(terms.CurrentNextEquals(pos, "MUST") && terms.CurrentNextEquals(pos + 1, "BE")) ||
				terms.CurrentNextEquals(pos, "USING") ||
				terms.CurrentNextEquals(pos, "FROM") ||
				terms.CurrentNextEquals(pos, "TO") ||
				terms.CurrentNextEquals(pos, "MDTON") ||
				terms.CurrentNextEquals(pos, "UNDERLINE") ||
				terms.CurrentNextEquals(pos, "PIC") ||
				terms.CurrentNextEquals(pos, "UPSHIFT") ||
				terms.CurrentNextEquals(pos, "FROM") ||
				terms.CurrentNextEquals(pos, "DIM") ||
				terms.CurrentNextEquals(pos, "VALUE") ||
				terms.CurrentNextEquals(pos, "ADVISORY") ||
				terms.CurrentNextEquals(pos, "PROTECTED") ||
				terms.CurrentNextEquals(pos, "NORMAL") ||
				terms.CurrentNextEquals(pos, "WHEN") ||
				terms.CurrentNextEquals(pos, "TO") ||
				terms.CurrentNextEquals(pos, "ON") ||
				terms.CurrentNextEquals(pos, "DEPENDING") ||
				terms.CurrentNextEquals(pos, "YIELDS") ||
				terms.CurrentNextEquals(pos, "EXEC") ||
				terms.CurrentNextEquals(pos, "WHEN") ||
				terms.CurrentNextEquals(pos, "GIVING") ||
				terms.CurrentNextEquals(pos, "UP") ||
				terms.CurrentNextEquals(pos, "DOWN") ||
				terms.CurrentNextEquals(pos, "INDEXED") ||
				terms.CurrentNextEquals(pos, "SHADOWED") ||
				terms.CurrentNextEquals(pos, "AT") ||
				terms.CurrentNextEquals(pos, "UNTIL") ||
				terms.CurrentNextEquals(pos, "ESCAPE") ||
				terms.CurrentNextEquals(pos, "FILE") ||
				terms.CurrentNextEquals(pos, "REVERSE") ||
				terms.CurrentNextEquals(pos, "ELSE");
		}

		public override string ToDocumentationString()
		{
			StringBuilder buf = new StringBuilder();

			foreach (ITerm term in Items)
			{
				if (buf.Length != 0)
				{
					buf.Append(' ');
				}
				buf.Append(term.ToDocumentationString());
			}

			return buf.ToString().TrimEnd();
		}

		public override string ToCListStringList()
		{
			StringBuilder buf = new StringBuilder();

			foreach (ITerm term in Items)
			{
				if (buf.Length != 0)
				{
					buf.Append(',');
				}
				buf.Append(StringHelper.EnsureQuotes(term.ToCListStringList()));
			}

			return buf.ToString().TrimEnd();
		}

		public override string ToStringTryConvertToInt()
		{
			StringBuilder buf = new StringBuilder();

			foreach (ITerm term in Items)
			{
				if (buf.Length != 0)
				{
					buf.Append(", ");
				}
				string t = StringHelper.StripQuotes(term.ToCListStringList());
				if (StringHelper.IsInt(t))
				{
					buf.Append(t);
				}
				else
				{
					buf.Append(StringHelper.EnsureQuotes(t));
				}
			}

			return buf.ToString().TrimEnd();
		}

		public override string ToString()
		{
			return ToCListStringList();
		}

		public override IExpr Clone()
		{
			ValueList vl = new ValueList();
			vl.Items.AddRange(Items);
			return vl;
		}
	}
}
