using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DOR.Core.Net.Command
{
	public class Pong : ICommand
	{
		DateTime? m_dtm;

		public Pong(NetCommandHandler handler)
		: base(CommandId.CMD_PONG, handler)
		{
		}

		public Pong(DateTime dtm, NetCommandHandler handler)
		: base(CommandId.CMD_PONG, handler)
		{
			m_dtm = dtm;
		}

		public override bool IsComplete
		{
			get { return m_dtm.HasValue; }
		}

		public override void Reset()
		{
			m_dtm = null;
		}

		public DateTime? Value
		{
			get { return m_dtm; }
		}

		public override void IPacket_OnData(DateTime dtm)
		{
			if (IsComplete)
			{
				throw new Exception("Packet is already complete.");
			}

			m_dtm = dtm;
		}

		public override void Send(Stream writer)
		{
			Packet pkt = new Packet();
			pkt.Append((byte)ID);
			pkt.Append(m_dtm.Value);
			pkt.Send(writer);
		}

		public override void Execute(Stream sock)
		{
			if (null != m_handler)
			{
				m_handler(this, sock);
			}
		}
	}
}
