using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core.Data.Tandem
{
	public class Define
	{
		public string Name
		{
			get;
			private set;
		}

		public string FileName
		{
			get;
			private set;
		}

		public Define(string name, string filename)
		{
			Name = name;
			FileName = filename;
		}
	}
}
