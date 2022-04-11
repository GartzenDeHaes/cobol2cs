using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

using DOR.Core;
using DOR.Core.Config;
using DOR.Core.Data;
using DOR.Core.Data.Tandem;
using DOR.Core.Logging;

namespace DOR.WorkingStorage
{
	public abstract class CobolBase : IDisposable
	{
		protected Thread _myThread;

		protected ILogger Logger
		{
			get;
			set;
		}

		protected IBufferOffset InputRecord
		{
			get;
			set;
		}

		#region Environment

		public string SqlEnvironmentCode
		{
			get
			{
				switch (Configuration.AppConfig.DorEnvironment.EnvironmentType)
				{
					case EnvironmentType.Prod:
						return "P";
					case EnvironmentType.Test:
						return "T";
					case EnvironmentType.Demo:
						return "D";
					default:
						throw new DOR.Core.ConfigurationException("Unknown environment");
				}
			}
		}

		public string SqlEnvironmentName
		{
			get
			{
				switch (Configuration.AppConfig.DorEnvironment.EnvironmentType)
				{
					case EnvironmentType.Prod:
						return "PROD";
					case EnvironmentType.Test:
						return "TEST";
					case EnvironmentType.Demo:
						return "DEMO";
					default:
						return "UNKN";
				}
			}
		}

		#endregion

		public CobolBase()
		{
		}

		public virtual IBufferOffset Main(IBufferOffset data, ILogger logger, ISqlDataAccess connection)
		{
			InputRecord = data;
			Logger = logger;
			// connection is not used here

			return null;
		}

		#region Unstring

		// use length of field
		public void Unstring
		(
			IBufferOffset src,
			IBufferOffset field,
			ref int pos
		)
		{
			if (pos < 0 || pos > src.Length)
			{
				pos = -1;
				return;
			}
			field.Set(src.ToString().Substring(pos, field.Length));
			pos += field.Length;
		}

		public void Unstring
		(
			IBufferOffset src, 
			string delim, 
			IBufferOffset field,
			IBufferOffset delimDest,
			ref int pos
		)
		{
			if (pos < 0)
			{
				field.Initialize();
				if (null != (object)delimDest)
				{
					delimDest.Initialize();
				}
				return;
			}
			string ssrc = src.ToString();
			int newPos = ssrc.IndexOf(delim, pos);

			if (newPos < 0)
			{
				if (null != (object)delimDest)
				{
					delimDest.Initialize();
				}
				field.Set(ssrc.Substring(pos));
				pos = -1;
				return;
			}

			if (null != (object)delimDest)
			{
				delimDest.Set(delim);
			}

			field.Set(ssrc.Substring(pos, newPos));
			pos = newPos + delim.Length;
		}

		#endregion

		#region Range

		public object CreateRange(int low, int hi)
		{
			return new RangeEvaluator(low, hi);
		}

		public object CreateRange(int low, IBufferOffset hi)
		{
			return new RangeEvaluator(low, hi.ToInt());
		}

		public object CreateRange(IBufferOffset low, IBufferOffset hi)
		{
			return new RangeEvaluator(low.ToInt(), hi.ToInt());
		}

		public object CreateRange(long low, long hi)
		{
			return new RangeEvaluator(low, hi);
		}

		#endregion

		#region Math

		public long Mod(IBufferOffset val, long mod)
		{
			return val.ToInt() % mod;
		}

		public long Mod(long val, long mod)
		{
			return val % mod;
		}

		#endregion

		#region TAL

		public void SQLCA_TOBUFFER2_
		(
			WsRecord sqlca, 
			WsRecord buf, 
			int maxlen, 
			string omitted, 
			IBufferOffset linesRetured, 
			string omitted2, 
			int maxLines
		)
		{
			// this is just a placebo
		}

		public void SQLCA_TOBUFFER2_
		(
			WsRecord sqlca,
			WsRecord buf,
			int maxlen,
			string omitted,
			int linesRetured,
			string omitted2,
			int maxLines
		)
		{
			// this is just a placebo
		}

		public void SQLCA_DISPLAY2_(WsRecord sqlca, IBufferOffset outFileNum)
		{
			Console.WriteLine("WARNING: CobolHelper.SQLCA_DISPLAY2_() not implemented");
		}

