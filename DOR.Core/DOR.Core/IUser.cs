using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core.Net;
using Personnel.Model;

namespace DOR.Core
{
	/// <summary>
	/// User and employee interface.
	/// </summary>
	public interface IUser
	{
		/// <summary>Agency number</summary>
		int AgencyId { get; }

		/// <summary>Windows domain name</summary>
		string Domain { get; }

		/// <summary>Windows logon ID</summary>
		string LogonId { get; }

		string GuardianId { get; }

		/// <summary>Windows domain and logon</summary>
		string LogonIdWithDomain { get; }

		/// <summary>Tandem user ID</summary>
		int DorUserId { get; }

		int PositionNumber { get; }

		int OrgFundingCode { get; }

		string JobClassCode { get; }

		string JobClassName { get; }

		string SuffixName { get; }

		/// <summary>
		/// First name.  This is the perfered name from AD if available.
		/// </summary>
		string FirstName { get; }

		/// <summary>
		/// Middle.  This is the perfered name from AD if available.
		/// </summary>
		string MiddleInitial { get; }

		/// <summary>
		/// Last.  This is the perfered name from AD if available.
		/// </summary>
		string LastName { get; }

		/// <summary>
		/// Formated name.
		/// </summary>
		string FullName { get; }

		/// <summary>Full namem without the middle initial.</summary>
		string LastNameFirstName { get; }

		string Initials { get; }

		/// <summary>Fully qualified email address.</summary>
		IEmailAddress Email { get; }

		/// <summary>Desk phone number.</summary>
		PhoneNumber Phone { get; }

		IPhoneNumber Fax { get; }

		/// <summary>Department centeralized PO box address.</summary>
		IPostalAddress MailingAddress { get; }

		/// <summary>Address of the employee's workplace.</summary>
		IPostalAddress ShippingAddress { get; }

		/// <summary>DOR division name.</summary>
		string DivisionName { get; }

		string DivisionNameAbbr { get; }

		/// <summary>DOR division lookup code.</summary>
		DivisionCode DivisionCode { get; }

		int SectionCode { get; }

		/// <summary>The employee's supervisor Tandem user ID.</summary>
		int SupervisorId { get; }

		/// <summary>First name of the employee's supervisor.</summary>
		string SupervisorFirstName { get; }

		/// <summary>Last name of the employee's supervisor.</summary>
		string SupervisorLastName { get; }

		int SupervisorPositionNumber { get; }

		/// <summary>Returns true if the employee has an attendance unit.</summary>
		bool IsSupervisor { get; }

		string AttendanceUnit { get; }

		/// <summary>
		/// Returns true if the employee has the AD flag set, or is authorized to
		/// act on behaf of an AD (like an AA).
		/// </summary>
		bool IsAssistantDirector { get; }

		/// <summary></summary>
		Date CreatedOn { get; }

		Date ActiveDate { get; }

		Date InactiveDate { get; }

		string LastGuardianLogonDate { get; }

		int EmploymentStatusCode { get; }

		int ActiveSrvrqQty { get; }

		string WorkScheduleCode { get; }

		bool HasVPN { get; }

		int ComplianceRegion { get; }
		int ComplianceDistrict { get; }
		int ComplianceSubDistrict { get; }
	}
}
