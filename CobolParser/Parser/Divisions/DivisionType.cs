using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser
{
	public enum DivisionType
	{
		Unknown = 0,
		Identification = (1<<1),
		Environment = (1<<2),
		Data = (1<<3),
		Procedure = (1<<4),
		Any = Identification | Environment | Data | Procedure,
		DataOrProcedure = Data | Procedure
	}
}
