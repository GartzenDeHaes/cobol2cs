using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;

using DOR.Core.Collections;

namespace DOR.Core.Net
{
	/// <summary>
	/// RSA public key encryption.
	/// </summary>
	public class RSAPublicKey
	{
		private int m_bitSize;
		private BigInteger m_modulus;
		private BigInteger m_exponent;

		public RSAPublicKey()
		{
		}

		public RSAPublicKey(BigInteger modulus, BigInteger exponent, int bitSize)
		{
			m_modulus = modulus;
			m_exponent = exponent;
			m_bitSize = bitSize;
		}

		public int BitSize
		{
			get { return m_bitSize; }
			set { m_bitSize = value; }
		}

		int InputBlockSize
		{
			get
			{
				return (m_bitSize - 1) / 8;
			}
		}

		public BigInteger Modulus
		{
			get { return m_modulus; }
			set { m_modulus = value; }			
		}

		public BigInteger Exponent
		{
			get { return m_exponent; }
			set { m_exponent = value; }
		}

		static private byte[] LongToBytes(long l)
		{
			byte[] bytes = new byte[8];

			bytes[0] = (byte)(l & 0xFF);
			bytes[1] = (byte)((l >> 8) & 0xFF);
			bytes[2] = (byte)((l >> 16) & 0xFF);
			bytes[3] = (byte)((l >> 24) & 0xFF);
			bytes[4] = (byte)((l >> 32) & 0xFF);
			bytes[5] = (byte)((l >> 40) & 0xFF);
			bytes[6] = (byte)((l >> 48) & 0xFF);
			bytes[7] = (byte)((l >> 56) & 0xFF);

			return bytes;
		}

		private byte[] ExtractRange(byte[] ar, int start, int len)
		{
			byte[] ret = new byte[len];

			for (int x = 0; x < len; x++)
			{
				ret[x] = ar[start + x];
			}

			return ret;
		}

		public byte[] EncryptBinary(byte[] plainText)
		{
			//Debug.Assert(m_bitSize != 0);

			Vector<byte> encBytes = new Vector<byte>();
			BigInteger enc;
			byte[] bytes;
			int byteCount = m_bitSize / 8 + (( m_bitSize % 8 ) == 0 ? 0 : 1 );

			for(int ptPos = 0; ptPos < plainText.Length; ptPos += InputBlockSize)
			{
				BigInteger txt = new BigInteger(ExtractRange(plainText, ptPos, InputBlockSize));

				enc = BigInteger.ModPow(txt, m_exponent, m_modulus);

				//bytes = LongToBytes(enc);
				bytes = enc.ToByteArray();

				encBytes.AddRangeWithPad(bytes, byteCount);

				Debug.Assert((encBytes.Count % byteCount) == 0);
			}

			return encBytes.ToArray();
		}

		public string EncryptText(byte[] plainText)
		{
			byte[] encBytes = EncryptBinary(plainText);
			return Convert.ToBase64String(encBytes);
		}

		public string EncryptText(string plainText)
		{
			return EncryptText(ASCIIEncoding.ASCII.GetBytes(plainText));
		}
	}
}
