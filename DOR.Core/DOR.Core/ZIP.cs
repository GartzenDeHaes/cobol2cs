using System;

using DOR.Core.ComponentModel;

namespace DOR.Core
{
	/// <summary>
	/// Interface for both US and foriegn postal codes
	/// </summary>
	[Serializable]
	public abstract class PostalCode : NotifyPropertyChangedBase, IPostalCode
	{
		/// <summary>Base of the postal code, fe the 5-digit ZIP.</summary>
		public string Base 
		{ 
			get { return PostalCodeBase; }
			set
			{
				PostalCodeBase = value;
				RaisePropertyChanged("Base");
			}
		}

		/// <summary>Extension of the postal code, fe the +4 of a ZIP.</summary>
		public int Extension 
		{ 
			get { return PostalCodeExtension; }
			set 
			{ 
				PostalCodeExtension = value;
				RaisePropertyChanged("Extension");
			}
		}

		/// <summary>Returns true if there is an extension.</summary>
		public bool HasExtension 
		{ 
			get { return PostalCodeHasExtension; } 
		}

		/// <summary>Base of the postal code, fe the 5-digit ZIP.</summary>
		public abstract string PostalCodeBase
		{
			get;
			set;
		}

		/// <summary>Extension of the postal code, fe the +4 of a ZIP.</summary>
		public abstract int PostalCodeExtension
		{
			get;
			set;
		}

		/// <summary>Returns true if there is an extension.</summary>
		public abstract bool PostalCodeHasExtension 
		{ 
			get; 
		}

		/// <summary>Ensure sub-classes return the formated postal code.</summary>
		public abstract override string ToString();

		#region IDataErrorInfo Members

		public abstract string Error
		{
			get;
		}

		public abstract string this[string columnName]
		{
			get;
		}

		#endregion

		/// <summary>
		/// Can parse UK, CA, and US postal codes.
		/// </summary>
		/// <param name="code">Postal code</param>
		public static IPostalCode Parse(string code)
		{
			if (CanadianPostalCode.IsCaPostalCode(code))
			{
				return CanadianPostalCode.Parse(code);
			}
			if (UkPostalCode.IsUkPostalCode(code))
			{
				return UkPostalCode.Parse(code);
			}
			return ZIP.Parse(code);
		}

		/// <summary>
		/// Can parse UK, CA, and US postal codes.
		/// </summary>
		/// <param name="code">Postal code</param>
		public static IPostalCode Parse(string code, string extension)
		{
			if (string.IsNullOrEmpty(extension))
			{
				return Parse(code);
			}
			return ZIP.Parse(code, extension);
		}

		/// <summary>
		/// Can parse UK, CA, and US postal codes.
		/// </summary>
		/// <param name="code">Postal code</param>
		public static IPostalCode Parse(string code, int extension)
		{
			if (0 == extension)
			{
				return Parse(code);
			}
			return ZIP.Parse(code, extension);
		}

		/// <summary>
		/// Returns true if code is a valid UK, CA, or US postal codes.
		/// </summary>
		/// <param name="code">Postal code</param>
		public static bool IsPostalCode(string code)
		{
			return CanadianPostalCode.IsCaPostalCode(code) || ZIP.IsZip(code) || UkPostalCode.IsUkPostalCode(code);
		}

		/// <summary>
		/// Returns true if code is a valid UK, CA, or US postal codes.
		/// </summary>
		/// <param name="code">Postal code</param>
		public static bool IsPostalCode(string code, string extension)
		{
			return ((null == extension || extension.Length == 0) && CanadianPostalCode.IsCaPostalCode(code))
			       || ZIP.IsZip(code, extension);
		}
	}

	/// <summary>
	/// http://en.wikipedia.org/wiki/Canadian_postal_code
	/// </summary>
	[Serializable]
	public class CanadianPostalCode : PostalCode
	{
		private string _code;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="code">CA postal code</param>
		public CanadianPostalCode(string code)
		{
			_code = code;
		}

		/// <summary>Base of the postal code, fe the 5-digit ZIP.</summary>
		public override string PostalCodeBase
		{
			get { return _code; }
			set 
			{ 
				_code = value;
				RaisePropertyChanged("PostalCodeBase");
			}
		}

		/// <summary>CA postal codes do not have extensions.</summary>
		public override int PostalCodeExtension
		{
			get { return 0; }
			set
			{
				throw new ArgumentException("Canadian postal codes do not have extensions");
			}
		}

