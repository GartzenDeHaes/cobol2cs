using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core
{
	public interface IPostalAddress
	{
		/// <summary>
		/// Address Line 1
		/// </summary>
		string Line1 { get; }

		/// <summary>
		/// Address City
		/// </summary>
		string City { get; }

		/// <summary>
		/// Address State
		/// </summary>
		string State { get; }

		/// <summary>
		/// Address Zip (full)
		/// </summary>
		IPostalCode ZIP { get; }

		/// <summary>
		/// Indicates that the Address has been run through USPS standardization.
		/// </summary>
		bool IsStandardized { get; }
	}
}
