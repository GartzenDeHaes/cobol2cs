using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CobolParser.TandemData
{
	[Serializable]
	public class ColumnDef
	{
		public string Name
		{
			get;
			private set;
		}

		public int Size
		{
			get;
			private set;
		}

		public string SqlType
		{
			get;
			private set;
		}

		public int Precision
		{
			get;
			private set;
		}

		public int Scale
		{
			get;
			private set;
		}

		public bool IsIdentity
		{
			get;
			private set;
		}

		public bool IsNullable
		{
			get;
			private set;
		}

		public bool IsSigned
		{
			get;
			private set;
		}

		public ColumnDef
		(
			string name,
			int size,
			string type,
			int prec,
			int scale,
			bool isIdent,
			bool isNullable,
			bool isSigned
		)
		{
			Name = name;
			Size = size;
			SqlType = type;
			Precision = prec;
			Scale = scale;
			IsIdentity = isIdent;
			IsNullable = isNullable;
			IsSigned = isSigned;
		}

		public string CsharpTypeName()
		{
			if (Name.Equals("TRA_ID", StringComparison.InvariantCultureIgnoreCase))
			{
				return "TraId";
			}
			if (SqlType == "TIMESTAMP")
			{
				return "DateTime";
			}
			else if (Scale != 0 || SqlType == "DECIMAL")
			{
				return "decimal";
			}
			else if (SqlType == "INTEGER")
			{
				if (Size > 12)
				{
					return "long";
				}
				else
				{
					return "int";
				}
			}
			else if (SqlType == "CHAR")
			{
				return "string";
			}
			else if (SqlType == "BIGINT")
			{
				return "long";
			}
			else if (SqlType == "SMALLINT")
			{
				return "short";
			}
			else if (SqlType == "DATE")
			{
				return "Date";
			}
			else if (SqlType == "VARCHAR")
			{
				return "string";
			}
			else
			{
				throw new Exception("ColumDef type not mapped");
			}
		}

		public string ParseClass()
		{
			if (Name.Equals("TRA_ID", StringComparison.InvariantCultureIgnoreCase))
			{
				return "TraId";
			}
			if (SqlType == "TIMESTAMP")
			{
				return "DateTime";
			}
			else if (Scale != 0 || SqlType == "DECIMAL")
			{
				return "Decimal";
			}
			else if (SqlType == "INTEGER")
			{
				if (Size > 12)
				{
					return "Int64";
				}
				else
				{
					return "Int32";
				}
			}
			else if (SqlType == "CHAR")
			{
				return "";
			}
			else if (SqlType == "BIGINT")
			{
				return "Int64";
			}
			else if (SqlType == "SMALLINT")
			{
				return "Int16";
			}
			else if (SqlType == "DATE")
			{
				return "Date";
			}
			else if (SqlType == "VARCHAR")
			{
				return "";
			}
			else
			{
				throw new Exception("ColumDef type not mapped");
			}
		}
	}
}
