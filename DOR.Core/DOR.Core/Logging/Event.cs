using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;

using DOR.Core;

namespace DOR.Core.Logging
{
	/// <summary>
	/// A syslog entry.
	/// <see cref="http://tools.ietf.org/search/rfc5424"/>
	/// </summary>
	/// <remarks>
	/// DOR extentions to the syslog format:
	/// 1.  Process name will consist of user-logon@process-name.exe
	/// 2.  PID will be a system identifier code, such as 1 for SA, 2 for WFTC, etc.  These should match the codes in SystemType table.
	/// </remarks>
	public class Event
	{
		public Facility Facility 
		{ 
			get; 
			set; 
		}

		public Severity Severity
		{
			get; 
			set; 
		}

		public DateTime OccuredOn
		{
			get; 
			set; 
		}

		public string Host
		{
			get; 
			set; 
		}

		public string Process
		{
			get; 
			set; 
		}

		public string UserLogon
		{
			get;
			set;
		}

		public SystemPID PID
		{
			get;
			set;
		}

		public string Message
		{
			get; 
			set; 
		}

		public int Priority
		{
			get { return (int)Facility * 8 + (int)Severity; }
		}

		private void _Init()
		{
			OccuredOn = DateTime.UtcNow;
			Host = Environment.MachineName;
			Process = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
			UserLogon = "" + WindowsIdentity.GetCurrent().Name;
		}

		public Event()
		{
			_Init();

			Facility = Facility.Local7;
			Severity = Severity.Debug;
			PID = SystemPID.Unknown;
			Message = "Unknown event";
		}

		public Event
		(
			Facility f,
			Severity s,
			SystemPID pid,
			string msg
		)
		{
			_Init();

			Facility = f;
			Severity = s;
			PID = pid;
			Message = msg;
		}

		public Event
		(
			Facility f,
			Severity s,
			DateTime dtm,
			string host,
			string proc,
			string logon,
			SystemPID pid, 
			string msg
		)
		{
			Facility = f;
			Severity = s;
			OccuredOn = dtm;
			Host = host;
			Process = proc;
			UserLogon = logon;
			PID = pid;
			Message = msg;
		}

		public override string ToString()
		{
			StringBuilder buf = new StringBuilder(30 + Host.Length + Process.Length + Message.Length);

			buf.Append('<');
			buf.Append(((int)Priority).ToString());
			buf.Append('>');
			buf.Append(OccuredOn.ToString("s", System.Globalization.CultureInfo.InvariantCulture ));
			buf.Append(' ');
			buf.Append(Host);
			buf.Append(' ');
			if (string.IsNullOrWhiteSpace(UserLogon) || UserLogon.IndexOf(' ') < 0)
			{
				SimpleFileLogger.WriteS(SystemPID.Unknown, "Expected space in UserLogon '" + UserLogon + "'");
			}
			buf.Append(UserLogon);
			buf.Append('@');
			buf.Append(Process);
			if (PID > 0)
			{
				buf.Append('[');
				buf.Append(((int)PID).ToString());
				buf.Append(']');
			}
			buf.Append(": ");
			buf.Append(Message);
	
			if (buf.Length < 1025)
			{
				return buf.ToString();
			}
			return buf.ToString().Substring(0, 1024);
		}

		public string ToShortString()
		{
			StringBuilder buf = new StringBuilder(20 + Message.Length);
			buf.Append(OccuredOn.ToShortTimeString());
			buf.Append(' ');
	
			switch(Severity)
			{
				case Severity.Emergency:
					buf.Append("PANIC ");
					break;
				case Severity.Alert:
					buf.Append("ALERT ");
					break;
				case Severity.Critical:
					buf.Append("CRITL ");
					break;
				case Severity.Error:
					buf.Append("ERROR ");
					break;
				case Severity.Warning:
					buf.Append("WARNG ");
					break;
				case Severity.Notice:
					buf.Append("NOTIC ");
					break;
				case Severity.Info:
					buf.Append("INFO: ");
					break;
				case Severity.Debug:
					buf.Append("DEBUG ");
					break;
				default:
					buf.Append("????? ");
					break;
			}
	
			buf.Append(Message);
	
			if (buf.Length < 1025)
			{
				return buf.ToString();
			}
	
			return buf.ToString().Substring(0, 1024);
		}

		public static Event Parse(string line)
		{
			if (line[0] != '<')
			{
				throw new FormatException("syslog event lines begin with '<'");
			}

			int pos = 1;
			int end = line.IndexOf('>', pos);
			if (0 > end)
			{
				throw new FormatException("Priority must be enclosed by '<' and '>'");
			}
			string str = StringHelper.MidStr(line, pos, end);
			if (! StringHelper.IsInt(str))
			{
				throw new FormatException("Priority must be a number; found '" + str + "'");
			}
			int priority = Int32.Parse(str);

			pos = end + 1;
			end = line.IndexOf(' ', pos);
			str = StringHelper.MidStr(line, pos, end);
			DateTime dtm;
			if (!DateTime.TryParse(str, out dtm))
			{
				throw new FormatException("Invalid DateTime '" + str + "'");
			}

			pos = end + 1;
			end = line.IndexOf(' ', pos);
			string host = StringHelper.MidStr(line, pos, end);

			pos = end + 1;
			end = line.IndexOf(' ', pos);
			str = StringHelper.MidStr(line, pos, end);
			int pid = -1;
			string process;
			if (str.IndexOf('[') > -1)
			{
				int bpos = str.IndexOf('[');
				process = str.Substring(0, bpos);
				string spid = str.Substring(bpos + 1, str.IndexOf(']'));
				if (!Int32.TryParse(spid, out pid))
				{
					throw new FormatException("PID must be numeric, found '" + spid + "'");
				}
			}
			else
			{
				process = str;
			}

			pos = end + 1;
			end = line.IndexOf(':', pos);
			string msg = StringHelper.MidStr(line, pos, end).Trim();

			string userLogon = "";
			if ((pos = process.IndexOf('@')) > -1)
			{
				userLogon = process.Substring(pos + 1);
				process = process.Substring(0, pos);
			}

			return new Event((Facility)(priority / 8), (Severity)(priority & 0xF), dtm, host, process, userLogon, (SystemPID)pid, msg);
		}
	}
}
