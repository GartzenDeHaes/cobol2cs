using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;

using DOR.Core.Data.Tandem;

namespace DOR.Core.Data.SqlClient
{
	/// <summary>
	/// Dummy data interface.
	/// </summary>
	public class NullDataAccessBase : IDataAccess, IDisposable
	{
		public string ConnectionString
		{
			get;
			set;
		}

		/// <summary>
		/// Show whether a transaction is currently in process.
		/// </summary>
		public bool TransactionInProcess
		{
			get { return false; }
		}

		/// <summary>
		/// close a connection to the database and release it's resources
		/// </summary>
		public void Close()
		{
		}

		/// <summary>
		/// Begin a SQL Transaction on the current connection
		/// </summary>
		public void BeginTrans()
		{
		}

		/// <summary>
		/// Commit the current SQL Transaction
		/// </summary>
		public void CommitTrans()
		{
		}

		/// <summary>
		/// Begin a SQL Transaction on the current connection
		/// </summary>
		public void RollbackTrans()
		{
		}

		public IDbCommand CreateCommand(string cmdText)
		{
			return null;
		}

		public IDataReader ExecuteReader(string cmdText, DateTime? expiresOn = null)
		{
			return new TandemDataReader(new XmlDocument());
		}

		#region IDisposable Members

		// Dispose(bool disposing) executes in two distinct scenarios.
		// If disposing equals true, the method has been called directly
		// or indirectly by a user's code. Managed and unmanaged resources
		// can be disposed.
		// If disposing equals false, the method has been called by the
		// runtime from inside the finalizer and you should not reference
		// other objects. Only unmanaged resources can be disposed.
		private void Dispose(bool disposing)
		{
		}

		// This object will be cleaned up by the Dispose method.
		// Therefore, you should call GC.SupressFinalize to
		// take this object off the finalization queue
		// and prevent finalization code for this object
		// from executing a second time.
		public void Dispose()
		{
			Dispose(true);
		}

		#endregion
	}
}
