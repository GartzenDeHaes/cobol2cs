using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Net.Mail;

using DOR.Core.Net;

namespace DOR.Core.Logging
{
	/// <summary>
	/// Abstract base class for the Control and Exception reports.
	/// </summary>
	/// <remarks>Each report uses two files: the current report and a monthly archive.</remarks>
	public abstract class Report
	{
		private readonly string m_fileName;
		private readonly string m_archiveFileName;

		protected DateTime m_inception;	// when the run started
		protected string m_processName;
		
		internal Report( string path, string procName, string fileSuffix )
		{
			DateTime now = DateTime.Now;
			m_processName = procName;
			
            string dirPath = Directory.GetCurrentDirectory() + "\\" + path;
			if (!Directory.Exists(dirPath))
			{
				Directory.CreateDirectory(dirPath);
			}

			if (!dirPath.EndsWith("\\"))
			{
				dirPath += "\\";
			}

            m_fileName = dirPath + "_" + procName + fileSuffix;
            m_archiveFileName = String.Format
			(
				"{0}{1}_{2}{3}{4}",
				dirPath, 
				procName, 
				now.Year.ToString("0000"), 
				now.Month.ToString("00"), 
				fileSuffix
			);

            // Remove old report
			if (File.Exists(m_fileName))
			{
				File.Delete(m_fileName);
			}
		}

        internal Report(StringBuilder fullpath, string procName, string fileSuffix)
        {
            DateTime now = DateTime.Now;
            m_processName = procName;

			if (!Directory.Exists(fullpath.ToString()))
			{
				Directory.CreateDirectory(fullpath.ToString());
			}

			if (!fullpath.ToString().EndsWith("\\"))
			{
				fullpath.Append("\\");
			}

            m_fileName = fullpath.ToString() + "_" + procName + fileSuffix;
			m_archiveFileName = String.Format
			(
				"{0}{1}_{2}{3}{4}",
				fullpath.ToString(),
				procName,
				now.Year.ToString("0000"),
				now.Month.ToString("00"),
				fileSuffix
			);

            // Remove old report
			if (File.Exists(m_fileName))
			{
				File.Delete(m_fileName);
			}
        }

		protected Report()
		{
		}

        public string Filename
        {
            get
            {
                return m_fileName;
            }
        }

		public abstract void AddMessage( string msg );

		protected virtual void Write( string s )
		{
			StreamWriter writer = File.AppendText( m_fileName );
			writer.Write( s );
			writer.Close();

			writer = File.AppendText( m_archiveFileName );
			writer.Write( s );
			writer.Close();
		}

		protected void EmailReport(string smptServer, EmailAddress to, EmailAddress from, string subject)
		{
			List<EmailAddress> toList = new List<EmailAddress>();
			toList.Add(to);
			EmailReport(smptServer, toList, from, subject);
		}

		protected void EmailReport(EmailAddress to, EmailAddress from, string subject)
		{
			List<EmailAddress> toList = new List<EmailAddress>();
			toList.Add(to);
			EmailReport(null, toList, from, subject);
		}

		protected void EmailReport(string smptServer, List<EmailAddress> toList, EmailAddress from, string subject)
		{
			if (File.Exists(m_fileName))
			{
				MailAddressCollection toMailAddrCol = new MailAddressCollection();
				foreach (EmailAddress ea in toList)
				{
					toMailAddrCol.Add(ea.Email);
				}
				string text = File.ReadAllText(m_fileName);
				Email email = new Email(subject, text, toMailAddrCol, new MailAddressCollection(), from, m_processName, false);
				if (smptServer == null)
				{
					email.Send();
				}
				else
				{
					email.Send(smptServer);
				}
			}
		}
	}

	/// <summary>
	/// The exception report outputs information about each exception
	/// </summary>
	public class ExceptionReport : Report
	{
		private const string m_filesuffix = "_EXCPT.txt";
        private int m_errorCount;

		public ExceptionReport
		( 
			string path, 
			string processName
		)
		: base( path, processName, m_filesuffix )
		{
			m_processName = processName;
		}

        public ExceptionReport
        (
            StringBuilder fullpath,
            string processName
        )
        : base(fullpath, processName, m_filesuffix)
        {
            m_processName = processName;
        }

