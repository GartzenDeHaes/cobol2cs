using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

using DOR.Core.ComponentModel;

namespace DOR.Core.Config
{
	public class DorEnvironment : NotifyPropertyChangedBase
	{
		private EnvironmentType _environmentType;
		public EnvironmentType EnvironmentType
		{
			get { return _environmentType; }
			set
			{
				if (_environmentType != value)
				{
					_environmentType = value;
					RaisePropertyChanged("EnvironmentType");
				}
			}
		}

		public DorEnvironment()
		{
		}

		public DorEnvironment(EnvironmentType et)
		{
			EnvironmentType = et;
		}

		public override string ToString()
		{
			switch (EnvironmentType)
			{
				case EnvironmentType.Test:
					return "TEST";
				case EnvironmentType.Prod:
					return "PROD";
				case EnvironmentType.Dev:
					return "DEV";
				case EnvironmentType.Demo:
					return "DEMO";
				default:
					return "UKNOWN";
			}
		}

		public static DorEnvironment Parse(string lit)
		{
			switch (lit.ToUpper())
			{
				case "UNKNOWN":
					return new DorEnvironment(EnvironmentType.Unknown);
				case "TEST":
					return new DorEnvironment(EnvironmentType.Test);
				case "PROD":
					return new DorEnvironment(EnvironmentType.Prod);
				case "DEV":
					return new DorEnvironment(EnvironmentType.Dev);
				case "DEMO":
					return new DorEnvironment(EnvironmentType.Demo);
				default:
					throw new ConfigurationException("Unknown environment " + lit);
			}
		}
	}
}
