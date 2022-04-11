using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DOR.Core;

namespace CobolParser
{
	public class GuardianPath
	{
		public string FullPath
		{
			get;
			private set;
		}

		public string Drive
		{
			get;
			private set;
		}

		public string Volume
		{
			get;
			private set;
		}

		public string File
		{
			get;
			private set;
		}

		public GuardianPath(string drive, string vol, string file)
		{
			Drive = drive.ToUpper();
			Volume = vol.ToUpper();
			File = file.ToUpper();
			FullPath = Drive + "." + Volume + "." + File;
		}

		public GuardianPath(string path)
		{
			path = StringHelper.StripQuotes(path);
			Debug.Assert(IsGuardianPath(path));

			FullPath = path.ToUpper();

			string[] parts = FullPath.Split(new char[] { '.' });

			if (parts.Length != 3)
			{
				throw new Exception("Invalid GUARDIAN path of " + path);
			}

			Drive = parts[0];
			Debug.Assert(Drive[0] == '$');

			Volume = parts[1];
			File = parts[2];
		}

		public string WindowsFileName()
		{
			return System.IO.Path.Combine(System.IO.Path.Combine(ImportManager.BaseDirectory, Drive.Substring(1) + "." + Volume), File);
		}

		public override int GetHashCode()
		{
			return FullPath.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			GuardianPath path = obj as GuardianPath;
			if (null == path)
			{
				return false;
			}

			return FullPath == path.FullPath;
		}

		public override string ToString()
		{
			return FullPath;
		}

		public string ToProbablyProgramName()
		{
			StringBuilder buf = new StringBuilder();

			if (File[0] == 's')
			{
				buf.Append("Srv");
			}
			else if (File[0] == 'r')
			{
				buf.Append("Req");
			}
			else if (File[0] == 'u')
			{
				buf.Append("Sub");
			}
			else if (File[0] == 'b')
			{
				buf.Append("B");
			}
			else if 
			(
				File.StartsWith("CLIB") || 
				File == "COBLIB" || 
				File == "COPYLIB" ||
				File == "COPYLIBT" || 
				File == "ENV" ||
				File == "EMAIL" ||
				File == "EMAILSQL" ||
				File == "SQLCA"
			)
			{
				return Char.ToUpper(File[0]).ToString() + File.Substring(1).ToLower();
			}
			else
			{
				throw new Exception("ToProbablyProgramName error");
			}

			string sys = File.Substring(4, 2);
			buf.Append(Char.ToUpper(sys[0]));
			buf.Append(Char.ToLower(sys[1]));

			buf.Append(Char.ToUpper(File[File.Length-1]));

			string num = File.Substring(1, 3);
			Debug.Assert(StringHelper.IsInt(num));

			buf.Append(Char.ToUpper(num[0]));
			buf.Append(Char.ToLower(num[1]));
			buf.Append(Char.ToLower(num[2]));

			return buf.ToString();
		}

		public static bool IsGuardianPath(string path)
		{
			return path.Length > 1 && 
				StringHelper.StripQuotes(path)[0] == '$' && 
				StringHelper.CountOccurancesOf(path, '.') == 2;
		}
	}
}
