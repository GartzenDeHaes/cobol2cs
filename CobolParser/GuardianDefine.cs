using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DOR.Core;

namespace CobolParser
{
	public class GuardianDefine
	{
		public string DefineName
		{
			get;
			private set;
		}

		public GuardianDefine(string def)
		{
			def = StringHelper.StripQuotes(def);

			if (def[0] == '=')
			{
				DefineName = def.Substring(1).ToUpper();
			}
			else
			{
				DefineName = def.ToUpper();
			}
		}

		public override int GetHashCode()
		{
			return DefineName.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			GuardianDefine def = obj as GuardianDefine;
			if (def == null)
			{
				return false;
			}
			return DefineName == def.DefineName;
		}

		public override string ToString()
		{
			return "=" + DefineName;
		}

		public static bool IsDefine(string def)
		{
			return def.Length > 2 && StringHelper.StripQuotes(def)[0] == '=';
		}
	}
}
