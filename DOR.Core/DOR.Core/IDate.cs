using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

//using DOR.Core.Data;

namespace DOR.Core
{
	public interface IDate : IComparable, INotifyPropertyChanged, IDataErrorInfo
	{
		int Year { get; set; }
		int Month { get; set; }
		int Day { get; set; }

		int YearTwoDigit { get; }
		int Quarter { get; }
		bool IsLeapYear { get; }
		bool IsWeekend { get; }
		bool IsHoliday { get; }
		string MonthAbbr { get; }
		DayOfWeek DayOfWeek { get; }
	}
}
