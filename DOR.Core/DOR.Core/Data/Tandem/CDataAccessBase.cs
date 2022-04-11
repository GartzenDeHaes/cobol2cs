using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;

using DOR.Core;
using DOR.Core.Collections;
using DOR.Core.Config;
using DOR.Core.Data;
using DOR.Core.Net;

namespace DOR.Core.Data.Tandem
{
	/// <summary>
	/// DA base for use with sqlmp2c generated server
	/// </summary>
	public class CDataAccessBase : IDataAccess, IDisposable
	{
		private static Random _rnd = new Random();
		private static Vector<CServerInfo> _servers = new Vector<CServerInfo>();

		private SQLSA _sqlsa;
		public SQLSA SQLSA
		{
			get { return _sqlsa; }
		}

		public string BrokerURL
		{
			get;
			private set;
		}

		private string UserId
		{
			get;
			set;
		}

		private string Password
		{
			get;
			set;
		}

		public string System
		{
			get;
			private set;
		}

		private long TransactionId
		{
			get;
			set;
		}

		/// <summary>
		/// Show whether a transaction is currently in process.
		/// </summary>
		public bool TransactionInProcess
		{
			get { return TransactionId != 0; }
		}

		/// <summary>
		/// Set to true to load performance stats
		/// </summary>
		public bool LoadSQLSA
		{
			get;
			set;
		}

		/// <summary>
		/// Number of times to retry if there's a connection error to the server.
		/// </summary>
		public int MaxRetries
		{
			get;
			set;
		}

		public CDataAccessBase()
		: this("TANDEM_CONNECT")
		{
		}

		public CDataAccessBase(string server, int port, string userId, string password)
		{
			AddServer(server, port, Configuration.AppConfig.DorEnvironment.EnvironmentType);
			UserId = userId;
			Password = password;
			MaxRetries = 3;
		}

		public CDataAccessBase(string connectionStringName)
		{
			MaxRetries = 3;
			string connectStr = "";

			if (Configuration.AppConfig.HasKey(connectionStringName))
			{
				connectStr = Configuration.AppConfig.StringAt(connectionStringName);
			}
			else if (Configuration.AppConfig.HasKey(connectionStringName + "." + Configuration.AppConfig.GetEnvironmentName()))
			{
				connectStr = Configuration.AppConfig.StringAt(connectionStringName + "." + Configuration.AppConfig.GetEnvironmentName());
			}
			else
			{
				throw new ConfigurationException("AppSettings key for connection string " + connectionStringName + " not found");
			}

			int port = 0;
			string serverIP = null;
			string[] parts = connectStr.Split(new char[] { ';' });

			for (int x = 0; x < parts.Length; x++)
			{
				if (String.IsNullOrEmpty(parts[x]))
				{
					continue;
				}

				int pos = parts[x].IndexOf('=');
				string key = parts[x].Substring(0, pos).ToUpper();
				string val = parts[x].Substring(pos + 1);

				if (key == "USER ID")
				{
					UserId = val;
				}
				else if (key == "PASSWORD")
				{
					Password = val;
				}
				else if (key == "SERVER")
				{
					serverIP = val;
				}
				else if (key == "PORT")
				{
					port = Int32.Parse(val);
				}
				else if (key == "SYSTEM" || key == "DATABASE")
				{
					System = val;
				}
				else if (key == "BROKER")
				{
					BrokerURL = val;
				}
				else if (key == "SQLSA")
				{
					if (val == "Y" || val == "y" || val == "TRUE" || val == "true")
					{
						LoadSQLSA = true;
					}
				}
				else
				{
					throw new ConfigurationException("unknown connection string key " + key);
				}
			}

			if (String.IsNullOrWhiteSpace(BrokerURL) && String.IsNullOrWhiteSpace(serverIP))
			{
				throw new ConfigurationException("BROKER or SERVER is required in connect string");
			}

			if (String.IsNullOrWhiteSpace(serverIP))
			{
				UpdateServerList();
			}
			else
			{
				AddServer(serverIP, port, Configuration.AppConfig.DorEnvironment.EnvironmentType);
			}
		}

		public CDataAccessBase(string server, int port)
		{
			AddServer(server, port, Configuration.AppConfig.DorEnvironment.EnvironmentType);

			System = "ZZ";
			UserId = "";
			Password = "";
			MaxRetries = 3;
		}

		private void AddServer(string ip, int port, EnvironmentType env)
		{
			lock (_servers)
			{
				//bool exists = (from s in _servers where s.IP == ip && s.Port == port select s).Any();
				//if (!exists)
				//{
					_servers.Add(new CServerInfo(ip, port, env));
				//}
			}
		}

