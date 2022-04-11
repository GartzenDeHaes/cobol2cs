using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.WorkingStorage
{
	[Serializable]
	public enum BufferType
	{
		Unknown = 0,
		Int,
		String,
		Decimal,
		Binary
	}
}
