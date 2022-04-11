using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core.Data.Tandem;
using DOR.Core.Config;
using System.Xml;
using DOR.Core.Data;

namespace CobolParser.TandemData
{
	public class TandemDataAccess : TandemDataAccessBase
	{
		public TandemDataAccess()
		: this(Configuration.AppConfig)
		{
		}

		public TandemDataAccess(IConfiguration config)
		: base(config)
		{
			ServletDirectoryName = "servlet-et";
		}

		public IList<ColumnDef> Invoke(string define)
		{
			List<TandemParameter> prm = new List<TandemParameter>();

			prm.Add(BuildParameter("action", "inquire"));

			if (define[0] == '=')
			{
				prm.Add(BuildParameter("define", define.Substring(1)));
			}
			else
			{
				prm.Add(BuildParameter("define", define));
			}

			XmlDocument doc = CallPost
			(
				"et.SelInvoke",
				null,
				prm.ToArray(),
				60 * 24
			);

			if (doc.DocumentElement.HasChildNodes)
			{
				List<ColumnDef> defs = new List<ColumnDef>();

				foreach (XmlNode c in doc.DocumentElement.ChildNodes)
				{
					defs.Add
					(
						new ColumnDef
						(
							c.Attributes["name"].InnerText,
							Int32.Parse(c.Attributes["size"].InnerText),
							c.Attributes["type"].InnerText,
							Int32.Parse(c.Attributes["precision"].InnerText),
							Int32.Parse(c.Attributes["scale"].InnerText),
							Convert.ToBoolean(c.Attributes["identity"].InnerText),
							Convert.ToBoolean(c.Attributes["nullable"].InnerText != "0"),
							Convert.ToBoolean(c.Attributes["signed"].InnerText)
						)
					);
				}

				return defs;
			}
			else
			{
				throw new DataAccessException("Unknown error 2: " + doc.ToString());
			}
		}
	}
}
