using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core
{
	public class NtGroupAuthorization : ISecurityAuthorization
	{
		/// <summary>Unique ID</summary>
		public int Id { get; private set; }

		/// <summary>Obsolete permissions are deactivated, not deleted.</summary>
		public bool IsActive { get { return true; } }

		/// <summary>System code, fe BRMS.</summary>
		public string System { get { return "Active Directory"; } }

		/// <summary>Permission name, fe INQUIRE.</summary>
		public string Name { get; private set; }

		/// <summary>Full permission name, fe BRMS INQUIRE.</summary>
		public string FullyQualifiedName { get { return Name; } }

		public NtGroupAuthorization(string name)
		{
			Name = name;
			Id = name.GetHashCode();
		}
	}
}
