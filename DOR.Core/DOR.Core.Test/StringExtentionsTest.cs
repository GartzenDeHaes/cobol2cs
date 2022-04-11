using DOR.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DOR.Core.Test
{
    
    
    /// <summary>
    ///This is a test class for StringExtentionsTest and is intended
    ///to contain all StringExtentionsTest Unit Tests
    ///</summary>
	[TestClass()]
	public class StringExtentionsTest
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
		///A test for Crc8
		///</summary>
		[TestMethod()]
		public void Crc8Test()
		{
			string str = "42kljn";
			byte expected = 18; 
			byte actual;
			actual = StringExtentions.Crc8(str);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for Crc8
		///</summary>
		[TestMethod()]
		public void Crc8Test1()
		{
			string str = "431hj";
			byte expected = 61;
			byte actual;
			actual = StringExtentions.Crc8(str);
			Assert.AreNotEqual(expected, actual);
		}

		[TestMethod()]
		public void Crc8Test2()
		{
			string str = "4hj";
			byte expected = 61;
			byte actual;
			actual = StringExtentions.Crc8(str);
			Assert.AreNotEqual(expected, actual);
		}

		/// <summary>
		///A test for Split
		///</summary>
		[TestMethod()]
		public void SplitTest()
		{
			string str = "hgokjh";
			string delim = "hg";
			string[] expected = new string[]{"", "okjh"};
			string[] actual;
			actual = StringExtentions.Split(str, delim);

			for (int y = 0; y < actual.Length; y++)
			{	
				Assert.AreEqual(expected[y].ToString(), actual[y].ToString());
			}

		}

		/// <summary>
		///A test for Split
		///</summary>
		[TestMethod()]
		public void SplitTest1()
		{
			string str = "147852";
			string delim = "478";
			string[] expected = new string[] { "1", "52" };
			string[] actual;
			actual = StringExtentions.Split(str, delim);

			for (int y = 0; y < actual.Length; y++)
			{
				Assert.AreEqual(expected[y].ToString(), actual[y].ToString());
			}

		}
	}
}
