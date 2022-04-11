using System;
#if INTEGRATION
using ExpectedException = NUnit.Framework.ExpectedExceptionAttribute;
using Assert = NUnit.Framework.Assert;
using TestMethod = NUnit.Framework.TestAttribute;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestContext = System.Object;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

using DOR.Core;

namespace DOR.Core.Test
{
	/// <summary>
	///This is a test class for DateTest and is intended
	///to contain all DateTest Unit Tests.
	///</summary>
	[TestClass]
	public class DateTest
	{
		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext { get; set; }

		#region Additional test attributes

		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//

		#endregion

		/// <summary>
		///A test for IsDate
		///</summary>
		[TestMethod]
		public void IsDateValidInStringFormatTest()
		{
			bool actual = Date.IsDate("Jan 4,2008");
			Assert.AreEqual(true, actual);
		}

		/// <summary>
		///A test for IsDate
		///</summary>
		[TestMethod]
		public void IsDateValidInIntegerFormatTest()
		{
			var year = 2008;
			var mo = 1;
			var day = 1;
			bool actual = Date.IsDate(year, mo, day);
			Assert.AreEqual(true, actual);
		}

		/// <summary>
		///A test for Date Constructor
		///</summary>
		[TestMethod]
		public void DateConstructorTest()
		{
			DateTime dt = DateTime.Now;
			var target = new Date(dt);
			Assert.AreEqual(DateTime.Now.Year, target.Year);
			Assert.AreEqual(DateTime.Now.Month, target.Month);
			Assert.AreEqual(DateTime.Now.Day, target.Day);

			var date = new Date(DBNull.Value);
			Assert.AreEqual(DateTime.MinValue.Year, date.Year);
			Assert.AreEqual(DateTime.MinValue.Month, date.Month);
			Assert.AreEqual(DateTime.MinValue.Day, date.Day);
			date = new Date(target);
			Assert.AreEqual(DateTime.Now.Year, date.Year);
			Assert.AreEqual(DateTime.Now.Month, date.Month);
			Assert.AreEqual(DateTime.Now.Day, date.Day);
			date = new Date(null);
			Assert.AreEqual(DateTime.MinValue.Year, date.Year);
			Assert.AreEqual(DateTime.MinValue.Month, date.Month);
			Assert.AreEqual(DateTime.MinValue.Day, date.Day);
			date = new Date((object)dt);
			Assert.AreEqual(DateTime.Now.Year, date.Year);
			Assert.AreEqual(DateTime.Now.Month, date.Month);
			Assert.AreEqual(DateTime.Now.Day, date.Day);
		}

		/// <summary>
		///A test for Date Constructor for invalid month
		///</summary>		
		[TestMethod]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void InvalidMonthConstructorTest()
		{
			var year = 2008;
			var month = -12;
			var day = 5;
			new Date(year, month, day);
		}

		/// <summary>
		///A test for Date Constructor for invalid day
		///</summary>		
		[TestMethod]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void InvalidDayConstructorTest()
		{
			var year = 2008;
			var month = 12;
			var day = 35;
			new Date(year, month, day);
		}

