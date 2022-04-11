using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Caching;
using System.Text;
using System.Web;
using System.Xml;

using DOR.Core.Config;

namespace DOR.Core.Data.Tandem
{
	/// <summary>
	/// Provides a similar interface as MsSqlDataAccessBase for Tandem servlet calls.  Use as an
	/// inner class if both Tandem and MSSQL data access is required.
	/// </summary>
	public class TandemDataAccessBase : IDataAccess, IDisposable
	{
		/// Contains the name of the servlet that accepts the full domain and user name.
		//protected const string CommandLineServletName = "cl.CommandLine";
		protected const string CommandLineServletName = "clweb.CommandLine";

		protected IConfiguration Config
		{
			get;
			set;
		}

		public string ConnectionString
		{
			get { return ServletDirectoryName; }
			set { ServletDirectoryName = value; }
		}

		public TandemDataAccessBase(IConfiguration config)
		{
			Config = config;
			ServletDirectoryName = "servlet";
		}

		public TandemDataAccessBase()
		: this(Configuration.AppConfig)
		{
		}

		public virtual string ServletDirectoryName
		{
			get;
			set;
		}

		protected virtual string BaseUrl
		{
			get
			{
				string tandemUrl = "tandem8";
				if (! Config.ContainsKey(CommonConfigurationKeys.TandemDnsName))
				{
					throw new DOR.Core.Config.ConfigurationException("Set " + CommonConfigurationKeys.TandemDnsName + " in the configuration");
				}

				tandemUrl = (string)Config[CommonConfigurationKeys.TandemDnsName];

				if (tandemUrl.IndexOf("/servlet") > -1)
				{
					throw new ArgumentException("Do not append '/servlet' to TANDEM_DNS in the config file.");
				}

				Debug.Assert(!tandemUrl.EndsWith("/"));

				if (! String.IsNullOrEmpty(ServletDirectoryName))
				{
					tandemUrl += "/" + ServletDirectoryName;
				}

				tandemUrl = StringHelper.EnsureTrailingChar(tandemUrl, '/');

				return "http://" + tandemUrl;
			}
		}

		public static void CheckServerAdvisoryReply(XmlDocument doc, out string replyCode)
		{
			XmlNode node = doc.DocumentElement;
			XmlNode replyNode = node.SelectSingleNode("ReplyCode");
			
			replyCode = "0";

			if (null != (replyNode = node.SelectSingleNode("error")))
			{
				replyCode = replyNode.InnerText;
			}
			else
			{
				replyCode = replyNode.InnerText;
			}

			if (replyCode.Equals("0"))
			{
				return;
			}

			if (null != (node = node.SelectSingleNode("AdvisoryMessage")))
			{
				replyCode = node.InnerText;
			}
		}

		/// <summary>
		/// Checks the xml string that is sent to make sure it has a reply code of "0"
		/// </summary>
		/// <param name="XML">Possibly XML, maybe some sort of error message.</param>
		/// <param name="ServletName">Servlet name for error messages.</param>
		public static XmlDocument XMLIsErrorFree(string xml, string servletName)
		{
			if (String.IsNullOrWhiteSpace(xml))
			{
				throw new DataAccessException("no data returned calling the " + servletName + " program " + xml);
			}

			//if the string returned is not in an XML format, then no connection to the Tandem
			//could be made.
			if (xml[0] == '<' && xml[1] == 'H' && xml[2] == '1' && xml[3] == '>')
			{
				throw new DataAccessException("H1 server received calling the " + servletName + " program " + xml);
			}

			if (xml[0] != '<')
			{
				//If the connection to the Tandem cannot be made, a message is returned from the C# TandemServlet
				//function. 
				throw new DataAccessException("XML string could not be read (" + xml + ")");
			}

			try
			{
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(xml);

				// If servlet error then throw exception
				if (doc.DocumentElement.SelectSingleNode("//error") != null)
				{
					throw new DataAccessException(servletName + " Error: " + doc.DocumentElement.SelectSingleNode("//error").InnerText);
				}

				return doc;
			}
			catch (Exception e)
			{
				throw new DataAccessException(e.ToString() + "  XML String:" + xml);
			}
		}
	
		/// <summary>
		/// Some servlets return invalid XML.  This allows sub classes to correct the problem
		/// prior to parsing.
		/// </summary>
		/// <param name="xml">The XML from the tandem call.</param>
		/// <returns>Valid XML.</returns>
		protected virtual string PreprocessXml(string xml)
		{
			return xml;
		}

