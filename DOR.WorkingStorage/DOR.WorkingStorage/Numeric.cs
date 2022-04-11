using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core;

namespace DOR.WorkingStorage
{
	public class Numeric
	{
		private static Numeric _single;

		public static Numeric Instance()
		{
			if (null == _single)
			{
				_single = new Numeric();
			}

			return _single;
		}

		public static bool operator ==(Numeric b1, IBufferOffset b2)
		{
			return StringHelper.IsNumeric(b2.ToString().Trim());
		}

		public static bool operator !=(Numeric b1, IBufferOffset b2)
		{
			return ! StringHelper.IsNumeric(b2.ToString().Trim());
		}

		public override bool Equals(object obj)
		{
			if (obj is IBufferOffset)
			{
				return this == (IBufferOffset)obj;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
