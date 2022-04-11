using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace DOR.Core.Data.Tandem
{
	public class SQLSA
	{
		public int num_tables;

		public struct STATS_TYPE
		{
			public string table_name;
			public int records_accessed;
			public int records_used;
			public int disk_reads;
			public int messages;
			public int message_bytes;
			public int waits;
			public int escalations;
		}

		public STATS_TYPE[] stats = new STATS_TYPE[16];

		public static SQLSA Parse(XmlDocument doc)
		{
			SQLSA sqlsa = new SQLSA();
			XmlNode stats = doc.DocumentElement.SelectSingleNode("stats");

			if (stats == null)
			{
				sqlsa.num_tables = 0;
			}
			else
			{
				sqlsa.num_tables = stats.ChildNodes.Count;

				for (int x = 0; x < stats.ChildNodes.Count; x++)
				{
					sqlsa.stats[x].disk_reads = Int32.Parse(stats.ChildNodes[x].Attributes["disc_reads"].Value);
					sqlsa.stats[x].escalations = Int32.Parse(stats.ChildNodes[x].Attributes["escalations"].Value);
					sqlsa.stats[x].message_bytes = Int32.Parse(stats.ChildNodes[x].Attributes["message_bytes"].Value);
					sqlsa.stats[x].messages = Int32.Parse(stats.ChildNodes[x].Attributes["messages"].Value);
					sqlsa.stats[x].records_accessed = Int32.Parse(stats.ChildNodes[x].Attributes["records_accessed"].Value);
					sqlsa.stats[x].records_used = Int32.Parse(stats.ChildNodes[x].Attributes["records_used"].Value);
					sqlsa.stats[x].table_name = stats.ChildNodes[x].Attributes["table_name"].Value;
					sqlsa.stats[x].waits = Int32.Parse(stats.ChildNodes[x].Attributes["waits"].Value);
				}
			}

			return sqlsa;
		}
	}
}
