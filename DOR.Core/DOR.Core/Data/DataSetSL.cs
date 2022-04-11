#if SILVERLIGHT

using System;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DOR.Core.Data
{
	public class DataSet
	{
		public ObservableCollection<DataTable> Tables
		{
			get;
			private set;
		}

		public DataSet()
		{
			Tables = new ObservableCollection<DataTable>();
		}

		public void ReadXml(XElement schema, XElement data)
		{
			foreach (var t in Tables)
			{
				t.Rows.Clear();
			}

			Tables.Clear();

			var names = (from name in data.Elements() select name.Name).Distinct();

			foreach (var name in names)
			{
				Tables.Add
				(
					new DataTable
					(
						(
						from 
							n 
						in 
							schema.Elements().Elements().Elements().Elements()
						where 
							n.Attribute("name").Value == name
						select 
							n
						).First(), 
						(from t in data.Descendants() where t.Name == name select t).AsEnumerable<XElement>()
					)
				);
			}
		}
	}
}

#endif
