using System;
using System.Data;
using System.Data.SqlClient;

using DOR.Core;

namespace DOR.Core.Data
{
	#region ParamBuilder Class

	/// <summary>
	/// Class to dynamically build SQLParameters based on the value type.
	/// </summary>
	public static class ParameterBuilder
	{
		/// <summary>
		/// Sets the parameter to Direction.Output.
		/// </summary>
		/// <param name="param"></param>
		/// <returns>The param arguemnt.</returns>
		public static SqlParameter SetOutputParameter(SqlParameter param)
		{
			param.Direction = ParameterDirection.Output;
			return param;
		}

		public static SqlParameter BuildParameter(string name, SqlDbType type, int len)
		{
			var Param = new SqlParameter(name, type, len);
			return Param;
		}

		/// <summary>
		/// build a SqlParameter from a decimal number
		/// </summary>
		/// <param name="name">Parameter Name String</param>
		/// <param name="value">decimal value</param>
		/// <returns></returns>
		public static SqlParameter BuildParameter(string name, decimal value)
		{
			var Param = new SqlParameter(name, SqlDbType.Money);
			Param.Value = value;
			return Param;
		}

		/// <summary>
		/// build a SqlParameter from a long
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static SqlParameter BuildParameter(string name, long value)
		{
			var Param = new SqlParameter(name, SqlDbType.BigInt);
			Param.Value = value;
			return Param;
		}

		/// <summary>
		/// build a SqlParameter from a short
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static SqlParameter BuildParameter(string name, short value)
		{
			var Param = new SqlParameter(name, SqlDbType.SmallInt);
			Param.Value = value;
			return Param;
		}
		/// <summary>
		/// build a SqlParameter from a datetime
		/// </summary>
		/// <param name="name">Parameter Name String</param>
		/// <param name="value">datetime value</param>
		/// <returns></returns>
		public static SqlParameter BuildParameter(string name, DateTime value)
		{
			if (value == DateTime.MinValue)
			{
				value = new DateTime(1753, 1, 1);
			}
			else if (value == DateTime.MaxValue)
			{
				value = new DateTime(9999, 12, 31, 23, 59, 59);
			}
			var Param = new SqlParameter(name, SqlDbType.DateTime);
			Param.Value = value;
			return Param;
		}

		/// <summary>
		/// build a SqlParameter from a string
		/// </summary>
		/// <param name="name">Parameter Name String</param>
		/// <param name="value">string value</param>
		/// <returns></returns>
		public static SqlParameter BuildParameter(string name, string value)
		{
			if (value == null)
			{
				return BuildNullParameter(name, SqlDbType.VarChar);
			}

			var param = new SqlParameter(name, SqlDbType.VarChar);
			param.Value = value;
			return param;
		}

		/// <summary>
		/// Build null parameter.
		/// </summary>
		public static SqlParameter BuildParameter(string name, SqlDbType type)
		{
			return BuildNullParameter(name, type);
		}

		/// <summary>
		/// build a SqlParameter from an int
		/// </summary>
		/// <param name="name">Parameter Name String</param>
		/// <param name="value">int value</param>
		/// <returns></returns>
		public static SqlParameter BuildParameter(string name, int value)
		{
			var Param = new SqlParameter(name, SqlDbType.Int);
			Param.Value = value;
			return Param;
		}

		public static SqlParameter BuildParameter(string name, char value)
		{
			var Param = new SqlParameter(name, SqlDbType.Char, 1);
			Param.Value = value;
			return Param;
		}

		/// <summary>
		/// build a SqlParameter from Money
		/// </summary>
		/// <param name="name">Parameter Name String</param>
		/// <param name="value">int value</param>
		/// <returns></returns>
		public static SqlParameter BuildParameter(string name, Money value)
		{
			var param = new SqlParameter(name, SqlDbType.Money);
			if (null == (object)value)
			{
				param.Value = DBNull.Value;
			}
			else
			{
				param.Value = (decimal)value;
			}
			return param;
		}

		/// <summary>
		/// build a null valued SqlParameter
		/// </summary>
		/// <param name="name">Parameter Name String</param>
		/// <returns></returns>
		public static SqlParameter BuildNullParameter(string name, SqlDbType type)
		{
			var param = new SqlParameter(name, type);
			param.Value = DBNull.Value;
			return param;
		}