		public void UpdateServerList()
		{
			lock (_servers)
			{
				_servers.Clear();

				using (WebClient wc = new WebClient())
				{
					using (StreamReader reader = new StreamReader(wc.OpenRead(BrokerURL + "SqlsInterface.aspx?cmd=servers")))
					{
						var chcomma = new char[] { ',' };
						string line;

						while (!String.IsNullOrWhiteSpace(line = reader.ReadLine()))
						{
							if (line == "OK")
							{
								continue;
							}
							if (line.StartsWith("ERROR"))
							{
								throw new Exception(line);
							}

							string[] parts = line.Split(chcomma);

							_servers.Add
							(
								new CServerInfo
								(
									parts[0],
									Int32.Parse(parts[1]),
									DorEnvironment.Parse(parts[2]).EnvironmentType
								)
							);
						}
					}
				}
			}
		}

		protected virtual string PreprocessXml(string xml)
		{
			if (xml.IndexOf("&#1") > -1)
			{
				return xml.Replace("&#13;", "\n").Replace("&#10;", "\r");
			}
			return xml;
		}

		#region IDataAccess

		public string ConnectionString
		{
			get 
			{
				lock (_servers)
				{
					if (_servers.Count == 0)
					{
						return String.Empty;
					}
					return "server=" + _servers[0].IP + ";port=" + _servers[0].Port + ";User ID=" + UserId + ";Password=" + Password;
				}
			}

			set { throw new NotImplementedException(); }
		}

		private Packet PreparePacket(string cmdName)
		{
			Packet pkt = new Packet();
			pkt.Append(cmdName);
			pkt.AppendPair(UserId, Password);
			pkt.AppendPair("%%system%%", System);
			pkt.AppendPair("%%call%%", "Y");
			return pkt;
		}

		private TandemDataReader Call(Packet pkt, DateTime? expiresOn = null)
		{
			return new TandemDataReader(_Call(pkt, expiresOn));
		}

		private XmlDocument _Call(Packet pkt, DateTime? expiresOn = null)
		{
			string parameterKey = null;
			if (expiresOn.HasValue)
			{
				parameterKey = pkt.GetLongHashCode().ToString();
				XmlDocument cached = FileCache.Get(parameterKey);
				if (cached != null)
				{
					return cached;
				}
			}

			string xml = null;

			for (int retries = 0; retries < MaxRetries; retries++)
			{
				try
				{
					CServerInfo sinfo;

					lock (_servers)
					{
						if (_servers.Count == 0)
						{
							throw new Exception("No CSQL servers online");
						}

						sinfo = _servers[_rnd.Next() % _servers.Count];
					}

					using (TcpClient client = new TcpClient(sinfo.IP, sinfo.Port))
					{
						if (TransactionId != 0)
						{
							pkt.Append("%%TRANSACTION-ID%%");
							pkt.Append(TransactionId);
						}

						pkt.Send(client.GetStream());

						using (StreamReader reader = new StreamReader(client.GetStream()))
						{
							xml = PreprocessXml(reader.ReadToEnd());
						}
						client.Close();
					}

					break;
				}
				catch (SocketException se)
				{
					if (retries < MaxRetries - 1)
					{
						UpdateServerList();
					}
					else
					{
						throw new Exception("Error connecting to CSQL server", se);
					}
				}
			}

			XmlDocument doc = TandemDataAccessBase.XMLIsErrorFree(xml, "CDataAccessBase");

			if (LoadSQLSA)
			{
				_sqlsa = SQLSA.Parse(doc);
			}

			if (expiresOn.HasValue)
			{
				FileCache.Put(parameterKey, doc, expiresOn.Value);
			}

			return doc;
		}

		private IDataReaderEx Call
		(
			string cmdName,
			ICParameter[] prms,
			DateTime? expiresOn = null
		)
		{
			Packet pkt = PreparePacket(cmdName);

			if (prms != null)
			{
				for (int x = 0; x < prms.Length; x++)
				{
					prms[x].Set(pkt);
				}
			}

			return Call(pkt, expiresOn);
		}

		public IDataReaderEx ExecuteReader
		(
			string cmdName,
			ICParameter[] prms,
			DateTime? expiresOn = null
		)
		{
			return Call(cmdName, prms, expiresOn);
		}

		public IDataReaderEx ExecuteReader
		(
			string sql
		)
		{
			return Call
			(
				"SQL", 
				new ICParameter[] 
				{ 
					new CParameter<string>("system", "ZZ"),
					new CParameter<string>("sql", sql)
				}
			);
		}

