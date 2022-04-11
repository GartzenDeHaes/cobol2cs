using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using DOR.Core.Collections;

namespace DOR.Core.Net
{
	public class HttpHeader
	{
		private Vector<Association<string, string>> m_headers = new Vector<Association<string,string>>(20);
		private Dictionary<string, int> m_headerIdx = new Dictionary<string, int>();
		private Dictionary<string, HttpCookie> m_cookies = new Dictionary<string, HttpCookie>();

		public HttpHeader Clone()
		{
			HttpHeader h = new HttpHeader();
			for (int x = 0; x < m_headers.Count; x++)
			{
				h.AddHeader(m_headers[x].Key, m_headers[x].Value);
			}

			foreach (var c in m_cookies.Values)
			{
				string path = c.Attributes.ContainsKey("path") ? c.Attributes["path"] : "";
				h.m_cookies.Add(c.Key, new HttpCookie(c.Key, c.Value, path));
			}

			return h;
		}

		public void AddHeader(string _name, string val)
		{
			string name = _name.ToUpper();
			if (!m_headerIdx.ContainsKey(name))
			{
				int idx = m_headers.Count;
				m_headers.Add(new Association<String, String>(_name, val));
				m_headerIdx.Add(name, idx);
			}
			else
			{
				m_headers[m_headerIdx[name]].Value = val;
			}
		}

		public string GetHeader(string _name)
		{
			string name = _name.ToUpper();
			if ( ! m_headerIdx.ContainsKey(name) )
			{
				int idx = m_headers.Count;
				m_headers.Add( new Association<String, String>(_name, "") );
				m_headerIdx.Add( name, idx );
			}
			return m_headers[m_headerIdx[name]].Right;
		}

		public void SetCookie(string key, string value)
		{
			HttpCookie cookie = new HttpCookie(key, value, "");
			m_cookies.Add(key, cookie);
			m_headers.Add(new Association<String, String>("Set-Cookie", cookie.ToString()));
			m_headerIdx.Add("SET-COOKIE", m_headers.Count);
		}

		public void Write(Stream stream)
		{
			string header = ToString();
			stream.Write( HttpUtility.ToByteArray(header), 0, header.Length );
		}

		public void ParseLine( string line )
		{
			int colonPos = line.IndexOf(':');
			if (0 > colonPos)
			{
				throw new InvalidArgumentException("Expected a colon " + line);
			}
			string key = line.Substring(0, colonPos);
			string val = line.Substring(colonPos + 1).Trim();
			m_headers[m_headerIdx[key]] = new Association<string,string>(key, val.Trim());

			m_headerIdx.Add(key.ToUpper(), m_headers.Count-1);
	
			if (key.Equals("Cookie", StringComparison.InvariantCultureIgnoreCase))
			{
				HttpCookie cookie = HttpCookie.Parse(val);
				m_cookies.Add(cookie.Key, cookie);
			}
		}

		public override string ToString()
		{
			StringBuilder buf = new StringBuilder();

			int headcount = m_headers.Count;
			for ( int x = 0; x < headcount; x++ )
			{
				Association<string, string> pair = m_headers[x];
				buf.Append( pair.Left );
				buf.Append( ":\t" );
				buf.Append( pair.Right );
				buf.Append( "\r\n" );	
			}

			if ( m_headers.Count > 0 )
			{
				buf.Append( "\r\n" );
			}
			else
			{
				buf.Append( "\r\n\r\n" );
			}

			return buf.ToString();
		}

		public bool HasHeader(string name)
		{ 
			return m_headerIdx.ContainsKey(name.ToUpper()); 
		}

		public bool HasCookie(string name)
		{
			return m_cookies.ContainsKey(name);
		}
	
		public HttpCookie GetCookie(string key)
		{
			return m_cookies[key];
		}

		public string ContentLength()
		{ 
			return GetHeader("CONTENT-LENGTH"); 
		}

		public void SetContentLength(string value)
		{
			AddHeader("CONTENT-LENGTH", value);
		}

		public string ContentType()
		{ 
			return GetHeader("CONTENT-TYPE"); 
		}

		public void SetContentType(string value)
		{
			AddHeader("CONTENT-TYPE", value);
		}

		public int Count() 
		{ 
			return m_headers.Count; 
		}
	}
}
