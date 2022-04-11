using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core.Net
{
	public interface IEmailAddress
	{
		/// <summary>
		/// The full email address.
		/// </summary>
		string Email { get; }

		/// <summary>
		/// The host DNS or IP.
		/// </summary>
		string Host { get; }

		/// <summary>
		/// The user ID.
		/// </summary>
		string User { get; }
	}
}
