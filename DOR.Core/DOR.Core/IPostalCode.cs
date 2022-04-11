using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DOR.Core
{
	public interface IPostalCode : IDataErrorInfo, INotifyPropertyChanged
	{
		/// <summary>Base of the postal code, fe the 5-digit ZIP.</summary>
		string Base { get; set; }

		/// <summary>Extension of the postal code, fe the +4 of a ZIP.</summary>
		int Extension { get; set; }

		/// <summary>Returns true if there is an extension.</summary>
		bool HasExtension { get; }
	}
}
