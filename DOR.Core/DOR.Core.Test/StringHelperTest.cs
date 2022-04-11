using DOR.Core;
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
using System.Text;
#endif

namespace DOR.Core.Test
{
    
    
    /// <summary>
    ///This is a test class for StringHelperTest and is intended
    ///to contain all StringHelperTest Unit Tests
    ///</summary>
	[TestClass()]
	public class StringHelperTest
	{


		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

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
		///A test for IsHexNum
		///</summary>
		[TestMethod()]
		public void IsHexNumTest()
		{
			string s = "0xE10a"; 
			bool expected = true; 
			bool actual;
			actual = StringHelper.IsHexNum(s);
			Assert.AreEqual(expected, actual);			
		}

		/// <summary>
		/// Test for Reverse
		/// </summary>
		[TestMethod()]
		public void ReverseTest()
		{
			string s = "43sd34";
			var expected = "43ds34";
			var actual = StringHelper.Reverse(s);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for IsHexNum
		///</summary>
		[TestMethod()]
		public void IsHexNumFormatHashTest()
		{
			string s = "#E10a";
			bool expected = true;
			bool actual;
			actual = StringHelper.IsHexNum(s);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for IsHexNum
		///</summary>
		[TestMethod()]
		public void IsHexNumNotANumberTest()
		{
			string s = "0xEW0a";
			bool expected = false;
			bool actual;
			actual = StringHelper.IsHexNum(s);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for CountOccurancesOf
		///</summary>
		[TestMethod()]
		public void CountOccurancesOfTest()
		{
			string str ="thssdasd sadas d aas 12 asd sadad"; 
			char ch = ' '; 
			int expected = 6; 
			int actual;
			actual = StringHelper.CountOccurancesOf(str, ch);
			Assert.AreEqual(expected, actual);			
		}

		/// <summary>
		///A test for CountOccurancesOf
		///</summary>
		[TestMethod()]
		public void CountOccurancesOfTest1()
		{
			string str = "thssdasd sads12 asd sadad";
			string key = "sad";
			int expected = 2;
			int actual;
			actual = StringHelper.CountOccurancesOf(str, key);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for IsUpper
		///</summary>
		[TestMethod()]
		public void IsUpperTest()
		{
			string str = "ASDW"; 
			bool expected = true; 
			bool actual;
			actual = StringHelper.IsUpper(str);
			Assert.AreEqual(expected, actual);			
		}

		/// <summary>
		///A test for IsNumeric
		///</summary>
		[TestMethod()]
		public void IsNumericTest()
		{
			string str = "1023456987";
			bool expected = true; 
			bool actual;
			actual = StringHelper.IsNumeric(str);
			Assert.AreEqual(expected, actual);		
		}

		/// <summary>
		///A test for StripQuotes
		///</summary>
		[TestMethod()]
		public void StripQuotesTest()
		{
			string str = "\"Department of Revenue\""; 
			string expected = "Department of Revenue";
			string actual;
			actual = StringHelper.StripQuotes(str);
			Assert.AreEqual(expected, actual);			
		}

		/// <summary>
		///A test for HasPath
		///</summary>
		[TestMethod()]
		public void HasPathTest()
		{
			string filename = "//dor/dev";
			bool expected = true; 
			bool actual;
			actual = StringHelper.HasPath(filename);
			Assert.AreEqual(expected, actual);			
		}

		/// <summary>
		///A test for EnsureTrailingChar
		///</summary>
		[TestMethod()]
		public void EnsureTrailingCharTest()
		{
			string str = "testT";
			char ch = 'T'; 
			string expected = "testT"; 
			string actual;
			actual = StringHelper.EnsureTrailingChar(str, ch);
			Assert.AreEqual(expected, actual);			
		}

		/// <summary>
		///A test for ParseRightInt32
		///</summary>
		[TestMethod()]
		public void ParseRightInt32Test()
		{
			string str = "321"; 
			int expected = 321; 
			int actual;
			actual = StringHelper.ParseRightInt32(str);
			Assert.AreEqual(expected, actual);			
		}

		/// <summary>
		///A test for RightStr
		///</summary>
		[TestMethod()]
		public void RightStrTest()
		{
			string str = "Department of Revenue"; 
			int count = 10; 
			string expected = "of Revenue"; 
			string actual;
			actual = StringHelper.RightStr(str, count);
			Assert.AreEqual(expected, actual);			
		}

		/// <summary>
		///A test for MidStr
		///</summary>
		[TestMethod()]
		public void MidStrTest()
		{
			string str = "Department of Revenue";
			int start = 11; 
			int stop = 15;
			string expected = "of R";
			string actual;
			actual = StringHelper.MidStr(str, start, stop);
			Assert.AreEqual(expected, actual);			
		}

		/// <summary>
		///A test for ParseMoney
		///</summary>
		[TestMethod()]
		public void ParseMoneyTest()
		{
			string str = "$10,000.05"; 
			Decimal expected = new Decimal(10000.05); 
			Decimal actual;
			actual = StringHelper.ParseMoney(str);
			Assert.AreEqual(expected, actual);			
		}

		/// <summary>
		///A test for RemoveNonNumerics
		///</summary>
		[TestMethod()]
		public void RemoveNonNumericsTest()
		{
			string str = "asd12fg34fr5a"; 
			string expected = "12345"; 
			string actual;
			actual = StringHelper.RemoveNonNumerics(str);
			Assert.AreEqual(expected, actual);			
		}

		/// <summary>
		///A test for RequiresXmlEncoding
		///</summary>
		[TestMethod()]
		public void RequiresXmlEncodingTest()
		{
			string str = "<\revenue>";
			bool expected = true; 
			bool actual;
			actual = StringHelper.RequiresXmlEncoding(str);
			Assert.AreEqual(expected, actual);			
		}

		/// <summary>
		///A test for XmlEncode
		///</summary>
		[TestMethod()]
		public void XmlEncodeTest()
		{			
			string str ="&_as_<_as_>_as_\"_as_"; 
			string expected = "&amp;_as_&lt;_as_&gt;_as_&quot;_as_";
			string actual;
			actual = StringHelper.XmlEncode(str);
			Assert.AreEqual(expected, actual);		
		}

		/// <summary>
		/// A test for EnsureQuotes
		/// </summary>
		[TestMethod()]
		public void EnsureQuotesTest()
		{
			string str = "\"lk2j34\"";
			string expected = "\"lk2j34\"";
			string actual;
			actual = StringHelper.EnsureQuotes(str);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// A test for EnsureQuotes
		/// </summary>
		[TestMethod()]
		public void EnsureQuotesTest1()
		{
			string str = "lk2j34";
			string expected = "\"lk2j34\"";
			string actual;
			actual = StringHelper.EnsureQuotes(str);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// A test for IsInt
		/// </summary>
		[TestMethod()]
		public void IsIntTest()
		{
			string str = "4312341234";
			Assert.IsTrue(StringHelper.IsInt(str));
		}

		/// <summary>
		/// A test for IsInt
		/// </summary>
		[TestMethod()]
		public void IsIntTest1()
		{
			string str = "43143fdasd2341234";
			Assert.IsFalse(StringHelper.IsInt(str));
		}

		/// <summary>
		/// A test for IsInt
		/// </summary>
		[TestMethod()]
		public void IsIntTest2()
		{
			string str = "";
			Assert.IsFalse(StringHelper.IsInt(str));
		}

		/// <summary>
		/// A test for ParseRightInt32
		/// </summary>
		[TestMethod()]
		public void ParseRightInt32Test1()
		{
			string str = "234";
			int expected = 234;
			int actual;
			actual = StringHelper.ParseRightInt32(str);
			Assert.AreEqual(expected, actual);

		}

		/// <summary>
		/// A test for ParseRightInt32
		/// </summary>
		[TestMethod()]
		public void ParseRightInt32Test2()
		{
			string str = "fdsa32";
			int expected = 32;
			int actual;
			actual = StringHelper.ParseRightInt32(str);
			Assert.AreEqual(expected, actual);

		}

		/// <summary>
		/// A test method for PadRight
		/// </summary>
		[TestMethod()]
		public void PadRight()
		{
			string s = "falk";
			string expected = "falkk";
			string actual;
			actual = StringHelper.PadRight(s, 5, 'k');
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// A test method for PadRight
		/// </summary>
		[TestMethod()]
		public void PadRight1()
		{
			string s = "falk";
			string expected = "falkk";
			string actual;
			actual = StringHelper.PadRight(s, 6, 'k');
			Assert.AreNotEqual(expected, actual);
		}

		/// <summary>
		/// A test method for PadLeft
		/// </summary>
		[TestMethod()]
		public void PadLeftTest()
		{			
			string s = "falk";
			string expected = "ffalk";
			string actual;
			actual = StringHelper.PadRight(s, 1, 'f');
			Assert.AreNotEqual(expected, actual);
		}

		/// <summary>
		/// A test method for RepeatChar
		/// </summary>
		[TestMethod()]
		public void RepeatCharTest()
		{
			char ch = 'c';
			string expected = "cccc";
			string actual = StringHelper.RepeatChar(ch, 4);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// A test method for TrimLeadingTest
		/// </summary>
		[TestMethod()]
		public void TrimLeadingTest()
		{
			string str = "ht432nnnn";
			string expected = "t432nnnn";
			string actual;
			actual = StringHelper.TrimLeading(str, 'h');
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// A test method for Trim
		/// </summary>
		[TestMethod()]
		public void TrimTest()
		{
			string str = "   trimmed";
			string expected = "trimmed";
			string actual;
			actual = StringHelper.Trim(str);
			Assert.AreEqual(expected, actual);
		}


		/// <summary>
		///A test for StripControlChars
		///</summary>
		[TestMethod()]
		public void StripControlCharsTest()
		{
			string s = "rkje\nls";
			string expected = "rkjels"; 
			string actual;
			actual = StringHelper.StripControlChars(s);
			Assert.AreEqual(expected, actual);
		}


	}
}
