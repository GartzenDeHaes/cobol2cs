using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using DOR.Core;
using DOR.Core.ComponentModel;
using DOR.WorkingStorage.Pic;

namespace DOR.WorkingStorage
{
	[Serializable]
	public abstract class IBufferOffset : RegisterNotifyPropertyChangedBase
	{
		public abstract MemoryBuffer Buffer
		{
			get;
		}

		public abstract PicFormat Format
		{
			get;
			protected set;
		}

		public abstract string Redefines
		{
			get;
			protected set;
		}

		public abstract int Occurances
		{
			get;
		}

		public abstract string[] Values
		{
			get;
			protected set;
		}

		public abstract bool IsSpaces
		{
			get;
		}

		public abstract string Name
		{
			get;
			protected set;
		}

		public abstract int Start
		{
			get;
			protected set;
		}

		public abstract int Length
		{
			get;
			protected set;
		}

		public abstract string Value
		{
			get;
		}

		public abstract void Initialize();
		public abstract void Reset();

		public abstract void Set(int i);
		public abstract void Set(long i);
		public abstract void Set(string s);
		public abstract void Set(decimal d);
		public abstract void Set(double d);
		public abstract void Set(bool b);
		public abstract void Set(NumericTemp i, bool round = false);
		public abstract void Set(Zeroes zs);
		public abstract void Set(HighValues zs);
		public abstract void Set(LowValues zs);
		public abstract void Set(Spaces ss);
		public abstract void Set(IBufferOffset ws);
		public abstract long ToInt();
		public abstract long ToLong();
		public abstract int ToInt32();
		public abstract int ToInt16();
		public abstract decimal ToDecimal();
		public abstract double ToDouble();
		public abstract bool ToBool();

		public abstract IBufferOffset CreateSubRange(int start, int len);

		public abstract string ToRawString();
		public abstract string ToDebugPrintString();
		public abstract string ToString(string format);

		public override abstract bool Equals(object obj);
		public override abstract int GetHashCode();

		public abstract void Inc(long val);

		public abstract void Inc(decimal val);

		public abstract void Inc(double val);

		public abstract void Inc(IBufferOffset val);

		public int IndexOf(char ch)
		{
			for (int x = 0; x < Length; x++)
			{
				if (Buffer[x + Start] == (byte)ch)
				{
					return x;
				}
			}
			return -1;
		}

		public abstract bool IsOneOf(params int[] values);
		public abstract bool IsOneOf(params string[] values);
		public abstract bool IsInRange(int low, int hi);
		public abstract bool IsInRange(string low, string hi);

		public static bool operator ==(IBufferOffset b1, IBufferOffset b2)
		{
			if (b1.Format.IsNumeric || b2.Format.IsNumeric)
			{
				return b1.ToDecimal() == b2.ToDecimal();
			}
			return b1.ToRawString().CompareTo(b1.Format.ToRawString(b2.ToRawString())) == 0;
		}

		public static bool operator ==(IBufferOffset b1, NumericTemp b2)
		{
			if (b1.Format.IsNumeric)
			{
				return b1.ToDecimal() == b2.ToDecimal();
			}
			return b1.ToRawString().CompareTo(b1.Format.ToRawString(b2.ToString())) == 0;
		}

		public static bool operator !=(IBufferOffset b1, NumericTemp b2)
		{
			return !(b1 == b2);
		}

		public static bool operator ==(IBufferOffset b1, decimal b2)
		{
			if (b1.Format.IsNumeric)
			{
				return b1.ToDecimal() == b2;
			}
			return b1.ToRawString().CompareTo(b1.Format.ToRawString(b2.ToString())) == 0;
		}

		public static bool operator !=(IBufferOffset b1, decimal b2)
		{
			return !(b1 == b2);
		}

		public static bool operator >=(IBufferOffset b1, NumericTemp b2)
		{
			if (b1.Format.IsNumeric)
			{
				return b1.ToDecimal() >= b2.ToDecimal();
			}
			return b1.ToRawString().CompareTo(b1.Format.ToRawString(b2.ToString())) >= 0;
		}

		public static bool operator <=(IBufferOffset b1, NumericTemp b2)
		{
			if (b1.Format.IsNumeric)
			{
				return b1.ToDecimal() <= b2.ToDecimal();
			}
			return b1.ToRawString().CompareTo(b1.Format.ToRawString(b2.ToString())) <= 0;
		}

		public static bool operator ==(IBufferOffset b1, Spaces b2)
		{
			return b1.ToRawString().Trim().Length == 0;
		}

		public static bool operator ==(IBufferOffset b1, Numeric b2)
		{
			return StringHelper.IsNumeric(b1.ToRawString().Trim());
		}

