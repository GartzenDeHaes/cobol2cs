using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace DOR.Core.Net
{
	public class Email
	{
		private string m_subject;
		protected string m_body;
		private MailAddressCollection m_recipients;
		private MailAddressCollection m_ccRecipients;
		private EmailAddress m_fromAddress;
		private string m_fromName;
		private bool m_isBodyHtml;
		private List<string> m_attachFileNames = new List<string>();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="subject"></param>
		/// <param name="body"></param>
		/// <param name="recipients"></param>
		/// <param name="ccRecipients"></param>
		/// <param name="fromAddress"></param>
		/// <param name="isBodyHtml"></param>
		public Email
		(
			string subject,
			string body,
			MailAddressCollection recipients,
			MailAddressCollection ccRecipients,
			EmailAddress fromAddress,
			string fromName,
			bool isBodyHtml
		)
		{
			m_subject = subject;
			m_body = body;
			m_recipients = recipients;
			m_ccRecipients = ccRecipients;
			m_fromAddress = fromAddress;
			m_fromName = fromName;
			m_isBodyHtml = isBodyHtml;
		}

		public Email
		(
			string subject,
			string body,
			EmailAddress fromAddress,
			string fromName,
			bool isBodyHtml
		)
		{
			m_subject = subject;
			m_body = body;
			m_recipients = new MailAddressCollection();
			m_ccRecipients = new MailAddressCollection();
			m_fromAddress = fromAddress;
			m_fromName = fromName;
			m_isBodyHtml = isBodyHtml;
		}

		public void AddAttachement(string filename)
		{
			m_attachFileNames.Add(filename);
		}

		public bool IsBodyHtml
		{
			get { return m_isBodyHtml; }
		}

		public EmailAddress FromAddress
		{
			get { return m_fromAddress; }
		}

		public string FromName
		{
			get { return m_fromName; }
		}

		public string Subject
		{
			get { return m_subject; }
		}

		public virtual string Body
		{
			get { return m_body; }
		}

		public MailAddressCollection Recipients
		{
			get { return m_recipients; }
		}

		public MailAddressCollection CcRecipients
		{
			get { return m_ccRecipients; }
		}

		public virtual void Send(string smtpServer = null)
		{
			MailMessage message = new MailMessage();
			foreach (MailAddress ccRecipient in CcRecipients)
			{
				message.CC.Add(ccRecipient);
			}
			foreach (MailAddress recipient in Recipients)
			{
				message.To.Add(recipient);
			}
			message.Body = Body;
			message.From = new MailAddress(m_fromAddress.Email, m_fromName);
			message.Subject = Subject;
			message.IsBodyHtml = IsBodyHtml;

			foreach (string attchfn in m_attachFileNames)
			{
				message.Attachments.Add(new Attachment(attchfn));
			}

			SmtpClient client;
			if (smtpServer == null)
			{
				client = new SmtpClient();
			}
			else
			{
				client = new SmtpClient(smtpServer);
			}
			client.Send(message);
			client = null;
		}
	}
}