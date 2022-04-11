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
#endif

namespace DOR.Core.Test
{
    
    
    /// <summary>
    ///This is a test class for CanadianPostalCodeTest and is intended
    ///to contain all CanadianPostalCodeTest Unit Tests
    ///</summary>
	[TestClass()]
	public class CanadianPostalCodeTest
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
		///A test for IsCaPostalCode
		///</summary>
		[TestMethod()]
		public void IsCaPostalCodeTest()
		{
			string code = " K1A 0B1 ";
			bool expected = true; 
			bool actual;
			actual = CanadianPostalCode.IsCaPostalCode(code);
			Assert.AreEqual(expected, actual);			
		}

		/// <summary>
		///A test for IsCaPostalCode
		///</summary>
		[TestMethod()]
		public void IsCaPostalCodeInvalidTest()
		{
			string code = " K1A 0_1 "; 
			bool expected = false; 
			bool actual;
			actual = CanadianPostalCode.IsCaPostalCode(code);
			Assert.AreEqual(expected, actual);			
		}

		/// <summary>
		///A test for IsCaPostalCode
		///</summary>
		[TestMethod()]
		public void IsCaPostalCodeTest1()
		{
			string code = " K1A 05N ";
			bool expected = false;
			bool actual;
			actual = CanadianPostalCode.IsCaPostalCode(code);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for IsCaPostalCode
		///</summary>
		[TestMethod()]
		public void IsCaPostalCodeInvalidTest1()
		{
			string code = " K1A 12_ ";
			bool actual;
			bool expected = false; 
			actual = CanadianPostalCode.IsCaPostalCode(code);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for Parse
		///</summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException))]				
		public void ParseTestForInvalidFormat1()
		{
			string code = " K1A _78 "; 
			CanadianPostalCode.Parse(code);			
		}

		//// This check was removed to accomadate BRMS junk data.
		//[TestMethod()]
		//[ExpectedException(typeof(ArgumentException))]
		//public void ParseTestForInvalidLength()
		//{
		//    string code = " K1A 0B1A ";
		//    CanadianPostalCode.Parse(code);
		//}
	}
}
