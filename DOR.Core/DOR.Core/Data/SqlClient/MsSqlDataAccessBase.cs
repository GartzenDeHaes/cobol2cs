using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Xml;

using DOR.Core.Config;

namespace DOR.Core.Data.SqlClient
{
	/// <summary>
	/// Base class for a "flat" data access layer.
	/// </summary>
	/// <example>
	///		class MyAppDataAccess : MsSqlDataAccessBase, IDisposable
	///		{
	///			public void DeleteActivity(int activityId)
	///			{
	///				ExecuteNonQuery
	///				(
	///					"delActivity",
	///					new SqlParameter[] {ParameterBuilder.BuildParameter("@Activity_ID", activityId) }
	///				);
	///			}
	///		}
	///	
	///		...
	/// 
	///		using (var data = new MyAppDataAccess())
	///		{
	///			data.DeleteActivity(123);
	///		}
	/// </example>
	public abstract class MsSqlDataAccessBase : IDisposable, IDataAccess
	{
		#region Members

		private SqlConnection m_connection;
		private SqlTransaction m_transaction;
		private bool m_transactionInProcess;
		private string m_connectString;

		#endregion Members

		#region Properties

		public string ConnectionString
		{
			get { return m_connectString; }
			set { m_connectString = value; }
		}

		/// <summary>
		/// Show whether a transaction is currently in process.
		/// </summary>
		public bool TransactionInProcess
		{
			get { return m_transactionInProcess; }
		}

		private IConfiguration Config
		{
			get;
			set;
		}

		#endregion Properties

		#region Ctor's

		/// <summary>
		/// Default constructor - always opens a connection.  Uses either appSettings
		/// or connectStrings.  _TEST, _DEMO, or _PROD will be appended.
		/// </summary>
		public MsSqlDataAccessBase(IConfiguration config, string configFileConnectStringKey)
		{
			Config = config;
			Init(configFileConnectStringKey);
		}

		/// <summary>
		/// Default constructor - always opens a connection.  Uses the default
		/// config file key of CONNECTSTRING.
		/// </summary>
		public MsSqlDataAccessBase(IConfiguration config)
		{
			Config = config;
			Init("CONNECTSTRING");
		}

		/// <summary>
		/// Default constructor - always opens a connection.  Uses either appSettings
		/// or connectStrings.  _TEST, _DEMO, or _PROD will be appended.
		/// </summary>
		public MsSqlDataAccessBase(string configFileConnectStringKey)
		{
			Config = Configuration.AppConfig;
			Init(configFileConnectStringKey);
		}

		/// <summary>
		/// Default constructor - always opens a connection.  Uses the default
		/// config file key of CONNECTSTRING.
		/// </summary>
		public MsSqlDataAccessBase()
		{
			Config = Configuration.AppConfig;
			Init("CONNECTSTRING");
		}

		private void Init(string connectStringKey)
		{
			m_connectString = connectStringKey;

			if (! Config.ContainsKey(m_connectString))
			{
				if (Config.ContainsKey(m_connectString + "." + Config.GetBlsEnvironmentName()))
				{
					m_connectString += "." + Config.GetBlsEnvironmentName();
				}
				else if (Config.ContainsKey(m_connectString + "." + Config.GetEnvironmentName()))
				{
					m_connectString += "." + Config.GetEnvironmentName();
				}
				else if (Config.ContainsKey(m_connectString + "_" + Config.GetEnvironmentName()))
				{
					m_connectString += "_" + Config.GetEnvironmentName();
				}
				else
				{
					throw new DataAccessException("Connection string is missing in app settings for " + m_connectString);
				}
			}

			m_connectString = (string)Config[m_connectString];

			m_connection = new SqlConnection(m_connectString);
			m_connection.Open();
		}

		#endregion Ctor's

		#region Dtor

		/// <summary>
		/// Destructor to clean up connection if need be
		/// </summary>
		~MsSqlDataAccessBase()
		{
			Close();
			Dispose();
		}

		#endregion

		#region Connection Open/Close