		/// <summary>Returns true if there is an extension.</summary>
		public override bool PostalCodeHasExtension
		{
			get { return false; }
		}

		/// <summary>Return the formated postal code.</summary>
		public override string ToString()
		{
			return _code;
		}

		#region IDataErrorInfo Members

		public override string Error
		{
			get
			{
				return ValidationHelper.ErrorCheckProperties(this);
			}
		}

		public override string this[string columnName]
		{
			get
			{
				switch (columnName)
				{
					case "PostalCodeBase":
						if (! IsCaPostalCode(_code))
						{
							return "Invalid Canadian postal code";
						}
						break;
				}

				return String.Empty;
			}
		}

		#endregion
	
		/// <summary>
		/// Returns true if code is a valid CA postal code.
		/// </summary>
		public static bool IsCaPostalCode(string code)
		{
			int spaceCtn = StringHelper.CountOccurancesOf(code, ' ');
			if (spaceCtn > 1)
			{
				code = code.Trim();
			}
			if (code.Length < 6)
			{
				return false;
			}

			int pos = 0;
			if (!Char.IsLetter(code[pos++]))
			{
				return false;
			}
			if (!Char.IsDigit(code[pos++]))
			{
				return false;
			}
			if (!Char.IsLetter(code[pos++]))
			{
				return false;
			}
			while (pos < code.Length && code[pos] == ' ')
			{
				pos++;
			}
			if (pos + 3 > code.Length)
			{
				return false;
			}
			if (!Char.IsDigit(code[pos++]))
			{
				return false;
			}
			if (!Char.IsLetter(code[pos++]))
			{
				return false;
			}
			if (!Char.IsDigit(code[pos]))
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Parse the postal code.
		/// </summary>
		/// <exception cref="ArgumentException">Throws an exception if the code is not valid.</exception>
		/// <param name="code"></param>
		/// <returns></returns>
		public new static CanadianPostalCode Parse(string code)
		{
			if (!IsCaPostalCode(code))
			{
				throw new ArgumentException("Invalid postal code " + code);
			}
			if (code.Length == 7 && code[3] == ' ')
			{
				return new CanadianPostalCode(code);
			}
			code = code.Replace(" ", "");
			if (code.Length < 6)
			{
				throw new ArgumentException("Invalid postal code " + code);
			}
			return new CanadianPostalCode(code.Substring(0, 3) + " " + code.Substring(3));
		}
	}

	/// <summary>
	/// http://en.wikipedia.org/wiki/UK_postcodes
	/// </summary>
	[Serializable]
	public class UkPostalCode : PostalCode
	{
		private string _code;

		/// <summary>
		/// Construtor
		/// </summary>
		/// <param name="code">UK postal code</param>
		private UkPostalCode(string code)
		{
			_code = code;
		}

		/// <summary>Base of the postal code, fe the 5-digit ZIP.</summary>
		public override string PostalCodeBase
		{
			get { return _code; }
			set
			{
				_code = value;
				RaisePropertyChanged("PostalCodeBase");
			}
		}

		/// <summary>UK postal codes do not have extensions.</summary>
		public override int PostalCodeExtension
		{
			get { return 0; }
			set
			{
				throw new ArgumentException("UK postal codes do not have extensions");
			}
		}

		/// <summary>Always false.</summary>
		public override bool PostalCodeHasExtension
		{
			get { return false; }
		}

		/// <summary>Return the formated postal code.</summary>
		public override string ToString()
		{
			return _code;
		}

		#region IDataErrorInfo Members

		public override string Error
		{
			get
			{
				return ValidationHelper.ErrorCheckProperties(this);
			}
		}

		public override string this[string columnName]
		{
			get
			{
				switch (columnName)
				{
					case "PostalCodeBase":
						if (!IsUkPostalCode(_code))
						{
							return "Invalid UK postal code";
						}
						break;
				}

				return String.Empty;
			}
		}

		#endregion
	
		/// <summary>
		/// Returns true if code is a valid UK postal code.
		/// </summary>
		public static bool IsUkPostalCode(string code)
		{
			int spaceCtn = StringHelper.CountOccurancesOf(code, ' ');
			if (spaceCtn != 1)
			{
				return false;
			}

			int spacePos = code.IndexOf(' ');
			if (!Char.IsLetter(code[0]))
			{
				return false;
			}
			if (!Char.IsDigit(code[spacePos + 1]))
			{
				return false;
			}
			if (!Char.IsLetter(code[code.Length-1]))
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Parses the postal code.
		/// </summary>
		/// <exception cref="ArgumentException">Throws an exception if the code is not valid.</exception>
		public new static UkPostalCode Parse(string code)
		{
			if (!IsUkPostalCode(code))
			{
				throw new ArgumentException("Invalid postal code " + code);
			}
			return new UkPostalCode(code);
		}
	}