		/// <summary>
		/// build a SqlParameter from a nullable Date Time
		/// </summary>
		/// <param name="name">Parameter Name String</param>
		/// <param name="value">nullable Date Time value</param>
		/// <returns></returns>
		public static SqlParameter BuildParameter(string name, DateTime? value)
		{
			if (value.HasValue)
			{
				if (value.Value == DateTime.MinValue)
				{
					value = new DateTime(1753, 1, 1);
				}
				else if (value.Value == DateTime.MaxValue)
				{
					value = new DateTime(9999, 12, 31, 23, 59, 59);
				}
			}
			var Param = new SqlParameter(name, SqlDbType.DateTime);
			Param.Value = value.HasValue ? (object) value.Value : DBNull.Value;
			return Param;
		}

		/// <summary>
		/// build a SqlParameter from a Date
		/// </summary>
		/// <param name="name">Parameter Name String</param>
		/// <param name="value">nullable Date Time value</param>
		/// <returns></returns>
		public static SqlParameter BuildParameter(string name, Date value)
		{
			var param = new SqlParameter(name, SqlDbType.DateTime);
			if (null == (object)value)
			{
				param.Value = DBNull.Value;
			}
			else
			{
				if (value == Date.MinValue)
				{
					value = new Date(1753, 1, 1);
				}
				else if (value == Date.MaxValue)
				{
					value = new Date(9999, 12, 31);
				}
				param.Value = value.ToDateTime();
			}
			return param;
		}

		/// <summary>
		/// build a SqlParameter from a int? value
		/// </summary>
		/// <param name="name">Parameter Name String</param>
		/// <param name="value">int? value</param>
		/// <returns></returns>
		public static SqlParameter BuildParameter(string name, int? value)
		{
			var param = new SqlParameter(name, SqlDbType.Int);
			param.Value = !value.HasValue ? (object) DBNull.Value : value.Value;
			return param;
		}

		/// <summary>
		/// build a SqlParameter from a nullable decimal (decimal?)
		/// </summary>
		/// <param name="name">Parameter Name String</param>
		/// <param name="value">nullable decimal value</param>
		/// <returns></returns>
		public static SqlParameter BuildParameter(string name, decimal? value)
		{
			var Param = new SqlParameter(name, SqlDbType.Money);
			Param.Value = !value.HasValue ? (object) DBNull.Value : value.Value;
			return Param;
		}

		/// <summary>
		/// build a SqlParameter from a bool
		/// </summary>
		/// <param name="name">Parameter Name String</param>
		/// <param name="value">bool value</param>
		/// <returns></returns>
		public static SqlParameter BuildParameter(string name, bool value)
		{
			var Param = new SqlParameter(name, SqlDbType.Bit);
			Param.Value = value ? 1 : 0;
			return Param;
		}

		public static SqlParameter BuildParameter(string name, bool? value)
		{
			var Param = new SqlParameter(name, SqlDbType.Bit);
			Param.Value = value.HasValue ? (object)value.Value : DBNull.Value;
			return Param;
		}

		public static SqlParameter BuildParameter(string name, byte value)
		{
			var param = new SqlParameter(name, SqlDbType.TinyInt);
			param.Value = value;
			return param;
		}

		public static SqlParameter BuildParameter(string name, byte? value)
		{
			var param = new SqlParameter(name, SqlDbType.TinyInt);
			param.Value = value.HasValue ? (object)value.Value : DBNull.Value;
			return param;
		}

		/// <summary>
		/// build a SqlParameter from a guid
		/// </summary>
		/// <param name="name">Parameter Name String</param>
		/// <param name="value">Guid value</param>
		/// <returns></returns>
		public static SqlParameter BuildParameter(string name, Guid value)
		{
			var param = new SqlParameter(name, SqlDbType.UniqueIdentifier);
			param.Value = value;
			return param;
		}

		/// <summary>
		/// Build an image parameter
		/// </summary>
		/// <param name="name">parameter name</param>
		/// <param name="value">binary data</param>
		public static SqlParameter BuildParameter(string name, byte[] value)
		{
			var param = new SqlParameter(name, SqlDbType.Image);
			param.Value = value;
			return param;
		}

	}

	#endregion ParamBuilder class
}
