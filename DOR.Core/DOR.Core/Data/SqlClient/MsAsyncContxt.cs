using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DOR.Core.Data.SqlClient
{
	public class MsAsyncContext : AsyncContext
	{
		internal SqlCommand Command
		{
			get;
			private set;
		}

		public IDataReader Reader
		{
			get;
			set;
		}

		public int RecordCount
		{
			get;
			set;
		}

		public string ParameterKey
		{
			get;
			private set;
		}

		public MsAsyncContext(SqlCommand cmd, string paramKey, int? cacheDurationInMinutes)
		{
			Command = cmd;
			ParameterKey = paramKey;
			CacheDurationInMinutes = cacheDurationInMinutes;
		}

		public MsAsyncContext(IDataReader reader)
		{
			Reader = reader;
		}
	}
}
