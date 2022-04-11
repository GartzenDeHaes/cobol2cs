using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

using DOR.Core.ComponentModel;

namespace DOR.Core
{
	/// <summary>Argument to ToString if you need to specify the formating.</summary>
	public enum PhoneNumberAreaCodeFormatStyle
	{
		/// <summary>(000) 000-0000</summary>
		UseParenthesis,
		/// <summary>000-000-0000</summary>
		UseDash
	}

	/// <summary>
	/// A Phone Number
	/// </summary>
	[Serializable]
	[DataContract]
	public class PhoneNumber : NotifyPropertyChangedBase, IPhoneNumber
	{
		[DataMember]
		private int _countryCode = 1;
		[DataMember]
		private int _area = 0;
		[DataMember]
		private int _prefix = 0;
		[DataMember]
		private int _suffix = 0;
		[DataMember]
		private int _ext = 0;

		/// <summary>1 for US, 2 for Canada, 3 for Mexico</summary>
		public int CountryCode
		{
			get { return _countryCode; }
			set
			{
				_countryCode = value;
				RaisePropertyChanged("CountryCode");
			}
		}

		/// <summary>
		/// AreaCode Getter
		/// </summary>
		public int AreaCode
		{
			get { return _area; }
			set
			{
				_area = value;
				RaisePropertyChanged("AreaCode");
			}
		}

		/// <summary>
		/// Prefix getter
		/// </summary>
		public int Prefix
		{
			get	{ return _prefix; }
			set
			{
				_prefix = value;
				RaisePropertyChanged("Prefix");
			}
		}

		/// <summary>
		/// Suffix getter
		/// </summary>
		public int Suffix
		{
			get { return _suffix; }
			set
			{
				_suffix = value;
				RaisePropertyChanged("Suffix");
			}
		}

		/// <summary>
		/// Extension getter
		/// </summary>
		public int Extension
		{
			get { return _ext; }
			set
			{
				_ext = value;
				RaisePropertyChanged("Extension");
			}
		}

		public PhoneNumber()
		{
		}

		/// <summary>
		/// Construct from integers for area, prefix and suffix
		/// </summary>
		/// <param name="area"></param>
		/// <param name="prefix"></param>
		/// <param name="suffix"></param>
		public PhoneNumber(int area, int prefix, int suffix) 
		: this(area, prefix, suffix, 0)
		{
		}

		/// <summary>
		/// Construct from integers for area, prefix and suffix plus 
		/// a string for extension
		/// </summary>
		/// <param name="area"></param>
		/// <param name="prefix"></param>
		/// <param name="suffix"></param>
		/// <param name="ext"></param>
		public PhoneNumber(int area, int prefix, int suffix, int ext)
		: this(1, area, prefix, suffix, ext)
		{
		}

		/// <summary>
		/// Construct from integers for area, prefix and suffix plus 
		/// a string for extension
		/// </summary>
		/// <param name="area"></param>
		/// <param name="prefix"></param>
		/// <param name="suffix"></param>
		/// <param name="ext"></param>
		/// <param name="countryCode">1 for US</param>
		public PhoneNumber(int countryCode, int area, int prefix, int suffix, int ext)
		{
			_countryCode = countryCode;
			_area = area;
			_prefix = prefix;
			_suffix = suffix;
			_ext = ext;
		}

		/// <summary>
		/// Construct from a long integer (assumes area, prefix & suffix)
		/// (e.g. long phoneNum = 3602342345)
		/// </summary>
		/// <param name="phoneNum"></param>
		public PhoneNumber( long phoneNum )
		{
			ParseNumeric( phoneNum, out _area, out _prefix, out _suffix );
		}

		/// <summary>
		/// Construct a phone # from an input string.  Expects a full
		/// phone # (area, pre & suffix) without extension.  Punctuation "(" & "-" & " ")
		/// are okay.
		/// </summary>
		/// <param name="phoneNum"></param>
		public static PhoneNumber Parse(string phoneNum)
		{
			PhoneNumber phone;

			if (TryParse(phoneNum, out phone))
			{
				return phone;
			}

			throw new FormatException(phoneNum + " is an invalid phone number");
		}

