using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Text
{
	public static class CsFieldNameConverter
	{
		private static StringBuilder _buf = new StringBuilder();
		private static char[] _dash = new char[] { '-' };
		private static char[] _underscore = new char[] { '_' };

		public static string Convert(string name, bool special = false)
		{
			if (special)
			{
				if (name.IndexOf("-") < 0)
				{
					return name + "_";
				}
				else
				{
					return name.Replace("-", "_");
				}
			}

			string[] parts = name.Split(_dash);

			_buf.Clear();

			for (int x = 0; x < parts.Length; x++)
			{
				if (x == 0 && Char.IsNumber(parts[x][0]))
				{
					_buf.Append('_');
				}

				if (parts[x].Length > 0)
				{
					_buf.Append(Char.ToUpper(parts[x][0]));
				}
	
				for (int y = 1; y < parts[x].Length; y++)
				{
					_buf.Append(Char.ToLower(parts[x][y]));
				}
			}

			return _buf.ToString();
		}

		public static string ConvertHarder(string name, bool special = false)
		{
			string[] parts = name.Split(_underscore);

			_buf.Clear();

			for (int x = 0; x < parts.Length; x++)
			{
				if (parts[x].Length > 0)
				{
					_buf.Append(Char.ToUpper(parts[x][0]));
				}

				for (int y = 1; y < parts[x].Length; y++)
				{
					_buf.Append(Char.ToLower(parts[x][y]));
				}
			}

			return _buf.ToString();
		}
	}
}
