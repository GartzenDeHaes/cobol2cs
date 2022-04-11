using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace DOR.Core.Net
{
	public class EmailMeetingRequest : Email
	{
		private DateTime m_startTime;
		private DateTime m_endTime;
		private string m_location;
		private bool m_setReminder;
		private int m_minutesBeforeStart;
		private Guid m_guid;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="startTime">must be less then endTime</param>
		/// <param name="endTime"></param>
		/// <param name="subject"></param>
		/// <param name="body"></param>
		/// <param name="location"></param>
		/// <param name="requiredAttendees">must contain one valid email address</param>
		/// <param name="optionalAttendees"></param>
		/// <param name="setReminder"></param>
		/// <param name="minutesBeforeStart">must be greater then -1 and less then 20,161 </param>
		/// <param name="organizerEmail"></param>
		/// <param name="organizerName"></param>
		/// <param name="guid">if an empty string is passed a guid will be generated and set</param>
		public EmailMeetingRequest
		(
			DateTime startTime,
			DateTime endTime,
			string subject,
			string body,
			string location,
			bool setReminder,
			int minutesBeforeStart,
			EmailAddress organizerEmail,
			string organizerName
		)
			: base(subject, body, organizerEmail, organizerName, true)
		{
			if (startTime > endTime)
			{
				throw new ArgumentException("startTime must be less then or equal to endTime");
			}
			m_startTime = startTime;
			m_endTime = endTime;
			m_location = location;
			m_setReminder = setReminder;
			if (minutesBeforeStart > 20160)
			{
				throw new ArgumentException("minutesBeforeStart must be less then or equal to 20,160");
			}
			if (minutesBeforeStart < 0)
			{
				throw new ArgumentException("minutesBeforeStart must not be less then 0 (zero)");
			}
			m_minutesBeforeStart = minutesBeforeStart;
			m_guid = Guid.NewGuid();
		}

		public Guid GUID
		{
			get { return m_guid; }
			set { m_guid = value; }
		}

		public DateTime StartTime
		{
			get { return m_startTime; }
		}

		public DateTime EndTime
		{
			get { return m_endTime; }
		}

		public string Location
		{
			get { return m_location; }
		}

		public bool SetReminder
		{
			get { return m_setReminder; }
		}

		public int MinutesBeforeStart
		{
			get { return m_minutesBeforeStart; }
		}

		public override void Send(string smtpServer)
		{
			int hours = 0;
			int minutes = 0;
			if (MinutesBeforeStart > 60)
			{
				minutes = MinutesBeforeStart % 60;
				hours = MinutesBeforeStart / 60;
			}
			else
			{
				minutes = MinutesBeforeStart;
			}

			MailMessage msg = new MailMessage();

			System.Net.Mime.ContentType textType = new System.Net.Mime.ContentType("text/plain");
			System.Net.Mime.ContentType HTMLType = new System.Net.Mime.ContentType("text/html");
			System.Net.Mime.ContentType calendarType = new System.Net.Mime.ContentType("text/calendar");

			calendarType.Parameters.Add("method", "REQUEST");
			calendarType.Parameters.Add("name", "meeting.ics");

			string bodyText = GenerateTextBody
			(
				FromName,
				m_startTime.ToLongDateString() + " " + m_startTime.ToLongTimeString(),
				m_endTime.ToLongDateString() + " " + m_endTime.ToLongTimeString(),
				System.TimeZone.CurrentTimeZone.StandardName,
				m_location,
				Body
			);

			AlternateView textView = AlternateView.CreateAlternateViewFromString(bodyText, textType);
			msg.AlternateViews.Add(textView);

			string bodyHTML = GenerateHtmlBody
			(
				Subject,
				FromName,
				m_startTime.ToLongDateString() + " " + m_startTime.ToLongTimeString(),
				m_endTime.ToLongDateString() + " " + m_endTime.ToLongTimeString(),
				System.TimeZone.CurrentTimeZone.StandardName,
				m_location,
				Body
			);
			AlternateView HTMLView = AlternateView.CreateAlternateViewFromString(bodyHTML, HTMLType);
			msg.AlternateViews.Add(HTMLView);

			string calDateFormat = "yyyyMMddTHHmmssZ";

			string bodyCalendar = GenerateCalendarFormat
			(
				m_startTime.ToUniversalTime().ToString(calDateFormat),
				m_endTime.ToUniversalTime().ToString(calDateFormat),
				m_location,
				FromName,
				FromAddress.Email,
				GUID.ToString("B"),
				m_body,
				Subject,
				DateTime.Now.ToUniversalTime().ToString(calDateFormat),
				Recipients.ToString(),
				m_setReminder,
				hours.ToString("00"),
				minutes.ToString("00"),
				CcRecipients.ToString()
			);

			AlternateView calendarView = AlternateView.CreateAlternateViewFromString(bodyCalendar, calendarType);
			calendarView.TransferEncoding = TransferEncoding.SevenBit;
			msg.AlternateViews.Add(calendarView);

			msg.From = new MailAddress(FromAddress.Email, FromName);
			msg.Subject = Subject;

			foreach (MailAddress ccRecipient in CcRecipients)
			{
				msg.CC.Add(ccRecipient);
			}

			foreach (MailAddress recipient in Recipients)
			{
				msg.To.Add(recipient);
			}

			SmtpClient client = new SmtpClient(smtpServer);
			client.Send(msg);
			client = null;
		}


		private static string GenerateCalendarFormat
		(
			string startDateTime,
			string endDateTime,
			string location,
			string organizerName,
			string organizerEmail,
			string guid,
			string summary,
			string subject,
			string now,
			string attendeeList,
			bool setReminder,
			string reminderHours,
			string reminderMinutes,
			string optionalAttendeeList
		)
		{
			StringBuilder bodyCalendar = new StringBuilder();
			bodyCalendar.Append("BEGIN:VCALENDAR\r\n");
			bodyCalendar.Append("METHOD:REQUEST\r\n");
			bodyCalendar.Append("PRODID:Microsoft CDO for Microsoft Exchange\r\n");
			bodyCalendar.Append("VERSION:2.0\r\n");
			bodyCalendar.Append("BEGIN:VTIMEZONE\r\n");
			bodyCalendar.Append("TZID:(GMT-08:00) Pacific Time (US & Canada)\r\n");
			bodyCalendar.Append("X-MICROSOFT-CDO-TZID:11\r\n");
			bodyCalendar.Append("BEGIN:STANDARD\r\nDTSTART:16010101T020000\r\n");
			bodyCalendar.Append("TZOFFSETFROM:-0500\r\nTZOFFSETTO:-0600\r\n");
			bodyCalendar.Append("RRULE:FREQ=YEARLY;WKST=MO;INTERVAL=1;BYMONTH=11;BYDAY=1SU\r\n");
			bodyCalendar.Append("END:STANDARD\r\nBEGIN:DAYLIGHT\r\n");
			bodyCalendar.Append("DTSTART:16010101T020000\r\n");
			bodyCalendar.Append("TZOFFSETFROM:-0600\r\nTZOFFSETTO:-0500\r\n");
			bodyCalendar.Append("RRULE:FREQ=YEARLY;WKST=MO;INTERVAL=1;BYMONTH=3;BYDAY=2SU\r\n");
			bodyCalendar.Append("END:DAYLIGHT\r\n");
			bodyCalendar.Append("END:VTIMEZONE\r\n");
			bodyCalendar.Append("BEGIN:VEVENT\r\n");
			bodyCalendar.Append("DTSTAMP:{8}\r\n");
			bodyCalendar.Append("DTSTART:{0}\r\n");
			bodyCalendar.Append("SUMMARY:{7}\r\n");
			bodyCalendar.Append("UID:{5}\r\n");
			bodyCalendar.Append("ATTENDEE;CN=\"{9}\";ROLE=REQ-PARTICIPANT;PARTSTAT=NEEDS-ACTION;RSVP=FALSE:MAILTO:{9}\r\n");
			bodyCalendar.Append("ATTENDEE;CN=\"{12}\";ROLE=OPT-PARTICIPANT;PARTSTAT=NEEDS-ACTION;RSVP=FALSE:MAILTO:{12}\r\n");
			bodyCalendar.Append("ACTION;RSVP=TRUE;CN=\"{4}\":MAILTO:{4}\r\nORGANIZER;CN=\"{3}\":mailto:{4}\r\nLOCATION:{2}\r\n");
			bodyCalendar.Append("DTEND:{1}\r\n");
			bodyCalendar.Append("DESCRIPTION:{7}\\N\r\n");
			bodyCalendar.Append("SEQUENCE:1\r\n");
			bodyCalendar.Append("PRIORITY:5\r\n");
			bodyCalendar.Append("CLASS:\r\n");
			bodyCalendar.Append("CREATED:{8}\r\n");
			bodyCalendar.Append("LAST-MODIFIED:{8}\r\n");
			bodyCalendar.Append("STATUS:CONFIRMED\r\n");
			bodyCalendar.Append("TRANSP:OPAQUE\r\n");
			bodyCalendar.Append("X-MICROSOFT-CDO-BUSYSTATUS:BUSY\r\n");
			bodyCalendar.Append("X-MICROSOFT-CDO-INSTTYPE:0\r\n");
			bodyCalendar.Append("X-MICROSOFT-CDO-INTENDEDSTATUS:BUSY\r\n");
			bodyCalendar.Append("X-MICROSOFT-CDO-ALLDAYEVENT:FALSE\r\n");
			bodyCalendar.Append("X-MICROSOFT-CDO-IMPORTANCE:1\r\n");
			bodyCalendar.Append("X-MICROSOFT-CDO-OWNERAPPTID:-1\r\n");
			bodyCalendar.Append("X-MICROSOFT-CDO-ATTENDEE-CRITICAL-CHANGE:{8}\r\n");
			bodyCalendar.Append("X-MICROSOFT-CDO-OWNER-CRITICAL-CHANGE:{8}\r\n");

			if (setReminder)
			{
				bodyCalendar.Append("BEGIN:VALARM\r\n");
				bodyCalendar.Append("ACTION:DISPLAY\r\n");
				bodyCalendar.Append("DESCRIPTION:REMINDER\r\n");
				bodyCalendar.Append("TRIGGER;RELATED=START:-PT{10}H{11}M00S\r\n");
				bodyCalendar.Append("END:VALARM\r\n");
			}

			bodyCalendar.Append("END:VEVENT\r\n");
			bodyCalendar.Append("END:VCALENDAR\r\n");


			return string.Format
			(
				bodyCalendar.ToString(),
				/*{0}*/		startDateTime,
				/*{1}*/		endDateTime,
				/*{2}*/		location,
				/*{3}*/		organizerName,
				/*{4}*/ 	organizerEmail,
				/*{5}*/ 	guid,
				/*{6}*/ 	summary,
				/*{7}*/ 	subject,
				/*{8}*/ 	now,
				/*{9}*/ 	attendeeList,
				/*{10}*/	reminderHours,
				/*{11}*/	reminderMinutes,
				/*{12}*/	optionalAttendeeList
			); ;
		}

		private static string GenerateHtmlBody
		(
			string subject,
			string organizerName,
			string startDateTime,
			string endDateTime,
			string currentTimeZone,
			string location,
			string summary
		)
		{
			StringBuilder bodyHTML = new StringBuilder("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 3.2//EN\">\r\n");
			bodyHTML.Append("<HTML>\r\n");
			bodyHTML.Append("<HEAD>\r\n");
			bodyHTML.Append("<META HTTP-EQUIV=\"Content-Type\" CONTENT=\"text/html; charset=utf-8\">\r\n");
			bodyHTML.Append("<META NAME=\"Generator\" CONTENT=\"MS Exchange Server version 6.5.7652.24\">\r\n");
			bodyHTML.Append("<TITLE>{0}</TITLE>\r\n");
			bodyHTML.Append("</HEAD>\r\n<BODY>\r\n<!-- Converted from text/plain format -->\r\n");
			bodyHTML.Append("<P><FONT SIZE=2>Type:Single Meeting<BR>\r\nOrganizer:{1}<BR>\r\n");
			bodyHTML.Append("Start Time:{2}<BR>\r\n");
			bodyHTML.Append("End Time:{3}<BR>\r\n");
			bodyHTML.Append("Time Zone:{4}<BR>\r\n");
			bodyHTML.Append("Location:{5}<BR>\r\n");
			bodyHTML.Append("<BR>\r\n*~*~*~*~*~*~*~*~*~*<BR>\r\n");
			bodyHTML.Append("<BR>\r\n");
			bodyHTML.Append("{6}<BR>\r\n");
			bodyHTML.Append("</FONT>\r\n");
			bodyHTML.Append("</P>\r\n\r\n");
			bodyHTML.Append("</BODY>\r\n</HTML>");

			return string.Format
			(
				bodyHTML.ToString(),
				/*{0}*/		subject,
				/*{1}*/		organizerName,
				/*{2}*/		startDateTime,
				/*{3}*/		endDateTime,
				/*{4}*/ 	currentTimeZone,
				/*{5}*/ 	location,
				/*{6}*/ 	summary
			); ;
		}

		private static string GenerateTextBody
		(
			string organizerName,
			string startDateTime,
			string endDateTime,
			string currentTimeZone,
			string location,
			string summary
		)
		{
			StringBuilder bodyText = new StringBuilder("Type:Single Meeting\r\n");
			bodyText.Append("Organizer: {0}\r\n");
			bodyText.Append("Start Time:{1}\r\n");
			bodyText.Append("End Time:{2}\r\n");
			bodyText.Append("Time Zone:{3}\r\n");
			bodyText.Append("Location: {4}\r\n\r\n");
			bodyText.Append("*~*~*~*~*~*~*~*~*~*\r\n\r\n");
			bodyText.Append("{5}");

			return string.Format
			(
				bodyText.ToString(),
				/*{0}*/	organizerName,
				/*{1}*/	startDateTime,
				/*{2}*/	endDateTime,
				/*{3}*/	currentTimeZone,
				/*{4}*/	location,
				/*{5}*/	summary
			);
		}
	}
}