		/// <summary>
		/// Converts a date from Tandem into a CLR DateTime
		/// </summary>
		/// <param name="objDT">An SQL object from tandem.</param>
		/// <returns>A DateTime that represents the input.</returns>
		public static DateTime ConvertTandemDateToDateTime(object objDT)
		{
			if (objDT.GetType() == typeof(int))
			{
				int intDateTime = (int)objDT;
				if (intDateTime > 0)
				{
					string strDateTime = ((int)objDT).ToString("####/##/##");
					if (strDateTime.EndsWith("00"))
					{
						strDateTime = strDateTime.Substring(0, strDateTime.Length - 1) + "1";
					}
					return DateTime.Parse(strDateTime);
				}
				else
				{
					return DateTime.MinValue;
				}
			}
			else if (objDT.GetType() == typeof(DateTime))
			{
				return (DateTime)objDT;
			}
			else if (objDT is DBNull)
			{
				return DateTime.MinValue;
			}
			else if (objDT is string)
			{
				string sobjDT = (string)objDT;

				if (String.IsNullOrWhiteSpace(sobjDT))
				{
					return DateTime.MinValue;
				}

				if (sobjDT.Length == 8 && StringHelper.IsInt(sobjDT))
				{
					if (sobjDT[6] == '0' && sobjDT[7] == '0')
					{
						sobjDT = sobjDT.Substring(0, 6) + "01";
					}
					return ConvertTandemDateToDateTime(Int32.Parse(sobjDT));
				}

				if (sobjDT == "0")
				{
					return DateTime.MinValue;
				}

				return DateTime.Parse(sobjDT.Replace('-', '/'));
			}
			else
			{
				throw new ArgumentException(objDT.GetType().ToString());
			}
		}

		/// <summary>
		/// Tandem has invalid data for some of these, so try to make it valid.
		/// </summary>
		/// <param name="revint"></param>
		/// <returns></returns>
		public static Date ForceRevIntDateValid(int revint)
		{
			if (0 == revint)
			{
				return null;
			}

			int year = revint / 10000;
			int mo = (revint - (year * 10000)) / 100;
			int day = (revint - (year * 10000)) - (mo * 100);

			if (year < 1900)
			{
				if (year < 50)
				{
					year += 2000;
				}
				else
				{
					year += 1900;
				}
			}

			if (mo == 0)
			{
				mo = 1;
			}
			else if (mo > 12)
			{
				mo = 12;
			}

			if (day == 0)
			{
				day = 1;
			}
			else if (day > 31)
			{
				day = 31;
			}

			try
			{
				return new Date(year, mo, day);
			}
			catch (Exception)
			{
			}

			Date dt = new Date(year, mo, 1);
			dt = dt.AddMonths(1);
			return dt.AddDays(-1);
		}

		public static string ConvertDateTimeToTandemYearToDay(DateTime? dtm)
		{
			if (null == dtm || !dtm.HasValue)
			{
				return "0001-01-01:00:00:00.000";
			}
			return dtm.Value.ToString("yyyy'-'MM'-'dd':'HH':'mm':'ss.fff");
		}

		public static string ConvertDateTimeToRevInt(DateTime? dtm)
		{
			if (null == dtm || !dtm.HasValue)
			{
				return "00000000";
			}
			return dtm.Value.ToString("yyyyMMdd");
		}

		protected TandemParameter BuildParameter(string key, string value)
		{
			return new TandemParameter(key, value);
		}

		protected TandemParameter BuildParameter(string key, int value)
		{
			return new TandemParameter(key, value.ToString());
		}

		protected TandemParameter BuildParameter(string key, decimal value)
		{
			return new TandemParameter(key, value.ToString());
		}

		protected TandemParameter BuildParameter(string key, double value)
		{
			return new TandemParameter(key, value.ToString());
		}

		protected TandemParameter BuildParameter(string key, float value)
		{
			return new TandemParameter(key, value.ToString());
		}

		protected TandemParameter BuildParameter(string key, Money value)
		{
			return new TandemParameter(key, value.ToString());
		}

		protected TandemParameter BuildParameter(string key, Date value)
		{
			return new TandemParameter(key, value.ToString());
		}

