using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace DOR.Core
{
	/// <summary>
	/// This address is defined a little differently from DOR.Auth.Address.
	/// </summary>
	[Serializable]
	public class StreetAddress : IPostalAddress
	{
		#region Members

		private string m_addressLine1;
		private string m_city;
		private string m_state;
		private readonly IPostalCode m_zip;

		#endregion Members

		#region Properties
		/// <summary>
		/// Address Line 1
		/// </summary>
		public string Line1
		{
			get { return m_addressLine1; }
		}
		
		/// <summary>
		/// Address City
		/// </summary>
		public string City
		{
			get { return m_city; }
		}
		/// <summary>
		/// Address State
		/// </summary>
		public string State
		{
			get { return m_state; }
		}

		/// <summary>
		/// Address Zip (full)
		/// </summary>
		public IPostalCode ZIP
		{
			get { return m_zip; }
		}

		/// <summary>
		/// Indicates that the Address has been run through USPS standardization.
		/// </summary>
		public bool IsStandardized
		{
			get;
			set;
		}

		#endregion Properties

		#region C'tors
		/// <summary>
		/// Construct from another <see cref="StreetAddress"/>
		/// </summary>
		/// <param name="address"></param>
		public StreetAddress(StreetAddress address)
		{
			Init(address.Line1, address.City, address.m_state);
			m_zip = PostalCode.Parse(address.ZIP.Base, address.ZIP.Extension);
		}
		/// <summary>
		/// Initialize from address line, city, state & string zip
		/// </summary>
		/// <param name="addressLine1"></param>
		/// <param name="city"></param>
		/// <param name="state"></param>
		/// <param name="zip"></param>
		public StreetAddress(string addressLine1, string city, string state, string zip)
		{
			Init(addressLine1, city, state);
			m_zip = PostalCode.Parse(zip);
		}

		/// <summary>
		/// Initialize from address line, city, state & ZIP object
		/// </summary>
		/// <param name="addressLine1"></param>
		/// <param name="city"></param>
		/// <param name="state"></param>
		/// <param name="zip"></param>
		public StreetAddress(string addressLine1, string city, string state, IPostalCode zip)
		{
			Init(addressLine1, city, state);
			m_zip = zip;
		}

		/// <summary>
		/// Initialize from address line, city, state, zip & zip4 strings
		/// </summary>
		/// <param name="addressLine1"></param>
		/// <param name="city"></param>
		/// <param name="state"></param>
		/// <param name="zip5"></param>
		/// <param name="zipdash4"></param>
		public StreetAddress(string addressLine1, string city, string state, string zip5, string zipdash4)
		{
			Init(addressLine1, city, state);
			if (zipdash4.Length > 0)
			{
				if (!StringHelper.IsNumeric(zipdash4))
				{
					throw new FormatException("ZIP plus four must be numeric (" + addressLine1 + "," + city + "," + zip5 + "-" + zipdash4 + ")");
				}
			}
			if (zipdash4.Length > 4)
			{
				throw new FormatException("ZIP four cannot be longer than four digits (" + zipdash4 + ")");
			}
			if ((string.IsNullOrEmpty(city)) && !StringHelper.IsNumeric(zip5))
			{
				throw new FormatException("ZIP code must be numeric");
			}
			if (zip5.Length == 0 && (string.IsNullOrEmpty(city)))
			{
				throw new FormatException("Please enter a ZIP code");
			}
			if (zipdash4.Length > 0)
			{
				m_zip = new ZIP(Int32.Parse(zip5), Int32.Parse(zipdash4));
			}
			else
			{
				m_zip = zip5.Length > 0 ? new ZIP(Int32.Parse(zip5)) : new ZIP(0);
			}
		}
		#endregion C'tors

		#region Public Methods

		/// <summary>Returns the address in a single line format</summary>
		public override string ToString()
		{
			return m_addressLine1 + " " + m_city + ", " + m_zip.ToString();
		}

		/// <summary>
		/// Required override
		/// </summary>
		public override bool Equals(object o)
		{
			var otherAddress = o as StreetAddress;
			if (otherAddress != null)
			{
				return otherAddress.m_zip.ToString() == m_zip.ToString() &&
						otherAddress.m_state == m_state &&
						otherAddress.m_addressLine1 == m_addressLine1 &&
						otherAddress.m_city == m_city;
			}
			return false;
		}

		/// <summary>
		/// Required override
		/// </summary>
		public override int GetHashCode()
		{
			return m_zip.GetHashCode();
		}

		#endregion Public Methods

		#region Validation

		/// <summary>
		/// Returns true if the address is valid or the ZIP+4 is valid, but not both.  This
		/// is used by GIS to only do a ZIP+4 lookup if an address is not entered.
		/// </summary>
		public static bool IsAddressXorZipPlus4Valid
		(
			string addressLine1, 
			string city, 
			string state, 
			string zip5, 
			string zipdash4, 
			out string[] errorMessages
		)
		{
			if (!IsAddressValid(addressLine1, city, state, zip5, zipdash4, out errorMessages))
			{
				if (PostalCode.IsPostalCode(zip5, zipdash4))
				{
					return (string.IsNullOrEmpty(addressLine1) || addressLine1.IndexOf(' ') < 0) &&
					       /*(null == city || city.Length == 0) && */
					       (null == state || state.Length == 2);
				}
				return false;
			}
			return true;
		}

		/// <summary>
		/// Validation for data from a web page.
		/// </summary>
		/// <param name="errorMessages">Error messages for the validation summary.</param>
		public static bool IsAddressValid
		(
			string addressLine1, 
			string city, 
			string state, 
			string zip5, 
			string zipdash4, 
			out string[] errorMessages
		)
		{
			if (!IsAddressValid(addressLine1, city, state, zip5, out errorMessages))
			{
				return false;
			}
			// if there is a zip extension, check it now.
			if (zipdash4 != null && zipdash4.Trim().Length > 0)
			{
				if (!PostalCode.IsPostalCode(zip5, zipdash4))
				{
					errorMessages = new string[] { "StreetAddressLocale.InvalidZIP4" };
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Validation for data from a web page.
		/// </summary>
		/// <param name="errorMessages">Error messages for the validation summary.</param>
		public static bool IsAddressValid
		(
			string addressLine1, 
			string city, 
			string state, 
			string zip5, 
			out string[] errorMessages
		)
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			//StreetAddressLocalizer localizer = new StreetAddressLocalizer();

			if (string.IsNullOrEmpty(addressLine1) || addressLine1.IndexOf(' ') < 0)
			{
				messages.Add("StreetAddressLocale.StreetAddressRequired");
				isValid = false;
			}
			if (string.IsNullOrEmpty(state))
			{
				messages.Add("StreetAddressLocale.StateIsRequired");
				isValid = false;
			}
			if (state == null || state.Length != 2)
			{
				messages.Add("StreetAddressLocale.InvalidState");
				isValid = false;
			}
			if (string.IsNullOrEmpty(city))
			{
				messages.Add("StreetAddressLocale.CityIsRequired");
				isValid = false;
			}
			if (string.IsNullOrEmpty(zip5))
			{
				messages.Add("StreetAddressLocale.ZipIsRequired");
				isValid = false;
			}
			if (! PostalCode.IsPostalCode(zip5))
			{
				messages.Add("StreetAddressLocale.InvalidZIP");
				isValid = false;
			}

			errorMessages = messages.ToArray();
			return isValid;
		}

		#endregion Public Static Methods

		#region Private Methods

		private void Init(string addressLine1, string city, string state)
		{
			if (state.Length != 2)
			{
				throw new FormatException("Expected two digit state (" + state + ")");
			}
			m_addressLine1 = addressLine1;
			m_city = city;
			m_state = state;
		}

		#endregion Private Methods
	}
}