		public int ExecuteNonQuery
		(
			string cmdName,
			ICParameter[] prms,
			DateTime? expiresOn = null
		)
		{
			IDataReaderEx reader = Call(cmdName, prms, expiresOn);

			if (SQLSA.num_tables == 0)
			{
				throw new DataAccessException("SQLSA missing from return");
			}
			return SQLSA.stats[0].records_used;
		}

		public int ExecuteNonQuery
		(
			string sql
		)
		{
			IDataReaderEx reader = ExecuteReader(sql);

			if (SQLSA.num_tables == 0)
			{
				throw new DataAccessException("SQLSA missing from return");
			}
			return SQLSA.stats[0].records_used;
		}

		/// <summary>
		/// close a connection to the database and release it's resources
		/// </summary>
		public void Close()
		{
			if (TransactionId != 0)
			{
				RollbackTrans();
			}
		}

		/// <summary>
		/// Begin a SQL Transaction on the current connection
		/// </summary>
		public void BeginTrans()
		{
			if (TransactionId != 0)
			{
				throw new DataAccessException("nested transactions not supported");
			}

			Packet pkt = PreparePacket("SQL");
			pkt.AppendPair("sql", "BEGIN TRANSACTION");

			using (TandemDataReader reader = Call(pkt))
			{
				if (!reader.Read())
				{
					throw new DataAccessException("Internal error, can't read result of BEGIN TRANSACTION");
				}

				if (reader.GetString("message") != "OK")
				{
					throw new DataAccessException("BEGIN TRANS: " + reader.GetString("message"));
				}

				TransactionId = reader.GetInt64("transid");

				if (TransactionId < 1)
				{
					throw new DataAccessException("Internal error, invalid transaction ID");
				}
			}
		}

		/// <summary>
		/// Commit the current SQL Transaction
		/// </summary>
		public void CommitTrans()
		{
			if (TransactionId == 0)
			{
				throw new DataAccessException("transaction is not in progress");
			}

			Packet pkt = PreparePacket("SQL");
			pkt.AppendPair("sql", "COMMIT TRANSACTION");

			using (TandemDataReader reader = Call(pkt))
			{
				if (!reader.Read())
				{
					throw new DataAccessException("Internal error, can't read result of COMMIT TRANSACTION");
				}

				TransactionId = 0;

				if (reader.GetString(0) != "OK")
				{
					throw new DataAccessException("COMMIT failed " + reader.GetString(0));
				}
			}
		}

		/// <summary>
		/// Begin a SQL Transaction on the current connection
		/// </summary>
		public void RollbackTrans()
		{
			if (TransactionId == 0)
			{
				throw new DataAccessException("transaction is not in progress");
			}

			Packet pkt = PreparePacket("SQL");
			pkt.AppendPair("sql", "ABORT TRANSACTION");

			using (TandemDataReader reader = Call(pkt))
			{
				if (!reader.Read())
				{
					throw new DataAccessException("Internal error, can't read result of ABORT TRANSACTION");
				}

				TransactionId = 0;

				if (reader.GetString(0) != "OK")
				{
					throw new DataAccessException("ABORT failed " + reader.GetString(0));
				}
			}
		}

		#endregion

		#region server info

		public IDataReaderEx GetServerStats()
		{
			Packet pkt = PreparePacket("ADMIN");
			pkt.AppendPair("COMMAND", "stats");
			return Call(pkt);
		}

		public List<Define> ListTables()
		{
			Packet pkt = PreparePacket("ADMIN");
			List<Define> lst = new List<Define>();

			pkt.AppendPair("COMMAND", "info_defines");

			using (TandemDataReader reader = Call(pkt, DateTime.Now.AddHours(1)))
			{
				while (reader.Read())
				{
					lst.Add
					(
						new Define
						(
							reader.GetString("define"),
							reader.GetString("file")
						)
					);
				}
			}

			return lst;
		}

		public List<string> ListSystems()
		{
			Packet pkt = PreparePacket("ADMIN");
			List<string> lst = new List<string>();

			pkt.AppendPair("COMMAND", "systemslist");

			using (TandemDataReader reader = Call(pkt))
			{
				while (reader.Read())
				{
					lst.Add(reader.GetString("system"));
				}
			}

			return lst;
		}

		public List<string> ListProceduresForSystem(string sys)
		{
			Packet pkt = PreparePacket("ADMIN");
			List<string> lst = new List<string>();

			pkt.AppendPair("COMMAND", "proclist");

			using (TandemDataReader reader = Call(pkt))
			{
				while (reader.Read())
				{
					if (reader.GetString("system") == sys)
					{
						lst.Add(reader.GetString("procedure"));
					}
				}
			}

			return lst;
		}

