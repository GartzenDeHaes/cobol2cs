using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core.ComponentModel
{
	public interface IRaisePropertyChanged
	{
		void RaisePropertyChanged(string name);
	}
}
