using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.WorkingStorage
{
	public class HighValues
	{
		private static HighValues _single;

		public static HighValues Instance()
		{
			if (null == _single)
			{
				_single = new HighValues();
			}

			return _single;
		}
	}
}
