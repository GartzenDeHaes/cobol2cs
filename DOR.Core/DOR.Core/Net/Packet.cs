using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using DOR.Core;
using DOR.Core.Collections;

namespace DOR.Core.Net
{
	public class Packet
	{
		private Vector<byte> m_buf = new Vector<byte>();

		public Packet()
		{
			Clear();
		}

		public void Clear()
		{
			m_buf.Clear();
			m_buf.Add(1);	// little endian
			m_buf.Add(0);	// byte count MCB
			m_buf.Add(0);	// byte count LCB
			m_buf.Add(63);	// packet check byte
		}

		public void Send(Stream sock)
		{
			short count = (short)m_buf.Count;
			Debug.Assert(0 == m_buf.ElementAt(1));
			Debug.Assert(0 == m_buf.ElementAt(2));
			m_buf.SetElementAt( 1, (byte)(count >> 8) );
			m_buf.SetElementAt( 2, (byte)(count & 0xFF) );

			sock.Write( m_buf.ToArray(), 0, m_buf.Count );
		}

		private void _AppendRaw(byte i)
		{
			m_buf.Add(i);
		}

		private void _AppendRaw(short i)
		{
			m_buf.Add((byte)((i >> 8) & 0xFF));
			m_buf.Add((byte)(i & 0xFF));
		}

		private void _AppendRaw(int i)
		{
			m_buf.Add((byte)(i >> 24));
			m_buf.Add((byte)((i >> 16) & 0xFF));
			m_buf.Add((byte)((i >> 8) & 0xFF));
			m_buf.Add((byte)(i & 0xFF));
		}

		private void _AppendRaw(long i)
		{
			m_buf.Add((byte)((i >> 56) & 0xFF));
			m_buf.Add((byte)((i >> 48) & 0xFF));
			m_buf.Add((byte)((i >> 40) & 0xFF));
			m_buf.Add((byte)((i >> 32) & 0xFF));
			m_buf.Add((byte)((i >> 24) & 0xFF));
			m_buf.Add((byte)((i >> 16) & 0xFF));
			m_buf.Add((byte)((i >> 8) & 0xFF));
			m_buf.Add((byte)(i & 0xFF));
		}

		public int PacketSize
		{
			get { return m_buf.Count; }
		}

		public void Append(long i)
		{
			m_buf.Add((byte)'L');
			_AppendRaw(i);
			Debug.Assert(0 == m_buf.ElementAt(1));
			Debug.Assert(0 == m_buf.ElementAt(2));
		}

		public void Append(int i)
		{
			m_buf.Add( (byte)'I' );
			_AppendRaw( i );
			Debug.Assert(0 == m_buf.ElementAt(1));
			Debug.Assert(0 == m_buf.ElementAt(2));
		}

		public void Append(short i)
		{
			m_buf.Add( (byte)'X' );
			_AppendRaw( i );
			Debug.Assert(0 == m_buf.ElementAt(1));
			Debug.Assert(0 == m_buf.ElementAt(2));
		}

		public void Append(byte i)
		{
			m_buf.Add( (byte)'B' );
			m_buf.Add( i );
			Debug.Assert(0 == m_buf.ElementAt(1));
			Debug.Assert(0 == m_buf.ElementAt(2));
		}

		public void Append(bool b)
		{
			m_buf.Add( (byte)'F' );
			m_buf.Add( (byte)((b)?1:0) );
			Debug.Assert(0 == m_buf.ElementAt(1));
			Debug.Assert(0 == m_buf.ElementAt(2));
		}

		public void Append(string str)
		{
			Append(str, str.Length);
		}

		public void AppendPair(string cmd, string prm)
		{
			Append(cmd, cmd.Length);
			Append(prm, prm.Length);
		}

		public void Append(string str, int count)
		{
			Debug.Assert(str.Length < Int16.MaxValue);

			// append datatype
			m_buf.Add( (byte)'S' );

			// append the number of characters
			int numchar = count;
			_AppendRaw( (short)count );

			// append the size of each char
			byte charsize = (byte)1;
			_AppendRaw( charsize );

			byte[] bytes = UTF8Encoding.UTF8.GetBytes(str);
			for ( int x = 0; x < numchar; x++ )
			{
				m_buf.Add(bytes[x]);
			}

			Debug.Assert(0 == m_buf.ElementAt(1));
			Debug.Assert(0 == m_buf.ElementAt(2));
		}

		public void Append(byte[] buf)
		{
			Append(buf, 0, buf.Length);
		}

		public void Append(byte[] buf, int len)
		{
			Append(buf, 0, len);
		}

		public void Append(byte[] buf, int start, int len)
		{
			Debug.Assert(buf.Length < Int16.MaxValue);
			_AppendRaw( (byte)'R' );
			_AppendRaw( (short)(len - start) );

			for ( int x = start; x < start+len; x++ )
			{
				_AppendRaw( buf[x] );
			}
			Debug.Assert(0 == m_buf.ElementAt(1));
			Debug.Assert(0 == m_buf.ElementAt(2));
		}

		public void Append(float f)
		{
			string buf = f.ToString();
			Debug.Assert(buf.Length < 128);
			
			_AppendRaw( (byte)'f' );
			_AppendRaw( (byte)buf.Length );

			for ( int x = 0; x < buf.Length; x++ )
			{
				m_buf.Add( (byte)buf[x] );
			}
			Debug.Assert(0 == m_buf.ElementAt(1));
			Debug.Assert(0 == m_buf.ElementAt(2));
		}

		public void Append(double d)
		{
			string buf = d.ToString("F15");
			Debug.Assert(buf.Length < 128);
			
			_AppendRaw( (byte)'d' );
			_AppendRaw( (byte)buf.Length );

			for ( int x = 0; x < buf.Length; x++ )
			{
				m_buf.Add( (byte)buf[x] );
			}
			Debug.Assert(0 == m_buf.ElementAt(1));
			Debug.Assert(0 == m_buf.ElementAt(2));
		}

		public void Append(DateTime dtm)
		{
			_AppendRaw( (byte)'t' );
			_AppendRaw( (short)dtm.Year );
			_AppendRaw( (byte)dtm.Month );
			_AppendRaw( (byte)dtm.Day );
			_AppendRaw( (byte)dtm.Hour );
			_AppendRaw( (byte)dtm.Minute );
			_AppendRaw( (byte)dtm.Second );
		}

		public void Append(Date dt)
		{
			_AppendRaw( (byte)'D' );
			_AppendRaw( (short)dt.Year );
			_AppendRaw( (byte)dt.Month );
			_AppendRaw( (byte)dt.Day );
		}

		public void Append(object d)
		{
			if (d is Int64)
			{
				Append((long)d);
			}
			else if (d is Int32)
			{
				Append((int)d);
			}
			else if (d is Double)
			{
				Append((double)d);
			}
			else if (d is Decimal)
			{
				Append((decimal)d);
			}
			else if (d is String)
			{
				Append((string)d);
			}
			else
			{
				throw new ArgumentException("Can't down cast generic " + d.ToString());
			}
		}

		public long GetLongHashCode()
		{
			return StringHelper.GetLongHashCode(m_buf);
		}
	}
}
