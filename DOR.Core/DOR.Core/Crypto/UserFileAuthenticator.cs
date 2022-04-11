using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using DOR.Core.Collections;
using DOR.Core.Config;

namespace DOR.Core.Crypto
{
	[Obsolete]
	public class UserFileAuthenticator
	{
		Dictionary<string, string> _uidpwIdx = new Dictionary<string, string>();

		public string System
		{
			get;
			private set;
		}

		public string FileName
		{
			get { return System + ".sud"; }
		}

		public bool AuthenticateUser(string userId, string password)
		{
			userId = userId.ToLowerInvariant();

			if (!_uidpwIdx.ContainsKey(userId))
			{
				return false;
			}

			return _uidpwIdx[userId].Equals(password);
		}

		public static UserFileAuthenticator Load(string system)
		{
			string pw;

			if (Configuration.AppConfig.ContainsKey("USERDB_PW." + Configuration.AppConfig.GetBlsEnvironmentName()))
			{
				pw = Configuration.AppConfig.StringAt("USERDB_PW." + Configuration.AppConfig.GetBlsEnvironmentName());
			}
			else if (Configuration.AppConfig.ContainsKey("USERDB_PW." + Configuration.AppConfig.GetEnvironmentName()))
			{
				pw = Configuration.AppConfig.StringAt("USERDB_PW." + Configuration.AppConfig.GetEnvironmentName());
			}
			else if (Configuration.AppConfig.ContainsKey("USERDB_PW"))
			{
				pw = Configuration.AppConfig.StringAt("USERDB_PW");
			}
			else
			{
				throw new ConfigurationException("Config file parameter USERDB_PW is required for auto configuration");
			}

			return Load(system, pw);
		}

		public static UserFileAuthenticator Load(string system, string password)
		{
			if (String.IsNullOrWhiteSpace(password))
			{
				throw new ArgumentException("password is required");
			}

			switch (password[0])
			{
				case '0':
					if 
					(
						Configuration.AppConfig.DorEnvironment.EnvironmentType != EnvironmentType.Dev && 
						Configuration.AppConfig.DorEnvironment.EnvironmentType != EnvironmentType.Test
					)
					{
						throw new ConfigurationException("Can't use DEV or TEST password in " + Configuration.AppConfig.GetEnvironmentName());
					}
					break;
				case '1':
					if 
					(
						Configuration.AppConfig.DorEnvironment.EnvironmentType != EnvironmentType.Dev && 
						Configuration.AppConfig.DorEnvironment.EnvironmentType != EnvironmentType.Test
					)
					{
						throw new ConfigurationException("Can't use DEV or TEST password in " + Configuration.AppConfig.GetEnvironmentName());
					}
					break;
				case '2':
					if (Configuration.AppConfig.DorEnvironment.EnvironmentType != EnvironmentType.Demo)
					{
						throw new ConfigurationException("Can't use DEMO password in " + Configuration.AppConfig.GetEnvironmentName());
					}
					break;
				case '3':
					if (Configuration.AppConfig.DorEnvironment.EnvironmentType != EnvironmentType.Prod)
					{
						throw new ConfigurationException("Can't use PROD password in " + Configuration.AppConfig.GetEnvironmentName());
					}
					break;
				default:
					throw new ArgumentException("Invalid password");
			}

			UserFileAuthenticator udb = new UserFileAuthenticator();
			udb.System = system;

			string filename = udb.FileName;
			string alltxt;

			if (Configuration.AppConfig.ContainsKey(filename))
			{
				alltxt = Configuration.AppConfig.StringAt(filename);
			}
			else
			{
				if (Configuration.AppConfig.ContainsKey("USERDB_DIR." + Configuration.AppConfig.GetBlsEnvironmentName()))
				{
					filename = Path.Combine(Configuration.AppConfig.StringAt("USERDB_DIR." + Configuration.AppConfig.GetBlsEnvironmentName()), filename);
				}
				else if (Configuration.AppConfig.ContainsKey("USERDB_DIR." + Configuration.AppConfig.GetEnvironmentName()))
				{
					filename = Path.Combine(Configuration.AppConfig.StringAt("USERDB_DIR." + Configuration.AppConfig.GetEnvironmentName()), filename);
				}
				else if (Configuration.AppConfig.ContainsKey("USERDB_DIR"))
				{
					filename = Path.Combine(Configuration.AppConfig.StringAt("USERDB_DIR"), filename);
				}

				if (! File.Exists(filename))
				{
					filename = "..\\" + udb.FileName;
					if (!File.Exists(filename))
					{
						filename = "..\\..\\" + udb.FileName;
						if (!File.Exists(filename))
						{
							throw new FileNotFoundException(udb.FileName + " not found");
						}
					}
				}

				alltxt = File.ReadAllText(filename);
			}

			const string sectdelim = "**!!--XXX\t";
			int pos = alltxt.IndexOf(sectdelim);

			string scrc = alltxt.Substring(0, pos);
			alltxt = alltxt.Substring(pos + sectdelim.Length);

			byte crc = Byte.Parse(scrc);
			byte ccrc = alltxt.Crc8();

			if (crc != ccrc)
			{
				throw new Exception("CRC-8 failed; file is corrupt");
			}

			int env = Int32.Parse(password[0].ToString());
			string[] sects = alltxt.Split(sectdelim);

			string sect;

			if (sects.Length == 1)
			{
				sect = sects[0];
			}
			else
			{
				sect = sects[env];
			}
			
			string txt = StringHelper.StripNulls(SimpleBlockCypher.DecryptString(sect, password));

			string[] lines = txt.Split("\r\n");

			switch (env)
			{
				case 0:
					if (lines[0] != "ENV=DEV" && lines[0] != "ENV=TEST")
					{
						throw new Exception("Expected DEV or TEST user database; found '" + lines[0] + "'");
					}
					break;
				case 1:
					if (lines[0] != "ENV=DEV" && lines[0] != "ENV=TEST")
					{
						throw new Exception("Expected TEST user database; found '" + lines[0] + "'");
					}
					break;
				case 2:
					if (lines[0] != "ENV=DEMO")
					{
						throw new Exception("Expected DEMO user database; found '" + lines[0] + "'");
					}
					break;
				case 3:
					if (lines[0] != "ENV=PROD")
					{
						throw new Exception("Expected PROD user database; found '" + lines[0] + "'");
					}
					break;
			}

			for (int x = 1; x < lines.Length; x++)
			{
				pos = lines[x].IndexOf('=');
				string uid = lines[x].Substring(0, pos);
				string pw = lines[x].Substring(pos + 1);

				udb._uidpwIdx.Add(uid.ToLowerInvariant(), pw);
			}

			return udb;
		}
	}
}
