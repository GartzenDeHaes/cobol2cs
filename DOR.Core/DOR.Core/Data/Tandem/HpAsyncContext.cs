using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace DOR.Core.Data.Tandem
{
	public class HpAsyncContext : AsyncContext
	{
		public string Uri
		{
			get;
			set;
		}

		public WebRequest Request
		{
			get;
			set;
		}

		public string ParameterKey
		{
			get;
			set;
		}

		public XmlDocument Xml
		{
			get;
			set;
		}

		public IDataReaderEx Reader
		{
			get;
			set;
		}

		public bool CreateReader
		{
			get;
			private set;
		}

		public HpAsyncContext(string uri, string key, int? cacheDurationInMinutes, bool createReader)
		{
			Uri = uri;
			ParameterKey = key;
			CacheDurationInMinutes = cacheDurationInMinutes;
			CreateReader = createReader;
		}

		public HpAsyncContext(string uri, WebRequest req, string key, int? cacheDurationInMinutes, bool createReader)
		{
			Uri = uri;
			Request = req;
			ParameterKey = key;
			CacheDurationInMinutes = cacheDurationInMinutes;
			CreateReader = createReader;
		}
	}
}
