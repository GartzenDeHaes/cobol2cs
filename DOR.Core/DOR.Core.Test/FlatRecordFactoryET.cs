using System;
using DOR.Core.IO;
using System.Diagnostics.CodeAnalysis;

namespace DOR.Core.Test
{
	/// <summary>
	/// Summary description for FlatRecordDefs.
	/// </summary>
	[ExcludeFromCodeCoverage]
	public class FlatRecordFactoryET
	{
		/// <summary>
		/// No instances
		/// </summary>
		private FlatRecordFactoryET()
		{
		}

		/// <summary>
		/// Header record
		/// </summary>
		public static FixedRecordDef HRecord
		{
			get
			{
				if (m_hrec == null)
				{
					m_hrec = new FixedRecordDef(200);
					m_hrec.AddField("RecordType", 2, "H ");
					m_hrec.AddField("DateReceived", 6, '0');
					m_hrec.AddField("BatchNum", 4, '0');
					m_hrec.AddField("Filler1", 7, ' ');
					m_hrec.AddField("BatchRollNum", 6, '9');
					m_hrec.AddField("Filler2", 2, ' ');
					m_hrec.AddField("PenaltyWaiverInd", 1, '0');
					m_hrec.AddField("Filler3", 10, ' ');
					m_hrec.AddField("BatchAmount", 14, '0');
					m_hrec.AddField("Filler4", 1, ' ');
					m_hrec.AddField("LockboxReceivedDate", 6, '0');
					m_hrec.AddField("LockboxBatchNum", 4, '0');
					m_hrec.AddField("Filler5", 11, ' ');
					m_hrec.AddField("DataType", 1, ' ');
					m_hrec.AddField("WillNotBeOnBcsFileInd", 1, '2');
					m_hrec.AddField("Filler6", 124, ' ');
				}
				return m_hrec;
			}
		}

		/// <summary>
		/// Money record
		/// </summary>
		public static FixedRecordDef MRecord
		{
			get
			{
				if (m_mrec == null)
				{
					m_mrec = new FixedRecordDef(200);
					m_mrec.AddField("RecordType", 2, "M ");
					m_mrec.AddField("DateReceived", 6, '0');
					m_mrec.AddField("BatchNum", 4, '0');
					m_mrec.AddField("BatchSerialNum", 6, '0');
					m_mrec.AddField("Filler1", 1, ' ');
					m_mrec.AddField("TRA", 9, '0');
					m_mrec.AddField("Period", 2, '0');
					m_mrec.AddField("Year", 2, '0');
					m_mrec.AddField("PenaltyRate", 2, '0');
					m_mrec.AddField("Filler2", 4, ' ');
					m_mrec.AddField("AmountPaid", 14, '0');
					m_mrec.AddField("Filler3", 3, ' ');
					m_mrec.AddField("PenaltyAmount", 12, '0');
					m_mrec.AddField("TransactionCode", 3, ' ');
					m_mrec.AddField("Filler4", 2, ' ');
					m_mrec.AddField("ReturnType", 2, ' ');		// added TAXTYPE_ID [Excise, Use, or Jenkins Tax]
					m_mrec.AddField("DataType", 1, ' ');
					m_mrec.AddField("Filler5", 1, ' ');
					m_mrec.AddField("InvoiceCounter", 3, '0');
					m_mrec.AddField("DocumentNum", 9, ' ');
					m_mrec.AddField("PaymentDate", 6, '0');	// MMDDYY
					m_mrec.AddField("Filler6", 1, ' ');
					m_mrec.AddField("TTASAuditNum", 7, ' ');
					m_mrec.AddField("Filler7", 1, ' ');
					m_mrec.AddField("PartialAuditNum", 4, '0');
					m_mrec.AddField("Filler8", 18, ' ');
					m_mrec.AddField("NoFishTaxDue", 1, '0');
					m_mrec.AddField("Filler9", 1, ' ');
					m_mrec.AddField("BNGTaxDueInd", 1, '0');
					m_mrec.AddField("Filler10", 1, ' ');
					m_mrec.AddField("SSTInterestAmount", 14, '0');
					m_mrec.AddField("Filler11", 1, ' ');
					m_mrec.AddField("SSTDiscountAmount", 14, '0');
					m_mrec.AddField("Filler12", 42, ' ');
				}
				return m_mrec;
			}
		}

