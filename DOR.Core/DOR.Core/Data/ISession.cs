using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core.Data
{
	/// <summary>
	/// Interface for an application session.  Add additional custom properties
	/// for common session items such as TRA and period year.
	/// </summary>
	public interface ISession
	{
		/// <summary>Standard method for accessing the TRA being viewed.</summary>
		int? TraId { get; set; }

		/// <summary>The currenly logged on user</summary>
		IUser CurrentUser { get; }

		/// <summary>Standard method for accessing the DOR user ID being viewed (not the logged on user).</summary>
		int? DorUserId { get; set; }

		object this[string idx] { get; set; }

		bool ContainsKey(string idx);

		object Get(string key, bool throwException);
		object Get(string key);
		void Set(string key, object data, int expiresInMinutes);
		void Set(string key, object data);

		void Remove(string key);
		void Clear();

		void SaveChanges();
	}
}