		protected TandemParameter BuildParameter(string key, DateTime value)
		{
			return new TandemParameter(key, value.ToString());
		}

		private string ParameterKey
		(
			string servletName,
			string methodPostOrGet,
			string uriParameterPrefix,
			TandemParameter[] uriParameters,
			TandemParameter[] httpHeaders,
			TandemParameter[] postData
		)
		{
			StringBuilder buf = new StringBuilder();
			buf.Append(servletName);
			buf.Append("&");
			buf.Append(methodPostOrGet);
			buf.Append("&");
			buf.Append(uriParameterPrefix);
			buf.Append("&");

			if (uriParameters != null)
			{
				for (int x = 0; x < uriParameters.Length; x++)
				{
					buf.Append(uriParameters[x].Key);
					buf.Append("=");
					buf.Append(uriParameters[x].Value);
					buf.Append("&");
				}
			}
			if (httpHeaders != null)
			{
				for (int x = 0; x < httpHeaders.Length; x++)
				{
					buf.Append(httpHeaders[x].Key);
					buf.Append("=");
					buf.Append(httpHeaders[x].Value);
					buf.Append("&");
				}
			}
			if (postData != null)
			{
				for (int x = 0; x < postData.Length; x++)
				{
					buf.Append(postData[x].Key);
					buf.Append("=");
					buf.Append(postData[x].Value);
					buf.Append("&");
				}
			}
			return buf.ToString();
		}

		private WebRequest StartCallTandem
		(
			string servletName,
			string methodPostOrGet,
			string uriParameterPrefix,
			TandemParameter[] uriParameters,
			TandemParameter[] httpHeaders,
			TandemParameter[] postData
		)
		{
			StringBuilder url = new StringBuilder(BaseUrl, 255);
			url.Append(servletName);

			if (!String.IsNullOrEmpty(uriParameterPrefix) || (null != uriParameters && uriParameters.Length > 0))
			{
				url.Append('?');
				url.Append(uriParameterPrefix);
			}

			if (null != uriParameters && uriParameters.Length > 0)
			{
				for (int x = 0; x < uriParameters.Length; x++)
				{
					if (x > 0)
					{
						url.Append('&');
					}
					url.Append(StringHelper.UriEncode(uriParameters[x].Key));
					url.Append('=');
					url.Append(StringHelper.UriEncode(uriParameters[x].Value));
				}
			}

			WebRequest request = WebRequest.Create(url.ToString());
			request.Method = methodPostOrGet;

			if (httpHeaders != null && httpHeaders.Length > 0)
			{
				NameValueCollection headers = new NameValueCollection();
				for (int x = 0; x < httpHeaders.Length; x++)
				{
					headers.Add(httpHeaders[x].Key, httpHeaders[x].Value);
				}

				request.Headers.Add(headers);
			}

			if (null != postData && postData.Length > 0)
			{
				request.ContentType = "application/x-www-form-urlencoded; charset=utf-8";

				char[] escapeChars = new char[] { '&', '=', '\b', '\t', '\v', '\n', '\f', '\r', '"', '\\', '+', '%' };
				StringBuilder buf = new StringBuilder();
				for (int x = 0; x < postData.Length; x++)
				{
					if (x > 0)
					{
						buf.Append('&');
					}
					buf.Append(postData[x].Key);
					buf.Append('=');
					if (postData[x].Value.IndexOfAny(escapeChars) > -1)
					{
						buf.Append(StringHelper.HttpPostFormatString(postData[x].Value));
					}
					else
					{
						buf.Append(postData[x].Value);
					}
				}

				request.ContentLength = buf.Length;

				StreamWriter stm = new StreamWriter(request.GetRequestStream());
				stm.Write(buf.ToString());
				stm.Close();
				stm.Dispose();
			}

			return request;
		}

		protected XmlDocument CheckCache
		(
			string servletName,
			string methodPostOrGet,
			string uriParameterPrefix,
			TandemParameter[] uriParameters,
			TandemParameter[] httpHeaders,
			TandemParameter[] postData,
			int? cacheDurationInMinutes,
			out string parameterKey
		)
		{
			parameterKey = null;

			if (cacheDurationInMinutes.HasValue && cacheDurationInMinutes != 0)
			{
				parameterKey = ParameterKey(servletName, methodPostOrGet, uriParameterPrefix, uriParameters, httpHeaders, postData);

				ObjectCache cache = MemoryCache.Default;
				var cached = (XmlDocument)cache.Get(parameterKey);
				if (cached != null)
				{
					return cached;
				}
			}

			return null;
		}

