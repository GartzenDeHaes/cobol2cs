using DOR.WorkingStorage.Pic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace DOR.WorkingStorage.Test
{
    
    
    /// <summary>
    ///This is a test class for PicFormatTest and is intended
    ///to contain all PicFormatTest Unit Tests
    ///</summary>
	[TestClass()]
	public class PicFormatTest
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
		///A test for Parse
		///</summary>
		[TestMethod()]
		public void ParseTest()
		{
			PicFormat pic = PicFormat.Parse("");
			Assert.AreEqual(0, pic.Length);
			Assert.AreEqual(0, pic.CharClassCount);

			pic = PicFormat.Parse("X");
			Assert.AreEqual(1, pic.Length);
			Assert.AreEqual(1, pic.CharClassCount);
			Assert.AreEqual("1", pic.Format("1"));
			Assert.AreEqual("A", pic.Format("A"));

			pic = PicFormat.Parse("99");
			Assert.AreEqual("01", pic.Format("01"));
			Assert.AreEqual("01", pic.Format(" 1"));

			pic = PicFormat.Parse("S999");
			Assert.AreEqual("+001", pic.Format("1"));

			pic = PicFormat.Parse("--");
			Assert.AreEqual(2, pic.Length);
			Assert.AreEqual(1, pic.CharClassCount);
			Assert.AreEqual(" 1", pic.Format("1"));
			Assert.AreEqual("  ", pic.Format("0"));
			Assert.AreEqual("-1", pic.Format("-1"));

			pic = PicFormat.Parse("--,--");
			Assert.AreEqual(4, pic.Length);
			Assert.AreEqual(1, pic.CharClassCount);
			Assert.AreEqual(" 7,02", pic.Format("702"));
			Assert.AreEqual("-7,02", pic.Format("-702"));
			Assert.AreEqual("-7,02", pic.Format("-702"));

			pic = PicFormat.Parse("-(2),--");
			Assert.AreEqual(4, pic.Length);
			Assert.AreEqual(1, pic.CharClassCount);
			Assert.AreEqual(" 7,02", pic.Format("702"));
			Assert.AreEqual(" 7,02", pic.Format("+702"));
			Assert.AreEqual("-7,02", pic.Format("-702"));
			Assert.AreEqual("-7,02", pic.Format("-702"));

			pic = PicFormat.Parse("9(9)");
			Assert.AreEqual(9, pic.Length);
			Assert.AreEqual(2, pic.CharClassCount);
			Assert.AreEqual("000001234", pic.Format("1234"));

			pic = PicFormat.Parse("9.9");
			Assert.AreEqual(3, pic.Length);
			Assert.AreEqual(4, pic.CharClassCount);
			Assert.AreEqual("1.0", pic.Format("1.0"));
			Assert.AreEqual("0.0", pic.Format("0.0"));

			pic = PicFormat.Parse("9V9");
			Assert.AreEqual(2, pic.Length);
			Assert.AreEqual(4, pic.CharClassCount);
			Assert.AreEqual("1.0", pic.Format("10"));
			Assert.AreEqual("0.0", pic.Format("00"));

			pic = PicFormat.Parse("S9(2)V9(2)");
			Assert.AreEqual(5, pic.Length);
			Assert.AreEqual(4, pic.CharClassCount);
			Assert.AreEqual("-01.00", pic.Format("-1.0"));
			Assert.AreEqual("+12.34", pic.Format("1234"));
			Assert.AreEqual("-12.34", pic.Format("-1234"));
			
			pic = PicFormat.Parse("ZZ,ZZZ,ZZZ,ZZZ.99-");
			Assert.AreEqual(15, pic.Length);
			Assert.AreEqual(4, pic.CharClassCount);
			string s = pic.Format("00000000023.40-");
			Assert.AreEqual("            23.40-", s);
			Assert.AreEqual("            23.40 ", pic.Format("00000000023.40+"));
		}

		[TestMethod()]
		public void toRawTest()
		{
			PicFormat pic = PicFormat.Parse("X");
			Assert.AreEqual("9", pic.ToRawString(9));
			Assert.AreEqual("9", pic.ToRawString((decimal)9));
			Assert.AreEqual("9", pic.ToRawString("9"));

			pic = PicFormat.Parse("XX");
			Assert.AreEqual("9 ", pic.ToRawString(9));
			Assert.AreEqual("99", pic.ToRawString(99));
			Assert.AreEqual("9 ", pic.ToRawString((decimal)9));
			Assert.AreEqual("9 ", pic.ToRawString("9"));

			pic = PicFormat.Parse("9(9)");
			Assert.AreEqual("000123456", pic.ToRawString(123456));

			pic = PicFormat.Parse("X");
			Assert.AreEqual("1", pic.ToRawString(1));
			Assert.AreEqual("A", pic.ToRawString("A"));

			pic = PicFormat.Parse("99");
			Assert.AreEqual("01", pic.ToRawString("1"));

			pic = PicFormat.Parse("--");
			Assert.AreEqual(" 1", pic.ToRawString(1));
			Assert.AreEqual(" 0", pic.ToRawString(0));
			Assert.AreEqual("-1", pic.ToRawString(-1));

			pic = PicFormat.Parse("--");
			Assert.AreEqual(" 1", pic.ToRawString(1));
			Assert.AreEqual(" 0", pic.ToRawString(0));
			Assert.AreEqual("-1", pic.ToRawString(-1));

			pic = PicFormat.Parse("--,--");
			Assert.AreEqual(" 702", pic.ToRawString(" 7,02"));
			Assert.AreEqual("-702", pic.ToRawString("-7,02"));

			pic = PicFormat.Parse("-(2),--");
			Assert.AreEqual(" 702", pic.ToRawString(" 7,02"));
			Assert.AreEqual("-702", pic.ToRawString("-7,02"));

			pic = PicFormat.Parse("9(9)");
			Assert.AreEqual("000001234", pic.ToRawString(1234));

			pic = PicFormat.Parse("9.9");
			Assert.AreEqual("1.0", pic.ToRawString(1.0m));
			Assert.AreEqual("0.0", pic.ToRawString(0.0m));

			pic = PicFormat.Parse("9V9");
			Assert.AreEqual("10", pic.ToRawString(1.0m));
			Assert.AreEqual("00", pic.ToRawString(0.0m));

			pic = PicFormat.Parse("S9(2)V9(2)");
			Assert.AreEqual("+1234", pic.ToRawString(12.34m));
			Assert.AreEqual("-1234", pic.ToRawString(-12.34m));
			Assert.AreEqual("+0230", pic.ToRawString(2.3m));
			Assert.AreEqual("+1230", pic.ToRawString(12.3m));

			pic = PicFormat.Parse("ZZ,ZZZ,ZZZ,ZZZ.99-");
			Assert.AreEqual("00000000023.41-", pic.ToRawString(-23.41m));
			Assert.AreEqual("00000000023.40-", pic.ToRawString(-23.4m));
			Assert.AreEqual("00000000023.40+", pic.ToRawString(23.40m));

			pic = PicFormat.Parse("S9(3)");
			Assert.AreEqual("+003", pic.ToRawString(" 03"));
		}

		[TestMethod()]
		public void MaskRegressionTest()
		{
			PicFormat pic = PicFormat.Parse("99/99/99");
			Assert.AreEqual(6, pic.Length);
			Assert.AreEqual(6, pic.CharClassCount);

			Assert.AreEqual("12/34/56", pic.Format("123456"));
			Assert.AreEqual("123456", pic.ToRawString("12/34/56"));

			Assert.AreEqual("00/00/00", pic.Format("000000"));
			Assert.AreEqual("000000", pic.ToRawString("00/00/00"));
		}

		[TestMethod()]
		public void PlusSignTest()
		{
			PicFormat pic = PicFormat.Parse("++++.9");
			Assert.AreEqual(6, pic.Length);
			Assert.AreEqual(3, pic.CharClassCount);
			Assert.AreEqual("+123.1", pic.Format(" 123.1"));
			Assert.AreEqual(" +23.0", pic.Format("  23.0"));
			Assert.AreEqual(" +23.0", pic.ToRawString("23.0"));

			pic = PicFormat.Parse("----.9");
			Assert.AreEqual(6, pic.Length);
			Assert.AreEqual(3, pic.CharClassCount);
			Assert.AreEqual(" 123.1", pic.Format(" 123.1"));
			Assert.AreEqual("  23.0", pic.Format("  23.0"));
			Assert.AreEqual("  23.0", pic.ToRawString("23"));
			Assert.AreEqual("-123.1", pic.Format("-123.1"));
			Assert.AreEqual(" -23.0", pic.Format("-23.0"));
			Assert.AreEqual(" -23.0", pic.ToRawString("-23"));
		}

		[TestMethod()]
		public void TrailingMinusRegressionTest()
		{
			PicFormat pic = PicFormat.Parse("---.-");
			Assert.AreEqual(5, pic.Length);
			Assert.AreEqual(3, pic.CharClassCount);
			Assert.AreEqual("     ", pic.Format("000.0"));
			Assert.AreEqual("     ", pic.Format("00000"));
		}

		[TestMethod()]
		public void LeadingMinusRegressionTest()
		{
			PicFormat pic = PicFormat.Parse("-");
			Assert.AreEqual(1, pic.Length);
			Assert.AreEqual(1, pic.CharClassCount);
			Assert.AreEqual("V", pic.Format("V"));
			Assert.AreEqual(" ", pic.Format(""));
			Assert.AreEqual("-", pic.Format("-"));
			Assert.AreEqual(" ", pic.Format("0"));
		}
	}
}
