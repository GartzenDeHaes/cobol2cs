
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

using System;
using DOR.Core.IO;
using DOR.Core;

namespace DOR.Core.Test
{
    
    
    /// <summary>
    ///This is a test class for FixedFieldDataTest and is intended
    ///to contain all FixedFieldDataTest Unit Tests
    ///</summary>
	[TestClass]
	public class FixedFieldDataTest
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
		///A test for Value set by constructor
		///</summary>
		[TestMethod]
		public void ValueTest()
		{
			FixedFieldDef field = new FixedFieldDef("testField", 10, 0, 0, ' ');
			var target = new FixedFieldData(field);
			Assert.AreEqual(target.Value, "".PadRight(10));

			const string TEST_INIT_VALUE = "testinit  ";
			field = new FixedFieldDef("testField", 10, 0, 0, TEST_INIT_VALUE);
			target = new FixedFieldData(field);
			Assert.AreEqual(target.Value, TEST_INIT_VALUE);
		}

		/// <summary>
		///A test for SetSpecial
		///</summary>
		[TestMethod]
		public void SetSpecialTest()
		{
			const string NUMERIC_CERT = "12341234  ";
			const string ALPHA_CERT = "  1234asdf";

			FixedFieldDef field = new FixedFieldDef("testField", 10, 0, 0, ' ');
			var target = new FixedFieldData(field);

			target.SetSpecial(NUMERIC_CERT);
			Assert.AreEqual("  12341234", target.Value, "SetSpecial() should right justify numeric values");

			target.SetSpecial(ALPHA_CERT);
			Assert.AreEqual("1234asdf  ", target.Value, "SetSpecial() should left justify alpha values");

			// SetSpecial with an empty string should return the default (i.e. '  1234asdf')
			target = new FixedFieldData(new FixedFieldDef("testField", 10, 0, 0, "  1234asdf"));
			target.SetSpecial(string.Empty);
			Assert.AreEqual("  1234asdf", target.Value, "SetSpecial() should left justify alpha values");
		}

		/// <summary>
		///A test for SetSignedIntLeft
		///</summary>
		[TestMethod]
		public void SetSignedIntLeftTest()
		{
			const int SIGNED_INT1 = +125;
			const int SIGNED_INT2 = -4234;

			FixedFieldDef field = new FixedFieldDef("testField", 10, 0, 0, ' ');
			var target = new FixedFieldData(field);

			target.SetSignedIntLeft(SIGNED_INT1);
			Assert.AreEqual("+      125",
				target.Value,
				"SetSignedIntLeft() should left justify int values & put + in first byte");

			target.SetSignedIntLeft(SIGNED_INT2);
			Assert.AreEqual("-     4234",
				target.Value,
				"SetSignedIntLeft() should left justify int values & put - in first byte");
		}

		/// <summary>
		///A test for SetMoney
		///</summary>
		[TestMethod]
		public void SetMoneyTest()
		{
			const decimal SIGNED_DECIMAL1 = +125.01m;
			const decimal SIGNED_DECIMAL2 = -4234.34m;
			const decimal SIGNED_DECIMAL3 = -4234.341234m;
			const decimal SIGNED_DECIMAL4 = -4234.349234m;

			FixedFieldDef field = new FixedFieldDef("testField", 10, 0, 0, ' ');
			var target = new FixedFieldData(field);
			// this function (apparently) removes the decimal (i.e. mult by 100) 
			// and then treats in like a signed int. 
			target.SetMoney(SIGNED_DECIMAL1);
			Assert.AreEqual("+    12501", target.Value);

			target.SetMoney(SIGNED_DECIMAL2);
			Assert.AreEqual("-   423434", target.Value);

			target.SetMoney(SIGNED_DECIMAL3);
			Assert.AreEqual("-   423434", target.Value);

			// rounding occurs
			target.SetMoney(SIGNED_DECIMAL4);
			Assert.AreEqual("-   423435", target.Value);
		}

		/// <summary>
		///A test for SetLeft
		///</summary>
		[TestMethod]
		public void SetLeftTest()
		{
			const string TEST_STRING_TO_LJUST = "1234a";

			FixedFieldDef field = new FixedFieldDef("testField", 10, 0, 0, ' ');
			var target = new FixedFieldData(field);
			target.SetLeft(TEST_STRING_TO_LJUST);
			Assert.AreEqual("1234a     ", target.Value);
		}