		public static bool TryParse(string phoneNum, out PhoneNumber phone)
		{
			phone = null;

			try
			{
				string stripped = phoneNum.ToUpper();

				if (StringHelper.CountOccurancesOf(stripped, '-') > 3)
				{
					// Fix for weird data
					if (stripped.IndexOf("--") > 0)
					{
						stripped = stripped.Replace("--", "EXT");
					}
				}
				if (stripped.IndexOf('(') > -1)
				{
					stripped = stripped.Replace("(", "").Replace(")", "");
				}
				if (stripped.IndexOf(' ') > -1)
				{
					stripped = stripped.Replace(" ", "");
				}
				if (stripped.IndexOf('-') > -1)
				{
					stripped = stripped.Replace("-", "");
				}
				if (stripped.IndexOf('/') > -1)
				{
					// BRMS has some phones like 360/123-1234
					stripped = stripped.Replace("/", "");
				}
				if (stripped.IndexOf(':') > -1)
				{
					// phone control adds this
					stripped = stripped.Replace(":", "");
				}

				if (0 == stripped.Length)
				{
					return false;
				}

				int ext = 0;

				if (stripped.IndexOf("EXT") > 0)
				{
					int pos = stripped.IndexOf("EXT");
					string sext = stripped.Substring(pos + 3).Trim();
					if (!String.IsNullOrEmpty(sext))
					{
						ext = Int32.Parse(StringHelper.RemoveNonNumerics(sext));
					}
					stripped = StringHelper.MidStr(stripped, 0, pos);
				}

				int countryCode = 1;

				if (stripped.Length == 11)
				{
					if (Char.IsDigit(stripped[0]))
					{
						countryCode = Int32.Parse(stripped[0].ToString());
					}
					stripped = stripped.Substring(1);
				}
				if (stripped.Length != 10)
				{
					throw new FormatException(phoneNum + " is an invalid phone number");
				}
				if (StringHelper.IsNumeric(stripped))
				{
					long iphoneNum = Int64.Parse(stripped);
					int area, suffix, prefix;
					ParseNumeric(iphoneNum, out area, out prefix, out suffix);
					phone = new PhoneNumber(countryCode, area, prefix, suffix, ext);
					return true;
				}
				else
				{
					return false;
				}
			}
			catch(Exception)
			{
				return false;
			}
		}

		private static void ParseNumeric( long phoneNum, out int area, out int prefix, out int suffix )
		{
			area = (int)(phoneNum / 10000000L);
			phoneNum -= area * 10000000L;
			prefix = (int)(phoneNum / 10000);
			suffix = (int)(phoneNum - prefix * 10000);
		}

		/// <summary>
		/// Returns a formatted string in the form
		/// (999) 999-9999 EXT: 123 OR
		/// 999-999-9999 EXT: 123
		/// </summary>
		/// <returns></returns>
		public string ToString(PhoneNumberAreaCodeFormatStyle areaCodeFormat)
		{
			if (0 == _area && 0 == _prefix && 0 == _suffix)
			{
				return String.Empty;
			}

			if (areaCodeFormat == PhoneNumberAreaCodeFormatStyle.UseParenthesis)
			{
				return ToString();
			}

			string format;

			if (_ext > 0)
			{
				if (areaCodeFormat == PhoneNumberAreaCodeFormatStyle.UseParenthesis)
				{
					format = "({0}) {1}-{2} EXT: {3}";
				}
				else
				{
					format = "{0}-{1}-{2} EXT: {3}";
				}
				return string.Format
				(
					format,
					_area.ToString("000"),
					_prefix.ToString("000"),
					_suffix.ToString("0000"),
					_ext
				);
			}

			if (areaCodeFormat == PhoneNumberAreaCodeFormatStyle.UseParenthesis)
			{
				format = "({0}) {1}-{2}";
			}
			else
			{
				format = "{0}-{1}-{2}";
			}
			
			return string.Format
			(
				format,
				_area.ToString("000"),
				_prefix.ToString("000"),
				_suffix.ToString("0000")
			);
		}

		/// <summary>
		/// Returns a formatted string in the form
		/// (999) 999-9999 EXT: blah
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if ( 0 == _area && 0 == _prefix && 0 == _suffix )
			{
				return String.Empty;
			}
	
			if (_ext > 0)
			{
				return String.Format
				(
					"({0}) {1}-{2} EXT: {3}", 
					_area.ToString("000"), 
					_prefix.ToString("000"), 
					_suffix.ToString("0000"), 
					_ext
				);
			}

			return String.Format
			(
				"({0}) {1}-{2}", 
			    _area.ToString("000"), 
			    _prefix.ToString("000"), 
			    _suffix.ToString("0000")
			);
		}

