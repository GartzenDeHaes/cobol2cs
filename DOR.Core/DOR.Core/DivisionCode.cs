using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Personnel.Model
{
	/// <summary>
	/// Operating division codes from tandem.
	/// </summary>
	public enum DivisionCode
	{
		Unknown = 0,
		Executive = 1,
		LegilationAndPolicy = 2,
		Research = 3,
		Appeals = 4,
		BusinessFinancial = 5,
		IS = 6,
		HumanResources = 7,
		Audit = 8,
		Compliance = 9,
		TAA = 10,
		ITA = 11,
		PropertyTax = 12,
		SpecialPrograms = 13,
		TaxpayerServices = 14
	}

	public class DorDivision
	{
		private DivisionCode m_code;

		public DorDivision(DivisionCode code)
		{
			m_code = code;
		}

		public DivisionCode Code
		{
			get { return m_code; }
		}

		public string ShortName
		{
			get { return DivisionShortName(m_code); }
		}

		public string Name
		{
			get { return DivisionName(m_code); }
		}

		public static DorDivision[] ListDivisions()
		{
			List<DorDivision> divs = new List<DorDivision>();

			divs.Add(new DorDivision(DivisionCode.Appeals));
			divs.Add(new DorDivision(DivisionCode.Audit));
			divs.Add(new DorDivision(DivisionCode.BusinessFinancial));
			divs.Add(new DorDivision(DivisionCode.Compliance));
			divs.Add(new DorDivision(DivisionCode.Executive));
			divs.Add(new DorDivision(DivisionCode.HumanResources));
			divs.Add(new DorDivision(DivisionCode.IS));
			divs.Add(new DorDivision(DivisionCode.ITA));
			divs.Add(new DorDivision(DivisionCode.LegilationAndPolicy));
			divs.Add(new DorDivision(DivisionCode.PropertyTax));
			divs.Add(new DorDivision(DivisionCode.Research));
			divs.Add(new DorDivision(DivisionCode.SpecialPrograms));
			divs.Add(new DorDivision(DivisionCode.TAA));
			divs.Add(new DorDivision(DivisionCode.TaxpayerServices));

			return divs.ToArray();
		}

		public static string DivisionShortName(DivisionCode code)
		{
			switch (code)
			{
				case DivisionCode.Executive:
					return "EXEC";
				case DivisionCode.LegilationAndPolicy:
					return "L&P";
				case DivisionCode.Research:
					return "RESRCH";
				case DivisionCode.Appeals:
					return "APPEALS";
				case DivisionCode.BusinessFinancial:
					return "B&FS";
				case DivisionCode.IS:
					return "IS";
				case DivisionCode.HumanResources:
					return "HR";
				case DivisionCode.Audit:
					return "AUDIT";
				case DivisionCode.Compliance:
					return "COMP";
				case DivisionCode.TAA:
					return "TAA";
				case DivisionCode.ITA:
					return "ITA";
				case DivisionCode.PropertyTax:
					return "PRTX";
				case DivisionCode.SpecialPrograms:
					return "SP PROG";
				case DivisionCode.TaxpayerServices:
					return "TS";
			}

			return "Unknown";
		}

		public static string DivisionName(DivisionCode code)
		{
			switch (code)
			{
				case DivisionCode.Executive:
					return "EXECUTIVE";
				case DivisionCode.LegilationAndPolicy:
					return "LEGISLATION & POLICY";
				case DivisionCode.Research:
					return "RESEARCH";
				case DivisionCode.Appeals:
					return "APPEALS";
				case DivisionCode.BusinessFinancial:
					return "BUSINESS & FINANCIAL SERVICES";
				case DivisionCode.IS:
					return "INFORMATION SERVICES";
				case DivisionCode.HumanResources:
					return "HUMAN RESOURCES";
				case DivisionCode.Audit:
					return "AUDIT";
				case DivisionCode.Compliance:
					return "COMPLIANCE";
				case DivisionCode.TAA:
					return "TAXPAYER ACCOUNT ADMINISTRATION";
				case DivisionCode.ITA:
					return "INTERPRETATIONS & TECHNICAL ADVICE";
				case DivisionCode.PropertyTax:
					return "PROPERTY TAX DIVISION";
				case DivisionCode.SpecialPrograms:
					return "SPECIAL PROGRAMS";
				case DivisionCode.TaxpayerServices:
					return "TAXPAYER SERVICES";
			}

			return "Unknown";
		}
	}
}
