using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core;
using DOR.Core.Collections;

namespace CobolParser.Expressions.Terms
{
	public class Id : ITerm
	{
		public bool IsAll
		{
			get;
			private set;
		}

		public StringNode Value
		{
			get;
			private set;
		}

		public Vector<IExpr> Offsets
		{
			get;
			private set;
		}

		public Id(Terminalize terms)
		{
			Offsets = new Vector<IExpr>(2);

			if (terms.CurrentEquals("ALL"))
			{
				IsAll = true;
				terms.Next();
			}
			Value = terms.Current;
			terms.Next();

			while (terms.Current.Type == StringNodeType.LPar)
			{
				terms.Match(StringNodeType.LPar);
				Offsets.Add(IExpr.Parse(terms));
				terms.Match(StringNodeType.RPar);
			}

			Symbol = CobolProgram.CurrentSymbolTable.AddReference(this);
		}

		public override string ToDocumentationString()
		{
			StringBuilder buf = new StringBuilder();
			buf.Append(Value.Str);
			if (null != Offsets && Offsets.Count > 0)
			{
				for (int x = 0; x < Offsets.Count; x++)
				{
					buf.Append('(');
					buf.Append(Offsets[x].ToDocumentationString());
					buf.Append(')');
				}
			}

			return buf.ToString();
		}

		public override string ToCListStringList()
		{
			StringBuilder buf = new StringBuilder();
			buf.Append(Value.Str);
			if (null != Offsets && Offsets.Count > 0)
			{
				for (int x = 0; x < Offsets.Count; x++)
				{
					buf.Append('[');
					buf.Append(Offsets[x].ToDocumentationString());
					buf.Append(']');
				}
			}

			return buf.ToString();
		}

		public override string ToString()
		{
			return ToCListStringList();
		}
	}
}
