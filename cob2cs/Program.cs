using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using DOR.Core;
using DOR.Core.Config;
using CobolParser;
using DOR.Core.Collections;
using CobolParser.Records;
using System.Data.Objects;
using CobolParser.Verbs;
using DOR.Core.Data.Tandem;
using CobolParser.TandemData;
using CobolParser.Text;
using CobolParser.Parser;

namespace DOR
{
	class Program
	{
		static void Main(string[] args)
		{
			ProgramArguments pa = new ProgramArguments(args);

			if (pa.HasAnyOfSwitches("?|help", '|'))
			{
				Console.WriteLine("cob2cs [--root=c:\\temp\\][--wpf][--nativetypes][--sqlbatch] filename");
				return;
			}

			if (pa.HasSwitch("nativetypes"))
			{
				Offset.AttemptNativeTypeConversion = true;
			}

			if (pa.HasSwitch("root"))
			{
				ImportManager.BaseDirectory = pa["root"];
			}
			else
			{
				ImportManager.BaseDirectory = "c:\\temp\\COBOL\\";
			}

			if (pa.HasSwitch("sqlbatch"))
			{
				CsSql.BatchTransactions = true;
			}

			string[] sourceFiles;

			if (pa.Parameters.Length == 1)
			{
				sourceFiles = new string[1];
				sourceFiles[0] = Path.Combine(Path.Combine(ImportManager.BaseDirectory, "d08.source"), pa.Parameters[0]);
			}
			else
			{
				sourceFiles = Directory.GetFiles
				(
					Path.Combine(ImportManager.BaseDirectory, "d08.source")
				);
			}

			string[] ddlFiles = Directory.GetFiles
			(
				Path.Combine(ImportManager.BaseDirectory, "d23.revprod")
			);

			foreach (string dfile in ddlFiles)
			{
				string f = Path.GetFileName(dfile).ToUpper();
				ImportManager.AddDefine(new GuardianDefine("=" + f), new GuardianPath("$d23", "revprod", f));
			}

			ImportManager.AddDorNonDatabaseDefines();

			//try
			//{
				// --wpf --2cs c:\temp\d08.source\S130AL1q
				if (pa.Parameters.Length == 0)
				{
					Console.WriteLine("A filename is required");
					return;
				}
				for (int x = 0; x < pa.Parameters.Count(); x++)
				{
					CodeGen.ToChash(pa.Parameters[x], pa.HasSwitch("wpf"));
				}
			//}
			//catch (Exception ex)
			//{
			//	Console.WriteLine(ex.ToString());
			//}

			Console.WriteLine();
			Console.WriteLine("Press return to continue ...");
			Console.ReadKey();
		}
	}
}
