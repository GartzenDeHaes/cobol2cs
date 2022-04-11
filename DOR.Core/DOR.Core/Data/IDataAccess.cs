using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DOR.Core.Data
{
	public interface IDataAccess
	{
		string ConnectionString
		{
			get;
			set;
		}

		/// <summary>
		/// Show whether a transaction is currently in process.
		/// </summary>
		bool TransactionInProcess
		{
			get;
		}

		/// <summary>
		/// close a connection to the database and release it's resources
		/// </summary>
		void Close();

		/// <summary>
		/// Begin a SQL Transaction on the current connection
		/// </summary>
		void BeginTrans();

		/// <summary>
		/// Commit the current SQL Transaction
		/// </summary>
		void CommitTrans();

		/// <summary>
		/// Begin a SQL Transaction on the current connection
		/// </summary>
		void RollbackTrans();
	}
}
