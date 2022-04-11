using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.WorkingStorage
{
	public class AbortRunException : Exception
	{
		public IBufferOffset OutRecord
		{
			get;
			private set;
		}

		public AbortRunException(IBufferOffset rec)
		{
			OutRecord = rec;
		}

		public AbortRunException()
		{
		}

		public AbortRunException(string msg)
		: base(msg)
		{
		}
	}
}
