using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core.Data.Tandem
{
	/// <summary>
	/// Used with TandemDataAccessBase.
	/// </summary>
	/// <example>
	///	XmlDocument doc = CallUri
	///	(
	///		"elf.TraCreditLiab",
	///		new TandemParameter[] 
	///		{
	///			BuildParameter("action", "inquire"),
	///			BuildParameter("what", "liabilities"),
	///			BuildParameter("tra", tra.ToString("000000000"))
	///		}
	///	);
	/// </example>
	public class TandemParameter
	{
		private string m_key;
		private string m_value;

		public TandemParameter(string key, string value)
		{
			m_key = key;
			m_value = value;
		}

		public string Key
		{
			get { return m_key; }
		}

		public string Value
		{
			get { return m_value; }
		}

		public static TandemParameter[] Append(TandemParameter[] lst, params TandemParameter[] prms)
		{
			List<TandemParameter> l = new List<TandemParameter>(lst);
			for (int x = 0; x < prms.Length; x++)
			{
				l.Add(prms[x]);
			}
			return l.ToArray();
		}

		public static TandemParameter[] Create(string key, string value)
		{
			return new TandemParameter[] { new TandemParameter(key, value) };
		}
	}
}
