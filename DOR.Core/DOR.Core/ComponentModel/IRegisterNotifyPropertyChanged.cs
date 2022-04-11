using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core.ComponentModel
{
	public interface IRegisterNotifyPropertyChanged
	{
		void RegisterNotifyPropertyChanged(IRaisePropertyChanged inpc, string propName);
		void RaisePropertyChanged();
		void RaisePropertyChanged(string propertyName);
	}
}
