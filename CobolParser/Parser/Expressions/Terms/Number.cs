using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DOR.Core;

namespace CobolParser.Expressions.Terms
{
	public class Number : ITerm
	{
		public string Value
		{
			get;
			private set;
		}

		public Number(int value)
		{
			Value = value.ToString();
		}

		public Number(Terminalize terms)
		{
			Value = terms.Current.Str;
			Debug.Assert(StringHelper.IsNumeric(Value) || Value[0] == '+');
			terms.Next();
		}

		public override string ToDocumentationString()
		{
			return Value;
		}

		public override string ToString()
		{
			return Value;
		}
	}
}
