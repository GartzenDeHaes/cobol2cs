using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;

using DOR.Core.Config;

namespace DOR.Core.Data.Tandem
{
	public class TandemDirectSql : TandemDataAccessBase, ISqlDataAccess
	{
		private StringBuilder _sqlBatch = new StringBuilder();

		public bool IncludeMetaData
		{
			get;
			set;
		}

		public TandemDirectSql()
		{
		}

		public TandemDirectSql(IConfiguration config)
		: base(config)
		{
		}
	
		public override void BeginTrans()
		{
			if (TransactionInProcess)
			{
				throw new InvalidOperationException("Transaction already in progress.");
			}
			TransactionInProcess = true;
			_sqlBatch.Clear();
		}

		public override void RollbackTrans()
		{
			TransactionInProcess = false;
			_sqlBatch.Clear();
		}

		public override void CommitTrans()
		{
			XmlDocument doc = CallPost
			(
				"et.SelSql",
				null,
				new TandemParameter[]
				{
					BuildParameter("action", "batch"),
					BuildParameter("sql", _sqlBatch.ToString())
				}
			);

			TransactionInProcess = false;
			_sqlBatch.Clear();
		}

		public IDbCommand CreateCommand(string cmdText)
		{
			return new TandemCommand(cmdText, this);
		}

		public IDataReader ExecuteReader(string sql, int? cacheDurationInMinutes = null)
		{
			TandemParameter[] prms;
			if (IncludeMetaData)
			{
				prms = new TandemParameter[]
				{
					BuildParameter("action", "inquire"),
					BuildParameter("returnMetaData", "true"),
					BuildParameter("sql", sql)
				};
			}
			else
			{
				prms = new TandemParameter[]
				{
					BuildParameter("action", "inquire"),
					BuildParameter("sql", sql)
				};
			}
			XmlDocument doc = CallPost
			(
				"et.SelSql",
				null,
				prms,
				cacheDurationInMinutes
			);

			return new TandemDataReader(doc);
		}

		public int ExecuteNonQuery(string sql)
		{
			if (TransactionInProcess)
			{
				if (_sqlBatch.Length > 0)
				{
					_sqlBatch.Append("; ");
				}
				_sqlBatch.Append(sql);
				
				return -1;
			}

			XmlDocument doc = CallPost
			(
				"et.SelSql",
				null,
				new TandemParameter[]
				{
					BuildParameter("action", "update"),
					BuildParameter("sql", sql)
				}
			);

			return Int32.Parse(doc.DocumentElement.FirstChild.Attributes["count"].InnerText);
		}
	}
}
