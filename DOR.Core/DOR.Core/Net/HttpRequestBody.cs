using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using DOR.Core.Collections;

namespace DOR.Core.Net
{
	public class HttpRequestBodyGeneric : IHttpRequestBody
	{
		StringBuilder m_buf = new StringBuilder();

		public string GetValue(string key)
		{
			return "";
		}

		public bool HasKey(string key)
		{
			return false;
		}

		public int ByteCount
		{
			get { return m_buf.Length; }
		}

		public IHttpRequestBody Clone()
		{
			HttpRequestBodyGeneric req = new HttpRequestBodyGeneric();
			for (int x = 0; x < m_buf.Length; x++)
			{
				req.m_buf.Append(m_buf[x]);
			}

			return req;
		}

		public void Parse(byte[] cp, int pos, int len, int contentLen)
		{
			for (int x = pos; x < len; x++)
			{
				m_buf.Append((char)cp[x]);
			}
		}

		public void Write(Stream strm)
		{
			strm.Write(HttpUtility.ToByteArray(m_buf.ToString()), 0, m_buf.Length);
		}
	}

	public class HttpRequestBodyFormData : IHttpRequestBody
	{
		private enum State
		{
			HTTPBODY_STATE_NAME = 0,
			HTTPBODY_STATE_VAL = 1
		};

		private Vector<Association<string, string>> m_data = new Vector<Association<string,string>>();
		private Dictionary<string, string> m_idx = new Dictionary<string,string>();
		private int m_byteCount;

		private State m_state = State.HTTPBODY_STATE_NAME;
		private StringBuilder m_accum = new StringBuilder();

		public Association<string, string> Item(int idx) 
		{ 
			return m_data[idx]; 
		}

		public int ByteCount
		{
			get { return m_byteCount; }
		}

		public HttpRequestBodyFormData()
		{
		}

		public IHttpRequestBody Clone()
		{
			HttpRequestBodyFormData req = new HttpRequestBodyFormData();

			for (int x = 0; x < m_data.Count; x++)
			{
				req.m_data.Add(m_data[x].Clone());
			}

			foreach(var key in m_idx.Keys)
			{
				req.m_idx.Add(key, m_idx[key]);
			}

			req.m_byteCount = m_byteCount;
			req.m_state = m_state;

			for (int x = 0; x < m_accum.Length; x++)
			{
				req.m_accum.Append(m_accum[x]);
			}

			return req;
		}

		public string GetValue(string key)
		{
			return m_idx[key];
		}

		public bool HasKey(string key)
		{
			return m_idx.ContainsKey(key);
		}

		public void Parse( byte[] cp, int pos, int len, int contentLen )
		{
			m_byteCount += len;
			string key = "";

			for ( int x = 0; x < len; x++ )
			{
				char ch = (char)cp[x+pos];

				switch ( m_state )
				{
				case State.HTTPBODY_STATE_NAME:
					if ( '=' == ch || '&' == ch || '\0' == ch )
					{
						key = m_accum.ToString();
						m_data.Add( new Association<string, string>(key, "") );
						m_idx.Add(key, m_data.Peek().Value);
						m_accum.Length = 0;
						m_state = State.HTTPBODY_STATE_VAL;
						break;
					}

					m_accum.Append( ch );
					break;

				case State.HTTPBODY_STATE_VAL:
					if ( ch == '&' || ch == '\0' || ch == '\r' || ch == '\n' )
					{
						m_data.Peek().Value = m_accum.ToString();
						if (m_data.Peek() != null)
						{
							m_idx.Add(m_data.Peek().Key, m_data.Peek().Value);
						}
						m_accum.Length = 0;
						m_state = State.HTTPBODY_STATE_NAME;
						break;
					}

					m_accum.Append( ch );
					break;

				default:
					throw new Exception("HttpRequestBody::Parse: state corrupted");
				}
			}

			if ( m_state == State.HTTPBODY_STATE_VAL )
			{
				m_data.Peek().Value = m_accum.ToString();
				if (m_data.Peek() != null)
				{
					m_idx.Add(m_data.Peek().Key, m_data.Peek().Value);
				}
				m_accum.Length = 0;
				m_state = State.HTTPBODY_STATE_NAME;
			}
		}

		public void Write( Stream strm )
		{
			int count = m_data.Count;
			for ( int x = 0; x < count; x++ )
			{
				string key = m_data[x].Key;
				string val = m_data[x].Value;

				if ( x > 0 )
				{
					strm.WriteByte((byte)'&');
				}

				if ( HttpUtility.UrlEncodeRequired(key) )
				{
					string enc = HttpUtility.UrlEncode(key);
					strm.Write( HttpUtility.ToByteArray(enc), 0, enc.Length );
				}
				else
				{
					strm.Write(HttpUtility.ToByteArray(key), 0, key.Length);
				}
				strm.WriteByte((byte)'=');
				if ( HttpUtility.UrlEncodeRequired(val) )
				{
					string enc = HttpUtility.UrlEncode(val);
					strm.Write(HttpUtility.ToByteArray(enc), 0, enc.Length);
				}
				else
				{
					strm.Write(HttpUtility.ToByteArray(val), 0, val.Length);
				}
			}
		}

		public override string ToString()
		{
			MemoryStream ms = new MemoryStream();
			Write( ms );

			StringBuilder buf = new StringBuilder();

			while (ms.CanRead)
			{
				buf.Append((char)ms.ReadByte());
			}
			return buf.ToString();
		}
	}
}
