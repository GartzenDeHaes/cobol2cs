using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using DOR.Core.Collections;
using System.Globalization;

namespace DOR.Core
{
	public static class StringExtentions
	{
		public static string[] Split(this String str, string delim)
		{
			Vector<string> parts = new Vector<string>();

			int delimlen = delim.Length;
			int delimpos = str.IndexOf(delim);
			int pos = 0;

			string span = null;

			if (delimpos < 0)
			{
				parts.Add(str);
				return parts.ToArray();
			}
			while (pos < str.Length)
			{
				span = StringHelper.MidStr(str, pos, delimpos);
				pos = delimpos + delimlen;
				parts.Add(span);

				delimpos = str.IndexOf(delim, delimpos + 1);
				if (delimpos < 0)
				{
					break;
				}
			}
			pos = str.Length - pos;
			Debug.Assert(pos >= 0);
			if (pos > 0)
			{
				span = StringHelper.RightStr(str, pos);
				parts.Add(span);
			}

			return parts.ToArray();
		}

		static byte[] table = new byte[256];
		// x8 + x7 + x6 + x4 + x2 + 1
		const byte poly = 0xd5;
		
		public static byte Crc8(this String str)
		{
			byte crc = 0;
			for (int x = 0; x < str.Length; x++)
			{
				byte b = (byte)str[x];
				crc = table[crc ^ b];
			}
			return crc;			
		}

		public static string ToTitleCase(this string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return string.Empty;
			}

#if SILVERLIGHT
			return TitleCase(text);
#else
			var culture = CultureInfo.InvariantCulture;
			TextInfo textInfo = culture.TextInfo;

			return textInfo.ToTitleCase(text.ToLower());
#endif
		}

#if SILVERLIGHT
		public static string TitleCase(string str)
		{
			return Regex.Replace(str, @"\w+", (m) =>
			{
				string tmp = m.Value;
				return char.ToUpper(tmp[0]) + tmp.Substring(1, tmp.Length - 1).ToLower();
			});
		}
#endif

		static StringExtentions()
		{
			for ( int i = 0; i < 256; ++i ) 
			{
				int temp = i;
				for ( int j = 0; j < 8; ++j ) 
				{
					if ( ( temp & 0x80 ) != 0 ) 
					{
						temp = ( temp << 1 ) ^ poly;
					} else 
					{
						temp <<= 1;
					}
				}
				table[i] = (byte)temp;
			}
		}
	}
}
