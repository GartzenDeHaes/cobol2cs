using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Xml;

using DOR.Core.Collections;

namespace DOR.Core.Data.Tandem
{
	[Serializable]
	public class TandemDataReader : IDataReaderEx
	{
		private XmlDocument _doc;
		private int _pos = -1;
		private XmlNodeList _rows;
		private Dictionary<string, int> _colIdx = new Dictionary<string, int>();
		private XmlNodeList _meta;

		internal XmlDocument Document
		{
			get { return _doc; }
		}

		public TandemDataReader()
		{
			_doc = new XmlDocument();
			_rows = _doc.SelectNodes("ROWS/row");
		}

		public TandemDataReader(string xml)
		{
			_doc = new XmlDocument();
			_doc.LoadXml(xml);
			_rows = _doc.SelectNodes("ROWS/row");
			LoadMetadata();
		}

		public TandemDataReader(XmlDocument doc)
		{
			_doc = doc;
			_rows = _doc.SelectNodes("ROWS/row");
			LoadMetadata();
		}

		public TandemDataReader(IDataReader reader)
		: this(ConvertToCompatibleXml(reader))
		{
		}

		private void MetaAddInner(string name, int idx)
		{
			int pos = name.IndexOf('.');
			if (pos > -1)
			{
				string col = name.Substring(pos + 1);
				if (!_colIdx.ContainsKey(col))
				{
					_colIdx.Add(col, idx);
				}
			}

			if (_colIdx.ContainsKey(name))
			{
				_colIdx.Add(name + idx, idx);
			}
			else
			{
				_colIdx.Add(name, idx);
			}
		}

		private void LoadMetadata()
		{
			if (_colIdx.Count > 0)
			{
				return;
			}

			_meta = _doc.SelectNodes("/ROWS/META/*");
			if (_meta.Count == 0)
			{
				if (_rows.Count == 0)
				{
					return;
				}
				for (int x = 0; x < _rows[0].Attributes.Count; x++)
				{
					XmlNode a = _rows[0].Attributes[x];
					MetaAddInner(a.Name, x);
				}
				return;
			}
			for (int x = 0; x < _meta.Count; x++)
			{
				MetaAddInner(_meta.Item(x).Name, x);
			}
		}

		// Summary:
		//     Gets a value indicating the depth of nesting for the current row.
		//
		// Returns:
		//     The level of nesting.
		public int Depth { get { return 1; } }
		
		//
		// Summary:
		//     Gets a value indicating whether the data reader is closed.
		//
		// Returns:
		//     true if the data reader is closed; otherwise, false.
		public bool IsClosed { get; private set; }

		//
		// Summary:
		//     Gets the number of rows changed, inserted, or deleted by execution of the
		//     SQL statement.
		//
		// Returns:
		//     The number of rows changed, inserted, or deleted; 0 if no rows were affected
		//     or the statement failed; and -1 for SELECT statements.
		public int RecordsAffected { get { return _rows.Count; } }

		// Summary:
		//     Closes the System.Data.IDataReader Object.
		public void Close()
		{
			IsClosed = true;
		}

		//
		// Summary:
		//     Returns a System.Data.DataTable that describes the column metadata of the
		//     System.Data.IDataReader.
		//
		// Returns:
		//     A System.Data.DataTable that describes the column metadata.
		//
		// Exceptions:
		//   System.InvalidOperationException:
		//     The System.Data.IDataReader is closed.
		public DataTable GetSchemaTable()
		{
			throw new NotImplementedException();
		}

		//
		// Summary:
		//     Advances the data reader to the next result, when reading the results of
		//     batch SQL statements.
		//
		// Returns:
		//     true if there are more rows; otherwise, false.
		public bool NextResult()
		{
			throw new NotImplementedException();
		}

		//
		// Summary:
		//     Advances the System.Data.IDataReader to the next record.
		//
		// Returns:
		//     true if there are more rows; otherwise, false.
		public bool Read()
		{
			if (++_pos >= _rows.Count)
			{
				return false;
			}

			return true;
		}

		// Summary:
		//     Gets the number of columns in the current row.
		//
		// Returns:
		//     When not positioned in a valid recordset, 0; otherwise, the number of columns
		//     in the current record. The default is -1.
		public int FieldCount 
		{ 
			get 
			{
				if (_rows.Count > 0)
				{
					return _rows[0].Attributes.Count;
				}
				LoadMetadata();

				if (_meta.Count == 0)
				{
					throw new Exception("no rows or meta data");
				}
				return _meta.Count;
			}
		}

