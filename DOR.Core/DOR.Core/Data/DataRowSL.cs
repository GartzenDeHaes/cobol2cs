#if SILVERLIGHT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using DOR.Core.ComponentModel;

namespace DOR.Core.Data
{
	// We use this class to hold a row of data from the DataSet XML in a format that
	// the DataGrid can bind directly to.
	public class DataRow : NotifyPropertyChangedBase
	{
		// For DataGrid sorting to work, it needs to see the correct compile-time type of each
		// bound value. In Silverlight 4, we can use string indexers in our binding paths, but
		// the binding path must resolve to the right data type, so we need a dictionary for each
		// type of data we want to support. We bind each column of the DataGrid to one of these 
		// dictionaries, with a binding path like: IntVal[SomeColumn]
		public Dictionary<string, object> ValueMap = new Dictionary<string, object>();

		public int Count
		{
			get { return ValueMap.Count; }
		}

		public object this[string key]
		{
			get { return ValueMap[key]; }
			set
			{
				ValueMap[key] = value;
				RaisePropertyChanged("[" + key + "]");
			}
		}

		public IEnumerable<string> Keys
		{
			get
			{
				return ValueMap.Keys;
			}
		}
		
		internal DataRow
		(
			XElement row,
			IEnumerable<XElement> columns
		)
		{
			// Walk through the columns in the table's schema definition,
			// and load each corresponding row value into one of the dictionaries,
			// according to its data type. Note that we treat all types as nullable,
			// rather than read the nullability from the schema.
			foreach(XElement column in columns)
			{
				string columnName = (string)column.Attribute("name");
				string dataType = (string)column.Attribute("type");

				switch(dataType)
				{
					case "xs:int":
						ValueMap[columnName] = (int?)row.Element(columnName);
						break;
					case "xs:string":
						ValueMap[columnName] = (string)row.Element(columnName);
						break;
					case "xs:decimal":
						ValueMap[columnName] = (decimal?)row.Element(columnName);
						break;
					case "xs:dateTime":
						// For dateTimes, we don't want any time zone to be implied; we just want to take the date as-is.
						string dtStr = row.Element(columnName).Value;
						DateTime? dt = (dtStr == null) ? (DateTime?)null : XmlConvert.ToDateTime(dtStr, XmlDateTimeSerializationMode.Unspecified);
						ValueMap[columnName] = dt;
						break;
					default:
						throw new Exception(string.Format("Unknown column type: {0}", dataType));
				}
			}
		}

		internal XElement GetXml
		(
			string rowElementName
		)
		{
			return new XElement
			(
				rowElementName,
				ValueMap.Where(kvp => kvp.Value != null).Select(kvp => new XElement(kvp.Key, kvp.Value))
			);
		}
	}
}

#endif
