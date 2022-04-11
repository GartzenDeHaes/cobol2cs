#if SILVERLIGHT

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace DOR.Core.Data
{
	public class DataTable
	{
		private ObservableCollection<DataRow> _rows = new ObservableCollection<DataRow>();
		public ObservableCollection<DataRow> Rows
		{
			get { return _rows; }
		}

		public DataTable()
		{
		}

		public DataTable(XElement schema, IEnumerable<XElement> data)
		{
			// From the schema, find the table we will use to populate the DataGrid. We'll use 
			// Microsoft's msdata:MainDataTable attribute to lead us to the table of interest.
			string tableName = schema.Attribute("name").Value;

			// Find the schema description of the DataTable named by msdata:MainDataTable.
			XElement table = schema;

			// Get the list of columns in the DataTable. Each of these elements will provide
			// us with the name, data type, and caption of each column.
			List<XElement> columns = table.Elements().Elements().Elements().ToList();

			// Set up the DataGrid to match the columns from the schema.
			//foreach (XElement column in columns)
			//{
			//    DataGridTextColumn gridCol = new DataGridTextColumn();

			//    // We use the msdata:Caption attribute to specify the column header.
			//    gridCol.Header = (string)column.Attribute(msdata + "Caption");

			//    // Ask our DataRow class to generate a binding path for this column,
			//    // using the schema definition of the column.
			//    Binding binding = new Binding(DataRow.GetBindingPath(column));

			//    // Depending on the type of column, do a bit of light formatting.
			//    // You would extend this according to the needs of your user interface. 
			//    string dataType = (string)column.Attribute("type");
			//    if (dataType == "xs:int")
			//    {
			//        // Right-align integer values.
			//        gridCol.CellStyle = (Style)Resources["AlignRight"];
			//    }
			//    if (dataType == "xs:decimal")
			//    {
			//        // Right-align decimal values, and format them accounting-style.
			//        gridCol.CellStyle = (Style)Resources["AlignRight"];
			//        binding.StringFormat = "#,##0.00 ;(#,##0.00)";
			//    }
			//    else if (dataType == "xs:dateTime")
			//    {
			//        // In this sample, our DateTimes don't have any time portion, so just show the date.
			//        binding.StringFormat = "MMM d, yyyy";
			//    }

			//    // Add the new column to the grid.
			//    gridCol.Binding = binding;
			//    DataGrid1.Columns.Add(gridCol);
			//}

			// Generate a collection of our DataRow objects. Each DataRow holds a copy of
			// one of the rows from the XML. It uses the column schema definition to load
			// the data from the XML.
			foreach (XElement row in data)
			{
				Rows.Add(new DataRow(row, columns));
			}
		}

		private static readonly Regex PropertNameRegex =
				new Regex(@"^[A-Za-z]+[A-Za-z1-9_]*$", RegexOptions.Singleline);

		private static readonly Dictionary<string, Type> _typeBySigniture =
				new Dictionary<string, Type>();

		public IEnumerable ToDataSource()
		{
			if (Rows.Count == 0)
			{
				return new object[] { };
			}

			IDictionary firstDict = Rows[0].ValueMap;

			string typeSigniture = GetTypeSigniture(firstDict);

			Type objectType = GetTypeByTypeSigniture(typeSigniture);

			if (objectType == null)
			{
				TypeBuilder tb = GetTypeBuilder(typeSigniture);

				ConstructorBuilder constructor =
							tb.DefineDefaultConstructor(
										MethodAttributes.Public |
										MethodAttributes.SpecialName |
										MethodAttributes.RTSpecialName);

				foreach (DictionaryEntry pair in firstDict)
				{
					string key = pair.Key.ToString();
					int dotPos = key.IndexOf('.');
					if (dotPos > -1)
					{
						key = key.Substring(dotPos + 1);
					}
					if (PropertNameRegex.IsMatch(Convert.ToString(key), 0))
					{
						CreateProperty(tb,
										Convert.ToString(key),
										GetValueType(pair.Value));
					}
					else
					{
						throw new ArgumentException(
									@"Each key of IDictionary must be
                                alphanumeric and start with character.");
					}
				}
				objectType = tb.CreateType();

				_typeBySigniture.Add(typeSigniture, objectType);
			}

			return GenerateEnumerable
			(
				objectType, 
				from r in Rows select r.ValueMap, 
				firstDict
			);
		}

		private static Type GetTypeByTypeSigniture(string typeSigniture)
		{
			Type type;
			return _typeBySigniture.TryGetValue(typeSigniture, out type) ? type : null;
		}

		private static Type GetValueType(object value)
		{
			return value == null ? typeof(object) : value.GetType();
		}

		private static string GetTypeSigniture(IDictionary firstDict)
		{
			StringBuilder sb = new StringBuilder();
			foreach (DictionaryEntry pair in firstDict)
			{
				string key = pair.Key.ToString();
				int dotPos = key.IndexOf('.');
				if (dotPos > -1)
				{
					key = key.Substring(dotPos + 1);
				}
				sb.AppendFormat("_{0}_{1}", key, GetValueType(pair.Value));
			}
			return sb.ToString().GetHashCode().ToString().Replace("-", "Minus");
		}

		private static IEnumerable GenerateEnumerable
		(
			Type objectType, 
			IEnumerable list, 
			IDictionary firstDict
		)
		{
			var listType = typeof(List<>).MakeGenericType(new[] { objectType });
			var listOfCustom = Activator.CreateInstance(listType);

			foreach (IDictionary currentDict in list)
			{
				if (currentDict == null)
				{
					throw new ArgumentException("IDictionary entry cannot be null");
				}
				var row = Activator.CreateInstance(objectType);
				foreach (DictionaryEntry pair in firstDict)
				{
					if (currentDict.Contains(pair.Key))
					{
						string key = pair.Key.ToString();
						int dotPos = key.IndexOf('.');
						if (dotPos > -1)
						{
							key = key.Substring(dotPos + 1);
						}
						PropertyInfo property =
							objectType.GetProperty(Convert.ToString(key));
						property.SetValue(
							row,
							Convert.ChangeType(
									currentDict[pair.Key],
									property.PropertyType,
									null),
							null);
					}
				}
				listType.GetMethod("Add").Invoke(listOfCustom, new[] { row });
			}
			return listOfCustom as IEnumerable;
		}

		private static TypeBuilder GetTypeBuilder(string typeSigniture)
		{
			AssemblyName an = new AssemblyName("TempAssembly" + typeSigniture);
			AssemblyBuilder assemblyBuilder =
				AppDomain.CurrentDomain.DefineDynamicAssembly(
					an, AssemblyBuilderAccess.Run);
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

			TypeBuilder tb = moduleBuilder.DefineType("TempType" + typeSigniture
								, TypeAttributes.Public |
								TypeAttributes.Class |
								TypeAttributes.AutoClass |
								TypeAttributes.AnsiClass |
								TypeAttributes.BeforeFieldInit |
								TypeAttributes.AutoLayout
								, typeof(object));
			return tb;
		}

		private static void CreateProperty(
						TypeBuilder tb, string propertyName, Type propertyType)
		{
			FieldBuilder fieldBuilder = tb.DefineField("_" + propertyName,
														propertyType,
														FieldAttributes.Private);


			PropertyBuilder propertyBuilder =
				tb.DefineProperty(
					propertyName, PropertyAttributes.HasDefault, propertyType, null);
			MethodBuilder getPropMthdBldr =
				tb.DefineMethod("get_" + propertyName,
					MethodAttributes.Public |
					MethodAttributes.SpecialName |
					MethodAttributes.HideBySig,
					propertyType, Type.EmptyTypes);

			ILGenerator getIL = getPropMthdBldr.GetILGenerator();

			getIL.Emit(OpCodes.Ldarg_0);
			getIL.Emit(OpCodes.Ldfld, fieldBuilder);
			getIL.Emit(OpCodes.Ret);

			MethodBuilder setPropMthdBldr =
				tb.DefineMethod("set_" + propertyName,
				  MethodAttributes.Public |
				  MethodAttributes.SpecialName |
				  MethodAttributes.HideBySig,
				  null, new Type[] { propertyType });

			ILGenerator setIL = setPropMthdBldr.GetILGenerator();

			setIL.Emit(OpCodes.Ldarg_0);
			setIL.Emit(OpCodes.Ldarg_1);
			setIL.Emit(OpCodes.Stfld, fieldBuilder);
			setIL.Emit(OpCodes.Ret);

			propertyBuilder.SetGetMethod(getPropMthdBldr);
			propertyBuilder.SetSetMethod(setPropMthdBldr);
		}
	}
}

#endif
