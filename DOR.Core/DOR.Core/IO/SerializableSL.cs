using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core
{
#if SILVERLIGHT
	[System.AttributeUsage(System.AttributeTargets.Class |
						   System.AttributeTargets.Struct)
	]
	public class Serializable : System.Attribute
	{
	}
#endif
}
