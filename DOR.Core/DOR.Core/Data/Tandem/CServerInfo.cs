using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core.Config;

namespace DOR.Core.Data.Tandem
{
	public class CServerInfo
	{
		public string IP
		{
			get;
			set;
		}

		public int Port
		{
			get;
			set;
		}

		public EnvironmentType Env
		{
			get;
			set;
		}

		public CServerInfo(string ip, int port, EnvironmentType env)
		{
			IP = ip;
			Port = port;
			Env = env;
		}
	}
}
