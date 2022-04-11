using System;
using System.Collections.Generic;
using System.Text;

namespace DOR.Core.Crypto
{
	/// <summary>
	/// Simple block cypher used to mask data.  Not for use where real encryption is required.
	/// </summary>
	public class SimpleBlockCypher
	{
		private static int[] _cypherBlocksFrom = {14081, 32173, 18493, 41387, 55823, 43411, 48593, 11423};
		private static int[] _cypherBlocksTo   = {57859, 49031, 49199, 20011, 51803, 4973,  57787, 40093};

		private SimpleBlockCypher()
		{
		}

		public static string EncryptString(string plainText, string password)
		{
			int pwHash = HashString(password);
			int[] enc = new int[plainText.Length/2 + plainText.Length % 2];
			
			// Encrypt
			int pos = 0;
			for ( int x = 0; x < plainText.Length; x += 2 )
			{
				char ch1 = plainText[x];
				char ch2 = '\0';
				if ( x + 1 < plainText.Length )
				{
					ch2 = plainText[x + 1];
				}
				enc[pos++] = Encrypt16Bits((int)ch1 | (((int)ch2) << 8), pwHash);
			}
			
			// Bin to string
			StringBuilder encTextBuf = new StringBuilder();
			for ( int x = 0; x < enc.Length; x++ )
			{
				encTextBuf.Append(Bin2Text16(enc[x]));
			}
			return encTextBuf.ToString();
		}
		
		public static string DecryptString(string encText, string password)
		{
			int pwHash = HashString(password);
			
			// String to bin
			int[] enc = new int[encText.Length/4];
			int pos = 0;
			for ( int x = 0; x < encText.Length; x += 4 )
			{
				string hex = encText.Substring(x, 4);
				enc[pos++] = Text2Bin16(hex);
			}
			
			// Decrypt
			StringBuilder output = new StringBuilder();
			for ( int x = 0; x < enc.Length; x++ )
			{
				int data = Decrypt16Bits(enc[x], pwHash);
				char ch1 = (char)(data & 0xFF);
				char ch2 = (char)((data >> 8) & 0xFF);
				output.Append(ch1);
				output.Append(ch2);
			}
			
			return output.ToString();
		}

		public static int Encrypt16Bits(int data, int pwhash)
		{
			return RotateBits(data, pwhash, 1);
		}

		public static int Decrypt16Bits(int data, int pwhash)
		{
			return RotateBits(data, pwhash, -1);
		}

		private static int RotateBits(int val, int hash, int dir)
		{
			int mask = 1 | (1<<1) | (1<<2);
			int x;
			
			if ( 0 > dir )
			{
				x = 4;
			}
			else
			{
				x = 0;
			}
			while ( x >= 0 && x < 5 )
			{
				int bits = (hash & (mask << (x*3))) >> (x*3);
				if ( bits >= 8 )
				{
					throw new Exception("bit problem");
				}
				int blockf = _cypherBlocksFrom[bits];
				int blockt = _cypherBlocksTo[bits];
				if ( CountBits(blockf, 16) != CountBits(blockt, 16) )
				{
					throw new Exception("bits not equal at " + bits);
				}
				val = SwapBits(val, blockf, blockt, dir);
				
				x += dir;
			}
			return val;
		}

		private static string NibbleToHex(int nibble)
		{
			if ( nibble > 0xF )
			{
				throw new Exception("internal error");
			}
			switch ( nibble )
			{
			case 0:
				return "0";
			case 1:
				return "1";
			case 2:
				return "2";
			case 3:
				return "3";
			case 4:
				return "4";
			case 5:
				return "5";
			case 6:
				return "6";
			case 7:
				return "7";
			case 8:
				return "8";
			case 9:
				return "9";
			case 10:
				return "A";
			case 11:
				return "B";
			case 12:
				return "C";
			case 13:
				return "D";
			case 14:
				return "E";
			case 15:
				return "F";
			default:
				throw new Exception("NIBBLE error");
			}
		}

		public static string Bin2Text16(int val)
		{
			string result = NibbleToHex((val >> 12) & 0xF);
			result += NibbleToHex((val >> 8) & 0xF);
			result += NibbleToHex((val >> 4) & 0xF);
			result += NibbleToHex(val & 0xF);
			return result;
		}

		public static int Text2Bin16(string str)
		{
			return Int32.Parse(str, System.Globalization.NumberStyles.HexNumber);
			//return parseInt("0x" + str, 16);
		}

		public static string Bin2Text8(int val)
		{
			string result = NibbleToHex((val >> 4) & 0xF);
			result += NibbleToHex(val & 0xF);
			return result;
		}

		public static int Text2Bin8(string str)
		{
			return Int32.Parse("0x" + str);
			//return parseInt("0x" + str, 16);
		}

		private static int CountBits(int val, int bitlen)
		{
			int count = 0;
			for ( int x = 0; x < bitlen; x++ )
			{
				if ( (val & (1<<x)) != 0 )
				{
					count++;
				}
			}
			return count;
		}

		public static bool IsPrime(int n) 
		{
			bool prime = true;
			for (int i = 3; i <= Math.Sqrt(n); i += 2)
			{
				if ((n % i) == 0) 
				{
					prime = false;
					break;
				}
			}
			if (( (n%2) !=0 && prime && n > 2) || n == 2) 
			{
				return true;
			} 
			else 
			{
				return false;
			}
		}

		public static string ZeroPadLeft(int val, int len)
		{
			string sval = "" + val;
			while ( sval.Length < len )
			{
				sval = '0' + sval;
			}
			return sval;
		}

		public static int HashString(string str)
		{			
			int hash = 0;
			int c;
			for ( int x = 0; x < str.Length; x++ )
			{
				c = (int)str[x];
				hash = c + (hash << 6) + (hash << 16) - hash;
			}
			return hash;
		}

		private static int BitPos(int val, int pos, int dir)
		{
			for ( int x = pos+dir; x >= 0 && x < 32; x += dir )
			{
				if ( (val & (1<<x)) != 0 )
				{
					return x;
				}
			}
			throw new Exception("bitPos error");
		}

		private static int SwapBits(int val, int blockf, int blockt, int dir)
		{
			int bitcount = CountBits(blockf, 16);
			int fpos;
			int tpos;
			
			if ( 1 != dir && -1 != dir )
			{
				throw new Exception("swap bits dir must be 1 or -1");
			}
			if ( dir > 0 )
			{
				fpos = -1;
				tpos = -1;
			}
			else
			{
				fpos = 16;
				tpos = 16;
			}
			for ( int x = 0; x < bitcount; x++ )
			{
				fpos = BitPos(blockf, fpos, dir);
				tpos = BitPos(blockt, tpos, dir);
				int fmask = (1<<fpos);
				int tmask = (1 << tpos);
				int fbit = val & fmask;
				int tbit = val & tmask;
				if ( fbit != 0 )
				{
					val |= tmask;
				}
				else
				{
					val &= ~tmask;
				}
				if ( tbit != 0 )
				{
					val |= fmask;
				}
				else
				{
					val &= ~fmask;
				}
			}
			return val;
		}
	}
}
