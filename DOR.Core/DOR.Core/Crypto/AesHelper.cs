using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DOR.Core.Crypto
{
	public static class AesHelper
	{
		private static readonly string _iv = "()7(*&7a*(7(*Z)&";

		public static string Encrypt(string txt, string pw)
		{
			return EncryptionHelper.Encrypt(txt, pw, 256, _iv, 128, new AesCryptoServiceProvider());
		}

		public static string Decrypt(string enctxt, string pw)
		{
			return EncryptionHelper.Decrypt(enctxt, pw, 256, _iv, 128, new AesCryptoServiceProvider());
		}
	}
}
