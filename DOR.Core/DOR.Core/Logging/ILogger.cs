using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core.Logging
{
	/// <summary>
	/// Common interface for logging systems.
	/// </summary>
	public interface ILogger : IDisposable
	{
		void Write
		(
			Facility f,
			Severity s,
			SystemPID pid,
			string msg
		);

		void Write
		(
			Facility f,
			Severity s,
			DateTime dtm,
			string host,
			string proc,
			string userLogonId,
			SystemPID pid, 
			string msg
		);

		void Write(SystemPID pid, Exception ex);
	}
}
