using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DOR.Core.Net
{
	public class HttpResponse
	{
		private enum State
		{
			HTTPRES_STATE_VERSION = 0,
			HTTPRES_STATE_CODE = 1,
			HTTPRES_STATE_MESSAGE = 2,
			HTTPRES_STATE_HEADERS = 3,
			HTTPRES_STATE_BODY = 4
		};

		public string HttpVersion
		{
			get;
			set;
		}

		public int StatusCode
		{
			get;
			set;
		}

		public HttpHeader Header
		{
			get;
			private set;
		}

		public MemoryStream Body
		{
			get;
			private set;
		}

		public long ContentLength
		{
			get { return Body.Length; }
		}

		private StringBuilder m_accum = new StringBuilder();
		private State m_state = State.HTTPRES_STATE_VERSION;

		public HttpResponse()
		{
			Body = new MemoryStream();
			HttpVersion = "HTTP/1.0";
			StatusCode = 200;
			Header = new HttpHeader();
		}

		public void AddHeader(string header, string value)
		{
			Header.AddHeader(header, value);
		}

		public void Redirect(string location)
		{
			StatusCode = 302;
			Header.AddHeader("Location", location);
		}

		public void Write(Stream strm)
		{
			StringBuilder buf = new StringBuilder();

			Header.AddHeader("Content-Length", ContentLength.ToString());

			buf.Append(HttpVersion);
			buf.Append(' ');
			buf.Append(StatusCode.ToString());
			buf.Append(' ');
			buf.Append(GetResponseText(StatusCode));
			buf.Append("\r\n");
			buf.Append(Header.ToString());

			strm.Write(HttpUtility.ToByteArray(buf.ToString()), 0, buf.Length);

			while (Body.CanRead)
			{
				strm.WriteByte((byte)Body.ReadByte());
			}
		}

		public void Parse( Stream strm )
		{
			int chb = strm.ReadByte();
			byte[] chbuf = new byte[1];

			while ( chb > 0 )
			{
				chbuf[0] = (byte)chb;
				Parse( chbuf, 1 );

				if ( State.HTTPRES_STATE_BODY == m_state )
				{
					break;
				}

				chb = strm.ReadByte();
			}

			byte[] kbuf = new byte[256];
			int count;
			while ((count = strm.Read(kbuf, 0, kbuf.Length)) > 0)
			{
				Parse(kbuf, count);
			}
		}

		private void Parse(byte[] data, int len)
		{
			if ( m_state == State.HTTPRES_STATE_BODY )
			{
				Body.Write( data, 0, len );
				return;
			}

			for ( int x = 0; x < len; x++ )
			{
				char ch = (char)data[x];

				switch ( m_state )
				{
				case State.HTTPRES_STATE_VERSION:
					if ( ch == ' ' )
					{
						HttpVersion = m_accum.ToString();
						m_accum.Length = 0;
						m_state = State.HTTPRES_STATE_CODE;
						break;
					}
					m_accum.Append( ch );
					break;

				case State.HTTPRES_STATE_CODE:
					if ( ch == ' ' )
					{
						StatusCode = Int32.Parse(m_accum.ToString());
						m_accum.Length = 0;
						m_state = State.HTTPRES_STATE_MESSAGE;
						break;
					}
					m_accum.Append( ch );
					break;

				case State.HTTPRES_STATE_MESSAGE:
					if ( ch == '\r' )
					{
						break;
					}
					if ( ch == '\n' )
					{
						m_accum.Length = 0;
						m_state = State.HTTPRES_STATE_HEADERS;
						break;
					}
					m_accum.Append( ch );
					break;

				case State.HTTPRES_STATE_HEADERS:
					if ( ch == '\r' )
					{
						break;
					}
					if ( ch == '\n' )
					{
						if ( m_accum.Length == 0 )
						{
							m_state = State.HTTPRES_STATE_BODY;
						}
						else
						{
							string headerLine = m_accum.ToString();
							Header.ParseLine( headerLine );
							m_accum.Length = 0;
						}
						break;
					}
					else
					{
						m_accum.Append( ch );
					}
					break;

				case State.HTTPRES_STATE_BODY:
					Body.WriteByte( (byte)ch );
					break;

				default:
					throw new Exception("Corrupted state in HttpResponse::Parse");
				}
			}
		}

		public static string GetResponseText(int responseCode)
		{
			switch (responseCode)
			{
				case 100: return "Continue";
				case 101: return "Switching Protocols";
				case 102: return "Processing";
				case 200: return "OK";
				case 201: return "Created";
				case 202: return "Accepted";
				case 203: return "Non-Authoritative Information";
				case 204: return "No Content";
				case 205: return "Reset Content";
				case 206: return "Partial Content";
				case 207: return "Multi-Status";
				case 300: return "Multiple Choices";
				case 301: return "Moved Permanently";
				case 302: return "Found";
				case 303: return "See Other";
				case 304: return "Not Modified";
				case 305: return "Use Proxy";
				case 307: return "Temporary Redirect";
				case 400: return "Bad Request";
				case 401: return "Unauthorized";
				case 402: return "Payment Required";
				case 403: return "Forbidden";
				case 404: return "Not Found";
				case 405: return "Method Not Allowed";
				case 406: return "Not Acceptable";
				case 407: return "Proxy Authentication Required";
				case 408: return "Request Timeout";
				case 409: return "Conflict";
				case 410: return "Gone";
				case 411: return "Length Required";
				case 412: return "Precondition Failed";
				case 413: return "Request Entity Too Large";
				case 414: return "Request-Uri Too Long";
				case 415: return "Unsupported Media Type";
				case 416: return "Requested Range Not Satisfiable";
				case 417: return "Expectation Failed";
				case 422: return "Unprocessable Entity";
				case 423: return "Locked";
				case 424: return "Failed Dependency";
				case 500: return "Internal Server Error";
				case 501: return "Not Implemented";
				case 502: return "Bad Gateway";
				case 503: return "Service Unavailable";
				case 504: return "Gateway Timeout";
				case 505: return "Http Version Not Supported";
				case 507: return "Insufficient Storage";
				default:
					return "Unknown";
			}
		}
	}
}
