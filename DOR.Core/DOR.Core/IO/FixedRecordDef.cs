using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DOR.Core.Collections;

namespace DOR.Core.IO
{
	/// <summary>
	/// Definition for a fixed width file record.  (See FlatRecordFactory)
	/// </summary>
	public class FixedRecordDef
	{
		/// <summary>
		/// Define a record
		/// </summary>
		/// <param name="linelen">Length of the record in chars</param>
		public FixedRecordDef(int linelen)
		{
			m_linelen = linelen;
		}

		/// <summary>
		/// Add a field to the record
		/// </summary>
		/// <param name="name">Unique field name</param>
		/// <param name="len">Length in chars</param>
		/// <param name="defData">Default data (length must equal field length</param>
		public void AddField(string name, int len, string defData)
		{
			FixedFieldDef fld = new FixedFieldDef(name, len, m_recordEnd, m_fields.Count, defData);
			AddField(fld);
		}

		/// <summary>
		/// Add a field to the record
		/// </summary>
		/// <param name="name">Unique field name</param>
		/// <param name="len">Length in chars</param>
		/// <param name="fill">Fill default field data</param>
		public void AddField(string name, int len, char fill)
		{
			FixedFieldDef fld = new FixedFieldDef(name, len, m_recordEnd, m_fields.Count, fill);
			AddField(fld);
		}

		/// <summary>
		/// Add the created field
		/// </summary>
		/// <param name="fld"></param>
		private void AddField(FixedFieldDef fld)
		{
			m_recordEnd += fld.Length;
			Debug.Assert(m_recordEnd <= m_linelen, "Field exceeds max record size");

			m_fields.Add(fld);
			m_fieldIndex.Add(fld.Name, fld);
		}

		/// <summary>
		/// Parse a record from a text file
		/// </summary>
		/// <param name="record">A line from the fixed width file</param>
		/// <returns>FixedRecord</returns>
		public FixedRecord Parse(string record)
		{
			if (record.Length != m_linelen)
			{
				throw new FormatException("Record length should be " + m_linelen + " long, but record to parse was " + record.Length + ". [" + record + "]");
			}

			FixedFieldData[] data = new FixedFieldData[m_fields.Count];
			for (int x = 0; x < m_fields.Count; x++)
			{
				data[x] = new FixedFieldData((FixedFieldDef)m_fields[x], record);
			}
			return new FixedRecord(this, data);
		}

		/// <summary>
		/// Creates an empty data record
		/// </summary>
		/// <returns>FixedRecord</returns>
		public FixedRecord Create()
		{
			FixedFieldData[] data = new FixedFieldData[m_fields.Count];

			for (int x = 0; x < m_fields.Count; x++)
			{
				data[x] = new FixedFieldData((FixedFieldDef)m_fields[x]);
			}
			return new FixedRecord(this, data);
		}

		/// <summary>
		/// Get a record from the record catch
		/// </summary>
		/// <returns>FixedRecord</returns>
		public FixedRecord CreateCached()
		{
			lock (m_cache)
			{
				if (m_cache.Count == 0)
				{
					return Create();
				}
				FixedRecord rcd = (FixedRecord)m_cache[m_cache.Count - 1];
				m_cache.RemoveElementAt(m_cache.Count - 1);
				return rcd;
			}
		}

		/// <summary>
		/// Put a record back into the cache
		/// </summary>
		/// <param name="rcd"></param>
		public void ReleaseCached(FixedRecord rcd)
		{
			lock (m_cache)
			{
				rcd.Reset();
				m_cache.Add(rcd);
			}
		}

		/// <summary>
		/// Get the field position in the file
		/// </summary>
		/// <param name="name">Field name</param>
		/// <returns>field ordinal position</returns>
		public int GetFieldOrdinal(string name)
		{
			return ((FixedFieldDef)m_fieldIndex[name]).OrdinalPosition;
		}

		protected Dictionary<string, FixedFieldDef> m_fieldIndex = new Dictionary<string, FixedFieldDef>();
		protected List<FixedFieldDef> m_fields = new List<FixedFieldDef>();
		protected int m_linelen;
		protected int m_recordEnd;

		protected Vector<FixedRecord> m_cache = new Vector<FixedRecord>();
	}
}
