using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core
{
	public class InvalidArgumentException : Exception
	{
		public InvalidArgumentException(string msg)
		: base(msg)
		{
		}
	}
}
