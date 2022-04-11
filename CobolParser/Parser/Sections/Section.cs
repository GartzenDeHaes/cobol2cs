using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core.Collections;

namespace CobolParser
{
	public class Section
	{
		public Vector<IVerb> Verbs
		{
			get;
			private set;
		}

		public StringNode Name
		{
			get;
			private set;
		}

		public Section(StringNode name)
		{
			Verbs = new Vector<IVerb>();
			Name = name;
		}
	}
}
