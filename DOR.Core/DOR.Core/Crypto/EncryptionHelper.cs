using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DOR.Core.Crypto
{
	public static class EncryptionHelper
	{
		private static byte[] HashKey(byte[] key, int bits)
		{
			byte[] hash = new byte[bits / 8];

			for (int x = 0; x < hash.Length && x < key.Length; x++)
			{
				hash[x] = key[x];
			}

			if (hash.Length > key.Length)
			{
				return hash;
			}

			int hashPos = 0;

			for (int x = hash.Length; x < key.Length; x++)
			{
				hash[hashPos % hash.Length] ^= key[x];
			}

			return hash;
		}

		public static string Encrypt
		(
			string txt, 
			string key, 
			int keyBits,
			string iv,
			int ivBits,
			SymmetricAlgorithm provider
		)
		{
			// Encode message and password
			byte[] messageBytes = UTF8Encoding.UTF8.GetBytes(txt);
			byte[] passwordBytes = UTF8Encoding.UTF8.GetBytes(key);
			passwordBytes = HashKey(passwordBytes, keyBits);

			byte[] ivBytes = UTF8Encoding.UTF8.GetBytes(iv);
			ivBytes = HashKey(ivBytes, ivBits);

			// Set encryption settings -- Use password for both key and init. vector
			ICryptoTransform transform = provider.CreateEncryptor(passwordBytes, ivBytes);
			CryptoStreamMode mode = CryptoStreamMode.Write;

			// Set up streams and encrypt
			MemoryStream memStream = new MemoryStream();
			CryptoStream cryptoStream = new CryptoStream(memStream, transform, mode);
			cryptoStream.Write(messageBytes, 0, messageBytes.Length);
			cryptoStream.FlushFinalBlock();

			// Read the encrypted message from the memory stream
			byte[] encryptedMessageBytes = new byte[memStream.Length];
			memStream.Position = 0;
			memStream.Read(encryptedMessageBytes, 0, encryptedMessageBytes.Length);

			// Encode the encrypted message as base64 string
			return Convert.ToBase64String(encryptedMessageBytes);
		}

		public static string Decrypt
		(
			string enctxt, 
			string key, 
			int keyBits,
			string iv,
			int ivBits,
			SymmetricAlgorithm provider
		)
		{
			// Convert encrypted message and password to bytes
			byte[] encryptedMessageBytes = Convert.FromBase64String(enctxt);
			byte[] passwordBytes = UTF8Encoding.UTF8.GetBytes(key);
			passwordBytes = HashKey(passwordBytes, keyBits);

			byte[] ivBytes = UTF8Encoding.UTF8.GetBytes(iv);
			ivBytes = HashKey(ivBytes, ivBits);

			// Set encryption settings -- Use password for both key and init. vector
			ICryptoTransform transform = provider.CreateDecryptor(passwordBytes, ivBytes);
			CryptoStreamMode mode = CryptoStreamMode.Write;

			// Set up streams and decrypt
			MemoryStream memStream = new MemoryStream();
			CryptoStream cryptoStream = new CryptoStream(memStream, transform, mode);
			cryptoStream.Write(encryptedMessageBytes, 0, encryptedMessageBytes.Length);
			cryptoStream.FlushFinalBlock();

			// Read decrypted message from memory stream
			byte[] decryptedMessageBytes = new byte[memStream.Length];
			memStream.Position = 0;
			memStream.Read(decryptedMessageBytes, 0, decryptedMessageBytes.Length);

			var ar = memStream.ToArray();
			return UTF8Encoding.UTF8.GetString(ar, 0, ar.Length);
		}
	}
}