		public static bool operator ==(IBufferOffset b1, Zeroes b2)
		{
			return b1.ToDecimal() == 0;
		}

		public static bool operator ==(IBufferOffset b1, string b2)
		{
			return b1.ToString() == b2;
		}

		public static bool operator !=(IBufferOffset b1, IBufferOffset b2)
		{
			return !(b1 == b2);
		}

		public static bool operator !=(IBufferOffset b1, Spaces b2)
		{
			return !(b1 == b2);
		}

		public static bool operator !=(IBufferOffset b1, Numeric b2)
		{
			return ! StringHelper.IsNumeric(b1.ToString().Trim());
		}

		public static bool operator !=(IBufferOffset b1, Zeroes b2)
		{
			return b1.ToDecimal() != 0;
		}

		public static bool operator !=(IBufferOffset b1, string b2)
		{
			return b1.ToString() != b2;
		}

		public static bool operator <(IBufferOffset b1, IBufferOffset b2)
		{
			if (b1.Format.IsNumeric || b2.Format.IsNumeric)
			{
				return b1.ToDecimal() < b2.ToDecimal();
			}
			return b1.ToRawString().CompareTo(b1.Format.ToRawString(b2.ToRawString())) < 0;
		}

		public static bool operator <(IBufferOffset b1, NumericTemp b2)
		{
			return b1.ToDecimal() < b2.ToDecimal();
		}

		public static NumericTemp operator +(IBufferOffset b1, NumericTemp b2)
		{
			if (b1.Format.IsFloat || !b2.IsInt)
			{
				return new NumericTemp(b1.ToDecimal() + b2.ToDecimal());
			}
			return new NumericTemp(b1.ToInt() + b2.ToInt());
		}

		public static NumericTemp operator -(IBufferOffset b1, NumericTemp b2)
		{
			if (b1.Format.IsFloat || !b2.IsInt)
			{
				return new NumericTemp(b1.ToDecimal() - b2.ToDecimal());
			}
			return new NumericTemp(b1.ToInt() - b2.ToInt());
		}

		public static NumericTemp operator *(IBufferOffset b1, NumericTemp b2)
		{
			if (b1.Format.IsFloat || !b2.IsInt)
			{
				return new NumericTemp(b1.ToDecimal() * b2.ToDecimal());
			}
			return new NumericTemp(b1.ToInt() * b2.ToInt());
		}

		public static NumericTemp operator /(IBufferOffset b1, NumericTemp b2)
		{
			if (b1.Format.IsFloat || !b2.IsInt)
			{
				return new NumericTemp(b1.ToDecimal() / b2.ToDecimal());
			}
			return new NumericTemp(b1.ToInt() / b2.ToInt());
		}

		public static NumericTemp operator %(IBufferOffset b1, NumericTemp b2)
		{
			if (b1.Format.IsFloat || !b2.IsInt)
			{
				return new NumericTemp(b1.ToDecimal() % b2.ToDecimal());
			}
			return new NumericTemp(b1.ToInt() % b2.ToInt());
		}

		public static bool operator <(IBufferOffset b1, Spaces b2)
		{
			throw new NotImplementedException("What exactly does this mean?");
		}

		public static bool operator >(IBufferOffset b1, IBufferOffset b2)
		{
			if (b1.Format.IsNumeric || b2.Format.IsNumeric)
			{
				return b1.ToDecimal() > b2.ToDecimal();
			}
			return b1.ToRawString().CompareTo(b1.Format.ToRawString(b2.ToRawString())) > 0;
		}

		public static bool operator >(IBufferOffset b1, NumericTemp b2)
		{
			return b1.ToDecimal() > b2.ToDecimal();
		}

		public static bool operator >(IBufferOffset b1, Spaces b2)
		{
			return !b1.IsSpaces;
		}

		public static bool operator >(IBufferOffset b1, Zeroes b2)
		{
			return b1.ToInt() > 0;
		}

		public static bool operator <(IBufferOffset b1, Zeroes b2)
		{
			return b1.ToInt() < 0;
		}

		public static bool operator <=(IBufferOffset b1, IBufferOffset b2)
		{
			if (b1.Format.IsNumeric || b2.Format.IsNumeric)
			{
				return b1.ToDecimal() <= b2.ToDecimal();
			}
			return b1.ToRawString().CompareTo(b1.Format.ToRawString(b2.ToRawString())) <= 0;
		}

		public static bool operator >=(IBufferOffset b1, IBufferOffset b2)
		{
			if (b1.Format.IsNumeric || b2.Format.IsNumeric)
			{
				return b1.ToDecimal() >= b2.ToDecimal();
			}
			return b1.ToRawString().CompareTo(b1.Format.ToRawString(b2.ToRawString())) >= 0;
		}
		
