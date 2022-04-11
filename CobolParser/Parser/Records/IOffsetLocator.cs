using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Records
{
	public interface IOffsetLocator
	{
		INamedField LocateField(string name, string parentName);
	}
}
