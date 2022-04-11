using System;
using System.Globalization;

namespace DOR.Core
{
	/// <summary>
	/// Represents a Money (US Currency) value and all functions associated with it.
	/// Arithmetic and type conversion Operators are overloaded so that Money behaves 
	/// similar to an intrinsic type (e.g. decimal or double).
	/// </summary>
	[Serializable]
	public class Money : IComparable
	{
		#region Members
		
		decimal m_amt;

		#endregion Members

		#region Ctors

		/// <summary>
		/// Default Constructor sets value to zero
		/// </summary>
		public Money()
		{
		}

		/// <summary>
		/// Construct from a Money type
		/// </summary>
		/// <param name="m"></param>
		public Money(Money m)
		{
			m_amt = m.m_amt;
		}

		/// <summary>
		/// Construct from a decimal value
		/// </summary>
		/// <param name="decAmt"></param>
		public Money(decimal decAmt)
		{
			m_amt = decAmt;
			Round();
		}

		/// <summary>
		/// Construct from a decimal value
		/// </summary>
		/// <param name="decAmt"></param>
		public Money(double amount)
		{
			m_amt = (decimal)amount;
			Round();
		}

		/// <summary>
		/// Construct from string (string must be in correct format or exception will be thrown)
		/// </summary>
		/// <param name="samt"></param>
		public static Money Parse(object oamt)
		{
			if (oamt is DBNull)
			{
				return null;
			}
			if (null == oamt)
			{
				return null;
			}
			if (oamt is decimal)
			{
				return new Money((decimal)oamt);
			}

			string samt = oamt.ToString();
			if (string.IsNullOrEmpty(samt))
			{
				return null;
			}

			return new Money(Decimal.Parse(samt, NumberStyles.Currency));
		}

		#endregion Ctors

		#region Public Methods

		/// <summary>
		/// Return the current value of the money amount
		/// </summary>
		public decimal Value
		{
			get { return m_amt; }
		}

		public long Dollars
		{
			get
			{
				string s = m_amt.ToString();
				return Int64.Parse(s.Substring(0, s.IndexOf('.')));
			}
		}

		public int Cents
		{
			get
			{
				string s = m_amt.ToString();
				return Int32.Parse(s.Substring(s.IndexOf('.') + 1));
			}
		}

		/// <summary>
		/// default string value includes zeros to two places
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return ToString(true);
		}

		/// <summary>
		/// To string specifying if zeros after decimal are to be returned
		/// </summary>
		/// <param name="returnZeros"></param>
		/// <returns></returns>
		public string ToString(bool returnZeros)
		{
			return Format(m_amt, returnZeros);
		}
		
		/// <summary>
		/// To string allowing number of decimal places to be specified
		/// </summary>
		/// <param name="returnZeros"></param>
		/// <param name="numDecimals"></param>
		/// <returns></returns>
		public string ToString(bool returnZeros, int numDecimals)
		{
			return Format(m_amt, returnZeros, numDecimals);
		}

		/// <summary>
		/// Required for GetHashCode override.
		/// </summary>
		public override bool Equals(object o)
		{
			var m = o as Money;
			if (m != null)
			{
				return m.m_amt == m_amt;
			}
			return false;
		}

		/// <summary>
		/// HashCode based on the amount.
		/// </summary>
		public override int GetHashCode()
		{
			return m_amt.GetHashCode();
		}

		/// <summary>
		/// IComparable interface
		/// </summary>
		public int CompareTo(object obj)
		{
			if (obj is Money)
			{
				return (this < (Money)obj) ? -1 : (this > (Money)obj) ? 1 : 0;
			}
			if (obj is decimal)
			{
				return (this < (decimal)obj) ? -1 : (this > (decimal)obj) ? 1 : 0;
			}
			throw new ArgumentException("Cannot convert " + obj.GetType().Name + " to Money");
		}

		#endregion Public Methods

		#region Public Static Methods

		public static Money Zero = new Money();

		/// <summary>
		/// Format a decimal number as a money string specifying number of decimals   
		/// </summary>
		/// <param name="amt"></param>
		/// <param name="returnZeros"></param>
		/// <param name="numDecimals"></param>
		/// <returns></returns>
		public static string Format(decimal amt, bool returnZeros, int numDecimals)
		{
			if ( !returnZeros && amt == 0 )
			{
				return "";
			}
			
			//NumberFormatInfo moneyFormat =(NumberFormatInfo)NumberFormatInfo.InvariantInfo.Clone();
			moneyFormat.CurrencyDecimalDigits = numDecimals;
			moneyFormat.CurrencySymbol = "";
			
			return amt.ToString("c", moneyFormat);
		}

		/// <summary>
		/// Format a decimal number as a money string with or without trailing zeros
		/// </summary>
		/// <param name="amt"></param>
		/// <param name="returnZeros"></param>
		/// <returns></returns>
		public static string Format(decimal amt, bool returnZeros)
		{
			return Format(amt,returnZeros,2);
		}

