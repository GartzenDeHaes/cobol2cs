using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core.Collections;

namespace CobolParser.Expressions.Terms
{
	public class OffsetReference : ITerm
	{
		public Vector<Id> OffsetChain
		{
			get;
			private set;
		}

		public OffsetReference(Terminalize terms)
		{
			OffsetChain = new Vector<Id>();

			OffsetChain.Add(new Id(terms));

			while 
			(
				terms.CurrentEquals("OF") ||
				terms.CurrentEquals("IN")
			)
			{
				terms.Next();
				OffsetChain.Add(new Id(terms));
			}

			Symbol = CobolProgram.CurrentSymbolTable.AddReference(this);
		}

		public Id FindOffset()
		{
			for (int x = 0; x < OffsetChain.Count; x++)
			{
				if (OffsetChain[x].Offsets.Count > 0)
				{
					return OffsetChain[x];
				}
			}
			return null;
		}

		public override string ToDocumentationString()
		{
			StringBuilder buf = new StringBuilder();

			foreach (ITerm s in OffsetChain)
			{
				if (buf.Length != 0)
				{
					buf.Append('.');
				}
				buf.Append(s.ToDocumentationString());
			}

			return buf.ToString();
		}
	}
}
