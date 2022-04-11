using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core.Config
{
	public class ConfigurationException : Exception
	{
		public ConfigurationException()
		: base()
		{
		}

		public ConfigurationException(string msg)
		: base(msg)
		{
		}
	}
}