	/// <summary>
	/// A ZIP + 4
	/// </summary>
	[Serializable]
	public class ZIP : PostalCode
	{
		private static readonly char[] _dashArray = new char[] {'-'};
		private int _plus4;
		private int _zip;

		#region Properties

		/// <summary></summary>
		public int Zip5
		{
			get { return _zip; }
			set { _zip = value; }
		}

		/// <summary></summary>
		public int Plus4
		{
			get { return _plus4; }
			set { _plus4 = value; }
		}

		/// <summary></summary>
		public bool HasPlus4
		{
			get { return 0 != _plus4; }
		}

		/// <summary>Base of the postal code, fe the 5-digit ZIP.</summary>
		public override string PostalCodeBase
		{
			get { return _zip.ToString("00000"); }
			set
			{
				_zip = Int32.Parse(value);
			}
		}

		/// <summary>Extension of the postal code, fe the +4 of a ZIP.</summary>
		public override int PostalCodeExtension
		{
			get
			{
				return Plus4;
			}
			set
			{
				Plus4 = value;
				RaisePropertyChanged("PostalCodeExtension");
			}
		}

		/// <summary>Returns true if there is an extension.</summary>
		public override bool PostalCodeHasExtension
		{
			get { return HasPlus4; }
		}

		#endregion

		#region C'tors

