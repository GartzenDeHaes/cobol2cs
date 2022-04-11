using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core.Net.Command
{
	public class ProtocolException : Exception
	{
		public ProtocolException(string msg)
		: base(msg)
		{
		}
	}
}