		protected ExceptionReport()
		{
		}

		public int ErrorCount
		{
			get	
			{ 
				return m_errorCount; 
			}
		}

        public bool HasErrors
        {
            get
            {
                return m_errorCount > 0;
            }
        }

		public override void AddMessage( string msg )
		{
			AppendError( msg );
		}

		public void AppendError( Exception ex )
		{
			AppendError ( ex.ToString() );
		}

		public void AppendError( string msg )
		{
			StringBuilder buf = new StringBuilder(40);

			if ( m_errorCount == 0 )
			{
				// write exception report header
				buf.Append( "\r\n*******************************************************************\r\n PROCESS: " );
				buf.Append( m_processName );
				buf.Append( "\r\n" );
				buf.Append( " DATE: " );
				buf.Append( DateTime.Now.ToString() );
				buf.Append( "\r\n\r\n" );
			}
			m_errorCount++;
			buf.Append( " " + msg.Replace("\n", "\n\t") );
			buf.Append( "\r\n" );
			Write( buf.ToString() );
		}

		public void RunComplete()
		{
			if ( m_errorCount > 0 )
			{
				Write("\r\n*******************************************************************");
			}
		}

		public void EmailExceptionReport(EmailAddress to, EmailAddress from)
		{
			EmailExceptionReport(null, to, from);
		}

		public void EmailExceptionReport(string smtpServer, EmailAddress to, EmailAddress from)
		{
			List<EmailAddress> toList = new List<EmailAddress>();
			toList.Add(to);

			EmailExceptionReport(smtpServer, toList, from);
		}

		public void EmailExceptionReport(List<EmailAddress> to, EmailAddress from)
		{
			EmailExceptionReport(null, to, from);
		}

		public void EmailExceptionReport(string smtpServer, List<EmailAddress> to, EmailAddress from)
		{
			if (ErrorCount > 0)
			{
				EmailReport(smtpServer, to, from, m_processName + " " + DateTime.Now.ToString("MM/dd/yyyy HH:mm") + " Exception Report");
			}
		}
	}

	/// <summary>
	/// The control report outputs a summary of the run including whether there werer any exceptions
	/// </summary>
	public class ControlReport : Report
	{
		private const string m_filesuffix = "_CNTRL.txt";

		private int m_messageCount;
		private readonly ExceptionReport m_exceptionReport;

        /// <summary>
        /// Do not use. It is used for Unit Testing only.
        /// </summary>
        protected ControlReport(ExceptionReport exr) 
		{
			m_exceptionReport = exr;
		}

        /// <summary>
        /// Stores the control report in a sub directory of the current directory
        /// </summary>
        /// <param name="path"></param>
        /// <param name="processName"></param>
		public ControlReport
		( 
			string path, 
			string processName
		)
		: base( path, processName, m_filesuffix )
		{
			m_processName = processName;
			
			Write
			( 
				string.Format
				(
					"\r\n PROCESS: {0}\r\n BEGIN: {1}\r\n\r\n", 
					processName, 
					DateTime.Now.ToString()
				) 
			);

			m_exceptionReport = new ExceptionReport(path, processName);
		}

        /// <summary>
        /// Stores the control report at the full path location passed in.
        /// </summary>
        /// <param name="fullpath"></param>
        /// <param name="processName"></param>
        public ControlReport
        (
            StringBuilder fullpath,
            string processName
        )
        : base(fullpath, processName, m_filesuffix)
        {
            m_processName = processName;

            Write
			(
				string.Format
				(
					"\r\n PROCESS: {0}\r\n BEGIN: {1}\r\n\r\n", 
					processName, DateTime.Now.ToString()
				)
			);
            
			m_exceptionReport = new ExceptionReport(fullpath, processName);
        }

        public ExceptionReport ExceptionReport
        {
            get
            {
                return m_exceptionReport;
            }
        }

		public virtual int ErrorCount
		{
			get
			{
				return m_exceptionReport.ErrorCount;
			}
		}

        public bool HasErrors
        {
            get
            {
                return m_exceptionReport.HasErrors;
            }
        }

		public virtual void AddError( Exception ex )
		{
			m_exceptionReport.AppendError( ex );
		}

