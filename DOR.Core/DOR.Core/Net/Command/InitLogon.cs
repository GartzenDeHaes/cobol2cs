using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DOR.Core.Net.Command
{
	/// <summary>
	/// Client request to the server to initate the logon process
	/// </summary>
	public class InitLogon : ICommand
	{
		private enum InitLogonState
		{
			ST_START = 0,
			ST_MAJOR = 1,
			ST_MINOR = 2,
			ST_DONE = 4
		}

		private Guid m_clientId;
		private int m_clientVersionMajor;
		private int m_clientVersionMinor;
		private InitLogonState m_state = InitLogonState.ST_START;

		public InitLogon(NetCommandHandler handler)
		: base(CommandId.CMD_INITLOGON, handler)
		{
		}

		public InitLogon
		(
			NetCommandHandler handler,
			Guid clientId, 
			int clientVersionMajor, 
			int clientVersionMinor
		)
		: base(CommandId.CMD_INITLOGON, handler)
		{
			Set(clientId, clientVersionMajor, clientVersionMinor);
		}

		public void Set
		(
			Guid clientId,
			int clientVersionMajor,
			int clientVersionMinor
		)
		{
			m_clientId = clientId;
			m_clientVersionMajor = clientVersionMajor;
			m_clientVersionMinor = clientVersionMinor;
			m_state = InitLogonState.ST_DONE;
		}

		public override bool IsComplete
		{
			get { return m_state == InitLogonState.ST_DONE; }
		}

		public override void Reset()
		{
			m_state = InitLogonState.ST_START;
		}

		public override void IPacket_OnData(byte[] data, int len)
		{
			if (IsComplete)
			{
				throw new ProtocolException("Packet is already complete.");
			}
			if (m_state != InitLogonState.ST_START)
			{
				throw new ProtocolException("Unexpected binary in InitLogon");
			}
			byte[] guid = new byte[16];
			data.CopyTo(guid, 0);
			m_clientId = new Guid(guid);
			m_state = InitLogonState.ST_MAJOR;
		}

		public override void IPacket_OnData(int i)
		{
			if (IsComplete)
			{
				throw new ProtocolException("Packet is already complete.");
			}
			if (m_state == InitLogonState.ST_MAJOR)
			{
				m_clientVersionMajor = i;
				m_state = InitLogonState.ST_MINOR;
			}
			else if (m_state == InitLogonState.ST_MINOR)
			{
				m_clientVersionMinor = i;
				m_state = InitLogonState.ST_DONE;
			}
			else
			{
				throw new ProtocolException("Unexpected int32 in InitLogon");
			}
		}

		public override void Send(Stream writer)
		{
			Packet pkt = new Packet();
			pkt.Append((byte)ID);
			pkt.Append(m_clientId.ToByteArray());
			pkt.Append(m_clientVersionMajor);
			pkt.Append(m_clientVersionMinor);
			pkt.Send(writer);
		}
	}
}
