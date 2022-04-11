using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DOR.Core.Collections;
using CobolParser.Parser;

namespace CobolParser
{
	public abstract class IDivision
	{
		public Vector<Section> Sections
		{
			get;
			private set;
		}

		public DivisionType DivisionType
		{
			get;
			private set;
		}

		protected IDivision(DivisionType div)
		{
			Sections = new Vector<Section>();
			DivisionType = div;
		}

		public abstract void Parse(Terminalize terms);
	}
}
