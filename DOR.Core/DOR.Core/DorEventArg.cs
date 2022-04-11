using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core
{
	public class DorEventArg
	{
		public DorEventArg(string key, object val)
		{
			Key = key;
			Value = val;
		}

		public DorEventArg(string key)
		{
			Key = key;
		}

		public string Key
		{
			get;
			private set;
		}

		public object Value
		{
			get;
			private set;
		}
	}
}
