using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core
{
	/// <summary>
	/// Impersonation by NtLogon has failed.
	/// </summary>
	public class ImpersonationException : Exception
	{
		public ImpersonationException()
		: base("Impersonation failed")
		{
		}
	}
}
