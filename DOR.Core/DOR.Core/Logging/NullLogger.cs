using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core.Logging
{
	public class NullLogger : ILogger
	{
		public StringBuilder Messages
		{
			get;
			private set;
		}

		public void Write
		(
			Facility f,
			Severity s,
			SystemPID pid,
			string msg
		)
		{
			Messages.Append(msg);
			Messages.Append("\r\n");
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
			Messages.Append(msg);
			Messages.Append("\r\n");
		}

		public void Write(SystemPID pid, Exception ex)
		{
			Messages.Append(ex.Message);
			Messages.Append("\r\n");
		}

		public NullLogger()
		{
			Messages = new StringBuilder();
		}

		public void Dispose()
		{
		}
	}
}
