using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using DOR.Core.Collections;

namespace DOR.Core
{
	/// <summary>
	/// StringHelper has a number of string utility functions.
	/// </summary>
	public static class StringHelper
	{
		public static long GetLongHashCode(string s)
		{
			long h = 0;

			for (int x = 0; x < s.Length; x++)
			{
				h = (h << 6) ^ (h >> 58) ^ s[x];
			}

			if (h < 0)
			{
				return -h;
			}

			return h;
		}

		public static long GetLongHashCode(Vector<byte> s)
		{
			long h = 0;

			for (int x = 0; x < s.Count; x++)
			{
				h = (h << 6) ^ (h >> 58) ^ s[x];
			}

			if (h < 0)
			{
				return -h;
			}
			return h;
		}

		public static long GetLongHashCode(StringBuilder s)
		{
			long h = 0;

			for (int x = 0; x < s.Length; x++)
			{
				h = (h << 6) ^ (h >> 58) ^ s[x];
			}

			if (h < 0)
			{
				return -h;
			}
			return h;
		}

		public static int GetHashCode(StringBuilder s)
		{
			long h = GetLongHashCode(s);

			return (int)((h & 0xFFFFFFFF) ^ (h >> 32));
		}

		public static int GetHashCode(string s)
		{
			long h = GetLongHashCode(s);

			return (int)((h & 0xFFFFFFFF) ^ (h >> 32));
		}

		public static string Reverse(string s)
		{
			StringBuilder buf = new StringBuilder(s.Length);
			buf.Length = s.Length;

			int center = s.Length / 2;
			for (int x = 0; x < center; x++)
			{
				buf[x] = s[(s.Length - 1) - x];
				buf[(s.Length - 1) - x] = s[x];
			}

			if ((s.Length % 2) != 0)
			{
				buf[center] = s[center];
			}

			return buf.ToString();
		}

		public static string EnsureQuotes(string s)
		{
			if (s.Length > 1 && s[0] == '"' && s[s.Length - 1] == '"')
			{
				return s;
			}

			return "\"" + s + "\"";
		}

		/// <summary>
		/// Returns true if the argument is a C or HTML format hex number.
		/// </summary>
		public static bool IsHexNum(string value)
		{
			if (1 >= value.Length)
			{
				return Char.IsNumber(value[0]);
			}
			int pos = 0;
			if (value[0] == '#')
			{
				pos = 1;
			}
			else if (value[0] == '0' && (value[1] == 'x' || value[1] == 'X'))
			{
				pos = 2;
			}
			for (; pos < value.Length; pos++)
			{
				if (Char.IsNumber(value[pos]))
				{
					continue;
				}
				if ((value[pos] >= 'a' && value[pos] <= 'f') || (value[pos] >= 'A' && value[pos] <= 'F'))
				{
					continue;
				}
				return false;
			}
			return true;
		}

		/// <summary>
		/// Returns the number of time ch occures in str.
		/// </summary>
		public static int CountOccurancesOf(string value, char ch)
		{
			int count = 0;
			for (int x = 0; x < value.Length; x++)
			{
				if (value[x] == ch)
				{
					count++;
				}
			}
			return count;
		}

		/// <summary>
		/// Returns the number of time ch occures in str.
		/// </summary>
		public static int CountOccurancesOf(StringBuilder value, char ch)
		{
			int count = 0;
			for (int x = 0; x < value.Length; x++)
			{
				if (value[x] == ch)
				{
					count++;
				}
			}
			return count;
		}

		/// <summary>
		/// Returns the number of time ch occures in str.
		/// </summary>
		public static int CountOccurancesOf(string value, string key)
		{
			int pos = 0;
			int count = 0;

			while (pos >= 0)
			{
				if (-1 < (pos = value.IndexOf(key, pos)))
				{
					count++;
					pos++;
				}
			}

			return count;
		}

