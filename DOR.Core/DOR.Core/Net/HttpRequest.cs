using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace DOR.Core.Net
{
	public class HttpRequest
	{
		private enum State
		{
			HTTPREQ_STATE_METHOD = 0,
			HTTPREQ_STATE_URI = 1,
			HTTPREQ_STATE_VERSION = 2,
			HTTPREQ_STATE_HEADERS = 3,
			HTTPREQ_STATE_BODY = 4
		};

		public string Method
		{
			get;
			set;
		}

		public HttpUri Uri
		{
			get;
			set;
		}

		string HttpVersion
		{
			get;
			set;
		}

		public HttpHeader Header
		{
			get;
			private set;
		}

		public IHttpRequestBody Body
		{
			get;
			private set;
		}

		private State m_state = State.HTTPREQ_STATE_METHOD;
		private StringBuilder m_accum = new StringBuilder(128);

		public HttpRequest()
		{
			Method = "GET";
			Uri = new HttpUri();
			HttpVersion = "HTTP/1.0";
			Header = new HttpHeader();
			Body = null;
		}

		private bool ParseLine(byte[] data, int len, ref int pos)
		{
			while ( pos < len )
			{
				char ch = (char)data[pos++];
				if ( ch == '\r' )
				{
					continue;
				}
				if ( ch == '\n' )
				{
					return true;
				}
				m_accum.Append( ch );
			}
			return false;
		}

		public void Parse(byte[] data, int len)
		{
			int pos = 0;

			while ( pos < len )
			{
				switch ( m_state )
				{
				case State.HTTPREQ_STATE_METHOD:
					while ( pos < len )
					{
						char ch = (char)data[pos++];
						if ( ch == ' ' ) 
						{
							Method = m_accum.ToString();
							m_accum.Length = 0;
							m_state = State.HTTPREQ_STATE_URI;
							break;
						}
						m_accum.Append( ch );
						if ( m_accum.Length > 6 )
						{
							throw new Exception("Invalid HTTP method " + m_accum.ToString());
						}
					}
					break;

				case State.HTTPREQ_STATE_URI:
					while ( pos < len )
					{
						char ch = (char)data[pos++];
						if ( ch == ' ' ) 
						{
							string str = m_accum.ToString();
							Uri = HttpUri.Parse(str);
							m_accum.Length = 0;
							m_state = State.HTTPREQ_STATE_VERSION;
							break;
						}
						m_accum.Append( ch );
					}
					break;

				case State.HTTPREQ_STATE_VERSION:
					if ( ParseLine(data, len, ref pos) )
					{
						HttpVersion = m_accum.ToString();
						m_accum.Length = 0;
						m_state = State.HTTPREQ_STATE_HEADERS;
					}
					break;

				case State.HTTPREQ_STATE_HEADERS:
					if ( ParseLine(data, len, ref pos) )
					{
						if ( m_accum.Length == 0 )
						{
							m_state = State.HTTPREQ_STATE_BODY;
							break;
						}
						Header.ParseLine(m_accum.ToString());
						m_accum.Length = 0;
					}
					break;

				case State.HTTPREQ_STATE_BODY:
					if ( null == Body )
					{
						if ( Header.HasHeader("Content-Type") )
						{
							string contentType = Header.GetHeader("Content-Type");
							if ( contentType.Equals("application/x-www-form-urlencoded") )
							{
								Body = new HttpRequestBodyFormData();
							}
							else
							{
								Body = new HttpRequestBodyGeneric();
							}
						}
						else
						{
							Body = new HttpRequestBodyGeneric();
						}
					}
					int contentLen;
					if ( Header.HasHeader("Content-Length") )
					{
						string scl = Header.GetHeader("Content-Length");
						Debug.Assert( StringHelper.IsInt(scl) );
						contentLen = Int32.Parse(scl);
					}
					else
					{
						contentLen = 0;
					}

					Body.Parse( data, pos, len - pos, contentLen );
					pos = len;
					break;

				default:
					throw new Exception("HttpRequest::Parse: corrupted state.");
				}
			}
		}
		
		public bool IsComplete()
		{
			if ( Header.Count() == 0 )
			{
				return false;
			}

			int contentLen;
			if ( Header.HasHeader("Content-Length") )
			{
				string scl = Header.GetHeader("Content-Length");
				Debug.Assert( StringHelper.IsInt(scl) );
				contentLen = Int32.Parse(scl);
			}
			else
			{
				contentLen = 0;
			}

			if ( null != Body && contentLen > 0 )
			{
				return Body.ByteCount >= contentLen;
			}
			else
			{
				// request does not have a content lenth, should check Transfer-Encoding and media type "multipart/byteranges".
				return m_state == State.HTTPREQ_STATE_BODY;
			}
		}

		public HttpResponse Send()
		{
			HttpResponse resp = new HttpResponse();
	
			TcpClient sock = new TcpClient(Uri.Server, Uri.Port);
			sock.ReceiveTimeout = 30 * 1000;
			
			if ( null == Body )
			{
				Header.SetContentLength("0");
			}
			else
			{
				Header.SetContentLength(Body.ByteCount.ToString());
			}

			if (Header.ContentType() == "" )
			{
				Header.SetContentType("application/x-www-form-urlencoded");
			}

			string httpReq = ToString();
			sock.GetStream().Write(HttpUtility.ToByteArray(httpReq), 0, httpReq.Length);

			Stream reader = sock.GetStream();
			resp.Parse(reader);
			sock.Close();

			return resp;
		}

		public HttpRequest Clone()
		{
			HttpRequest req = new HttpRequest();
			req.Method = Method;
			req.Uri = Uri.Clone();
			req.m_state = m_state;
			req.HttpVersion = HttpVersion;
			req.Header = Header.Clone();

			if (Body != null)
			{
				req.Body = Body.Clone();
			}

			return req;
		}

		public override string ToString()
		{
			StringBuilder buf = new StringBuilder();

			buf.Append( Method );
			buf.Append( ' ' );
			buf.Append( Uri.AbsolutePath() );
			buf.Append( ' ' );
			buf.Append( HttpVersion );
			buf.Append( "\r\n" );
			buf.Append( Header.ToString() );

			if ( null != Body )
			{
				buf.Append( Body.ToString() );
			}

			return buf.ToString();
		}
	}
}
