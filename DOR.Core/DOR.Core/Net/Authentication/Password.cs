using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core.Net.Authentication
{
	public class Password
	{
		public string Text
		{
			get;
			private set;
		}

		public DateTime Created
		{
			get;
			private set;
		}

		public string CreatedBy
		{
			get;
			private set;
		}

		public Password(string text, DateTime created, string createdBy)
		{
			Text = text;
			Created = created;
			CreatedBy = createdBy;
		}

		public static Password Parse(string line)
		{
			return null;
		}
	}
}