		/// <summary>
		///A test for SetRight
		///</summary>
		[TestMethod]
		public void SetRightTest()
		{
			const string TEST_STRING_TO_RJUST = "1234a";

			FixedFieldDef field = new FixedFieldDef("testField", 10, 0, 0, ' ');
			var target = new FixedFieldData(field);
			target.SetRight(TEST_STRING_TO_RJUST);
			Assert.AreEqual("     1234a", target.Value);
		}

		/// <summary>
		///A test for SetIntLeft
		///</summary>
		[TestMethod]
		public void SetIntLeftTest()
		{
			const int TEST_INT_TO_LJUST = 125;
			const int TEST_INT_TO_LJUST2 = -44125;

			FixedFieldDef field = new FixedFieldDef("testField", 10, 0, 0, ' ');
			var target = new FixedFieldData(field);
			// this function (apparently) removes the decimal (i.e. mult by 100) 
			// and then treats in like a signed int. 
			target.SetIntLeft(TEST_INT_TO_LJUST);
			Assert.AreEqual("       125", target.Value);
			target.SetIntLeft(TEST_INT_TO_LJUST2);
			Assert.AreEqual("    -44125", target.Value);
		}

		/// <summary>
		///A test for SetDecimal
		///</summary>
		[TestMethod]
		public void SetDecimalTest()
		{
			const decimal TEST_DECIMAL = 125m;
			const decimal TEST_DECIMAL2 = -44125m;

			FixedFieldDef field = new FixedFieldDef("testField", 10, 0, 0, ' ');
			var target = new FixedFieldData(field);
			// this function (apparently) removes the decimal (i.e. mult by 100) 
			// and then treats in like a signed int. 
			target.SetDecimal(TEST_DECIMAL);
			Assert.AreEqual("       125", target.Value);
			target.SetDecimal(TEST_DECIMAL2);
			Assert.AreEqual("    -44125", target.Value);
		}

		/// <summary>
		///A test for SetDate
		///</summary>
		[TestMethod]
		public void SetDate()
		{
			Date TEST_DATE = Date.Parse("12/09/2007");
			FixedFieldDef field = new FixedFieldDef("testField", 6, 0, 0, ' ');
			var target = new FixedFieldData(field);
			target.SetDate(TEST_DATE);
			Assert.AreEqual("120907", target.Value);
		}

		/// <summary>
		///A test for SetDate
		///</summary>
		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void SetDateThrowsIfFieldIsWrongLength()
		{
			Date TEST_DATE = Date.Parse("12/09/2007");
			FixedFieldDef field = new FixedFieldDef("testField", 9, 0, 0, ' ');
			var target = new FixedFieldData(field);
			target.SetDate(TEST_DATE);
		}

		/// <summary>
		///A test for SetDateTime
		///</summary>
		[TestMethod]
		public void SetDateTimeTestWithDateValue()
		{
			DateTime TEST_DATETIME = DateTime.Parse("12/09/2007 01:01:01");
			FixedFieldDef field = new FixedFieldDef("testField", 26, 0, 0, ' ');
			var target = new FixedFieldData(field);
			// this function (apparently) removes the decimal (i.e. mult by 100) 
			// and then treats in like a signed int. 
			target.SetDateTime(TEST_DATETIME);
			Assert.AreEqual("2007-12-09:01:01:01.000000", target.Value);
		}

		/// <summary>
		///A test for SetDateTime
		///</summary>
		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void SetDateTimeTestFieldWrongSizeThrows1()
		{
			Date TEST_DATE = Date.Parse("12/09/2007");

			FixedFieldDef field = new FixedFieldDef("testField", 25, 0, 0, ' ');
			var target = new FixedFieldData(field);
			target.SetDateTime(TEST_DATE);
		}

		/// <summary>
		///A test for SetDateTime
		///</summary>
		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void SetDateTimeTestFieldWrongSizeThrows()
		{
			DateTime TEST_DATETIME = DateTime.Parse("12/09/2007 01:01:01");

			FixedFieldDef field = new FixedFieldDef("testField", 25, 0, 0, ' ');
			var target = new FixedFieldData(field);
			target.SetDateTime(TEST_DATETIME);
		}

