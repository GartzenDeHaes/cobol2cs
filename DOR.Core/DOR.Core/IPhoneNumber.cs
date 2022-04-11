using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DOR.Core
{
	public interface IPhoneNumber : IDataErrorInfo, INotifyPropertyChanged
	{
		int CountryCode { get; set; }

		int AreaCode { get; set; }

		int Prefix { get; set; }

		int Suffix { get; set; }

		int Extension { get; set; }

		/// <summary>Phone numbers are stored as numerics in some systems.</summary>
		long ToInt();
	}
}
