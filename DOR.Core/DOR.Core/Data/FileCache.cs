using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using DOR.Core.Logging;
using DOR.Core.Data.Tandem;

namespace DOR.Core.Data
{
	public static class FileCache
	{
		public static XmlDocument Get(string requestKey)
		{
			try
			{
				string filename = FileNameFor(requestKey);
				if (!File.Exists(filename))
				{
					return null;
				}

				XmlDocument doc = new XmlDocument();
				doc.Load(filename);

				XmlNode cnode = doc.SelectSingleNode("//ZZcache");
				if (cnode == null)
				{
					// bad file?
					File.Delete(filename);
					return null;
				}

				doc.DocumentElement.RemoveChild(cnode);

				DateTime expiresOn = DateTime.Parse(cnode.Attributes["expires"].InnerText);
				if (expiresOn < DateTime.Now)
				{
					File.Delete(filename);
					return null;
				}

				return doc;
			}
			catch (Exception ex)
			{
				SimpleFileLogger.WriteS(SystemPID.Unknown, ex);
			}

			return null;
		}

		public static void Put(string requestKey, XmlDocument doc, DateTime expiresOn)
		{
			try
			{
				XmlNode cnode = doc.SelectSingleNode("/ZZcache");
				if (cnode == null)
				{
					cnode = doc.CreateElement("ZZcache");
					XmlAttribute attr = doc.CreateAttribute("expires");
					cnode.Attributes.Append(attr);
					doc.DocumentElement.AppendChild(cnode);
				}

				cnode.Attributes["expires"].InnerText = expiresOn.ToString();

				string filename = FileNameFor(requestKey);
				if (File.Exists(filename))
				{
					File.Delete(filename);
				}

				doc.Save(filename);

				doc.DocumentElement.RemoveChild(cnode);
			}
			catch (Exception ex)
			{
				SimpleFileLogger.WriteS(SystemPID.Unknown, ex);
			}
		}

		private static string FileNameFor(string requestKey)
		{
			string filename = Hash(requestKey).Replace("-", "") + ".xml";
			return Path.Combine(Path.GetTempPath(), filename);
		}

		private static string Hash(string data)
		{
			SHA1CryptoServiceProvider sha1h = new SHA1CryptoServiceProvider();
			using (var s = new MemoryStream(ASCIIEncoding.Default.GetBytes(data)))
			{
				return BitConverter.ToString(sha1h.ComputeHash(s));
			}
		}		
	}
}
