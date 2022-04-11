using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.WorkingStorage
{
	public class Spaces
	{
		private static Spaces _single;

		public static Spaces Instance()
		{
			if (null == _single)
			{
				_single = new Spaces();
			}

			return _single;
		}

		public static bool operator ==(Spaces b1, IBufferOffset b2)
		{
			return b2.ToString().Trim().Length == 0;
		}

		public static bool operator !=(Spaces b1, IBufferOffset b2)
		{
			return b2.ToString().Trim().Length != 0;
		}

		public static bool operator ==(Spaces b1, string b2)
		{
			return b2.Trim().Length == 0;
		}

		public static bool operator !=(Spaces b1, string b2)
		{
			return b2.Trim().Length != 0;
		}

		public override bool Equals(object obj)
		{
			if (obj is Spaces)
			{
				return true;
			}

			return obj.ToString().Trim().Length == 0;
		}

		public override int GetHashCode()
		{
			return " ".GetHashCode();
		}
	}
}