		public static NumericTemp operator +(IBufferOffset b1, IBufferOffset b2)
		{
			if (b1.Format.IsFloat || b2.Format.IsFloat)
			{
				return new NumericTemp(b1.ToDecimal() + b2.ToDecimal());
			}
			return new NumericTemp(b1.ToInt() + b2.ToInt());
		}

		public static NumericTemp operator -(IBufferOffset b1, IBufferOffset b2)
		{
			if (b1.Format.IsFloat || b2.Format.IsFloat)
			{
				return new NumericTemp(b1.ToDecimal() - b2.ToDecimal());
			}
			return new NumericTemp(b1.ToInt() - b2.ToInt());
		}

		public static NumericTemp operator *(IBufferOffset b1, IBufferOffset b2)
		{
			if (b1.Format.IsFloat || b2.Format.IsFloat)
			{
				return new NumericTemp(b1.ToDecimal() * b2.ToDecimal());
			}
			return new NumericTemp(b1.ToInt() * b2.ToInt());
		}

		public static NumericTemp operator /(IBufferOffset b1, IBufferOffset b2)
		{
			if (b1.Format.IsFloat || b2.Format.IsFloat)
			{
				return new NumericTemp(b1.ToDecimal() / b2.ToDecimal());
			}
			return new NumericTemp(b1.ToInt() / b2.ToInt());
		}

		public static NumericTemp operator %(IBufferOffset b1, IBufferOffset b2)
		{
			if (b1.Format.IsFloat || b2.Format.IsFloat)
			{
				return new NumericTemp(b1.ToDecimal() % b2.ToDecimal());
			}
			return new NumericTemp(b1.ToInt() % b2.ToInt());
		}

		public static bool operator ==(int b1, IBufferOffset b2)
		{
			return b2 == b1;
		}

		public static bool operator !=(int b1, IBufferOffset b2)
		{
			return !(b1 == b2);
		}

		public static bool operator ==(long b1, IBufferOffset b2)
		{
			return b2 == b1;
		}

		public static bool operator !=(long b1, IBufferOffset b2)
		{
			return !(b1 == b2);
		}

		public static bool operator ==(decimal b1, IBufferOffset b2)
		{
			return b2 == b1;
		}

		public static bool operator !=(decimal b1, IBufferOffset b2)
		{
			return !(b1 == b2);
		}

		public static bool operator ==(IBufferOffset b1, long b2)
		{
			if (b1.Format.IsNumeric)
			{
				return b1.ToDecimal() == b2;
			}
			return b1.ToRawString().CompareTo(b1.Format.ToRawString(b2.ToString())) == 0;
		}

		public static bool operator !=(IBufferOffset b1, long b2)
		{
			return !(b1 == b2);
		}

		public static bool operator <(IBufferOffset b1, long b2)
		{
			if (b1.Format.IsNumeric)
			{
				return b1.ToDecimal() < b2;
			}
			return b1.ToRawString().CompareTo(b1.Format.ToRawString(b2.ToString())) < 0;
		}

		public static bool operator >(IBufferOffset b1, long b2)
		{
			if (b1.Format.IsNumeric)
			{
				return b1.ToDecimal() > b2;
			}
			return b1.ToRawString().CompareTo(b1.Format.ToRawString(b2.ToString())) > 0;
		}

		public static bool operator <=(IBufferOffset b1, long b2)
		{
			if (b1.Format.IsNumeric)
			{
				return b1.ToDecimal() <= b2;
			}
			return b1.ToRawString().CompareTo(b1.Format.ToRawString(b2.ToString())) <= 0;
		}

		public static bool operator >=(IBufferOffset b1, long b2)
		{
			if (b1.Format.IsNumeric)
			{
				return b1.ToDecimal() >= b2;
			}
			return b1.ToRawString().CompareTo(b1.Format.ToRawString(b2.ToString())) >= 0;
		}

		public static NumericTemp operator +(IBufferOffset b1, long b2)
		{
			if (b1.Format.IsFloat)
			{
				return new NumericTemp(b1.ToDecimal() + (decimal)b2);
			}
			return new NumericTemp(b1.ToInt() + b2);
		}

		public static NumericTemp operator -(IBufferOffset b1, long b2)
		{
			if (b1.Format.IsFloat)
			{
				return new NumericTemp(b1.ToDecimal() - (decimal)b2);
			}
			return new NumericTemp(b1.ToInt() - b2);
		}