		// Summary:
		//     Gets the column located at the specified index.
		//
		// Parameters:
		//   i:
		//     The zero-based index of the column to get.
		//
		// Returns:
		//     The column located at the specified index as an System.Object.
		//
		// Exceptions:
		//   System.IndexOutOfRangeException:
		//     The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.

		public object this[int i] 
		{
			get 
			{ 
				var val =_rows[_pos].Attributes[i].InnerText;
				var type = GetDataTypeName(i).ToLower();

				switch (type)
				{
					case "int":
					case "integer":
						return Int32.Parse(val);
					case "varchar":
						return val;
					case "double":
						return Double.Parse(val);
					case "decimal":
						return Decimal.Parse(val);
					case "char":
						// fixed length string, not a single char
						return val;
					case "smallint":
						return Int16.Parse(val);
					case "tinyint":
						return Byte.Parse(val);
				}

				throw new Exception("internal error, unknown type");
			}
		}

		//
		// Summary:
		//     Gets the column with the specified name.
		//
		// Parameters:
		//   name:
		//     The name of the column to find.
		//
		// Returns:
		//     The column with the specified name as an System.Object.
		//
		// Exceptions:
		//   System.IndexOutOfRangeException:
		//     No column with the specified name was found.
		public object this[string name] 
		{ 
			get 
			{
				return this[_colIdx[name]];
			} 
		}

		// Summary:
		//     Gets the value of the specified column as a Boolean.
		//
		// Parameters:
		//   i:
		//     The zero-based column ordinal.
		//
		// Returns:
		//     The value of the column.
		//
		// Exceptions:
		//   System.IndexOutOfRangeException:
		//     The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.
		public bool GetBoolean(int i)
		{
			if (IsDBNull(i))
			{
				return false;
			}
			return Boolean.Parse(_rows[_pos].Attributes[i].InnerText);
		}

		public bool GetBoolean(string col)
		{
			return GetBoolean(GetOrdinal(col));
		}

		//
		// Summary:
		//     Gets the 8-bit unsigned integer value of the specified column.
		//
		// Parameters:
		//   i:
		//     The zero-based column ordinal.
		//
		// Returns:
		//     The 8-bit unsigned integer value of the specified column.
		//
		// Exceptions:
		//   System.IndexOutOfRangeException:
		//     The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.
		
		public byte GetByte(int i)
		{
			if (IsDBNull(i))
			{
				return 0;
			}
			return Byte.Parse(_rows[_pos].Attributes[i].InnerText);
		}

		//
		// Summary:
		//     Reads a stream of bytes from the specified column offset into the buffer
		//     as an array, starting at the given buffer offset.
		//
		// Parameters:
		//   i:
		//     The zero-based column ordinal.
		//
		//   fieldOffset:
		//     The index within the field from which to start the read operation.
		//
		//   buffer:
		//     The buffer into which to read the stream of bytes.
		//
		//   bufferoffset:
		//     The index for buffer to start the read operation.
		//
		//   length:
		//     The number of bytes to read.
		//
		// Returns:
		//     The actual number of bytes read.
		//
		// Exceptions:
		//   System.IndexOutOfRangeException:
		//     The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.
		
		public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			throw new NotImplementedException();
		}

		//
		// Summary:
		//     Gets the character value of the specified column.
		//
		// Parameters:
		//   i:
		//     The zero-based column ordinal.
		//
		// Returns:
		//     The character value of the specified column.
		//
		// Exceptions:
		//   System.IndexOutOfRangeException:
		//     The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.
		public char GetChar(int i)
		{
			if (IsDBNull(i))
			{
				return '\0';
			}
			return Char.Parse(_rows[_pos].Attributes[i].InnerText);
		}

		public char GetChar(string fieldName)
		{
			return GetChar(GetOrdinal(fieldName));
		}

