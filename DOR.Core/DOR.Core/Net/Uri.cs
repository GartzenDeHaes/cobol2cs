using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DOR.Core.Collections;

namespace DOR.Core.Net
{
	public class HttpUri
	{
		public string Protocol
		{
			get;
			set;
		}

		public string Server
		{
			get;
			set;
		}

		public int Port
		{
			get;
			set;
		}

		public string Path
		{
			get;
			set;
		}

		public string Filename
		{
			get;
			set;
		}

		public string FileExt
		{
			get;
			set;
		}

		public Vector<Association<string, string>> Args
		{
			get;
			set;
		}

		private Dictionary<string, string> m_argIdx = new Dictionary<string,string>();


		public bool HasArg(string argname)
		{ 
			return m_argIdx.ContainsKey(argname); 
		}

		public string GetArg(string argname) 
		{ 
			return m_argIdx[argname];
		}

		public HttpUri()
		{
			Protocol = "http";
			Server = "";
			Port = 80;
			Path = "";
			Filename = "";
			FileExt = "";
			Args = new Vector<Association<string,string>>();
		}

		public static HttpUri Parse( string cstr )
		{
			/// @TODO -- This parse needs work

			HttpUri uri = new HttpUri();
			string str = cstr.Replace('\\', '/');

			int pos = 0;
			int idx = str.IndexOf(':');

			if ( idx > -1 )
			{
				// protocol
				uri.Protocol = str.Substring(pos, idx).ToLower();
				pos = idx + 1;

				if ( pos >= str.Length )
				{
					return uri;
				}
				if ( str[pos++] != '/' )
				{
					throw new InvalidArgumentException("Expected ://");
				}
				if ( pos >= str.Length )
				{
					return uri;
				}
				if ( str[pos++] != '/' )
				{
					throw new InvalidArgumentException("Expected ://");
				}
				if ( pos >= str.Length )
				{
					return uri;
				}

				// server dns name
				string temp = "";

				if ( (idx = str.IndexOf(':', pos + 1)) < 0 )
				{
					idx = str.IndexOf('/', pos + 1);
				}
				if ( 0 > idx )
				{
					temp = str.Substring(pos);
					pos += temp.Length;
				}
				else
				{
					temp = StringHelper.MidStr(str, pos, idx);
					pos = idx;
				}
				uri.Server = temp;

				if ( pos >= str.Length )
				{
					return uri;
				}

				if ( str[pos] == ':' )
				{
					pos++;
					idx = str.IndexOf('/', pos + 1);
					temp = StringHelper.MidStr(str, pos, idx);
					pos += temp.Length;
					uri.Port = Int32.Parse(temp);
				}
			}
			if ( pos >= str.Length )
			{
				return uri;
			}
			string fullpath;
			if ( 0 > (idx = str.IndexOf('?', pos)) )
			{
				// no params
				fullpath = str.Substring(pos);
				pos = str.Length;
			}
			else
			{
				// params
				fullpath = StringHelper.MidStr(str, pos, idx);
				pos = idx;
			}
			if ( 0 > (idx = fullpath.IndexOf('/')) )
			{
				uri.Filename = fullpath;
			}
			else if ( fullpath.IndexOf('.') < 0 )
			{
				// there is no file name
				uri.Path = fullpath;
			}
			else
			{
				idx = fullpath.LastIndexOf("/");
				Debug.Assert(idx > -1);
				uri.Filename = (fullpath.Substring(idx + 1));
				uri.Path = fullpath.Substring(0, idx + 1);
			}

			if ((idx = uri.Filename.IndexOf('.')) > -1)
			{
				uri.FileExt = uri.Filename.Substring(idx + 1);
			}
			if ( pos >= str.Length )
			{
				return uri;
			}
			Debug.Assert( str[pos] == '?' );
			string parms = str.Substring(pos+1);
			string[] parts = parms.Split(new char[] {'&'});

			for ( int x = 0; x < parts.Length; x++ )
			{
				string part = parts[x];
				if ( 0 > (idx = part.IndexOf('=')) )
				{
					Association<string, string> pair = new Association<string,string>(HttpUtility.UrlDecode(part), "");
					uri.Args.Add(pair);
				}
				else
				{
					string key = part.Substring(0, idx);
					string val = part.Substring(idx+1);
			
					Association<string, string> pair = new Association<string,string>(HttpUtility.UrlDecode(key), HttpUtility.UrlDecode(val));
					uri.Args.Add(pair);
					uri.m_argIdx.Add(pair.Key, pair.Value);
				}
			}

			return uri;
		}

		public string AbsolutePath()
		{
			StringBuilder buf = new StringBuilder();

			if ( ! Path.StartsWith("/") )
			{
				buf.Append('/');
			}
			buf.Append( Path );
			if ( ! Path.EndsWith("/") )
			{
				buf.Append('/');
			}
			buf.Append( Filename );

			int argcount = Args.Count;
			if ( argcount > 0 )
			{
				buf.Append('?');
			}

			for ( int x = 0; x < argcount; x++ )
			{
				Association<string, string> pair = Args[x];
				if ( HttpUtility.UrlEncodeRequired(pair.Key) )
				{
					buf.Append( HttpUtility.UrlEncode(pair.Key) );
				}
				else
				{
					buf.Append( pair.Key );
				}
				buf.Append( "=" );
				if ( HttpUtility.UrlEncodeRequired(pair.Value) )
				{
					buf.Append( HttpUtility.UrlEncode(pair.Value) );
				}
				else
				{
					buf.Append( pair.Value );
				}
				if ( x + 1 < argcount )
				{
					buf.Append( "&" );
				}
			}

			return buf.ToString();
		}

		public HttpUri Clone()
		{
			HttpUri uri = new HttpUri();
			uri.FileExt = FileExt;
			uri.Filename = Filename;
			uri.Path = Path;
			uri.Port = Port;
			uri.Protocol = Protocol;
			uri.Server = Server;
			
			foreach (var a in Args)
			{
				var dup = a.Clone();
				uri.Args.Add(dup);
				uri.m_argIdx.Add(dup.Key, dup.Value);
			}

			return uri;
		}

		public override string ToString()
		{
			StringBuilder buf = new StringBuilder();
	
			if ( Server.Length > 0 )
			{
				buf.Append( Protocol );
				buf.Append( "://" );
				buf.Append( Server );
			}

			if ( Port != 80 )
			{
				buf.Append(':');
				buf.Append(Port.ToString());
			}

			buf.Append( AbsolutePath() );

			return buf.ToString();
		}
	}
}
