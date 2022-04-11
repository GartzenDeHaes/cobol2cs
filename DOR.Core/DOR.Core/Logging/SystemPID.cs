using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core.Logging
{
	/// <summary>
	/// DOR predefined program ID's.
	/// </summary>
	public enum SystemPID
	{
		Unknown = 0,
		SystemAdmin = 1,
		WFTC = 2,		//< Working Family Tax Credit
		ExciseTax = 3,	//< ET
		ET = 3,			//< ET
		BRMS = 4,		//< BRMS
		BR = 4,			//< BRMS
		CI = 4,			//< BRMS
		Personnel = 5,	//< Personnel
		PR = 5,			//< Personnel
		AL = 6,			//< Leave
		SS = 7,			//< Security
		BI = 8,			//< IRS?
		CL = 9,			//< Command line
		CM = 10,		//< ECMS
		CR = 11,		//< CRMS
		OR = 12,		//< OSR
		TA = 13,		//< TARIS
		BLS = 14,		//< Business Lic,
		CTS = 15,	//< Core Tax System,
		ACS = 16		// ACS
	}

	public class DorSystem
	{
		public static string SystemName(SystemPID pid)
		{
			switch (pid)
			{
				case SystemPID.SystemAdmin:
					return "Admin";
				case SystemPID.WFTC:
					return "Working Family Tax Credit";
				case SystemPID.ET:
					return "Excise Tax";
				case SystemPID.BRMS:
					return "Business Registration";
				case SystemPID.PR:
					return "Personnel";
				case SystemPID.AL:
					return "Leave";
				case SystemPID.SS:
					return "Security";
				case SystemPID.BI:
					return "BI";
				case SystemPID.CL:
					return "Command Line";
				case SystemPID.CM:
					return "Electronic Case Management";
				case SystemPID.CR:
					return "Credits";
				case SystemPID.OR:
					return "Outstanding Returns";
				case SystemPID.TA:
					return "Receivables";
				case SystemPID.BLS:
					return "BLS";
				case SystemPID.ACS:
					return "Compliance";
				default:
					return "Unknown";
			}
		}

		public static string SystemCode(SystemPID pid)
		{
			switch(pid)
			{
				case SystemPID.SystemAdmin:
					return "ZZ";
				case SystemPID.WFTC:
					return "WFTC";
				case SystemPID.ET:			
					return "ET";
				case SystemPID.BRMS:
					return "BRMS";
				case SystemPID.PR:
					return "PR";
				case SystemPID.AL:
					return "AL";
				case SystemPID.SS:
					return "SS";
				case SystemPID.BI:
					return "BI";
				case SystemPID.CL:
					return "CL";
				case SystemPID.CM:
					return "ECMS";
				case SystemPID.CR:
					return "CRMS";
				case SystemPID.OR:
					return "OSR";
				case SystemPID.TA:
					return "TA";
				case SystemPID.BLS:
					return "BLS";
				case SystemPID.ACS:
					return "ACS";
				default:
					return "XX";
			}
		}
	}
}
