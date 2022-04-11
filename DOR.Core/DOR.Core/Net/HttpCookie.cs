using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core.Collections;

namespace DOR.Core.Net
{
	public class HttpCookie
	{
		public string Key
		{
			get;
			set;
		}

		public string Value
		{
			get;
			set;
		}

		public Dictionary<string, string> Attributes
		{
			get;
			private set;
		}

		public HttpCookie()
		{
			Attributes = new Dictionary<string, string>();
		}

		public HttpCookie(string key, string value, string path)
		: this()
		{
			Key = key;
			Value = value;
			Attributes.Add("path", path);
		}

		public static HttpCookie Parse(string hdr)
		{
			string[] parts = hdr.Split(new char[] {';'});
	
			if (parts.Length == 0)
			{
				return new HttpCookie();
			}
	
			int pos;
			string key;
			string value;

			if (0 > (pos = parts[0].IndexOf('=')))
			{
				key = parts[0].Trim();
				value = "";
			}
			else
			{
				key = parts[0].Substring(0, pos);
				value = parts[0].Substring(pos + 1);
			}
	
			HttpCookie cookie = new HttpCookie(HttpUtility.HtmlDecode(key), HttpUtility.HtmlDecode(value), "");
	
			for ( int x = 1; x < parts.Length; x++ )
			{
				if (0 > (pos = parts[x].IndexOf('=')))
				{
					key = parts[x].Trim();
					value = "";
				}
				else
				{
					key = parts[x].Substring(0, pos);
					value = parts[x].Substring(pos + 1);
				}
				cookie.Attributes.Add(HttpUtility.HtmlDecode(key), HttpUtility.HtmlDecode(value));
			}
	
			return cookie;
		}

		public override string ToString()
		{
			StringBuilder buf = new StringBuilder();
	
			buf.Append(HttpUtility.HtmlEncode(Key));
			buf.Append('=');
			buf.Append(HttpUtility.HtmlEncode(Value));
	
			if (Attributes.Count > 0)
			{
				buf.Append("; ");
		
				foreach(var key in Attributes.Keys)
				{
					buf.Append(HttpUtility.HtmlEncode(key));
					buf.Append('=');
					buf.Append(HttpUtility.HtmlEncode(Attributes[key]));
					buf.Append(';');
				}
			}
	
			return buf.ToString();		
		}
	}
}