		public void SQLCADISPLAY(WsRecord sqlca, IBufferOffset outFileNum)
		{
			Console.WriteLine("WARNING: CobolHelper.SQLCADISPLAY() not implemented");
		}

		public void SQLCADISPLAY(WsRecord sqlca, long outFileNum)
		{
			Console.WriteLine("WARNING: CobolHelper.SQLCADISPLAY() not implemented");
		}

		public string GETSTARTUPTEXT(IBufferOffset messagePortion, IBufferOffset outfileExternalName)
		{
			string fn = messagePortion.ToString().Trim();
			if (fn == "OUT")
			{
				return "1";
			}
			return "";
		}

		public string GETSTARTUPTEXT(string messagePortion, string outfileExternalName)
		{
			string fn = messagePortion.ToString().Trim();
			if (fn == "OUT")
			{
				return "1";
			}
			return "";
		}

		public void GETPARAMTEXT(IBufferOffset paramName, IBufferOffset value)
		{
			Console.WriteLine("WARNING: CobolHelper.GETPARAMTEXT(" + paramName.ToString() + ") not implemented");
		}

		// * Convert the external name to a internal name
		public void FNAMEEXPAND(IBufferOffset externalFileNmae, IBufferOffset internalName1, IBufferOffset internalName2)
		{
			Console.WriteLine("WARNING: CobolHelper.FNAMEEXPAND() not implemented");
		}

		// * Convert the external name to a internal name
		public void FNAMEEXPAND(string externalFileNmae, string internalName1, string internalName2)
		{
			Console.WriteLine("WARNING: CobolHelper.FNAMEEXPAND() not implemented");
		}

		public void OPEN(IBufferOffset fileName, IBufferOffset fileNumberOut)
		{
			Console.WriteLine("WARNING: OPEN " + fileName.ToString() + " not implemented");
		}

		public void OPEN(IBufferOffset fileName, long fileNumberOut)
		{
			Console.WriteLine("WARNING: OPEN " + fileName.ToString() + " not implemented");
		}

		public void OPEN(string fileName, long fileNumberOut)
		{
			Console.WriteLine("WARNING: OPEN " + fileName + " not implemented");
		}

		public void FILEINFO(IBufferOffset fileNumber, IBufferOffset errorReplyNum)
		{
			Console.WriteLine("WARNING: CobolHelper.FILEINFO() not implemented");
		}

		public void FILEINFO(long fileNumber, long errorReplyNum)
		{
			Console.WriteLine("WARNING: CobolHelper.FILEINFO() not implemented");
		}

		public void TIME(MemoryBuffer buf, int start)
		{
			DateTime dtm = DateTime.Now;
			buf.Set(dtm.Year, start, 5, true);
			buf.Set(dtm.Month, start + 5, 5, true);
			buf.Set(dtm.Day, start + 5 * 2, 5, true);
			buf.Set(dtm.Hour, start + 5 * 3, 5, true);
			buf.Set(dtm.Minute, start + 5 * 4, 5, true);
			buf.Set(dtm.Second, start + 5 * 5, 5, true);
			buf.Set(dtm.Millisecond, start + 5 * 6, 5, true);
		}

		public long SERVERCLASS_SEND_
		(
			IBufferOffset pathmon,
			IBufferOffset pathmonLenth,
			IBufferOffset serverName,
			IBufferOffset serverNameLength,
			IBufferOffset pathsendBuffer,
			IBufferOffset pathsendBufferLength,
			IBufferOffset maxServerReplySize,
			IBufferOffset serverReplyLength,
			IBufferOffset timeout
		)
		{
			throw new NotImplementedException("SERVERCLASS_SEND_");
		}

		public long SERVERCLASS_SEND_INFO_(IBufferOffset errorCode1, IBufferOffset errorCode2)
		{
			throw new NotImplementedException("SERVERCLASS_SEND_");
		}

		private static DateTime _inception = new DateTime(1, 1, 1);

		/// <summary>
		/// coverte date to 4 BYTE numeric (it must be valid for the output to be modified
		/// by adding days).
		/// </summary>
		public long COMPUTEJULIANDAYNO(IBufferOffset year, IBufferOffset month, IBufferOffset day)
		{
			return COMPUTEJULIANDAYNO(year.ToInt(), month.ToInt(), day.ToInt());
		}

