using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using DOR.Core;
using DOR.Core.Net;

namespace DOR.Core.Net.Command
{
	public enum CommandId
	{
		CMD_UNKNOWN = 0,
		CMD_PING = 1,
		CMD_PONG = 2,
		CMD_INITLOGON = 3,
		CMD_LOGON = 4,
		CMD_PUBLICKEY = 5,
		CMD_ACK = 6,
		CMD_NAK = 7,
		CMD_OPEN_SESSION = 8,
		CMD_CLOSE_SESSION = 9,
		CMD_FILENAME_QUERY = 10
	}

	public delegate void NetCommandHandler(ICommand cmd, Stream sock);

	public abstract class ICommand : IPacketListener
	{
		protected CommandId m_id;
		protected NetCommandHandler m_handler;

		public ICommand(CommandId id, NetCommandHandler handler)
		{
			m_id = id;
			m_handler = handler;
		}

		public CommandId ID 
		{
			get { return m_id; }
		}

		public abstract bool IsComplete { get; }
		public abstract void Reset();
		public abstract void Send(Stream writer);

		public virtual void Execute(Stream sock)
		{
			if (null != m_handler)
			{
				m_handler(this, sock);
			}
		}

		public virtual void IPacket_OnData(long i)
		{
			throw new Exception("Unexpected data long");
		}

		public virtual void IPacket_OnData(int i)
		{
			throw new Exception("Unexpected data int");
		}

		public virtual void IPacket_OnData(short i)
		{
			throw new Exception("Unexpected data short");
		}

		public virtual void IPacket_OnData(char i)
		{
			throw new Exception("Unexpected data char");
		}

		public virtual void IPacket_OnData(bool b)
		{
			throw new Exception("Unexpected data bool");
		}

		public virtual void IPacket_OnData(float f)
		{
			throw new Exception("Unexpected data float");
		}

		public virtual void IPacket_OnData(double d)
		{
			throw new Exception("Unexpected data double");
		}

		public virtual void IPacket_OnData(string str)
		{
			throw new Exception("Unexpected data string");
		}

		public virtual void IPacket_OnData(byte[] data, int len)
		{
			throw new Exception("Unexpected data bin");
		}

		public virtual void IPacket_OnData(DateTime dtm)
		{
			throw new Exception("Unexpected data datetime");
		}

		public virtual void IPacket_OnData(Date i)
		{
			throw new Exception("Unexpected data date");
		}

		public virtual void IStreamRead_OnError(string msg)
		{
		}

		public virtual void IStreamRead_OnClose()
		{
		}
	}
}
