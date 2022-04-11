using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DOR.Core.Data
{
	public interface ISqlDataAccess : IDataAccess, IDisposable
	{
		int ExecuteNonQuery(string sql);

		IDbCommand CreateCommand(string cmdText);

		IDataReader ExecuteReader(string sql, int? cacheDurationInMinutes = null);
	}
}
