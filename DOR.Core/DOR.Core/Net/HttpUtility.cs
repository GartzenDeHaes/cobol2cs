using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core.Net
{
	public static class HttpUtility
	{
		private static bool NotEncoded (char c)
		{
			return (c == '!' || c == '\'' || c == '(' || c == ')' || c == '*' || c == '-' || c == '.' || c == '_');
		}

		public static bool HtmlAttributeEncodeRequired (string cp)
		{
			for (int i = 0; i < cp.Length; i++) 
			{
				if ( cp[i] == '&' || cp[i] == '"' || cp[i] == '<') 
				{
					return true;
				}
			}
			return false;
		}

		public static string HtmlAttributeEncode (string cp)
		{
			int i;

			if (null == cp) 
			{
				return "";
			}
			
			if (! HtmlAttributeEncodeRequired(cp))
			{
				return cp;
			}

			StringBuilder output = new StringBuilder();

			for (i = 0; i < cp.Length; i++)
			{
				switch ( cp[i] ) 
				{
				case '&' : 
					output.Append ("&amp;");
					break;
				case '"' :
					output.Append ("&quot;");
					break;
				case '<':
					output.Append ("&lt;");
					break;
				default:
					output.Append ( cp[i] );
					break;
				}
			}

			return output.ToString();
		}

		private static int _GetInt (char c)
		{
			if (c >= '0' && c <= '9')
				return c - '0';

			if (c >= 'a' && c <= 'f')
				return c - 'a' + 10;

			if (c >= 'A' && c <= 'F')
				return c - 'A' + 10;

			return -1;
		}

		private static int _GetChar (string bytes, int offset, int length)
		{
			int value = 0;
			int end = length + offset;
			for (int i = offset; i < end; i++) 
			{
				int current = _GetInt (bytes[i]);
				if (current == -1)
				{
					return -1;
				}
				value = (value << 4) + current;
			}

			return value;
		}

		public static string UrlDecode (string cp)
		{
			if (null == cp) 
			{
				return "";
			}

			if ( cp.IndexOf('%') < 0 && cp.IndexOf('+') < 0 )
			{
				return cp;
			}

			StringBuilder output = new StringBuilder();
			StringBuilder bytes = new StringBuilder();
			int xchar;

			for (int i = 0; i < cp.Length; i++) 
			{
				if (cp[i] == '%' && i + 2 < cp.Length && cp[i + 1] != '%') 
				{
					if (cp[i + 1] == 'u' && i + 5 < cp.Length) 
					{
						if (bytes.Length > 0) 
						{
							output.Append (bytes);
							bytes.Length = 0;
						}

						xchar = _GetChar (cp, i + 2, 4);
						if (xchar != -1) 
						{
							output.Append ((char) xchar);
							i += 5;
						} 
						else 
						{
							output.Append ('%');
						}
					} 
					else if ((xchar = _GetChar(cp, i + 1, 2)) != -1) 
					{
						bytes.Append ( (char)xchar );
						i += 2;
					} 
					else 
					{
						output.Append ('%');
					}
					continue;
				}

				if (bytes.Length > 0) 
				{
					output.Append (bytes);
					bytes.Length = 0;
				}

				if (cp[i] == '+') 
				{
					output.Append (' ');
				} 
				else 
				{
					output.Append (cp[i]);
				}
			}

			if (bytes.Length > 0) 
			{
				output.Append (bytes);
			}

			return output.ToString();
		}

		private static char[] _urlHexChars = {'0','1','2','3','4','5','6','7','8','9','a','b','c','d','e','f'};

		public static bool UrlEncodeRequired (string cp)
		{
			for (int i = 0; i < cp.Length; i++) 
			{
				char c = cp[i];
				if ((c < '0') || (c < 'A' && c > '9') || (c > 'Z' && c < 'a') || (c > 'z')) 
				{
					if (NotEncoded (c))
					{
						continue;
					}
					return true;
				}
			}
			return false;
		}

		public static string UrlEncode (string cp)
		{
			if (cp == null)
			{
				return "";
			}

			if (cp[0] == '\0' || cp.Length == 0)
			{
				return "";
			}

			int i;
			bool isUnicode = false;

			if (! UrlEncodeRequired(cp))
			{
				return cp;
			}

			StringBuilder result = new StringBuilder();

			for (i = 0; i < cp.Length; i++)
			{
				char c = cp[i];

				if (c > 255) 
				{
					int idx;
					int i2 = (int) c;

					result.Append('%');
					result.Append ('u');
					idx = i2 >> 12;
					result.Append (_urlHexChars [idx]);
					idx = (i2 >> 8) & 0x0F;
					result.Append (_urlHexChars [idx]);
					idx = (i2 >> 4) & 0x0F;
					result.Append (_urlHexChars [idx]);
					idx = i2 & 0x0F;
					result.Append (_urlHexChars [idx]);
					continue;
				}
		
				if (c > ' ' && NotEncoded (c)) 
				{
					result.Append(c);
					continue;
				}
				if (c == ' ') 
				{
					result.Append('+');
					continue;
				}
				if ( (c < '0') ||
					(c < 'A' && c > '9') ||
					(c > 'Z' && c < 'a') ||
					(c > 'z')) 
				{
					if (isUnicode && c > 127) 
					{
						result.Append ('%');
						result.Append ('u');
						result.Append ('0');
						result.Append ('0');
					}
					else
					{
						result.Append('%');
					}
					int idx = ((int) c) >> 4;
					result.Append (_urlHexChars [idx]);
					idx = ((int) c) & 0x0F;
					result.Append (_urlHexChars [idx]);
				}
				else
				{
					result.Append (c);
				}
			}

			return result.ToString();
		}

		public static string HtmlDecode (string cp)
		{
			if (cp == null)
			{
				throw new InvalidArgumentException("HtmlDecode: argument was null");
			}

			if (cp.IndexOf('&') < 0)
			{
				return cp;
			}

			StringBuilder entity = new StringBuilder();
			StringBuilder output = new StringBuilder();

			// 0 -> nothing,
			// 1 -> right after '&'
			// 2 -> between '&' and ';' but no '#'
			// 3 -> '#' found after '&' and getting numbers
			int state = 0;
			int number = 0;
			bool have_trailing_digits = false;

			for (int i = 0; i < cp.Length; i++) 
			{
				char c = cp[i];
				if (state == 0) 
				{
					if (c == '&') 
					{
						entity.Append (c);
						state = 1;
					} 
					else 
					{
						output.Append (c);
					}
					continue;
				}

				if (c == '&') 
				{
					state = 1;
					if (have_trailing_digits) 
					{
						entity.Append(number.ToString());
						have_trailing_digits = false;
					}

					output.Append(entity);
					entity.Length = 0;
					entity.Append ('&');
					continue;
				}

				if (state == 1) 
				{
					if (c == ';') 
					{
						state = 0;
						output.Append(entity);
						output.Append(c);
						entity.Length = 0;
					} 
					else 
					{
						number = 0;
						if (c != '#') 
						{
							state = 2;
						} 
						else 
						{
							state = 3;
						}
						entity.Append (c);
					}
				} 
				else if (state == 2) 
				{
					entity.Append (c);
					if (c == ';') 
					{
						// This only checks the most common entities, since I don't want a static hashtable in the library.
						if (entity.Equals("&nbsp;"))
						{
							output.Append(' ');
						}
						else if (entity.Equals("&amp;"))
						{
							output.Append('&');
						}
						else if (entity.Equals("&lt;"))
						{
							output.Append('<');
						}
						else if (entity.Equals("&gt;"))
						{
							output.Append('>');
						}
						else if (entity.Equals("&quot;"))
						{
							output.Append('"');
						}
						else
						{
							output.Append(entity);
						}
						state = 0;
						entity.Length = 0;
					}
				} 
				else if (state == 3) 
				{
					if (c == ';') 
					{
						if (number > 65535) 
						{
							output.Append("&#");
							output.Append(number.ToString());
							output.Append(";");
						} 
						else 
						{
							output.Append ((char) number);
						}
						state = 0;
						entity.Length = 0;
						have_trailing_digits = false;
					} 
					else if (Char.IsDigit(c)) 
					{
						number = number * 10 + ((int) c - '0');
						have_trailing_digits = true;
					} 
					else 
					{
						state = 2;
						if (have_trailing_digits) 
						{
							entity.Append (number.ToString());
							have_trailing_digits = false;
						}
						entity.Append(c);
					}
				}
			}

			if (entity.Length > 0) 
			{
				output.Append(entity);
			} 
			else if (have_trailing_digits) 
			{
				output.Append (number.ToString());
			}
			return output.ToString ();
		}

		public static bool HtmlEncodeRequired (string cp)
		{
			for (int i = 0; i < cp.Length; i++) 
			{
				char c = cp[i];
				if (c == '&' || c == '"' || c == '<' || c == '>' || c > 159) 
				{
					return true;
				}
			}
			return false;
		}

		public static string HtmlEncode (string cp) 
		{
			if (cp == null)
			{
				return "";
			}

			int i;

			if (!HtmlEncodeRequired(cp))
			{
				return cp;
			}

			StringBuilder output = new StringBuilder();

			for (i = 0; i < cp.Length; i++)
			{
				switch (cp[i]) 
				{
				case '&':
					output.Append ("&amp;");
					break;
				case '>' : 
					output.Append ("&gt;");
					break;
				case '<':
					output.Append ("&lt;");
					break;
				case '"':
					output.Append ("&quot;");
					break;
				default:
					// MS starts encoding with &# from 160 and stops at 255.
					// We don't do that. One reason is the 65308/65310 unicode
					// characters that look like '<' and '>'.
					if (cp[i] > 159) 
					{
						output.Append ("&#");
						output.Append (cp[i].ToString());
						output.Append (";");
					} 
					else 
					{
						output.Append (cp[i]);
					}
					break;
				}
			}
			return output.ToString();
		}

		public static byte[] ToByteArray(string s)
		{
			byte[] bytes = new byte[s.Length];
			for (int x = 0; x < s.Length; x++)
			{
				bytes[x] = (byte)s[x];
			}

			return bytes;
		}
	}
}