		public ZIP()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="zip">Can be 5 or 9 digits</param>
		/// <exception cref="ArgumentException">Throws an exception if the zip is not valid.</exception>
		public ZIP(int zip)
		{
			if (zip > 99999)
			{
				_zip = zip/10000;
				_plus4 = zip - _zip * 10000;
				if (_zip > 99999)
				{
					throw new ArgumentException("Invalid zip code " + zip);
				}
				if (_plus4 > 9999)
				{
					throw new ArgumentException("Invalid zip code " + zip);
				}
			}
			else
			{
				_zip = zip;
				_plus4 = 0;
			}
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="zip">0-99999</param>
		/// <param name="plus4">0-9999</param>
		/// <exception cref="ArgumentException">Throws an exception if the zip is not valid.</exception>
		public ZIP(int zip, int plus4)
		{
			_zip = zip;
			_plus4 = plus4;
			if (_zip > 99999)
			{
				throw new ArgumentException("Invalid zip code " + zip);
			}
			if (_plus4 > 9999)
			{
				throw new ArgumentException("Invalid zip code " + zip);
			}
		}

		#endregion

		/// <summary>Returns a 9 digit zip.</summary>
		public int ToInt()
		{
			return _zip * 10000 + _plus4;
		}

		/// <summary>If there is a plus 4, this returns 00000-0000.  Otherwise, 00000 is returned.</summary>
		public override string ToString()
		{
			if (0 != _plus4)
			{
				return _zip.ToString("00000") + "-" + _plus4.ToString("0000");
			}
			return _zip.ToString("00000");
		}

		/// <summary>If there is a plus 4, this returns 000000000.  Otherwise, 00000 is returned.</summary>
		public string ToShortString()
		{
			if (0 != _plus4)
			{
				return _zip.ToString("00000") + _plus4.ToString("0000");
			}
			return _zip.ToString("00000");
		}

		#region IDataErrorInfo Members

		public override string Error
		{
			get
			{
				return ValidationHelper.ErrorCheckProperties(this);
			}
		}

		public override string this[string columnName]
		{
			get
			{
				switch (columnName)
				{
					case "Zip5":
						if (_zip > 99999 || _zip < 1)
						{
							return "Invalid ZIP code";
						}
						break;

					case "Plus4":
						if (_plus4 > 9999 || _plus4 < 0)
						{
							return "Invalid plus 4";
						}
						break;
				}

				return String.Empty;
			}
		}

		#endregion
	
		/// <summary>
		/// Parse the ZIP
		/// </summary>
		/// <param name="zip5">ZIP</param>
		/// <param name="zipPlus4">+4</param>
		/// <exception cref="ArgumentException">An exception is thrown if the ZIP is invalid.</exception>
		/// <returns></returns>
		public new static ZIP Parse(string zip5, string zipPlus4)
		{
			if (null == zipPlus4 || zipPlus4.Length == 0)
			{
				return Parse(zip5);
			}
			if (zip5.IndexOf(' ') > -1)
			{
				zip5 = zip5.Trim();
			}
			if (zipPlus4.IndexOf(' ') > -1)
			{
				zipPlus4 = zipPlus4.Trim();
			}
			if (zip5.Length == 5 && zipPlus4.Length == 0)
			{
				return Parse(zip5);
			}
			if (zip5.Length > 5 || zipPlus4.Length > 4)
			{
				throw new ArgumentException("Invalid ZIP code " + zip5 + "-" + zipPlus4);
			}
			if (!StringHelper.IsNumeric(zip5) || (!StringHelper.IsNumeric(zipPlus4) && zipPlus4 != string.Empty))
			{
				throw new ArgumentException("Invalid ZIP code " + zip5 + "-" + zipPlus4);
			}
			if (zipPlus4 == string.Empty)
			{
				return new ZIP(Int32.Parse(zip5));
			}

			return new ZIP(Int32.Parse(zip5), Int32.Parse(zipPlus4));
		}

		/// <summary>
		/// Parse a 5 or 9 digit ZIP.
		/// </summary>
		/// <param name="zip">The ZIP</param>
		/// <exception cref="ArgumentException">An exception is thrown if the ZIP is invalid.</exception>
		public new static ZIP Parse(string zip)
		{
			int idx;
			if (zip.IndexOf(' ') > -1)
			{
				zip = zip.Replace(" ", "");
			}

			if ((idx = zip.IndexOf('-')) > -1)
			{
				return Parse(zip.Substring(0, idx), zip.Substring(idx + 1));
			}

			if (zip.Length == 9)
			{
				return Parse(zip.Substring(0, 5), zip.Substring(5, 4));
			}

			if (zip.Length == 0)
			{
				return new ZIP(0);
			}

			if (!StringHelper.IsNumeric(zip))
			{
				throw new ArgumentException("Invalid ZIP code " + zip);
			}

			return new ZIP(Int32.Parse(zip));
		}

		/// <summary>
		/// Returns true if the ZIP can be parsed.
		/// </summary>
		/// <param name="zip">ZIP code</param>
		/// <param name="plus4">+4</param>
		public static bool IsZip(string zip, string plus4)
		{
			if (zip.Length != 5)
			{
				return false;
			}
			if (StringHelper.CountOccurancesOf(zip, '-') > 1)
			{
				return false;
			}
			if (zip.IndexOf('-') > -1)
			{
				zip = zip.Replace("-", "");
			}
			if (null == plus4 || plus4.Length == 0)
			{
				return false;
			}
			if (null == zip)
			{
				return false;
			}
			if (zip.IndexOf(' ') > -1)
			{
				zip = zip.Trim();
			}
			if (plus4.IndexOf(' ') > -1)
			{
				plus4 = plus4.Trim();
			}
			if (!IsZip(zip))
			{
				return false;
			}
			return StringHelper.IsNumeric(zip) && zip.Length < 6 && 
					plus4 != null && plus4.Length < 5 &&
					(StringHelper.IsNumeric(plus4) || plus4.Length == 0);
		}

		/// <summary>
		/// Returns true if the ZIP can be parsed.
		/// </summary>
		/// <param name="zip">5 or 9 digit ZIP</param>
		public static bool IsZip(string zip)
		{
			if (zip.Trim().Length < 5)
			{
				return false;
			}
			if (StringHelper.CountOccurancesOf(zip, '-') > 1)
			{
				return false;
			}
			if (zip.IndexOf('-') > -1)
			{
				zip = zip.Replace("-", "");
			}
			if (zip.IndexOf(' ') > -1)
			{
				zip = zip.Trim();
			}
			int dashpos = zip.IndexOf('-');
			if (dashpos > -1)
			{
				string[] parts = zip.Split(_dashArray);
				if (parts.Length != 2)
				{
					return false;
				}
				return IsZip(parts[0], parts[1]);
			}
			if (zip.Length > 5)
			{
				return IsZip(zip.Substring(0, 5), zip.Substring(5));
			}
			return zip.Length < 6 && StringHelper.IsNumeric(zip);
		}
	}
}