		//
		// Summary:
		//     Reads a stream of characters from the specified column offset into the buffer
		//     as an array, starting at the given buffer offset.
		//
		// Parameters:
		//   i:
		//     The zero-based column ordinal.
		//
		//   fieldoffset:
		//     The index within the row from which to start the read operation.
		//
		//   buffer:
		//     The buffer into which to read the stream of bytes.
		//
		//   bufferoffset:
		//     The index for buffer to start the read operation.
		//
		//   length:
		//     The number of bytes to read.
		//
		// Returns:
		//     The actual number of characters read.
		//
		// Exceptions:
		//   System.IndexOutOfRangeException:
		//     The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.
		public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			throw new NotImplementedException();
		}

		//
		// Summary:
		//     Returns an System.Data.IDataReader for the specified column ordinal.
		//
		// Parameters:
		//   i:
		//     The index of the field to find.
		//
		// Returns:
		//     An System.Data.IDataReader.
		//
		// Exceptions:
		//   System.IndexOutOfRangeException:
		//     The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.
		
		public IDataReader GetData(int i)
		{
			throw new NotImplementedException();
		}

		//
		// Summary:
		//     Gets the data type information for the specified field.
		//
		// Parameters:
		//   i:
		//     The index of the field to find.
		//
		// Returns:
		//     The data type information for the specified field.
		//
		// Exceptions:
		//   System.IndexOutOfRangeException:
		//     The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.

		public string GetDataTypeName(int i)
		{
			LoadMetadata();
			if (_meta == null || _meta.Count == 0)
			{
				if (StringHelper.IsInt(GetString(i)))
				{
					return "INTEGER";
				}
				else if (StringHelper.IsNumeric(GetString(i)))
				{
					return "FLOAT";
				}
				return "VARCHAR";
			}

			return _meta[i].Attributes["type"].Value;
		}

		public string GetDataTypeName(string colName)
		{
			return GetDataTypeName(GetOrdinal(colName));
		}

		public int GetScale(int i)
		{
			LoadMetadata();
			if (_meta == null || _meta.Count == 0)
			{
				throw new Exception("no metadata found");
			}
			return Int32.Parse(_meta[i].Attributes["scale"].Value);
		}

		public int GetScale(string col)
		{
			return GetPrecision(GetOrdinal(col));
		}

		public int GetPrecision(int i)
		{
			LoadMetadata();
			if (_meta == null || _meta.Count == 0)
			{
				throw new Exception("no metadata found");
			}
			return Int32.Parse(_meta[i].Attributes["precision"].Value);
		}

		public int GetPrecision(string col)
		{
			return GetPrecision(GetOrdinal(col));
		}

		//
		// Summary:
		//     Gets the date and time data value of the specified field.
		//
		// Parameters:
		//   i:
		//     The index of the field to find.
		//
		// Returns:
		//     The date and time data value of the specified field.
		//
		// Exceptions:
		//   System.IndexOutOfRangeException:
		//     The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.
		
		public DateTime GetDateTime(int i)
		{
			if (IsDBNull(i))
			{
				return DateTime.MinValue;
			}

			string txt = _rows[_pos].Attributes[i].InnerText;
			DateTime dtm;

			if (DateTime.TryParse(txt, out dtm))
			{
				return dtm;
			}

			if (DateTime.TryParseExact(txt, "yyyy-MM-dd:HH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtm))
			{
				return dtm;
			}

			if (DateTime.TryParseExact(txt, "yyyy-MM-dd HH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtm))
			{
				return dtm;
			}

			throw new FormatException("Can't convert " + txt + " to DateTime");
		}

		public DateTime GetDateTime(string col)
		{
			return GetDateTime(GetOrdinal(col));
		}

		//
		// Summary:
		//     Gets the fixed-position numeric value of the specified field.
		//
		// Parameters:
		//   i:
		//     The index of the field to find.
		//
		// Returns:
		//     The fixed-position numeric value of the specified field.
		//
		// Exceptions:
		//   System.IndexOutOfRangeException:
		//     The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.
		public decimal GetDecimal(int i)
		{
			if (IsDBNull(i))
			{
				return 0;
			}
			string value = _rows[_pos].Attributes[i].InnerText;
			if (String.IsNullOrEmpty(value))
			{
				return 0;
			}
			if (!StringHelper.IsNumeric(value))
			{
				Debug.Assert(false);
			}
			return Decimal.Parse(value);
		}

		public decimal GetDecimal(string col)
		{
			return GetDecimal(GetOrdinal(col));
		}

		//
		// Summary:
		//     Gets the double-precision floating point number of the specified field.
		//
		// Parameters:
		//   i:
		//     The index of the field to find.
		//
		// Returns:
		//     The double-precision floating point number of the specified field.
		//
		// Exceptions:
		//   System.IndexOutOfRangeException:
		//     The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.

		public double GetDouble(int i)
		{
			if (IsDBNull(i))
			{
				return 0;
			}
			return Double.Parse(_rows[_pos].Attributes[i].InnerText);
		}

		public double GetDouble(string col)
		{
			return GetDouble(GetOrdinal(col));
		}

		//
		// Summary:
		//     Gets the System.Type information corresponding to the type of System.Object
		//     that would be returned from System.Data.IDataRecord.GetValue(System.Int32).
		//
		// Parameters:
		//   i:
		//     The index of the field to find.
		//
		// Returns:
		//     The System.Type information corresponding to the type of System.Object that
		//     would be returned from System.Data.IDataRecord.GetValue(System.Int32).
		//
		// Exceptions:
		//   System.IndexOutOfRangeException:
		//     The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.
		public Type GetFieldType(int i)
		{
			throw new NotImplementedException();
		}

		//
		// Summary:
		//     Gets the single-precision floating point number of the specified field.
		//
		// Parameters:
		//   i:
		//     The index of the field to find.
		//
		// Returns:
		//     The single-precision floating point number of the specified field.
		//
		// Exceptions:
		//   System.IndexOutOfRangeException:
		//     The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.

		public float GetFloat(int i)
		{
			if (IsDBNull(i))
			{
				return 0;
			}
			return (float)Double.Parse(_rows[_pos].Attributes[i].InnerText);
		}

		//
		// Summary:
		//     Returns the GUID value of the specified field.
		//
		// Parameters:
		//   i:
		//     The index of the field to find.
		//
		// Returns:
		//     The GUID value of the specified field.
		//
		// Exceptions:
		//   System.IndexOutOfRangeException:
		//     The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.
		
		public Guid GetGuid(int i)
		{
			throw new NotImplementedException();
		}

		//
		// Summary:
		//     Gets the 16-bit signed integer value of the specified field.
		//
		// Parameters:
		//   i:
		//     The index of the field to find.
		//
		// Returns:
		//     The 16-bit signed integer value of the specified field.
		//
		// Exceptions:
		//   System.IndexOutOfRangeException:
		//     The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.

		public short GetInt16(int i)
		{
			if (IsDBNull(i))
			{
				return 0;
			}
			return Int16.Parse(_rows[_pos].Attributes[i].InnerText);
		}

		public short GetInt16(string col)
		{
			return GetInt16(GetOrdinal(col));
		}

		//
		// Summary:
		//     Gets the 32-bit signed integer value of the specified field.
		//
		// Parameters:
		//   i:
		//     The index of the field to find.
		//
		// Returns:
		//     The 32-bit signed integer value of the specified field.
		//
		// Exceptions:
		//   System.IndexOutOfRangeException:
		//     The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.
		
		public int GetInt32(int i)
		{
			if (IsDBNull(i))
			{
				return 0;
			}
			string val = _rows[_pos].Attributes[i].InnerText;
			if (val.IndexOf('.') > -1)
			{
				return (int)Decimal.Parse(val);
			}
			if (String.IsNullOrEmpty(val))
			{
				return 0;
			}
			return Convert.ToInt32(val);
		}

		public int GetInt32(string col)
		{
			return GetInt32(GetOrdinal(col));
		}

		//
		// Summary:
		//     Gets the 64-bit signed integer value of the specified field.
		//
		// Parameters:
		//   i:
		//     The index of the field to find.
		//
		// Returns:
		//     The 64-bit signed integer value of the specified field.
		//
		// Exceptions:
		//   System.IndexOutOfRangeException:
		//     The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.
		public long GetInt64(int i)
		{
			if (IsDBNull(i))
			{
				return 0;
			}
			return Int64.Parse(_rows[_pos].Attributes[i].InnerText);
		}

		public long GetInt64(string col)
		{
			return GetInt64(GetOrdinal(col));
		}

		//
		// Summary:
		//     Gets the name for the field to find.
		//
		// Parameters:
		//   i:
		//     The index of the field to find.
		//
		// Returns:
		//     The name of the field or the empty string (""), if there is no value to return.
		//
		// Exceptions:
		//   System.IndexOutOfRangeException:
		//     The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.
		
		public string GetName(int i)
		{
			if (_rows.Count > 0)
			{
				return _rows[0].Attributes[i].Name;
			}

			LoadMetadata();
			if (_meta.Count == 0)
			{
				throw new Exception("No rows or metadata");
			}
			return _meta[i].Name;
		}

		//
		// Summary:
		//     Return the index of the named field.
		//
		// Parameters:
		//   name:
		//     The name of the field to find.
		//
		// Returns:
		//     The index of the named field.
		public int GetOrdinal(string name)
		{
			return _colIdx[name];
		}

		//
		// Summary:
		//     Gets the string value of the specified field.
		//
		// Parameters:
		//   i:
		//     The index of the field to find.
		//
		// Returns:
		//     The string value of the specified field.
		//
		// Exceptions:
		//   System.IndexOutOfRangeException:
		//     The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.
		
		public string GetString(int i)
		{
			string s = _rows[_pos].Attributes[i].InnerText;
			if (s == "$$SQLNULL$$")
			{
				return "";
			}

			return s;
		}

		//
		// Summary:
		//     Return the value of the specified field.
		//
		// Parameters:
		//   i:
		//     The index of the field to find.
		//
		// Returns:
		//     The System.Object which will contain the field value upon return.
		//
		// Exceptions:
		//   System.IndexOutOfRangeException:
		//     The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.
		public object GetValue(int i)
		{
			if (IsDBNull(i))
			{
				return null;
			}
			return _rows[_pos].Attributes[i].InnerText;
		}

		//
		// Summary:
		//     Populates an array of objects with the column values of the current record.
		//
		// Parameters:
		//   values:
		//     An array of System.Object to copy the attribute fields into.
		//
		// Returns:
		//     The number of instances of System.Object in the array.
		
		public int GetValues(object[] values)
		{
			throw new NotImplementedException();
		}

		//
		// Summary:
		//     Return whether the specified field is set to null.
		//
		// Parameters:
		//   i:
		//     The index of the field to find.
		//
		// Returns:
		//     true if the specified field is set to null; otherwise, false.
		//
		// Exceptions:
		//   System.IndexOutOfRangeException:
		//     The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.
		public bool IsDBNull(int i)
		{
			return _rows[_pos].Attributes[i].InnerText == "$$SQLNULL$$";
		}

		public string GetString(string f)
		{
			return GetString(GetOrdinal(f));
		}

		public bool IsDBNull(string f)
		{
			return IsDBNull(GetOrdinal(f));
		}

		public override string ToString()
		{
			return _doc.InnerXml;
		}

		public DataSet ToDataSet()
		{
			LoadMetadata();

			DataSet ds = new DataSet();
			DataTable dt = ds.Tables.Add();

			for (int x = 0; x < FieldCount; x++)
			{
				dt.Columns.Add(GetName(x));
			}

			foreach (XmlNode row in _rows)
			{
				DataRow dr = dt.NewRow();
				dt.Rows.Add(dr);

				for (int x = 0; x < FieldCount; x++)
				{
					dr[x] = row.Attributes[x].Value;
				}
			}

			dt = ds.Tables.Add("SQLSA");

			dt.Columns.Add("table_name");
			dt.Columns.Add("records_accessed");
			dt.Columns.Add("records_used");
			dt.Columns.Add("disk_reads");
			dt.Columns.Add("messages");
			dt.Columns.Add("message_bytes");
			dt.Columns.Add("waits");
			dt.Columns.Add("escalations");

			SQLSA sqlsa = SQLSA.Parse(_doc);
			for (int x = 0; x < sqlsa.num_tables; x++)
			{
				DataRow dr = dt.NewRow();
				dr["table_name"] = sqlsa.stats[x].table_name;
				dr["records_accessed"] = sqlsa.stats[x].records_accessed;
				dr["records_used"] = sqlsa.stats[x].records_used;
				dr["disk_reads"] = sqlsa.stats[x].disk_reads;
				dr["messages"] = sqlsa.stats[x].messages;
				dr["message_bytes"] = sqlsa.stats[x].message_bytes;
				dr["waits"] = sqlsa.stats[x].waits;
				dr["escalations"] = sqlsa.stats[x].escalations;

				dt.Rows.Add(dr);
			}

			return ds;
		}

		public void Dispose()
		{
			_rows = null;
			_colIdx.Clear();
			_colIdx = null;
		}

		public TandemDataReader Clone()
		{
			return new TandemDataReader((XmlDocument)_doc.CloneNode(true));
		}

		public static XmlDocument ConvertToCompatibleXml(IDataReader reader)
		{
			var doc = new XmlDocument();
			var rows = doc.CreateElement("ROWS");
			var meta = doc.CreateElement("META");

			doc.AppendChild(rows);
			rows.AppendChild(meta);

			for (int x = 0; x < reader.FieldCount; x++)
			{
				var node = doc.CreateElement(reader.GetName(x));
				var attr = doc.CreateAttribute("type");
				
				attr.AppendChild(doc.CreateTextNode(reader.GetDataTypeName(x)));
				node.Attributes.Append(attr);

				meta.AppendChild(node);
			}

			while(reader.Read())
			{
				var row = doc.CreateElement("row");

				for (int x = 0; x < reader.FieldCount; x++)
				{
					var attr = doc.CreateAttribute(reader.GetName(x));

					attr.AppendChild(doc.CreateTextNode(reader[x].ToString()));
					row.Attributes.Append(attr);
				}

				rows.AppendChild(row);
			}

			return doc;
		}
	}
}