		/// <summary>
		/// ELF record
		/// </summary>
		public static FixedRecordDef NRecord
		{
			get
			{
				if (m_nrec == null)
				{
					m_nrec = new FixedRecordDef(200);
					m_nrec.AddField("RecordType", 2, "N ");
					m_nrec.AddField("DateReceived", 6, '0');
					m_nrec.AddField("BatchNum", 4, '0');
					m_nrec.AddField("BatchSerialNum", 6, '0');
					m_nrec.AddField("Filler1", 1, ' ');
					m_nrec.AddField("TRA", 9, '0');
					m_nrec.AddField("Period", 2, '0');
					m_nrec.AddField("Year", 2, '0');
					m_nrec.AddField("Filler2", 1, ' ');
					m_nrec.AddField("FillingDate", 26, '0'); //YYYY-MM-DD:HH:NN:SS.000000
					m_nrec.AddField("Filler3", 1, ' ');
					m_nrec.AddField("ConfirmNum", 22, '0');
					m_nrec.AddField("Filler4", 1, ' ');
					m_nrec.AddField("WarehouseDate", 26, '0'); //YYYY-MM-DD:HH:NN:SS.000000
					m_nrec.AddField("Filler5", 1, ' ');
					m_nrec.AddField("PaymentType", 3, ' ');
					m_nrec.AddField("Filler6", 87, ' ');
				}
				return m_nrec;
			}
		}

		/// <summary>
		/// Tax record
		/// </summary>
		public static FixedRecordDef TRecord
		{
			get
			{
				if (m_trec == null)
				{
					m_trec = new FixedRecordDef(200);
					m_trec.AddField("RecordType", 2, "T ");
					m_trec.AddField("DateReceived", 6, '0');
					m_trec.AddField("BatchNum", 4, '0');
					m_trec.AddField("BatchSerialNum", 6, '0');
					m_trec.AddField("Filler1", 1, ' ');
					m_trec.AddField("TRA", 9, '0');
					m_trec.AddField("Period", 2, '0');
					m_trec.AddField("Year", 2, '0');
					m_trec.AddField("LineCode", 4, '0');
					m_trec.AddField("SplitNum", 2, '0');
					m_trec.AddField("GrossAmount", 14, '0');
					m_trec.AddField("Filler2", 1, ' ');
					m_trec.AddField("TaxDue", 14, '0');
					m_trec.AddField("TransactionCode", 3, ' ');
					m_trec.AddField("LocationCode", 4, '0');		// not used
					m_trec.AddField("DataType", 1, ' ');			// not used
					m_trec.AddField("Filler3", 125, ' ');
				}
				return m_trec;
			}
		}

		/// <summary>
		/// Credit record
		/// </summary>
		public static FixedRecordDef PRecord
		{
			get
			{
				if (m_prec == null)
				{
					m_prec = new FixedRecordDef(200);
					m_prec.AddField("RecordType", 2, "P ");
					m_prec.AddField("DateReceived", 6, '0');
					m_prec.AddField("BatchNum", 4, '0');
					m_prec.AddField("BatchSerialNum", 6, '0');
					m_prec.AddField("Filler1", 1, ' ');
					m_prec.AddField("TRA", 9, '0');
					m_prec.AddField("Period", 2, '0');
					m_prec.AddField("Year", 2, '0');
					m_prec.AddField("LineCode", 4, '0');
					m_prec.AddField("Filler2", 2, ' ');
					m_prec.AddField("CreditAmount", 14, '0');
					m_prec.AddField("Filler3", 6, ' ');
					m_prec.AddField("CertId", 9, '0');
					m_prec.AddField("TransactionCode", 3, ' ');
					m_prec.AddField("CreditTypeCode", 4, '0');
					m_prec.AddField("Filler4", 7, ' ');
					m_prec.AddField("DocumentNum", 7, '0');
					m_prec.AddField("PaymentDate", 6, '0'); // MMDDYY
					m_prec.AddField("Filler5", 106, ' ');
				}
				return m_prec;
			}
		}

