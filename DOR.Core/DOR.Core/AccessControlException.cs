using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core
{
	public class AccessControlException : Exception
	{
		public AccessControlException()
		: base("Unauthorized")
		{
		}

		public AccessControlException(int screenNumber)
		: base("The current user does not have access to function " + screenNumber)
		{
		}
	}
}