		public virtual void AddError( string msg )
		{
			m_exceptionReport.AppendError( msg );
		}

		/// <summary>
		/// Add non-error message to the control report.
		/// </summary>
		public override void AddMessage( string msg )
		{
            Write(" " + msg + "\r\n");
			m_messageCount++;
		}

		/// <summary>
		/// Call to write and close the reports.
		/// </summary>
		public virtual void RunComplete()
		{
			m_exceptionReport.RunComplete();
			
			Write( string.Format("\r\n END: {0}\r\n\r\n", DateTime.Now.ToString()));

			if ( ErrorCount > 0 )
			{
				Write( "\r\n" );
				AddMessage("!!! ERRORS IN PROCESSING: SEE EXCEPTION REPORT !!!");
				Write( "\r\n\r\n" );
			}
			else
			{
				Write( "\r\n" );
				AddMessage( "CONTROL REPORT IN BALANCE" );
				Write( "\r\n\r\n" );
			}
			Write("*******************************************************************\r\n");
		}

		public void EmailControlReport(string smtpServer, EmailAddress to, EmailAddress from, string appendedSubject)
		{
			// Add ability to append to the subject line
			string subject = m_processName + " " + DateTime.Now.ToString("MM/dd/yyyy HH:mm") + (ErrorCount > 0 ? " ERRORS IN PROCESSING" : " IN BALANCE");
			if (appendedSubject.Length > 0)
			{
				subject += " - " + appendedSubject;
			}

			if (HasErrors)
			{
				EmailExceptionReport(smtpServer, to, from);
			}
			else
			{
				EmailReport(smtpServer, to, from, subject);
			}
		}

		public void EmailControlReport(string smtpServer, EmailAddress to, EmailAddress from)
		{
			EmailControlReport(smtpServer, to, from, string.Empty);
		}

		public void EmailControlReport(EmailAddress to, EmailAddress from)
		{
			EmailControlReport(null, to, from, string.Empty);
		}

		public void EmailControlReport(List<EmailAddress> to, EmailAddress from)
		{
			foreach (var t in to)
			{
				EmailControlReport(null, t, from, string.Empty);
			}
		}

		public void EmailControlReport(string smtpServer, string[] toEmails, EmailAddress from, string appendedSubject)
		{
			List<EmailAddress> recipients = new List<EmailAddress>();
			foreach (string email in toEmails)
			{
				recipients.Add(EmailAddress.Parse(email));
			}

			// Add ability to append to the subject line
			string subject = m_processName + " " + DateTime.Now.ToString("MM/dd/yyyy HH:mm") + (ErrorCount > 0 ? " ERRORS IN PROCESSING" : " IN BALANCE");
			if (appendedSubject.Length > 0)
				subject += " - " + appendedSubject;

			EmailReport(smtpServer, recipients, from, subject);
		}

		public void EmailControlReport(string smtpServer, string[] toEmails, EmailAddress from)
		{
			EmailControlReport(smtpServer, toEmails, from, string.Empty);

			//List<EmailAddress> recipients = new List<EmailAddress>();
			//foreach (string email in toEmails)
			//{
			//    recipients.Add(EmailAddress.Parse(email));
			//}

			//EmailReport(smtpServer, recipients, from, m_processName + " " + DateTime.Now.ToString("MM/dd/yyyy HH:mm") + (ErrorCount > 0 ? " ERRORS IN PROCESSING" : " IN BALANCE"));
		}

		public void EmailExceptionReport(EmailAddress to, EmailAddress from)
		{
			m_exceptionReport.EmailExceptionReport(to, from);
		}

		public void EmailExceptionReport(string smtpServer, EmailAddress to, EmailAddress from)
		{
			m_exceptionReport.EmailExceptionReport(smtpServer, to, from);
		}

		public void EmailExceptionReport(string smtpServer, string[] toEmails, EmailAddress from)
		{
			List<EmailAddress> recipients = new List<EmailAddress>();
			foreach (string email in toEmails)
			{
				recipients.Add(EmailAddress.Parse(email));
			}
			m_exceptionReport.EmailExceptionReport(smtpServer, recipients, from);
		}
	}
}
