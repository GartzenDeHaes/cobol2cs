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
    ///This is a test class for ZIPTest and is intended
    ///to contain all ZIPTest Unit Tests
    ///</summary>
	[TestClass()]
	public class ZIPTest
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
		///A test for ZIP Constructor when extension is invalid
		///</summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException))]					
		public void ZIPConstructorForInvalidZipExtensionTest()
		{
			int zip = 98504; 
			int plus4 = 91861; 
			ZIP target = new ZIP(zip, plus4);			
		}

		/// <summary>
		///A test for ZIP Constructor when zip code is invalid
		///</summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException))]
		public void ZIPConstructorForInvalidZipTest()
		{
			int zip = 985045;
			int plus4 = 9186;
			ZIP target = new ZIP(zip, plus4);
		}

		/// <summary>
		///A test for Parse for correct Zip code with spaces
		///</summary>
		[TestMethod()]
		public void ParseTest()
		{
			string zip5 = " 98501"; 
			string zipPlus4 = " 8569 "; 
			ZIP expected = new ZIP(98501,8569); 
			ZIP actual;
			actual = ZIP.Parse(zip5, zipPlus4);
			string expectedToStr = expected.ToString();
			string actualToStr = actual.ToString();
			Assert.AreEqual(expectedToStr, actualToStr);			
		}

		/// <summary>
		///A test for Parse for invalid length
		///</summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException))]
		public void ParseForInvalidZipLengthTest()
		{
			string zip5 = " 985011";
			string zipPlus4 = " 8569 ";
			ZIP.Parse(zip5, zipPlus4);
		}


		/// <summary>
		///A test for Parse for non numeric characters
		///</summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException))]
		public void ParseForInvalidZipNonNumericTest()
		{
			string zip5 = " 9850A";
			string zipPlus4 = " 8569 ";
			ZIP.Parse(zip5, zipPlus4);			
		}



		/// <summary>
		///A test for Parse when correct string format is provided
		///</summary>
		[TestMethod()]
		public void ParseForSingleStringAsInputTest()
		{
			string zip = "98501-0561";
			ZIP expected = new ZIP(98501, 0561);
			ZIP actual;
			actual = ZIP.Parse(zip);
			string expectedToStr = expected.ToString();
			string actualToStr = actual.ToString();
			Assert.AreEqual(expectedToStr, actualToStr);			
		
		}

		/// <summary>
		///A test for Parse when correct string format is provided
		///</summary>
		[TestMethod()]
		public void ParseForWithNoExtensionAsInputTest()
		{
			string zip = "98501";
			ZIP expected = new ZIP(98501);
			ZIP actual;
			actual = ZIP.Parse(zip);
			string expectedToStr = expected.ToString();
			string actualToStr = actual.ToString();
			Assert.AreEqual(expectedToStr, actualToStr);
		}

		/// <summary>
		///A test for Parse for non numeric characters
		///</summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException))]
		public void ParseForZipNonNumericTest()
		{
			string zip = " 9850A";			
			ZIP.Parse(zip);
		}

		/// <summary>
		///A test for IsZip when full valid zip code is provided 
		///</summary>
		[TestMethod()]
		public void IsZipTest()
		{
			string zip = "98501";
			string plus4 = "8569"; 
			bool expected = true; 
			bool actual;
			actual = ZIP.IsZip(zip, plus4);
			Assert.AreEqual(expected, actual);			
		}


		/// <summary>
		///A test for IsZip when invalid zip code is provided 
		///</summary>
		[TestMethod()]		
		public void IsZipForNonNumericTest()
		{
			string zip = "9850A";
			string plus4 = "8569";
			bool expected = false;
			bool actual;
			actual = ZIP.IsZip(zip, plus4);
			Assert.AreEqual(expected, actual);	
			Assert.AreEqual(expected,ZIP.IsZip("98501","856A")); 
		}

		/// <summary>
		///A test for IsZip for valid zip code
		///</summary>
		[TestMethod()]
		public void IsZipForSingleInputTest()
		{
			string zip = "98501-8569"; 
			bool expected = true; 
			bool actual;
			actual = ZIP.IsZip(zip);
			Assert.AreEqual(expected, actual);
			Assert.AreEqual(expected, ZIP.IsZip("98501"));
			Assert.AreEqual(expected, ZIP.IsZip("985018569"));
		}

		/// <summary>
		///A test for IsZip for invalid zip code
		///</summary>
		[TestMethod()]
		public void IsZipForSingleInvalidZipAsInputTest()
		{
			string zip = "9850A-8569";
			bool expected = false;
			bool actual;
			actual = ZIP.IsZip(zip);
			Assert.AreEqual(expected, actual);
			Assert.AreEqual(expected, ZIP.IsZip("9850A"));
			Assert.AreEqual(expected, ZIP.IsZip("98501-856A"));
		}


		/// <summary>
		///A test for ZIP Constructor
		///</summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException))]
		public void ZIPConstructorForErrorTest()
		{
			int zip = 2147483647; 
			ZIP target = new ZIP(zip);
		}
	
		[TestMethod()]		
		public void ZIPConstructorTest()
		{
			int zip = 985108567; 
			ZIP target = new ZIP(zip);
			Assert.IsTrue(target.GetType() == typeof(ZIP));
			Assert.AreEqual("98510", target.PostalCodeBase);
			Assert.AreEqual(8567, target.PostalCodeExtension);
			Assert.IsTrue(target.HasPlus4);
			Assert.AreEqual(8567, target.Plus4);
			Assert.AreEqual(98510, target.Zip5);
		}

		[TestMethod()]
		public void ZIP_Regression1()
		{
			ZIP target = ZIP.Parse("12345", "    ");
			Assert.AreEqual("12345", target.PostalCodeBase);
			Assert.AreEqual(0, target.PostalCodeExtension);
			Assert.IsFalse(target.HasPlus4);
			Assert.AreEqual(0, target.Plus4);
			Assert.AreEqual(12345, target.Zip5);

			Assert.IsTrue(ZIP.IsPostalCode("12345", "    "));
		}
	}
}
