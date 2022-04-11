using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DOR.Core;

namespace DOR.WorkingStorage
{
	[Serializable]
	public class MemoryBuffer
	{
		private byte[] _bytes;

		public int Length
		{
			get { return _bytes.Length; }
		}

		public MemoryBuffer(int len)
		{
			_bytes = new byte[len];
			ClearTo(' ');
		}

		public byte this[int idx]
		{
			get { return _bytes[idx]; }
			set { _bytes[idx] = value; }
		}

		public void ClearTo(char value)
		{
			ClearTo(value, 0, Length);
		}

		public void ClearTo(char value, int start, int len)
		{
			for (int x = start; x < start + len; x++)
			{
				_bytes[x] = (byte)value;
			}
		}

		private static char[] _dot = new char[] { '.' };

		public void Set(decimal i, int start, int maxlen, bool signed, int decimals)
		{
			if (i < 0 && !signed)
			{
				throw new ArgumentException("Attemp to put negative value into unsigned field " + i.ToString());
			}

			string si = i.ToString();
			int dotPos = si.IndexOf('.');

			if (dotPos < 0)
			{
				Set((long)i, start, maxlen - decimals, signed);
				return;
			}

			string left = si.Substring(0, dotPos);
			string decs = si.Substring(dotPos + 1);
			decs = StringHelper.PadRight(decs, decimals, '0');

			if (maxlen != decimals)
			{
				Set((long)Int64.Parse(left), start, maxlen - decimals, signed);
			}
			else
			{
				Debug.Assert(left == "0");
			}
			
			Set(decs, start + (maxlen - decimals), decimals, true);
		}

		public void Set(string s, int start, int maxlen, bool noClear = false)
		{
			//Debug.Assert(s.Length <= maxlen);

			if (!noClear && s.Length < maxlen)
			{
				ClearTo(' ', start, maxlen);
			}

			for (int x = 0; x < s.Length && x < maxlen; x++)
			{
				_bytes[x + start] = (byte)s[x];
			}
		}

		public void Set(long i, int start, int maxlen, bool signed)
		{
			if (i < 0 && !signed)
			{
				throw new ArgumentException("Attemp to put negative value into unsigned field " + i.ToString());
			}

			string si = i.ToString();

			if (si.Length > maxlen)
			{
				Debug.WriteLine("Truncating " + si);
				si = si.Substring(0, maxlen);
			}

			int bpos = start;
			int spos = 0;
			
			if (signed)
			{
			    _bytes[start] = i < 0 ? (byte)'-' : (byte)'+';
				bpos++;
				if (si[0] == '-' || si[0] == '+')
				{
					spos++;
				}
			}

			for (int x = 0; x < maxlen - si.Length - (signed && si[0] != '-' && si[0] != '+' ? 1 : 0); x++)
			{
				_bytes[bpos++] = (byte)'0';
			}

			while (spos < si.Length)
			{
				_bytes[bpos++] = (byte)si[spos++];
			}
		}

		public void Set(int i, int start, int maxlen, bool signed)
		{
			Set((long)i, start, maxlen, signed);
		}

		public void Set(MemoryBuffer data, int dstart, int dlen, int start, int len)
		{
			Debug.Assert(dlen == len);

			for (int x = 0; x < len && x < dlen; x++)
			{
				_bytes[x + start] = data[x + dstart];
			}
		}

		public void Get(StringBuilder buf, int start, int len)
		{
			buf.Clear();
			for (int x = start; x < start + len; x++)
			{
				buf.Append((char)_bytes[x]);
			}
		}

		public void Get(out long i, int start, int len, bool signed)
		{
			StringBuilder buf = new StringBuilder();
			int pos = start;
			char ch;

			for (int x = pos; x < start + len; x++)
			{
				ch = (char)_bytes[x];
				if (ch == '\0')
				{
					Debug.WriteLine("Use of uninitialized data");
					i = 0;
					return;
				}
				if (ch != ' ')
				{
					buf.Append(ch);
				}
			}

			string s = buf.ToString().TrimStart();
			if (s.Length == 0)
			{
				i = 0;
			}
			else
			{
				try
				{
					i = Int64.Parse(s);
				}
				catch (Exception)
				{
					Debug.WriteLine("HI");
					i = 0;
				}
			}
		}

		public void Get(out int i, int start, int len, bool signed)
		{
			char ch;
			StringBuilder buf = new StringBuilder();

			for (int x = start; x < start + len; x++)
			{
				ch = (char)_bytes[x];
				if (ch == '\0')
				{
					throw new Exception("Internal error in working storage");
				}
				buf.Append(ch);
			}

			string s = buf.ToString().TrimStart();
			if (s.Length == 0)
			{
				i = 0;
			}
			else
			{
				try
				{
					i = Int32.Parse(s);
				}
				catch (Exception)
				{
					Debug.WriteLine("HI");
					i = 0;
				}
			}
		}

		public void Get(out decimal i, int start, int maxlen, bool signed, int decimals)
		{
			decimal sign = 1;
			if (signed && _bytes[start] == (byte)'-')
			{
				sign = -1;
				start++;
				maxlen--;
			}

			string raw = ToString(start, maxlen).Trim();
			if (raw.Length == 0)
			{
				i = 0;
				return;
			}

			if (raw.IndexOf(":") > -1 || raw.IndexOf("-") > 0)
			{
				// timestamp
				throw new Exception("Internal error, can't convert timestamp to decimal");
			}

			StringBuilder buf = new StringBuilder();
			int len = maxlen - decimals;

			for (int x = 0; x < len; x++)
			{
				buf.Append((char)_bytes[x + start]);
			}

			buf.Append('.');

			for (int x = len; x < maxlen; x++)
			{
				buf.Append((char)_bytes[x + start]);				
			}

			i = sign * Decimal.Parse(buf.ToString());
		}

		public override string ToString()
		{
			StringBuilder buf = new StringBuilder();

			for (int x = 0; x < _bytes.Length; x++)
			{
				buf.Append((char)_bytes[x]);
			}

			return buf.ToString();
		}

		public string ToString(int start, int len)
		{
			StringBuilder buf = new StringBuilder();

			for (int x = 0; x < len; x++)
			{
				buf.Append((char)_bytes[x + start]);
			}

			return buf.ToString();
		}

		public bool Equals(MemoryBuffer mb, int start, int len)
		{
			if (start + len >= _bytes.Length)
			{
				return false;
			}

			for (int x = 0; x < len; x++)
			{
				if (_bytes[x + start] != mb._bytes[x + start])
				{
					return false;
				}
			}

			return true;
		}

		public void CopyFrom(MemoryBuffer buf, int sourceStart, int destStart, int len)
		{
			int destlen = len > buf.Length ? buf.Length : len;
			Array.Copy(buf._bytes, sourceStart, _bytes, destStart, destlen);
		}
	}
}
