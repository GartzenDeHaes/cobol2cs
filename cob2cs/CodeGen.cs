using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using DOR.Core.Collections;
using CobolParser;
using CobolParser.Text;
using CobolParser.Sections;

namespace DOR
{
	public static class CodeGen
	{
		public static void ToChash(string filename, bool wpf)
		{
			if (!filename.StartsWith(ImportManager.BaseDirectory))
			{
				filename = Path.Combine(ImportManager.BaseDirectory, filename);
			}

			GuardianPath gpath = ImportManager.GuardianPathFromWindows(filename);
			Console.WriteLine("Converting {0}", gpath.ToString());

			CsFormater fmtr = new CsFormater(gpath, wpf);
		}
	}
}