		/// <summary>
		///A test for Date Constructor
		///</summary>
		[TestMethod]
		public void DateConstructorRevFormatTest()
		{
			var revdate = 20000506;
			var actual = new Date(revdate);
			var expected = new Date(2000, 05, 06);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for ToDateTime
		///</summary>
		[TestMethod]
		public void ToDateTimeTest()
		{
			var target = new Date(2009, 1, 1);
			DateTime actual = target.ToDateTime();
			Assert.AreEqual(2009, actual.Year);
			Assert.AreEqual(1, actual.Month);
			Assert.AreEqual(1, actual.Day);
		}

		/// <summary>
		///A test for ToRevInt
		///</summary>
		[TestMethod]
		public void ToRevIntTest()
		{
			var target = new Date(2009, 1, 1);
			var expected = 20090101;
			int actual = target.ToRevInt();
			Assert.AreEqual(expected, actual);
		}


		/// <summary>
		///A test for AsRevInt
		///</summary>
		[TestMethod]
		public void AsRevIntTest()
		{
			var target = new Date(2008, 12, 12);
			var expected = 20081212;
			int actual = target.AsRevInt;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for ToString
		///</summary>
		[TestMethod]
		public void ToStringTest()
		{
			var target = new Date(2008, 5, 1);
			var expected = "05/01/2008";
			string actual = target.ToString();
			Assert.AreEqual(expected, actual);
		}


		/// <summary>
		///A test for Parse
		///</summary>
		[TestMethod]
		public void ParseInMonthAsNameFormatTest()
		{
			var dt = "Jan 05,2008";
			var expected = new Date(2008, 1, 5);
			Date actual = Date.Parse(dt);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for Parse
		///</summary>
		[TestMethod]
		public void ParseInDateSeparatedBySlashFormatTest()
		{
			var dt = "1/5/2008";
			var expected = new Date(2008, 1, 5);
			Date actual = Date.Parse(dt);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for op_LessThanOrEqual
		///</summary>
		[TestMethod]
		public void op_LessThanOrEqualTest()
		{
			var d1 = new Date(2008, 12, 05);
			var d2 = new Date(2007, 12, 07);
			bool actual = (d1 <= d2);
			Assert.AreEqual(false, actual);
		}


		/// <summary>
		///A test for op_LessThan
		///</summary>
		[TestMethod]
		public void op_LessThanTest()
		{
			var d1 = new Date(2008, 05, 05);
			var d2 = new Date(2008, 06, 06);
			bool actual = (d1 < d2);
			Assert.AreEqual(true, actual);
		}


		/// <summary>
		///A test for op_Inequality
		///</summary>
		[TestMethod]
		public void op_InequalityTest()
		{
			Date d1 = Date.Now;
			DateTime d2 = DateTime.Now;
			bool actual = (d1 != d2);
			Assert.AreEqual(false, actual);
		}
		

		/// <summary>
		///A test for op_GreaterThanOrEqual
		///</summary>
		[TestMethod]
		public void op_GreaterThanOrEqualTest()
		{
			var d1 = new Date(2008, 12, 5);
			var d2 = new Date(2008, 12, 5);
			bool actual = (d1 >= d2);
			Assert.AreEqual(true, actual);
		}


		/// <summary>
		///A test for op_GreaterThan
		///</summary>
		[TestMethod]
		public void op_GreaterThanTest()
		{
			var d1 = new Date(2000, 5, 6);
			var d2 = new Date();
			bool actual = (d1 > d2);
			Assert.AreEqual(false, actual);
		}

		/// <summary>
		///A test for op_Equality
		///</summary>
		[TestMethod]
		public void op_EqualityTest()
		{
			var d1 = new Date();
			var d2 = new Date();
			bool actual = (d1 == d2);
			Assert.AreEqual(true, actual);
		}

		/// <summary>
		///A test for Format
		///</summary>
		[TestMethod]
		public void FormatTest()
		{
			var target = new Date(2008, 5, 6);
			var frmt = "MMDDYY";
			var expected = "050608";
			string actual = target.Format(frmt);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for Format for exception
		///</summary>
		[TestMethod]
		[ExpectedException(typeof (ArgumentException))]
		public void FormatForExceptionTest()
		{
			var target = new Date(2008, 5, 6);
			var frmt = "MMsDYY";
			target.Format(frmt);
		}

		/// <summary>
		///A test for Equals
		///</summary>
		[TestMethod]
		public void EqualsTest()
		{
			var target = new Date();
			bool actual = target.Equals(null);
			Assert.AreEqual(false, actual);
		}


		/// <summary>
		///A test for op_Inequality
		///</summary>
		[TestMethod]
		public void op_InequalityWithDateTimeTest()
		{
			var d1 = new Date();
			DateTime d2 = DateTime.Now;
			bool actual = (d1 != d2);
			Assert.AreEqual(false, actual);
		}

		/// <summary>
		///A test for op_GreaterThanOrEqual
		///</summary>
		[TestMethod]
		public void op_GreaterThanOrEqualWithDateTimeTest()
		{
			var d1 = new Date(2007, 12, 18);
			var d2 = new DateTime(2007, 12, 4);
			bool actual = (d1 >= d2);
			Assert.AreEqual(true, actual);
		}

		/// <summary>
		///A test for op_LessThanOrEqual
		///</summary>
		[TestMethod]
		public void op_LessThanOrEqualWithDateTimeTest()
		{
			var d1 = new Date(2007, 12, 18);
			var d2 = new DateTime(2007, 12, 4);
			bool actual = (d1 <= d2);
			Assert.AreEqual(false, actual);
		}

		/// <summary>
		///A test for op_GreaterThan
		///</summary>
		[TestMethod]
		public void op_GreaterThanTestWithDateTime()
		{
			var d1 = new Date(2007, 12, 18);
			var d2 = new DateTime(2007, 12, 4);
			bool actual = (d1 > d2);
			Assert.AreEqual(true, actual);
		}

		/// <summary>
		/// go through all of the holidays for a given year
		/// in 2008 there are 251 business days.  Should work the same
		/// forward or backward.
		/// </summary>
		[TestMethod]
		public void AddBusinessDaysToDateTime()
		{
			Date startDate = new Date(2007, 12, 31);
			Date newDate = startDate.AddBusinessDays(251);
			Assert.AreEqual(2008, newDate.Year);
			Assert.AreEqual(12, newDate.Month);
			Assert.AreEqual(31, newDate.Day);

			startDate = new Date(2008, 12, 31);
			newDate = startDate.AddBusinessDays(-251);
			Assert.AreEqual(2007, newDate.Year);
			Assert.AreEqual(12, newDate.Month);
			Assert.AreEqual(31, newDate.Day);
		}

		[TestMethod]
		public void HolidayTest()
		{
			Assert.AreEqual(true, new Date(2002, 1, 1).IsHoliday);
			Assert.AreEqual(true, (new Date(2002, 2, 18).IsHoliday));
			Assert.AreEqual(true, (new Date(2002, 5, 27).IsHoliday));
			Assert.AreEqual(true, (new Date(2002, 7, 4).IsHoliday));
			Assert.AreEqual(true, (new Date(2002, 9, 2).IsHoliday));
			Assert.AreEqual(true, (new Date(2002, 11, 11).IsHoliday));
			Assert.AreEqual(true, (new Date(2002, 11, 28).IsHoliday));
			Assert.AreEqual(true, (new Date(2002, 11, 29).IsHoliday));
			Assert.AreEqual(true, (new Date(2002, 12, 25).IsHoliday));

			Assert.AreEqual(true, (new Date(2003, 1, 1).IsHoliday));
			Assert.AreEqual(true, (new Date(2003, 2, 17).IsHoliday));
			Assert.AreEqual(true, (new Date(2003, 5, 26).IsHoliday));
			Assert.AreEqual(true, (new Date(2003, 7, 4).IsHoliday));
			Assert.AreEqual(true, (new Date(2003, 9, 1).IsHoliday));
			Assert.AreEqual(true, (new Date(2003, 11, 11).IsHoliday));
			Assert.AreEqual(true, (new Date(2003, 11, 27).IsHoliday));
			Assert.AreEqual(true, (new Date(2003, 11, 28).IsHoliday));
			Assert.AreEqual(true, (new Date(2003, 12, 25).IsHoliday));

			Assert.AreEqual(true, (new Date(2004, 1, 1).IsHoliday));
			Assert.AreEqual(true, (new Date(2004, 2, 16).IsHoliday));
			Assert.AreEqual(true, (new Date(2004, 5, 31).IsHoliday));
			Assert.AreEqual(true, (new Date(2004, 7, 5).IsHoliday));
			Assert.AreEqual(true, (new Date(2004, 9, 6).IsHoliday));
			Assert.AreEqual(true, (new Date(2004, 11, 11).IsHoliday));
			Assert.AreEqual(true, (new Date(2004, 11, 25).IsHoliday));
			Assert.AreEqual(true, (new Date(2004, 11, 26).IsHoliday));
			Assert.AreEqual(true, (new Date(2004, 12, 24).IsHoliday));

			Assert.AreEqual(true, (new Date(2004, 12, 31).IsHoliday));
			Assert.AreEqual(true, (new Date(2005, 2, 21).IsHoliday));
			Assert.AreEqual(true, (new Date(2005, 5, 30).IsHoliday));
			Assert.AreEqual(true, (new Date(2005, 7, 4).IsHoliday));
			Assert.AreEqual(true, (new Date(2005, 9, 5).IsHoliday));
			Assert.AreEqual(true, (new Date(2005, 11, 11).IsHoliday));
			Assert.AreEqual(true, (new Date(2005, 11, 24).IsHoliday));
			Assert.AreEqual(true, (new Date(2005, 11, 25).IsHoliday));
			Assert.AreEqual(true, (new Date(2005, 12, 26).IsHoliday));

			Assert.AreEqual(true, (new Date(2006, 1, 2).IsHoliday));
		}

		/// <summary>
		/// Date instance version of the AddBusinessDays function
		/// </summary>
		[TestMethod]
		public void AddBusinessDaysToDate()
		{
			Date startDate = new Date(2008, 1, 1);
			Date newDate = startDate.AddBusinessDays(251);
			Assert.AreEqual(2008, newDate.Year);
			Assert.AreEqual(12, newDate.Month);
			Assert.AreEqual(31, newDate.Day);

			startDate = new Date(2008, 12, 31);
			newDate = startDate.AddBusinessDays(-251);
			Assert.AreEqual(2007, newDate.Year);
			Assert.AreEqual(12, newDate.Month);
			Assert.AreEqual(31, newDate.Day);

		}

		[TestMethod]
		public void AddMonths()
		{
			Date dt = new Date(2008, 1, 31);
			Date ret = dt.AddMonths(1);
			Assert.AreEqual(2008, ret.Year);
			Assert.AreEqual(2, ret.Month);
			Assert.AreEqual(29, ret.Day);

			ret = dt.AddMonths(-1);
			Assert.AreEqual(2007, ret.Year);
			Assert.AreEqual(12, ret.Month);
			Assert.AreEqual(31, ret.Day);

			ret = dt.AddMonths(13);
			Assert.AreEqual(2009, ret.Year);
			Assert.AreEqual(2, ret.Month);
			Assert.AreEqual(28, ret.Day);
		}

		[TestMethod]
		public void DayOfWeekTest()
		{
			Date dt = new Date(1982, 4, 24);
			Assert.AreEqual(DayOfWeek.Saturday, dt.DayOfWeek);

			dt = new Date(1783, 9, 18);
			Assert.AreEqual(DayOfWeek.Thursday, dt.DayOfWeek);

			dt = new Date(2054, 6, 19);
			Assert.AreEqual(DayOfWeek.Friday, dt.DayOfWeek);

			dt = new Date(2002, 2, 18);
			Assert.AreEqual(DayOfWeek.Monday, dt.DayOfWeek);
		}

		[TestMethod]
		public void TandemDateParse()
		{
			Date dt = new Date(1982, 4, 24);
			Assert.AreEqual(dt, Date.Parse("19820424"));

			Assert.IsNull(Date.Parse("0"));
		}

		[TestMethod]
		public void EurapeanDateParse()
		{
			Date dt = new Date(1965, 1, 26);
			Assert.AreEqual(dt, Date.Parse("1965-01-26"));
		}
	}
}