		/// <summary>
		/// close a connection to the database and release it's resources
		/// </summary>
		public void Close()
		{
			try
			{
				// disposing a SQLConnection also performs a close
				if (m_connection != null)
				{
					if (m_transactionInProcess && m_transaction != null)
					{
						m_transaction.Dispose();
					}

					if (m_connection.State == ConnectionState.Open)
					{
						try
						{
							m_connection.Close();
						}
						catch (Exception)
						{
							// ignore.
						}
					}
				}
			}
			finally
			{
				m_transaction = null;
			}
		}

		#endregion Connection Open/Close

		#region Transaction Handlers

		/// <summary>
		/// Begin a SQL Transaction on the current connection
		/// </summary>
		public void BeginTrans()
		{
			if (m_transactionInProcess)
			{
				throw new DataAccessException("Transaction already in progress");
			}

			if (m_connection == null)
			{
				throw new DataAccessException("Transaction cannot be started.  Connection is closed.");
			}
			m_transaction = m_connection.BeginTransaction(IsolationLevel.Unspecified);
			m_transactionInProcess = true;
		}

		/// <summary>
		/// Commit the current SQL Transaction
		/// </summary>
		public void CommitTrans()
		{
			if (!m_transactionInProcess)
			{
				throw new DataAccessException("Cannot Commit - Transaction has not been started");
			}
			try
			{
				m_transaction.Commit();
			}
			finally
			{
				m_transaction.Dispose();
				m_transaction = null;
				m_transactionInProcess = false;
			}
		}

		/// <summary>
		/// Begin a SQL Transaction on the current connection
		/// </summary>
		public void RollbackTrans()
		{
			if (!m_transactionInProcess)
			{
				throw new DataAccessException("Cannot Rollback - Transaction has not been started");
			}

			try
			{
				m_transaction.Rollback();
			}
			finally
			{
				m_transaction.Dispose();
				m_transactionInProcess = false;
			}
		}

		#endregion Transaction Handlers

		#region Command Helpers

		/// <summary>
		/// Create a sql command with parameters
		/// </summary>
		/// <param name="storedProcName"></param>
		/// <param name="commandParameters"></param>
		/// <returns></returns>
		private SqlCommand CreateCommand
		(
			string storedProcName,
			params SqlParameter[] commandParameters
		)
		{
			SqlCommand cmd = (SqlCommand)CreateCommand(storedProcName);
			cmd.Parameters.AddRange(commandParameters);
			return cmd;
		}

		/// <summary>
		/// Create a SqlCommand without parameters
		/// </summary>
		/// <param name="storedProcName"></param>
		/// <returns></returns>
		private SqlCommand CreateCommand(string storedProcName)
		{
			if (m_connection == null)
			{
				throw new DataAccessException("Connection is closed.");
			}
			SqlCommand cmd = m_connection.CreateCommand();
			cmd.CommandText = storedProcName;
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandTimeout = 60;

			// we check the RETURN_VALUE on all of our DB calls.
			cmd.Parameters.Add("@RETURN_VALUE", SqlDbType.Int);
			cmd.Parameters["@RETURN_VALUE"].Direction = ParameterDirection.ReturnValue;
			if (m_transactionInProcess)
			{
				cmd.Transaction = m_transaction;
			}
			
			return cmd;
		}

		public DataSet ExecuteDataAdapter(string cmdText, int? cacheDurationInMinutes, params SqlParameter[] parameters)
		{
			if (cacheDurationInMinutes.HasValue && cacheDurationInMinutes > 0)
			{
				var paramKey = ParameterKey(cmdText, parameters);
				var cds = (DataSet)MemoryCache.Default.Get(paramKey);

				if (cds != null)
				{
					return cds;
				}
			}

			using (var cmd = CreateCommand(cmdText, parameters))
			{
				DataSet ds = new DataSet();
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				da.Fill(ds);

				CheckCommandReturnValue(cmd);

				if (cacheDurationInMinutes.HasValue && cacheDurationInMinutes > 0)
				{
					var paramKey = ParameterKey(cmdText, parameters);
					var policy = new CacheItemPolicy();
					policy.AbsoluteExpiration = DateTime.Now.AddMinutes((double)cacheDurationInMinutes);

					MemoryCache.Default.Add(paramKey, ds, policy);
				}

				return ds;
			}
		}

