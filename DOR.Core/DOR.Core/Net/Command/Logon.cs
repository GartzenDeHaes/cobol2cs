using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DOR.Core.Net.Command
{
	public class Logon : ICommand
	{
		private enum LogonState
		{
			LO_START = 0,
			LO_PW = 1,
			LO_DONE = 2
		}

		private string m_logonId;
		private string m_password;
		private RSAPublicKey m_key;

		private LogonState m_state = LogonState.LO_START;

		public Logon(NetCommandHandler handler)
		: base(CommandId.CMD_LOGON, handler)
		{
		}

		public Logon
		(
			NetCommandHandler handler,
			RSAPublicKey key,
			string logonId, 
			string password
		)
		: base(CommandId.CMD_LOGON, handler)
		{
			Set(key, logonId, password);
		}

		public void Set
		(
			RSAPublicKey key,
			string logonId,
			string password
		)
		{
			m_key = key;
			m_logonId = logonId;
			m_password = password;
			m_state = LogonState.LO_DONE;
		}

		public override bool IsComplete
		{
			get { return m_state == LogonState.LO_DONE; }
		}

		public override void Reset()
		{
			m_state = LogonState.LO_START;
		}

		public override void IPacket_OnData(String s)
		{
			if (IsComplete)
			{
				throw new ProtocolException("Packet is already complete.");
			}
			if (m_state == LogonState.LO_START)
			{
				m_logonId = s;
				m_state = LogonState.LO_PW;
			}
			else if (m_state == LogonState.LO_PW)
			{
				m_password = s;
				m_state = LogonState.LO_DONE;
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
			pkt.Append(m_key.EncryptText(m_logonId));
			pkt.Append(m_key.EncryptText(m_password));
			pkt.Send(writer);
		}
	}
}
