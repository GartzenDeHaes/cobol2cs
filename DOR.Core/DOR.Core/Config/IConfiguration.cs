using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core.Collections;

namespace DOR.Core.Config
{
	public interface IConfiguration
	{
		int Count { get; }

		bool ContainsKey(string key);

		object DefaultValue { get; }

		object this[string key]
		{
			get;
			set;
		}

		void DefineParameter(IConfigurationParameter prm);
		void DefineParameter(string typeName, string key, object value);

		DorEnvironment DorEnvironment { get; }
		string GetBlsEnvironmentName();
		string GetEnvironmentName();

		int IntAt(string key);
		string StringAt(string key);
		double DoubleAt(string key);
		DateTime DateTimeAt(string key);
		Date DateAt(string key);

		bool HasKey(string key);

		void ParseParameters(string keyValuePairs, char delimiter);

		string ToString();
	}
}
