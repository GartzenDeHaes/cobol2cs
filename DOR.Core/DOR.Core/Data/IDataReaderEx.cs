using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DOR.Core.Data
{
	public interface IDataReaderEx : IDataReader
	{
		decimal GetDecimal(string f);
		short GetInt16(string f);
		int GetInt32(string f);
		string GetString(string f);
		bool IsDBNull(string f);
		DateTime GetDateTime(string f);
		char GetChar(string f);

		DataSet ToDataSet();

		string GetDataTypeName(string colName);
	}
}
