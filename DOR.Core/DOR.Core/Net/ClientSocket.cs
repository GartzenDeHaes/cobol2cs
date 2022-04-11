using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using DOR.Core.Collections;
using DOR.Core.Net.Command;

namespace DOR.Core.Net
{
	public delegate void ClientSocketCloseHandler(ClientSocket cs);
	public delegate void ClientSocketErrorHandler(ClientSocket cs, string message);
	public delegate void ClientSocketInitHandler(ClientSocket cs);
	
	public class ClientSocket
	{
		private enum PacketReadState
		{
			PKT_ENDIAN,
			PKT_SIZE_MCB,
			PKT_SIZE_LCB,
			PKT_DT,
			PKT_DT_LEN_MCB,
			PKT_DT_LEN_LCB,
			PKT_CHARSIZE,
			PKT_DATA,
			PKT_ERROR,
			PKT_CLOSED
		};

		private ClientSocketCloseHandler m_closeHandler;
		private ClientSocketErrorHandler m_errorHandler;

		private ICommandFactory m_fact;
		private PacketReadState m_state = PacketReadState.PKT_ENDIAN;
		private TcpClient m_client;
		private Thread m_thread;
		private volatile bool m_running;

		private ICommand m_currentCommand;
		private Vector<byte> m_buf = new Vector<byte>();
		private bool m_isLittleEndian;
		private bool m_revBytes;
		private int m_pktSize;
		private char m_datatype;
		private int m_datalen;
		private int m_charsize;
		private int m_readPos;

		public ClientSocket
		(
			string host, 
			int port, 
			ICommandFactory fact,
			ClientSocketCloseHandler closeHandler,
			ClientSocketErrorHandler errorHandler
		)
		{
			m_closeHandler = closeHandler;
			m_errorHandler = errorHandler;
			
			m_fact = fact;
			m_client = new TcpClient(host, port);
			m_client.LingerState.Enabled = true;

			m_thread = new Thread(new ThreadStart(Run));
			m_thread.Start();
		}

		public bool IsRunning
		{
			get { return m_running; }
		}

		public Stream GetStream()
		{
			return m_client.GetStream();
		}

		public void Stop()
		{
			m_running = false;
			m_client.Close();
		}

		public void Close()
		{
			Stop();
		}

		public void Join(int timeoutMs)
		{
			m_thread.Join(timeoutMs);
		}

		private void Run()
		{
			m_running = true;
			byte[] buf = new byte[256];
			int bufLenMinusOne = buf.Length - 1;

			while (m_running)
			{
				try
				{
					int shouldBeOne = m_client.GetStream().Read(buf, 0, 1);
					if (shouldBeOne != 1)
					{
						m_running = false;
						if (null != m_closeHandler)
						{
							m_closeHandler(this);
						}
						return;
					}

					int available = m_client.Available;
					int count = m_client.GetStream().Read(buf, 1, (available >= bufLenMinusOne ? bufLenMinusOne : available));

					for (int pos = 0; pos < count + 1; pos++)
					{
						if (m_readPos == m_pktSize)
						{
							m_state = PacketReadState.PKT_ENDIAN;
						}
						else
						{
							Debug.Assert(0 == m_pktSize || m_readPos < m_pktSize);
						}

						m_readPos++;
						byte b = buf[pos];

						switch (m_state)
						{
							case PacketReadState.PKT_ENDIAN:
								m_isLittleEndian = 0 != b;
								m_revBytes = !m_isLittleEndian;
								m_state = PacketReadState.PKT_SIZE_MCB;
								break;
							case PacketReadState.PKT_SIZE_MCB:
								m_pktSize = (int)b << 8;
								m_state = PacketReadState.PKT_SIZE_LCB;
								break;
							case PacketReadState.PKT_SIZE_LCB:
								m_pktSize |= (int)b & 0xFF;
								m_state = PacketReadState.PKT_DT;
								break;
							case PacketReadState.PKT_DT:
								m_datatype = (char)b;
								ConfigDataType();
								break;
							case PacketReadState.PKT_DT_LEN_MCB:
								m_datalen = (int)b << 8;
								m_state = PacketReadState.PKT_DT_LEN_LCB;
								break;
							case PacketReadState.PKT_DT_LEN_LCB:
								m_datalen |= (int)b & 0xFF;
								if (m_datatype == 'S')
								{
									m_state = PacketReadState.PKT_CHARSIZE;
								}
								else
								{
									m_state = PacketReadState.PKT_DATA;
								}
								break;
							case PacketReadState.PKT_CHARSIZE:
								m_charsize = (int)b;
								m_datalen *= m_charsize;
								m_state = PacketReadState.PKT_DATA;
								break;
							case PacketReadState.PKT_DATA:
								m_buf.Add(b);
								if (m_buf.Count == m_datalen)
								{
									_ParseData();
									m_buf.Clear();
									m_state = PacketReadState.PKT_DT;
								}
								Debug.Assert(m_buf.Count < m_datalen);
								break;
							case PacketReadState.PKT_ERROR:
								Debug.Assert(false);
								break;
						}

						if (m_readPos == m_pktSize)
						{
							m_state = PacketReadState.PKT_ENDIAN;
							m_readPos = 0;
						}
						Debug.Assert(0 == m_pktSize || m_readPos <= m_pktSize);
					}
				}
				catch (Exception ex)
				{
					if (null != m_errorHandler)
					{
						m_errorHandler(this, ex.ToString());
					}
					m_running = false;
				}
			}

			if (null != m_closeHandler)
			{
				m_closeHandler(this);
			}
			m_client.Close();
		}

