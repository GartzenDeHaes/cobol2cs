using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DOR.Core.IO
{
	/// <summary>
	/// Definition of a fixed width field
	/// </summary>
	public class FixedFieldDef
	{
		/// <summary>
		/// Define a field with a repeating fill character
		/// </summary>
		/// <param name="name">Field Name (Unique in record)</param>
		/// <param name="len">Length in characters</param>
		/// <param name="recposition">Character position in record</param>
		/// <param name="ordnal">Field number (starting at zero)</param>
		/// <param name="fill">Default fill character</param>
		public FixedFieldDef(string name, int len, int recposition, int ordnal, char fill)
		{
			m_name = name;
			m_len = len;
			m_recpos = recposition;
			m_fieldpos = ordnal;
			m_templateData = new string(fill, m_len);
		}

		/// <summary>
		/// Define a field with default data
		/// </summary>
		/// <param name="name">Field Name (Unique in record)</param>
		/// <param name="len">Length in characters</param>
		/// <param name="recposition">Character position in record</param>
		/// <param name="ordnal">Field number (starting at zero)</param>
		/// <param name="defaultData">Default field data (lenght must equal field length)</param>
		public FixedFieldDef(string name, int len, int recposition, int ordnal, string defaultData)
		{
			if (defaultData.Length != len)
			{
				throw new FormatException(name + " incorrect length");
			}
			m_name = name;
			m_len = len;
			m_recpos = recposition;
			m_fieldpos = ordnal;
			m_templateData = defaultData;
		}

		/// <summary>
		/// Size in chars
		/// </summary>
		public int Length
		{
			get
			{
				return m_len;
			}
		}

		/// <summary>
		/// Record unique field name
		/// </summary>
		public string Name
		{
			get
			{
				return m_name;
			}
		}

		/// <summary>
		/// Default field data
		/// </summary>
		public string DefaultData
		{
			get
			{
				return m_templateData;
			}
		}

		/// <summary>
		/// Zero based field position
		/// </summary>
		public int OrdinalPosition
		{
			get
			{
				return m_fieldpos;
			}
		}

		/// <summary>
		/// Extract the data for this field.
		/// </summary>
		/// <param name="record">A single flat record line</param>
		/// <returns></returns>
		public string Parse(string record)
		{
			Debug.Assert(m_recpos + m_len <= record.Length, m_name + " record too short");
			return record.Substring(m_recpos, m_len);
		}

		protected string m_name;
		protected int m_len;
		protected int m_recpos;
		/// <summary>Field ordinal position</summary>
		protected int m_fieldpos;
		protected string m_templateData;
	}
}
