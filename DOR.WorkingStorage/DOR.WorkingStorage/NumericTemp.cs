using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.WorkingStorage
{
	public class NumericTemp
	{
		private long _i;
		private decimal _d;

		public bool IsInt
		{
			get;
			private set;
		}

		public NumericTemp(decimal d)
		{
			_d = d;
			IsInt = false;
		}

		public NumericTemp(long i)
		{
			_i = i;
			IsInt = true;
		}

		public long ToInt()
		{
			return IsInt ? _i : (long)_d;
		}

		public decimal ToDecimal()
		{
			return IsInt ? (decimal)_i : _d;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is NumericTemp))
			{
				return false;
			}

			return this == (NumericTemp)obj;
		}

		public override int GetHashCode()
		{
			return ToDecimal().GetHashCode();
		}

		public static bool operator ==(NumericTemp b1, NumericTemp b2)
		{
			if (!b1.IsInt || !b2.IsInt)
			{
				return b1.ToDecimal() == b2.ToDecimal();
			}
			return b1.ToInt() == b2.ToInt();
		}

		public static bool operator !=(NumericTemp b1, NumericTemp b2)
		{
			return !(b1 == b2);
		}

		public static bool operator <(NumericTemp b1, NumericTemp b2)
		{
			if (!b1.IsInt || !b2.IsInt)
			{
				return b1.ToDecimal() < b2.ToDecimal();
			}
			return b1.ToInt() < b2.ToInt();
		}

		public static bool operator >(NumericTemp b1, NumericTemp b2)
		{
			if (!b1.IsInt || !b2.IsInt)
			{
				return b1.ToDecimal() > b2.ToDecimal();
			}
			return b1.ToInt() > b2.ToInt();
		}

		public static bool operator <=(NumericTemp b1, NumericTemp b2)
		{
			if (!b1.IsInt || !b2.IsInt)
			{
				return b1.ToDecimal() <= b2.ToDecimal();
			}
			return b1.ToInt() <= b2.ToInt();
		}

		public static bool operator >=(NumericTemp b1, NumericTemp b2)
		{
			if (!b1.IsInt || !b2.IsInt)
			{
				return b1.ToDecimal() >= b2.ToDecimal();
			}
			return b1.ToInt() >= b2.ToInt();
		}

		public static NumericTemp operator +(NumericTemp b1, NumericTemp b2)
		{
			if (!b1.IsInt || !b2.IsInt)
			{
				return new NumericTemp(b1.ToDecimal() + b2.ToDecimal());
			}
			return new NumericTemp(b1.ToInt() + b2.ToInt());
		}

		public static NumericTemp operator -(NumericTemp b1, NumericTemp b2)
		{
			if (!b1.IsInt || !b2.IsInt)
			{
				return new NumericTemp(b1.ToDecimal() - b2.ToDecimal());
			}
			return new NumericTemp(b1.ToInt() - b2.ToInt());
		}

		public static NumericTemp operator *(NumericTemp b1, NumericTemp b2)
		{
			if (!b1.IsInt || !b2.IsInt)
			{
				return new NumericTemp(b1.ToDecimal() * b2.ToDecimal());
			}
			return new NumericTemp(b1.ToInt() * b2.ToInt());
		}

		public static NumericTemp operator /(NumericTemp b1, NumericTemp b2)
		{
			if (!b1.IsInt || !b2.IsInt)
			{
				return new NumericTemp(b1.ToDecimal() / b2.ToDecimal());
			}
			return new NumericTemp(b1.ToInt() / b2.ToInt());
		}

		public static bool operator |(NumericTemp b1, NumericTemp b2)
		{
			if (!b1.IsInt || !b2.IsInt)
			{
				return b1.ToDecimal() != 0 || b2.ToDecimal() != 0;
			}
			return (b1.ToInt() | b2.ToInt()) != 0;
		}

		public static bool operator &(NumericTemp b1, NumericTemp b2)
		{
			if (!b1.IsInt || !b2.IsInt)
			{
				return b1.ToDecimal() != 0 && b2.ToDecimal() != 0;
			}
			return (b1.ToInt() & b2.ToInt()) != 0;
		}

		public static bool operator <(NumericTemp b1, IBufferOffset b2)
		{
			if (!b1.IsInt || b2.Format.IsFloat)
			{
				return b1.ToDecimal() < b2.ToDecimal();
			}
			return b1.ToInt() < b2.ToInt();
		}

		public static bool operator >(NumericTemp b1, IBufferOffset b2)
		{
			if (!b1.IsInt || b2.Format.IsFloat)
			{
				return b1.ToDecimal() > b2.ToDecimal();
			}
			return b1.ToInt() > b2.ToInt();
		}

		public static bool operator <=(NumericTemp b1, IBufferOffset b2)
		{
			if (!b1.IsInt || b2.Format.IsFloat)
			{
				return b1.ToDecimal() <= b2.ToDecimal();
			}
			return b1.ToInt() <= b2.ToInt();
		}

		public static bool operator >=(NumericTemp b1, IBufferOffset b2)
		{
			if (!b1.IsInt || b2.Format.IsFloat)
			{
				return b1.ToDecimal() >= b2.ToDecimal();
			}
			return b1.ToInt() >= b2.ToInt();
		}

		public static NumericTemp operator +(NumericTemp b1, IBufferOffset b2)
		{
			if (!b1.IsInt || b2.Format.IsFloat)
			{
				return new NumericTemp(b1.ToDecimal() + b2.ToDecimal());
			}
			return new NumericTemp(b1.ToInt() + b2.ToInt());
		}

		public static NumericTemp operator -(NumericTemp b1, IBufferOffset b2)
		{
			if (!b1.IsInt || b2.Format.IsFloat)
			{
				return new NumericTemp(b1.ToDecimal() - b2.ToDecimal());
			}
			return new NumericTemp(b1.ToInt() - b2.ToInt());
		}

		public static NumericTemp operator *(NumericTemp b1, IBufferOffset b2)
		{
			if (!b1.IsInt || b2.Format.IsFloat)
			{
				return new NumericTemp(b1.ToDecimal() * b2.ToDecimal());
			}
			return new NumericTemp(b1.ToInt() * b2.ToInt());
		}

		public static NumericTemp operator /(NumericTemp b1, IBufferOffset b2)
		{
			if (!b1.IsInt || b2.Format.IsFloat)
			{
				return new NumericTemp(b1.ToDecimal() / b2.ToDecimal());
			}
			return new NumericTemp(b1.ToInt() / b2.ToInt());
		}

		public static bool operator |(NumericTemp b1, IBufferOffset b2)
		{
			if (!b1.IsInt || b2.Format.IsFloat)
			{
				return b1.ToDecimal() != 0 || b2.ToDecimal() != 0;
			}
			return (b1.ToInt() | b2.ToInt()) != 0;
		}

		public static bool operator &(NumericTemp b1, IBufferOffset b2)
		{
			if (!b1.IsInt || b2.Format.IsFloat)
			{
				return b1.ToDecimal() != 0 && b2.ToDecimal() != 0;
			}
			return (b1.ToInt() & b2.ToInt()) != 0;
		}

		public static NumericTemp operator +(NumericTemp b1, int b2)
		{
			if (!b1.IsInt)
			{
				return new NumericTemp(b1.ToDecimal() + (decimal)b2);
			}
			return new NumericTemp(b1.ToInt() + b2);
		}

		public static NumericTemp operator -(NumericTemp b1, int b2)
		{
			if (!b1.IsInt)
			{
				return new NumericTemp(b1.ToDecimal() - (decimal)b2);
			}
			return new NumericTemp(b1.ToInt() - b2);
		}

		public static NumericTemp operator *(NumericTemp b1, int b2)
		{
			if (!b1.IsInt)
			{
				return new NumericTemp(b1.ToDecimal() * (decimal)b2);
			}
			return new NumericTemp(b1.ToInt() * b2);
		}

		public static NumericTemp operator /(NumericTemp b1, int b2)
		{
			if (!b1.IsInt)
			{
				return new NumericTemp(b1.ToDecimal() / (decimal)b2);
			}
			return new NumericTemp(b1.ToInt() / b2);
		}

		public static bool operator <(NumericTemp b1, int b2)
		{
			if (!b1.IsInt)
			{
				return b1.ToDecimal() < (decimal)b2;
			}
			return b1.ToInt() < b2;
		}

		public static bool operator >(NumericTemp b1, int b2)
		{
			if (!b1.IsInt)
			{
				return b1.ToDecimal() > (decimal)b2;
			}
			return b1.ToInt() > b2;
		}

		public static bool operator <=(NumericTemp b1, int b2)
		{
			if (!b1.IsInt)
			{
				return b1.ToDecimal() <= (decimal)b2;
			}
			return b1.ToInt() <= b2;
		}

		public static bool operator >=(NumericTemp b1, int b2)
		{
			if (!b1.IsInt)
			{
				return b1.ToDecimal() >= (decimal)b2;
			}
			return b1.ToInt() >= b2;
		}

		public static bool operator ==(NumericTemp b1, double b2)
		{
			if (!b1.IsInt)
			{
				return b1.ToDecimal() == (decimal)b2;
			}
			return (double)b1.ToInt() == b2;
		}

		public static bool operator !=(NumericTemp b1, double b2)
		{
			return !(b1 == b2);
		}

		public static NumericTemp operator +(NumericTemp b1, double b2)
		{
			if (!b1.IsInt)
			{
				return new NumericTemp(b1.ToDecimal() + (decimal)b2);
			}
			return new NumericTemp((decimal)(b1.ToInt() + b2));
		}

		public static NumericTemp operator -(NumericTemp b1, double b2)
		{
			if (!b1.IsInt)
			{
				return new NumericTemp(b1.ToDecimal() - (decimal)b2);
			}
			return new NumericTemp((decimal)(b1.ToInt() - b2));
		}

		public static NumericTemp operator *(NumericTemp b1, double b2)
		{
			if (!b1.IsInt)
			{
				return new NumericTemp(b1.ToDecimal() * (decimal)b2);
			}
			return new NumericTemp((decimal)(b1.ToInt() * b2));
		}

		public static NumericTemp operator /(NumericTemp b1, double b2)
		{
			if (!b1.IsInt)
			{
				return new NumericTemp(b1.ToDecimal() / (decimal)b2);
			}
			return new NumericTemp((decimal)(b1.ToInt() / b2));
		}

		public static bool operator <(NumericTemp b1, double b2)
		{
			if (!b1.IsInt)
			{
				return b1.ToDecimal() < (decimal)b2;
			}
			return b1.ToInt() < b2;
		}

		public static bool operator >(NumericTemp b1, double b2)
		{
			if (!b1.IsInt)
			{
				return b1.ToDecimal() > (decimal)b2;
			}
			return b1.ToInt() > b2;
		}

		public static bool operator <=(NumericTemp b1, double b2)
		{
			if (!b1.IsInt)
			{
				return b1.ToDecimal() <= (decimal)b2;
			}
			return b1.ToInt() <= b2;
		}

		public static bool operator >=(NumericTemp b1, double b2)
		{
			if (!b1.IsInt)
			{
				return b1.ToDecimal() >= (decimal)b2;
			}
			return b1.ToInt() >= b2;
		}
	}
}