		protected void ClearCache
		(
			string servletName,
			string methodPostOrGet,
			string uriParameterPrefix,
			TandemParameter[] uriParameters,
			TandemParameter[] httpHeaders,
			TandemParameter[] postData
		)
		{
			var parameterKey = ParameterKey(servletName, methodPostOrGet, uriParameterPrefix, uriParameters, httpHeaders, postData);
			MemoryCache.Default.Remove(parameterKey);
		}

		/// <summary>
		/// Call a tandem servlet using TANDEM_URL from the config file.
		/// </summary>
		/// <param name="servletName">Fully qualified servlet name.</param>
		/// <param name="methodPostOrGet">POST or GET</param>
		/// <param name="uriParameterPrefix">Prefix to go after the ? (like xml=").</param>
		/// <param name="uriParameters">URI query string parameters, can be null.</param>
		/// <param name="httpHeaders">HTTP headers (security uses these), can be null.</param>
		/// <param name="postData">POST HTTP body parameters, can be null.</param>
		/// <returns></returns>
		protected XmlDocument CallTandem
		(
			string servletName,
			string methodPostOrGet,
			string uriParameterPrefix,
			TandemParameter[] uriParameters,
			TandemParameter[] httpHeaders,
			TandemParameter[] postData,
			int? cacheDurationInMinutes
		)
		{
			string parameterKey;
			XmlDocument doc = CheckCache(servletName, methodPostOrGet, uriParameterPrefix, uriParameters, httpHeaders, postData, cacheDurationInMinutes, out parameterKey);
			if (doc != null)
			{
				return doc;
			}

			WebRequest request = StartCallTandem(servletName, methodPostOrGet, uriParameterPrefix, uriParameters, httpHeaders, postData);
			Stream respStream = request.GetResponse().GetResponseStream();
			
			return EndCallTandem(respStream, request.RequestUri.ToString(), parameterKey, cacheDurationInMinutes);
		}

		protected string DownloadTextFile(string filename)
		{
			WebRequest request = StartCallTandem(filename, "GET", null, null, null, null);
			Stream respStream = request.GetResponse().GetResponseStream();

			using (StreamReader read = new StreamReader(respStream))
			{
				string txt = read.ReadToEnd();
				
				read.Close();
				respStream.Close();
				respStream.Dispose();

				return txt;
			}
		}

		protected IAsyncResult BeginCallTandem
		(
			string servletName,
			string methodPostOrGet,
			string uriParameterPrefix,
			TandemParameter[] uriParameters,
			TandemParameter[] httpHeaders,
			TandemParameter[] postData,
			bool returnReader = false,
			int? cacheDurationInMinutes = null
		)
		{
			string parameterKey = null;

			if (cacheDurationInMinutes.HasValue && cacheDurationInMinutes > 0)
			{
				parameterKey = ParameterKey(servletName, methodPostOrGet, uriParameterPrefix, uriParameters, httpHeaders, postData);

				ObjectCache cache = MemoryCache.Default;
				XmlDocument doc = (XmlDocument)cache.Get(parameterKey);
				if (doc != null)
				{
					var ctxch = new HpAsyncContext(servletName, (WebRequest)null, parameterKey, cacheDurationInMinutes, returnReader);
					ctxch.Xml = doc;
					
					if (returnReader)
					{
						ctxch.Reader = new TandemDataReader(doc);
					}

					return new DummyAsyncResult(ctxch);
				}
			}

			WebRequest request = StartCallTandem(servletName, methodPostOrGet, uriParameterPrefix, uriParameters, httpHeaders, postData);

			HpAsyncContext ctx = new HpAsyncContext(request.RequestUri.ToString(), request, parameterKey, cacheDurationInMinutes, returnReader);

			return request.BeginGetResponse(RespCallback, ctx);
		}