		/// <summary>
		///A test for SetDate
		///</summary>
		[TestMethod]
		public void SetDateTestFromDorDate()
		{

			Date TEST_DATE  = new Date(DateTime.Parse("12/09/2007 01:01:01"));

			FixedFieldDef field = new FixedFieldDef("testField", 26, 0, 0, ' ');
			var target = new FixedFieldData(field);
			// this function (apparently) removes the decimal (i.e. mult by 100) 
			// and then treats in like a signed int. 
			target.SetDateTime(TEST_DATE);
			Assert.AreEqual("2007-12-09:00:00:00.000000", target.Value);
		}

		/// <summary>
		///A test for Set
		///</summary>
		[TestMethod]
		public void SetTest()
		{
			FixedFieldDef field = new FixedFieldDef("testField", 10, 0, 0, ' ');
			var target = new FixedFieldData(field);
			const string val = "xyzzy@1234";
			target.Set(val);
			Assert.AreEqual(val, target.Value);
		}

		/// <summary>
		///A test for Set
		///</summary>
		[TestMethod]
		[ExpectedException(typeof(ApplicationException))]
		public void SetTestDataMustBeExactFieldLength()
		{
			FixedFieldDef field = new FixedFieldDef("testField", 10, 0, 0, ' ');
			var target = new FixedFieldData(field);
			const string val = "xyzzy@1234xxx";
			try
			{
				target.Set(val);
			}
			catch (Exception)
			{
				throw new ApplicationException("xx");
			}
		}

		///// <summary>
		/////A test for Right (i.e. returns n characters from the right of a string
		/////</summary>
		//[TestMethod]
		//public void RightTest()
		//{
		//    const string STR = "1234asdf5678jkl;";
		//    const int LEN = 8;
		//    string expected = STR.Substring(STR.Length - LEN);
		//    string actual = FixedFieldData_Accessor.Right(STR, LEN);
		//    Assert.AreEqual(expected, actual);

		//    // test string < than requested len (should return string)
		//    Assert.AreEqual("abc", FixedFieldData_Accessor.Right("abc", 30));

		//}

		/// <summary>
		///A test for Reset (sets the value of a field back to its default)
		///</summary>
		[TestMethod]
		public void ResetTest()
		{
			const string DEFAULT_VALUE = "1234567890";
			FixedFieldDef field = new FixedFieldDef("testField", 10, 0, 0, DEFAULT_VALUE);
			var target = new FixedFieldData(field);
			const string NEW_VALUE = "0987654321";
			target.Set(NEW_VALUE);
			Assert.AreEqual(NEW_VALUE, target.Value);

			// reset should set the value back to the default of DEFAULT_VALUE
			target.Reset();
			Assert.AreEqual(DEFAULT_VALUE, target.Value);

			field = new FixedFieldDef("testField", 10, 0, 0, ' ');
			target = new FixedFieldData(field);
			target.Set(NEW_VALUE);
			Assert.AreEqual(NEW_VALUE, target.Value);

			// reset should set the value back to the default (all spaces)
			target.Reset();
			Assert.AreEqual("".PadRight(10), target.Value);
		}

		/// <summary>
		///A test for ParseYear 
		/// ParseYear tacks on the century to a 2-byte numeric field 
		/// using a fixed window (1951 to 2050)
		///</summary>
		[TestMethod]
		public void ParseYearTest()
		{
			FixedFieldDef field = new FixedFieldDef("testField", 2, 0, 0, "02");
			var target = new FixedFieldData(field);
			int actual = target.ParseYear();
			Assert.AreEqual(2002, actual);

			target.Set("50");
			actual = target.ParseYear();
			Assert.AreEqual(2050, actual);

			target.Set("51");
			actual = target.ParseYear();
			Assert.AreEqual(1951, actual);

			// four byte year should return the correct year
			target = new FixedFieldData(new FixedFieldDef("testField", 4, 0, 0, "2008"));
			Assert.AreEqual(2008, target.ParseYear());
		}

