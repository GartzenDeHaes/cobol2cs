using System;

namespace DOR.Core.Config
{
	public interface IConfigurationParameter
	{
		IConfigurationParameter Clone();

		string DefaultValue { get; set; }
		bool IsDefault { get; set; }
		string Name { get; set; }
		bool Required { get; set; }
		DOR.Core.Date ToDate();
		DateTime ToDateTime();
		double ToDouble();
		int ToInt();
		string ToString();
		string TypeName { get; set; }
		object Value { get; set; }
	}
}