		private XmlDocument GetFromCache(string paramKey, int? cacheDurationInMinutes)
		{
			if (cacheDurationInMinutes.HasValue && cacheDurationInMinutes > 0)
			{
				return (XmlDocument)MemoryCache.Default.Get(paramKey);
			}

			return null;
		}

		public IDataReader PutInCache(IDataReader reader, string paramKey, int? cacheDurationInMinutes)
		{
			if (cacheDurationInMinutes.HasValue && cacheDurationInMinutes > 0)
			{
				var doc = Tandem.TandemDataReader.ConvertToCompatibleXml(reader);
				var policy = new CacheItemPolicy();
				policy.AbsoluteExpiration = DateTime.Now.AddMinutes((double)cacheDurationInMinutes);

				MemoryCache.Default.Add(paramKey, doc, policy);

				reader.Close();

				return new Tandem.TandemDataReader(doc);
			}

			return reader;
		}

		protected IDataReader ExecuteReader(string cmdText, int? cacheDurationInMinutes, params SqlParameter[] parameters)
		{
			var paramKey = ParameterKey(cmdText, parameters);
			var doc = GetFromCache(paramKey, cacheDurationInMinutes);
			if (doc != null)
			{
				return new Tandem.TandemDataReader(doc);
			}

			using (var cmd = CreateCommand(cmdText, parameters))
			{
				return PutInCache(cmd.ExecuteReader(), paramKey, cacheDurationInMinutes);
			}
		}

		protected IAsyncResult BeginExecuteReader(string cmdText, int? cacheDurationInMinutes, params SqlParameter[] parameters)
		{
			var paramKey = ParameterKey(cmdText, parameters);
			var doc = GetFromCache(paramKey, cacheDurationInMinutes);
			if (doc != null)
			{
				return new DummyAsyncResult(new MsAsyncContext(new Tandem.TandemDataReader((XmlDocument)doc)));
			}

			var cmd = CreateCommand(cmdText, parameters);
			var ctx = new MsAsyncContext(cmd, paramKey, cacheDurationInMinutes);

			return cmd.BeginExecuteReader(RespCallbackExecuteReader, ctx);
		}

		private void RespCallbackExecuteReader(IAsyncResult result)
		{
			var ctx = (MsAsyncContext)result.AsyncState;

			try
			{
				ctx.Reader = ctx.Command.EndExecuteReader(result);
				ctx.Command.Dispose();

				ctx.Reader = PutInCache(ctx.Reader, ctx.ParameterKey, ctx.CacheDurationInMinutes);
			}
			catch (Exception ex)
			{
				ctx.AsyncException = ex;
			}

			ctx.CompletedEvent.Set();
		}

		/// <summary>
		/// Execute NonQuery without parameters
		/// </summary>
		/// <param name="storedProcName"></param>
		/// <returns></returns>
		protected int ExecuteNonQuery(string storedProcName, params SqlParameter[] parameters)
		{
			using (SqlCommand cmd = (SqlCommand)CreateCommand(storedProcName, parameters))
			{
				int returnValue = cmd.ExecuteNonQuery();
				CheckCommandReturnValue(cmd);
				return returnValue;
			}
		}

		protected void ClearCache(string storedProcName, params SqlParameter[] parameters)
		{
			MemoryCache.Default.Remove(ParameterKey(storedProcName, parameters));
		}

		protected IAsyncResult BeginExecuteNonQuery(string storedProcName, params SqlParameter[] parameters)
		{
			var cmd = CreateCommand(storedProcName, parameters);
			var ctx = new MsAsyncContext(cmd, null, null);
			return cmd.BeginExecuteNonQuery(RespCallbackNonQuery, ctx);
		}

		private void RespCallbackNonQuery(IAsyncResult result)
		{
			var ctx = (MsAsyncContext)result.AsyncState;

			try
			{
				ctx.RecordCount = ctx.Command.EndExecuteNonQuery(result);
				CheckCommandReturnValue(ctx.Command);
				ctx.Command.Dispose();
			}
			catch (Exception ex)
			{
				ctx.AsyncException = ex;
			}

			ctx.CompletedEvent.Set();
		}

