using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DOR.Core.Net.Command
{
	public class Ack : ICommand
	{
		private enum AckState
		{
			ACK_START = 0,
			ACK_DONE = 1
		}

		private string m_msg;

		private AckState m_state = AckState.ACK_START;

		public Ack(NetCommandHandler handler)
		: base(CommandId.CMD_ACK, handler)
		{
		}

		public Ack
		(
			NetCommandHandler handler,
			string msg
		)
		: base(CommandId.CMD_ACK, handler)
		{
			m_msg = msg;
			m_state = AckState.ACK_DONE;
		}

		public string Message
		{
			get { return m_msg; }
			set { m_msg = value; }
		}

		public override bool IsComplete
		{
			get { return m_state == AckState.ACK_DONE; }
		}

		public override void Reset()
		{
			m_state = AckState.ACK_START;
		}

		public override void IPacket_OnData(string s)
		{
			if (IsComplete)
			{
				throw new ProtocolException("Packet is already complete.");
			}
			if (m_state == AckState.ACK_START)
			{
				m_msg = s;
				m_state = AckState.ACK_DONE;
			}
			else
			{
				throw new ProtocolException("Unexpected string in Logon");
			}
		}

		public override void Send(Stream writer)
		{
			Packet pkt = new Packet();
			pkt.Append((byte)ID);
			pkt.Append(m_msg);
			pkt.Send(writer);
		}
	}
}