		/// <summary>
		/// Local tax
		/// </summary>
		public static FixedRecordDef XRecord
		{
			get
			{
				if (m_xrec == null)
				{
					m_xrec = new FixedRecordDef(200);
					m_xrec.AddField("RecordType", 2, "X ");
					m_xrec.AddField("DateReceived", 6, '0');
					m_xrec.AddField("BatchNum", 4, '0');
					m_xrec.AddField("BatchSerialNum", 6, '0');
					m_xrec.AddField("Filler1", 1, ' ');
					m_xrec.AddField("TRA", 9, '0');
					m_xrec.AddField("Period", 2, '0');
					m_xrec.AddField("Year", 2, '0');
					m_xrec.AddField("LineCode", 4, '0');
					m_xrec.AddField("SplitNum", 2, '0');
					m_xrec.AddField("TaxableAmount", 14, '0');
					m_xrec.AddField("Filler2", 1, ' ');
					m_xrec.AddField("TaxDue", 14, '0');
					m_xrec.AddField("TransactionCode", 3, ' ');
					m_xrec.AddField("County", 2, '0');
					m_xrec.AddField("Loc", 2, '0');
					m_xrec.AddField("DataType", 1, ' ');			// not used
					m_xrec.AddField("Filler3", 1, ' ');
					m_xrec.AddField("HotelDays", 6, '0'); //updated from 4 to 6 by ryan dolby 06-19-2007
					m_xrec.AddField("Filler4", 118, ' ');
				}
				return m_xrec;
			}
		}

		/// <summary>
		/// Deduction
		/// </summary>
		public static FixedRecordDef RRecord
		{
			get
			{
				if (m_rrec == null)
				{
					m_rrec = new FixedRecordDef(200);
					m_rrec.AddField("RecordType", 2, "R ");
					m_rrec.AddField("DateReceived", 6, '0');
					m_rrec.AddField("BatchNum", 4, '0');
					m_rrec.AddField("BatchSerialNum", 6, '0');
					m_rrec.AddField("Filler1", 1, ' ');
					m_rrec.AddField("TRA", 9, '0');
					m_rrec.AddField("Period", 2, '0');
					m_rrec.AddField("Year", 2, '0');
					m_rrec.AddField("LineCode", 4, '0');
					m_rrec.AddField("SplitNum", 2, '0');
					m_rrec.AddField("DedCode", 3, '0');
					m_rrec.AddField("Filler2", 12, ' ');
					m_rrec.AddField("DedAmount", 14, '0');
					m_rrec.AddField("TransactionCode", 3, ' ');
					m_rrec.AddField("Filler3", 4, ' ');
					m_rrec.AddField("DataType", 1, ' ');
					m_rrec.AddField("Filler4", 125, ' ');
				}
				return m_rrec;
			}
		}

		/// <summary>
		/// Deduction continuation
		/// </summary>
		public static FixedRecordDef R1Record
		{
			get
			{
				if (m_r1rec == null)
				{
					m_r1rec = new FixedRecordDef(200);
					m_r1rec.AddField("RecordType", 2, "R1");
					m_r1rec.AddField("DateReceived", 6, '0');
					m_r1rec.AddField("BatchNum", 4, '0');
					m_r1rec.AddField("BatchSerialNum", 6, '0');
					m_r1rec.AddField("Filler1", 1, ' ');
					m_r1rec.AddField("TRA", 9, '0');
					m_r1rec.AddField("Period", 2, '0');
					m_r1rec.AddField("Year", 2, '0');
					m_r1rec.AddField("LineCode", 4, '0');
					m_r1rec.AddField("SplitNum", 2, '0');
					m_r1rec.AddField("DedCode", 3, '0');
					m_r1rec.AddField("Filler2", 1, ' ');
					m_r1rec.AddField("Reason", 78, ' ');
					m_r1rec.AddField("Filler3", 80, ' ');
				}
				return m_r1rec;
			}
		}

		private static FixedRecordDef m_hrec = null;
		private static FixedRecordDef m_mrec = null;
		private static FixedRecordDef m_nrec = null;
		private static FixedRecordDef m_trec = null;
		private static FixedRecordDef m_prec = null;
		private static FixedRecordDef m_xrec = null;
		private static FixedRecordDef m_rrec = null;
		private static FixedRecordDef m_r1rec = null;
	}
}