		/// <summary>
		///A test for ParseYear 
		/// Should blow up if the data is not numeric
		///</summary>
		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void ParseYearTestDataMustBeNumeric()
		{
			FixedFieldDef field = new FixedFieldDef("testField", 2, 0, 0, "xx");
			var target = new FixedFieldData(field);
			target.ParseYear();
		}

		/// <summary>
		///A test for ParseYear 
		/// Should blow up if the data is not numeric
		///</summary>
		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void ParseYearTestDataMustBeCorrectLength()
		{
			FixedFieldDef field = new FixedFieldDef("testField", 3, 0, 0, "003");
			var target = new FixedFieldData(field);
			int actual = target.ParseYear();
			Assert.AreEqual(2003, actual);
		}

		/// <summary>
		///A test for ParseMoney 
		/// (takes a signed integer literal, divides by 100 and returns a decimal val) 
		///</summary>
		[TestMethod]
		public void ParseMoneyTest()
		{
			FixedFieldDef field = new FixedFieldDef("testField", 8, 0, 0, "+0012345");
			var target = new FixedFieldData(field);
			const decimal EXPECTED = 123.45m;
			decimal actual = target.ParseMoney();
			Assert.AreEqual(EXPECTED, actual);

			// try a negative money amount
			target.Set("-0098765");
			Assert.AreEqual(-987.65m, target.ParseMoney());

		}

		/// <summary>
		///A test for ParseMoney 
		/// the value must be signed 
		///</summary>
		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void ParseMoneyTestMustBeSigned()
		{
			FixedFieldDef field = new FixedFieldDef("testField", 8, 0, 0, "00012345");
			var target = new FixedFieldData(field);
			target.ParseMoney();
		}

		/// <summary>
		///A test for ParseMoney 
		/// the value must be parsably numeric 
		///</summary>
		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void ParseMoneyTestMustBeValidDecimal()
		{
			FixedFieldDef field = new FixedFieldDef("testField", 8, 0, 0, "+x012345");
			var target = new FixedFieldData(field);
			target.ParseMoney();
		}

		/// <summary>
		///A test for ParseInt
		///</summary>
		[TestMethod]
		public void ParseIntTest()
		{
			FixedFieldDef field = new FixedFieldDef("testField", 8, 0, 0, "00012345");
			var target = new FixedFieldData(field);
			var expected = 12345;
			int actual = target.ParseInt();
			Assert.AreEqual(expected, actual);

			// zeros should return zero
			target.Set("00000000");
			Assert.AreEqual(0, target.ParseInt());

			// blanks should return zero
			target.Set("        ");
			Assert.AreEqual(0, target.ParseInt());
		}

		/// <summary>
		///A test for ParseInt
		///</summary>
		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void ParseIntTestMustBeNumbersOnly()
		{
			FixedFieldDef field = new FixedFieldDef("testField", 8, 0, 0, "00012.45");
			var target = new FixedFieldData(field);
			target.ParseInt();
		}

		/// <summary>
		///A test for ParseInt
		///</summary>
		[TestMethod]
		public void ParseIntTestDataCanBeSigned()
		{
			FixedFieldDef field = new FixedFieldDef("testField", 8, 0, 0, "-0001245");
			var target = new FixedFieldData(field);
			var expected = -1245;
			int actual = target.ParseInt();
			Assert.AreEqual(expected, actual);
		}


		/// <summary>
		///A test for ParseDecimal
		///</summary>
		[TestMethod]
		public void ParseDecimalTest()
		{
			FixedFieldDef field = new FixedFieldDef("testField", 8, 0, 0, "00123.45");
			var target = new FixedFieldData(field);
			var expected = 123.45m;
			decimal actual = target.ParseDecimal();
			Assert.AreEqual(expected, actual);

			target.Set("+1234.12");
			actual = target.ParseDecimal();
			Assert.AreEqual(1234.12m, actual);
		}

		/// <summary>
		///A test for ParseDecimal (must be numeric)
		///</summary>
		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void ParseIntTestMustBeNumberic()
		{
			FixedFieldDef field = new FixedFieldDef("testField", 10, 0, 0, "00x0012.45");
			var target = new FixedFieldData(field);
			target.ParseDecimal();
		}

