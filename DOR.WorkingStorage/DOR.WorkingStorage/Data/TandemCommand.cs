using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DOR.Core.Collections;
using DOR.WorkingStorage;

namespace DOR.Core.Data.Tandem
{
	public class TandemCommand : IDbCommand
	{
		private class TandemCmdPrm
		{
			private IBufferOffset _off;

			public string Name
			{
				get;
				private set;
			}

			public BufferType Type
			{
				get { return _off.Format.IsFloat ? BufferType.Decimal : (_off.Format.IsNumeric ? BufferType.Int : BufferType.String); }
			}

			public TandemCmdPrm(IBufferOffset off, string name)
			{
				_off = off;
				Name = name;
			}

			public override string ToString()
			{
				return _off.ToString();
			}
		}

		private string _sql;
		private ISqlDataAccess _conn;
		private Vector<TandemCmdPrm> _prms = new Vector<TandemCmdPrm>();

		// Summary:
		//     Gets or sets the text command to run against the data source.
		//
		// Returns:
		//     The text command to execute. The default value is an empty string ("").
		public string CommandText 
		{
			get { return _sql; }
			set { _sql = value; }
		}

		//
		// Summary:
		//     Gets or sets the wait time before terminating the attempt to execute a command
		//     and generating an error.
		//
		// Returns:
		//     The time (in seconds) to wait for the command to execute. The default value
		//     is 30 seconds.
		//
		// Exceptions:
		//   System.ArgumentException:
		//     The property value assigned is less than 0.
		public int CommandTimeout 
		{ 
			get; 
			set; 
		}

		//
		// Summary:
		//     Indicates or specifies how the System.Data.IDbCommand.CommandText property
		//     is interpreted.
		//
		// Returns:
		//     One of the System.Data.CommandType values. The default is Text.
		public CommandType CommandType
		{
			get { return System.Data.CommandType.Text; }
			set { }
		}

		//
		// Summary:
		//     Gets or sets the System.Data.IDbConnection used by this instance of the System.Data.IDbCommand.
		//
		// Returns:
		//     The connection to the data source.
		public IDbConnection Connection 
		{
			get { return null; }
			set { }
		}

		//
		// Summary:
		//     Gets the System.Data.IDataParameterCollection.
		//
		// Returns:
		//     The parameters of the SQL statement or stored procedure.
		public IDataParameterCollection Parameters 
		{
			get { return null; }
		}
		
		//
		// Summary:
		//     Gets or sets the transaction within which the Command object of a .NET Framework
		//     data provider executes.
		//
		// Returns:
		//     the Command object of a .NET Framework data provider executes. The default
		//     value is null.
		public IDbTransaction Transaction { get; set; }

		//
		// Summary:
		//     Gets or sets how command results are applied to the System.Data.DataRow when
		//     used by the System.Data.IDataAdapter.Update(System.Data.DataSet) method of
		//     a System.Data.Common.DbDataAdapter.
		//
		// Returns:
		//     One of the System.Data.UpdateRowSource values. The default is Both unless
		//     the command is automatically generated. Then the default is None.
		//
		// Exceptions:
		//   System.ArgumentException:
		//     The value entered was not one of the System.Data.UpdateRowSource values.
		public UpdateRowSource UpdatedRowSource { get; set; }

		public TandemCommand(string sql, ISqlDataAccess conn)
		{
			_sql = sql;
			_conn = conn;
		}

		// Summary:
		//     Attempts to cancels the execution of an System.Data.IDbCommand.
		public void Cancel()
		{
		}

		//
		// Summary:
		//     Creates a new instance of an System.Data.IDbDataParameter object.
		//
		// Returns:
		//     An IDbDataParameter object.
		public IDbDataParameter CreateParameter()
		{
			return null;
		}

		public void AddParameter(IBufferOffset off, string name)
		{
			_prms.Add(new TandemCmdPrm(off, name));
		}

		public IDataReader ExecuteReader()
		{
			return _conn.ExecuteReader(ToString());
		}

		public IDataReader ExecuteReader(DateTime? expiresOn)
		{
			return _conn.ExecuteReader(ToString());
		}

		//
		// Summary:
		//     Executes an SQL statement against the Connection object of a .NET Framework
		//     data provider, and returns the number of rows affected.
		//
		// Returns:
		//     The number of rows affected.
		//
		// Exceptions:
		//   System.InvalidOperationException:
		//     The connection does not exist.-or- The connection is not open.
		public int ExecuteNonQuery()
		{
			return _conn.ExecuteNonQuery(ToString());
		}

		//
		// Summary:
		//     Executes the System.Data.IDbCommand.CommandText against the System.Data.IDbCommand.Connection,
		//     and builds an System.Data.IDataReader using one of the System.Data.CommandBehavior
		//     values.
		//
		// Parameters:
		//   behavior:
		//     One of the System.Data.CommandBehavior values.
		//
		// Returns:
		//     An System.Data.IDataReader object.
		public IDataReader ExecuteReader(CommandBehavior behavior)
		{
			throw new NotImplementedException();
		}

		//
		// Summary:
		//     Executes the query, and returns the first column of the first row in the
		//     resultset returned by the query. Extra columns or rows are ignored.
		//
		// Returns:
		//     The first column of the first row in the resultset.
		public object ExecuteScalar()
		{
			IDataReader reader = ExecuteReader();
			if (reader.RecordsAffected == 0)
			{
				return null;
			}
			reader.Read();
			return reader[0];
		}

		//
		// Summary:
		//     Creates a prepared (or compiled) version of the command on the data source.
		//
		// Exceptions:
		//   System.InvalidOperationException:
		//     The System.Data.OleDb.OleDbCommand.Connection is not set.-or- The System.Data.OleDb.OleDbCommand.Connection
		//     is not System.Data.OleDb.OleDbConnection.Open().
		public void Prepare()
		{
		}

		public override string ToString()
		{
			string sql = _sql;

			for (int x = 0; x < _prms.Count; x++)
			{
				Debug.Assert(sql.IndexOf(_prms[x].Name) > -1);

				string value = _prms[x].ToString();

				if 
				(
					_prms[x].Type == BufferType.String ||
					(
						_prms[x].Type == BufferType.Decimal &&	// timestamp
						value.IndexOf(":") > -1 && 
						value.IndexOf("-") > -1
					)
				)
				{
					sql = sql.Replace("?[" + _prms[x].Name + "]", "\"" + value + "\"");
				}
				else
				{
					sql = sql.Replace("?[" + _prms[x].Name + "]", value);
				}
			}

			return sql;
		}

		public virtual void Dispose()
		{
			_conn = null;
			_prms.Clear();
		}
	}
}
