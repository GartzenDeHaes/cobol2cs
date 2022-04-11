using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace DOR.Core.Config
{
	public class Configuration : IConfiguration
	{
		private Dictionary<string, IConfigurationParameter> _idx = new Dictionary<string, IConfigurationParameter>();
		private IConfigurationParameter _defaultValue;

		#region Properties

		public int Count 
		{
			get { return _idx.Count; }
		}

		public object DefaultValue
		{
			get { return null == _defaultValue ? null : _defaultValue.Value; }
			set
			{
				if (null == _defaultValue)
				{
					throw new InvalidCastException("No default value");
				}

				_defaultValue.Value = value;
			}
		}

		public object this[string key]
		{
			get
			{
				key = key.ToUpper();
				if (! HasKey(key))
				{
					throw new ConfigurationException("Invalid screen parameter of " + key);
				}
				return _idx[key].Value;
			}
			set
			{
				key = key.ToUpper();
				if (!HasKey(key))
				{
					throw new ConfigurationException("Invalid screen parameter of " + key);
				}

				if (key != CommonConfigurationKeys.DorEnvironment)
				{
					_idx[key].Value = value;
				}
				else
				{
					if (null == _dorEnvironment)
					{
						if 
						(
							! String.IsNullOrWhiteSpace((string)_idx[key].Value) &&
							(string)_idx[key].Value != "UNKNOWN"
						)
						{
							value = _idx[key].Value;
						}
						_dorEnvironment = DorEnvironment.Parse(value.ToString());
					}
					else
					{
						_dorEnvironment.EnvironmentType = DorEnvironment.Parse(value.ToString()).EnvironmentType;
					}

					_idx[key].Value = value;

					if (_dorEnvironment.EnvironmentType == EnvironmentType.Prod)
					{
						this[CommonConfigurationKeys.TandemDnsName] = "192.209.32.8";
					}
					else if (_dorEnvironment.EnvironmentType == EnvironmentType.Demo)
					{
						this[CommonConfigurationKeys.TandemDnsName] = "192.209.32.8:1028";
					}
					else if 
					(
						_dorEnvironment.EnvironmentType == EnvironmentType.Test ||
						_dorEnvironment.EnvironmentType == EnvironmentType.Dev
					)
					{
						this[CommonConfigurationKeys.TandemDnsName] = "192.209.32.8:1027";
					}
					else
					{
						this[CommonConfigurationKeys.TandemDnsName] = "";
					}
				}
			}
		}

		private DorEnvironment _dorEnvironment;
		public DorEnvironment DorEnvironment
		{
			get
			{
				if (null == _dorEnvironment)
				{
					if (HasKey(CommonConfigurationKeys.DorEnvironment))
					{
						_dorEnvironment = DorEnvironment.Parse((string)this[CommonConfigurationKeys.DorEnvironment]);
					}
					else
					{
						_dorEnvironment = new DorEnvironment();
					}
				}
				return _dorEnvironment;
			}
			set
			{
				_dorEnvironment = value;
				this[CommonConfigurationKeys.DorEnvironment] = _dorEnvironment.ToString();
			}
		}

		#endregion

		#region C'tors

		public Configuration()
		{
			DefineParameter("int", CommonConfigurationKeys.ScreenCode, null);
			DefineParameter("int", CommonConfigurationKeys.NextScreenCode, null);
		}

		public Configuration(IList<ConfigurationParameter> prms)
		: this()
		{
			foreach(ConfigurationParameter p in prms)
			{
				DefineParameter(p);
			}
		}

		public void ParseParameters(string commandLine)
		{
			ParseParameters(commandLine, ' ');
		}

		/// <summary>
		/// Command line string constructor
		/// </summary>
		/// <param name="session">The session</param>
		/// <param name="commandLine">Must start with the screen number, for example "105 tra=123123123</param>
		public void ParseParameters
		(
			string keyValuePairs,
			char delimiter
		)
		{
			int pos = keyValuePairs.IndexOf(delimiter);
			if (0 > pos)
			{
				pos = keyValuePairs.Length;
			}

			string screenSpec = keyValuePairs.Substring(0, pos);
			if (StringHelper.IsInt(screenSpec))
			{
				keyValuePairs = keyValuePairs.Substring(pos).Trim();
				this[CommonConfigurationKeys.ScreenCode] = Int32.Parse(screenSpec);
			}

			List<string> parts = new List<string>();
			
			int start = 0;
			bool inquote = false;

			for (int x = 0; x < keyValuePairs.Length; x++)
			{
				char ch = keyValuePairs[x];
				if (ch == '"')
				{
					if (inquote)
					{
						parts.Add(keyValuePairs.Substring(start, x));
						start = x + 1;
					}
					else
					{
						inquote = true;
					}
					continue;
				}

				if (ch == delimiter)
				{
					parts.Add(keyValuePairs.Substring(start, x));
					start = x + 1;
					continue;
				}
			}

			if (start < keyValuePairs.Length)
			{
				parts.Add(keyValuePairs.Substring(start));
			}

			foreach (string s in parts)
			{
				pos = s.IndexOf('=');
				if (0 > pos)
				{
					if (null == _defaultValue)
					{
						throw new IndexOutOfRangeException(s + " does not have a value (should be " + s + " + =avalue)");
					}
					_defaultValue.Value = s;
					continue;
				}

				string key = s.Substring(0, pos).ToUpper();
				string val = s.Substring(pos + 1);

				this[key] = val;
			}
		}

		public void DefineParameter(IConfigurationParameter prm)
		{
			_idx.Add(prm.Name, prm.Clone());

			if (prm.IsDefault)
			{
				_defaultValue = _idx[prm.Name];
			}
		}

		public void DefineParameter(string typeName, string key, object value)
		{
			key = key.ToUpper();

			if (_idx.ContainsKey(key))
			{
				_idx[key].Value = value;
				return;
			}

			ConfigurationParameter p = new ConfigurationParameter
			(
				key,
				false,
				"",
				false,
				typeName,
				value
			);
			_idx.Add(key, p);
		}

		public void Parse(IDictionary<string, string> configs)
		{
			foreach (var k in configs.Keys)
			{
				DefineParameter("string", k, configs[k]);
			}

			if (ContainsKey("MACHINENAME"))
			{
				ConfigureMachine(StringAt("MACHINENAME"));
			}
		}

		#endregion

		public bool ContainsKey(string key)
		{
			return _idx.ContainsKey(key.ToUpper());
		}

		public string GetBlsEnvironmentName()
		{
			switch (DorEnvironment.EnvironmentType)
			{
				case EnvironmentType.Dev:
					return "LOCAL";
				case EnvironmentType.Test:
					return "DEV";
				case EnvironmentType.Demo:
					return "QA";
				case EnvironmentType.Prod:
					return "PROD";
				default:
					throw new ConfigurationException("Internal error with BLS environment type");
			}
		}

		public string GetEnvironmentName()
		{
			switch (DorEnvironment.EnvironmentType)
			{
				case EnvironmentType.Dev:
					return "DEV";
				case EnvironmentType.Test:
					return "TEST";
				case EnvironmentType.Demo:
					return "DEMO";
				case EnvironmentType.Prod:
					return "PROD";
				default:
					throw new ConfigurationException("Internal error with environment type");
			}
		}

		public string GetBlsConfigSetting(string baseKey)
		{
			return StringAt(baseKey + "." + GetBlsEnvironmentName());
		}

		public string GetDorConfigSetting(string baseKey)
		{
			return StringAt(baseKey + "." + GetEnvironmentName());
		}

		#region Typed Value Accessors

		public int IntAt(string key)
		{
			return _idx[key.ToUpper()].ToInt();
		}

		public string StringAt(string key)
		{
			key = key.ToUpper();
			if (!_idx.ContainsKey(key))
			{
				throw new ConfigurationException(key + " not found");
			}
			return _idx[key].ToString();
		}

		public double DoubleAt(string key)
		{
			return _idx[key.ToUpper()].ToDouble();
		}

		public DateTime DateTimeAt(string key)
		{
			return _idx[key.ToUpper()].ToDateTime();
		}

		public Date DateAt(string key)
		{
			return _idx[key.ToUpper()].ToDate();
		}

		public bool HasKey(string key)
		{
			return _idx.ContainsKey(key.ToUpper());
		}

		#endregion

		#region Parse & ToString

		public override string ToString()
		{
			StringBuilder buf = new StringBuilder();

			foreach (var key in _idx.Keys)
			{
				IConfigurationParameter prm = _idx[key];
				if (null == prm.Value)
				{
					continue;
				}

				string val = prm.Value.ToString();
				if (val.IndexOf(' ') > -1)
				{
					buf.Append('"');
				}
				buf.Append(key);
				buf.Append('=');
				buf.Append(prm.Value.ToString());
				if (val.IndexOf(' ') > -1)
				{
					buf.Append('"');
				}
				buf.Append(' ');
			}

			return buf.ToString().TrimEnd();
		}

		#endregion

		#region Static Singleton

		public static Configuration AppConfig
		{
			get;
			private set;
		}

		static Configuration()
		{
			AppConfig = new Configuration();

			AppConfig.DefineParameter("string", CommonConfigurationKeys.DorUserId, 0);
			AppConfig.DefineParameter("string", CommonConfigurationKeys.DorEnvironment, "UNKNOWN");
			AppConfig.DefineParameter("string", CommonConfigurationKeys.TandemDnsName, "");

#if ! SILVERLIGHT

			foreach (string p in ConfigurationManager.AppSettings.AllKeys)
			{
				AppConfig.DefineParameter("string", p.ToUpper(), ConfigurationManager.AppSettings[p]);
			}

			for (int x = 0; x < ConfigurationManager.ConnectionStrings.Count; x++)
			{
				AppConfig.DefineParameter("string", ConfigurationManager.ConnectionStrings[x].Name.ToUpper(), ConfigurationManager.ConnectionStrings[x].ConnectionString);
			}

			if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["MACHINENAME"]))
			{
				ConfigureMachine(ConfigurationManager.AppSettings["MACHINENAME"]);
			}
			else
			{
				ConfigureMachine(Environment.MachineName);
			}
#endif
		}

		private static void ConfigureMachine(string machineName)
		{
			machineName = machineName.ToUpper();

			if (machineName.StartsWith("DORWK") || machineName == "LOCALHOST")
			{
				AppConfig[CommonConfigurationKeys.DorEnvironment] = EnvironmentType.Dev;
			}
			else if (_prodBoxen.IndexOf(machineName) > -1)
			{
				AppConfig[CommonConfigurationKeys.DorEnvironment] = EnvironmentType.Prod;
			}
			else if (_demoBoxen.IndexOf(machineName) > -1)
			{
				AppConfig[CommonConfigurationKeys.DorEnvironment] = EnvironmentType.Demo;
			}
			else if (_testBoxen.IndexOf(machineName) > -1)
			{
				AppConfig[CommonConfigurationKeys.DorEnvironment] = EnvironmentType.Test;
			}
			else
			{
				throw new ConfigurationException("Unconfigured machine " + machineName);
			}
		}

		#endregion

		private static readonly string _prodBoxen =
			"DORAPTUM1001P|" +
			"DORPRODELFWEB1|" +
			"DORPRODELFWEB2|" +
			"DORPRODWF1|" +
			"DORPDORWF2|" +
			"DORPRODEFILEA|" +
			"DORPRODEFILEB|" +
			"DORPRODBATCH|" +
			"DORPRODSFTP|" +
			"DORPRODEFILE1|" +
			"DORPRODEFILE2|" +
			"DORAPPSPROD|" +
			"DORAPPSSVCPROD|" +
			"DORWEBSPHRPROD|" +
			"DORPRODBATCH|" +
			"DORPRODSFTP|" +
			"DORPRODKOFAX|" +
			"DORAPPSSVCPROD1|" +
			"DORAPPSSVCPROD2|" +
			"DORWEBPROD1|" +
			"DORWEBPROD2|" +
			"DORXNETPROD1|" +
			"DORXNETPROD2|" +
			"DORJOBSCDPROD1|" +
			"DORJOBSCDPROD2|" +
			"DORJOBSPCLPROD1|" +
			"DORJOBSPCLPROD2|" +
			"DORJOBAGTPROD1|" +
			"DORJOBAGTPROD2|" +
			"DORWEBSVCPROD1|" +
			"DORWEBSVCPROD2|" +
			"DORAPPSPROD1|" +
			"DORAPPSPROD2|" +
			"DORXNETPROD1|" +
			"DORXNETPROD2|" +
			"DORBLSPROD1|" +
			"DORBLSPROD2|";

		private static readonly string _demoBoxen =
			"DORAPTUM0011D|" +
			"DORDEMOELF|" +
			"DORDEVCMS|" +
			"DORDEVBATCH|" +
			"DORDEMOSFTP|" +
			"DORAPPSDEMO|" +
			"DORAPPSSVCDEMO|" +
			"DORWEBSPHRDEMO|" +
			"DORDEMOBATCH|" +
			"DORDEMOKOFAX|" +
			"DORAPPSSVCDEMO1|" +
			"DORAPPSSVCDEMO2|" +
			"DORWEBDEMO1|" +
			"DORWEBDEMO2|" +
			"DORWEBPROD1|" +
			"DORWEBPROD2|" +
			"DORJOBSCDDEMO1|" +
			"DORJOBSCDDEMO2|" +
			"DORJOBSPCLDEMO1|" +
			"DORJOBSPCLDEMO2|" +
			"DORJOBAGTDEMO1|" +
			"DORJOBAGTDEMO2|" +
			"DORWEBSVCDEMO1|" +
			"DORWEBSVCDEMO2|" +
			"DORAPPSDEMO1|" +
			"DORAPPSDEMO2|" +
			"DORXNETDEMO|" +
			"DORXNETDEMO1|" +
			"DORXNETDEMO2|" +
			"DORBLSDEMO1|" +
			"DORBLSDEMO2|";

		private static readonly string _testBoxen =
			"DORAPTUM0101T|" +
			"DORTESTELF|" +
			"DORAPPSTEST|" +
			"DORAPPSTEMP|" +
			"DORAPPSSVCTEST|" +
			"DORWEBSPHRTEST|" +
			"DORDEVBATCH|" +
			"DORDEVSFTP|" +
			"DORTESTKOFAX|" +
			"DORAPPSSVCTEST|" +
			"DORWEBTEST|" +
			"DORJOBSCDTEST|" +
			"DORJOBSPCLTEST|" +
			"DORJOBAGTTEST|" +
			"DORWEBSVCTEST|" +
			"DORXNETTEST|" +
			"DORBLSTEST|";
	}
}
