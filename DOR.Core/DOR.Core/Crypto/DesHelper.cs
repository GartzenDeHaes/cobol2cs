using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DOR.Core.Crypto
{
	public static class DesHelper
	{
		public static string Encrypt(string txt, string pw)
		{
			return EncryptionHelper.Encrypt(txt, pw, 64, pw, 64, new DESCryptoServiceProvider());
		}

		public static string Decrypt(string enctxt, string pw)
		{
			return EncryptionHelper.Decrypt(enctxt, pw, 64, pw, 64, new DESCryptoServiceProvider());
		}
	}
}