		/// <summary>
		/// Execute Scalar with parameters
		/// </summary>
		/// <param name="storedProcName"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		protected object ExecuteScalar
		(
			string storedProcName,
			params SqlParameter[] parameters
		)
		{
			using (SqlCommand cmd = CreateCommand(storedProcName, parameters))
			{
				object toReturn = cmd.ExecuteScalar();
				CheckCommandReturnValue(cmd);
				return toReturn;
			}
		}

		/// <summary>
		/// Execute Scalar without parameters
		/// </summary>
		/// <param name="storedProcName"></param>
		/// <returns></returns>
		private object ExecuteScalar(string storedProcName)
		{
			using (SqlCommand cmd = (SqlCommand)CreateCommand(storedProcName))
			{
				object toReturn = cmd.ExecuteScalar();
				CheckCommandReturnValue(cmd);
				return toReturn;
			}
		}

		protected MsAsyncContext EndAsync(IAsyncResult async)
		{
			var ctx = (MsAsyncContext)async.AsyncState;

			ctx.CompletedEvent.Wait();
			ctx.CompletedEvent.Dispose();
			ctx.RethrowAnyAsyncException();

			if (ctx.SubQueries != null)
			{
				foreach (var q in ctx.SubQueries)
				{
					EndAsync(q);
				}
			}

			return ctx;
		}

		#endregion Command Helpers

		private string ParameterKey
		(
			string cmdText,
			SqlParameter[] parameters
		)
		{
			StringBuilder buf = new StringBuilder();
			buf.Append(cmdText);
			buf.Append("&");

			if (parameters != null)
			{
				for (int x = 0; x < parameters.Length; x++)
				{
					buf.Append(parameters[x].ParameterName);
					buf.Append("=");
					buf.Append(parameters[x].Value);
					buf.Append("&");
				}
			}

			return buf.ToString();
		}

		protected DateTime? DateTimeHandleDbNull(object val)
		{
			if (val is DBNull)
			{
				return null;
			}

			if (val is DateTime)
			{
				return (DateTime)val;
			}

			if (null == val)
			{
				return null;
			}

			return DateTime.Parse(val.ToString());
		}

		protected Decimal? DecimalHandleDbNull(object val)
		{
			if (val is DBNull)
			{
				return null;
			}

			if (val is Decimal)
			{
				return (Decimal)val;
			}

			if (null == val)
			{
				return null;
			}

			return Decimal.Parse(val.ToString());
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
			if (disposing)
			{
				Close();

				try
				{
					if (m_connection != null)
					{
						m_connection.Dispose();
					}
				}
				catch (InvalidOperationException)
				{
					// this has been added to prevent the "InvalidOperationException" 
					// from going unhandeled the connection has been closed so 
					// nothing needs to be done.
				}
			}
			m_connection = null;
			m_transaction = null;
		}

		// This object will be cleaned up by the Dispose method.
		// Therefore, you should call GC.SupressFinalize to
		// take this object off the finalization queue
		// and prevent finalization code for this object
		// from executing a second time.
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		#region Protected Helper Methods

		protected static void CheckCommandReturnValue(SqlCommand cmd)
		{
			var returnValue = (Int32)cmd.Parameters["@RETURN_VALUE"].Value;
			if (returnValue != 0)
			{
				throw new DataAccessException
				(
					string.Format
					(
						CultureInfo.InvariantCulture,
						 @"Stored procedure '{0}' received the following return code: {1}",
						cmd.CommandText,
						returnValue
					)
				);
			}
		}

		protected static string XmlReaderToString(XmlReader reader)
		{
			StringBuilder buf = new StringBuilder(256);

			while (reader.Read())
			{
				if (reader.IsStartElement())
				{
					buf.Append(reader.ReadOuterXml().Trim());
					buf.Append(Environment.NewLine);
				}
			}

			reader.Close();

			return buf.ToString();
		}

		#endregion Protected Helper Methods
	}
}