		/// <summary>
		/// coverte date to 4 BYTE numeric (it must be valid for the output to be modified
		/// by adding days).
		/// </summary>
		public long COMPUTEJULIANDAYNO(long year, long month, long day)
		{
			//DateTime dtm = new DateTime((int)year.ToInt(), (int)month.ToInt(), (int)day.ToInt());
			//// the number of microseconds between noon 1/1/4713 BC and 00:00 12/31/1974
			//long mils = 211024440000000000L;
			//DateTime nsf = new DateTime(1974, 12, 31);
			//return (((long)(dtm - nsf).TotalMilliseconds) + mils);

			DateTime dtm = new DateTime((int)year, (int)month, (int)day);

			return (dtm - _inception).Days;
		}

		/// <summary>
		/// coverte 4 BYTE numeric to a date
		/// </summary>
		public void INTERPRETJULIANDAYNO(IBufferOffset julian, IBufferOffset outYear, IBufferOffset outMonth, IBufferOffset outDay)
		{
			long j = julian.ToInt();
			INTERPRETJULIANDAYNO(j, outYear, outMonth, outDay);
		}

		public void INTERPRETJULIANDAYNO(long julian, IBufferOffset outYear, IBufferOffset outMonth, IBufferOffset outDay)
		{
			DateTime dtm = _inception.Add(TimeSpan.FromDays(julian));

			outYear.Set(dtm.Year);
			outMonth.Set(dtm.Month);
			outDay.Set(dtm.Day);

			//long mils = 211024440000000000L;
			//DateTime nsf = new DateTime(1974, 12, 31);
			//DateTime j = nsf.AddMilliseconds(julian - mils);
			//outYear.Set(j.Year);
			//outMonth.Set(j.Month);
			//outDay.Set(j.Day);
		}

		public void ABORTTRANSACTION()
		{
			Console.WriteLine("WARNING: ABORTTRANSACTION not implemented");
		}

		#endregion

		#region SQL

		/// <summary>
		/// INTO :SQL-CONVERT-YTD TYPE AS DATETIME YEAR TO DAY
		/// </summary>
		public string DatetimeYearToDay(string sdtm)
		{
			DateTime dtm = DateTime.Parse(sdtm);
			return dtm.Year.ToString() + "-" +
				dtm.Month.ToString("00") + "-" +
				dtm.Day.ToString("00");
		}

		public string DatetimeYearToMonth(string sdtm)
		{
			return sdtm;
		}

		public string DatetimeFraction6(string dtm)
		{
			return dtm;
		}

		public string DatetimeDay(string sdtm)
		{
			Console.WriteLine("WARNING: CobolHelper.DatetimeDay() may not be correct");
			return sdtm;
		}

		public string DatetimeMonth(string sdtm)
		{
			Console.WriteLine("WARNING: CobolHelper.DatetimeMonth() may not be correct");

			return sdtm;
		}

		public string DatetimeYear(string sdtm)
		{
			Console.WriteLine("WARNING: CobolHelper.DatetimeYear() may not be correct");

			return sdtm;
		}

		public string Timestamp(string d)
		{
			Debug.Assert(StringHelper.CountOccurancesOf(d, ' ') == 1);
			return d.Replace(" ", ":");
		}

		public string DatetimeYearToFraction6(string sdtm)
		{
			Debug.Assert(StringHelper.CountOccurancesOf(sdtm, ' ') == 1);
			return sdtm.Replace(" ", ":");
		}

		public string GET_JULIANTIMESTAMP()
		{
			Console.WriteLine("WARNING: CobolHelper.ET_JULIANTIMESTAMP() may not be correct");

			// the number of microseconds between noon 1/1/4713 BC and 00:00 12/31/1974
			long mils = 211024440000000000L;
			DateTime nsf = new DateTime(1974, 12, 31);
			return (((long)(DateTime.Now - nsf).TotalMilliseconds) + mils).ToString();
		}

		public string IntervalDay18(string days)
		{
			// This function cannot every be called, becuase java does not
			// support coverting interval to any java type.  So, these queries
			// will error out on tandem.
			return days;
		}

		#endregion

		public void Dispose()
		{
		}
	}
}
