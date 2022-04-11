using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.WorkingStorage
{
	public class Zeroes
	{
		private static Zeroes _single;

		public static Zeroes Instance()
		{
			if (null == _single)
			{
				_single = new Zeroes();
			}

			return _single;
		}

		public static bool IsZeroes(string z)
		{
			if (String.IsNullOrEmpty(z))
			{
				return false;
			}
			for (int x = 0; x < z.Length; x++)
			{
				if (z[x] != '0')
				{
					return false;
				}
			}

			return true;
		}
	}
}