		/// <summary>
		/// Returns true if the argument is upper case, ignores non letters.
		/// </summary>
		public static bool IsUpper(string value)
		{
			for (int x = 0; x < value.Length; x++)
			{
				if (Char.IsLetter(value[x]) && !Char.IsUpper(value[x]))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Returns true if the argument is an integer or non-exponential floting point.
		/// </summary>
		public static bool IsNumeric(string value)
		{
			if (value.Length == 0 || "." == value || "-" == value || "+" == value)
			{
				return false;
			}

			int dotcount = 0;
			int start = value[0] == '+' || value[0] == '-' ? 1 : 0;

			for (int x = start; x < value.Length; x++)
			{
				char ch = value[x];
				if (ch == '.' && dotcount == 0)
				{
					dotcount++;
					continue;
				}
				if (!Char.IsDigit(ch))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Returns true if the argument consists entirely of numerics.
		/// </summary>
		public static bool IsInt(string value)
		{
			if (value.Length == 0)
			{
				return false;
			}

			int start = value[0] == '-' || value[0] == '+' ? 1 : 0;
			
			for (int x = start; x < value.Length; x++)
			{
				if (!Char.IsDigit(value[x]))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Removes balanced quotes, if any, from the argument.
		/// </summary>
		/// <returns>Returns the argument, minus begining and ending quotes (if any).</returns>
		public static string StripQuotes(string value)
		{
			if (value[0] == '"' && value[value.Length - 1] == '"')
			{
				return value.Substring(1, value.Length - 2);
			}
			return value;
		}

		/// <summary>
		/// Returns true if the argument has HTML path characters.
		/// </summary>
		public static bool HasPath(string filename)
		{
			return filename.IndexOf('/') > -1 || filename.IndexOf("..") > -1;
		}

		/// <summary>
		/// Returns filepath, ensuring that ch is the last character.  This typically
		/// used when pulling file paths out of config files where the trailing '/'
		/// may be ommitted by the user.
		/// </summary>
		public static string EnsureTrailingChar(string filepath, char ch)
		{
			if (filepath[filepath.Length - 1] != ch)
			{
				return filepath + ch.ToString();
			}
			return filepath;
		}

		/// <summary>
		/// This implements javascript's parseInt function.  parseInt will return 123 for strings of the form '123abc'.
		/// </summary>
		public static int ParseRightInt32(string value)
		{
			StringBuilder buf = new StringBuilder(value.Length);
			for (int x = value.Length - 1; x >= 0; x--)
			{
				if (!Char.IsDigit(value[x]))
				{
					break;
				}
#if SILVERLIGHT
				buf.Insert(0, value[x].ToString());
#else
				buf.Insert(0, value[x]);
#endif
			}
			if (buf.Length == 0)
			{
				return 0;
			}
			return Int32.Parse(buf.ToString());
		}

		/// <summary>
		/// Implements VB RIGHT$
		/// </summary>
		/// <param name="value">The string to split</param>
		/// <param name="count">The number of characters to return starting from the end of the string</param>
		public static string RightStr(string value, int count)
		{
			return value.Substring(value.Length - count);
		}

		/// <summary>
		/// Implements VB MID$
		/// </summary>
		/// <param name="value">The string to split</param>
		/// <param name="start">The index to start from</param>
		/// <param name="stop">The index to end at</param>
		public static string MidStr(string value, int start, int stop)
		{
			return value.Substring(start, stop - start);
		}

		public static string SubstringPadLeft(string s, int start, int len, char pad)
		{
			if (start + len > s.Length)
			{
				return PadLeft(s.Substring(start), len, pad);
			}
			else
			{
				return s.Substring(start, len);
			}
		}

		public static string SubstringPadRight(string s, int start, int len, char pad)
		{
			if (start + len > s.Length)
			{
				return PadRight(s.Substring(start), len, pad);
			}
			else
			{
				return s.Substring(start, len);
			}
		}

		public static string[] Split(string src, string delim)
		{
			Vector<string> ret = new Vector<string>();
			int oldpos = 0;
			int pos = src.IndexOf(delim);

			while (pos > -1)
			{
				ret.Add(StringHelper.MidStr(src, oldpos, pos));
				oldpos = pos + delim.Length;
				pos = src.IndexOf(delim, oldpos);
			}

			ret.Add(src.Substring(oldpos));

			return ret.ToArray();
		}

		/// <summary>
		/// Parses numerics of the form '$123,123.00'
		/// </summary>
		public static decimal ParseMoney(string value)
		{
			return (value.Length == 0) ? 0 : Decimal.Parse(value.Replace("$", "").Replace(",", ""));
		}

		/// <summary>
		/// Convert to string handling DBNull and null.
		/// </summary>
		public static string Parse(object o)
		{
			if (o is DBNull)
			{
				return null;
			}
            if (null == o)
            {
                return null;
            }
			if (o is string)
			{
				return (string)o;
			}
			return o.ToString();
		}

		/// <summary>
		/// Returns the argument with all numeric characters removed.
		/// </summary>
		public static string RemoveNonNumerics(string value)
		{
			Regex nonNumerics = new Regex("[^0-9]");
			return nonNumerics.Replace(value, "");
		}

		private static char[] m_xmlSpChars = new char[] { '&', '<', '>', '"', '\'' };

		/// <summary>
		/// Returns true if the argument requires xml encoding.
		/// </summary>
		public static bool RequiresXmlEncoding(string value)
		{
			return value.IndexOfAny(m_xmlSpChars) > -1;
		}

		/// <summary>
		/// A simplistic XML encoder.  If available, use the encoders from System.Web instead.
		/// </summary>
		/// <seealso cref="System.Web.HttpUtility"/>
		public static string XmlEncode(string value)
		{
			if (!RequiresXmlEncoding(value))
			{
				return value;
			}
			StringBuilder buf = new StringBuilder(value.Length + 30);
			for (int x = 0; x < value.Length; x++)
			{
				switch (value[x])
				{
					case '&':
						buf.Append("&amp;");
						break;
					case '<':
						buf.Append("&lt;");
						break;
					case '>':
						buf.Append("&gt;");
						break;
					case '"':
						buf.Append("&quot;");
						break;
					case '\'':
						buf.Append("&apos;");
						break;
					case '+':
						buf.Append("%2B");
						break;
					case '%':
						buf.Append("%25");
						break;
					case '\r':
						buf.Append("%10");
						break;
					case '\n':
						buf.Append("%13");
						break;
					default:
						buf.Append(value[x]);
						break;
				}
			}
			return buf.ToString();
		}

		/// <summary>
		/// Superset of XmlEncode that converts control and white space to %XX format.
		/// </summary>
		/// <param name="value">A string</param>
		/// <returns>The encoded string.</returns>
		/// <seealso cref="System.Web.HttpUtility"/>
		public static string UriEncode(string value)
		{
			value = XmlEncode(value);

			StringBuilder buf = new StringBuilder(value.Length + 30);
			for (int x = 0; x < value.Length; x++)
			{
				if (Char.IsWhiteSpace(value[x]) || Char.IsControl(value[x]))
				{
					buf.Append('%');
					buf.Append(((int)value[x]).ToString("00"));
				}
				else
				{
					buf.Append(value[x]);
				}
			}
			return buf.ToString();
		}

		public static string HttpPostFormatString(string s)
		{
			StringBuilder buf = new StringBuilder(s.Length + 10);

			for (int x = 0; x < s.Length; x++)
			{
				switch (s[x])
				{
					case '\n':
						buf.Append("%13");
						break;
					case '\r':
						buf.Append("%10");
						break;
					case '+':
						buf.Append("%2B");
						break;
					case '%':
						buf.Append("%25");
						break;
					default:
						buf.Append(s[x]);
						break;
				}
			}
			return buf.ToString();
		}

		/// <summary>
		/// Convert the string to a "c" style escape format.  This is necessary for quoted HTTP body post data.
		/// </summary>
		public static string EscapeString(string s)
		{
			StringBuilder buf = new StringBuilder(s.Length + 10);

			for (int x = 0; x < s.Length; x++)
			{
				switch (s[x])
				{
					case '\b':
						buf.Append("\\b");
						break;
					case '\t':
						buf.Append("\\t");
						break;
					case '\v':
						buf.Append("\\v");
						break;
					case '\f':
						buf.Append("\\f");
						break;
					case '\n':
						buf.Append("\\n");
						break;
					case '\r':
						buf.Append("\\r");
						break;
					case '"':
						buf.Append("\\\"");
						break;
					case '\'':
					    buf.Append("\\'");
					    break;
					case '\\':
						buf.Append("\\\\");
						break;
					case '+':
						buf.Append("%2B");
						break;
					case '%':
						buf.Append("%25");
						break;
					default:
						buf.Append(s[x]);
						break;
				}
			}
			return buf.ToString();
		}

		/// <summary>
		/// Remove all control chars from the string
		/// </summary>
		public static string StripControlChars(string s)
		{
			StringBuilder buf = new StringBuilder(s.Length);

			for (int x = 0; x < s.Length; x++)
			{
				if (!Char.IsControl(s[x]))
				{
					buf.Append(s[x]);
				}
			}
			return buf.ToString();
		}

		public static string StripNulls(string s)
		{
			if (s.IndexOf('\0') < 0)
			{
				return s;
			}

			return s.Replace("\0", "");
		}

		public static bool AreEqual(StringBuilder sb, string str)
		{
			if (sb.Length != str.Length)
			{
				return false;
			}
			for (int x = 0; x < str.Length; x++)
			{
				if (str[x] != sb[x])
				{
					return false;
				}
			}
			return true;
		}

		public static string ToStringOrNbsp(object o)
		{
			if (null == o || (o is DBNull))
			{
				return "&nbsp;";
			}
			return o.ToString();
		}

		/// <summary>
		/// Trim that ignores null;
		/// </summary>
		public static string Trim(string str)
		{
			if (null == str)
			{
				return null;
			}
			return str.Trim();
		}

		public static string TrimLeading(string str, char ch)
		{
			int pos = 0;
			while (str[pos] == ch)
			{
				pos++;
			}
			if (pos == 0)
			{
				return str;
			}
			return str.Substring(pos);
		}

		public static string RepeatChar(char ch, int times)
		{
			if (times <= 0)
			{
				return String.Empty;
			}
			StringBuilder buf = new StringBuilder(times);
			for (int x = 0; x < times; x++)
			{
				buf.Append(ch);
			}

			return buf.ToString();
		}

		public static string PadLeft(string s, int len, char ch)
		{
			if (len == 0)
			{
				return s;
			}
			return RepeatChar(ch, len - s.Length) + s;
		}

		public static string PadRight(string s, int len, char ch)
		{
			if (len == 0)
			{
				return s;
			}
			return s + RepeatChar(ch, len - s.Length);
		}
	}
}
