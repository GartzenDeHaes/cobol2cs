using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core.Logging
{
	/// <summary>
	/// This is the purpose or source of the event.  
	/// </summary>
	public enum Facility
	{
		/// <summary>kernel messages</summary>
		Kernal = 0,
		/// <summary>user-level messages</summary>
		User = 1,
		/// <summary>mail system</summary>
		Mail = 2,
		/// <summary>system daemons</summary>
		Daemon = 3,
		/// <summary>security/authentication messages</summary>
		Authentication = 4,
		/// <summary>messages generated internally by syslogd</summary>
		Syslog = 5,
		/// <summary>line printer subsystem</summary>
		Printer = 6,
		/// <summary>network news subsystem</summary>
		News = 7,
		/// <summary>UUCP subsystem</summary>
		UUCP = 8,
		/// <summary>clock daemon</summary>
		Cron1 = 9,
		/// <summary>security/authorization messages</summary>
		Authorization = 10,
		/// <summary>FTP daemon</summary>
		FTP = 11,
		/// <summary>NTP subsystem</summary>
		NTP = 12,
		/// <summary>log audit</summary>
		LogAudit = 13,
		/// <summary>log alert</summary>
		LogAlert = 14,
		/// <summary>clock daemon</summary>
		Cron2 = 15,
		/// <summary></summary>
		Local0 = 16,
		/// <summary></summary>
		Local1 = 17,
		/// <summary></summary>
		Local2 = 18,
		/// <summary></summary>
		Local3 = 19,
		/// <summary></summary>
		Local4 = 20,
		/// <summary></summary>
		Local5 = 21,
		/// <summary></summary>
		Local6 = 22,
		/// <summary></summary>
		Local7 = 23
	}
}
