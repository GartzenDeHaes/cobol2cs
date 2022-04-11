using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.WorkingStorage
{
	public class LowValues
	{
		private static LowValues _single;

		public static LowValues Instance()
		{
			if (null == _single)
			{
				_single = new LowValues();
			}

			return _single;
		}
	}
}
