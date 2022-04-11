using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core.Config
{
	/// <summary>
	/// Enumerated type representing the four possible environments that an application can
	/// run in: Production, Demo, Test, and Development.
	/// </summary>
	public enum EnvironmentType
	{
		Unknown = 0,
		/// <summary>Production</summary>
		Prod = 1,
		/// <summary>User Test</summary>
		Demo = 2,
		/// <summary>Test</summary>
		Test = 3,
		/// <summary>Some apps have dev too</summary>
		Dev = 4
	}
}
