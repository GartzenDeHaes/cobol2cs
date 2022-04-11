using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core.Net.Command
{
	public class NetCommandFactory : ICommandFactory
	{
		private NetCommandHandler m_pingHandler;
		private NetCommandHandler m_pongHandler;
		private NetCommandHandler m_initLogonHandler;
		private NetCommandHandler m_publicKeyHandler;
		private NetCommandHandler m_logonHandler;
		private NetCommandHandler m_ackHandler;
		private NetCommandHandler m_nakHandler;

		public NetCommandFactory
		(
			NetCommandHandler pingHandler,
			NetCommandHandler pongHandler,
			NetCommandHandler initLogonHandler,
			NetCommandHandler publicKeyHandler,
			NetCommandHandler logonHandler,
			NetCommandHandler ackHandler,
			NetCommandHandler nakHandler
		)
		{
			m_pingHandler = pingHandler;
			m_pongHandler = pongHandler;
			m_initLogonHandler = initLogonHandler;
			m_publicKeyHandler = publicKeyHandler;
			m_logonHandler = logonHandler;
			m_ackHandler = ackHandler;
			m_nakHandler = nakHandler;
		}

		public ICommand GetCommand(CommandId id)
		{
			switch (id)
			{
				case CommandId.CMD_PING:
					return new Ping(m_pingHandler);
				case CommandId.CMD_PONG:
					return new Pong(m_pongHandler);
				case CommandId.CMD_INITLOGON:
					return new InitLogon(m_initLogonHandler);
				case CommandId.CMD_LOGON:
					return new Logon(m_logonHandler);
				case CommandId.CMD_PUBLICKEY:
					return new PublicKey(m_publicKeyHandler);
				case CommandId.CMD_ACK:
					return new Ack(m_ackHandler);
				case CommandId.CMD_NAK:
					return new Nak(m_nakHandler);
				default:
					throw new Exception("Unimplemented command id " + id);
			}
		}

		public void ReleaseCommand(ICommand cmd)
		{

		}
	}
}
