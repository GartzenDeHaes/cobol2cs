using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core
{
	/// <summary>
	/// This is a tandem style security permission in the from of SYSTEM PERMNAME
	/// </summary>
	public interface ISecurityAuthorization
	{
		/// <summary>Unique ID</summary>
		int Id { get; }

		/// <summary>Obsolete permissions are deactivated, not deleted.</summary>
		bool IsActive { get; }

		/// <summary>System code, fe BRMS.</summary>
		string System { get; }

		/// <summary>Permission name, fe INQUIRE.</summary>
		string Name { get; }

		/// <summary>Full permission name, fe BRMS INQUIRE.</summary>
		string FullyQualifiedName { get; }
	}
}
