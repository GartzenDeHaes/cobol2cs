using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DOR.Core.IO
{
	/// <summary>
	/// Field data for a record
	/// </summary>
	public class FixedFieldData
	{
		protected FixedFieldDef m_field;
		protected string m_data;

		/// <summary>
		/// Initialize the field data from the field definition
		/// </summary>
		/// <param name="field"></param>
		public FixedFieldData(FixedFieldDef field)
		{
			m_field = field;
			m_data = field.DefaultData;
		}

		/// <summary>
		/// Initialize the field data from a record
		/// </summary>
		/// <param name="field"></param>
		/// <param name="record"></param>
		public FixedFieldData(FixedFieldDef field, string record)
		{
			m_field = field;
			m_data = field.Parse(record);
		}

		/// <summary>
		/// The field's data
		/// </summary>
		public string Value
		{
			get
			{
				return m_data;
			}
		}

		/// <summary>
		/// Parse the field data as a Date (MMDDYY)
		/// </summary>
		/// <returns>Date</returns>
		public Date ParseDate()
		{
			Debug.Assert(m_field.Length == 6);
			string mm = m_data.Substring(0, 2);
			string dd = m_data.Substring(2, 2);
			string yy = m_data.Substring(4, 2);

			int iyy = Int32.Parse(yy);
			if (iyy > 50)
			{
				iyy += 1900;
			}
			else
			{
				iyy += 2000;
			}
			return new Date(iyy, Int32.Parse(mm), Int32.Parse(dd));
		}

		/// <summary>
		/// Parse the field's data as an integer
		/// </summary>
		/// <returns>int</returns>
		public int ParseInt()
		{
			string i = StripZeros(m_data).Trim();
			if (i.Length == 0)
			{
				i = "0";
			}
			return Int32.Parse(i);
		}

		/// <summary>
		/// Parse the field's data as money (+00000000000000;-00000000000000)
		/// </summary>
		/// <returns>money</returns>
		public decimal ParseMoney()
		{
			char sign = m_data[0];
			if (!(sign == '+' || sign == '-'))
				throw new FormatException("Invalid money format - money fields must be signed: " + m_data);

			decimal amt = Decimal.Parse(m_data.Substring(1)) / 100;
			if (sign == '-')
			{
				return -amt;
			}
			return amt;
		}

		/// <summary>
		/// Parse a largish integer
		/// </summary>
		/// <returns>decimal</returns>
		public decimal ParseDecimal()
		{
			return Decimal.Parse(StripZeros(m_data));
		}

		/// <summary>
		/// Parse a string with the format of "YYYY-MM-DD:HH:NN:SS.000000"
		/// </summary>
		/// <returns>DateTime</returns>
		public DateTime ParseDateTime()
		{
			int yyyy = Int32.Parse(m_data.Substring(0, 4));
			int mm = Int32.Parse(m_data.Substring(5, 2));
			int dd = Int32.Parse(m_data.Substring(8, 2));

			string timepart = m_data.Substring(11);
			string[] tps = timepart.Split(new char[] { ':' });
			return new DateTime(yyyy, mm, dd, Int32.Parse(tps[0]), Int32.Parse(tps[1]), (int)Double.Parse(tps[2]));
		}

		/// <summary>
		/// Return a four digit year
		/// </summary>
		/// <returns>int</returns>
		public int ParseYear()
		{
			if (!(m_field.Length == 2 || m_field.Length == 4))
				throw new FormatException("Field is not of correct length to be a year field: " + m_field.Name);
			if (m_field.Length == 4)
			{
				return ParseInt();
			}
			int yy = Int32.Parse(m_data);
			if (yy > 50)
			{
				return yy + 1900;
			}
			return yy + 2000;
		}

		/// <summary>
		/// Set the field to the default value.
		/// </summary>
		public void SetDefault()
		{
			m_data = m_field.DefaultData;
		}

		/// <summary>
		/// Set a short date format (MMDDYY)
		/// </summary>
		/// <param name="dt">date</param>
		public void SetDate(Date dt)
		{
			if (m_field.Length != 6)
				throw new FormatException("Date containing field must be 6 bytes but this field (" + m_field.Name + ") is " + m_field.Length + " bytes long.");
			m_data = dt.Format("MMDDYY");
		}

		/// <summary>
		/// Set a left justified integer, pad with the default fill
		/// </summary>
		/// <param name="i">int</param>
		public void SetIntLeft(int i)
		{
			m_data = Right((m_field.DefaultData + i.ToString()), m_field.Length);

			Debug.Assert(m_data.Length == m_field.Length);
			if (m_data.Length != m_field.Length)
			{
				throw new Exception(m_field.Name + " incorrect length");
			}
		}

		public void SetSignedIntLeft(int i)
		{
			char sign = '+';
			if (i < 0)
			{
				sign = '-';
				i = -i;
			}
			m_data = sign + Right(m_field.DefaultData + i.ToString(), m_field.Length - 1);
			Debug.Assert(m_data.Length == m_field.Length, "Assumption failure");

			Debug.Assert(m_data.Length == m_field.Length);
			if (m_data.Length != m_field.Length)
			{
				throw new Exception(m_field.Name + " incorrect length");
			}
		}

		/// <summary>
		/// Set the exact string.  The length must equal the field size.
		/// </summary>
		/// <param name="val">the data</param>
		public void Set(string val)
		{
			m_data = val;

			if (m_data.Length != m_field.Length)
			{
				throw new Exception(m_field.Name + " incorrect length");
			}
		}

		public void Set(long val, char leftFill)
		{
			m_data = StringHelper.PadLeft(val.ToString(), m_field.Length, leftFill);

			if (m_data.Length != m_field.Length)
			{
				throw new Exception(m_field.Name + " incorrect length");
			}
		}

		/// <summary>
		/// Set the field as tandem format money (+00000000000000;-00000000000000)
		/// </summary>
		/// <param name="val">money</param>
		public void SetMoney(decimal val)
		{
			char sign = '+';
			if (val < 0)
			{
				sign = '-';
				val = -val;
			}
			m_data = sign + Right((m_field.DefaultData +
								val.ToString("#0.00").Replace(".", "")),
								m_field.Length - 1);
			Debug.Assert(m_data.Length == m_field.Length, "Assumption failure");

			Debug.Assert(m_data.Length == m_field.Length);
			if (m_data.Length != m_field.Length)
			{
				throw new Exception(m_field.Name + " incorrect length");
			}
		}

		/// <summary>
		/// Set left justified, padded with the default fill
		/// </summary>
		/// <param name="val">data</param>
		public void SetLeft(string val)
		{
			m_data = val + m_field.DefaultData;
			m_data = m_data.Substring(0, m_field.Length);
			//m_data = Right((m_field.DefaultData + val), m_field.Length);
			//m_data = m_data.TrimStart().PadRight(m_field.Length);

			Debug.Assert(m_data.Length == m_field.Length);
			if (m_data.Length != m_field.Length)
			{
				throw new Exception(m_field.Name + " incorrect length");
			}
		}

		/// <summary>
		/// Set right justified, padded with the default fill
		/// </summary>
		/// <param name="val">data</param>
		public void SetRight(string val)
		{
			m_data = Right((m_field.DefaultData + val), m_field.Length);

			Debug.Assert(m_data.Length == m_field.Length);
			if (m_data.Length != m_field.Length)
			{
				throw new Exception(m_field.Name + " incorrect length");
			}
		}

		/// <summary>
		/// format as YYYY-MM-DD:HH:NN:SS.000000
		/// </summary>
		/// <param name="dtm">date</param>
		public void SetDateTime(DateTime dtm)
		{
			m_data = dtm.ToString("yyyy-MM-dd:HH:mm:ss.000000");
			if (m_data.Length != m_field.Length)
			{
				throw new FormatException(m_field.Name + " incorrect length");
			}
		}

		/// <summary>
		/// format as YYYY-MM-DD:HH:NN:SS.000000
		/// </summary>
		/// <param name="dt"></param>
		public void SetDateTime(Date dt)
		{
			m_data = string.Format("{0}-{1}-{2}:00:00:00.000000",
				dt.Year,
				dt.Month.ToString("00"),
				dt.Day.ToString("00"));

			if (m_data.Length != m_field.Length)
			{
				throw new FormatException(
					string.Format("{0} incorrect length for datetime", m_field.Name));
			}
		}

		/// <summary>
		/// Left justify largish integer
		/// </summary>
		/// <param name="val"></param>
		public void SetDecimal(decimal val)
		{
			m_data = Right((m_field.DefaultData + val.ToString()), m_field.Length);

			Debug.Assert(m_data.Length == m_field.Length);
			if (m_data.Length != m_field.Length)
			{
				throw new Exception(m_field.Name + " incorrect length");
			}
		}

		/// <summary>
		/// Special handling for the cert id.  Certs are left justified if numeric,
		/// right justified otherwise.
		/// </summary>
		/// <param name="val">cert id</param>
		public void SetSpecial(string val)
		{
			val = val.Trim();
			if (val.Length == 0)
			{
				m_data = m_field.DefaultData;
				return;
			}
			int ival = 0;
			if (IsNumeric(val, ref ival))
			{
				// left justify
				SetIntLeft(ival);
			}
			else
			{
				// right justify
				m_data = (val + m_field.DefaultData).Substring(0, m_field.Length);
			}

			Debug.Assert(m_data.Length == m_field.Length);
			if (m_data.Length != m_field.Length)
			{
				throw new Exception(m_field.Name + " incorrect length");
			}
		}

		/// <summary>
		/// Is the string a numeric value?
		/// </summary>
		/// <param name="str">certid</param>
		/// <param name="i">the value if numeric</param>
		/// <returns></returns>
		internal static bool IsNumeric(string str, ref int i)
		{
			return int.TryParse(str, out i);
		}

		/// <summary>
		/// Just like VB Right$
		/// </summary>
		/// <param name="str">the string</param>
		/// <param name="len">lenght to return</param>
		/// <returns></returns>
		internal static string Right(string str, int len)
		{
			if (str.Length <= len)
			{
				return str;
			}
			return str.Substring(str.Length - len);
		}

		/// <summary>
		/// Return the substring right padded with spaces
		/// </summary>
		/// <param name="str">data</param>
		/// <param name="start">start position</param>
		/// <param name="len">length</param>
		/// <returns></returns>
		internal static string PadSubstring(string str, int start, int len)
		{
			if (len + start > str.Length)
			{
				string part = str.Substring(start, str.Length - start);
				return part + (new string(' ', len - part.Length));
			}
			return str.Substring(start, len);
		}

		/// <summary>
		/// Strip leading zeros from a string
		/// </summary>
		/// <param name="str">the string</param>
		/// <returns>the string without leading zeros</returns>
		internal static string StripZeros(string str)
		{
			string ret = "";
			bool copying = false;

			for (int x = 0; x < str.Length; x++)
			{
				if (!copying && str[x] == '0')
				{
					ret += ' ';
				}
				else
				{
					copying = true;
					ret += str[x];
				}
			}
			return ret;
		}

		/// <summary>
		/// Reset field to default value
		/// </summary>
		public void Reset()
		{
			m_data = m_field.DefaultData;
			Debug.Assert(m_data.Length == m_field.Length);

			if (m_data.Length != m_field.Length)
			{
				throw new Exception(m_field.Name + " incorrect length");
			}
		}
	}
}
