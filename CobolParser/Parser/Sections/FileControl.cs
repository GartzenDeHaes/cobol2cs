using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CobolParser.Divisions.Proc;

namespace CobolParser.Sections
{
	public class FileControl
	{
		public List<Select> Selects
		{
			get;
			private set;
		}

		public List<ReceiveControl> ReceiveControls
		{
			get;
			private set;
		}

		public FileControl(Terminalize terms)
		{
			Selects = new List<Select>();
			ReceiveControls = new List<ReceiveControl>();

			terms.Match("FILE-CONTROL");
			terms.Match(StringNodeType.Period);

			while (true)
			{
				if (terms.CurrentEquals("SELECT"))
				{
					Selects.Add(new Select(terms));
				}
				else if (terms.CurrentEquals("RECEIVE-CONTROL"))
				{
					ReceiveControls.Add(new ReceiveControl(terms));
				}
				else
				{
					break;
				}
			}
		}

		public Select Find(string name)
		{
			foreach (var s in Selects)
			{
				if (s.MessageName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				{
					return s;
				}
			}

			return null;
		}

		public string ReplyMessageName()
		{
			if (ReceiveControls.Count == 0)
			{
				return "void";
			}

			foreach (var rc in ReceiveControls)
			{
				if (!String.IsNullOrEmpty(rc.ReplyMessage))
				{
					return rc.ReplyMessage;
				}
			}

			return "void";
		}

		public bool UsesDollarReceive()
		{
			foreach (var s in Selects)
			{
				if (s.FileName.Equals("$RECEIVE", StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}

			return false;
		}
	}
}