		/// <summary>
		/// Format a <see cref="Money"/> as a string with or without trailing zeros
		/// </summary>
		/// <param name="m"></param>
		/// <param name="returnZeros"></param>
		/// <returns></returns>
		public static string Format(Money m, bool returnZeros)
		{
			return Format(m.m_amt, returnZeros);
		}

		/// <summary>
		/// Validate that a string value can be interpreted as a <see cref="Money"/> value
		/// </summary>
		/// <param name="sAmt"></param>
		/// <returns></returns>
		public static bool IsMoney(string sAmt)
		{
			decimal result;
			return Decimal.TryParse(sAmt, out result);
		}

		#endregion Public Static Methods

		#region Private Methods

		/// <summary>
		/// rounding of the current value to two decimal places
		/// </summary>
		private void Round()
		{
			if ( m_amt == 0 )
				return;

#if SILVERLIGHT
			m_amt = Decimal.Round(m_amt, 2);
#else
            m_amt = Decimal.Round(m_amt, 2, MidpointRounding.AwayFromZero);
#endif
		}
		
		// Get the culture independant format
		private static readonly NumberFormatInfo moneyFormat =
							(NumberFormatInfo)NumberFormatInfo.InvariantInfo.Clone();

		#endregion Private Methods

		#region Operator overloads

		/// <summary>
		/// Multiply a <see cref="Money"/> times a Double value
		/// </summary>
		/// <param name="m"></param>
		/// <param name="d"></param>
		/// <returns></returns>
        public static Money operator *(Money m, double d)
		{
			return new Money(m.m_amt * (decimal)d);
		}
		
		/// <summary>
		/// Multiply a <see cref="Money"/> times a decimal value
		/// </summary>
		/// <param name="m"></param>
		/// <param name="d"></param>
		/// <returns></returns>
		public static Money operator *(Money m, decimal d)
		{
			return new Money(m.m_amt * d);
		}

		/// <summary>
		/// Multiply a <see cref="Money"/> times an int value
		/// </summary>
		/// <param name="m"></param>
		/// <param name="d"></param>
		/// <returns></returns>
		public static Money operator *(Money m, int d)
		{
			return new Money(m.m_amt * d);
		}

		/// <summary>
		/// Multiply two <see cref="moneyFormat"/> values
		/// </summary>
		/// <param name="m1"></param>
		/// <param name="m2"></param>
		/// <returns></returns>
		public static Money operator *(Money m1, Money m2)
		{
			return new Money(m1.m_amt * m2.m_amt);
		}
		
		/// <summary>
		/// Add two <see cref="moneyFormat"/> values
		/// </summary>
		/// <param name="m1"></param>
		/// <param name="m2"></param>
		/// <returns></returns>
		public static Money operator +(Money m1, Money m2)
		{
			return new Money(m1.m_amt + m2.m_amt);
		}

		/// <summary>
		/// Add a decimal to a <see cref="Money"/> value
		/// </summary>
		/// <param name="m1"></param>
		/// <param name="d"></param>
		/// <returns></returns>
		public static Money operator +(Money m1, decimal d)
		{
			return new Money(m1.m_amt + d);
		}

		/// <summary>
		/// Subtract two <see cref="Money"/> values
		/// </summary>
		/// <param name="m1"></param>
		/// <param name="m2"></param>
		/// <returns></returns>
		public static Money operator -(Money m1, Money m2)
		{
			return new Money(m1.m_amt - m2.m_amt);
		}

		/// <summary>
		/// Subtract a decimal from a <see cref="Money"/> value
		/// </summary>
		/// <param name="m1"></param>
		/// <param name="d"></param>
		/// <returns></returns>
		public static Money operator -(Money m1, decimal d)
		{
			return new Money(m1.m_amt - d);
		}

		/// <summary>
		/// Compare is a <see cref="Money"/> value greater than a decimal
		/// </summary>
		/// <param name="m1"></param>
		/// <param name="dec"></param>
		/// <returns></returns>
		public static bool operator >(Money m1, decimal dec)
		{
			return m1.m_amt > dec;
		}

		/// <summary>
		/// Compare is a <see cref="Money"/> value less than a decimal
		/// </summary>
		/// <param name="m1"></param>
		/// <param name="dec"></param>
		/// <returns></returns>
		public static bool operator <(Money m1, decimal dec)
		{
			return m1.m_amt < dec;
		}

		/// <summary>
		/// Compare is a <see cref="Money"/> value greater than a Money value
		/// </summary>
		/// <param name="m1"></param>
		/// <param name="m2"></param>
		/// <returns></returns>
		public static bool operator >(Money m1, Money m2)
		{
			return m1.m_amt > m2.m_amt;
		}

		/// <summary>
		/// Compare is a <see cref="Money"/> value less than a decimal 
		/// </summary>
		/// <param name="m1"></param>
		/// <param name="m2"></param>
		/// <returns></returns>
		public static bool operator <(Money m1, Money m2)
		{
			return m1.m_amt < m2.m_amt;
		}

