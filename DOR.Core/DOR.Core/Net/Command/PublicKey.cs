using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace DOR.Core.Net.Command
{
	public class PublicKey : ICommand
	{
		private enum PublicKeyState
		{
			PK_START = 0,
			PK_MOD = 1,
			PK_EXP = 2,
			PK_DONE = 3
		}

		private RSAPublicKey m_rsa;
		private PublicKeyState m_state = PublicKeyState.PK_START;

		public PublicKey(NetCommandHandler handler)
		: base(CommandId.CMD_PUBLICKEY, handler)
		{
			m_rsa = new RSAPublicKey();
			m_rsa.BitSize = 32;
		}

		public PublicKey(NetCommandHandler handler, RSAPublicKey rsa)
		: base(CommandId.CMD_PUBLICKEY, handler)
		{
			m_rsa = rsa;
			m_state = PublicKeyState.PK_DONE;
		}

		public RSAPublicKey Key
		{
			get { return m_rsa; }
		}

		public override bool IsComplete
		{
			get { return m_state == PublicKeyState.PK_DONE; }
		}

		public override void Reset()
		{
			m_state = PublicKeyState.PK_START;
			m_rsa = new RSAPublicKey();
		}

		public override void Send(Stream writer)
		{
			Packet pkt = new Packet();
			pkt.Append((byte)ID);
			pkt.Append((long)m_rsa.Modulus);
			pkt.Append((long)m_rsa.Exponent);
			pkt.Send(writer);
		}

		public override void IPacket_OnData(int i)
		{
			if (IsComplete)
			{
				throw new ProtocolException("Packet is already complete.");
			}
			if (m_state == PublicKeyState.PK_START)
			{
				m_rsa.BitSize = i;
				m_state = PublicKeyState.PK_MOD;
			}
			else if (m_state == PublicKeyState.PK_MOD)
			{
				m_rsa.Modulus = new BigInteger(i);
				m_state = PublicKeyState.PK_EXP;
			}
			else if (m_state == PublicKeyState.PK_EXP)
			{
				m_rsa.Exponent = new BigInteger(i);
				m_state = PublicKeyState.PK_DONE;
			}
			else
			{
				throw new ProtocolException("Unexpected int32 in PublicKey");
			}
		}

		public override void IPacket_OnData(long i)
		{
			if (IsComplete)
			{
				throw new ProtocolException("Packet is already complete.");
			}
			if (m_state == PublicKeyState.PK_MOD)
			{
				m_rsa.Modulus = new BigInteger(i);
				m_state = PublicKeyState.PK_EXP;
			}
			else if (m_state == PublicKeyState.PK_EXP)
			{
				m_rsa.Exponent = new BigInteger(i);
				m_state = PublicKeyState.PK_DONE;
			}
			else
			{
				throw new ProtocolException("Unexpected int64 in PublicKey");
			}
		}
	}
}
