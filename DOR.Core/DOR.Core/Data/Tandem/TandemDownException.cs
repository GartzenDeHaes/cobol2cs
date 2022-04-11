using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core.Data.Tandem
{
	/// <summary> Exception object for when Tandem is down.</summary>
	public class TandemDownException : ApplicationException
	{
		/// <summary> Default constructor.</summary>
		public TandemDownException()
			: this("Tandem not responding")
		{
		}

		/// <summary> Constructor - Initializes the exception description.</summary>
		/// <param name="sMsg"> The exception message description.</param>
		public TandemDownException(string sMsg)
			: base(sMsg)
		{
		}
	}
}