		/// <summary>
		/// Are two <see cref="Money"/> values equal (==)
		/// </summary>
		/// <param name="m1"></param>
		/// <param name="m2"></param>
		/// <returns></returns>
		public static bool operator ==(Money m1, Money m2)
		{
			if ( (object)m1 != null && (object)m2 == null )
				return(false);

			if ( (object)m1 == null && (object)m2 != null )
				return(false);

			if ( (object)m1 == null && (object)m2 == null )
				return(true);

			return m1.m_amt == m2.m_amt;
		}

		/// <summary>
		/// Are two <see cref="Money"/> values unequal (!=)
		/// </summary>
		/// <param name="m1"></param>
		/// <param name="m2"></param>
		/// <returns></returns>
		public static bool operator !=(Money m1, Money m2)
		{
			if ( (object)m1 != null && (object)m2 == null )
				return(true);

			if ( (object)m1 == null && (object)m2 != null )
				return(true);

			if ( (object)m1 == null && (object)m2 == null )
				return(false);

			return m1.m_amt != m2.m_amt;
		}

		/// <summary>
		/// Compare equality (==) between a <see cref="Money"/> and an Int value
		/// </summary>
		/// <param name="m1"></param>
		/// <param name="m2"></param>
		/// <returns></returns>
		public static bool operator ==(Money m1, int m2)
		{
			if ((object)m1 == null)
				return (false);

			return m1.m_amt == m2;
		}

		/// <summary>
		/// Compare equality (==) between a <see cref="Money"/> and an Int value
		/// </summary>
		/// <param name="m1"></param>
		/// <param name="m2"></param>
		/// <returns></returns>
		public static bool operator ==(Money m1, decimal m2)
		{
			if ((object)m1 == null)
				return (false);

			return m1.m_amt == m2;
		}

		/// <summary>
		/// Compare equality (==) between a <see cref="Money"/> and an Int value
		/// </summary>
		/// <param name="m1"></param>
		/// <param name="m2"></param>
		/// <returns></returns>
		public static bool operator !=(Money m1, decimal m2)
		{
			if ((object)m1 == null)
				return (true);

			return m1.m_amt != m2;
		}

		/// <summary>
		/// Compare in-equality (!=) between a <see cref="Money"/> and an Int value
		/// </summary>
		/// <param name="m1"></param>
		/// <param name="m2"></param>
		/// <returns></returns>
		public static bool operator !=(Money m1, int m2)
		{
			if ( (object)m1 == null )
				return(true);

			return m1.m_amt != m2;
		}

		/// <summary>
		/// explicit cast to decimal
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		public static implicit operator decimal(Money m)
		{
			return null == m ? 0 : m.m_amt;
		}

		/// <summary>
		/// explicit cast to double
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
        public static explicit operator double(Money m)
        {
            return Convert.ToDouble(m.m_amt);
        }

		/// <summary>
		/// explicit cast to int  //TODO: shouldn't this round first
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
        public static explicit operator int(Money m)
        {
            return Convert.ToInt32(m.m_amt);
        }

		/// <summary>
		/// Unary subtract a <see cref="Money"/> value
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		public static Money operator -(Money m)
		{
			return new Money( -m.m_amt );
		}

		/// <summary>
		/// Compare greater than or equal (>=) between two <see cref="Money"/> values
		/// </summary>
		/// <param name="m1"></param>
		/// <param name="m2"></param>
		/// <returns></returns>
		public static bool operator >=(Money m1, int m2)
		{
			return m1.m_amt >= m2;
		}

		/// <summary>
		/// Compare less than or equal (&lt;=) between a <see cref="Money"/> object and an int
		/// </summary>
		/// <param name="m1"></param>
		/// <param name="m2"></param>
		/// <returns></returns>
		public static bool operator <=(Money m1, int m2)
		{
			return m1.m_amt <= m2;
		}

		/// <summary>
		/// Compare greater than or equal (>=) between a <see cref="Money"/> object and a decimal
		/// </summary>
		/// <param name="m1"></param>
		/// <param name="m2"></param>
		/// <returns></returns>
		public static bool operator >=(Money m1, decimal m2)
		{
			return m1.m_amt >= m2;
		}

		/// <summary>
		/// Compare less than or equal (&lt;=) between a <see cref="Money"/> object and a decimal
		/// </summary>
		/// <param name="m1"></param>
		/// <param name="m2"></param>
		/// <returns></returns>
		public static bool operator <=(Money m1, decimal m2)
		{
			return m1.m_amt <= m2;
		}

		/// <summary>
		/// Compare greater than or equal (>=) between two <see cref="Money"/> objects
		/// </summary>
		/// <param name="m1"></param>
		/// <param name="m2"></param>
		/// <returns></returns>
		public static bool operator >=(Money m1, Money m2)
		{
			return m1.m_amt >= m2.m_amt;
		}

		/// <summary>
		/// Compare less than or equal (&lt;=) between two <see cref="Money"/> object and a decimal
		/// </summary>
		/// <param name="m1"></param>
		/// <param name="m2"></param>
		/// <returns></returns>
		public static bool operator <=(Money m1, Money m2)
		{
			return m1.m_amt <= m2.m_amt;
		}

		#endregion Operator overloads
	}
}
