using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser
{
	public class SyntaxError : Exception
	{
		public GuardianPath FileName
		{
			get;
			private set;
		}

		public int LineNumber
		{
			get;
			private set;
		}

		public SyntaxError(GuardianPath file, int line, string msg)
		: base(msg)
		{
			FileName = file;
			LineNumber = line;
		}
	}
}
