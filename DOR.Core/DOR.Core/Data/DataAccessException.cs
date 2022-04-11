using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core.Data
{
	/// <summary>
	/// Thrown when RETURN_VALUE is non-zero.
	/// </summary>
	public class DataAccessException : Exception
	{
		public DataAccessException(string msg)
		: base(msg)
		{
		}
	}
}
