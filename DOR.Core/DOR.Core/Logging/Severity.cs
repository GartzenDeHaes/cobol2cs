using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core.Logging
{
	/// <summary>
	/// Severity of the event.
	/// </summary>
	public enum Severity
	{
		/// <summary>System is unusable</summary>
		Emergency = 0,
		/// <summary>Action must be taken immediately</summary>
		Alert = 1,
		/// <summary>Critical condition</summary>
		Critical = 2,
		/// <summary>Error condition</summary>
		Error = 3,
		/// <summary>Warning</summary>
		Warning = 4,
		/// <summary>normal but significant condition</summary>
		Notice = 5,
		/// <summary>General information</summary>
		Info = 6,
		/// <summary>debugging</summary>
		Debug = 7
	}

	public class DorSeverity
	{
		public static string SeverityLit(Severity s)
		{
			switch (s)
			{
				case Severity.Emergency:
					return "Emergency";
				case Severity.Alert:
					return "Alert";
				case Severity.Critical:
					return "Critical";
				case Severity.Error:
					return "Error";
				case Severity.Warning:
					return "Warning";
				case Severity.Notice:
					return "Notice";
				case Severity.Info:
					return "Info";
				case Severity.Debug:
					return "Debug";
				default:
					return "Unknown";
			}
		}
	}
}