		private XmlDocument EndCallTandem
		(
			Stream strm, 
			string uri, 
			string parameterKey,
			int? cacheDurationInMinutes
		)
		{
			using (StreamReader read = new StreamReader(strm))
			{
				string xml = PreprocessXml(read.ReadToEnd());

				// Check for <H1>Server Error</H1> or advisory reply
				XmlDocument doc = XMLIsErrorFree(xml, uri);

				read.Close();
				strm.Close();
				strm.Dispose();

				if (cacheDurationInMinutes.HasValue && cacheDurationInMinutes > 0)
				{
					ObjectCache cache = MemoryCache.Default;
					var policy = new CacheItemPolicy();
					
					policy.AbsoluteExpiration = DateTime.Now.AddMinutes((double)cacheDurationInMinutes);
					cache.Add(parameterKey, doc, policy);
				}

				return doc;
			}
		}

		private void RespCallback(IAsyncResult result)
		{
			HpAsyncContext ctx = (HpAsyncContext)result.AsyncState;

			try
			{
				using (var responseStream = ctx.Request.EndGetResponse(result).GetResponseStream())
				{
					ctx.Xml = EndCallTandem(responseStream, ctx.Uri, ctx.ParameterKey, ctx.CacheDurationInMinutes);
					responseStream.Close();
				}
				if (ctx.CreateReader)
				{
					ctx.Reader = new TandemDataReader(ctx.Xml);
				}
			}
			catch (Exception ex)
			{
				ctx.AsyncException = ex;
			}

			ctx.CompletedEvent.Set();
		}

		/// <summary>
		/// Call a tandem servlet using TANDEM_URL from the config file.
		/// </summary>
		/// <param name="servletName">Fully qualified servlet name.</param>
		/// <param name="postData">POST HTTP body parameters, can be null.</param>
		/// <returns></returns>
		protected TandemDataReader ExecuteReader
		(
			string servletName,
			int? cacheDurationInMinutes = null,
			params TandemParameter[] postData
		)
		{
			return new TandemDataReader
			(
				CallTandem
				(
					servletName, 
					"POST", 
					"", 
					null, 
					null,
					TandemParameter.Append
					(
						postData,
						new TandemParameter("version", "2"),
						new TandemParameter("returnMetaData", "true")
					),
					cacheDurationInMinutes
				)
			);
		}

		protected IAsyncResult BeginExecuteReader
		(
			string servletName,
			int? cacheDurationInMinutes = null,
			params TandemParameter[] postData
		)
		{
			return BeginCallTandem
			(
				servletName,
				"POST",
				"",
				null,
				null,
				TandemParameter.Append
				(
					postData,
					new TandemParameter("version", "2"),
					new TandemParameter("returnMetaData", "true")
				),
				true,
				cacheDurationInMinutes
			);
		}
	
		/// <summary>
		/// Returns effected row count
		/// </summary>
		/// <param name="servletName">Fully qualified servlet name.</param>
		/// <param name="postData">POST HTTP body parameters, can be null.</param>
		/// <returns></returns>
		protected int Execute
		(
			string servletName,
			int? cacheDurationInMinutes = null,
			TandemParameter[] postData = null
		)
		{
			XmlDocument doc = CallTandem
			(
				servletName,
				"POST",
				"",
				null,
				null,
				TandemParameter.Append
				(
					postData, 
					new TandemParameter("version", "2")
				),
				cacheDurationInMinutes
			);

			string cnt = doc.ChildNodes[0].Attributes["count"].InnerText;
			return Int32.Parse(cnt);
		}

		/// <summary>
		/// Make a tandem servlet call with the parameters as part of the URI query string.
		/// </summary>
		/// <param name="servletName">Fully qualified servlet name (do not include /servlet/).</param>
		/// <param name="parameters">Parameter array.</param>
		/// <returns>A raw XML document.</returns>
		protected XmlDocument CallUri
		(
			string servletName, 
			int? cacheDurationInMinutes = null,
			params TandemParameter[] parameters
		)
		{
			return CallTandem(servletName, "GET", "", parameters, null, null, cacheDurationInMinutes);
		}

		protected IAsyncResult BeginCallUri
		(
			string servletName,
			int? cacheDurationInMinutes = null,
			params TandemParameter[] parameters
		)
		{
			return BeginCallTandem(servletName, "GET", "", parameters, null, null, false, cacheDurationInMinutes);
		}

		/// <summary>
		/// Make a tandem servlet call with the parameters as part of the URI query string.
		/// </summary>
		/// <param name="servletName">Fully qualified servlet name (do not include /servlet/).</param>
		/// <param name="parameters">Parameter array.</param>
		/// <returns>A raw XML document.</returns>
		protected XmlDocument CallUriAndHeader
		(
			string servletName, 
			TandemParameter[] uri, 
			TandemParameter[] headers,
			int? cacheDurationInMinutes = null
		)
		{
			return CallTandem(servletName, "POST", "", uri, headers, null, cacheDurationInMinutes);
		}

