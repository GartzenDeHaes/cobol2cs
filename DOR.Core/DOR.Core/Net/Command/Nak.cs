using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DOR.Core.Net.Command
{
	public class Nak : Ack
	{
		public Nak(NetCommandHandler handler)
		: base(handler)
		{
			m_id = CommandId.CMD_NAK;
		}

		public Nak
		(
			NetCommandHandler handler,
			string msg
		)
		: base(handler, msg)
		{
			m_id = CommandId.CMD_NAK;
		}
	}
}
