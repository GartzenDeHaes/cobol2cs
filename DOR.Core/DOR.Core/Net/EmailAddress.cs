using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace DOR.Core.Net
{
	/// <summary>
	/// Validation and holder for email address.
	/// </summary>
	[Serializable]
	public class EmailAddress : IEmailAddress
	{
		private string m_email;

		/// <summary>
		/// Don't contruct, use Parse instead
		/// </summary>
		/// <param name="email"></param>
		private EmailAddress(string email)
		{
			m_email = email;
		}

		/// <summary>
		/// Create an email instance -- throws exception if invalid.
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		public static EmailAddress Parse(string email)
		{
			if (String.IsNullOrEmpty(email))
			{
				return null;
			}
			if (!IsEmail(email))
			{
				throw new ArgumentException("Invalid email address");
			}
			return new EmailAddress(email);
		}

		public static List<EmailAddress> ParseList(string email, char delim)
		{
			List<EmailAddress> lst = new List<EmailAddress>();
			string[] parts = email.Split(new char[] { delim });

			for (int x = 0; x < parts.Length; x++)
			{
				lst.Add(EmailAddress.Parse(parts[x]));
			}

			return lst;
		}

		/// <summary>
		/// The full email address.
		/// </summary>
		public string Email
		{
			get { return m_email; }
		}

		/// <summary>
		/// The host DNS or IP.
		/// </summary>
		public string Host
		{
			get { return m_email.Split('@')[1]; }
		}

		/// <summary>
		/// The user ID.
		/// </summary>
		public string User
		{
			get { return m_email.Split('@')[0]; }
		}

		/// <summary>
		/// The email.
		/// </summary>
		public override string ToString()
		{
			return m_email;
		}

		private static Regex _verifyRegx = new Regex
		(
			@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
			@"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
			@".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"
		);

		/// <summary>
		/// Validate the email address.
		/// </summary>
		/// <param name="email"></param>
		/// <param name="verifyServer">If true, also verify the email's host.</param>
		/// <returns></returns>
		public static bool IsEmail(string email)
		{
			if (String.IsNullOrEmpty(email))
			{
				return false;
			}

			return _verifyRegx.IsMatch(email);
		}
	}
}
