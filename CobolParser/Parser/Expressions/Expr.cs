using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DOR.Core;
using DOR.Core.Collections;
using CobolParser.Expressions.Terms;

namespace CobolParser.Expressions
{
	public class Expr : IExpr
	{
		public Vector<ITerm> Terms
		{
			get;
			private set;
		}

		public override bool IsStringLit 
		{
			get
			{
				return Terms.Count == 1 && Terms[0] is StringLit;
			}
		}

		public override bool IsNumber
		{
			get
			{
				return Terms.Count == 1 && Terms[0] is Number;
			}
		}

		public override int Count 
		{
			get { return Terms.Count; }
		}

		protected Expr()
		{
			Terms = new Vector<ITerm>();
		}

		public Expr(ITerm term)
		: this()
		{
			Terms.Add(term);
		}

		public Expr(Terminalize terms)
		: this()
		{
			if (terms.CurrentEquals("NOT"))
			{
				// IF NOT NAUPA-OWN-STATE-ABBRV-NAME = "WA"
				Terms.Add(new Operator(terms));
			}

			ITerm t;
			while (null != (t = ITerm.Parse(terms)))
			{
				Terms.Add(t);

				if (terms.Current.Type == StringNodeType.Comma)
				{
					// commas should have been filtered in Terminalize
					terms.Next();
				}

				if (terms.CurrentEquals("NUMERIC"))
				{
					Terms.Add(new Numeric(terms));
				}
				if (terms.CurrentEquals("POSITIVE"))
				{
					Terms.Add(new Positive(terms));
				}
				if (terms.CurrentEquals("NEGATIVE"))
				{
					Terms.Add(new Negative(terms));
				}
				if (terms.CurrentEquals("ALPHABETIC"))
				{
					Terms.Add(new Alphabetic(terms));
				}
				if (Operator.IsOperator(terms.Current))
				{
					if (terms.CurrentEquals("NOT") && (terms.CurrentNextEquals(1, "INVALID") || terms.CurrentNextEquals(1, "END")))
					{
						break;
					}
					while (Operator.IsOperator(terms.Current))
					{
						Terms.Add(new Operator(terms));
					}
				}
				else
				{
					break;
				}
			}
		}

		public override string ToDocumentationString()
		{
			StringBuilder buf = new StringBuilder();

			foreach (ITerm term in Terms)
			{
				if (buf.Length != 0)
				{
					buf.Append(' ');
				}
				buf.Append(term.ToDocumentationString());
			}

			return buf.ToString();
		}

		public override string ToCListStringList()
		{
			StringBuilder buf = new StringBuilder();

			foreach (ITerm term in Terms)
			{
				if (buf.Length != 0)
				{
					buf.Append(',');
				}
				buf.Append(StringHelper.EnsureQuotes(term.ToCListStringList()));
			}

			return buf.ToString();
		}

		public override string ToStringTryConvertToInt()
		{
			StringBuilder buf = new StringBuilder();

			foreach (ITerm term in Terms)
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

		public override IExpr Clone()
		{
			Expr e = new Expr();
			e.Terms.AddRange(Terms);
			return e;
		}

		public override string ToString()
		{
			StringBuilder buf = new StringBuilder();

			foreach (ITerm term in Terms)
			{
				if (buf.Length != 0)
				{
					buf.Append(',');
				}
				buf.Append(term.ToString());
			}

			return buf.ToString();
		}
	}
}
