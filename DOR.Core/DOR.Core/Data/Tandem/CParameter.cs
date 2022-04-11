using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DOR.Core.Net;

namespace DOR.Core.Data.Tandem
{
	public interface ICParameter
	{
		string Key
		{
			get;
		}

		string ValueString
		{
			get;
		}

		void Set(Packet pkt);
	}

	public class CParameter<T> : ICParameter
	{
		private string m_key;
		private T m_value;

		public CParameter(string key, T value)
		{
			m_key = key;
			m_value = value;
		}

		public string Key
		{
			get { return m_key; }
		}

		public T Value
		{
			get { return m_value; }
		}

		public string ValueString
		{
			get { return m_value.ToString(); }
		}

		public static ICParameter[] Append(ICParameter[] lst, params ICParameter[] prms)
		{
			List<ICParameter> l = new List<ICParameter>(lst);
			for (int x = 0; x < prms.Length; x++)
			{
				l.Add(prms[x]);
			}
			return l.ToArray();
		}

		public static ICParameter[] Create(string key, T value)
		{
			return new ICParameter[] { (ICParameter)(new CParameter<T>(key, value)) };
		}

		public void Set(Packet pkt)
		{
			pkt.Append(m_key);
			pkt.Append((object)m_value);
		}
	}
}