		private void ConfigDataType()
		{
			switch (m_datatype)
			{
				case 'L':
					m_datalen = 8;
					m_state = PacketReadState.PKT_DATA;
					break;
				case 'I':
					m_datalen = 4;
					m_state = PacketReadState.PKT_DATA;
					break;
				case 'X':
					m_datalen = 2;
					m_state = PacketReadState.PKT_DATA;
					break;
				case 'B':
					m_datalen = 1;
					m_state = PacketReadState.PKT_DATA;
					break;
				case 'S':
					m_state = PacketReadState.PKT_DT_LEN_MCB;
					break;
				case 'R':
					m_state = PacketReadState.PKT_DT_LEN_MCB;
					break;
				case 'F':
					m_datalen = 1;
					m_state = PacketReadState.PKT_DATA;
					break;
				case 'f':
					m_charsize = 1;
					m_state = PacketReadState.PKT_DT_LEN_MCB;
					break;
				case 'd':
					m_charsize = 1;
					m_state = PacketReadState.PKT_DT_LEN_MCB;
					break;
				case 'D':
					m_datalen = 3;
					m_state = PacketReadState.PKT_DATA;
					break;
				case 't':
					m_datalen = 7;
					m_state = PacketReadState.PKT_DATA;
					break;
				default:
					m_state = PacketReadState.PKT_ERROR;
					throw new Exception("Invalid datatype recevied");
			}
		}