		/// <summary>
		/// Make a tandem servlet call with the parameters as part of the URI query string.
		/// </summary>
		/// <param name="servletName">Fully qualified servlet name (do not include /servlet/).</param>
		/// <param name="parameters">HTTP body parameters.</param>
		/// <returns>A raw XML document.</returns>
		protected XmlDocument CallPost
		(
			string servletName, 
			TandemParameter[] uri, 
			TandemParameter[] parameters,
			int? cacheDurationInMinutes = null
		)
		{
			return CallTandem(servletName, "POST", "", uri, null, parameters, cacheDurationInMinutes);
		}

		protected IAsyncResult BeginCallPost
		(
			string servletName,
			TandemParameter[] uri,
			int? cacheDurationInMinutes = null,
			params TandemParameter[] parameters
		)
		{
			return BeginCallTandem(servletName, "POST", "", uri, null, parameters, false, cacheDurationInMinutes);
		}

		/// <summary>
		/// Make a tandem servlet call with header and post data.
		/// </summary>
		/// <param name="servletName">Fully qualified servlet name (do not include /servlet/).</param>
		/// <param name="header">Parameters to put in the header.</param>
		/// <param name="parameters">Parameters to put in the HTTP body.</param>
		/// <returns>A raw XML document.</returns>
		protected XmlDocument CallHeaderAndPost
		(
			string servletName, 
			TandemParameter[] header, 
			TandemParameter[] parameters,
			int? cacheDurationInMinutes = null
		)
		{
			return CallTandem(servletName, "POST", "", null, header, parameters, cacheDurationInMinutes);
		}

		protected HpAsyncContext EndAsync(IAsyncResult async)
		{
			var ctx = (HpAsyncContext)async.AsyncState;
			
			ctx.CompletedEvent.Wait();
			ctx.CompletedEvent.Dispose();
			ctx.RethrowAnyAsyncException();

			if (ctx.SubQueries != null)
			{
				foreach (var q in ctx.SubQueries)
				{
					EndAsync(q);
				}
			}

			return ctx;
		}

		/// <summary>
		/// Show whether a transaction is currently in process.
		/// </summary>
		public bool TransactionInProcess
		{
			get;
			protected set;
		}

		/// <summary>
		/// close a connection to the database and release it's resources
		/// </summary>
		public virtual void Close()
		{
		}

		/// <summary>
		/// Begin a SQL Transaction on the current connection
		/// </summary>
		public virtual void BeginTrans()
		{
			throw new NotImplementedException("Transactions are not supported through this interface.");
		}

		/// <summary>
		/// Commit the current SQL Transaction
		/// </summary>
		public virtual void CommitTrans()
		{
			throw new NotImplementedException("Transactions are not supported through this interface.");
		}

		/// <summary>
		/// Begin a SQL Transaction on the current connection
		/// </summary>
		public virtual void RollbackTrans()
		{
			throw new NotImplementedException("Transactions are not supported through this interface.");
		}

		public static bool ConvertTandemDecimalToBool(string val)
		{
			return val != "0.0" && val != "0";
		}

		public static int ConvertTandemDecimalToInt(object val)
		{
			if (val is Decimal)
			{
				return (int)(decimal)val;
			}

			var sval = val.ToString();
			sval = sval.Substring(0, sval.IndexOf('.'));

			return Int32.Parse(sval);
		}

		#region IDisposable Members

		// Dispose(bool disposing) executes in two distinct scenarios.
		// If disposing equals true, the method has been called directly
		// or indirectly by a user's code. Managed and unmanaged resources
		// can be disposed.
		// If disposing equals false, the method has been called by the
		// runtime from inside the finalizer and you should not reference
		// other objects. Only unmanaged resources can be disposed.
		protected virtual void Dispose(bool disposing)
		{
		}

		// This object will be cleaned up by the Dispose method.
		// Therefore, you should call GC.SupressFinalize to
		// take this object off the finalization queue
		// and prevent finalization code for this object
		// from executing a second time.
		public virtual void Dispose()
		{
			Dispose(true);
		}

		#endregion
	}
}