		/// <summary>
		///A test for ParseDateTime (input s/b "YYYY-MM-DD HH:NN:SS.000000")
		///</summary>
		[TestMethod]
		public void ParseDateTimeTest()
		{
			FixedFieldDef field = new FixedFieldDef("testField", 26, 0, 0, "2007-05-01 12:31:05.000000");
			var target = new FixedFieldData(field);
			var expected = DateTime.Parse("2007-05-01 12:31:05.000000");
			DateTime actual = target.ParseDateTime();
			Assert.AreEqual(expected, actual);

			// slashes s/b okay
			target.Set("2007/05/01 12:31:05.000000");
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for ParseDateTime (correct format)
		///</summary>
		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void ParseDateTimeTestMustBeValidData()
		{
			FixedFieldDef field = new FixedFieldDef("testField", 26, 0, 0, "2x07-05-01 12:31:05.000000");
			var target = new FixedFieldData(field);
			target.ParseDateTime();
		}

		/// <summary>
		///A test for ParseDateTime (correct format)
		///</summary>
		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void ParseDateTimeTestMustBeValidData2()
		{
			FixedFieldDef field = new FixedFieldDef("testField", 26, 0, 0, "2007-05-01 12x31:05.000000");
			var target = new FixedFieldData(field);
			target.ParseDateTime();
		}

		/// <summary>
		///A test for ParseDate (Data s/b MMDDYY)
		///</summary>
		[TestMethod]
		public void ParseDateTest()
		{
			FixedFieldDef field = new FixedFieldDef("testField", 6, 0, 0, "060105");
			var target = new FixedFieldData(field);
			var expected = Date.Parse("06/01/2005");
			Date actual = target.ParseDate();
			Assert.AreEqual(expected, actual);

			target.Set("010150");
			expected = Date.Parse("01/01/2050");
			actual = target.ParseDate();
			Assert.AreEqual(expected, actual);

			target.Set("010151");
			expected = Date.Parse("01/01/1951");
			actual = target.ParseDate();
			Assert.AreEqual(expected, actual);
		}

		///// <summary>
		/////A test for PadSubstring
		/////</summary>
		//[TestMethod]
		//public void PadSubstringTest()
		//{
		//    Assert.AreEqual("a         ", FixedFieldData_Accessor.PadSubstring("a", 0, 10));
		//    Assert.AreEqual("          ", FixedFieldData_Accessor.PadSubstring(string.Empty, 0, 10));
		//    Assert.AreEqual("abc", FixedFieldData_Accessor.PadSubstring("abcdefghi", 0, 3));
		//}

		/// <summary>
		///A test for IsNumeric
		///</summary>
		//[TestMethod]
		//public void IsNumericTest()
		//{
		//    int i = 0;
		//    Assert.IsFalse(FixedFieldData_Accessor.IsNumeric("xxxx", ref i));
		//    Assert.AreEqual(0, i);
		//    Assert.IsTrue(FixedFieldData_Accessor.IsNumeric("+99999", ref i));
		//    Assert.AreEqual(99999, i);
		//}

		/// <summary>
		///A test for FixedFieldData Constructor - 
		/// initializing from the field default char
		///</summary>
		[TestMethod]
		public void FixedFieldDataConstructorTest2()
		{
			FixedFieldDef field = new FixedFieldDef("testField", 6, 0, 0, 'x');
			var target = new FixedFieldData(field);
			Assert.AreEqual("xxxxxx", target.Value);
		}

		/// <summary>
		///A test for FixedFieldData Constructor - 
		/// initializing from the field default string
		///</summary>
		[TestMethod]
		public void FixedFieldDataConstructorTest1()
		{
			FixedFieldDef field = new FixedFieldDef("testField", 6, 0, 0, "060105");
			var target = new FixedFieldData(field);
			Assert.AreEqual("060105", target.Value);
		}

		/// <summary>
		///A test for FixedFieldData Constructor
		///</summary>
		[TestMethod]
		public void FixedFieldDataConstructorTest()
		{
			FixedFieldDef field = new FixedFieldDef("testField", 6, 10, 0, ' ');
			var record = "1234567890abcdef";
			var target = new FixedFieldData(field, record);
			Assert.AreEqual("abcdef", target.Value);
		}
	}
}