		/// <summary>
		/// Phone numbers are stored as numerics in some systems.
		/// </summary>
		/// <returns></returns>
		public long ToInt()
		{
			long ret = _area * 10000000L + _prefix * 10000 + _suffix;
			Debug.Assert(ret == Int64.Parse(_area.ToString() + _prefix.ToString("000") + _suffix.ToString("0000")));

			return ret;
		}

		#region IDataErrorInfo Members

		public string Error
		{
			get
			{
				string msg;
				if ((msg = this["CountryCode"]).Length > 0)
				{
					return msg;
				}
				if ((msg = this["AreaCode"]).Length > 0)
				{
					return msg;
				}
				if ((msg = this["Prefix"]).Length > 0)
				{
					return msg;
				}
				if ((msg = this["Suffix"]).Length > 0)
				{
					return msg;
				}
				if ((msg = this["Extension"]).Length > 0)
				{
					return msg;
				}

				return String.Empty;
			}
		}

		public string this[string columnName]
		{
			get
			{
				switch (columnName)
				{
					case "CountryCode":
						if (CountryCode > 99)
						{
							return "PhoneNumberLocale.CountryCodeTooLarge";
						}
						if (CountryCode < 1)
						{
							return "PhoneNumberLocale.CountryCodeTooSmall";
						}
						break;

					case "AreaCode":
						if (AreaCode > 999)
						{
							return "PhoneNumberLocale.AreaCode3Digits";
						}
						if (0 >= AreaCode)
						{
							return "PhoneNumberLocale.GreaterThanZero";
						}
						break;

					case "Prefix":
						if (Prefix > 999)
						{
							return "PhoneNumberLocale.Prefix3Digits";
						}
						if (Prefix < 200)
						{
							return "PhoneNumberLocale.GreaterThan199";
						}
						break;

					case "Suffix":
						if (Suffix > 9999)
						{
							return "PhoneNumberLocale.Suffix4Digits";
						}
						if (Suffix <= 0)
						{
							return "PhoneNumberLocale.GreaterThanZero";
						}
						break;

					case "Extension":
						break;
				}
				return "";
			}
		}

		#endregion

		/// <summary>
		/// Phone number validation
		/// </summary>
		/// <param name="phone">The phone number to check</param>
		/// <returns>True if phone is valid.</returns>
		public static bool IsValidPhoneNumber(string phone)
		{
			if (phone.IndexOf('(') > -1 || phone.IndexOf(')') > -1)
			{
				if (StringHelper.CountOccurancesOf(phone, ')') != 1 || StringHelper.CountOccurancesOf(phone, '(') != 1)
				{
					return false;
				}
			}
			string phoneStrip = phone.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");
			return phoneStrip.Length == 10;
		}

		/// <summary>
		/// Validate a phone number from it's component parts (area code, prefix and suffix) and return an 
		/// array of error message strings.
		/// </summary>
		/// <param name="areaCode"></param>
		/// <param name="prefix"></param>
		/// <param name="suffix"></param>
		/// <param name="phoneNumberErrors"></param>
		/// <returns></returns>
		public static bool IsValidPhoneNumber
		(
			string areaCode, 
			string prefix, 
			string suffix, 
			out string[] phoneNumberErrors
		)
		{
			List<string> errorMessages = new List<string>();
			//phone numbers format

			if (!StringHelper.IsInt(areaCode) || !StringHelper.IsInt(prefix) ||
				!StringHelper.IsInt(suffix))
			{
				errorMessages.Add("PhoneNumberLocale.PhoneIsNotNumeric");
			}

			if (areaCode.Length != 3)
			{
				errorMessages.Add("PhoneNumberLocale.AreaCode3Digits");
			}

			if (prefix.Length != 3)
			{
				errorMessages.Add("PhoneNumberLocale.Prefix3Digits");
			}

			if (suffix.Length != 4)
			{
				errorMessages.Add("PhoneNumberLocale.Suffix4Digits");
			}

			if (errorMessages.Count == 0)
			{
				int number = int.Parse(areaCode);
				if (number < 200)
				{
					errorMessages.Add("PhoneNumberLocale.AreaCodeNot0or1");
				}

				number = int.Parse(prefix);
				if (number < 200)
				{
					errorMessages.Add("PhoneNumberLocale.PrefixNot0or1");
				}
			}

			phoneNumberErrors = errorMessages.ToArray();
			return errorMessages.Count == 0;
		}
	}
}