		private void _ParseData()
		{
			int year;

			switch ( m_datatype )
			{
				case 'L':
					if (m_revBytes)
					{
						m_currentCommand.IPacket_OnData((long)((ulong)m_buf.ElementAt(0) | ((ulong)m_buf.ElementAt(1) << 8) | ((ulong)m_buf.ElementAt(2) << 16) | ((ulong)m_buf.ElementAt(3) << 24) | ((ulong)m_buf.ElementAt(4) << 32) | ((ulong)m_buf.ElementAt(5) << 40) | ((ulong)m_buf.ElementAt(6) << 48) | ((ulong)m_buf.ElementAt(7) << 56)));
					}
					else
					{
						m_currentCommand.IPacket_OnData((long)((ulong)m_buf.ElementAt(7) | ((ulong)m_buf.ElementAt(6) << 8) | ((ulong)m_buf.ElementAt(5) << 16) | ((ulong)m_buf.ElementAt(4) << 24) | ((ulong)m_buf.ElementAt(3) << 32) | ((ulong)m_buf.ElementAt(2) << 40) | ((ulong)m_buf.ElementAt(1) << 48) | ((ulong)m_buf.ElementAt(0) << 56)));
					}
					break;
				case 'I':
					if (m_revBytes)
					{
						m_currentCommand.IPacket_OnData((int)(m_buf.ElementAt(0) | (m_buf.ElementAt(1) << 8) | (m_buf.ElementAt(2) << 16) | (m_buf.ElementAt(3) << 24)));
					}
					else
					{
						m_currentCommand.IPacket_OnData((int)(m_buf.ElementAt(3) | (m_buf.ElementAt(2) << 8) | (m_buf.ElementAt(1) << 16) | (m_buf.ElementAt(0) << 24)));
					}
					break;
				case 'X':
					if (m_revBytes)
					{
						m_currentCommand.IPacket_OnData((short)(m_buf.ElementAt(0) | (m_buf.ElementAt(1) << 8)));
					}
					else
					{
						m_currentCommand.IPacket_OnData((short)(m_buf.ElementAt(1) | (m_buf.ElementAt(0) << 8)));
					}
					break;
				case 'B':
					if (null == m_currentCommand)
					{
						m_currentCommand = m_fact.GetCommand((CommandId)m_buf.ElementAt(0));
					}
					else
					{
						m_currentCommand.IPacket_OnData((byte)m_buf.ElementAt(0));
					}
					break;
				case 'S':
					m_currentCommand.IPacket_OnData(_ParseString());
					break;
				case 'R':
					m_currentCommand.IPacket_OnData(m_buf.ToArray(), m_datalen);
					break;
				case 'F':
					m_currentCommand.IPacket_OnData((bool)(0 != m_buf.ElementAt(0)));
					break;
				case 'f':
					m_currentCommand.IPacket_OnData((float)Double.Parse(_ParseString()));
					break;
				case 'd':
					m_currentCommand.IPacket_OnData(Double.Parse(_ParseString()));
					break;
				case 't':
					if (m_revBytes)
					{
						year = (int)(m_buf.ElementAt(0) | (int)(m_buf.ElementAt(1) << 8));
					}
					else
					{
						year = (int)(m_buf.ElementAt(1) | (int)(m_buf.ElementAt(0) << 8));
					}
					m_currentCommand.IPacket_OnData
					(
						new DateTime
						(
							year, 
							(int)m_buf.ElementAt(2),
							(int)m_buf.ElementAt(3),
							(int)m_buf.ElementAt(4),
							(int)m_buf.ElementAt(5),
							(int)m_buf.ElementAt(6)
						)
					);
					break;
				case 'D':
					if (m_revBytes)
					{
						year = (int)(m_buf.ElementAt(0) | (int)(m_buf.ElementAt(1) << 8));
					}
					else
					{
						year = (int)(m_buf.ElementAt(1) | (int)(m_buf.ElementAt(0) << 8));
					}
					m_currentCommand.IPacket_OnData
					(
						new Date
						(
							year, 
							(int)m_buf.ElementAt(2),
							(int)m_buf.ElementAt(3)
						)
					);
					break;
				default:
					throw new Exception("Internal error");
			}

			if (m_currentCommand.IsComplete)
			{
				m_currentCommand.Execute(m_client.GetStream());
				m_fact.ReleaseCommand(m_currentCommand);
				m_currentCommand = null;
			}
			m_state = PacketReadState.PKT_DT;
		}

		private string _ParseString()
		{
			StringBuilder sbuf = new StringBuilder(m_datalen + 1);
			if ( 1 == m_charsize )
			{
				for (int x = 0; x < m_datalen; x++ )
				{
					sbuf.Append( (char)m_buf.ElementAt(x) );
				}
			}
			else if ( 2 == m_charsize )
			{
				for (int x = 0; x < m_datalen; x += 2 )
				{
					if ( m_revBytes )
					{
						sbuf.Append( (char)((m_buf.ElementAt(x)) | (m_buf.ElementAt(x+1) << 8)) );
					}
					else
					{
						sbuf.Append( (char)((m_buf.ElementAt(x+1)) | (m_buf.ElementAt(x) << 8)) );
					}
				}
			}
			else
			{
				if (m_revBytes)
				{
					sbuf.Append((char)(m_buf.ElementAt(0) | (m_buf.ElementAt(1) << 8) | (m_buf.ElementAt(2) << 16) | (m_buf.ElementAt(3) << 24)));
				}
				else
				{
					sbuf.Append((char)(m_buf.ElementAt(3) | (m_buf.ElementAt(2) << 8) | (m_buf.ElementAt(1) << 16) | (m_buf.ElementAt(0) << 24)));
				}
			}
			return sbuf.ToString();
		}
	}
}