		public static NumericTemp operator *(IBufferOffset b1, long b2)
		{
			if (b1.Format.IsFloat)
			{
				return new NumericTemp(b1.ToDecimal() * (decimal)b2);
			}
			return new NumericTemp(b1.ToInt() * b2);
		}

		public static NumericTemp operator /(IBufferOffset b1, int b2)
		{
			if (b1.Format.IsFloat)
			{
				return new NumericTemp(b1.ToDecimal() / (decimal)b2);
			}
			return new NumericTemp(b1.ToInt() / b2);
		}

		public static NumericTemp operator %(IBufferOffset b1, int b2)
		{
			if (b1.Format.IsFloat)
			{
				return new NumericTemp(b1.ToDecimal() / (decimal)b2);
			}
			return new NumericTemp(b1.ToInt() % b2);
		}

		public static implicit operator bool(IBufferOffset b)
		{
			return b.ToBool();
		}

		public static bool operator ==(IBufferOffset b1, double b2)
		{
			if (b1.Format.IsNumeric)
			{
				return b1.ToDecimal() == (decimal)b2;
			}
			return b1.ToRawString().CompareTo(b1.Format.ToRawString(b2.ToString())) == 0;
		}

		public static bool operator !=(IBufferOffset b1, double b2)
		{
			return !(b1 == b2);
		}

		public static bool operator <(IBufferOffset b1, double b2)
		{
			if (b1.Format.IsNumeric)
			{
				return b1.ToDecimal() < (decimal)b2;
			}
			return b1.ToRawString().CompareTo(b1.Format.ToRawString(b2.ToString())) < 0;
		}

		public static bool operator >(IBufferOffset b1, double b2)
		{
			if (b1.Format.IsNumeric)
			{
				return b1.ToDecimal() > (decimal)b2;
			}
			return b1.ToRawString().CompareTo(b1.Format.ToRawString(b2.ToString())) > 0;
		}

		public static bool operator <=(IBufferOffset b1, double b2)
		{
			if (b1.Format.IsNumeric)
			{
				return b1.ToDecimal() <= (decimal)b2;
			}
			return b1.ToRawString().CompareTo(b1.Format.ToRawString(b2.ToString())) <= 0;
		}

		public static bool operator >=(IBufferOffset b1, double b2)
		{
			if (b1.Format.IsNumeric)
			{
				return b1.ToDecimal() >= (decimal)b2;
			}
			return b1.ToRawString().CompareTo(b1.Format.ToRawString(b2.ToString())) <= 0;
		}

		public static NumericTemp operator +(IBufferOffset b1, double b2)
		{
			if (b1.Format.IsFloat)
			{
				return new NumericTemp(b1.ToDecimal() + (decimal)b2);
			}
			return new NumericTemp((decimal)(b1.ToInt() + b2));
		}

		public static NumericTemp operator -(IBufferOffset b1, double b2)
		{
			if (b1.Format.IsFloat)
			{
				return new NumericTemp(b1.ToDecimal() - (decimal)b2);
			}
			return new NumericTemp((decimal)(b1.ToInt() - b2));
		}

		public static NumericTemp operator *(IBufferOffset b1, double b2)
		{
			if (b1.Format.IsFloat)
			{
				return new NumericTemp(b1.ToDecimal() * (decimal)b2);
			}
			return new NumericTemp((decimal)(b1.ToInt() * b2));
		}

		public static NumericTemp operator /(IBufferOffset b1, double b2)
		{
			if (b1.Format.IsFloat)
			{
				return new NumericTemp(b1.ToDecimal() / (decimal)b2);
			}
			return new NumericTemp((decimal)(b1.ToInt() / b2));
		}

		public static bool operator <(IBufferOffset b1, string b2)
		{
			return b1.ToRawString().CompareTo(b1.Format.ToRawString(b2.ToString())) < 0;
		}

		public static bool operator >(IBufferOffset b1, string b2)
		{
			return b1.ToString().CompareTo(b1.Format.Format(b2.ToString())) > 0;
		}

		public static bool operator <=(IBufferOffset b1, string b2)
		{
			return b1.ToString().CompareTo(b1.Format.Format(b2.ToString())) <= 0;
		}

		public static bool operator >=(IBufferOffset b1, string b2)
		{
			return b1.ToString().CompareTo(b1.Format.Format(b2.ToString())) >= 0;
		}

		public static explicit operator int(IBufferOffset b)
		{
			return b.ToInt32();
		}

		public static explicit operator long(IBufferOffset b)
		{
			return b.ToInt();
		}

		public static explicit operator decimal(IBufferOffset b)
		{
			return b.ToDecimal();
		}

		public static explicit operator string(IBufferOffset b)
		{
			return b.ToString();
		}
	}
}
