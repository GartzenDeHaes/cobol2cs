using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

using DOR.Core;

namespace DOR.Core.IO
{
	/// <summary>
	/// The data contained in a fixed width record
	/// </summary>
	public class FixedRecord
	{
		#region Members

		protected FixedRecordDef m_recdef;
		protected FixedFieldData[] m_data;

		#endregion Members

		#region Ctor's
		/// <summary>
		/// Initialize the record from the definition
		/// </summary>
		/// <param name="def"></param>
		/// <param name="fields"></param>
		public FixedRecord ( FixedRecordDef def, FixedFieldData[] fields )
		{
			m_recdef = def;
			m_data = fields;
		}
		#endregion Ctor's

		#region Public Properties
		/// <summary>
		/// Number of fields in the record
		/// </summary>
		public int Count
		{
			get
			{
				return m_data.Length;
			}
		}

		/// <summary>
		/// Data field by ordinal position
		/// </summary>
		public FixedFieldData this[ int index ]
		{
			get
			{
				return m_data[index];
			}
		}

		/// <summary>
		/// Data field by name
		/// </summary>
		public FixedFieldData this[ string name ]
		{
			get
			{
				return m_data[ m_recdef.GetFieldOrdinal( name ) ];
			}
		}
		#endregion Public Properties

		#region Public Methods

		/// <summary>
		/// Reset all fields to default values
		/// </summary>
		public void Reset()
		{
			for( int x = 0; x < m_data.Length; x++ )
			{
				m_data[x].Reset();
			}
		}

		/// <summary>
		/// Create the fixed text record
		/// </summary>
		/// <returns>Fixed text string</returns>
		public override string ToString()
		{
			StringBuilder buf = new StringBuilder();
			for( int x = 0; x < m_data.Length; x++ )
			{
				buf.Append( m_data[x].Value );
			}
			return buf.ToString() + "\r\n";
		}
		#endregion Public Methods
	}
}