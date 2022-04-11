using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using DOR.Core.Config;
using DOR.Core.Net;

namespace DOR.Core.Logging
{
	public class SimpleFileLogger : ILogger
	{
		private static object _mylock = new object();

		private static string GetFilename(SystemPID pid)
		{
			if (Configuration.AppConfig.HasKey("LOG_FILE"))
			{
				return (string)Configuration.AppConfig["LOG_FILE"];
			}
			if (Configuration.AppConfig.HasKey("LOG_FILE." + Configuration.AppConfig.GetBlsEnvironmentName()))
			{
				return (string)Configuration.AppConfig["LOG_FILE." + Configuration.AppConfig.GetBlsEnvironmentName()];
			}
			else
			{
				return DorSystem.SystemName(pid) + "_log.txt";
			}
		}

		public void Write
		(
			Facility f,
			Severity s,
			SystemPID pid,
			string msg
		)
		{
			string filename = GetFilename(pid);
			msg = DateTime.Now.ToString()
				+ " "
				+ DorSystem.SystemName(pid)
				+ " "
				+ DorSeverity.SeverityLit(s)
				+ "\r\n"
				+ msg
				+ "\r\n*****************************************************\r\n";

			lock (_mylock)
			{
				File.AppendAllText(filename, msg);
			}
		}

		public void Write
		(
			Facility f,
			Severity s,
			DateTime dtm,
			string host,
			string proc,
			string userLogonId,
			SystemPID pid,
			string msg
		)
		{
			string filename = GetFilename(pid);
			msg = dtm.ToString()
				+ " "
				+ DorSystem.SystemName(pid)
				+ " "
				+ DorSeverity.SeverityLit(s)
				+ " " + (int)f + " " + host + " " + proc + " " + userLogonId + "\r\n"
				+ msg
				+ "\r\n*****************************************************\r\n";

			lock (_mylock)
			{
				File.AppendAllText(filename, msg);
			}
		}

		public void Write(SystemPID pid, Exception ex)
		{
			string filename = GetFilename(pid);
			string innerMsg = " ";
			while (ex != null)
			{
				string msg = DateTime.Now.ToString()
					+ " "
					+ DorSystem.SystemName(pid)
					+ innerMsg
					+ ex.Message
					+ "\r\n"
					+ ex.StackTrace
					+ "\r\n";

				lock (_mylock)
				{
					File.AppendAllText(filename, msg);
				}
				ex = ex.InnerException;
				innerMsg = " INNER EXCEPTION ";
			}

			lock (_mylock)
			{
				File.AppendAllText(filename, "\r\n*****************************************************\r\n");
			}
		}

		public static void WriteS(SystemPID pid, Exception ex)
		{
			SimpleFileLogger l = new SimpleFileLogger();
			l.Write(pid, ex);
		}

		public static void WriteS(SystemPID pid, Exception ex, List<EmailAddress> addr)
		{
			WriteS(pid, ex);

			try
			{
				Email email = new Email(DorSystem.SystemName(pid) + " " + Configuration.AppConfig.GetEnvironmentName(), ex.ToString(), EmailAddress.Parse("nobody@nowhere.com"), "System", false);

				for (int x = 0; x < addr.Count; x++)
				{
					email.Recipients.Add(addr[x].ToString());
					email.Send();
					email.Recipients.Clear();
				}
			}
			catch (Exception)
			{
			}
		}

		public static void WriteS(SystemPID pid, string ex)
		{
			string filename = GetFilename(pid);
			string msg = DateTime.Now.ToString()
				+ " "
				+ ex
				+ "\r\n*****************************************************\r\n";

			lock (_mylock)
			{
				File.AppendAllText(filename, msg);
			}

			if (! Configuration.AppConfig.ContainsKey("LOG_EMAIL"))
			{
				return;
			}
			try
			{
				string addr = Configuration.AppConfig.StringAt("LOG_EMAIL");
				if (String.IsNullOrWhiteSpace(addr))
				{
					return;
				}

				Email em = new Email(DorSystem.SystemName(pid) + " " + Configuration.AppConfig.DorEnvironment.ToString() + " " + Environment.MachineName, msg, EmailAddress.Parse("system@dor.wa.gov"), DorSystem.SystemName(pid), false);

				if (addr.IndexOf(',') > -1)
				{
					string[] addrs = addr.Split(new char[] {','});
					for (int x = 0; x < addrs.Length; x++)
					{
						em.Recipients.Add(addrs[x]);
					}
				}
				else
				{
					em.Recipients.Add(addr);
				}
				em.Send();
			}
			catch (Exception)
			{
				// ignore
			}
		}

		public virtual void Dispose()
		{
		}
	}
}