		public List<string> ListUsers()
		{
			Packet pkt = PreparePacket("ADMIN");
			List<string> lst = new List<string>();

			pkt.AppendPair("COMMAND", "userlist");

			using (TandemDataReader reader = Call(pkt))
			{
				while (reader.Read())
				{
					lst.Add(reader.GetString("userid"));
				}
			}

			return lst;
		}

		public List<string> ListUserPerms(string userId)
		{
			Packet pkt = PreparePacket("ADMIN");
			List<string> lst = new List<string>();

			pkt.AppendPair("COMMAND", "userperms");
			pkt.AppendPair("userid", userId);

			using (TandemDataReader reader = Call(pkt))
			{
				while (reader.Read())
				{
					lst.Add(reader.GetString("perm"));
				}
			}

			return lst;
		}

		public string GetProcedureText
		(
			string system, 
			string procName
		)
		{
			Packet pkt = PreparePacket("ADMIN");
			List<string> lst = new List<string>();

			pkt.AppendPair("COMMAND", "displaysql");
			pkt.AppendPair("system", system);
			pkt.AppendPair("procedure", procName);

			using (TandemDataReader reader = Call(pkt))
			{
				if (reader.Read())
				{
					return reader.GetString("sql");
				}
			}

			throw new DataAccessException("Unknown error");
		}

		public IDataReaderEx Invoke(string define)
		{
			Packet pkt = PreparePacket("ADMIN");
			List<Define> lst = new List<Define>();

			pkt.AppendPair("COMMAND", "invoke");
			pkt.AppendPair("define", define);

			return Call(pkt);
		}

		#endregion

		#region obsolete

		public void Shutdown()
		{
			Packet pkt = PreparePacket("ADMIN");

			pkt.AppendPair("COMMAND", "shutdown");

			using (TandemDataReader reader = Call(pkt))
			{
				if (!reader.Read())
				{
					throw new DataAccessException("Internal error, can't read result of shutdown");
				}

				if (reader.GetString(0) != "shuting down")
				{
					throw new DataAccessException(reader.GetString(0));
				}
			}
		}

		public void CreateUser(string userId, string pw)
		{
			if (TransactionId != 0)
			{
				throw new DataAccessException("CREATE USER not valid with active transaction");
			}

			Packet pkt = PreparePacket("SQL");
			pkt.AppendPair("system", "ADMIN");
			pkt.AppendPair("sql", "CREATE LOGON " + userId + " PASSWORD '" + pw + "';");

			using (TandemDataReader reader = Call(pkt))
			{
				if (!reader.Read())
				{
					throw new DataAccessException("Internal error, can't read result of CREATE USER");
				}

				if (reader.GetString(0) != "OK")
				{
					throw new DataAccessException("CREATE USER ERROR: " + reader.GetString(0));
				}
			}
		}

		public void GrantExecute
		(
			string system,
			string userId, 
			string procedureName
		)
		{
			if (TransactionId != 0)
			{
				throw new DataAccessException("GRANT EXEC not valid with active transaction");
			}

			Packet pkt = PreparePacket("SQL");
			pkt.AppendPair("system", system);
			pkt.AppendPair("sql", "GRANT EXECUTE ON " + procedureName + " TO " + userId + ";");

			using (TandemDataReader reader = Call(pkt))
			{
				if (!reader.Read())
				{
					throw new DataAccessException("Internal error, can't read result of GRANT");
				}

				if (reader.GetString(0) != "OK")
				{
					throw new DataAccessException("GRANT EXEC ERROR: " + reader.GetString(0));
				}
			}
		}

		public void CreateProcedure
		(
			string system,
			string sql
		)
		{
			if (TransactionId != 0)
			{
				throw new DataAccessException("CREATE PROCEDURE not valid with active transaction");
			}

			Packet pkt = PreparePacket("SQL");
			pkt.AppendPair("system", system);
			pkt.Append("sql");
			pkt.Append(sql);

			using (TandemDataReader reader = Call(pkt))
			{
				if (!reader.Read())
				{
					throw new DataAccessException("Internal error, can't read result of CREATE");
				}

				if (reader.GetString(0) != "OK")
				{
					throw new DataAccessException("CREATE PROCEDURE ERROR: " + reader.GetString(0));
				}
			}
		}

#endregion

		public virtual void Dispose()
		{
		}
	}
}
