using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core.Collections;

namespace CobolParser
{
	public class CopyReplacement
	{
		public Vector<StringNode> FromTokens
		{
			get;
			set;
		}

		public Vector<StringNode> ToTokens
		{
			get;
			set;
		}

		public CopyReplacement()
		{
			FromTokens = new Vector<StringNode>();
			ToTokens = new Vector<StringNode>();
		}
	}
}
